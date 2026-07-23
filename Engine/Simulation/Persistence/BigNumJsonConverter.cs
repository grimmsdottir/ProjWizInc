using System.Text.Json;
using System.Text.Json.Serialization;
using ProjWizInc.Engine.Data.ADT;

namespace ProjWizInc.Engine.Simulation.Persistence {
    public class BigNumJsonConverter : JsonConverter<BigNum> {
        public override BigNum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            // Try reading as string (scientific/large) or number (small)
            if (reader.TokenType == JsonTokenType.String)
                return BigNum.Parse(reader.GetString()!);

            if (reader.TokenType == JsonTokenType.Number)
                return new BigNum(reader.GetDouble());
            throw new JsonException("Malformed BigNum data found. Attempted to parse non-number thing");
        }
        public override void Write(Utf8JsonWriter writer, BigNum value, JsonSerializerOptions options) {
            // Always write as string to maintain precision
            writer.WriteStringValue(value.ToScientific(false));
        }
    }
}
