using ProjWizInc.Core.ADT;
using ProjWizInc.Core.Data;
using ProjWizInc.Core.Definitions;
using ProjWizInc.Core.Events;
using ProjWizInc.Core.States.Interfaces;
using ProjWizInc.Core.States.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Managers {


    public class EconomyManager : IWriteEconomy {
        private readonly EventManager _events;
        private readonly EconomyState _state;
        private readonly object _lock = new();
        public EconomyManager(EventManager events, EconomyState state) {
            _state = state; ;
            _events = events;
            _events.Subscribe<UpdateLogicEvent>(Update);
            _events.Subscribe<ResourceGainedEvent>(AddResource);
        }
        public EconomyState State => _state;
        //we have to use a small paremeter here, even if we dont need anything, because of signalling reasons
        //but if we change UpdateFrameEvent with parameters later, we can check it with e
        public void Update(UpdateLogicEvent e) {

        }
        private void AddResource(ResourceGainedEvent e) {
            lock (_lock) {
                if (e.Id >= _state.Resources.Length) {
                    throw new IndexOutOfRangeException("Attempted to access resource at index " + e.Id + ". There are" +
                        "only " + _state.Resources.Length + " resource types");
                }
                _state.Resources[e.Id] += e.Amount;
            }
        }
        public BigNum GetResource(int resourceID) {
            lock (_lock) {
                if (resourceID >= _state.Resources.Length) {
                    throw new IndexOutOfRangeException("Attempted to access resource at index " + resourceID + ". There are" +
                        "only " + _state.Resources.Length + " resource types");
                }
                return _state.Resources[resourceID];
            }
        }
        //single resource adjustment, returns true if the adjustment was successful, false if it would have resulted in a negative resource
        public bool TryAdjustResources(int resourceId, in BigNum resourceAmount) {
            lock (_lock) {
                if (resourceId >= _state.Resources.Length) {
                    throw new IndexOutOfRangeException("Attempted to access resource at index " + resourceId + ". There are" +
                        "only " + _state.Resources.Length + " resource types");
                }
                //TODO: maybe allow for negative resources, but only up to a certain amount
                if (_state.Resources[resourceId] + resourceAmount < 0) {
                    return false;
                } else {
                    _state.Resources[resourceId] += resourceAmount;
                    return true;
                }
            }
        }
        //multi resource adjustment, returns true if the adjustment was successful, false if it would have resulted in a negative resource
        public bool TryAdjustResources(ReadOnlySpan<ResourceAdjustment> adjustments) {
            lock (_lock) {
                if (_state.Resources == null) {
                    throw new InvalidOperationException("Economy resources array is null.");
                }
                //phase 1: validate all adjustments
                for (int i = 0; i < adjustments.Length; i++) {
                    var adjustment = adjustments[i];
                    if (adjustment.ResourceId >= _state.Resources.Length) {
                        throw new IndexOutOfRangeException("Attempted to access resource at index " + adjustment.ResourceId + ". There are" +
                            "only " + _state.Resources.Length + " resource types");
                    }
                    if (_state.Resources[adjustment.ResourceId] + adjustment.ResourceAmount < 0) {
                        return false;
                    }
                }
                //phase 2: apply all adjustments
                for (int i = 0; i < adjustments.Length; i++) {
                    var adjustment = adjustments[i];
                    _state.Resources[adjustment.ResourceId] += adjustment.ResourceAmount;
                }
                return true;
            }
        }
    }
}
