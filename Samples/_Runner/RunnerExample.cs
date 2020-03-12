using UnityEngine;
using System.Collections;

namespace Adrenak.Unex.Samples{
    public class RunnerExample : MonoBehaviour {
	    IEnumerator Start() {
		    var runner = Runner.New(CoroutineA1());
		    runner.OnStateChange += state => Debug.Log(state);
		    runner.Run();

		    yield return new WaitForSeconds(.5f);
		    runner.Pause();
		    yield return new WaitForSeconds(.5f);
		    runner.Resume();
		    yield return new WaitForSeconds(2f);
		    runner.Destroy();
		    Runner.WaitForSeconds(2, () => Debug.Log("Printed after 2 seconds"));
        }

	    IEnumerator CoroutineA1() {
		    Debug.Log("Starting coroutine");
            yield return new WaitForSeconds(2);
		    Debug.Log("Done coroutine");
	    }
    }
}
