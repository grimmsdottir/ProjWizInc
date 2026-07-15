using ProjWizInc.Core.ADT;
using ProjWizInc.Core.Definitions;
using ProjWizInc.Core.Definitions.Features;
using ProjWizInc.Core.Events;
using ProjWizInc.Core.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace ProjWizInc.Core.Managers {
    public class JobState {
        public int ActiveJobID { get; set; } = -1;
        //we keep a cache of the active job's def, so we only need to search it up whenver we change jobs
        public JobDefinition? ActiveJobDef { get; set; } = null;
        //we also keep a cache of any components and their state vars like ticks and xp
        public RequiresTicksFeature? JobTicksRequired { get; set; } = null;
        public BigNum Ticks { get; set; } = 0;
        public PayoutFeature? JobPayout { get; set; } = null;
        //a handy reset function for nulling everything
        public void Reset() {
            ActiveJobID = -1;
            ActiveJobDef = null;
            JobTicksRequired = null;
            Ticks = 0;
            JobPayout = null;
        }
    }
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
