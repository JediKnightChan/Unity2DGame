using UnityEngine;

public class Entrance : MonoBehaviour
{
    // Unique ID for this entrance. Used to match the player's spawn point when transitioning between scenes.
    [SerializeField] private string entranceID;

    private void Start()
    {
        // Check if this entrance's ID matches the transition name stored in the SceneManagement singleton.
        if (entranceID == SceneManagement.Instance.SceneTransitionName)
        {
            // If it matches, move the player to this entrance's position.
            Player.Instance.transform.position = this.transform.position;
            CameraController.Instance.SetPlayerCameraFollow(); // Update camera follow
        }
    }
}