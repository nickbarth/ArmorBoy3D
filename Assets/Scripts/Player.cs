using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public enum Direction { Up, Down, Backward, Forward };
	
	public static bool attacking;
	public static bool dead;
	public static GameObject lastCheckPoint;

	public Animator anim;
	
	public GameObject swordStrike;
	public GameObject DeathParticles;

	private GameObject particles;

	private bool grounded;
	private bool walledRight;
	private bool walledLeft;
	private bool bouncing;

	private bool faceRight;
	private float coolDown;
	private RaycastHit hit;

	private float alpha;
	private float moveSpeed;
	private SpriteRenderer sprite; 

	void Start() {
		alpha = 1f;
		dead = false;
		attacking = false;
		coolDown = 0;
		faceRight = true;
		renderer.castShadows = true;
		renderer.receiveShadows = true;
		rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, 0.5f);
		sprite = gameObject.GetComponent<SpriteRenderer>();
	}
	
	void FixedUpdate() {
		float h = Input.GetAxis("Horizontal");

		if (GameManager.levelCompleted) return;

		if (h != 0 && !attacking) {
			Move (h);
		} else if (moveSpeed != 0 && !attacking) {
			Move (moveSpeed);
		} else {
			anim.SetBool("Walking", false);
		}
	}

	public void MoveSpeed(float s) {
		moveSpeed = s;
	}

	public void Move(float h) {
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
			attacking = true;
			anim.SetBool("Attacking", true);

			GameManager.MakeBreakablesTrigger();

			int noEnemiesLayer = 1 << 8;
			walledRight = Physics.Raycast(transform.position, Vector3.right, out hit, 0.2f, ~noEnemiesLayer);
			walledLeft = Physics.Raycast(transform.position, Vector3.left, out hit, 0.2f, ~noEnemiesLayer);

			if (attacking && !faceRight & walledLeft) {
				rigidbody.velocity = new Vector3(3f, -2f, 0f);
			} else if (attacking && faceRight & walledRight) {
				rigidbody.velocity = new Vector3(-3f, -2f, 0f);
			} else {
				rigidbody.velocity = new Vector3(faceRight ? 10.0f : -10.0f, 0f, 0f);
			}

			coolDown = 1f;
			
			var pos = new Vector3(transform.position.x + (faceRight ? 0.4f : -0.4f), transform.position.y, transform.position.z);
			particles = Instantiate(swordStrike, pos, Quaternion.Euler(0, (faceRight ? -90f : 90f), 0)) as GameObject;
			Destroy(particles, 1f);
		}
	}

	void Update() {
		Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 0.4f, Color.green);
		Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * 0.1f, Color.red);
		Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * 0.1f, Color.blue);

		if (alpha < 1f) {
			alpha += Time.deltaTime * 2;
			sprite.color = new Color(sprite.material.color.r, sprite.material.color.g, sprite.material.color.b, alpha);
		}

		if (GameManager.levelCompleted) return;
		
		int noEnemiesLayer = 1 << 8;
		walledRight = Physics.Raycast(transform.position, Vector3.right, out hit, 0.2f, ~noEnemiesLayer);
		walledLeft = Physics.Raycast(transform.position, Vector3.left, out hit, 0.2f, ~noEnemiesLayer);

		grounded = Physics.Raycast(transform.position, Vector3.down, out hit, 0.4f, ~noEnemiesLayer); 
		if (grounded) {
			bouncing = false;
		}
		
		if (grounded && hit.collider.gameObject.tag == "Fallable") {
			hit.collider.gameObject.GetComponent<Fallable>().Fall();
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
			if (attacking) {
				rigidbody.velocity = new Vector3(rigidbody.velocity.x * 0.5f, rigidbody.velocity.y * 0.5f, 0f);
			}
			anim.SetBool("Attacking", false);
			attacking = false;
			GameManager.MakeBreakablesTrigger(false);
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

		if (collider.gameObject.tag == "BadTouch" || (collider.gameObject.tag == "Enemy" && !attacking)) {
			Kill();
		}
	}

	void OnTriggerStay(Collider collider) {
		if (collider.gameObject.tag == "LevelPoint") {
			GameManager.levelCompleted = true;

			anim.SetBool("Attacking", false);
			anim.SetBool("Walking", true);

			if (transform.position.x < collider.gameObject.transform.position.x) {
				rigidbody.velocity = new Vector3(0f, 1f, 0f);
				float step = 2f * Time.deltaTime;
				transform.position = Vector3.MoveTowards(transform.position, collider.gameObject.transform.position, step);
			} else {
				rigidbody.velocity = new Vector3(0f, 5f, 0f);
			}
		}
	}

	public void Bounce(Direction direction) {
		if (bouncing) return;
		bouncing = true;
		
		if (direction == Direction.Backward) {
			rigidbody.velocity = new Vector3(faceRight ? -10f : 10f, 5f, 0f);
		}
	}
	
	public void Kill() {
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
		lastCheckPoint.gameObject.GetComponent<CheckPoint>().Respawn();
		yield return new WaitForSeconds(1);		
		gameObject.renderer.enabled = true;
		dead = false;
		collider.enabled = true;
		transform.position = new Vector3(lastCheckPoint.transform.position.x, lastCheckPoint.transform.position.y, transform.position.z);
		alpha = 0f;
		sprite.color = new Color(sprite.material.color.r, sprite.material.color.g, sprite.material.color.b, 0);
	}
}
