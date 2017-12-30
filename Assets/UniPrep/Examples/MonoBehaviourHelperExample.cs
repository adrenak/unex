using UnityEngine;
using UniPrep.Utils;

public class MonoBehaviourSingletonExample : MonoBehaviour {
    class SubscriberExample {
        public SubscriberExample() {
            MonoBehaviourSingleton.Instance.updateEvent += UnityUpdateEvent;
            MonoBehaviourSingleton.Instance.fixedUpdateEvent += UnityFixedUpdateEvent;
        }

        private void UnityFixedUpdateEvent() {
            Debug.Log("FixedUpdate() after " + Time.fixedDeltaTime + " seconds");
        }

        private void UnityUpdateEvent() {
            Debug.Log("Update() after" + Time.deltaTime + " seconds");
        }

        public void Unsubscribe() {
            MonoBehaviourSingleton.Instance.updateEvent -= UnityUpdateEvent;
            MonoBehaviourSingleton.Instance.fixedUpdateEvent -= UnityFixedUpdateEvent;

        }
    }

    SubscriberExample sub;

    private void Start() {
        sub = new SubscriberExample();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) 
            sub.Unsubscribe();
    }
}