using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;
using ProjWizInc.Core.Data.ADT;
using ProjWizInc.Core.Data.Entries;
using ProjWizInc.Core.Data.Blueprints.Specifications;
using ProjWizInc.Core.Data.Events;
using ProjWizInc.Core.States.Records;
using ProjWizInc.Core.Simulation.Events;
using ProjWizInc.Core.Simulation.Registries;
using ProjWizInc.Core.Data.Blueprints.Blueprints;

namespace ProjWizInc.Core.Simulation.Tickers {
    
    public class JobTicker {
        private readonly EventHub _events;
        private readonly JobRecord _state;
        private readonly BlueprintRegistry _defs;
        //we use -1 like null basically, if its -1 that means that nothings active
        public JobTicker(EventHub events, JobRecord state, BlueprintRegistry definitionManager) {
            _events = events;
            _defs = definitionManager;
            _state = state;
            if (_state.ActiveJobId != -1) {
                CacheState(_state.ActiveJobId);
            }
            _events.Subscribe<UpdateLogicEvent>(Update);
        }
        public JobRecord State => _state;
        public void ToggleJob(int jobId) {
            //basically if we click the current job, it just stops it, but if we click something else, we 
            //move to that one instead
            if (_state.ActiveJobId == jobId) {
                _state.Reset();
            } else {
                CacheState(jobId);
            }            
        }
        private void CacheState(int jobId) {
            _state.Reset();
            if (jobId < 0) {
                //todo: log and complain about illegal arguement
                return;
            } else {
                _state.ActiveJobId = jobId;
                _state.ActiveJobDef = _defs.GetDefinition<JobBlueprint>(jobId);
                _state.JobTicksRequired = _state.ActiveJobDef.GetComponent<RequiresTicksSpec>();
                _state.Ticks = 0;
                _state.JobPayout = _state.ActiveJobDef.GetComponent<PayoutSpec>();
            }
        }
        public void Update(UpdateLogicEvent e) {
            if (_state.ActiveJobId == -1) { return; }
            if (_state.JobTicksRequired != null) {
                _state.Ticks++;
                if (_state.Ticks >= _state.JobTicksRequired.RequiredTicks) {
                    _state.Ticks -= _state.JobTicksRequired.RequiredTicks;
                    CompleteJob();
                }
            }
        }
        public void CompleteJob() {
            if (_state.JobPayout != null) {
                foreach (ResourceEntry entry in _state.JobPayout.PayoutEntries) {
                    int id = entry.ResourceID;
                    BigNum amount = entry.Amount;
                    _events.Publish(new ResourceGainedEvent(id,amount));
                }
            }
            _events.Publish(new JobCompleted(_state.ActiveJobId));
        }
    }
}
