using System;
using UnityEngine;
using Adrenak.Unex;
using System.Collections;

namespace Adrenak.Unex.Samples{
    public class PoolDemo : MonoBehaviour {
        GameObjectPool m_Pool;
        public int emisssionRate;
        public GameObject prefabReference;
        public Vector3 maxForce;
        public Transform parent;
    
	    void Start () {
            m_Pool = new GameObjectPool(prefabReference);
            StartCoroutine(StartStuff());
	    }

        IEnumerator StartStuff() {
            while (true) {
                for(int i = 0; i < emisssionRate; i++) {
                    var newInstance = m_Pool.Get();

                    // Set a single parent to batch instances
                    newInstance.transform.SetParent(parent);

                    // slow down to zero, Set to origin and apply force
                    var rigidbody = newInstance.GetComponent<Rigidbody>();
                    rigidbody.velocity = Vector3.zero;

                    // Move to position
                    newInstance.transform.position = transform.position;

                    // random force value
                    var force = new Vector3(
                        maxForce.x * (UnityEngine.Random.value - .5F),
                        maxForce.y * (UnityEngine.Random.value - .5F),
                        maxForce.z * (UnityEngine.Random.value - .5F)
                    );

                    // Apply force
                    rigidbody.AddForce(force, ForceMode.Impulse);

				    // Handle recycling when the prefab hits the floor
				    HandleCollision(newInstance);
                }
                yield return null;
            }
        }

	    void HandleCollision(GameObject go) {
            go.GetMonitor().HandleTriggerEnter(_ => {
                if (_.name.Equals("Floor"))
                    m_Pool.Free(go);
            });
	    }
    }
}
