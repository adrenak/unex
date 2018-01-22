using UnityEngine;
using UniPrep.Utils;

public class OverFramesExample : MonoBehaviour {
	void Start () {
        // Spread a For loop from x to y over n frames
        int x = 0;
        int y = 10000;
        int n = 10;
        OverFrames.For(x, y, n, 
            i => {
                Debug.Log("Current index : " + i);
            },
            ()=> {
                Debug.Log("OverFrames.For over");
            }
        );
	}
}
