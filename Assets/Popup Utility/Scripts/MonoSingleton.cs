using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : Component
{
    private static T m_Instance;

    public static T Instance => m_Instance;
    
    private void Awake()
    {
        if (m_Instance != null)
        {
            Destroy(this);
            return;
        }
        else
        {
            m_Instance = this as T;
            DontDestroyOnLoad(this);
        }
        OnSingletonSpawn();
    }

    public virtual void OnSingletonSpawn()
    {
    }
}
