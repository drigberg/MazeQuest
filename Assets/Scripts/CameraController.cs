using UnityEngine;

public class CameraController : MonoBehaviour {
    // objects
    public Rigidbody player;
    public Camera mainCamera;
    public MazeGenerator mazeGenerator;

    // player follow parameters
    public float followDistance = 10.5f;
    public float height = 1.0f;
    public float turnSpeed = 0.2f;
    public bool lockedToPlayer = false;

    // glide parameters
    public bool glidingToAerialView = false;
    private float localTime = 0.0f;
    private float glideTime = 1.0f;
    private Vector3 glideStartPosition;
    private Quaternion glideStartRotation;
    private Vector3 glideTargetPosition;
    private Quaternion glideTargetRotation;

    void Start() {
        mainCamera = GetComponent<Camera>();
    }

    void Update() {
        if (glidingToAerialView) {
            GlideToAerialView();
        } else if (player && lockedToPlayer) {
            FollowPlayer();
        } else {
            transform.position = GetAerialViewPosition();
            transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
        }
    }

    public void StartGlideToAerialView() {
        lockedToPlayer = false;
        localTime = 0.0f;
        glidingToAerialView = true;
        glideStartPosition = transform.position;
        glideStartRotation = transform.rotation;
        glideTargetPosition = GetAerialViewPosition();
        glideTargetRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
    }

    public void GlideToAerialView() {
        localTime += Time.deltaTime / glideTime;
        transform.position = Vector3.Lerp(glideStartPosition, glideTargetPosition, localTime);
        // transform.eulerAngles = Vector3.Lerp(glideStartRotation, glideTargetRotation, localTime);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            glideTargetRotation,
            localTime);
        if (localTime >= glideTime) {
            glidingToAerialView = false;
        }
    }

    Vector3 GetAerialViewPosition() {
        if (mazeGenerator.uiController.state == "START_MENU") {
            return mazeGenerator.uiController.cameraPosition;
        } else {
            return new Vector3(
                mazeGenerator.mazeSideLength / 2.0f,
                (int)Mathf.Ceil(mazeGenerator.mazeSideLength * 0.914f + 11.43f),
                mazeGenerator.mazeSideLength / 2.0f
            );
        }
    }

    void FollowPlayer() {
        transform.position = player.transform.position - player.transform.forward * followDistance + player.transform.up * height;
        transform.rotation = player.rotation;
    }
}