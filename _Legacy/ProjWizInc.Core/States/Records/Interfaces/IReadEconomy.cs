using ProjWizInc.Core.Data.ADT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.States.Records.Interfaces {
    public interface IReadEconomy : IRead {
        int ResourceCount { get; }
        BigNum GetResourceAmount(int resourceId);
    }
}
