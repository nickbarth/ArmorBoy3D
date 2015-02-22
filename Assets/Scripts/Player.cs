using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public Animator anim;
	public bool grounded;
	public bool walledRight;
	public bool walledLeft;
	public bool breaking;

	public bool faceRight;
	public float coolDown;
	public RaycastHit hit;

	public float moveSpeed;

	public GameObject swordStrike;
	public GameObject DeathParticles;
	private GameObject particles;

	public static bool dead;
	public static GameObject lastCheckPoint;
	
	private static bool attacking;
	public static bool isAttacking() {
		return attacking;
	}
	public bool levelCompleted;

	void Start () {
		levelCompleted = false;
		dead = false;
		attacking = false;
		coolDown = 0;
		faceRight = true;
		renderer.castShadows = true;
		renderer.receiveShadows = true;
		rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, 0.5f);
	}
	
	void FixedUpdate () {
		float h = Input.GetAxis("Horizontal");

		if (levelCompleted) return;

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

		if ((!walledRight && h > 0 || !walledLeft && h < 0) && !dead) {
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
		if (coolDown == 0f && !dead) {

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
		Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z), transform.TransformDirection(faceRight ? Vector3.right : Vector3.left) * 0.5f, Color.yellow);

		if (levelCompleted) return;
		
		int noEnemiesLayer = 1 << 8;

		walledRight = Physics.Raycast(transform.position, Vector3.right, out hit, 0.1f, ~noEnemiesLayer);
		walledLeft = Physics.Raycast(transform.position, Vector3.left, out hit, 0.4f, ~noEnemiesLayer);


		grounded = Physics.Raycast(transform.position, Vector3.down, out hit, 0.4f, ~noEnemiesLayer); 
		if (grounded && hit.collider.gameObject.tag == "Fallable") {
			hit.collider.gameObject.GetComponent<Fallable>().Fall();
		}

		breaking = Physics.Raycast(transform.position, faceRight ? Vector3.right : Vector3.left, out hit, 0.5f, ~9); 
		if (breaking && hit.collider.gameObject.tag == "Breakable" && attacking) {
			hit.collider.gameObject.GetComponent<Breakable>().Break();
			rigidbody.velocity = new Vector3(rigidbody.velocity.x / 2, rigidbody.velocity.y / 2, rigidbody.velocity.z / 2);
		}

		if (particles) {
			var pos = new Vector3(transform.position.x + (faceRight ? 0.4f : -0.4f), transform.position.y, transform.position.z);
			particles.transform.position = pos;
			particles.transform.rotation = Quaternion.Euler(0, (faceRight ? -90f : 90f), 0);
		}
		
		if (attacking && !faceRight & walledLeft) {
			rigidbody.velocity = new Vector3(3f, -2f, 0f);
		}
		
		if (attacking && faceRight & walledRight) {
			rigidbody.velocity = new Vector3(-3f, -2f, 0f);
		}

		if (Input.GetKey(KeyCode.UpArrow)) {
			Jump();
		}

		if (Input.GetKey(KeyCode.Space)) {
			Attack();
		}
		
		if (coolDown != 0f) {
			coolDown += Time.deltaTime;
		}
		
		if (coolDown >= 1.5f) {
			rigidbody.velocity = new Vector3(0f, rigidbody.velocity.y, 0f);
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

		if (collision.gameObject.tag == "Enemy" && !attacking) {
			Kill();
		}
	}

	void OnTriggerEnter(Collider collider) {
		if (collider.gameObject.tag == "CheckPoint") {
			lastCheckPoint = collider.gameObject;
			collider.gameObject.SetActive(false);
		}

		if (collider.gameObject.tag == "BadTouch") {
			Kill();
		}
	}

	void OnTriggerStay(Collider collider) {
		if (collider.gameObject.tag == "LevelPoint") {
			levelCompleted = true;
			GameFader.EndScene();

			if (transform.position.x < collider.gameObject.transform.position.x) {
				rigidbody.velocity = new Vector3(0f, 1f, 0f);
				float step = 2f * Time.deltaTime;
				transform.position = Vector3.MoveTowards(transform.position, collider.gameObject.transform.position, step);
			} else {
				rigidbody.velocity = new Vector3(0f, 5f, 0f);
			}
		}
	}

	void Kill() {
		Object particles = Instantiate(DeathParticles, transform.position, Quaternion.identity);
		Destroy(particles, 1f);

		rigidbody.velocity = new Vector3(0f, 0f, 0f);

		if (!dead) {
			StartCoroutine(Respawn());
		}
	}

	IEnumerator Respawn () {
		dead = true;
		collider.enabled = false;
		gameObject.renderer.enabled = false;
		yield return new WaitForSeconds(1);		
		gameObject.renderer.enabled = true;
		dead = false;
		collider.enabled = true;
		transform.position = new Vector3(lastCheckPoint.transform.position.x, lastCheckPoint.transform.position.y, transform.position.z);
	}
}
