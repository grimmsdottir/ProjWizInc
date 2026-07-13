using ProjWizInc.Core.Definitions;
using ProjWizInc.Core.Events;
using ProjWizInc.Core.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace ProjWizInc.Core.Managers {
    internal class JobManager {
        private readonly EventBroker _events;
        private readonly DefinitionManager _defs;
        private readonly List<JobState> _state = [];
        public JobDefinition? ActiveJob { get; private set; }
        public JobManager(EventBroker events, DefinitionManager defs) {
            _events = events;
            _defs = defs;
            _events.Subscribe<UpdateLogicEvent>(Update);
        }
        public void ToggleTask(string id) {
            //need to refactor this to use the new definition manager later
            /*
            if (ActiveJob?.Id == id) {
                ActiveJob = null;
            } else {
                ActiveJob = _state.Find(j => j.Id == id);
                if (ActiveJob == null) { 
                    //log this as an error
                }
            }
            */
        }
        public void Update(UpdateLogicEvent e) {
            if (ActiveJob == null) { return; }
            //we will need to check what features the job has and resolve them
            /*
            ActiveJob.Progress++;
            if (ActiveJob.Progress >= ActiveJob.TicksRequired) { 
                ActiveJob.Progress -= ActiveJob.TicksRequired;
                CompleteJob();
            }
            */
        }
        private void CompleteJob() {
            //int id = 
            //_events.Publish(ResourceGainedEvent());
        }
    }
}
