using System;
using UnityEngine;
using System.Text;
using System.Collections;

namespace UniPrep.Utils {
    public class Work {
        // ================================================
        // FIELDS
        // ================================================
        const string k_ExecutorNamePrefix = "WORK_";
        static int m_WorkCount;
        WorkExecutor m_Executor;
        int m_ID;

        /// <summary>
        /// Returns if the coroutine is currently running
        /// </summary>
        public bool Running {
            get { return m_Executor.IsRunning(); }
        }

        public bool Paused {
            get { return m_Executor.IsPaused(); }
        }

        // ================================================
        // CONSTRUCTORS AND PREFABS
        // ================================================
        /// <summary>
        /// Construct and initializes a Job object
        /// </summary>
        /// <param name="method">The method to run</param>
        /// <param name="autoExecute">If the method should start executing immediately</param>
        public Work(IEnumerator method) {
            m_ID = ++m_WorkCount;
            var _executorName = k_ExecutorNamePrefix + m_ID;
            m_Executor = new GameObject(_executorName).AddComponent<WorkExecutor>();
            m_Executor.SetCoroutineMethod(method);
        }
        
        /// <summary>
        /// Runs the given action after the given delay
        /// </summary>
        /// <param name="delay">The duration for which the delay must occur.</param>
        /// <param name="onDone">The action invoked with the delay is over. </param>
        public static void StartDelayed(float delay, Action onDone) {
            Work delayWork = new Work(DelayCo(delay));
            delayWork.Begin(() => {
                onDone();
            });
        }

        private static IEnumerator DelayCo(float delayDuration) {
            yield return new WaitForSeconds(delayDuration);
            yield break;
        }

        public new string ToString() {
            return new StringBuilder()
                .Append("Job ID : ").Append(m_ID.ToString())
                .Append("Running : ").Append(Running.ToString())
                .Append("Paused : ").Append(Paused.ToString())
                .ToString();
        }

        // ================================================
        // EXTERNAL INVOKES
        // ================================================
        /// <summary>
        /// Starts the Work
        /// </summary>
        /// <param name="onCompleted">Callback when the work is done</param>
        public void Begin(Action onCompleted = null) {
            m_Executor.Begin(() => {
                if(onCompleted != null)
                    onCompleted();
            });
        }

        /// <summary>
        /// Pauses the Work 
        /// </summary>
        public void Pause() {
            m_Executor.Pause();
        }

        /// <summary>
        /// Resumes the Work
        /// </summary>
        public void Resume() {
            m_Executor.Resume();
        }

        /// <summary>
        /// Stops the work and cleans up the instance
        /// </summary>
        public void End() {
            m_Executor.End();
        }
    }

    public class WorkExecutor : MonoBehaviour {
        // ================================================
        // FIELDS
        // ================================================
        IEnumerator m_WorkCoroutine;
        bool m_Running;
        bool m_Paused;
        Action m_OnDone;

        void Awake() {
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;
        }

        void OnDestroy() {
            Terminate();
        }

        /// <summary>
        /// Sets the coroutine method to be wrapped in the executor
        /// </summary>
        /// <param name="method">The coroutine method to be wrapped</param>
        public void SetCoroutineMethod(IEnumerator method) {
            m_WorkCoroutine = method;
        }

        /// <summary>
        /// Starts the wrapped coroutine in the public coroutine
        /// </summary>
        /// <param name="callback">Callback invoked on completion.</param>
        public void Begin(Action onDone) {
            m_OnDone = onDone;
            m_Running = true;
            StartCoroutine(ExecutorMethod());
        }

        public void Pause() {
            m_Paused = true;
        }

        public void Resume() {
            m_Paused = false;
        }

        public void End() {
            m_Running = false;
        }

        public bool IsRunning() {
            return m_Running;
        }

        public bool IsPaused() {
            return m_Paused;
        }

        IEnumerator ExecutorMethod() {
            while (m_Running) {
                if (m_Paused)
                    yield return null;

                if (m_WorkCoroutine != null && m_WorkCoroutine.MoveNext())
                    yield return m_WorkCoroutine.Current;
                else
                    m_Running = false;
            }
            Terminate();
            yield break;
        }

        /// <summary>
        /// Terminates the coroutine and the gameobject it is running on
        /// </summary>
        public void Terminate() {
            StopCoroutine(ExecutorMethod());
            m_WorkCoroutine = null;
            m_OnDone();
            Destroy(gameObject);
        }
    }
}