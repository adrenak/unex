using UnityEngine;
using UniPrep.Utils;

public class MonoBehaviourHelperExample : MonoBehaviour {
    class SubscriberExample {
        public SubscriberExample() {
            MonoBehaviourHelper.Instance.updateEvent += UnityUpdateEvent;
            MonoBehaviourHelper.Instance.fixedUpdateEvent += UnityFixedUpdateEvent;
        }

        private void UnityFixedUpdateEvent() {
            Debug.Log("FixedUpdate() after " + Time.fixedDeltaTime + " seconds");
        }

        private void UnityUpdateEvent() {
            Debug.Log("Update() after" + Time.deltaTime + " seconds");
        }

        public void Unsubscribe() {
            MonoBehaviourHelper.Instance.updateEvent -= UnityUpdateEvent;
            MonoBehaviourHelper.Instance.fixedUpdateEvent -= UnityFixedUpdateEvent;
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