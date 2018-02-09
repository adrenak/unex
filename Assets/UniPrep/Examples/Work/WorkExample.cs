using UnityEngine;
using UniPrep.Utils;
using System.Collections;

public class WorkExample : MonoBehaviour {
    void Start () {
        var work = new Work<string>(DelayedMessage());
        work.Begin(result => {
            Debug.Log(result);
        });
	}
	
	IEnumerator DelayedMessage() {
        yield return new WaitForSeconds(1);
        yield return "That's awesome";
    }
}
