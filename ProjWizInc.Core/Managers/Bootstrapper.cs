using ProjWizInc.Core.Definitions;
using ProjWizInc.Core.Definitions.Blueprints;
using ProjWizInc.Core.Definitions.Common;
using ProjWizInc.Core.Definitions.Components;
using ProjWizInc.Core.Persistence;
using ProjWizInc.Core.States;
using ProjWizInc.Core.States.Managers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Managers {
    /*
     * this class exists to create the ContextManager and all the other managers, so that we dont have
     * to clutter the context manager for all the init and whatnot, and just have it manage runtime stuff
     */
    public class Bootstrapper {
        public const string DEFINITIONS_FILEPATH = "defs.json";
        public static CoreContext BuildContext() {
            //read jsons
            SerialisationService serialiser = new SerialisationService();
            //TODO: for now we just make a new state each time, but later on we should try to load a save file
            GameState gameState = new GameState();
            //now we slice up the GameState into slices to pass to the managers
            EconomyState economyState = gameState.economyState;
            JobState jobState = gameState.jobState;
            TimeState timeState = gameState.timeState;
            //special managers that need to be initialised first
            EventManager eventManager = new EventManager();
            //the definition manager is so special it gets its own section lol
            GameDefinitions rawData = serialiser.Load<GameDefinitions>(DEFINITIONS_FILEPATH);
            DefinitionManager definitionManager = BuildDefinitionManager(rawData);
            ResolveDictionaryLinks(rawData, definitionManager);
            //construct basic managers
            EconomyManager economyManager = new EconomyManager(eventManager,economyState,definitionManager.GetDefCount<ResourceDefinition>());
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
        private static void ResolveDictionaryLinks(GameDefinitions rawData, DefinitionManager definitionManager) {
            foreach (var job in rawData.Jobs) {
                foreach (var comp in job.Components.OfType<ILinkableDefinitionInterface>()) {
                    comp.ResolveLinks(definitionManager);
                }
            }
        }
        //the definition manager is so chonky we split it out to here
        private static DefinitionManager BuildDefinitionManager(GameDefinitions rawData) {
            //first we read the json and get the GameDefinitions
            
            //then we create the dicts
            Dictionary<Type, Dictionary<string, int>> typeKeyIdMap = [];
            Dictionary<Type, Array> typeIdDefMap = [];
            //then we fill the dictionaries
            PopulateAllDictionaries(rawData, typeKeyIdMap, typeIdDefMap);
            //we arent done yet, because we havent resolved links, but its good enough
            return new DefinitionManager(typeKeyIdMap, typeIdDefMap);
        }
        //wizardly reflection something magic, idk
        private static void PopulateAllDictionaries(
            GameDefinitions data,
            Dictionary<Type, Dictionary<string, int>> typeKeyIdMap,
            Dictionary<Type, Array> typeIdDefMap
            ) {
            // We look at every property in GameDefinitions (Resources, Jobs, etc.)
            var properties = typeof(GameDefinitions).GetProperties();

            foreach (var prop in properties) {
                // Check if the property is a List<T> where T inherits from DefinitionBase
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>)) {
                    var listValue = prop.GetValue(data);
                    if (listValue == null) continue;

                    // Get the specific type (e.g., ResourceDefinition)
                    var itemType = prop.PropertyType.GetGenericArguments()[0];

                    // Use 'MakeGenericMethod' to call your PopulateRegistry<T> method dynamically
                    var method = typeof(Bootstrapper).GetMethod(nameof(PopulateDictionaries), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                    var genericMethod = method.MakeGenericMethod(itemType);

                    genericMethod.Invoke(null, new object[] { listValue, typeKeyIdMap, typeIdDefMap });
                }
            }
        }
        /* this hecking thing is the definition buildertron, which is so complex, imma write this down here so i remember
         * T - the type of definition, which will need to be hard coded, for each type of definition,
         * like JobDefinition and Resource definition
         * items - this will be the contents of GameDefinitions.SomeDefinition that we got from the json
         * typeKeyIdMap - the name should be self evident, but its still a mindbender. so the key of the outer layer
         * dict is the dynamic subtype of the definition, so gold, wood etc in ResourceDefinitions, the the value is
         * another hecking dict, which maps strings to in, so the "gold" in .json gets translated to a number
         * typeIdDef - like above the outer layer is the same, but the inner layer is an array of actual definitions
         * so like the gold definition with all its components and name etc
         * we go through this song and dance so that when somebody wants a definition, they just give a number and 
         * we can give it them just like that
         * for the front end and for linking, they dont know the number of the thing, they only have the string of the 
         * thing, so thats what the first dictionary is for, which they use to peruse the second dictionary
         * also because of glorious pass by reference, we only need to init the outer dictionaries once and pass it
         */
        private static void PopulateDictionaries<T>(
            List<T> items,
            Dictionary<Type, Dictionary<string, int>> typeKeyIdMap,
            Dictionary<Type, Array> typeIdDefMap
            ) where T : DefinitionBase {
            var idDefMap = new T[items.Count];
            var keyIdMap = new Dictionary<string, int>();
            //so first we prepare the inner dict/array
            for (int i = 0; i < items.Count; i++) {
                items[i].Id = i;
                keyIdMap[items[i].Key] = i;
                idDefMap[i] = items[i];
            }
            //then we stock the outer dict
            typeKeyIdMap[typeof(T)] = keyIdMap;
            typeIdDefMap[typeof(T)] = idDefMap;
        }
        
    }
}
