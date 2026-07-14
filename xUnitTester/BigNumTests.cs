using ProjWizInc.Core.ADT;

namespace xUnitTester {
    public class BigNumTests {
        //check if we can read valid numbers correctly
        [Theory]
        [InlineData("100", 100)]
        [InlineData("12.5", 12.5)]
        [InlineData("1.0e10", 10000000000)]
        [InlineData("  500  ", 500)] // Testing the Trim()
        public void Parse_ValidatesAllFormats(string input, double expectedValue) {
            var result = BigNum.Parse(input);
            // You'll need an explicit/implicit cast to double for this assertion
            Assert.Equal(expectedValue, (double)result);
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