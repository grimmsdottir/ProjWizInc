using ProjWizInc.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.States {
    public class MainGameState {
        public static MainGameState Instance { get; } = new MainGameState() {};
        private MainGameState() { }
        

    }
    
}
