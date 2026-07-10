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
        private readonly ResourceState _resources;
        private readonly List<JobState> _state = [];
        public JobState? ActiveJob { get; private set; }
        public JobManager(EventBroker events, ResourceManager resourceManager) {
            _events = events;
            _resources = resourceManager.State;
            _events.Subscribe<UpdateLogicEvent>(Update);
        }
        public void ToggleTask(string id) {
            if (ActiveJob?.Id == id) {
                ActiveJob = null;
            } else {
                ActiveJob = _state.Find(j => j.Id == id);
                if (ActiveJob == null) { 
                    //log this as an error
                }
            }
        }
        public void Update(UpdateLogicEvent e) {
            if (ActiveJob == null) { return; }
            ActiveJob.Progress++;
            if (ActiveJob.Progress >= ActiveJob.TicksRequired) { 
                ActiveJob.Progress -= ActiveJob.TicksRequired;
            }

        }
    }
}
