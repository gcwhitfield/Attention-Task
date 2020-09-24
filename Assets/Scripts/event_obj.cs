using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System;

[Serializable]
public enum hand : byte { Present, NotPresent}

public class adhoc_event<T> : UnityEvent<T> { }

public class adhoc_event<T,T2> : UnityEvent<T,T2> { }

public class gen_event<T> : ScriptableObject
{

    private UnityEvent<T> _e;

    public UnityEvent<T> e
    {
        get { if (_e == null) _e = new adhoc_event<T>(); return _e; }
    }

    public void addListener(UnityAction<T> a) { e.AddListener(a); }

    public void RemoveAllListeners() { e.RemoveAllListeners(); }

    public void Invoke(T arg) { e.Invoke(arg); }

}

public class gen_event<T,T2> : ScriptableObject
{

    private UnityEvent<T,T2> _e;

    public UnityEvent<T,T2> e
    {
        get { if (_e == null) _e = new adhoc_event<T,T2>(); return _e; }
    }

    public void addListener(UnityAction<T,T2> a) { e.AddListener(a); }
    
    public void RemoveAllListeners() { e.RemoveAllListeners(); }

    public void Invoke(T arg,T2 arg2) { e.Invoke(arg,arg2); }

}


[CreateAssetMenu(menuName = "variables/event")]
public class event_obj : gen_event<hand> { }