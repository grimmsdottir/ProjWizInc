using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Simulation.Events {
    public interface IPublishEvents {
        public void Publish<T>(T eventArgs);
    }
}
