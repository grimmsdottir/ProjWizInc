using Microsoft.Win32;
using ProjWizInc.Core.Definitions;
using ProjWizInc.Core.Definitions.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static ProjWizInc.Core.Managers.GameDefinitions;

namespace ProjWizInc.Core.Managers {
    
    public class GameDefinitions {
        
        public List<ResourceDefinition> Resources { get; private set; } = [];
        public List<JobDefinition> Jobs { get; private set; } = [];
    }
    public class DefinitionBase {
        public string Key { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int Id { get; protected set; } = -1;
    }
    public class ResourceDefinition : DefinitionBase {
        public ResourceDefinition() {
        }
    }
    public class JobDefinition : DefinitionBase {
        //holds our components
        public List<FeatureInterface> Features { get; set; } = [];
        // Helper method to quickly find a specific component
        public T GetComponent<T>() where T : FeatureInterface {
            return Features.OfType<T>().FirstOrDefault();
        }
    }
    public class DefinitionManager {
        //The registry, which contains the defs with int keys(an array)
        //they are readonly, because they are generated during booting up, and should never need to be changed
        //unless we want to make funky custom build-a-job things
        private readonly ResourceDefinition[] _resourceDefs;
        private readonly JobDefinition[] _jobDefs;
        //translation lookups, only used once during booting, should clear/null them after we done
        private readonly Dictionary<string, int> _resourceKeys = [];
        private readonly Dictionary<string, int> _jobKeys = [];


        private const string RESOURCES_PATH = "resourceDefs.json";
        public List<ResourceDefinition> Registry { get; private set; }
        public DefinitionManager() { 
            Registry = [];
        }
        public void Init() {
            //read json as string
            string jsonString = File.ReadAllText(RESOURCES_PATH);
            //convert jsonString into a giant GameDefinitions object
            var data = JsonSerializer.Deserialize<GameDefinitions>(jsonString);
            //first we map our resources
            //for each resource, we map the string key (wood) to a ResourceDef object and an integer
            //we use the integer for accessing the definition because string keys are way slower than int keys
            for (int i = 0; i < data.Resources.Count; i++) { 
                var def = data.Resources[i];
                def.Id = i;
            }
            //now we map our jobs, which will be a bit more complex, due to components/features
            for (int i = 0;i < data.Jobs.Count; i++) {

            }
        }
        public int GetId(string key) => _keyToId[key];
        public ResourceDefinition GetDef(int id) => _idToDef[id];
        private string GetConfigPath(string fileName) {
            //for when we are done done then we place this next to the .exe
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDir, "Data", fileName);
        }
        public void GenerateTemplate() {
            string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
            string dataPath = Path.Combine(projectRoot, "Data", "defs.json");
            //string path = GetConfigPath(RESOURCES_PATH);
            if (!File.Exists(dataPath)) {
                Directory.CreateDirectory(Path.GetDirectoryName(dataPath));
            }
            var template = new GameDefinitions();
            template.Resources.Add(new ResourceDefinition("stone","Stone"));
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(template, options);
            File.WriteAllText(dataPath, json);
        }
    }
}
