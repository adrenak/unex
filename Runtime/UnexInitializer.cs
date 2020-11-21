using UnityEngine;

namespace Adrenak.Unex {
	public class UnexInitializer : MonoBehaviour {
		static UnexInitializer instance;

		void Awake() {
			if (instance == null)
				instance = GetComponent<UnexInitializer>();
			Initialize();
        }

        public static void Initialize() {
			Dispatcher.Init();
            Runnable.Init();
		}
	}
}
