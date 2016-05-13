using UnityEngine;

public abstract class CreationSingleton<T> : MonoBehaviour where T : CreationSingleton<T> 
{
    private static T m_Instance;
    public static T Instance
    {
        get
        {
            // Instance requiered for the first time, we look for it
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType(typeof(T)) as T;

                // Object not found, we create a temporary one
                if (m_Instance == null)
                {
                    m_Instance = new GameObject("Temp Instance of " + typeof(T), typeof(T)).GetComponent<T>();

                    // Problem during the creation, this should not happen
                    if (m_Instance == null)
                    {
                        Debug.LogError("Problem during the creation of " + typeof(T));
                    }
                }
            }

            return m_Instance;
        }
    }

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this as T;
        }
    }
}
