using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Simulation.Events {
    public interface ISubscribeEvents {
        public void Subscribe<T>(Action<T> action);
        public void Unsubscribe<T>(Action<T> action);
    }
}
