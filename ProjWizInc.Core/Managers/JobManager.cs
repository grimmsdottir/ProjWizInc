using ProjWizInc.Core.Events;
using ProjWizInc.Core.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Managers {
    internal class JobManager {
        private readonly EventBroker _events;
        private readonly ResourceState _resources;
        private readonly List<JobState> _state = [];
        private JobState _currentJob;
        public string? ActiveTaskId { get; private set; } = null;
        public JobManager(EventBroker events, ResourceManager resourceManager) {
            _events = events;
            _resources = resourceManager.State;
            _events.Subscribe<UpdateLogicEvent>(Update);
        }
        public void ToggleTask(string id) {
            // If the clicked task is already active, stop it.
            // Otherwise, set it as the active task (this automatically stops the previous one).
            ActiveTaskId = (ActiveTaskId == id) ? null : id;
        }
        public void Update(UpdateLogicEvent e) {
            if (ActiveTaskId == null) { return; }
            JobState? activeTask = null;
            if (ActiveTaskId != _currentJob.Id) {
                foreach (var task in _state) {
                    if (task.Id == ActiveTaskId) {
                        activeTask = task; 
                        _currentJob = task;
                        break;
                    }
                }
                if (activeTask == null) { return; }
            } else {
                activeTask = _currentJob;
            }
            activeTask.Progress++;
            if (activeTask.Progress >= activeTask.TicksRequired) {
                activeTask.Progress -= activeTask.TicksRequired;
                switch (activeTask.RewardType) { 
                }
            }

        }
    }
}
