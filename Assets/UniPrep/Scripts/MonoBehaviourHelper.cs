using UnityEngine;

namespace UniPrep {
    public class MonoBehaviourHelper : MonoBehaviour {
        static MonoBehaviourHelper instance;
        public static MonoBehaviourHelper Instance {
            get {
                if (instance.IsNotNull())
                    return instance;
                GameObject go = new GameObject("MonoBehaviourHelper");
                instance = go.AddComponent<MonoBehaviourHelper>();
                return instance;
            }
        }

        public delegate void Callback();
        public event Callback UpdateEvent;
        public event Callback FixedUpdateEvent;

        private void Update() {
            if (UpdateEvent != null) UpdateEvent();
        }

        private void FixedUpdate() {
            if (FixedUpdateEvent != null) FixedUpdateEvent();
        }
    }
}
