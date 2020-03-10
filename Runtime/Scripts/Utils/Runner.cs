using System.Collections;
using System;
using UnityEngine;

namespace Adrenak.Unex {
    public class Runner : MonoBehaviour {
        /// <summary>
        /// Represents the different states in which the instance can be in
        /// </summary>
        public enum State {
            Idle,
            Running,
            Finished,
            Paused,
            Destroyed
        }

        // ================================================
        // FIELDS
        // ================================================
        /// <summary>
        /// Event fired with the state of the instance changes
        /// </summary>
        public event Action<State> OnStateChange;
        Action m_OnDone;
        IEnumerator m_Method;
        Coroutine m_Coroutine;

        State m_State;
        public State RunnerState {
            get { return m_State; }
            private set {
                m_State = value;
                if (OnStateChange != null)
                    OnStateChange(m_State);
            }
        }

        // Prevents construction with the 'new' keyword as this class derives from MonoBehaviour
        Runner() { }

        void OnDestroy() {
            if (m_Coroutine != null) StopCoroutine(m_Coroutine);
            m_Method = null;
            RunnerState = State.Destroyed;
            Destroy(gameObject);
        }

        /// <summary>
        /// Disposes the instance
        /// </summary>
        public void Destroy() {
            Destroy(gameObject);
        }

        // ================================================
        //  INSTANCE METHODS API
        // ================================================
        public static Runner New(IEnumerator method) {
            return New(method, null);
        }

        public static Runner New() {
            return New(null, null);
        }

        /// <summary>
        /// Constructs a new Runner instance that executes on the given GameObject
        /// </summary>
        /// <param name="method">The IEnumerator method that the instance should run</param>
        /// <returns>The created instance</returns>
        public static Runner New(IEnumerator method, GameObject host) {
            GameObject go;
            if (host == null)
                go = new GameObject();
            else
                go = host;

            //go.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(go);

            var runner = go.AddComponent<Runner>();
            runner.m_Method = method;
            return runner;
        }

        /// <summary>
        /// Begins execution of the coroutine and invokes a callback when it gets over
        /// </summary>
        public void Run(Action onFinished) {
            m_OnDone = onFinished;
            RunnerState = State.Running;
            m_Coroutine = StartCoroutine(AsyncExecutor());
        }

        /// <summary>
        /// Debings the execution of a coroutine
        /// </summary>
        public void Run() {
            Run(null);
        }

        IEnumerator AsyncExecutor() {
            while (RunnerState == State.Running || RunnerState == State.Paused) {
                if (RunnerState == State.Paused)
                    yield return null;

                if (m_Method != null && m_Method.MoveNext())
                    yield return m_Method.Current;
                else {
                    RunnerState = State.Finished;
                    if (m_OnDone != null) m_OnDone();
                }
            }
            yield break;
        }

        /// <summary>
        /// Pauses the execution
        /// </summary>
        public void Pause() {
            if (RunnerState == State.Running)
                RunnerState = State.Paused;
        }

        /// <summary>
        /// Resumes the execution
        /// </summary>
        public void Resume() {
            RunnerState = State.Running;
        }

        // ================================================
        // STATIC METHODS API
        // ================================================
        static GameObject m_StaticHost;

        public static void Init() {
            m_StaticHost = new GameObject("Runner");
            m_StaticHost.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(m_StaticHost);
        }

        /// <summary>
        /// Auto starts a coroutine execution
        /// </summary>
        /// <param name="method">The IEnumerator method that has to be executed</param>
        /// <returns>The Runner instance</returns>
        public static Runner AutoRun(IEnumerator method) {
            return AutoRun(method, null);
        }

        /// <summary>
        /// Auto starts a coroutine execution and invokes a callback when it gets over
        /// </summary>
        /// <param name="method">The IEnumerator method that has to be executed</param>
        /// <returns>The Runner instance</returns>
        public static Runner AutoRun(IEnumerator method, Action onFinished) {
            Runner runner = Runner.New(method, m_StaticHost);
            runner.Run(() => {
                if (onFinished != null)
                    onFinished();
                runner.Destroy();
            });
            return runner;
        }

        /// <summary>
        /// Invokes the action with a delay
        /// </summary>
        /// <param name="seconds">The delay in seconds</param>
        /// <param name="action">The action to be invoked</param>
        public void WaitForSeconds(float seconds, Action action) {
            Runner runner = AutoRun(WaitForSecondsAsync(seconds));
            runner.OnStateChange += state => {
                if (state == State.Finished) {
                    action();
                    runner.Destroy();
                }
            };
        }

        IEnumerator WaitForSecondsAsync(float seconds) {
            yield return new WaitForSeconds(seconds);
        }

        public void WaitForEndOfFrame(Action action) {
            Runner runner = AutoRun(WaitForEndOfFrameAsync());
            runner.OnStateChange += state => {
                if (state == State.Finished) {
                    action();
                    runner.Destroy();
                }
            };
        }

        IEnumerator WaitForEndOfFrameAsync() {
            yield return new WaitForEndOfFrame();
        }

        /// <summary>
        /// Waits while the condition is true
        /// </summary>
        /// <param name="predicate">The condition to check</param>
        /// <param name="callback">Callback when the condition becomes false</param>
        public Runner WaitWhile(Func<bool> predicate, Action callback) {
            Runner runner = AutoRun(WaitWhileAsync(predicate, callback));
            runner.OnStateChange += state => {
                if (state == State.Finished) {
                    if (callback != null) callback();
                    runner.Destroy();
                }
            };
            return runner;
        }

        IEnumerator WaitWhileAsync(Func<bool> predicate, Action callback) {
            while (predicate()) yield return null;
        }

        /// <summary>
        /// Waits while the condition is false
        /// </summary>
        /// <param name="predicate">The condition to check</param>
        /// <param name="callback">Callback when the condition becomes true</param>
        public Runner WaitUntil(Func<bool> predicate, Action callback) {
            Runner runner = AutoRun(WaitUntilAsync(predicate, callback));
            runner.OnStateChange += state => {
                if (state == State.Finished) {
                    if (callback != null) callback();
                    runner.Destroy();
                }
            };
            return runner;
        }

        IEnumerator WaitUntilAsync(Func<bool> predicate, Action callback) {
            while (!predicate()) yield return null;
        }

        public Runner RunIf(Func<bool> predicate, Action callback) {
            return AutoRun(RunIfAsync(predicate, callback));
        }

        IEnumerator RunIfAsync(Func<bool> predicate, Action callback) {
            while (true) {
                if (predicate()) {
                    callback();
                    Destroy();
                }
                yield return null;
            }
        }
    }
}
