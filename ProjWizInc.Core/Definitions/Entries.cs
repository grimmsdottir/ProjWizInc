using ProjWizInc.Core.ADT;
using ProjWizInc.Core.Definitions.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Definitions {
    //this entry can 
    public class ResourcePayoutEntry : EntryInterface {
        public string ResourceID { get; set; }
        public BigNum Ammount { get; set; }
    }
}
