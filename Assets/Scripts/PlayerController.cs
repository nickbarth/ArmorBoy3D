using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public Animator anim;
	public bool grounded;
	public bool walledRight;
	public bool walledLeft;
	public bool faceRight;
	public float coolDown;
	public RaycastHit hit;

	public float moveSpeed;

	public GameObject swordStrike;
	public GameObject DeathParticles;

	private GameObject particles;
	private static bool attacking;
	private bool isDead;

	public GameObject lastCheckPoint;

	public static bool isAttacking () {
		return attacking;
	}

	void doTestFunction () {
		bool lol = true;
	}

	/*
	private void OnGUI(){

		if (GUI.RepeatButton (new Rect (20, 500, 100, 100), "<")) {
			Move(-0.5f);
		}
		
		if (GUI.RepeatButton (new Rect (130, 500, 100, 100), ">")) {
			Move(0.5f);
		}
		
		if(GUI.RepeatButton(new Rect(840, 500, 100, 100), "^")){
			Jump();
		}

		if(GUI.RepeatButton(new Rect(950, 500, 100, 100), "O")){
			Attack();
		}
	}
	*/

	void Start () {
		isDead = false;
		attacking = false;
		coolDown = 0;
		faceRight = true;
		renderer.castShadows = true;
		renderer.receiveShadows = true;
		rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, 0.5f);
	}
	
	void FixedUpdate () {
		float h = Input.GetAxis("Horizontal");

		if (h != 0) {
				Move (h);
		} else if (moveSpeed != 0) {
				Move (moveSpeed);
		} else {
			anim.SetBool("Walking", false);
		}
	}

	public void MoveSpeed(float s) {
		moveSpeed = s;
	}

	public void Move (float h) {
		if (Mathf.Abs(h) > 0.01f) {
			anim.SetBool("Walking", true);
		}

		if ((!walledRight && h > 0 || !walledLeft && h < 0) && !isDead) {
			transform.Translate(new Vector3(h * 0.1f, 0, 0));
		}

		if (h < -0.01f && faceRight || h > 0.01f && !faceRight) {
			faceRight = !faceRight;
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
	}

	public void Jump () {
		if (grounded) {
			rigidbody.velocity = new Vector3(0f, 5.0f, 0f);
		}
	}

	public void Attack () {
		if (coolDown == 0f && !isDead) {
			anim.SetBool("Attacking", true);
			attacking = true;
			rigidbody.velocity = new Vector3(faceRight ? 10.0f : -10.0f, 0f, 0f);
			coolDown = 1f;
			
			var pos = new Vector3(transform.position.x + (faceRight ? 0.4f : -0.4f), transform.position.y, transform.position.z);
			particles = Instantiate(swordStrike, pos, Quaternion.Euler(0, (faceRight ? -90f : 90f), 0)) as GameObject;
			Destroy(particles, 1f);
		}
	}

	void Update () {
		Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 0.4f, Color.green);
		Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * 0.1f, Color.red);
		Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * 0.4f, Color.blue);

		walledRight = Physics.Raycast(transform.position, Vector3.right, out hit, 0.1f); 
		walledLeft = Physics.Raycast(transform.position, Vector3.left, out hit, 0.4f); 
		grounded = Physics.Raycast(transform.position, Vector3.down, out hit, 0.4f); 

		if (particles) {
			var pos = new Vector3(transform.position.x + (faceRight ? 0.4f : -0.4f), transform.position.y, transform.position.z);
			particles.transform.position = pos;
			particles.transform.rotation = Quaternion.Euler(0, (faceRight ? -90f : 90f), 0);
		}

		if (Input.GetKey(KeyCode.UpArrow)) {
			Jump ();
		}

		if (Input.GetKey(KeyCode.Space)) {
			Attack();
		}
		
		if (coolDown != 0f) {
			coolDown += Time.deltaTime;
		}
		
		if (coolDown >= 1.5f) {
			anim.SetBool("Attacking", false);
			attacking = false;
		}

		if (coolDown >= 2.0f) {
			coolDown = 0f;
		}
	}
	
	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "BadTouch") {
			Kill();
		}
	}

	void OnTriggerEnter(Collider collider) {
		if (collider.gameObject.tag == "CheckPoint") {
			lastCheckPoint = collider.gameObject;
		}

		if (collider.gameObject.tag == "BadTouch") {
			Kill();
		}
	}

	void Kill() {
		Object particles = Instantiate(DeathParticles, transform.position, Quaternion.identity);
		Destroy(particles, 1f);
		if (!isDead) {
			StartCoroutine (Respawn ());
		}
	}

	IEnumerator Respawn () {
		isDead = true;
		gameObject.renderer.enabled = false;
		yield return new WaitForSeconds(1);		
		gameObject.renderer.enabled = true;
		isDead = false;
		transform.position = new Vector3(lastCheckPoint.transform.position.x, lastCheckPoint.transform.position.y, transform.position.z);
	}
}
