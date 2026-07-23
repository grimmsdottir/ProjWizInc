using ProjWizInc.Engine.Data.ADT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.State.Records.Interfaces {
    public interface IReadEconomy {
        public BigNum GetResource(int id);
        public int GetResourceCount();
        public void CopyTo(EconomyRecord copy);
    }
}
