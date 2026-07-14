using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.ADT {
    public readonly record struct BigNum {
        private readonly long _small;
        private readonly double _man;
        private readonly long _exp;
        private readonly bool _isLarge;
        private readonly bool _isNegative;

        private const long THRESHOLD = 1_000_000_000_000_000L; // 1e15
        private const int THRESH_POW = 15;
        private const double EPSILON = 1e-15;

        //public getters
        public long Small => _small;
        public double Man => _man;
        public long Exp => _exp;
        public bool IsLarge => _isLarge;
        public bool IsNegative => _isNegative;
        
        public BigNum(long value) {
            _isLarge = false;
            _small = value;
            _man = 0;
            _exp = 0;
            _isNegative = value >= 0;
        }
        public BigNum(String s) {
            this = Parse(s);
        }
        public BigNum(int value) : this((long)value) { }
        public BigNum(double value) {
            //this case is what if a long/int was casted to a double then passed in
            //also i forgot that doubles go all to the way to 1.8e308, compared to long's 9.0e18
            if (value % 1 == 0 && value <= long.MaxValue && value >= long.MinValue) { 
                _small = (long)value;
                _exp = 0;
                _man = value;
                _isLarge = false;
                if (value > 0) {
                    _isNegative = false;
                } else {
                    _isNegative = true;
                }
            } else {
                //this case means it is a decimal/fractional or much bigger than a long
                _isLarge = true;
                _small = 0;
                //holy math stack batman
                //so first, we take the absolute value of value, because logs dont like negative numbers
                //then we get the log10 of the value, which gives us basically the number of zeros in the value
                //then we floor and long it, because we dont need all that other fractionals for an exponent
                _exp = (long)Math.Floor(Math.Log10(Math.Abs(value)));
                _man = value / Math.Pow(10, _exp);
            }
        }
        public BigNum(double man, long exp) {
            //first we check if we are dumb enough to try passing in 0
            if (man == 0) {
                _man = 0;
                _exp = 0;
                _small = 0;
                _isLarge = false;
                return;
            }
            //we check for the "internal exponent" like 150 should be 1.5e2
            long intExp = (long)Math.Floor(Math.Log10(man));
            //we pass along the internal exponent to the real exponent
            exp += intExp;
            //and we flatten the mantissa down to be >= 1 && <10
            man /= Math.Pow(10, intExp);
            //sometimes we might end up with funky mantissas remaining like 10.00000000001 
            if (Math.Abs(man) >= 10) {
                man /= 10;
                exp++;
            }
            //lastly we check if the resulting man-exp is actually small enough to fit in a long
            double manExp = man * Math.Pow(10, exp);
            //quick check if the number is negative
            _isNegative = man < 0;
            //is the exponent between 0 and 18, and is it a whole number
            if (exp >= 0 && exp <= 18 && manExp % 1 == 0) {
                //it can be a long
                _small = (long)manExp;
                _isLarge = false;
                
                
            } else {
                //its big enough to need the full man-exp
                _small = 0;
                _isLarge = true;
            }
            //normal small long numbers dont have man or exp, but since this one came to us malformed
            //we just leave it in in case its important
            //regardless we will want to set the man and exp, so no need to repeat it twice below
            _man = man;
            _exp = exp;
        }
        public static BigNum operator +(BigNum a, BigNum b) {
            if (a._isNegative != b._isNegative) { return a - b; }
            //small + small
            if (!a._isLarge && !b._isLarge) {
                //if we add 2 smalls together, they can cross the boundry and overflow if we did a simple long sum
                //doubles can hold more than longs, so we can use double addition to be safe. precision should not matter
                //at the long boundry, 9e18
                double testSum = (double)a._small + (double)b._small;
                if (testSum <= long.MinValue || testSum >= long.MaxValue) {
                    //we can just use our constructor to resolve the addition, since we already added it
                    return new BigNum(testSum);
                } else {
                    //in this case we can just return the 2 smalls added together
                    //we dont use testSum because its a double and we dont need to lose precision over that
                    return new BigNum(a._small + b._small);
                }                
            }
            //if either numbers are big, we have to do the slower operation
            //if a is small, we just use the small, otherwise we take their man and exp
            var (man1, exp1) = a.GetParts();
            var (man2, exp2) = b.GetParts();
            //ensure that a is smaller than b
            if (exp1 < exp2) {
                double manTmp = man1;
                long expTmp = exp1;
                man1 = man2;
                exp1 = exp2;
                man2 = manTmp;
                exp2 = expTmp;
            }
            long diff = exp1 - exp2;
            if (diff > THRESH_POW) { 
                return new BigNum(man1,exp1);
            } else {
                return new BigNum(man1 + (man2 / Math.Pow(10, diff)), exp1);
            }
        }
        public static BigNum operator ++(BigNum a) {
            if (a._isLarge) {  return a; }
            return a + 1;
        }
        public static BigNum operator -(BigNum a, BigNum b) {
            if (a._isNegative != b._isNegative) { return a + b; }

            var (man1, exp1) = a.GetParts();
            var (man2, exp2) = b.GetParts();
            long diff = exp1 - exp2;
            if (diff > THRESH_POW) { return a; }
            return new BigNum(man1 - (man2 / Math.Pow(10, diff)), exp1);
        }
        public static BigNum operator --(BigNum a) {
            if (a._isLarge) { return a; }
            return a - 1;
        }
        public static BigNum operator *(BigNum a, BigNum b) {
            var (man1, exp1) = a.GetParts();
            var (man2, exp2) = b.GetParts();
            return new BigNum(man1 * man2, exp1 + exp2);
        }
        private (double m, long e) GetParts() => _isLarge ? (_man, _exp) : (_small, 0);
        //comparator section
        public static bool operator >(BigNum a, BigNum b) {
            //if both a and b are small, we do a regular comparison
            if (!a._isLarge && !b._isLarge) { 
                return a._small > b._small;
            }
            long expA = a._exp; 
            long expB = b._exp;
            //for the most time, we just need to compare exponents
            if (expA > expB) { return true; }
            if (expA < expB) { return false; }
            //if the code reached here, that means the exponents are equal, so we just compare mantissas
            return a._man > b._man;
        }
        public static bool operator <(BigNum a, BigNum b) => b > a;
        public static bool operator >=(BigNum a, BigNum b) => a > b || a == b;
        public static bool operator <=(BigNum a, BigNum b) => b >= a;
        public static BigNum Parse(string s) {
            if (string.IsNullOrWhiteSpace(s)) {
                throw new ArgumentException("Cannot parse empty string to BigNum");
            }
            //gotta trim whitespace for JSON and user input
            s = s.Trim();
            //case 1: scientific notation(1.0e50)
            if (s.Contains('e', StringComparison.OrdinalIgnoreCase)) {
                string[] parts = s.Split(new[] { 'e', 'E' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2) throw new FormatException("Invalid scientific notation format.");

                return new BigNum(double.Parse(parts[0]), long.Parse(parts[1]));
            }
            //case 2: smaller decimals. as it is it just yeets fractionals and turns doubles to longs
            if (s.Contains('.')) {
                return new BigNum(double.Parse(s));
            }
            //case 3: standard long input
            return new BigNum(long.Parse(s));
        }
        public override string ToString() {
            //todo: allow for variable string formating, like k,m,qd,qt etc 
            if (!_isLarge) {
                return _small.ToString();
            }
            return $"{_man:F2}e{_exp}";
        }
        //these 2 lines allow us to natively/automatically turn numbers into BigNums
        public static implicit operator BigNum(long v) => new BigNum(v);
        public static implicit operator BigNum(double v) => new BigNum(v, 0);
    }
}
