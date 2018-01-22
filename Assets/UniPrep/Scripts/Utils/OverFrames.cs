using System;
using UnityEngine;
using System.Collections;

namespace UniPrep.Utils {
    public class OverFrames : MonoBehaviour {
        static OverFrames instance;
        public delegate void ForLoopDelegate(int index);

        static OverFrames GetInstance() {
            if(instance == null) {
                var go = new GameObject("OverFramesManager");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<OverFrames>();
            }
            return instance;
        }

        public static void For(int start, int end, int frames, ForLoopDelegate loopBody, Action onEnd) {
            var instance = GetInstance();
            if (end - start < frames)
                Debug.LogError("end - start should be greater than frame count");
            instance.StartCoroutine(instance.ForCo(start, end, frames, loopBody, onEnd));
        }

        IEnumerator ForCo(int start, int end, int cycles, ForLoopDelegate loopBody, Action onEnd) {
            int i = 0;
            int cycleLength = (end - start) / cycles;

            while(i != end) {
                loopBody(i);
                if(i % cycleLength == 0)
                    yield return null;
                i++;
            }
            onEnd();
            yield break;
        }
    }
}