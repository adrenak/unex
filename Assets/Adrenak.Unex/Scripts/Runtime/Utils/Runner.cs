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
        Action m_OnFinished;
        IEnumerator m_Method;
        Coroutine m_Coroutine;

        State m_State;
        public State RunnerState {
            get { return m_State; }
            private set {
                m_State = value;
                OnStateChange?.Invoke(m_State);
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
        /// <summary>
        /// Constructs a new Runner instance
        /// </summary>
        /// <param name="method">The IEnumerator method that the instance should run</param>
        /// <returns>The created instance</returns>
        public static Runner New(IEnumerator method) {
            var runner = m_StaticHost.AddComponent<Runner>();
            runner.m_Method = method;
            return runner;
        }

        /// <summary>
        /// Begins execution of the coroutine and invokes a callback when it gets over
        /// </summary>
        public void Run(Action onFinished) {
            m_OnFinished = onFinished;
            RunnerState = State.Running;
            m_Coroutine = StartCoroutine(AsyncExecutor());
        }

        /// <summary>
        /// Debings the execution of a coroutine
        /// </summary>
        public void Run() {
            Run((Action)null);
        }

        IEnumerator AsyncExecutor() {
            while (RunnerState == State.Running || RunnerState == State.Paused) {
                if (RunnerState == State.Paused)
                    yield return null;

                if (m_Method != null && m_Method.MoveNext())
                    yield return m_Method.Current;
                else {
                    RunnerState = State.Finished;
                    if (m_OnFinished != null) m_OnFinished();
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
        public static Runner Run(IEnumerator method) {
            return Run(method, null);
        }

        /// <summary>
        /// Auto starts a coroutine execution and invokes a callback when it gets over
        /// </summary>
        /// <param name="method">The IEnumerator method that has to be executed</param>
        /// <returns>The Runner instance</returns>
        public static Runner Run(IEnumerator method, Action onFinished) {
            Runner runner = New(method);
            runner.Run(() => {
                onFinished?.Invoke();
                runner.Destroy();
            });
            return runner;
        }

        /// <summary>
        /// Invokes the action with a delay
        /// </summary>
        /// <param name="seconds">The delay in seconds</param>
        /// <param name="action">The action to be invoked</param>
        public static void WaitForSeconds(float seconds, Action action) {
            Runner runner = Run(WaitForSecondsAsync(seconds));
            runner.OnStateChange += state => {
                if (state == State.Finished) {
                    action();
                    runner.Destroy();
                }
            };
        }

        static IEnumerator WaitForSecondsAsync(float seconds) {
            yield return new WaitForSeconds(seconds);
        }

        public static void WaitForEndOfFrame(Action action) {
            Runner runner = Run(WaitForEndOfFrameAsync());
            runner.OnStateChange += state => {
                if (state == State.Finished) {
                    action();
                    runner.Destroy();
                }
            };
        }

        static IEnumerator WaitForEndOfFrameAsync() {
            yield return new WaitForEndOfFrame();
        }

        /// <summary>
        /// Waits while the condition is true
        /// </summary>
        /// <param name="predicate">The condition to check</param>
        /// <param name="callback">Callback when the condition becomes false</param>
        public static Runner WaitWhile(Func<bool> predicate, Action callback) {
            Runner runner = Run(WaitWhileAsync(predicate, callback));
            runner.OnStateChange += state => {
                if (state == State.Finished) {
                    if (callback != null) callback();
                    runner.Destroy();
                }
            };
            return runner;
        }

        static IEnumerator WaitWhileAsync(Func<bool> predicate, Action callback) {
            while (predicate()) yield return null;
        }

        /// <summary>
        /// Waits while the condition is false
        /// </summary>
        /// <param name="predicate">The condition to check</param>
        /// <param name="callback">Callback when the condition becomes true</param>
        public static Runner WaitUntil(Func<bool> predicate, Action callback) {
            Runner runner = Run(WaitUntilAsync(predicate, callback));
            runner.OnStateChange += state => {
                if (state == State.Finished) {
                    runner.Destroy();
                    callback?.Invoke();
                }
            };
            return runner;
        }

        static IEnumerator WaitUntilAsync(Func<bool> predicate, Action callback) {
            while (!predicate()) yield return null;
        }

        public static Runner RunWhen(Func<bool> predicate, Action callback) {
            Runner runner = Run(RunWhenAsync(predicate, callback));
            runner.OnStateChange += state => {
                if (state == State.Finished) {
                    runner.Destroy();
                    callback?.Invoke();
                }
            };
            return runner;
        }

        static IEnumerator RunWhenAsync(Func<bool> predicate, Action callback) {
            while (true) {
                if (predicate()) 
                    callback();
                yield return null;
            }
        }
    }
}
