using UnityEngine;

public class CameraController : MonoBehaviour {

    public Rigidbody player;
    public float followDistance = 10.5f;
    public float height = 1.0f;
    public float mazeSideLength;
    public float turnSpeed = 0.2f;
    public Camera mainCamera;

    void Start() {
        mainCamera = GetComponent<Camera>();
        mazeSideLength = 1.0f;
    }

    void Update() {
        if (player) {
            mainCamera.orthographic = false;
            transform.position = player.transform.position - player.transform.forward * followDistance + player.transform.up * height;
            transform.rotation = player.rotation;
        } else {
            mainCamera.orthographic = true;
            transform.position = new Vector3(
                mazeSideLength / 2.0f,
                mazeSideLength * 1.0f,
                mazeSideLength / 2.0f
            );
            transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
            mainCamera.orthographicSize = mazeSideLength * 0.5f + 5.0f;
        }
    }
}