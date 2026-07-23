using ProjWizInc.Core.Data.ADT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Simulation.Persistence {
    public class BigNumJsonConverter : JsonConverter<BigNum> {
        public override BigNum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            // Try reading as string (scientific/large) or number (small)
            if (reader.TokenType == JsonTokenType.String)
                return BigNum.Parse(reader.GetString()!);

            if (reader.TokenType == JsonTokenType.Number)
                return new BigNum(reader.GetDouble());
            //not sure if we should throw an exception here or something 
            return new BigNum(0);
        }
        public override void Write(Utf8JsonWriter writer, BigNum value, JsonSerializerOptions options) {
            // Always write as string to maintain precision
            writer.WriteStringValue(value.ToScientific(false));
        }
    }
}
