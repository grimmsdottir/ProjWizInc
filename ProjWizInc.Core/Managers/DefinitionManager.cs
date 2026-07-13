using Microsoft.Win32;
using ProjWizInc.Core.Definitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Managers {
    
    public class GameDefinitions {
        public List<ResourceDefinition> Resources { get; private set; } = [];
        public List<JobDefinition> Jobs { get; private set; } = [];
    }
    public class DefinitionManager {

        private readonly Dictionary<string,int> _keyToId = [];
        private readonly Dictionary<int, ResourceDefinition> _idToDef = [];
        private const string RESOURCES_PATH = "resourceDefs.json";
        public List<ResourceDefinition> Registry { get; private set; }
        public DefinitionManager() { 
            Registry = [];
        }
        public void ReadFiles() {
            string jsonString = File.ReadAllText(RESOURCES_PATH);
            var data = JsonSerializer.Deserialize<GameDefinitions>(jsonString);
            for (int i = 0;i < data.Resources.Count; i++) {
                var def = data.Resources[i];
                _keyToId[def.Key]= i;
                _idToDef[i] = def;
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
