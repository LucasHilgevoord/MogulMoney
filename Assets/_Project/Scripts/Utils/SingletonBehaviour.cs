using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Singleton that will destroy its gameObject if it detects there was already an instance of this type created.
/// </summary>
/// <typeparam name="t"></typeparam>
public class Singleton<t> : MonoBehaviour where t : Component
{
    public static t Instance { get; private set; }
        
    internal virtual void Awake()
    {
        if (Singleton<t>.Instance == null)
            Singleton<t>.Instance = this as t;
        else
        {
            Destroy(this.gameObject);
        }
    }
}
