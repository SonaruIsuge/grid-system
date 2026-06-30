using System;
using System.Collections.Generic;
using System.Linq;

namespace SNR_Event
{
    public class EventManager : TSingletonMonoBehaviour<EventManager>
    {
        private static Dictionary<Type, List<Delegate>> actionDictionary;

        /// <summary> A safer way to subscribe to an event. Checks if the instance exists before trying to subscribe. </summary>
        public static void Register<T>(Action<T> callback) where T : CustomEvent
        {
            if (IsApplicationQuiting)
                return;

            var instance = ExistingInstance != null ? ExistingInstance : Instance;

            if (instance == null)
                return;

            instance.RegisterInstance(callback);
        }

        /// <summary> A safer way to unsubscribe from an event. Checks if the instance exists before trying to unsubscribe. </summary>
        public static void Unregister<T>(Action<T> callback) where T : CustomEvent
        {
            if (IsApplicationQuiting)
                return;

            var instance = ExistingInstance != null ? ExistingInstance : Instance;

            if (instance == null)
                return;

            instance.UnregisterInstance(callback);
        }

        /// <summary> A safer way to raise an event. Checks if the instance exists before trying to publish. </summary>
        public static void RaiseEvent<T>(T args) where T : CustomEvent
        {
            if (IsApplicationQuiting)
                return;

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

        private void RegisterInstance<T>(Action<T> callback) where T : CustomEvent
        {
            var eventType = typeof(T);
            actionDictionary ??= new Dictionary<Type, List<Delegate>>();

            if (!actionDictionary.ContainsKey(eventType))
            {
                actionDictionary.Add(eventType, new List<Delegate>());
            }

            if (!actionDictionary[eventType].Contains(callback))
            {
                actionDictionary[eventType].Add(callback);
            }
        }

        private void UnregisterInstance<T>(Action<T> callback) where T : CustomEvent
        {
            if (actionDictionary == null)
                return;

            var type = typeof(T);
            if (!actionDictionary.ContainsKey(type))
                return;

            if (actionDictionary[type].Contains(callback))
            {
                actionDictionary[type].Remove(callback);
            }
        }

        private void RaiseEventInstance<T>(T args) where T : CustomEvent
        {
            if (actionDictionary == null)
                return;

            var type = typeof(T);
            if (actionDictionary.ContainsKey(type))
            {
                var actions = actionDictionary[type];
                foreach (var action in actions.Cast<Action<T>>().ToList())
                {
                    action(args);
                }
            }
        }
    }
}