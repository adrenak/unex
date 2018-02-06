using UnityEngine;
using UniPrep.Utils;

public class OverFramesExample : MonoBehaviour {
    // Disable this to check for GC in the profile (Debug.Log creates garbage)
    public bool debug;

	void Start() {
        // Spread a For loop from x to y over n frames
        int x = 0;
        int y = 100000000;
        int n = 50;
        OverFrames.For(x, y, n, 
            i => {
                float value = i * Random.value;
            },
            ()=> { }
        );
	}

    void Log(string msg) {
        if (debug)
            Debug.Log(msg);
    }
}
