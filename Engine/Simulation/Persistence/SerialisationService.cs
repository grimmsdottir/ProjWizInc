using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjWizInc.Engine.Simulation.Persistence {
    /*
     * this class serves a sort of helper or translator.
     * because we use polymorphism, a custom ADT and other future funky stuff, we want to clarify the rules here
     */
    public class SerialisationService {
        private readonly JsonSerializerOptions _options;
        public SerialisationService() {
            _options = new JsonSerializerOptions {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
            _options.Converters.Add(new BigNumJsonConverter());
        }
        //generic load for definitions, whatever that means
        public T? Load<T>(string filePath) {
            if (!File.Exists(filePath)) return default;
            string json = File.ReadAllText(filePath);
            return Deserialize<T>(json);
        }
        public string Serialize<T>(T obj) {
            return JsonSerializer.Serialize(obj, _options);
        }
        public T Deserialize<T>(string json) { 
            if (json == null) { throw new InvalidDataException("Attempted to deserialise null string"); }
            try {
                return JsonSerializer.Deserialize<T>(json, _options);
            } catch (JsonException ex) when (ex.HResult == unchecked((int)0x80131500)) {
                // Triggers for: missing braces, unclosed quotes, trailing commas, bad formatting
                throw new JsonException("The JSON structure itself is malformed and cannot be parsed.", ex);
            } catch (JsonException ex) {
                // Triggers for: unknown polymorphic $type tags, invalid data types (string instead of int)
                throw new InvalidDataException("The JSON is structurally valid, but contains invalid or unrecognized data.", ex);
            }
        }

    }
}
