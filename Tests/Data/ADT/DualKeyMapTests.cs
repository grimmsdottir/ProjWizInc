using ProjWizInc.Engine.Data.ADT;

namespace ProjWizInc.Engine.Tests.Data.ADT {
    public class DualKeyMapTests {
        private class TestState {
            public int Value;
            public TestState() {
                Value = 0;
            }
            public TestState(int value) {
                Value = value;
            }
        }
        [Fact]
        public void Constructor_ShouldInitializeCorrectMappings() {
            // Arrange
            string[] keys = new string[3] { "gold", "wood", "iron" };

            // Act
            DualKeyMap<TestState> map = new DualKeyMap<TestState>(keys);

            // Assert
            Assert.Equal(3, map.Length);
            Assert.True(map.ContainsId(0));
            Assert.True(map.ContainsId(1));
            Assert.True(map.ContainsId(2));
            Assert.Equal("gold", map.GetKey(0));
            Assert.Equal("wood", map.GetKey(1));
            Assert.Equal(0, map.GetId("gold"));
            Assert.Equal(1, map.GetId("wood"));
        }
        [Fact]
        public void Constructor_DictionaryShouldWorkToo() {
            //Arrange
            Dictionary<string,int> testSaveData = new Dictionary<string,int>();
            testSaveData.Add("gold", 100);
            testSaveData.Add("wood", 200);
            testSaveData.Add("rion", 300);

            //Act
            DualKeyMap<int> testMap =  new DualKeyMap<int>(testSaveData);
            Assert.Equal(3, testMap.Length);
            Assert.True(testMap.ContainsId(0));
            Assert.True(testMap.ContainsId(1));
            Assert.True(testMap.ContainsId(2));
            Assert.Equal("gold", testMap.GetKey(0));
            Assert.Equal("wood", testMap.GetKey(1));
            Assert.Equal(0, testMap.GetId("gold"));
            Assert.Equal(1, testMap.GetId("wood"));
            Assert.Equal(100, testMap.GetValue(0));
            Assert.Equal(100, testMap.GetValue("gold"));
            Assert.Equal(200, testMap.GetValue(1));
            Assert.Equal(200, testMap.GetValue("wood"));
        }
        [Fact]
        public void Indexers_ShouldProvideSymmetricAccess() {
            // Arrange
            string[] keys = new string[2] { "gold", "wood" };
            DualKeyMap<TestState> map = new DualKeyMap<TestState>(keys);
            TestState goldState = new TestState(150);

            // Act - Set using string key, Get using integer ID
            map["gold"] = goldState;

            // Assert
            TestState retrievedById = map[0];
            TestState retrievedByKey = map["gold"];

            Assert.Equal(150, retrievedById.Value);
            Assert.Equal(150, retrievedByKey.Value);
            Assert.Same(goldState, retrievedById);
        }
        [Fact]
        public void CopyTo_ShouldSuccessfullyCopyWhenKeysMatch() {
            // Arrange
            string[] keys = new string[2] { "gold", "wood" };
            DualKeyMap<TestState> source = new DualKeyMap<TestState>(keys);
            DualKeyMap<TestState> target = new DualKeyMap<TestState>(keys);

            source[0] = new TestState(100);
            source[1] = new TestState(50);

            // Act
            source.CopyTo(target);

            // Assert
            Assert.Equal(100, target[0].Value);
            Assert.Equal(50, target[1].Value);
        }
        [Fact]
        public void CopyTo_ShouldThrowExceptionOnKeyMismatch() {
            // Arrange
            string[] sourceKeys = new string[2] { "gold", "wood" };
            string[] targetKeys = new string[2] { "gold", "stone" }; // Mismatched key at index 1

            DualKeyMap<TestState> source = new DualKeyMap<TestState>(sourceKeys);
            DualKeyMap<TestState> target = new DualKeyMap<TestState>(targetKeys);

            // Local helper function instead of lambda expression
            void ExecutionToTest() {
                source.CopyTo(target);
            }

            // Act & Assert
            Assert.Throws<InvalidOperationException>(ExecutionToTest);
        }

        [Fact]
        public void Hydrate_ShouldPopulateCorrectly_WhenAllKeysArePresent() {
            // Arrange
            string[] keys = new string[2] { "gold", "wood" };
            DualKeyMap<TestState> map = new DualKeyMap<TestState>(keys);

            Dictionary<string, TestState> saveGameData = new Dictionary<string, TestState>();
            saveGameData.Add("gold", new TestState(500));
            saveGameData.Add("wood", new TestState(250));

            // Act
            map.Hydrate(saveGameData, false);

            // Assert
            Assert.Equal(500, map["gold"].Value);
            Assert.Equal(250, map["wood"].Value);
        }

        [Fact]
        public void Hydrate_ShouldSucceedWithDefaults_WhenExpectingMissingIsUnderstood() {
            // Arrange
            string[] keys = new string[3] { "gold", "wood", "newFeature" }; // "newFeature" was added in patch
            DualKeyMap<TestState> map = new DualKeyMap<TestState>(keys);

            Dictionary<string, TestState> saveGameData = new Dictionary<string, TestState>();
            saveGameData.Add("gold", new TestState(500));
            saveGameData.Add("wood", new TestState(250));
            // "newFeature" is completely missing from this older save file

            // Act
            map.Hydrate(saveGameData, true); // expectingMissing = true

            // Assert
            Assert.Equal(500, map["gold"].Value);
            Assert.Equal(250, map["wood"].Value);
            Assert.NotNull(map["newFeature"]);
            Assert.Equal(0, map["newFeature"].Value); // Safely defaulted to new TValue()
        }

        [Fact]
        public void Hydrate_ShouldThrowException_WhenMissingKeysAreUnexpected() {
            // Arrange
            string[] keys = new string[3] { "gold", "wood", "newFeature" };
            DualKeyMap<TestState> map = new DualKeyMap<TestState>(keys);

            Dictionary<string, TestState> saveGameData = new Dictionary<string, TestState>();
            saveGameData.Add("gold", new TestState(500));
            saveGameData.Add("wood", new TestState(250));

            // Local helper function instead of lambda expression
            void ExecutionToTest() {
                map.Hydrate(saveGameData, false); // expectingMissing = false
            }

            // Act & Assert
            Assert.Throws<InvalidDataException>(ExecutionToTest);
        }

        [Fact]
        public void Dehydrate_ShouldGenerateCorrectKeyValueMap() {
            // Arrange
            string[] keys = new string[2] { "gold", "wood" };
            DualKeyMap<TestState> map = new DualKeyMap<TestState>(keys);
            map[0] = new TestState(75);
            map[1] = new TestState(45);

            // Act
            Dictionary<string, TestState> result = map.Dehydrate();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(75, result["gold"].Value);
            Assert.Equal(45, result["wood"].Value);
        }

        [Fact]
        public void Reset_ShouldReinitializeAllValues() {
            // Arrange
            string[] keys = new string[2] { "gold", "wood" };
            DualKeyMap<TestState> map = new DualKeyMap<TestState>(keys);
            map[0] = new TestState(100);
            map[1] = new TestState(50);

            // Act
            map.Reset();

            // Assert
            Assert.Equal(0, map[0].Value);
            Assert.Equal(0, map[1].Value);
        }

    }
}
