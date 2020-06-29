using UnityEngine;

public class CameraController : MonoBehaviour {

    public Rigidbody player;
    public float followDistance = 10.5f;
    public float height = 1.0f;

    void Update() {
        if (player) {
            transform.position = player.transform.position - player.transform.forward * followDistance + player.transform.up * height;
            transform.rotation = player.rotation;
        }
    }
}