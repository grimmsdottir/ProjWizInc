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
        [InlineData(150, 0, 150.0, false)]     // Integer -> Small Path
        [InlineData(1.5, 2, 150.0, false)]     // Scientific (Small) -> Small Path
        [InlineData(1.5, 20, 0.0, true)]       // Scientific (Huge) -> Large Path
        [InlineData(0.01, 0, 0.01, false)]     // Decimal -> NOW Small Path (Previously Large)
        [InlineData(1.23, -2, 0.0123, false)]  // Negative Exp -> NOW Small Path
        public void Constructor_Normalization_PathCheck(double mIn, long eIn, double expectedSmall, bool expectedIsLarge) {
            var val = new BigNum(mIn, eIn);

            Assert.Equal(expectedIsLarge, val.IsLarge);

            if (!expectedIsLarge) {
                // Use precision: 15 to handle tiny floating point jitter
                Assert.Equal(expectedSmall, val.Small, 15);
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
        [Theory]
        [InlineData("100", "100", true)]   // Exactly equal (Small)
        [InlineData("1.00e10", "1.00e10", true)] // Exactly equal (Large)
        [InlineData("99", "100", true)]    // Less than
        [InlineData("101", "100", false)]  // Greater than (should fail <=)
        public void Operator_LessThanOrEqual_Works(string valA, string valB, bool expected) {
            BigNum a = BigNum.Parse(valA);
            BigNum b = BigNum.Parse(valB);

            bool result = a <= b;

            Assert.Equal(expected, result);
        }
        [Theory]
        [InlineData("100", "100", true)]   // Exactly equal (Small)
        [InlineData("1.00e10", "1.00e10", true)] // Exactly equal (Large)
        [InlineData("101", "100", true)]   // Greater than
        [InlineData("99", "100", false)]   // Less than (should fail >=)
        public void Operator_GreaterThanOrEqual_Works(string valA, string valB, bool expected) {
            BigNum a = BigNum.Parse(valA);
            BigNum b = BigNum.Parse(valB);

            bool result = a >= b;

            Assert.Equal(expected, result);
        }
        [Fact]
        public void Operator_Increment_SmallNumber_AddsOne() {
            BigNum a = new BigNum(99);

            // In C#, prefix/postfix ++ modifies the variable
            a++;

            Assert.Equal(new BigNum(100), a);
        }

        [Fact]
        public void Operator_Increment_LargeNumber_AborbsOne() {
            BigNum a = BigNum.Parse("1.00e20");

            a++;

            // Ticks should absorb the 1 and stay at 1.00e20 [3]
            Assert.Equal("1.00e20", a.ToString());
        }
        [Theory]
        [InlineData("100", "50", "50")]              // Small path subtraction
        [InlineData("1.10e19", "2e18", "9.00e18")]   // Crossing back under the long threshold
        [InlineData("10", "20", "-10")]             // Going negative
        public void Operator_Subtraction_CalculatesCorrectly(string valA, string valB, string expected) {
            BigNum a = BigNum.Parse(valA);
            BigNum b = BigNum.Parse(valB);

            BigNum result = a - b;

            Assert.Equal(expected, result.ToString());
        }
        [Theory]
        [InlineData("10", "20", "200")]               // Small path multiplication
        [InlineData("2.00e8", "3.00e8", "6.00e16")]   // Large path without normalization
        [InlineData("5.00e8", "5.00e8", "2.50e17")]   // Large path WITH normalization (5*5=25 -> 2.5e11)
        [InlineData("2.00e10", "3.00e10", "6.00e20")] //small * small = large
        [InlineData("2.00e10", "3.00e20", "6.00e30")] //small * small = large
        public void Operator_Multiplication_CalculatesCorrectly(string valA, string valB, string expected) {
            BigNum a = BigNum.Parse(valA);
            BigNum b = BigNum.Parse(valB);

            BigNum result = a * b;

            Assert.Equal(expected, result.ToString());
        }
    }
}