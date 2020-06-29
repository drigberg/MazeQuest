using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speed = 5.0f;
	public float turnSpeed = 0.2f;
	private Rigidbody rb;

	void Start() 
	{
		rb = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate () 
	{
		// move forwards/backwards
		float moveVertical = Input.GetAxis("Vertical");
		rb.AddForce(rb.transform.forward * moveVertical * speed);

		// strafe
		bool right = Input.GetKey(KeyCode.E);
		bool left = Input.GetKey(KeyCode.Q);
		if (right && !left) {
			rb.AddForce(rb.transform.right * speed);
		} else if (left && !right) {
			rb.AddForce(rb.transform.right * speed * -1);
		}

		// rotate
		float moveHorizontal = Input.GetAxis("Horizontal");
		TurnToFace(rb.transform.right * moveHorizontal);
	}

    public void TurnToFace(Vector3 targetDirection) {
        rb.AddTorque(Vector3.Cross(rb.transform.forward, targetDirection) * turnSpeed);
    }
}