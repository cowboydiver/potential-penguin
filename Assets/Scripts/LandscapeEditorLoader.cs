#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class LandscapeEditorLoader : MonoBehaviour
{
    void Start()
    {
        if(!EditorApplication.isPlaying)
        {
            Landscape landscape = GetComponent<Landscape>();
            landscape.RebuildMesh();
        }
    }
}
#endif