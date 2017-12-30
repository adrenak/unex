// ThreadPool.cs
//
// Copyright 2014 NYAHOON GAMES PTE. LTD. All Rights Reserved.
//
// Link to article : https://nyahoon.com/blog-en/366

using UnityEngine;
using System.Threading;

namespace UniPrep.Utils {
    /// <summary>
    /// Thread pool.
    /// This class itself is not thread safe. Only a single thread can call QueueUserWorkItem safely.
    /// </summary>
    public class ThreadPool {
        private static ThreadPool s_instance = null;
        public static void InitInstance() {
            if (s_instance == null) {
                InitInstance(128, 0);
            }
        }
        public static bool InitInstance(int queueSize, int threadNum) {
            if (s_instance != null) {
                Debug.LogWarning("TreadPool instance is already created.");
                return false;
            }
            s_instance = new ThreadPool(queueSize, threadNum);
            return true;
        }
        public static ThreadPool Instance {
            get { return s_instance; }
        }
        public static void QueueUserWorkItem(WaitCallback callback, object state) {
            s_instance.EnqueueTask(callback, state);
        }
        private Thread[] m_threadPool;
        struct TaskInfo {
            public WaitCallback callback;
            public object args;
        }
        private TaskInfo[] m_taskQueue;
        private int m_nPutPointer;
        private int m_nGetPointer;
        private int m_numTasks;
        private AutoResetEvent m_putNotification;
        private AutoResetEvent m_getNotification;
#if !UNITY_WEBGL
        // according to this page (https://docs.unity3d.com/401/Documentation/ScriptReference/MonoCompatibility.html),
        // Semaphore is not available on web player.
        private Semaphore m_semaphore;
#endif

        private ThreadPool(int queueSize, int threadNum) {
#if UNITY_WEBGL
			threadNum = 1;
#else
            if (threadNum == 0) {
                threadNum = SystemInfo.processorCount;
            }
#endif
            m_threadPool = new Thread[threadNum];
            m_taskQueue = new TaskInfo[queueSize];
            m_nPutPointer = 0;
            m_nGetPointer = 0;
            m_numTasks = 0;
            m_putNotification = new AutoResetEvent(false);
            m_getNotification = new AutoResetEvent(false);
#if !UNITY_WEBGL
            if (1 < threadNum) {
                m_semaphore = new Semaphore(0, queueSize);
                for (int i = 0; i < threadNum; ++i) {
                    m_threadPool[i] = new Thread(ThreadFunc);
                    m_threadPool[i].Start();
                }
            }
            else
#endif
            {
                m_threadPool[0] = new Thread(SingleThreadFunc);
                m_threadPool[0].Start();
            }
        }
        private void EnqueueTask(WaitCallback callback, object state) {
            while (m_numTasks == m_taskQueue.Length) {
                m_getNotification.WaitOne();
            }
            m_taskQueue[m_nPutPointer].callback = callback;
            m_taskQueue[m_nPutPointer].args = state;
            ++m_nPutPointer;
            if (m_nPutPointer == m_taskQueue.Length) {
                m_nPutPointer = 0;
            }
#if !UNITY_WEBGL
            if (m_threadPool.Length == 1) {
#endif
                if (Interlocked.Increment(ref m_numTasks) == 1) {
                    m_putNotification.Set();
                }
#if !UNITY_WEBGL
            }
            else {
                Interlocked.Increment(ref m_numTasks);
                m_semaphore.Release();
            }
#endif
        }
#if !UNITY_WEBGL
        private void ThreadFunc() {
            for (;;) {
                m_semaphore.WaitOne();
                int nCurrentPointer, nNextPointer;
                do {
                    nCurrentPointer = m_nGetPointer;
                    nNextPointer = nCurrentPointer + 1;
                    if (nNextPointer == m_taskQueue.Length) {
                        nNextPointer = 0;
                    }
                } while (Interlocked.CompareExchange(ref m_nGetPointer, nNextPointer, nCurrentPointer) != nCurrentPointer);
                TaskInfo task = m_taskQueue[nCurrentPointer];
                if (Interlocked.Decrement(ref m_numTasks) == m_taskQueue.Length - 1) {
                    m_getNotification.Set();
                }
                task.callback(task.args);
            }
        }
#endif
        private void SingleThreadFunc() {
            for (;;) {
                while (m_numTasks == 0) {
                    m_putNotification.WaitOne();
                }
                TaskInfo task = m_taskQueue[m_nGetPointer++];
                if (m_nGetPointer == m_taskQueue.Length) {
                    m_nGetPointer = 0;
                }
                if (Interlocked.Decrement(ref m_numTasks) == m_taskQueue.Length - 1) {
                    m_getNotification.Set();
                }
                task.callback(task.args);
            }
        }
    }
}