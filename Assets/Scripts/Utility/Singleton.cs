using UnityEngine;

/// <summary>
/// Provides a generic implementation of the Singleton design pattern for MonoBehaviour types.
/// Ensures that only one instance of the Singleton exists within the application at any time.
/// If no instance is found upon access, this script creates the Instance.
/// </summary>
/// <typeparam name="T">The type of the MonoBehaviour that should be a Singleton.</typeparam>
public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T s_Instance;

    public static T Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = (T)FindFirstObjectByType(typeof(T));

                if (s_Instance == null)
                {
                    SetupInstance();
                }
                else
                {
                    string typeName = typeof(T).Name;

                    Debug.Log("[Singleton] " + typeName + " instance already created: " +
                                s_Instance.gameObject.name);
                }
            }

            return s_Instance;
        }
    }

    public virtual void Awake()
    {
        RemoveDuplicates();
    }

    private static void SetupInstance()
    {
        // lazy instantiation
        s_Instance = (T)FindFirstObjectByType(typeof(T));

        if (s_Instance == null)
        {
            GameObject gameObj = new GameObject();
            gameObj.name = typeof(T).Name;

            s_Instance = gameObj.AddComponent<T>();
        }
    }

    public void RemoveDuplicates()
    {
        if (s_Instance == null)
        {
            s_Instance = this as T;
        }
        else if (s_Instance != this)
        {
            Destroy(gameObject);
        }
    }
}