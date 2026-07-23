
using ProjWizInc.Engine.Data.Configs;
using ProjWizInc.Engine.Simulation.Tickers.Interfaces;
using ProjWizInc.Engine.State.Records.Interfaces;
using System.Diagnostics;
using System.IO.Pipelines;

namespace ProjWizInc.Engine.Simulation.Core {
    public class HeartbeatManager {
        private double _timeAccumalator = 0;
        private double _presentationTimer = 0;
        private readonly Stopwatch _stopwatch;
        private CancellationTokenSource _cts;
        private IWriteTime _timeWriter;
        public int OfflineTicks { get; set; }
        public int FastForawardFactor { get; set; }
        public HeartbeatManager(
            IWriteTime timeWriter
            ) {
            _stopwatch = new Stopwatch();
            _cts = null;
            FastForawardFactor = 1;

            _timeWriter = timeWriter;
        }
        public void Start() {
            if (_cts != null) {
                throw new InvalidOperationException("Critical Error: Attempted to start HeartbeatManager while it is in progress.");
            }
            _cts = new CancellationTokenSource();
            _stopwatch.Start();
            Task.Run(() => GameLoop(_cts.Token));
        }
        private async Task GameLoop(CancellationToken token) {
            using PeriodicTimer periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(GlobalConfigs.TICK_LENGTH));
            while (!token.IsCancellationRequested && await periodicTimer.WaitForNextTickAsync(token)) {
                double secondsPassed = _stopwatch.Elapsed.TotalSeconds;
                bool hasTicked = false; 
                _stopwatch.Restart();
                if (secondsPassed > GlobalConfigs.MAX_BURST) {
                    secondsPassed = GlobalConfigs.MAX_BURST;
                }
                _timeAccumalator += secondsPassed;
                _presentationTimer += secondsPassed;
                //TODO: create some sort of thing that can snapshot 
                while (_timeAccumalator >= GlobalConfigs.TICK_LENGTH) {
                    PreTick();
                    Tick();
                    PostTick();
                    _timeAccumalator -= GlobalConfigs.TICK_LENGTH;
                    if (OfflineTicks > 0) {
                        int extraTicks = FastForawardFactor - 1;
                        for (int i = 0; i < extraTicks; i++) {
                            if (OfflineTicks > 0) {
                                PreTick();
                                Tick();
                                PostTick();
                                OfflineTicks--;
                            }
                        }
                    }
                    hasTicked = true;
                }
                if (hasTicked) {
                    //TODO: commit events
                }
                if (_presentationTimer >= GlobalConfigs.TARGET_PPS) {
                    Present();
                    _presentationTimer = 0;
                }
            }
        }
        //stuff that must be done BEFORE others
        private void PreTick() {
            _timeWriter.AdvanceTime();

        }
        //whatever stuff
        private void Tick() {
            
        }
        //stuff that must be done AFTER others
        private void PostTick() {

        }
        private void Present() {

        }
    }
}
