using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventWithParam<T> : ScriptableObject
{
    private List<GameEventWithParamListener<T>> listeners = new List<GameEventWithParamListener<T>>();

    public void Raise(T param)
    {
        // Backwards for easy removing listener inside listener itself
        for (int i = listeners.Count - 1; i >= 0; i --)
        {
            listeners[i].OnEventRaised(param);
        }
    }

    public void AddListener(GameEventWithParamListener<T> listener)
    {
        listeners.Add(listener);
    }

    public void RemoveListener(GameEventWithParamListener<T> listener)
    {
        listeners.Remove(listener);
    }
}
