using System;
using UnityEngine;

namespace Adrenak.Unex {
    [DisallowMultipleComponent]
    public class Monitor : MonoBehaviour {
        // OnTriggerEnter
        Action<Collider> m_OnTriggerEnter;
        public void HandleTriggerEnter(Action<Collider> callback) {
            m_OnTriggerEnter = callback;
        }
        void OnTriggerEnter(Collider collider) {
            m_OnTriggerEnter?.Invoke(collider);
        }

        // OnTriggerExit
        Action<Collider> m_OnTriggerExit;
        public void HandleTriggerExit(Action<Collider> callback) {
            m_OnTriggerExit = callback;
        }
        void OnTriggerExit(Collider collider) {
            m_OnTriggerExit?.Invoke(collider);
        }

        // OnTriggerStay
        Action<Collider> m_OnTriggerStay;
        public void HandleTriggerStay(Action<Collider> callback) {
            m_OnTriggerStay = callback;
        }
        void OnTriggerStay(Collider collider) {
            m_OnTriggerStay?.Invoke(collider);
        }

        // OnCollisionEnter
        Action<Collision> m_OnCollisionEnter;
        public void HandleCollisionEnter(Action<Collision> callback) {
            m_OnCollisionEnter = callback;
        }
        void OnCollisionEnter(Collision collision) {
            m_OnCollisionEnter?.Invoke(collision);
        }

        // OnCollisionExit
        Action<Collision> m_OnCollisionExit;
        public void HandleCollisionExit(Action<Collision> callback) {
            m_OnCollisionExit = callback;
        }
        void OnCollisionExit(Collision collision) {
            m_OnCollisionExit?.Invoke(collision);
        }

        // OnCollisionStay
        Action<Collision> m_OnCollisionStay;
        public void HandleCollisionStay(Action<Collision> callback) {
            m_OnCollisionStay = callback;
        }
        void OnCollisionStay(Collision collision) {
            m_OnCollisionStay?.Invoke(collision);
        }
    }

    public static class MonitorExtensions {
        public static Monitor GetMonitor(this GameObject gameObject) {
            Monitor monitor = gameObject.GetComponent<Monitor>();
            if (monitor == null)
                monitor = gameObject.AddComponent<Monitor>();
            return monitor;
        }
    }
}
