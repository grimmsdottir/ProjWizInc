using ProjWizInc.Core.ADT;

namespace xUnitTester.ADT {
    public class DualKeyMapTests {
        private const string GOLD = "gold";
        private const string WOOD = "wood";
        private struct TestStruct {
            public int value { get; set; }
            public TestStruct(int val) {
                value = val;
            }
        }
        private DualKeyMap<TestStruct> _testMap;
        private Dictionary<string, TestStruct> _testData;
        private bool _testExpectingMissing;
        private void ExecuteHydration() {
            _testMap.Hydrate(_testData, _testExpectingMissing);
        }
        private Dictionary<string, TestStruct> ExecuteDeyhdration() {
            return _testMap.Dehydrate();
        }
        [Theory]
        [InlineData(false)] // Scenario 1: Sequential IDs -> Successful Dehydration
        [InlineData(true)]  // Scenario 2: Unmapped ID Gap -> Fail-Fast Exception
        public void TestDehydrate(bool introduceIdGap) {
            Dictionary<string, int> testKeyIdMap = new Dictionary<string, int>() {
                { GOLD, 0 }
            };
            if (introduceIdGap) {
                //we add a gap
                testKeyIdMap.Add(WOOD, 2);
            } else {
                testKeyIdMap.Add(WOOD, 1);
            }
            _testMap = new DualKeyMap<TestStruct>(testKeyIdMap);
            TestStruct goldStruct  = new TestStruct(500);
            _testMap[0] = goldStruct;
            if (introduceIdGap) {
                TestStruct woodStruct = new TestStruct(200);
                _testMap[2] = woodStruct;
            } else {
                TestStruct woodStruct = new TestStruct(200);
                _testMap[1] = woodStruct;
            }
            if (introduceIdGap) {
                Assert.Throws<InvalidOperationException>(ExecuteDeyhdration);
            } else {
                Dictionary<string,TestStruct> saveData = _testMap.Dehydrate();  
                Assert.NotNull(saveData);
                Assert.Equal(2,saveData.Count);

                Assert.Equal(500, saveData[GOLD].value);
                Assert.Equal(200, saveData[WOOD].value);
            }
        }
        [Theory]
        [InlineData(true, true, false)]   // Scenario 1: Data is null, expecting missing -> Success (Defaulted)
        [InlineData(true, false, true)]   // Scenario 2: Data is null, NOT expecting missing -> Fail-Fast Exception
        [InlineData(false, true, false)]  // Scenario 3: Data is incomplete, expecting missing -> Success (Missing Defaulted)
        [InlineData(false, false, true)]  // Scenario 4: Data is incomplete, NOT expecting missing -> Fail-Fast Exception
        public void TestHydrate(bool isKeyValMapNull, bool expectingMissing, bool expectException) {
            Dictionary<string, int> testKeyIdMap = new Dictionary<string, int>() {
                { "gold", 0 },
                { "wood", 1 }
            };
            _testMap = new DualKeyMap<TestStruct>(testKeyIdMap);

            _testData = null;
            if (!isKeyValMapNull) {
                _testData = new Dictionary<string, TestStruct>();
                TestStruct testGoldStruct = new TestStruct(100);
                _testData.Add("gold", testGoldStruct);
            }
            if (expectException) {
                //verifies that the exception is thrown when it should
                Assert.Throws<KeyNotFoundException>(ExecuteHydration);
            } else {
                _testMap.Hydrate(_testData, expectingMissing);
                if (isKeyValMapNull) {
                    //if the map was null but we expected missing, then the values should be defaulted
                    Assert.Equal(0, _testMap[0].value);
                    Assert.Equal(0, _testMap[1].value);
                } else {
                    //if the save was incomplete but we expected missing, then the missing values should be defaulted
                    //"gold" should be hydrated with the value from the save
                    //"wood" should be defaulted since it was missing from the save
                    Assert.Equal(100, _testMap[0].value);
                    Assert.Equal(0, _testMap[1].value);
                }
            }
        }
        [Theory]
        [InlineData(false)] // Scenario 1: Resetting a sequential map of values
        [InlineData(true)]  // Scenario 2: Resetting a map containing unmapped ID gaps
        public void TestReset(bool introduceIdGap) {
            // 1. Arrange: Setup our definition mappings
            Dictionary<string, int> testKeyIdMap = new Dictionary<string, int>() {
                { GOLD, 0 }
            };

            if (introduceIdGap) {
                testKeyIdMap.Add(WOOD, 2); // Creates an unmapped gap at ID 1
            } else {
                testKeyIdMap.Add(WOOD, 1);
            }

            _testMap = new DualKeyMap<TestStruct>(testKeyIdMap);

            // 2. Arrange: Populate active slot values with non-default progress data
            _testMap[0] = new TestStruct(100);

            if (introduceIdGap) {
                _testMap[2] = new TestStruct(200);
            } else {
                _testMap[1] = new TestStruct(200);
            }

            // 3. Act: Verify initial modification, then execute the Reset operation
            Assert.Equal(100, _testMap[0].value);

            if (introduceIdGap) {
                Assert.Equal(200, _testMap[2].value);
                Assert.Equal(0, _testMap[1].value); // Unmapped ID 1 remains initialized to 0
            } else {
                Assert.Equal(200, _testMap[1].value);
            }

            // Perform the wipe
            _testMap.Reset();

            // 4. Assert: Verify all internal array slots have been reset back to 0 (default struct state)
            for (int i = 0; i < _testMap.Length; i = i + 1) {
                Assert.Equal(0, _testMap[i].value);
            }

            // Assert that the structural mappings, length, and keys are preserved
            int expectedLength = 2;
            if (introduceIdGap) {
                expectedLength = 3;
            }

            Assert.Equal(expectedLength, _testMap.Length);
            Assert.True(_testMap.ContainsKey(GOLD));
            Assert.True(_testMap.ContainsKey(WOOD));
        }
        [Theory]
        [InlineData(0, true, 500)]   // Scenario 1: Valid lower boundary ID -> Returns true, value 500
        [InlineData(1, true, 200)]   // Scenario 2: Valid upper boundary ID -> Returns true, value 200
        [InlineData(2, false, 0)]    // Scenario 3: Out-of-bounds upper ID -> Returns false, default struct (0)
        [InlineData(-5, false, 0)]   // Scenario 4: Out-of-bounds negative ID -> Returns false, default struct (0)
        public void TestTryGetValueById(int id, bool expectSuccess, int expectedValue) {
            // 1. Arrange: Setup mappings and active progress values
            Dictionary<string, int> testKeyIdMap = new Dictionary<string, int>() {
                { GOLD, 0 },
                { WOOD, 1 }
            };
            _testMap = new DualKeyMap<TestStruct>(testKeyIdMap);

            _testMap[0] = new TestStruct(500);
            _testMap[1] = new TestStruct(200);

            // 2. Act: Attempt the ID-based safe query
            TestStruct outputValue;
            bool success = _testMap.TryGetValue(id, out outputValue);

            // 3. Assert: Verify the returned status and matched structural value
            Assert.Equal(expectSuccess, success);
            Assert.Equal(expectedValue, outputValue.value);
        }

