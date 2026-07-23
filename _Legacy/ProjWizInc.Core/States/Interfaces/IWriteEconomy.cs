using ProjWizInc.Core.ADT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.States.Interfaces {
    public struct ResourceAdjustment {
        public int ResourceId;
        public BigNum ResourceAmount;
        public ResourceAdjustment(int resourceId, in BigNum resourceAmount) {
            ResourceId = resourceId;
            ResourceAmount = resourceAmount;
        }
    }
    public interface IWriteEconomy : IWrite{
        //try to adjust 1 resource by amount, return false if it fails
        bool TryAdjustResources(int resourceId, in BigNum resourceAmount);
        //like above, but for multiple resources, return false if any of the adjustments fail
        //if any would fail, all fail and no adjustments are made
        bool TryAdjustResources(ReadOnlySpan<ResourceAdjustment> adjustments);

    }
}
