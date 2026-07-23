using ProjWizInc.Core.Data.ADT;
using ProjWizInc.Core.Data.Blueprints.Specifications;
using ProjWizInc.Core.Data.Blueprints.Specifications.Interfaces;
using ProjWizInc.Core.Simulation.Persistence;
using System.Text.Json;

namespace xUnitTester.Systems.Persistence {
    public class SerialisationServiceTests {
        [Fact]
        public void BigNumJsonConverter_CanDeserialize_ScientificString() {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new BigNumJsonConverter());

            string jsonString = "\"1.50e2\"";
            BigNum result = JsonSerializer.Deserialize<BigNum>(jsonString, options);

            // 1.50e2 is 150, which should fall within the small path
            Assert.False(result.IsLarge);
            Assert.Equal(150.0, result.Small, 5);
        }
        [Fact]
        public void BigNumJsonConverter_CanDeserialize_LargeScientificString() {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new BigNumJsonConverter());

            string jsonString = "\"1.00e20\"";
            BigNum result = JsonSerializer.Deserialize<BigNum>(jsonString, options);

            // 1.00e20 exceeds the 1e15 threshold, so it should use the large path
            Assert.True(result.IsLarge);
            Assert.Equal(20, result.Exp);
            Assert.Equal(1.0, result.Man, 5);
        }
        [Fact]
        public void BigNumJsonConverter_CanDeserialize_Number() {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new BigNumJsonConverter());

            string jsonString = "123.4";
            BigNum result = JsonSerializer.Deserialize<BigNum>(jsonString, options);

            Assert.False(result.IsLarge);
            Assert.Equal(123.4, result.Small, 5);
        }
        [Fact]
        public void DynamicComponents_CanPolymorphicallyDeserialize() {
            // Setup serialization options with the custom BigNum converter registered
            JsonSerializerOptions options = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new BigNumJsonConverter());

            // Attempting to parse a polymorphic list of components
            string jsonString = "[" +
                                "  { \"$type\": \"ticks\", \"RequiredTicks\": \"1.00e2\" }," +
                                "  { \"$type\": \"payout\", \"PayoutEntries\": [] }" +
                                "]";

            ISpecification[] components = JsonSerializer.Deserialize<ISpecification[]>(jsonString, options);

            Assert.NotNull(components);
            Assert.Equal(2, components.Length);

            Assert.True(components[0] is RequiresTicksSpec);
            RequiresTicksSpec ticksComponent = (RequiresTicksSpec)components[0];
            Assert.Equal(new BigNum(100), ticksComponent.RequiredTicks);

            Assert.True(components[1] is PayoutSpec);
        }
    }
}
