using System;
using System.Collections.Generic;

namespace ProjWizInc.Core.Managers {
    /*
     * So the way the EventBroker works is basically as a broker
     * During bootup, Module A Subcribes to the EventBroker with an EventName and Function
     * IE: The renderer subscribes "RenderFramePls", RenderFrame(), and waits
     * During runtime, Module B publishes to the EventBroker with an EventName and Argument
     * IE: The tick manager decides that it is time for the next frame to be rendered, and publishes
     * "RenderFramePls",SomeData.
     * The EventBroker then sees that "RenderFramePls" is in the subscriptions, and runs RenderFrame()
     * TLDR: Subscription is "do thing if this event" and publishing is "this event happened"
     *
     * 2nd pass, now with funky typing instead of plain strings for event names
     * our subscriptions are now based on Type and Delegate
     * Type is typeOf(something), and Delegate is the parent of Action, because idk
     * Also, upgraded to a singleton so we have global access to all events for everyone
     * 
     * 3rd pass, apparently using new 160 times a second causes problems for the GC, so we swap to an array instead
     * it bypasses collection change errors by using an array. if any changes happen, the currently running publish
     * will continue working with the old array, and use the new array next loop
     */
    public class EventManager{
        private readonly Dictionary<Type, Delegate[]> _subscribers = [];
        //we use this funky thing as a lock to make it thread safe and prevent race conditions
        private readonly object _lock = new();
        public void Subscribe<T>(Action<T> handler) {
            var type = typeof(T);
            lock (_lock) {
                //like before, we check if the Event(type) already exists, and create it if it doesnt
                if (!_subscribers.TryGetValue(type, out var oldArray)) {
                    oldArray = Array.Empty<Delegate>();
                }
                //then we add a new Function(handler) used to be called callback
                //but now in the 3rd pass, we aren using a list anymore, so we gotta do an old school manual array
                //expansion
                var newArr = new Delegate[oldArray.Length + 1];
                Array.Copy(oldArray, newArr, oldArray.Length);
                newArr[^1] = handler;
                _subscribers[type] = newArr;
                //this does have the old heap problem, but subs dont run 160hz
            }

        }
        public void Unsubscribe<T>(Action<T> handler) {
            var type = typeof(T);
            lock (_lock) {
                if (_subscribers.TryGetValue(type, out var oldArray)) {
                    // Find where this handler lives in our array
                    int index = Array.IndexOf(oldArray, handler);
                    if (index >= 0) {
                        // If it's the last subscriber standing, remove the event completely
                        if (oldArray.Length == 1) {
                            _subscribers.Remove(type);
                        } else {
                            // Otherwise, allocate a new array (-1 size) and skip the removed handler
                            var newArray = new Delegate[oldArray.Length - 1];

                            // Copy everything before the removed item
                            if (index > 0) {
                                Array.Copy(oldArray, 0, newArray, 0, index);
                            }
                            // Copy everything after the removed item
                            if (index < oldArray.Length - 1) {
                                Array.Copy(oldArray, index + 1, newArray, index, oldArray.Length - index - 1);
                            }

                            _subscribers[type] = newArray; // Atomic pointer swap
                        }
                    }
                } else {
                    // log if we tried to unsubscribe from something that was never subscribed
                }
            }
            
        }
        public void Publish<T>(T eventArgs) {
            Delegate[]? handlers;
            lock (_lock) {
                _subscribers.TryGetValue(typeof(T), out handlers);
            }
            // Iterates directly over the array for zero-allocation
            if (handlers != null) {
                foreach (var h in handlers) {
                    ((Action<T>)h)(eventArgs);
                }
            }

        }


    }
}