        [Theory]
        [InlineData("gold", true, 500)]   // Scenario 1: Valid key -> Returns true, value 500
        [InlineData("wood", true, 200)]   // Scenario 2: Valid key -> Returns true, value 200
        [InlineData("stone", false, 0)]   // Scenario 3: Non-existent key -> Returns false, default struct (0)
        [InlineData(null, false, 0)]      // Scenario 4: Null key -> Returns false, default struct (0)
        public void TestTryGetValueByKey(string key, bool expectSuccess, int expectedValue) {
            // 1. Arrange: Setup mappings and active progress values
            Dictionary<string, int> testKeyIdMap = new Dictionary<string, int>() {
                { "gold", 0 },
                { "wood", 1 }
            };
            _testMap = new DualKeyMap<TestStruct>(testKeyIdMap);

            _testMap[0] = new TestStruct(500);
            _testMap[1] = new TestStruct(200);

            // 2. Act: Attempt the string-keyed safe query
            TestStruct outputValue;
            bool success = _testMap.TryGetValue(key, out outputValue);

            // 3. Assert: Verify the returned status and matched structural value
            Assert.Equal(expectSuccess, success);
            Assert.Equal(expectedValue, outputValue.value);
        }
        private string _missingKey;

        // Named helper methods to act as delegates for our exception assertions,
        // completely bypassing the need for lambda expressions.
        private void ExecuteInvalidGet() {
            _testMap.GetValue(_missingKey);
        }

