/// Jobs.cs
/// Copyright (c) 2011, Ken Rockot  <k-e-n-@-REMOVE-CAPS-AND-HYPHENS-oz.gs>.  All rights reserved.
/// Copyright (c) 2018, Vatsal Ambastha
/// 
/// Everyone is granted non-exclusive license to do anything at all with this code.
///
/// This is a new coroutine interface for Unity.
///
/// The motivation for this is twofold:
///
/// 1. The existing coroutine API provides no means of stopping specific
///    coroutines; StopCoroutine only takes a string argument, and it stops
///    all coroutines started with that same string; there is no way to stop
///    coroutines which were started directly from an enumerator.  This is
///    not robust enough and is also probably pretty inefficient.
///
/// 2. StartCoroutine and friends are MonoBehaviour methods.  This means
///    that in order to start a coroutine, a user typically must have some
///    component reference handy.  There are legitimate cases where such a
///    constraint is inconvenient.  This implementation hides that
///    constraint from the user.
///
/// Example usage:
///
/// ----------------------------------------------------------------------------
/// IEnumerator MyAwesomeJob()
/// {
///     while(true) {
///         Debug.Log("Logcat iz in ur consolez, spammin u wif messagez.");
///         yield return null;
///     }
/// }
///
/// IEnumerator JobKiller(float delay, Job t)
/// {
///     yield return new WaitForSeconds(delay);
///     t.Stop();
/// }
///
/// void SomeCodeThatCouldBeAnywhereInTheUniverse()
/// {
///     Job spam = new Job(MyAwesomeJob());
///     new Job(JobKiller(5, spam));
/// }
/// ----------------------------------------------------------------------------
///
/// When SomeCodeThatCouldBeAnywhereInTheUniverse is called, the debug console
/// will be spammed with annoying messages for 5 seconds.
///
/// Simple, really.  There is no need to initialize or even refer to JobManager.
/// When the first Job is created in an application, a "JobManager" GameObject
/// will automatically be added to the scene root with the JobManager component
/// attached.  This component will be responsible for dispatching all coroutines
/// behind the scenes.
///
/// Job also provides an event that is triggered when the coroutine exits.

using System;
using UnityEngine;
using System.Text;
using System.Collections;

namespace UniPrep.Utils {
    public class Job {
        // ================================================
        // FIELDS
        // ================================================
        const string k_ExecutorNamePrefix = "JOB_";
        static int m_JobCount;
        JobExecutor m_Executor;
        bool m_Running;
        int m_ID;

        /// <summary>
        /// Returns if the coroutine is currently running
        /// </summary>
        public bool Running {
            get { return m_Running; }
        }

        // ================================================
        // CONSTRUCTORS AND PRE DEF CREATION RULES
        // ================================================
        /// <summary>
        /// Construct and initializes a Job object
        /// </summary>
        /// <param name="p_Method">The method to run</param>
        /// <param name="p_AutoExecute">If the method should start executing immediately</param>
        public Job(IEnumerator p_Method, bool p_AutoExecute = true) {
            m_ID = ++m_JobCount;
            var _executorName = k_ExecutorNamePrefix + m_ID;
            m_Executor = new GameObject(_executorName).AddComponent<JobExecutor>();
            m_Executor.SetCoroutineMethod(p_Method);

            if (p_AutoExecute)
                Start();
        }
        
        /// <summary>
        /// Runs the given action after the given delay
        /// </summary>
        /// <param name="p_Delay">The duration for which the delay must occur.</param>
        /// <param name="p_OnDone">The action invoked with the delay is over. </param>
        public static void ProceedWithDelay(float p_Delay, Action p_OnDone) {
            Job delay = new Job(DelayMethod(p_Delay), false);
            delay.Start(() => {
                p_OnDone();
            });
        }

        // PREDEF DELAY ROUTINE
        private static IEnumerator DelayMethod(float p_Delay) {
            yield return new WaitForSeconds(p_Delay);
            yield break;
        }

        public new string ToString() {
            return new StringBuilder()
                .Append("Job ID : ").Append(m_ID.ToString())
                .Append("Running : ").Append(m_Running.ToString())
                .ToString();
        }

        // ================================================
        // EXTERNAL INVOKES
        // ================================================
        /// <summary>
        /// Starts the Job execution
        /// </summary>
        /// <param name="p_Completed">Callback when the job is done</param>
        public void Start(Action p_Completed) {
            m_Running = true;
            m_Executor.Execute(() => {
                Stop();
                p_Completed();
            });
        }

        /// <summary>
        /// Starts the Job execution
        /// </summary>
        public void Start() {
            Start(null);
        }

        /// <summary>
        /// Stops the coroutine execution
        /// </summary>
        public void Stop() {
            m_Running = false;
            if (m_Executor != null) {
                m_Executor.Terminate();
                m_Executor = null;
            }
        }
    }

    public class JobExecutor : MonoBehaviour {
        // ================================================
        // FIELDS
        // ================================================
        IEnumerator m_Method;
        bool m_Running;
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
        /// <param name="p_Method">The coroutine method to be wrapped</param>
        public void SetCoroutineMethod(IEnumerator p_Method) {
            m_Method = p_Method;
        }

        /// <summary>
        /// Starts the wrapped coroutine in the public coroutine
        /// </summary>
        /// <param name="callback">Callback invoked on completion.</param>
        public void Execute(Action p_OnDone) {
            this.m_OnDone = p_OnDone;
            m_Running = true;
            StartCoroutine(ExecutorMethod());
        }

        IEnumerator ExecutorMethod() {
            yield return null;
            while (true) {
                if (!m_Running)
                    break;

                if (m_Method != null && m_Method.MoveNext())
                    yield return m_Method.Current;
                else
                    Terminate();
            }
            yield break;
        }

        /// <summary>
        /// Terminates the coroutine and the gameobject it is running on
        /// </summary>
        public void Terminate() {
            if (!m_Running)
                return;

            m_Running = false;
            StopCoroutine(ExecutorMethod());
            m_Method = null;
            m_OnDone();
            Destroy(gameObject);
        }
    }
}