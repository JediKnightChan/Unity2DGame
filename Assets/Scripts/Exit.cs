using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    [SerializeField] private string sceneToLoad; // Name of the scene to load
    [SerializeField] private string IDToLoad; // Unique ID for the transition

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player
        if (other.gameObject.GetComponent<Player>())
        {
            // Set the transition name in the static class
            SceneManagement.Instance.SetTransitionName(IDToLoad);
            // Load the new scene
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}