        private void ExecuteInvalidSet() {
            _testMap.SetValue(_missingKey, new TestStruct(50));
        }

        [Theory]
        [InlineData(0, "gold", 999)] // Scenario 1: Accessing the lower boundary slot (gold/0)
        [InlineData(1, "wood", 123)] // Scenario 2: Accessing the upper boundary slot (wood/1)
        public void TestGettersAndSettersValidPaths(int id, string key, int newValue) {
            // 1. Arrange: Setup mappings and active progress values
            Dictionary<string, int> testKeyIdMap = new Dictionary<string, int>() {
                { "gold", 0 },
                { "wood", 1 }
            };
            _testMap = new DualKeyMap<TestStruct>(testKeyIdMap);

            // 2. Act & Assert: Test Writing via ID Indexer, Reading via String Key
            TestStruct stateA = new TestStruct(newValue);
            _testMap[id] = stateA;

            TestStruct readA = _testMap.GetValue(key);
            Assert.Equal(newValue, readA.value);

            // 3. Act & Assert: Test Writing via String Key, Reading via ID Indexer
            int alternateValue = newValue + 10;
            TestStruct stateB = new TestStruct(alternateValue);
            _testMap.SetValue(key, stateB);

            TestStruct readB = _testMap[id];
            Assert.Equal(alternateValue, readB.value);

            // 4. Act & Assert: Test Writing via SetValue(int), Reading via GetValue(int)
            int thirdValue = newValue + 20;
            TestStruct stateC = new TestStruct(thirdValue);
            _testMap.SetValue(id, stateC);

            TestStruct readC = _testMap.GetValue(id);
            Assert.Equal(thirdValue, readC.value);
        }

        [Theory]
        [InlineData("stone")] // Scenario 1: Querying a non-existent string key
        [InlineData(null)]    // Scenario 2: Querying a null key (safe guard test)
        public void TestInvalidStringAccessThrowsException(string invalidKey) {
            // 1. Arrange: Setup mappings
            Dictionary<string, int> testKeyIdMap = new Dictionary<string, int>() {
                { "gold", 0 },
                { "wood", 1 }
            };
            _testMap = new DualKeyMap<TestStruct>(testKeyIdMap);
            _missingKey = invalidKey;

            // 2. Act & Assert: Verify that both Get and Set throw KeyNotFoundException
            Assert.Throws<KeyNotFoundException>(ExecuteInvalidGet);
            Assert.Throws<KeyNotFoundException>(ExecuteInvalidSet);
        }
        private int _invalidId;

        // Named helper methods to act as delegates for our exception assertions,
        // completely bypassing the need for lambda expressions.
        private void ExecuteInvalidGetById() {
            _testMap.GetValue(_invalidId);
        }

        private void ExecuteInvalidSetById() {
            _testMap.SetValue(_invalidId, new TestStruct(100));
        }

        [Theory]
        [InlineData(0, 999)] // Scenario 1: Accessing the lower boundary slot (gold/0)
        [InlineData(1, 123)] // Scenario 2: Accessing the upper boundary slot (wood/1)
        public void TestIntegerAccessorsValidPaths(int id, int newValue) {
            // 1. Arrange: Setup mappings
            Dictionary<string, int> testKeyIdMap = new Dictionary<string, int>() {
                { "gold", 0 },
                { "wood", 1 }
            };
            _testMap = new DualKeyMap<TestStruct>(testKeyIdMap);

            // 2. Act & Assert: Test writing via indexer, reading via GetValue(int)
            TestStruct stateA = new TestStruct(newValue);
            _testMap[id] = stateA;

            TestStruct readA = _testMap.GetValue(id);
            Assert.Equal(newValue, readA.value);

            // 3. Act & Assert: Test writing via SetValue(int), reading via indexer
            int alternateValue = newValue + 10;
            TestStruct stateB = new TestStruct(alternateValue);
            _testMap.SetValue(id, stateB);

            TestStruct readB = _testMap[id];
            Assert.Equal(alternateValue, readB.value);
        }

