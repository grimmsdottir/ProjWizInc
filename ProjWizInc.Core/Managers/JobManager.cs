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
    internal class JobState {
        public int ActiveJobID { get; internal set; } = -1;
        //we keep a cache of the active job's def, so we only need to search it up whenver we change jobs
        public JobDefinition? ActiveJobDef { get; internal set; } = null;
        //we also keep a cache of any components and their state vars like ticks and xp
        public RequiresTicksFeature? JobTicksRequired { get; internal set; } = null;
        public BigNum Ticks { get; internal set; } = 0;
        public PayoutFeature? JobPayout { get; internal set; } = null;
        //a handy reset function for nulling everything
        public void Reset() {
            ActiveJobID = -1;
            ActiveJobDef = null;
            JobTicksRequired = null;
            Ticks = 0;
            JobPayout = null;
        }
    }
    internal class JobManager {
        //our few state fields. it should be ok to leave them here, because 
        
        private readonly EventBroker _events;
        private readonly DefinitionManager _defs;
        private readonly JobState _state;
        //we use -1 like null basically, if its -1 that means that nothings active
        
        public JobManager(EventBroker events, DefinitionManager defs) {
            _events = events;
            _defs = defs;
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
                
            }
        }
    }
}
