using ProjWizInc.Core.ADT;
using ProjWizInc.Core.Definitions;
using ProjWizInc.Core.Definitions.Features;
using ProjWizInc.Core.Events;
using ProjWizInc.Core.States.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace ProjWizInc.Core.Managers {
    
    public class JobManager {
        private readonly EventManager _events;
        private readonly JobState _state;
        private readonly DefinitionManager _defs;
        //we use -1 like null basically, if its -1 that means that nothings active
        
        public JobManager(EventManager events, JobState state, DefinitionManager definitionManager) {
            _events = events;
            _defs = definitionManager;
            _state = state;
            _events.Subscribe<UpdateLogicEvent>(Update);
        } 
        public void ToggleJob(int jobId) {
            //basically if we click the current job, it just stops it, but if we click something else, we 
            //move to that one instead
            if (_state.ActiveJobID == jobId) {
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
                _state.ActiveJobID = jobId;
                _state.ActiveJobDef = _defs.GetDefinition<JobDefinition>(jobId);
                _state.JobTicksRequired = _state.ActiveJobDef.GetFeature< RequiresTicksFeature>();
                _state.Ticks = 0;
                _state.JobPayout = _state.ActiveJobDef.GetFeature<PayoutFeature>();
            }
        }
        public void Update(UpdateLogicEvent e) {
            if (_state.ActiveJobID == -1) { return; }
            if (_state.JobTicksRequired != null) {
                _state.Ticks++;
                if (_state.Ticks >= _state.JobTicksRequired.RequiredTicks) {
                    _state.Ticks -= _state.JobTicksRequired.RequiredTicks;
                    CompleteJob();
                }
            }
        }
        private void CompleteJob() {
            if (_state.JobPayout != null) {
                foreach (ResourcePayoutEntry entry in _state.JobPayout.PayoutEntries) {
                    int id = entry.ResourceID;
                    BigNum amount = entry.Amount;
                    _events.Publish(new ResourceGainedEvent(id,amount));
                }
            }
            _events.Publish(new JobCompleted(_state.ActiveJobID));
        }
    }
}
