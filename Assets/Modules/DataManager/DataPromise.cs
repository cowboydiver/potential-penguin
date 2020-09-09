using UnityEngine;
using System;

public class DataPromise<T>
{
    public delegate void OnRejection(Exception e);
    public delegate void OnFulfillment(T result);
    public delegate void Callback(OnFulfillment onFulfillment, OnRejection onRejection);

    private OnFulfillment onFulfillment;
    private OnRejection onRejection;

    public DataPromise()
    {
    }

    public DataPromise(Callback callback)
    {
        callback(Fulfill, Reject);
    }

    public DataPromise<T> Then(OnFulfillment onFulfillment)
    {
        this.onFulfillment = onFulfillment;
        return this;
    }

    public DataPromise<T> Catch(OnRejection onRejection)
    {
        this.onRejection = onRejection;
        return this;
    }

    public void Fulfill(T result)
    {
        if (onFulfillment != null)
            onFulfillment(result);
    }

    public void Reject(Exception e)
    {
        if (onRejection != null)
            onRejection(e);
        else
            throw e;
    }
}
