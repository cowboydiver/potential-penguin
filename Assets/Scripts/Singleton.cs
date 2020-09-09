using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    protected static T inst;

    public static T Inst {
        get {
            if (inst == null) {
                inst = FindObjectOfType<T>();
            }
            return inst;
        }
    }
}