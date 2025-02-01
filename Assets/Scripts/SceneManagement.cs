using UnityEngine;

public class SceneManagement : MonoBehaviour
{
    // Singleton instance for easy access
    public static SceneManagement Instance { get; private set; }

    // Property to store the transition name
    public string SceneTransitionName { get; private set; }

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene loads
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    // Method to set the transition name
    public void SetTransitionName(string sceneTransitionName)
    {
        SceneTransitionName = sceneTransitionName;
    }
}