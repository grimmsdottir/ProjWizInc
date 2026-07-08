using System;
using System.Collections.Generic;

namespace ProjWizInc.Core
{
    /*
     * So the way the EventBroker works is basically as a broker
     * During bootup, Module A Subcribes to the EventBroker with an EventName and Function
     * IE: The renderer subscribes "RenderFramePls", RenderFrame(), and waits
     * During runtime, Module B publishes to the EventBroker with an EventName and Argument
     * IE: The tick manager decides that it is time for the next frame to be rendered, and publishes
     * "RenderFramePls",SomeData.
     * The EventBroker then sees that "RenderFramePls" is in the subscriptions, and runs RenderFrame()
     * TLDR: Subscription is "do thing if this event" and publishing is "this event happened"
     */
    /*
     * 2nd pass, now with funky typing instead of plain strings for event names
     * our subscriptions are now based on Type and Delegate
     * Type is typeOf(something), and Delegate is the parent of Action, because idk
     * 
     */
    public class EventBroker{
        private readonly Dictionary<Type, List<Delegate>> _subscribers = [];

        public void Subscribe<T>(Action<T> handler) {
            var type = typeof(T);
            //like before, we check if the Event(type) already exists, and create it if it doesnt
            if (!_subscribers.ContainsKey(type)) {
                _subscribers[type] = [];
            }
            //then we add a new Function(handler) used to be called callback
            _subscribers[type].Add(handler); 
        }
        public void Unubscribe<T>(Action<T> handler) {
            var type = typeof(T);
            if (_subscribers.ContainsKey(type)) {
                _subscribers[type].Remove(handler);
                if (_subscribers[type].Count == 0) {
                    _subscribers.Remove(type);
                }
            } else {
                //log if we tried to unsubscribe from something that was never subscribed
            }
        }
        public void Publish<T>(T eventArgs) {
            if (_subscribers.TryGetValue(typeof(T), out var handlers)) {
                foreach (var handler in handlers) {
                    ((Action<T>)handler)(eventArgs);
                }
            } else {
                //log if we get an empty publish, IE: we published an event we arent listening out for
            }
        }


    }
}
