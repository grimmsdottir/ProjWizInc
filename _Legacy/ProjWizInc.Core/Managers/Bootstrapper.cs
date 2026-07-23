using ProjWizInc.Core.ADT;
using ProjWizInc.Core.Definitions;
using ProjWizInc.Core.Definitions.Blueprints;
using ProjWizInc.Core.Definitions.Common;
using ProjWizInc.Core.Definitions.Components;
using ProjWizInc.Core.Persistence;
using ProjWizInc.Core.States;
using ProjWizInc.Core.States.Managers;

using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjWizInc.Core.Managers {
    /*
     * this class exists to create the ContextManager and all the other managers, so that we dont have
     * to clutter the context manager for all the init and whatnot, and just have it manage runtime stuff
     */
    public class Bootstrapper {
        public const string DEFS_DIRECTORY = "defs";
        //we split out BuildContext into 2, one for "proper" booting from file etc, and another which
        //can boot using specially provided defs and save for testing purposes
        public static CoreContext BuildContext() {
            SerialisationService serialiser = new SerialisationService();
            GameState gameState = new GameState();
            GameDefinitions masterDefinitions = new GameDefinitions();
            //first we scan and look for the defs directory
            if (Directory.Exists(DEFS_DIRECTORY)) {
                //we grab all the .json files in the defs directory
                string[] jsonFiles = Directory.GetFiles(DEFS_DIRECTORY, "*.json", SearchOption.AllDirectories);
                // Cache GameDefinitions properties once before entering the loop
                PropertyInfo[] properties = typeof(GameDefinitions).GetProperties();
                //we process each json
                for (int i = 0; i < jsonFiles.Length; i++) {
                    string filePath = jsonFiles[i];
                    //we load each json as a partial definition, which is an incomplete GameDefinition
                    GameDefinitions partialDefinitions = serialiser.Load<GameDefinitions>(filePath);
                    //reflection shinegigans to create the master definition from partial ones
                    //basically the compiler doesnt know how each GameDefinition.whatever is structured like
                    if (partialDefinitions != null) {
                        // Dynamically merge lists for any List<T> properties found
                        for (int p = 0; p < properties.Length; p++) {
                            PropertyInfo prop = properties[p];

                            // Verify if the property represents a generic list
                            if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>)) {
                                System.Collections.IList partialList = (System.Collections.IList)prop.GetValue(partialDefinitions);
                                System.Collections.IList masterList = (System.Collections.IList)prop.GetValue(masterDefinitions);

                                if (partialList != null && masterList != null) {
                                    for (int itemIndex = 0; itemIndex < partialList.Count; itemIndex++) {
                                        masterList.Add(partialList[itemIndex]);
                                    }
                                }
                            }
                        }
                    }
                }
            } else {
                Directory.CreateDirectory(DEFS_DIRECTORY);
            }

            // 1. Build DefinitionManager first to serve as the ID and count authority
            DefinitionManager definitionManager = BuildDefinitionManager(masterDefinitions);

            // 2. HYDRATE AND ALLOCATE SAVE STATE ARRAYS BEFORE CONTEXT AND MANAGER CONSTRUCTION
            // Refactored to cleanly support the state DualKeyMap transition
            if (gameState.economyState.Resources == null) {
                // If you have completed the transition of EconomyState.Resources to DualKeyMap<BigNum>:
                DualKeyMap<ResourceDefinition> resourceDefMap = definitionManager.GetMap<ResourceDefinition>();
                gameState.economyState.Resources = new DualKeyMap<BigNum>(resourceDefMap.KeyIdMap);
                gameState.economyState.Resources.Reset(); // Struct values automatically initialize to BigNum(0)

                /* 
                 * NOTE: If you have NOT yet finished refactoring EconomyState.Resources to DualKeyMap<BigNum> 
                 * and are still using a raw BigNum[] array, you can revert back to this legacy block:
                 *
                 * int resourceCount = definitionManager.GetDefCount<ResourceDefinition>();
                 * gameState.economyState.Resources = new BigNum[resourceCount];
                 * for (int i = 0; i < resourceCount; i++) {
                 *     gameState.economyState.Resources[i] = new BigNum(0);
                 * }
                 */
            }

            // 3. Pass the fully initialized, non-null gameState to compile the service manager graph
            CoreContext context = BuildContext(masterDefinitions, gameState);

            return context;
        }

        public static CoreContext BuildContext(GameDefinitions rawData, GameState gameState) {
            //now we slice up the GameState into slices to pass to the managers
            EconomyState economyState = gameState.economyState;
            JobState jobState = gameState.jobState;
            TimeState timeState = gameState.timeState;
            //special managers that need to be initialised first
            EventManager eventManager = new EventManager();
            //the definition manager is so special it gets its own section lol
            DefinitionManager definitionManager = BuildDefinitionManager(rawData);
            ResolveDictionaryLinks(definitionManager);
            //construct basic managers
            EconomyManager economyManager = new EconomyManager(eventManager, economyState);
            GameLoopManager gameLoopManager = new GameLoopManager(eventManager);
            JobManager jobManager = new JobManager(eventManager, jobState, definitionManager);
            TimeManager timeManager = new TimeManager(eventManager, timeState);
            return new CoreContext(
                definitionManager,
                eventManager,
                economyManager,
                gameLoopManager,
                jobManager,
                timeManager
                );
        }
        private static void ResolveDictionaryLinks(DefinitionManager definitionManager) {
            // Zero-allocation retrieval of the registry maps
            Dictionary<Type, IDualKeyMap>.ValueCollection allMaps = definitionManager.GetAllMaps();

            foreach (IDualKeyMap map in allMaps) {
                // Retrieve the contiguous array for polymorphic scanning
                Array idDefMaps = map.RawArray;

                foreach (object item in idDefMaps) {
                    DefinitionEntity entity = item as DefinitionEntity;
                    if (entity != null) {
                        List<IDefinitionComponentInterface> components = entity.Components;
                        for (int i = 0; i < components.Count; i++) {
                            IDefinitionComponentInterface component = components[i];
                            ILinkableDefinitionInterface linkable = component as ILinkableDefinitionInterface;
                            if (linkable != null) {
                                linkable.ResolveLinks(definitionManager);
                            }
                        }
                    }
                }
            }
        }


        // Consolidated and inlined reflection mapping pipeline
        private static DefinitionManager BuildDefinitionManager(GameDefinitions rawData) {
            Dictionary<Type, IDualKeyMap> typeMaps = new Dictionary<Type, IDualKeyMap>();
            PropertyInfo[] properties = typeof(GameDefinitions).GetProperties();

            for (int i = 0; i < properties.Length; i++) {
                PropertyInfo prop = properties[i];

                // Verify if the property represents a generic list mapping to a blueprint definition
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>)) {
                    object listValue = prop.GetValue(rawData);

                    if (listValue == null) {
                        continue;
                    }

                    // Extract the specific generic argument (e.g., ResourceDefinition)
                    System.Type itemType = prop.PropertyType.GetGenericArguments()[0];

                    // Dynamically resolve and invoke the open generic PopulateDictionaries helper
                    MethodInfo method = typeof(Bootstrapper).GetMethod(nameof(PopulateDictionaries), BindingFlags.Static | BindingFlags.NonPublic);
                    MethodInfo genericMethod = method.MakeGenericMethod(itemType);

                    genericMethod.Invoke(null, new object[] { listValue, typeMaps });
                }
            }

            return new DefinitionManager(typeMaps);
        }
        /* 
         * Constructs and populates strongly-typed DualKeyMaps from loaded configuration files.
         * The 'where T : DefinitionBase, new()' constraint transitively supports 
         * the parameterless constructor requirement of DualKeyMap.
         * It is neccesary because of reflection wizardy
         */
        private static void PopulateDictionaries<T>(
            List<T> items,
            Dictionary<Type, IDualKeyMap> typeMaps
        ) where T : DefinitionBase, new() {
            Dictionary<string, int> keyIdMap = new Dictionary<string, int>();

            // Phase 1: Establish sequential integer IDs and populate the forward mapping registry
            for (int i = 0; i < items.Count; i++) {
                items[i].Id = i;
                keyIdMap[items[i].Key] = i;
            }

            // Phase 2: Allocate the contiguous, flat runtime map
            DualKeyMap<T> map = new DualKeyMap<T>(keyIdMap);

            // Phase 3: Insert the definition instances into their mapped array positions
            for (int i = 0; i < items.Count; i++) {
                map.SetValue(i, items[i]);
            }

            // Phase 4: Register the completed map under its concrete Type key
            typeMaps[typeof(T)] = map;
        }


    }
}
