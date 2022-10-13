using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> where T : class, new()
{
    private static readonly T _inst = new T();

    public static T Inst 
    {
        get 
        {
            return _inst; 
        } 
    }

    private Singleton()
    {

    }
}
