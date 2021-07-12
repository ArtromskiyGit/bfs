using System;
using System.Collections.Generic;
using UnityEngine;

public class Scriptables : MonoBehaviour
{
    [SerializeField]
    private List<ScriptableObject> _scriptablesList;
    private static Dictionary<Type, ScriptableObject> scriptablesDictionary;
    private static Scriptables _instance;
    public static Scriptables Instance => _instance;

    void Awake()
    {
        _instance = this;
        scriptablesDictionary = new Dictionary<Type, ScriptableObject>();
        for (int i = 0; i < _scriptablesList.Count; i++)
        {
            scriptablesDictionary.Add(_scriptablesList[i].GetType(), _scriptablesList[i]);
        }

        foreach (var item in scriptablesDictionary)
        {
            if (item.Value is IInitable initable)
            {
                initable.Init();
            }
        }
    }

    public static T GetScriptable<T>() where T : ScriptableObject
    {
        T scriptable = null;
        try
        {
            scriptable = scriptablesDictionary[typeof(T)] as T;
        }
        catch
        {
            Debug.LogError("Can't find scriptable " + typeof(T).ToString());
        }
        return scriptable;
    }

}
