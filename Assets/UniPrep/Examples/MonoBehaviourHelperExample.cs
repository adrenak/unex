using UnityEngine;
using UniPrep;

public class MonoBehaviourHelperExample : MonoBehaviour {
    class SubscriberExample {
        public SubscriberExample() {
            MonoBehaviourHelper.Instance.UpdateEvent += UnityUpdateEvent;
            MonoBehaviourHelper.Instance.FixedUpdateEvent += UnityFixedUpdateEvent;
        }

        private void UnityFixedUpdateEvent() {
            Debug.Log("FixedUpdate() after " + Time.fixedDeltaTime + " sec");
        }

        private void UnityUpdateEvent() {
            Debug.Log("Update() after" + Time.deltaTime + " sec");
        }

        public void Unsubscribe() {
            MonoBehaviourHelper.Instance.UpdateEvent -= UnityUpdateEvent;
            MonoBehaviourHelper.Instance.FixedUpdateEvent -= UnityFixedUpdateEvent;

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