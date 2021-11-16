﻿using UnityEngine;

public class SingletonMonoDontDestroy<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static object _lock = new object();
    
    protected string className;

    public string GetClassName()
    {
        return className;
    }

    public static T Instance
    {
        get
        {
            
            lock (_lock)
            {
                //Debug.Log(typeof(T).Name);
                if (applicationIsQuitting)
                {
                    //Debug.Log("Null");
                    return null;
                }

                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        var obj = Resources.Load<GameObject>("Managers/" + typeof(T).Name) as GameObject;
                        if (obj == null)
                            return null;
                        GameObject singleton = Instantiate(obj);
                        _instance = singleton.GetComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).Name;
                        DontDestroyOnLoad(singleton);
                    }
                }
                return _instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;

    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    public virtual void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}
