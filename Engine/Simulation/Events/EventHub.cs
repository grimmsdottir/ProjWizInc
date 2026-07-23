namespace ProjWizInc.Engine.Simulation.Events {
    public class EventHub : ISubscribeEvents,IPublishEvents{
        private readonly Dictionary<Type, Delegate[]> _subscribers = [];
        public EventHub() { }
        public void Subscribe<T>(Action<T> action) {
            Type type = typeof(T);
            Delegate[] oldDelegates;
            //this check is to see if there are already any delegates assigned to this event
            if (!_subscribers.TryGetValue(type, out oldDelegates)) {
                //if there arent, we make a new one
                _subscribers[type] = new Delegate[] { action };
            } else {
                Delegate[] newDelegates = new Delegate[oldDelegates.Length + 1];
                Array.Copy(oldDelegates, newDelegates, oldDelegates.Length);
                newDelegates[^1] = action;
                _subscribers[type] = newDelegates;
            }
        }
        public void Unsubscribe<T>(Action<T> action) {
            Type type = typeof(T);
            Delegate[] oldDelegates;
            if (!_subscribers.TryGetValue(type, out oldDelegates)){
                throw new InvalidOperationException("Attempted to unsubscribe an action from "+type.Name+", which has now related events. Did unsubscribe from something you havent subscribed for?");
            }
            int index = Array.IndexOf(oldDelegates, action);
            if (index < 0) {
                throw new InvalidOperationException("Attempted to unsubscribe and action that is not subscribed to "+type.Name);
            }
            //if this is the last action subscribed to this event, we remove the type entirely
            if (oldDelegates.Length == 1) {
                _subscribers.Remove(type);
            } else {
                Delegate[] newDelegates = new Delegate[oldDelegates.Length - 1];
                if (index > 0) {
                    Array.Copy(oldDelegates, 0, newDelegates, 0, index);
                }
                if (index < oldDelegates.Length - 1) {
                    Array.Copy(oldDelegates, index + 1, newDelegates, index, oldDelegates.Length - index - 1);
                }
                _subscribers[type] = newDelegates;
            }

        }
        public void Publish<T>(T eventArgs) {
            Delegate[] actions;
            if (_subscribers.TryGetValue(typeof(T), out actions)) {
                for (int i = 0; i < actions.Length; i++) {
                    Action<T> action = (Action<T>)actions[i];
                    action(eventArgs);
                }
            } else {
                //log that we published an event that has no subscribers
            }
        }
    }
}
