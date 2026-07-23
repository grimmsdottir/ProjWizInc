using ProjWizInc.Core.Data.ADT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.States.Records {
    public class EconomyRecord {
        //changed the dictionary to using ints instead of strings for faster searching
        public DualKeyMap<BigNum> Resources { get; set; }
    }
}
