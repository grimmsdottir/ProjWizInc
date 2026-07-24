using System.Runtime.InteropServices;

namespace ProjWizInc.Engine.Data.ADT {
    /*
     * Our special custom ADT for handling numbers way bigger than a double.
     * Most of the math in our engine runs on this format. It is designed to be as performant as a non-primative struct can be
     * Calclulates values in double if possible, but automatically shifts to scientific mode if needed
     * Automatically normalises any value into the M * 10 ^ E, where 1 <= M < 10
     */
    // 1. Special states for division-by-zero or mathematical failures
    public enum NumericState : byte {
        Normal = 0,
        NaN = 1,
        PositiveInfinity = 2,
        NegativeInfinity = 3
    }
    // Guarantees 8-byte sequential packing on the stack with zero alignment padding
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public readonly record struct BigNum {
        //special static array that holds all the powers of 10 from 10^-324 to 10^308, so we dont have to call Math.Pow every time we need a power of 10
        private static readonly double[] PowersOf10;
        private const int MIN_POWER = -324;
        private const int MAX_POWER = 308;
        private const int BIAS = 324; // Offsets negative index bounds to start at 0
        //simplified constants for the threshold values, so we dont have to calculate them every time
        private const double THRESHOLD_HIGH = 1_000_000_000_000_000; // 1e15
        private const double THRESHOLD_LOW = 0.0001; //1e-4, we could use 1e-15, but visually 1e-4 is near the limit of human perception
        private const double EQUAL_THRESH = 0.00000000000001; //1e-14. if the ratio of difference in numbers is lesser or equal than we accept that it is equal
        private const int THRESH_POW = 15;
        private const double EPSILON = 1e-15;
        // === INSTANCE FIELDS (Ordered descending by size for optimal 24-byte stack footprint) ===
        private readonly double _small;       // Offset 0  (8 bytes)
        private readonly double _man;         // Offset 8  (8 bytes)
        private readonly int _exp;            // Offset 16 (4 bytes) - Swapped to int
        private readonly bool _isLarge;       // Offset 20 (1 byte)
        private readonly bool _isNegative;    // Offset 21 (1 byte)
        private readonly NumericState _state; // Offset 22 (1 byte)  - Exponent Special State
        private readonly byte _towerLevel;    // Offset 23 (1 byte)  - Tetration Exponent Tower Level - TBD
        static BigNum() {
            //we need an array to hold all the powers of 10 from 10^-324 to 10^308, so we dont have to call Math.Pow every time we need a power of 10
            PowersOf10 = new double[MAX_POWER - MIN_POWER + 1];
            for (int i = MIN_POWER; i <= MAX_POWER; i++) {
                int arrayIndex = i + BIAS;
                PowersOf10[arrayIndex] = Math.Pow(10, i);
            }
        }
        //our blessed O(1) power of 10 function, that uses the precomputed array to return the power of 10 in constant time
        public static double GetPowerOf10(int power) {
            if (power >= MIN_POWER && power <= MAX_POWER) {
                return PowersOf10[power + BIAS];
            }
            // Fallback for extreme exponent cases (e.g. infinite scales)
            return Math.Pow(10, power);
        }
        //public getters
        public double Small => _small;
        public double Man => _man;
        public int Exp => _exp;
        public bool IsLarge => _isLarge;
        public bool IsNegative => _isNegative;
        public BigNum() : this(0) { }
        //int and long constructors are just wrappers for the double constructor, since they are all small numbers
        public BigNum(int value) : this((double)value) { }
        public BigNum(long value) : this((double)value) { }
        //we now have a seperate constructor for double, that doesnt need to chain to the scientific constructor, since it can handle small numbers itself
        public BigNum(double value) {
            _isNegative = value < 0;
            _state = NumericState.Normal;
            _towerLevel = 0;
            double absVal = Math.Abs(value);

            if (value == 0) {
                _man = 0;
                _exp = 0;
                _small = 0;
                _isLarge = false;
                return;
            }

            if (absVal >= THRESHOLD_LOW && absVal < THRESHOLD_HIGH) {
                _small = value;
                _isLarge = false;
                //TODO: is neccesary?
                int intExp = (int)Math.Floor(Math.Log10(absVal));
                _exp = intExp;
                _man = value / GetPowerOf10(intExp);
            } else {
                _small = 0;
                _isLarge = true;

                int intExp = (int)Math.Floor(Math.Log10(absVal));
                _exp = intExp;
                _man = value / GetPowerOf10(intExp);
            }
        }
        //the old constructor that takes in a mantissa and exponent, and normalizes it to be in the correct range
        public BigNum(double man, int exp) {
            _state = NumericState.Normal;
            _towerLevel = 0;

            if (man == 0) {
                _man = 0;
                _exp = 0;
                _small = 0;
                _isLarge = false;
                _isNegative = false;
                return;
            }

            // Calculate internal exponent of input mantissa
            int intExp = (int)Math.Floor(Math.Log10(Math.Abs(man)));
            exp = checked(exp + intExp); // Checked arithmetic prevents silent overflow
            man /= GetPowerOf10(intExp);

            // Handle representation edge cases
            if (Math.Abs(man) >= 10) {
                man /= 10;
                exp = checked(exp + 1);
            }

            _isNegative = man < 0;

            // Checked boundary check using integer comparisons
            if (exp >= -4 && exp < 15) {
                _small = man * GetPowerOf10(exp);
                _isLarge = false;
            } else {
                _small = 0;
                _isLarge = true;
            }

            _man = man;
            _exp = exp;
        }
        public BigNum(string s) {
            BigNum val = Parse(s);
            _state = NumericState.Normal;
            _towerLevel = 0;
            _isLarge = val._isLarge;
            _small = val._small;
            _isNegative = val._isNegative;
            _man = val._man;
            _exp = val._exp;
        }
        public static BigNum operator +(BigNum a, BigNum b) {
            if (a._isNegative != b._isNegative) { return a - (-b); }
            //small + small
            if (!a._isLarge && !b._isLarge) {
                //if we add 2 smalls together, they can cross the boundry and overflow if we did a simple long sum
                //doubles can hold more than longs, so we can use double addition to be safe. precision should not matter
                //at the long boundry, 9e18
                double testSum = a._small + b._small;
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
            double man1 = a.Man;
            double man2 = b.Man;
            int exp1 = a.Exp;
            int exp2 = b.Exp;
            //ensure that a is smaller than b
            if (exp1 < exp2) {
                double manTmp = man1;
                int expTmp = exp1;
                man1 = man2;
                exp1 = exp2;
                man2 = manTmp;
                exp2 = expTmp;
            }
            int diff = checked(exp1 - exp2);
            if (diff > THRESH_POW) { 
                return new BigNum(man1,exp1);
            } else {
                return new BigNum(man1 + man2 / GetPowerOf10(diff), exp1);
            }
        }
        public static BigNum operator ++(BigNum a) {
            if (a._isLarge) {  return a; }
            return a + 1;
        }
        public static BigNum operator -(BigNum a, BigNum b) {
            if (a._isNegative != b._isNegative) { return a + -b; }

            double man1 = a.Man;
            double man2 = b.Man;
            int exp1 = a.Exp;
            int exp2 = b.Exp;
            int diff = checked( exp1 - exp2);
            //if the difference is too big, we can just return the larger number, because the smaller number is negligible
            if (diff > THRESH_POW) { return a; }
            //same with the other way around, if b is larger than a, we can just return b
            if (diff < -THRESH_POW) { return -b; }
            return new BigNum(man1 - man2 / GetPowerOf10(diff), exp1);
        }
        public static BigNum operator --(BigNum a) {
            if (a._isLarge) { return a; }
            return a - 1;
        }
        public static BigNum operator *(BigNum a, BigNum b) {
            double man1 = a.Man;
            double man2 = b.Man;
            int exp1 = a.Exp;
            int exp2 = b.Exp;
            return new BigNum(man1 * man2, checked(exp1 + exp2));
        }
        public static BigNum operator /(BigNum a, BigNum b) {
            if (b.Equals(new BigNum(0))) {
                throw new DivideByZeroException("Cannot divide BigNum by zero.");
            }

            double man1 = a.Man;
            double man2 = b.Man;
            int exp1 = a.Exp;
            int exp2 = b.Exp;
            return new BigNum(man1 / man2, checked(exp1 - exp2));
        }
        public static BigNum operator %(BigNum a, BigNum b) {
            if (b == new BigNum(0)) { throw new DivideByZeroException("Cannot modulo BigNum by zero"); }
            //if both numbers are small, we can use good ol double modulo
            if (!a.IsLarge && !b.IsLarge) {
                return new BigNum(a.Small % b.Small);
            }
            //otherwise we have to do the slow scientific path
            double man1 = a.Man;
            double man2 = b.Man;
            int exp1 = a.Exp;
            int exp2 = b.Exp;
            //same exponent means we can just do a regular modulo on the mantissas
            if (exp1 == exp2) {
                return new BigNum(man1 % man2, exp1);
            }
            //we use the math rule of if a < b, then a % b = a, so we just check the exponents and return the smaller one
            if (exp1 < exp2) {
                return a;
            }
            //now we come to the hard and slow path
            int expDiff = checked(exp1 - exp2);
            if (expDiff > THRESH_POW) {
                //if the difference is too big, we can just return a, because a is smaller than b
                return new BigNum(0);
            }
            // Scale the dividend mantissa to align exponents, then perform double modulo
            double scaledManA = man1 * GetPowerOf10(expDiff);
            double resultMan = scaledManA % man2;

            return new BigNum(resultMan, exp2);
        }
        //comparator section
        public static bool operator >(BigNum a, BigNum b) {
            // If both are small, perform fast hardware comparison
            if (!a._isLarge && !b._isLarge) {
                return a._small > b._small;
            }

            // 1. Mismatched Signs (Instant resolution)
            if (a._isNegative != b._isNegative) {
                return b._isNegative; // If b is negative, a must be positive, so a > b is true
            }

            // 2. Symmetrical Signs
            int expA = a._exp;
            int expB = b._exp;

            if (!a._isNegative) {
                // Both are Positive: larger exponent means larger number
                if (expA > expB) { return true; }
                if (expA < expB) { return false; }
                return a._man > b._man;
            } else {
                // Both are Negative: larger exponent means smaller (more-negative) number
                if (expA > expB) { return false; }
                if (expA < expB) { return true; }
                return a._man > b._man; // e.g. -1.5 * 10^20 > -2.0 * 10^20 is true
            }
        }
        //we do a logical delegation, since a < b is the same as b > a, we can just use the > operator to determine if a < b
        public static bool operator <(BigNum a, BigNum b) {
            return b > a;
        }
        public static bool operator >=(BigNum a, BigNum b) {
            return !(a < b);
        }
        public static bool operator <=(BigNum a, BigNum b) {
            return !(a > b);
        }
        //unary negation operator, that just flips the sign of the number
        public static BigNum operator -(BigNum a) {
            if (a._isLarge) {
                return new BigNum(-a._man, a._exp);
            }
            return new BigNum(-a._small);
        }
        // Explicit cast to double: allows progress bars to read ratios seamlessly
        public static explicit operator double(BigNum value) {
            if (!value._isLarge) {
                return value._small;
            }
            return value._man * GetPowerOf10(value._exp);
        }
        // Explicit cast to long: useful for standard integer operations
        public static explicit operator long(BigNum value) {
            if (!value._isLarge) {
                return (long)value._small;
            }

            double doubleVal = value._man * GetPowerOf10(value._exp);
            if (doubleVal >= long.MaxValue) {
                return long.MaxValue;
            }
            if (doubleVal <= long.MinValue) {
                return long.MinValue;
            }
            return (long)doubleVal;
        }
        // Explicit cast to int
        public static explicit operator int(BigNum value) {
            return (int)(long)value;//kinda funky, but is neccesary because it calls itself otherwise
        }
        public static BigNum Abs(BigNum value) {
            if (value._isLarge) {
                return new BigNum(Math.Abs(value._man), value._exp);
            }
            return new BigNum(Math.Abs(value._small));
        }

        public static BigNum Max(BigNum a, BigNum b) {
            if (a > b) {
                return a;
            }
            return b;
        }

        public static BigNum Min(BigNum a, BigNum b) {
            if (a < b) {
                return a;
            }
            return b;
        }

        public static BigNum Clamp(BigNum value, BigNum min, BigNum max) {
            if (value < min) {
                return min;
            }
            if (value > max) {
                return max;
            }
            return value;
        }
        public static BigNum Floor(BigNum value) {
            if (!value._isLarge) {
                return new BigNum(Math.Floor(value._small));
            }
            // For large values (>= 1e15), double-precision has already discarded
            // any fractional decimal digits. It is already mathematically an integer.
            return value;
        }
        //same as floor, at large scales, we can assume the number is already an integer, so we just return it as is
        public static BigNum Ceiling(BigNum value) {
            if (!value._isLarge) {
                return new BigNum(Math.Ceiling(value._small));
            }
            return value;
        }
        public static BigNum Pow(BigNum baseVal, int power) {
            if (power == 0) {
                return new BigNum(1);
            }
            if (power == 1) {
                return baseVal;
            }
            // Using the algebraic identity: (M * 10^E)^P = M^P * 10^(E * P)
            double newMan = Math.Pow(baseVal.Man, power);
            int newExp = baseVal.Exp * power;

            return new BigNum(newMan, newExp);
        }
        public static BigNum Pow(BigNum baseVal, double power) {
            if (power == 0.0) {
                return new BigNum(1);
            }
            if (power == 1.0) {
                return baseVal;
            }
            if (baseVal.Equals(new BigNum(0))) {
                return new BigNum(0);
            }
            if (baseVal.IsNegative && Math.Floor(power) != power) {
                throw new ArgumentException("Cannot raise a negative BigNum to a fractional power.");
            }

            // 1. Calculate the raw mantissa power: M^P
            double newMan = Math.Pow(baseVal.Man, power);

            // 2. Calculate E * P (resulting in a double)
            double rawExp = baseVal.Exp * power;

            // 3. Split rawExp into integer and fractional components
            double expFloor = Math.Floor(rawExp);
            int newExp = (int)expFloor;
            double expFraction = rawExp - expFloor;

            // 4. Multiply the fractional remainder back into the mantissa.
            // 10^expFraction is guaranteed to be within [1, 10) because expFraction is within [0, 1)
            newMan *= Math.Pow(10, expFraction);

            // 5. Pass to our scientific constructor to normalize if newMan falls outside [1, 10)
            return new BigNum(newMan, newExp);
        }
        /*
         * because BigNum is a struct, it uses Equals to operate ==, so this is not used
        public static bool operator ==(BigNum a, BigNum b) {}
        */
        public bool Equals(BigNum other) {
            //if one is bigly, and the other isnt, its clearly not the same already
            if (IsLarge != other.IsLarge) { return false; }
            //if they are both small numbers, we minus the two of them and if the difference is less than
            //the epsilon, its good enough to be equal
            if (!IsLarge) {
                double diff = Math.Abs(Small - other.Small);
                if (diff == 0) { return true; }
                if (other > this) {
                    if (diff / (Math.Abs((double)other)) <= EQUAL_THRESH) {
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    if (diff / Math.Abs(((double)this)) <= EQUAL_THRESH) {
                        return true;
                    } else {
                        return false;
                    }
                }
            }
            //otherwise they are both big numbers
            if (_exp == other._exp) {
                return Math.Abs(_man - other._man) < EPSILON;
            }
            return false;
        }
        //this also apparently important?
        public override int GetHashCode() => HashCode.Combine(_small, _man, _exp, _isLarge);
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

                return new BigNum(double.Parse(parts[0]), int.Parse(parts[1]));
            }
            //case 2: smaller decimals. as it is it just yeets fractionals and turns doubles to longs
            if (s.Contains('.')) {
                return new BigNum(double.Parse(s));
            }
            //case 3: standard long input
            return new BigNum(long.Parse(s));
        }
        public override string ToString() {
            if (!_isLarge) {
                // 1. Get the standard C# double string
                string s = _small.ToString(System.Globalization.CultureInfo.InvariantCulture);

                // 2. THE FIX: If C# used a capital 'E', replace it with lowercase 'e'!
                return s.Replace("E", "e");
            }

            // Explicitly format the double using InvariantCulture
            string formattedMan = _man.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
            return $"{formattedMan}e{_exp}";
        }
        //this one always returns the full 
        public string ToScientific(bool truncate) {
            string format = truncate ? "F2" : "G";
            string formattedMan = _man.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
            return $"{formattedMan}e{_exp}";
        }
        //these 2 lines allow us to natively/automatically turn numbers into BigNums
        public static implicit operator BigNum(long v) => new BigNum(v);
        public static implicit operator BigNum(double v) => new BigNum(v, 0);
    }
}
