using ProjWizInc.Core.ADT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Events {
    public readonly record struct UpdateRenderEvent();
    public readonly record struct UpdateLogicEvent();
    public readonly record struct AppStartedEvent(string Message);
    public readonly record struct GameStartedEvent();
    public readonly record struct TimeAdvancedEvent(long CurrentTime);
    public readonly record struct ResourceGainedEvent(int Id, BigNum Amount);
}
