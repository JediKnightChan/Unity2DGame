using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    // Singleton instance for the CameraController
    private static CameraController instance;

    // Public property to access the singleton instance
    public static CameraController Instance
    {
        get { return instance; }
    }

    // Reference to the CinemachineCamera component
    private CinemachineCamera cinemachineCamera;

    private void Awake()
    {
        // Ensure only one instance of CameraController exists
        if (instance != null && instance != this)
        {
            // If another instance exists, destroy this one
            Destroy(this.gameObject);
        }
        else
        {
            // Set this as the singleton instance
            instance = this;
        }

        // Optionally, prevent the CameraController from being destroyed on scene load
        if (!gameObject.transform.parent)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        // Set the camera to follow the player when the scene starts
        SetPlayerCameraFollow();
    }

    public void SetPlayerCameraFollow()
    {
        // Find the first CinemachineCamera in the scene
        cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
        // Set the camera's follow target to the player's transform
        cinemachineCamera.Follow = Player.Instance.transform;
    }
}