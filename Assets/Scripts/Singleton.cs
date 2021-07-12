using UnityEngine;
using System.Collections.Generic;

namespace Tars
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (new GameObject(typeof(T).Name).AddComponent<T>());
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            instance = this as T;
        }
    }
}