using ProjWizInc.Engine.Data.ADT;
using ProjWizInc.Engine.Data.Definitions.Defs;
using ProjWizInc.Engine.State.Records.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.State.Records {
    public class EconomyRecord : IReadEconomy {
        public BigNum[] Resources;
        public EconomyRecord() { }
        public EconomyRecord(int resourceCount) {
            Resources = new BigNum[resourceCount];
        }
        public EconomyRecord(BigNum[] resources) {
            Resources = resources;
        }
        public void CopyTo(EconomyRecord copy) {
            System.ReadOnlySpan<BigNum> sourceSpan = Resources.AsSpan();
            System.Span<BigNum> destinationSpan = copy.Resources.AsSpan();

            sourceSpan.CopyTo(destinationSpan);
        }
        public BigNum GetResource(int id) {
            return Resources[id];
        }
        public int GetResourceCount() {
            return Resources.Length;
        }
        public void AdjustResource(int id, BigNum amount) {
            Resources[id] += amount;
        }
        public bool TryAdjustResource(int id, BigNum amount) { 
            if (Resources[id] >= amount) {
                AdjustResource(id, amount);
                return true;
            } else {
                return false;
            }
        }
    }
}
