using System;
using System.Collections.Generic;
using System.Linq;
using UtilSNR.Common;

namespace SNR_Event
{
    public class EventManager : TSingletonBehaviour<EventManager>
    {
        public delegate void EventDelegate<T>(T args) where T : CustomEvent;

        private static Dictionary<Type, Delegate> actionDictionary = new();

        /// <summary> A safer way to subscribe to an event. Checks if the instance exists before trying to subscribe. </summary>
        public static void Register<T>(EventDelegate<T> callback) where T : CustomEvent
        {
            var instance = ExistingInstance != null ? ExistingInstance : Instance;

            if (instance == null)
                return;

            instance.RegisterInstance(callback);
        }

        /// <summary> A safer way to unsubscribe from an event. Checks if the instance exists before trying to unsubscribe. </summary>
        public static void Unregister<T>(EventDelegate<T> callback) where T : CustomEvent
        {
            var instance = ExistingInstance != null ? ExistingInstance : Instance;

            if (instance == null)
                return;

            instance.UnregisterInstance(callback);
        }

        /// <summary> A safer way to raise an event. Checks if the instance exists before trying to publish. </summary>
        public static void RaiseEvent<T>(T args) where T : CustomEvent
        {
            var instance = ExistingInstance != null ? ExistingInstance : Instance;

            if (instance == null)
                return;

            instance.RaiseEventInstance(args);
        }

        /// <summary> Clear all event subscriptions </summary>
        public static void Clear()
        {
            if (actionDictionary == null)
                return;

            actionDictionary.Clear();
            actionDictionary = null;
        }

        /// <summary> Clear subscriptions for a specific event type </summary>
        public static void ClearSubscriptions<T>() where T : CustomEvent
        {
            var eventType = typeof(T);

            actionDictionary.Remove(eventType);
        } 


        private void RegisterInstance<T>(EventDelegate<T> callback) where T : CustomEvent
        {
            var eventType = typeof(T);

            if (actionDictionary.TryGetValue(eventType, out var existDelegate))
            {
                Delegate.Combine(existDelegate, callback);
            }
            else
            {
                actionDictionary[eventType] = callback;
            }
        }

        private void UnregisterInstance<T>(EventDelegate<T> callback) where T : CustomEvent
        {
            var eventType = typeof(T);
            if(actionDictionary.TryGetValue(eventType, out var existDelegate))
            {
                var newDelegate = Delegate.Remove(existDelegate, callback);

                if(newDelegate != null)
                {
                    actionDictionary[eventType] = newDelegate;
                }
                else
                {
                    actionDictionary.Remove(eventType);
                }
            }
        }

        private void RaiseEventInstance<T>(T args) where T : CustomEvent
        {
            var eventType = typeof(T);
            
            if (actionDictionary.TryGetValue(eventType, out var existDelegate))
            {
                var callback = existDelegate as EventDelegate<T>;
                callback?.Invoke(args);
            }
        }
    }
}