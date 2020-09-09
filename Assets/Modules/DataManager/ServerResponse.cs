using System;

[Serializable]
public class ServerResponse<T> : ServerResponse
{
    public T data;
}

[Serializable]
public class ServerResponse
{
    public bool success;
    public string code;
    public string message;
}
