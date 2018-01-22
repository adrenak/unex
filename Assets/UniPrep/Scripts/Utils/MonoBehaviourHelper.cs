using UnityEngine;
using UniPrep.Extensions;

namespace UniPrep.Utils {
    public class MonoBehaviourHelper : MonoBehaviour {
        static MonoBehaviourHelper instance;
        public static MonoBehaviourHelper Instance {
            get {
                if (instance.IsNotNull())
                    return instance;
                GameObject go = new GameObject("MonoBehaviourSingleton");
                instance = go.AddComponent<MonoBehaviourHelper>();
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
