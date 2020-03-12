using System;
using UnityEngine;
using System.Collections.Generic;

namespace Adrenak.Unex {
    public abstract class Pool<T> {
        protected Stack<T> m_Available = new Stack<T>();
        HashSet<T> m_ToBeIgnored = new HashSet<T>();
        
        /// <summary>
        /// Gets a free instance from the pool. Constructs a new instance if none is free
        /// </summary>
        /// <returns></returns>
        public T Get() {
            while(m_Available.Count > 0) {
                var instance = m_Available.Pop();
                if (!m_ToBeIgnored.Contains(instance))
                    return instance;
            }
            Add();
            return Get();
        }

        /// <summary>
        /// Adds a new instance. To be override based on whether T is a component
        /// of a normal c# class
        /// </summary>
        protected abstract void Add();

        /// <summary>
        /// Frees up the instance so that it can be retrieved by another request later
        /// </summary>
        /// <param name="obj"></param>
        public void Free(T obj) {
            m_Available.Push(obj);
        }

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(T obj) {
            m_ToBeIgnored.Add(obj);
        }
    }

    /// <summary>
    /// An instance pool of a Unity component
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComponentPool<T> : Pool<T> where T : Component {
        GameObjectPool m_Pool = new GameObjectPool(new GameObject("ComponentPoolContainer"));
    
        protected override void Add() {
            var container = m_Pool.Get();
            container.name = typeof(T).FullName;
            var newInstance = container.AddComponent<T>();
            m_Available.Push(newInstance);
        }
    }

    /// <summary>
    /// An instance pool of a normal C# class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T> : Pool<T> {
        protected override void Add() {
            var def = default(T);
            var newInstance = Activator.CreateInstance(def.GetType());
            m_Available.Push((T)newInstance);
        }
    }

    /// <summary>
    /// A Gameobject instance pool. Eg. Pool of bullet shells
    /// </summary>
    public class GameObjectPool : Pool<GameObject> {
        GameObject m_Instance;

        /// <summary>
        /// Constructs a gameobject pool with the given instance
        /// </summary>
        /// <param name="instance"></param>
        public GameObjectPool(GameObject instance) {
            m_Instance = instance;
        }

        /// <summary>
        /// Instantiates a copy of <see cref="m_Instance"/> and adds it to the pool
        /// </summary>
        protected override void Add() {
            var newInstance = MonoBehaviour.Instantiate<GameObject>(m_Instance);
            m_Available.Push(newInstance);
        }
    }
}
