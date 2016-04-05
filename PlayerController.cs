using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private float moveSpeed = 15;
	public float rotateSpeed = 50;
	public float jumpForce = 2;
	private Vector3 moveDirection;
	float nextPort = 0;
	public float portalCD = 1.0f;

	private FauxGravityBody myBody;

	void Start() {
		myBody = GetComponent<FauxGravityBody> ();
	}

	void Update() {
		moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical")).normalized;

		float debugAngle = CameraAngle ();
		print (debugAngle);
	}

	void FixedUpdate() {
		transform.Rotate (0, Input.GetAxis ("Mouse X") * rotateSpeed * Time.deltaTime, 0);
		transform.GetChild (0).transform.RotateAround (transform.position, transform.right, -Input.GetAxis ("Mouse Y") * rotateSpeed * Time.deltaTime);

		GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + transform.TransformDirection(moveDirection) * moveSpeed * Time.deltaTime);

		if (Input.GetKeyDown(KeyCode.Space) && Physics.Raycast(transform.position, - transform.position + myBody.attractor.transform.position, 2.1f)) {
			GetComponent<Rigidbody> ().AddForce (jumpForce * (transform.position - myBody.attractor.transform.position).normalized);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (Time.time > nextPort) {
			nextPort = Time.time + portalCD;
			Portal portal = other.GetComponent<Portal> ();
			transform.position = portal.partner.position;
			myBody.attractor = portal.partner.GetComponent<Portal>().parent;
		}
	}

	float CameraAngle() {
		Vector3 u = transform.forward;
		Vector3 v = transform.GetChild (0).transform.forward;
		float radTheta = Mathf.Acos (Vector3.Dot (u, v) / u.magnitude * v.magnitude);
		float degTheta = radTheta * Mathf.Rad2Deg;
		return degTheta;
	}

	float ClampAngle(float angle, float min, float max) {
		if (angle < 0f) {
			angle += 360f;
		}
		if (angle > 360f) {
			angle -= 360f;
		}
		return Mathf.Clamp (angle, min, max);
	}
}
