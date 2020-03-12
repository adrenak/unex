using UnityEngine;

using System;
using System.Threading.Tasks;

namespace Adrenak.Unex {
    public class TaskX {
        public static Task WaitTillNextFrame() {
            return WaitForFrames(1);
        }

        public async static Task WaitForFrames(int frames) {
            int startFrame = Time.frameCount;
            while (true) {
                if (Time.frameCount >= startFrame + frames)
                    break;
                else
                    await Task.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime / 2));
            }
        }

        public async static Task WaitForSeconds(float seconds) {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
        }

        public async static Task WaitWhile(Func<bool> condition) {
            if (condition())
                await Task.FromResult(WaitTillNextFrame());
        }

        public async static Task WaitUntil(Func<bool> condition) {
            if (condition() == false)
                await Task.FromResult(WaitTillNextFrame());
        }
    }
}
