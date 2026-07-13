using Microsoft.Win32;
using ProjWizInc.Core.Definitions.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static ProjWizInc.Core.Definitions.GameDefinitions;

namespace ProjWizInc.Core.Definitions {
    
    internal class GameDefinitions {
        
        public List<ResourceDefinition> Resources { get; internal set; } = [];
        public List<JobDefinition> Jobs { get; internal set; } = [];
    }
    internal class DefinitionManager {
        //now we use Type for keys. this first dictionary is a dictionary of dictionaries
        //each type has its own dictionary, which maps strings to int
        private readonly Dictionary<Type, Dictionary<string, int>> _registry = [];
        //for each Type, it will have its own array, we use a generic Array, because we dont know the typing yet,
        //but it should basically be like <Type,Type[]>
        //the int ids from the first dictionary map to the position in the defStore, for high performance
        private readonly Dictionary<Type, Array> _defStore = [];


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

            
        }
        private void ResolveJobFeatures(JobDefinition job) {
            //foreach ()
        }
        public int GetIdFromKey(string key) {
            return -1;
        }
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

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(template, options);
            File.WriteAllText(dataPath, json);
        }
    }
}