        [Theory]
        [InlineData(-1)] // Scenario 1: Querying a negative out-of-bounds ID
        [InlineData(2)]  // Scenario 2: Querying an upper out-of-bounds ID (Length is 2)
        [InlineData(10)] // Scenario 3: Querying an extreme upper out-of-bounds ID
        public void TestIntegerAccessorsInvalidBoundsThrowsException(int invalidId) {
            // 1. Arrange: Setup mappings
            Dictionary<string, int> testKeyIdMap = new Dictionary<string, int>() {
                { "gold", 0 },
                { "wood", 1 }
            };
            _testMap = new DualKeyMap<TestStruct>(testKeyIdMap);
            _invalidId = invalidId;

            // 2. Act & Assert: Verify that both Get and Set throw IndexOutOfRangeException
            Assert.Throws<IndexOutOfRangeException>(ExecuteInvalidGetById);
            Assert.Throws<IndexOutOfRangeException>(ExecuteInvalidSetById);
        }

        private Dictionary<string, int> _nullMap;
        private Dictionary<string, int> _invalidMap;

        // Named helper methods to act as delegates for our constructor exception assertions,
        // completely bypassing the need for lambda expressions.
        private void ExecuteConstructorWithNull() {
            new DualKeyMap<TestStruct>(_nullMap);
        }

        private void ExecuteConstructorWithInvalidMap() {
            new DualKeyMap<TestStruct>(_invalidMap);
        }

        [Fact]
        public void TestConstructorThrowsArgumentNullException() {
            // 1. Arrange: Prepare a null mapping
            _nullMap = null;

            // 2. Act & Assert: Verify that the constructor fails-fast immediately
            Assert.Throws<ArgumentNullException>(ExecuteConstructorWithNull);
        }

        [Theory]
        [InlineData(-1)]   // Scenario 1: Sentinel uninitialized ID
        [InlineData(-5)]   // Scenario 2: Arbitrary negative ID
        [InlineData(-100)] // Scenario 3: Extreme negative ID
        public void TestConstructorThrowsArgumentOutOfRangeException(int negativeId) {
            // 1. Arrange: Create a configuration mapping containing an invalid negative ID
            _invalidMap = new Dictionary<string, int>();
            _invalidMap.Add("gold", 0);
            _invalidMap.Add("wood", negativeId);

            // 2. Act & Assert: Verify that the constructor detects this on the cold path and fails-fast
            Assert.Throws<ArgumentOutOfRangeException>(ExecuteConstructorWithInvalidMap);
        }
        [Theory]
        [InlineData("gold", true)]   // Scenario 1: Querying a valid registered key
        [InlineData("wood", true)]   // Scenario 2: Querying another valid registered key
        [InlineData("stone", false)] // Scenario 3: Querying a non-existent key
        public void TestContainsKey(string key, bool expectedResult) {
            // 1. Arrange: Setup our mapping dictionary and map instance
            Dictionary<string, int> testKeyIdMap = new Dictionary<string, int>() {
        { "gold", 0 },
        { "wood", 1 }
    };
            _testMap = new DualKeyMap<TestStruct>(testKeyIdMap);

            // 2. Act: Perform the key contains check
            bool result = _testMap.ContainsKey(key);

            // 3. Assert: Verify the returned status matches expectations
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(0, true)]   // Scenario 1: Lower bound valid ID
        [InlineData(1, true)]   // Scenario 2: Upper bound valid ID (Length is 2)
        [InlineData(-1, false)] // Scenario 3: Out-of-bounds negative ID
        [InlineData(2, false)]  // Scenario 4: Out-of-bounds upper ID
        public void TestContainsId(int id, bool expectedResult) {
            // 1. Arrange: Setup our mapping dictionary and map instance
            Dictionary<string, int> testKeyIdMap = new Dictionary<string, int>() {
        { "gold", 0 },
        { "wood", 1 }
    };
            _testMap = new DualKeyMap<TestStruct>(testKeyIdMap);

            // 2. Act: Perform the ID contains check
            bool result = _testMap.ContainsId(id);

            // 3. Assert: Verify the returned status matches expectations
            Assert.Equal(expectedResult, result);
        }
    }
}
