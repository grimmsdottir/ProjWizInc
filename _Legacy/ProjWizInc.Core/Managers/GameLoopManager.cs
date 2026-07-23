using ProjWizInc.Core.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ProjWizInc.Core.Data.Globals;

namespace ProjWizInc.Core.Managers {
    
    public class GameLoopManager {
        
        private double _timeAccumulator = 0;
        private readonly Stopwatch _stopwatch;
        //this CTS thing is a sort of remote that is used to gracefully end a looping async thread
        private CancellationTokenSource _cts;
        private readonly EventManager _events;
        public GameLoopManager(EventManager events) {
            _stopwatch = new Stopwatch();
            _events = events;

        }
        public void Start() {
            //if it is not null, that means that the Manager has already started, so this stops duplicating runs
            if (_cts != null) {
                //todo: log it
                return;
            }
            _cts = new CancellationTokenSource();
            _stopwatch.Start();
            //this one line kicks off the whole actual loop
            Task.Run(() => RunLoopAsync(_cts.Token));
        }
        public void Init() {
        }
        public void Stop() {
            //???
            _cts?.Cancel();
            _stopwatch.Stop();
            _cts = null;
        }
        private async Task RunLoopAsync(CancellationToken token) {
            //PeriodicTimer is another clock timer thing that is used for async and awaiting stuff
            //less accurate than Stopwatch, but Stopwatch cant control CPU flow
            //even though its less accurate, we dont need it to be super accurate because we have the accumalator
            //to catch up to it and stuff
            using var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(FRAME_LENGTH_MS));
            while (!token.IsCancellationRequested && await periodicTimer.WaitForNextTickAsync(token)) {
                double secondsPassed = _stopwatch.Elapsed.TotalSeconds;
                _stopwatch.Restart();
                //we cap the time passed so we dont melt the CPU with our CPU burst later
                if (secondsPassed > 0.25) secondsPassed = 0.25;
                _timeAccumulator += secondsPassed;
                //this lets us decouple UPS from FPS, 
                while (_timeAccumulator >= TICK_LENGTH) {
                    LogicStep();
                    _timeAccumulator -= TICK_LENGTH;
                }
                PresentationStep();
            }
        }
        private void LogicStep() {
            _events.Publish(new UpdateLogicEvent());
        }
        private void PresentationStep() {
            _events.Publish(new UpdateRenderEvent());
        }
    }
}
