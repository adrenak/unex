using UnityEngine;
using UniPrep.Extensions;

namespace UniPrep.Utils {
    public class MonoBehaviourSingleton : MonoBehaviour {
        static MonoBehaviourSingleton instance;
        public static MonoBehaviourSingleton Instance {
            get {
                if (instance.IsNotNull())
                    return instance;
                GameObject go = new GameObject("MonoBehaviourSingleton");
                instance = go.AddComponent<MonoBehaviourSingleton>();
                return instance;
            }
        }

        public delegate void Callback();
        public event Callback updateEvent;
        public event Callback fixedUpdateEvent;

        private void Update() {
            if (updateEvent != null) updateEvent();
        }

        private void FixedUpdate() {
            if (fixedUpdateEvent != null) fixedUpdateEvent();
        }
    }
}
