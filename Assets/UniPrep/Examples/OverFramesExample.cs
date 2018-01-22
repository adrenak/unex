using UnityEngine;
using UniPrep.Utils;

public class OverFramesExample : MonoBehaviour {

	// Use this for initialization
	void Start () {
        OverFrames of = OverFrames.Get();
        of.For(0, 20, 200, i => {
            Debug.Log(i);
        },
        ()=> {
            Debug.Log("Ended");
        });
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
