using ProjWizInc.Core.ADT;

namespace xUnitTester {
    public class BigNumTests {
        //reminder that our boundry is around 2e15
        private const int NUM_0 = 0;
        private const int NUM_1 = 1;
        private const int NUM_SMALL = 1000;
        private const double NUM_BIG = 1e25;
        private static BigNum _tmpBigNumA;
        private static BigNum _tmpBigNumB;
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
        public void Constructor_AlwaysNormalizesCorrectly(double mIn, int eIn, double mExp, long eExp) {
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
        public void Constructor_Normalization_PathCheck(double mIn, int eIn, double expectedSmall, bool expectedIsLarge) {
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
        // === CATEGORY 1: SMALL PATH / SMALL PATH (Result fits in double) ===
        [InlineData("10", "20", "30")]               // Simple whole-number addition
        [InlineData("12.5", "5.5", "18")]            // Decimals addition
        [InlineData("0.0001", "0.0002", "0.0003")]   // Tiny decimals on the small path

        // === CATEGORY 2: SMALL TO LARGE THRESHOLD PROMOTION (Crossing over 1e15) ===
        // 9.0e14 (small) + 2.0e14 (small) = 1.1e15. Since 1.1e15 is above 1e15, 
        // the constructor must promote the sum to the scientific large path: "1.10e15"
        [InlineData("9.00e14", "2.00e14", "1.10e15")]

        // === CATEGORY 3: LARGE PATH / LARGE PATH (Same Exponents) ===
        [InlineData("1.50e20", "2.50e20", "4.00e20")] // Standard same exponent addition
        [InlineData("5.00e20", "5.00e20", "1.00e21")] // Addition resulting in coefficient normalization (10e20 -> 1e21)

        // === CATEGORY 4: LARGE PATH / LARGE PATH (Different Exponents, Within 15 Orders) ===
        [InlineData("1.50e20", "2.00e18", "1.52e20")] // a is larger: 150e18 + 2e18 = 152e18 -> 1.52e20
        [InlineData("2.00e18", "1.50e20", "1.52e20")] // b is larger: 2e18 + 150e18 = 152e18 -> 1.52e20 (Verifies operand swapping!)

        // === CATEGORY 5: SCALE LIMITS (expDiff exceeds 15) ===
        [InlineData("1.50e50", "2.00e20", "1.50e50")] // a >> b (Returns a)
        [InlineData("2.00e20", "1.50e50", "1.50e50")] // b >> a (Returns b via operand swapping!)

        // === CATEGORY 6: SIGN VARIATIONS (Mismatched signs redirecting to subtraction) ===
        [InlineData("10", "-3", "7")]                 // Positive plus negative (small)
        [InlineData("-10", "3", "-7")]                // Negative plus positive (small)
        [InlineData("1.50e20", "-2.00e18", "1.48e20")]  // Positive plus negative (large)
        [InlineData("-1.50e20", "2.00e18", "-1.48e20")] // Negative plus positive (large)
        public void Operator_Addition_CalculatesCorrectly(string valA, string valB, string expected) {
            BigNum a = BigNum.Parse(valA);
            BigNum b = BigNum.Parse(valB);
            BigNum expectedBigNum = BigNum.Parse(expected);
            BigNum result = a + b;

            Assert.True(expectedBigNum == result, "Expected: " + expectedBigNum.ToString() + ", Actual: " + result.ToString());
        }
        // === CATEGORY 1: SMALL PATH / SMALL PATH (Result fits in double) ===
        [InlineData("100", "30", "70")]             // Simple whole-number subtraction
        [InlineData("12.5", "5.5", "7")]            // Decimals subtraction
        [InlineData("10", "20", "-10")]             // Small crossing below zero

        // === CATEGORY 2: LARGE TO SMALL THRESHOLD CROSSING (Collapsing under 1e15) ===
        // 1.01e15 (large) - 2.0e13 (large) = 9.9e14. Since 9.9e14 is below 1e15, 
        // the constructor must collapse it back to the small path double string "990000000000000"
        [InlineData("1.01e15", "2.00e13", "990000000000000")]

        // === CATEGORY 3: LARGE PATH / LARGE PATH (Same Exponents) ===
        [InlineData("5.00e20", "2.00e20", "3.00e20")] // Standard same exponent subtraction
        [InlineData("1.50e20", "1.50e20", "0")]       // Identity subtraction resulting in 0

        // === CATEGORY 4: LARGE PATH / LARGE PATH (Different Exponents, Within 15 Orders) ===
        [InlineData("1.50e20", "2.00e18", "1.48e20")]   // 150.0e18 - 2.0e18 = 148.0e18 -> 1.48e20
        [InlineData("2.50e18", "1.50e20", "-1.475e20")] // 2.5e18 - 150.0e18 = -147.5e18 -> -1.475e20

        // === CATEGORY 5: SCALE LIMITS (expDiff exceeds 15) ===
        [InlineData("1.50e50", "2.00e20", "1.50e50")]   // a >> b (Returns a)
        [InlineData("2.00e20", "1.50e50", "-1.50e50")]  // b >> a (Returns -b via our symmetric fix!)

        // === CATEGORY 6: SIGN VARIATIONS (Equal signs) ===
        [InlineData("-100", "-30", "-70")]           // Negative minus negative (small)
        [InlineData("-1.50e20", "-2.00e18", "-1.48e20")] // Negative minus negative (large)
        public void Operator_Subtraction_CalculatesCorrectly(string valA, string valB, string expected) {
            BigNum a = BigNum.Parse(valA);
            BigNum b = BigNum.Parse(valB);
            BigNum expectedBigNum = BigNum.Parse(expected);
            BigNum result = a - b;

            Assert.True(expectedBigNum == result, "Expected: " + expectedBigNum.ToString() + ", Actual: " + result.ToString());
        }
        [Theory]
        // === CATEGORY 1: SMALL PATH / SMALL PATH (Result fits in double) ===
        [InlineData("10", "20", "200")]             // Simple whole-number multiplication
        [InlineData("0.5", "10", "5")]              // Fractional multiplication
        [InlineData("1.5", "2.5", "3.75")]          // Small decimals

        // === CATEGORY 2: SMALL PATH TO LARGE PATH TRANSITIONS ===
        // Large values constructed as small-path doubles, crossing our 1e15 boundary
        [InlineData("2.00e10", "3.00e10", "6.00e20")] // 2e10 * 3e10 = 6e20
        [InlineData("5.00e8", "5.00e8", "2.50e17")]   // 5e8 * 5e8 = 2.5e17 (above 1e15)

        // === CATEGORY 3: LARGE PATH / LARGE PATH (Direct scale summation) ===
        [InlineData("2.00e20", "3.00e10", "6.00e30")] // Exponent addition (20 + 10 = 30)

        // === CATEGORY 4: LARGE PATH WITH MANTISSA NORMALIZATION (Coefficient >= 10) ===
        // 5.0e20 * 4.0e10 = 20.0e30. Normalizes 20.0 down to 2.0, shifting exponent to 31.
        [InlineData("5.00e20", "4.00e10", "2.00e31")]
        // 5.0e20 * 5.0e-22 = 25.0e-2. Normalizes 25.0 to 2.5, exp to -1 (0.25 on the small path)
        [InlineData("5.00e20", "5.00e-22", "0.25")]

        // === CATEGORY 5: SIGN VARIATIONS ===
        [InlineData("-10", "5", "-50")]             // Negative dividend
        [InlineData("10", "-5", "-50")]             // Negative divisor
        [InlineData("-10", "-5", "50")]             // Both negative
        [InlineData("-2.00e20", "3.00e10", "-6.00e30")] // Large negative dividend
        [InlineData("2.00e20", "-3.00e10", "-6.00e30")] // Large negative divisor
        [InlineData("-2.00e20", "-3.00e10", "6.00e30")]  // Large both negative

        // === CATEGORY 6: DECIMAL BOUNDARIES / TINY DECIMALS ===
        [InlineData("1.50e-10", "2.00e-15", "3.00e-25")] // Tiny fractional multiplication
        // 2.0e-5 * 5.0e-6 = 10.0e-11. Normalizes 10.0 to 1.0, exp to -10.
        [InlineData("2.00e-5", "5.00e-6", "1.00e-10")]

        // === CATEGORY 7: ZERO MULTIPLICATION LIMITS ===
        [InlineData("1.00e50", "0", "0")]           // Large positive multiplied by zero
        [InlineData("0", "-2.50e20", "0")]          // Zero multiplied by large negative
        public void Operator_Multiplication_CalculatesCorrectly(string valA, string valB, string expected) {
            BigNum a = BigNum.Parse(valA);
            BigNum b = BigNum.Parse(valB);
            BigNum expectedBigNum = BigNum.Parse(expected);
            BigNum result = a * b;

            Assert.True(expectedBigNum == result, "Expected: " + expectedBigNum.ToString() + ", Actual: " + result.ToString());
        }
        [Theory]
        // === CATEGORY 1: SMALL PATH / SMALL PATH (Result fits in double) ===
        [InlineData("10", "5", "2")]                // Simple exact division
        [InlineData("1", "2", "0.5")]              // Fractional result
        [InlineData("7.5", "2.5", "3")]            // Decimals division

        // === CATEGORY 2: LARGE PATH / LARGE PATH (Same Exponents) ===
        [InlineData("8.00e20", "2.00e20", "4")]     // Standard division (Outputs small path "4")
        [InlineData("1.50e20", "3.00e20", "0.5")]   // Outputs small path "0.5"

        // === CATEGORY 3: LARGE PATH / LARGE PATH (Different Exponents) ===
        [InlineData("1.00e30", "1.00e10", "1.00e20")]  // Exponent subtraction (30 - 10 = 20)
        [InlineData("6.00e50", "3.00e20", "2.00e30")]  // Exponent subtraction with coefficients
        [InlineData("2.50e15", "5.00e30", "5.00e-16")] // Negative exponent shift (15 - 30 = -15)

        // === CATEGORY 4: MIXED PATH (Large Dividend, Small Divisor) ===
        [InlineData("1.00e20", "100", "1.00e18")]   // Dividing large by small whole number
        [InlineData("1.50e16", "3", "5.00e15")]     // Dividing large by small decimal-scale

        // === CATEGORY 5: MIXED PATH (Small Dividend, Large Divisor) ===
        [InlineData("1", "1.00e16", "1.00e-16")]    // Dividend is much smaller than divisor
        [InlineData("500", "1.00e20", "5.00e-18")]  // Exponent shifting down past boundaries

        // === CATEGORY 6: SIGN VARIATIONS ===
        [InlineData("-10", "2", "-5")]              // Negative dividend
        [InlineData("10", "-2", "-5")]              // Negative divisor
        [InlineData("-10", "-2", "5")]              // Both negative
        [InlineData("-1.00e30", "1.00e10", "-1.00e20")] // Large negative dividend
        [InlineData("1.00e30", "-1.00e10", "-1.00e20")] // Large negative divisor
        [InlineData("-1.00e30", "-1.00e10", "1.00e20")] // Large both negative

        // === CATEGORY 7: ZERO DIVIDENDS (Division of zero is always zero) ===
        [InlineData("0", "100", "0")]               // Zero divided by small
        [InlineData("0", "1.00e30", "0")]           // Zero divided by large
        public void Operator_Division_CalculatesCorrectly(string valA, string valB, string expected) {
            BigNum a = new BigNum(valA);
            BigNum b = new BigNum(valB);
            BigNum expectedBigNum = new BigNum(expected);
            BigNum result = a / b;
            Assert.True(expectedBigNum == result, "Expected: " + expectedBigNum.ToString() + ", Actual: " + result.ToString());

        }
        private void ExecuteDivisionUnderTest() {
            BigNum result = _tmpBigNumA / _tmpBigNumB;
        }
        [Theory]
        [InlineData("10")]          // Small positive divided by zero
        [InlineData("-10")]         // Small negative divided by zero
        [InlineData("1.00e20")]     // Large positive divided by zero
        [InlineData("-1.00e20")]    // Large negative divided by zero
        [InlineData("0")]           // Zero divided by zero (NaN check)
        public void Operator_Division_ByZero_ThrowsDivideByZeroException(string valA) {
            // Arrange
            _tmpBigNumA = new BigNum(valA);
            _tmpBigNumB = new BigNum(0);

            // Act & Assert
            // We pass the method group directly to verify it throws the standard exception
            Assert.Throws<DivideByZeroException>(ExecuteDivisionUnderTest);
        }
        [Theory]
        // === CATEGORY 1: SMALL PATH (Positive to Negative) ===
        [InlineData("10", "-10")]                  // Small whole number
        [InlineData("125.5", "-125.5")]            // Small decimal
        [InlineData("150000", "-150000")]          // Large whole number on the small path

        // === CATEGORY 2: SMALL PATH (Negative to Positive) ===
        [InlineData("-10", "10")]                  // Small whole number
        [InlineData("-125.5", "125.5")]            // Small decimal
        [InlineData("-150000", "150000")]          // Large whole number on the small path

        // === CATEGORY 3: LARGE PATH (Positive to Negative) ===
        [InlineData("1.50e20", "-1.50e20")]        // Standard large exponent
        [InlineData("2.25e50", "-2.25e50")]        // Very large exponent
        [InlineData("1.00e100", "-1.00e100")]      // Centillion-scale exponent

        // === CATEGORY 4: LARGE PATH (Negative to Positive) ===
        [InlineData("-1.50e20", "1.50e20")]        // Standard large exponent
        [InlineData("-2.25e50", "2.25e50")]        // Very large exponent
        [InlineData("-1.00e100", "1.00e100")]      // Centillion-scale exponent

        // === CATEGORY 5: DECIMAL BOUNDARIES (Exponents <= -4) ===
        // Below 1e-4, the constructor switches to the Large path to preserve visual clarity.
        [InlineData("1.50e-5", "-1.50e-5")]        // Tiny fractional positive to negative
        [InlineData("-1.50e-5", "1.50e-5")]        // Tiny fractional negative to positive
        [InlineData("3.25e-25", "-3.25e-25")]      // Extremely small decimal positive to negative
        [InlineData("-3.25e-25", "3.25e-25")]      // Extremely small decimal negative to positive

        // === CATEGORY 6: ZERO BOUNDARY (Zero must remain unsigned) ===
        [InlineData("0", "0")]                     // Standard positive zero
        [InlineData("0.0", "0")]                   // Decimal zero
        [InlineData("-0", "0")]                    // Negative zero must resolve to unsigned zero
        public void Operator_Negation_CalculatesCorrectly(string valA, string expected) {
            BigNum a = new BigNum(valA);
            BigNum expectedBigNum = new BigNum(expected);
            BigNum result = -a;

            Assert.True(expectedBigNum == result, "Expected: " + expectedBigNum.ToString() + ", Actual: " + result.ToString());
        }
        // === CATEGORY 1: SMALL PATH (Both are below 1e15) ===
        [InlineData("10", "3", "1")]                // Standard whole-number modulo
        [InlineData("25", "5", "0")]                // Exact divisibility
        [InlineData("12.5", "5", "2.5")]            // Small decimals
        // === CATEGORY 2: MIXED PATH (Large Dividend, Small Divisor) ===
        // 1.500001e16 % 3.0e5. expDiff = 11 (within 15). 
        // scaledManA = 150000100000. resultMan = 150000100000 % 3 = 1.0. 
        // 1.0e5 is below 1e15, so returns small path string.
        [InlineData("1.500001e16", "3.0e5", "100000")]
        // === CATEGORY 3: LARGE PATH (Same Exponents, Mantissa Modulo) ===
        [InlineData("1.5e16", "3.0e15", "0")]       // 15.0e15 % 3.0e15 = 0
        [InlineData("2.5e16", "4.0e16", "2.50e16")] // |a| < |b| with same exponents
        // === CATEGORY 4: LARGE PATH (Different Exponents, Within 15 Orders) ===
        [InlineData("1.5e20", "4.0e18", "2.00e18")] // 150.0e18 % 4.0e18 = 2.0e18
        [InlineData("2.5e25", "6.0e24", "1.00e24")] // 25.0e24 % 6.0e24 = 1.0e24
        // === CATEGORY 5: SIGN VARIATIONS (Sign is determined by Dividend 'a') ===
        [InlineData("-10", "3", "-1")]              // Negative dividend, positive divisor
        [InlineData("10", "-3", "1")]               // Positive dividend, negative divisor
        [InlineData("-10", "-3", "-1")]             // Negative dividend, negative divisor
        [InlineData("-1.5e20", "4.0e18", "-2.00e18")] // Large negative dividend
        [InlineData("1.5e20", "-4.0e18", "2.00e18")]  // Large negative divisor
        // === CATEGORY 6: SCALE LIMITS (expDiff exceeds 15) ===
        // True remainder is lost to double precision limits. Return fallback 0.
        [InlineData("1.0e30", "1.0e10", "0")]
        [InlineData("1.5e50", "2.0e20", "0")]
        // === CATEGORY 7: DIVISOR DOMINANT (|a| < |b| returns 'a') ===
        [InlineData("5", "10", "5")]                // Small divisor dominant
        [InlineData("1.5e20", "3.0e25", "1.50e20")] // Large divisor dominant
        public void Operator_Modulo_CalculatesCorrectly(string valA, string valB, string expected) {
            BigNum a = new BigNum(valA);
            BigNum b = new BigNum(valB);
            BigNum result = a % b;
            Assert.Equal(expected, result.ToString());
        }
    }
}