using System;
using System.Collections.Generic;
using UnityEngine;

namespace Adrenak.Unex {
	public class Dispatcher : MonoBehaviour {
		static Dispatcher m_Instance;
		Queue<Action> m_Queue = new Queue<Action>();

		public static void Init() {
			if (m_Instance != null) return;

			var go = new GameObject("Adrenak.Unex.Dispatcher") {
				hideFlags = HideFlags.HideAndDontSave
			};
			DontDestroyOnLoad(go);
			m_Instance = go.AddComponent<Dispatcher>();
		}

		public static void Enqueue(Action action) {
			lock (m_Instance.m_Queue) {
				m_Instance.m_Queue.Enqueue(action);
			}
		}

		void Update() {
			lock (m_Instance.m_Queue) {
				while (m_Instance.m_Queue.Count > 0)
					m_Instance.m_Queue.Dequeue()();
			}
		}
	}
}
