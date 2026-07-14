using ProjWizInc.Core.ADT;

namespace xUnitTester {
    public class BigNumTests {
        //reminder that our boundry is around 2e15
        private const int NUM_0 = 0;
        private const int NUM_1 = 1;
        private const int NUM_SMALL = 1000;
        private const double NUM_BIG = 1e25;
        /*
         * CONSTRUCTOR AND PATH TESTS
         */
        [Fact]
        public void Constructor_SmallWholeNumber_IsNotLarge() {
            BigNum val = new(NUM_SMALL);
            //if val isLarge, than the test fails, otherwise it passes
            Assert.False(val.IsLarge);
        }
        [Fact]
        public void Constructor_BigNumber_IsLarge() {
            BigNum val = new(NUM_BIG);
            //if val is not Large, than the test fails, otherwise it passes
            Assert.True(val.IsLarge);
        }
        [Theory]
        [InlineData(150, 0, 1.5, 2)]          // Over 10: 150e0 -> 1.5e2
        [InlineData(0.01, 5, 1.0, 3)]         // Under 1: 0.01e5 -> 1.0e3
        [InlineData(950, 10, 9.5, 12)]        // Standard normalization
        [InlineData(0.0001, -2, 1.0, -6)]     // Negative exponents
        public void Constructor_AlwaysNormalizesCorrectly(double mIn, long eIn, double mExp, long eExp) {
            // Act
            var val = new BigNum(mIn, eIn);

            // Assert
            // We check the internal components. 
            // (Note: You might need to make these internal or public for the test)
            Assert.Equal(eExp, val.Exp);
            Assert.Equal(mExp, val.Man, 5); // 5 decimal places of precision
        }
        [Theory]
        [InlineData(150, 0, 150, false)]      // Should downshift to long
        [InlineData(1.5, 2, 150, false)]      // Should downshift to long
        [InlineData(1.5, 20, 0, true)]        // Stays large
        [InlineData(0.01, 0, 0, true)]        // Decimals are always large (Scientific path)
        public void Constructor_Normalization_PathCheck(double mIn, long eIn, long expectedSmall, bool expectedIsLarge) {
            var val = new BigNum(mIn, eIn);

            Assert.Equal(expectedIsLarge, val.IsLarge);
            if (!expectedIsLarge) {
                Assert.Equal(expectedSmall, val.Small);
            }
        }
        [Fact]
        public void Constructor_ExtremeUnnormalized_NormalizesCorrectly() {
            // 1,000,000e0 should become 1e6
            var val = new BigNum(1_000_000, 0);

            Assert.Equal(1.0, val.Man, 10);
            Assert.Equal(6, val.Exp);
        }

        [Fact]
        public void Constructor_TinyDecimal_NormalizesCorrectly() {
            // 0.000001e0 should become 1e-6
            var val = new BigNum(0.000001, 0);

            Assert.Equal(1.0, val.Man, 10);
            Assert.Equal(-6, val.Exp);
        }
        [Fact]
        public void Addition_CrossesThreshold_Successfully() {
            // Arrange: Use a number just below long.MaxValue (approx 9e18)
            BigNum a = new BigNum(9_000_000_000_000_000_000L);
            BigNum b = new BigNum(2_000_000_000_000_000_000L);

            // Act
            BigNum result = a + b;

            // Assert
            Assert.True(result.IsLarge);
            // 9e18 + 2e18 = 1.10e19
            Assert.Equal("1.10e19", result.ToString());
        }
        [Fact]
        public void Addition_MassivePlusTiny_HandlesAbsorption() {
            BigNum massive = BigNum.Parse("1.00e25");
            BigNum tiny = new BigNum(1);

            BigNum result = massive + tiny;

            // In a double-mantissa system, 1.00e25 + 1 is still 1.00e25
            // This test ensures your normalization logic doesn't freak out.
            Assert.Equal("1.00e25", result.ToString());
        }
        [Theory]
        [InlineData("0.5", "0.5", "1")]      // Two halves make a whole
        [InlineData("0.1", "10", "1")]       // Multiplication test: 0.1 * 10 = 1
        public void Decimals_CalculateCorrectly(string valA, string valB, string expected) {
            BigNum a = BigNum.Parse(valA);
            BigNum b = BigNum.Parse(valB);

            // Test both addition and multiplication
            if (valB == "0.5") Assert.Equal(expected, (a + b).ToString());
            if (valB == "10") Assert.Equal(expected, (a * 10).ToString());
        }
        [Theory]
        [InlineData("1.00e10", "5.00e9", true)]  // Scientific vs Scientific (Different exponents)
        [InlineData("1000", "999", true)]        // Long vs Long
        [InlineData("1.00e3", "999", true)]      // Scientific vs Long
        [InlineData("500", "1.00e2", true)]      // Long vs Scientific
        public void Comparison_GreaterAndLess_Works(string big, string small, bool expected) {
            BigNum a = BigNum.Parse(big);
            BigNum b = BigNum.Parse(small);

            Assert.Equal(expected, a > b);
            Assert.Equal(!expected, a < b);
        }
        //check if we can read valid numbers correctly
        [Theory]
        [InlineData("100", 100)]
        [InlineData("12.5", 12.5)]
        [InlineData("1.0e10", 10000000000)]
        [InlineData("  500  ", 500)] // Testing the Trim()
        public void Parse_ValidatesAllFormats(string input, double expectedValue) {
            var result = BigNum.Parse(input);
            // You'll need an explicit/implicit cast to double for this assertion
            //Assert.Equal(expectedValue, (double)result);
        }
        //so basically we create a "theory", which we put in what we expect should happen given some data
        //so for example, this theory is for addition, we expect that a + b = c
        [Theory]
        [InlineData("10", "20", "30")]               // Simple long path
        [InlineData("1.00e10", "1.00e10", "2.00e10")] // Simple mantissa path
        [InlineData("9e18", "2e18", "1.10e19")]       // Crossing the threshold
        [InlineData("1.00e20", "1", "1.00e20")]       // Testing absorption/precision
        public void Addition_CalculatesCorrectly(string valA, string valB, string expected) {
            var a = BigNum.Parse(valA);
            var b = BigNum.Parse(valB);

        }
    }
}