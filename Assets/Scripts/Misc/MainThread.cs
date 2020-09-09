using UnityEngine;
using System;
using System.Collections.Generic;

public class MainThread : MonoBehaviour
{
    static MainThread inst;

    static object queueLock = new object();

    Queue<Action> actions;

	void Awake()
    {
        if(inst == null)
        {
            inst = this;
            actions = new Queue<Action>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        lock(queueLock)
        {
			while(actions.Count > 0)
            {
                actions.Dequeue()();
            }
        }
    }

    public static void Run(Action action)
    {
        if(action != null)
        {
            lock (queueLock)
            {
                inst.actions.Enqueue(action);
            }
        }
    }
}