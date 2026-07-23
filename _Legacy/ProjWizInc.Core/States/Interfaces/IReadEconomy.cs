using ProjWizInc.Core.ADT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.States.Interfaces {
    public interface IReadEconomy : IRead {
        int ResourceCount { get; }
        BigNum GetResourceAmount(int resourceId);
    }
}
