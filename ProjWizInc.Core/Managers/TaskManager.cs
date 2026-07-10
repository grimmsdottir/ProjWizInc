using ProjWizInc.Core.Events;
using ProjWizInc.Core.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Managers {
    internal class TaskManager {
        private readonly EventBroker _events;
        private readonly ResourceState _resources;
        private readonly List<TaskState> _state = [];
        public string? ActiveTaskId { get; private set; } = null;
        public TaskManager(EventBroker events, ResourceManager resourceManager) {
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
            TaskState? activeTask = null ;
            foreach (var task in _state) { 
                if (task.Id == ActiveTaskId) {
                    activeTask = task; break;
                }
            }
            if (activeTask == null) { return; };
            activeTask.Progress++;
            if (activeTask.Progress >= activeTask.TicksRequired) {
                activeTask.Progress -= activeTask.TicksRequired;
                switch (activeTask.RewardType) { 
                }
            }

        }
    }
}
