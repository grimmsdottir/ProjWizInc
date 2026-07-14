using System;
using System.Collections.Generic;
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

        public BigNum(long value) {
            _isLarge = false;
            _small = value;
            _man = 0;
            _exp = 0;
            _isNegative = value >= 0;
        }
        public BigNum(int value) : this((long)value) { }
        public BigNum(double value) : this((long)value) { }
        public BigNum(double man, long exp) {
            if (Math.Abs(man) < EPSILON || man == 0) {
                this = new BigNum(0);
                return;
            }
            _isNegative = man >= 0;
            int shift = (int)Math.Floor(Math.Log10(Math.Abs(man)));
            double normMan = man / Math.Pow(10, shift);
            long normExp = exp + shift;
            if (normMan >= 0 && normExp < THRESH_POW) {
                _isLarge = false;
                _small = (long)(normMan * Math.Pow(10,normExp));
                _man = 0;
                _exp = 0;
            } else {
                _isLarge = true;
                _small = 0;
                _man = normMan;
                _exp = normExp;
            }
        }
        public static BigNum operator +(BigNum a, BigNum b) {
            if (a._isNegative != b._isNegative) { return a - b; }
            //small + small
            if (!a._isLarge && !b._isLarge) {
                long sum = a._small + b._small;
                if (Math.Abs(sum) < THRESHOLD) {
                    //= still small
                    return new BigNum(sum);
                } else {
                    //= big now
                    return new BigNum((double)sum,0);
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
            if (a._exp != b._exp) return a._exp > b._exp;
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
            if (!_isLarge) return _small.ToString("N0");
            return $"{_man:F2}e{_exp}";
        }
        //these 2 lines allow us to natively/automatically turn numbers into BigNums
        public static implicit operator BigNum(long v) => new BigNum(v);
        public static implicit operator BigNum(double v) => new BigNum(v, 0);
    }
}
