using System;
using System.Collections.Generic;

namespace ProjWizInc.Core
{
    public class EventBroker{
        private readonly Dictionary<string, List<Action<object>>> _subscriptions = [];
        public void Subscribe(String eventName, Action<Object> callback){
            if (!_subscriptions.ContainsKey(eventName)){
                _subscriptions[eventName] = [];
            }
            _subscriptions[eventName].Add(callback);
        }
        public void Publish(String eventName, object data) {
            if (_subscriptions.ContainsKey(eventName)){
                foreach (var callback in _subscriptions[eventName]){
                    callback?.Invoke(data);
                }
            }
        }

    }
}
