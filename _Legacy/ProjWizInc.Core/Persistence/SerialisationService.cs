using ProjWizInc.Core.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Persistence {
    /*
     * this class serves a sort of helper or translator.
     * because we use polymorphism, a custom ADT and other future funky stuff, we want to clarify the rules here
     */
    public class SerialisationService {
        private readonly JsonSerializerOptions _options;
        public SerialisationService() {
            _options = new JsonSerializerOptions {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                //TODO hook up bignum converter
            };
            _options.Converters.Add(new BigNumJsonConverter());
        }
        //generic load for definitions, whatever that means
        public T? Load<T>(string filePath) {
            if (!File.Exists(filePath)) return default;
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(json, _options);
        }
        /*
        // Save the entire GameState (Memory)
        public void SaveState(string filePath, GameState state) {
            string json = JsonSerializer.Serialize(state, _options);
            File.WriteAllText(filePath, json);
        }

        // Load the entire GameState (Memory)
        public GameState? LoadState(string filePath) {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<GameState>(json, _options);
        }
        */
    }
}
