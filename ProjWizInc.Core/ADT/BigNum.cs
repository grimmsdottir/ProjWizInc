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

        private const long THRESHOLD = 1_000_000_000_000_000L; // 1e15
        private const int THRESH_POW = 15;
        private const double EPSILON = 1e-15;

        public BigNum(long value) {
            _isLarge = false;
            _small = value;
            _man = 0;
            _exp = 0;
        }
        public BigNum(int value) : this((long)value) { }
        public BigNum(double man, long exp) {
            if (Math.Abs(man) < EPSILON) {
                this = new BigNum(0);
                return;
            }
            int shift = (int)Math.Floor(Math.Log10(Math.Abs(man)));
            double normMan = man / Math.Pow(10, shift);
            long normExp = exp + shift;
            if (normMan > 0 && normExp < THRESH_POW) {
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
            //small + small
            if (!a._isLarge && !b._isLarge) {
                long sum = a._small + b._small;
                if (Math.Abs(sum) < THRESHOLD) {
                    //= still small
                    return new BigNum(sum);
                } else {
                    //= big now
                    return new BigNum(0);
                }
            }
            //if either numbers are big, we have to do the slower operation
            //if a is small, we just use the small, otherwise we take their man and exp
            double man1;
            long exp1;
            if (a._isLarge) {
                man1 = a._man;
                exp1 = a._exp;
            } else {
                man1 = a._small;
                exp1 = 0;
            }
            //same with b
            double man2;
            long exp2;
            if (b._isLarge) {
                man2 = b._man;
                exp2 = b._exp;
            } else {
                man2 = b._small;
                exp2 = 0;
            }
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
            if (diff < THRESHOLD) { 
                return new BigNum(man1,exp1);
            } else {
                return new BigNum(man1 + (man2 / Math.Pow(10, diff)), exp1);
            }
        }
        public static BigNum operator *(BigNum a, BigNum b) {
            double man1;
            long exp1;
            if (a._isLarge) {
                man1 = a._man;
                exp1 = a._exp;
            } else {
                man1 = a._small;
                exp1 = 0;
            }
            //same with b
            double man2;
            long exp2;
            if (b._isLarge) {
                man2 = b._man;
                exp2 = b._exp;
            } else {
                man2 = b._small;
                exp2 = 0;
            }
            return new BigNum(man1 * man2, exp1 + exp2);
        }
        //these 2 lines allow us to natively/automatically turn numbers into BigNums
        public static implicit operator BigNum(long v) => new BigNum(v);
        public static implicit operator BigNum(double v) => new BigNum(v, 0);
    }
}
