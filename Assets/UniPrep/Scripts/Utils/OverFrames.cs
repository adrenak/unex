using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UniPrep.Utils {
    public class OverFrames : MonoBehaviour {
        bool isFree;
        static List<OverFrames> pool = new List<OverFrames>();

        public delegate void ForLoopDelegate(int index);

        static OverFrames Get() {
            if (pool.Count == 0) {
                pool.Add(CreateInstance());
                return pool[0];
            }

            var freeIndex = GetFirstFreeIndex();

            if (freeIndex == -1) {
                pool.Add(CreateInstance());
                return pool[0];
            }
            else
                return pool[GetFirstFreeIndex()];
        }

        static int GetFirstFreeIndex() {
            for (int i = 0; i < pool.Count; i++)
                if (pool[i].isFree)
                    return i;
            return -1;
        }

        static OverFrames CreateInstance() {
            var cted = new GameObject("OverFrames").AddComponent<OverFrames>();
            return cted;
        }

        public static void For(int start, int end, int frames, ForLoopDelegate loopBody, Action onEnd) {
            var instance = Get();
            if (end - start < frames)
                Debug.LogError("end - start should be greater than frame count");
            instance.isFree = false;
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
            isFree = true;
        }
    }
}