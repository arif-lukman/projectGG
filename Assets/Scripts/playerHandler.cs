using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerHandler : MonoBehaviour {

	//BUGS
	//player bisa terputar
	//kalo nyentuh objek healing orb loncatnya tinggi

	//TODO:
	//1. Fix bug
	//2. Bikin attack mechanism

	//class skill
	[System.Serializable]
	public class Skill{
		public string name;
		public float cooldown;
		public float timeLeft;
		public bool available;

		/* FOR NEXT DEV
		//fungsi cooldown
		void Cooldown(){
			available = false;
			if (cooldown <= 0f) {
				available = true;
			} else {
				//countdown
				cooldown -= Time.deltaTime;
				Debug.Log ("Time left to next evasion: " + cooldown);
			}
		}*/
	}

	public Transform bullet;

	//enum position state
	//pembanding
	//1. top
	//2. bot
	public enum posState {TOP, BOT, DOWN};

	//enum facing state
	//pemisah fungsi
	//1. right
	//2. left
	public enum facState {RIGHT, LEFT};

	//enum motion state
	//pemisah fungsi
	//1. moving
	//2. attacking
	//3. evading
	//4. knockbacked
	public enum mtnState {MOVING, ATTACKING, EVADING, KNOCKBACKED};

	//var boxcollider2d player
	public BoxCollider2D bc2d;

	//var rigidbody2d player
	public Rigidbody2D rb2d;

	//var sprite player
	public SpriteRenderer sp;

	//var UI debug
	public Text textUI;

	//var kecepatan gerak
	public float movSpd;

	//var kecepatan loncat
	public float jmpSpd;

	//var boleh loncat apa engga
	private bool canJump;

	//set state
	public posState posSt;
	private facState facSt;
	private mtnState mtnSt = mtnState.MOVING;

	//objek upper platform
	private GameObject top;
	private GameObject[] enemies;

	//nyimpen vektor2 arah
	private Vector2 dir;

	//evasion
	public Skill[] skills;

	// Use this for initialization
	void Start () {
		skills [0].timeLeft = skills [0].cooldown;
		rb2d = GetComponent<Rigidbody2D> ();
		bc2d = GetComponent<BoxCollider2D> ();
		sp = GetComponent<SpriteRenderer> ();
		rb2d.freezeRotation = true;
		top = GameObject.FindWithTag ("Upper Platform");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		enemies = GameObject.FindGameObjectsWithTag ("Monster");
		foreach (GameObject e in enemies) {
			Physics2D.IgnoreCollision (bc2d, e.GetComponent<BoxCollider2D> (), true);
		}
		//state handling
		StateHandling ();
		CooldownHandling ();
	}

	void OnCollisionStay2D(Collision2D collision){
		//cek collision antara objek ini dengan lantai
		if (collision.gameObject.tag == "Upper Platform" || collision.gameObject.tag == "Lower Platform") {
			canJump = true;
		}
		//Debug.Log ("Can Jump = " + canJump);
		if (collision.gameObject.tag == "Upper Platform") {
			posSt = posState.TOP;
		}

		if (collision.gameObject.tag == "Lower Platform") {
			posSt = posState.BOT;
		}
		//Debug.Log (state);
		//Debug.Log ("Player is on the " + collision.gameObject.tag);
	}

	void OnCollisionExit2D(Collision2D collision){
		//cek collision antara objek ini dengan lantai
		if (collision.gameObject.tag == "Upper Platform" || collision.gameObject.tag == "Lower Platform") {
			canJump = false;
		}
		//Debug.Log ("Can Jump = " + canJump);
	}

	void CooldownHandling(){
		if (!skills [0].available) {
			CooldownEvasion ();
			//StartCoroutine (Cooldown (skills[0].cooldown, "EnableEvasion") );
			//Debug.Log ("Handling cooldown");
		}
	}

	//fungsi state handling
	void StateHandling(){
		//variabel
		//var scaling
		Vector3 scaling;

		//handing facing state
		switch(facSt) {
		//facing right
		case facState.RIGHT:
			scaling = new Vector3 (1, transform.localScale.y, transform.localScale.z);
			Facing (scaling);
			break;

		//facing left
		case facState.LEFT:
			scaling = new Vector3 (-1, transform.localScale.y, transform.localScale.z);
			Facing (scaling);
			break;
		}

		//handling motion state
		switch(mtnSt){
		//moving
		case mtnState.MOVING:
			Moving ();
			break;

		//attacking
		case mtnState.ATTACKING:
			Attacking ();
			break;

		//evading
		case mtnState.EVADING:
			Evading ();
			break;

		//knockbacked
		case mtnState.KNOCKBACKED:
			break;
		}
	}

	//fungsi cooldown
	void CooldownEvasion(){
		skills[0].available = false;
		if (skills[0].timeLeft <= 0f) {
			skills [0].timeLeft = skills [0].cooldown;
			skills[0].available = true;
		} else {
			//countdown
			skills[0].timeLeft -= Time.deltaTime;
			Debug.Log ("Time left to next evasion: " + skills[0].timeLeft);
		}
	}

	//fungsi state facing
	void Facing(Vector3 _scaling){
		transform.localScale = _scaling;
	}

	//fungsi state moving
	void Moving(){
		if (Input.GetKey (KeyCode.RightArrow)) {
			//kanan
			Move("RIGHT");
		}
		else if (Input.GetKey (KeyCode.LeftArrow)) {
			//kiri
			Move("LEFT");
		}
		//input Evasion
		if (Input.GetKeyDown (KeyCode.LeftShift) && skills[0].available == true) {
			//evade
			//Evade ();
			mtnSt = mtnState.EVADING;
		}
		//input vertikal
		if (Input.GetKeyDown (KeyCode.UpArrow) && posSt == posState.TOP) {
			//loncat
			Jump ("UPTOP");
		} else if (Input.GetKeyDown (KeyCode.UpArrow) && posSt == posState.BOT) {
			//loncat
			Jump ("UPBOT");
		} else if (Input.GetKeyDown (KeyCode.DownArrow) && posSt == posState.TOP) {
			//turun
			Jump ("DOWN");
			posSt = posState.DOWN;
		} else if (Input.GetKeyDown (KeyCode.Z)) {
			//serang
			mtnSt = mtnState.ATTACKING;
		}
		if(!canJump && transform.position.y > top.transform.position.y && posSt != posState.DOWN){
			Physics2D.IgnoreCollision (bc2d, top.GetComponent<BoxCollider2D> (), false);
		}
	}

	//fungsi gerak
	void Move(string _dir){
		//gerak kanan
		if (_dir == "RIGHT") {
			dir = new Vector2 (movSpd, 0f);
			facSt = facState.RIGHT;
		} 
		//gerak kiri
		else if (_dir == "LEFT") {
			dir = new Vector2 (-movSpd, 0f);
			facSt = facState.LEFT;
		}
		rb2d.AddForce (dir);
	}
		
	//fungsi evade
	void Evading(){
		//Debug.Log ("Evading.");
		float dir = 0f;
		Vector2 move;
		//Vector2 nullPhysics;
		//cek facing state
		switch (facSt) {
		case facState.RIGHT:
			dir = -movSpd;
			break;

		case facState.LEFT:
			dir = movSpd;
			break;
		}
		//berikan gaya
		//nullPhysics = new Vector2(0f, 0f);
		move = new Vector2(dir * 25, 0f);
		//rb2d.AddForce(nullPhysics);
		rb2d.AddForce(move);
		//disable evasion
		//canEva = false;
		skills[0].available = false;
		//ganti state
		mtnSt = mtnState.MOVING;
	}

	//fungsi cooldown
	/*void Cooldown(float _time, string _function){
		if (_time <= 0f) {
			//panggil fungsi
			SendMessage (_function);
		} else {
			//countdown
			_time -= Time.deltaTime;
			Debug.Log ("Time left to next evasion: " + _time);
		}
	}*/

	//fungsi enable evasion
	/*void EnableEvasion(){
		if (!skills[0].available) {
			skills[0].available = true;
		}
	}*/

	//fungsi loncat
	void Jump(string _dir){
		//loncat keatas
		if (_dir == "UPTOP" || _dir == "UPBOT") {
			dir = new Vector2 (0f, jmpSpd);
		} 
		//loncat kebawah
		else if (_dir == "DOWN") {
			dir = new Vector2 (0f, 0f);
		}
		//Debug.Log (_dir);
		if (canJump) {
			if (_dir != "UPTOP")
				Physics2D.IgnoreCollision (bc2d, top.GetComponent<BoxCollider2D> (), true);
			//bc2d.enabled = false;
			rb2d.AddForce (dir);
		}
		//Debug.Log ("my y = " +transform.position.y);
		//Debug.Log ("his y = " +top.transform.position.y);
	}

	void Attacking(){
		//ambil lebar sprite
		//Debug.Log("Attacking");
		float bhalfwidth = bullet.GetComponent<SpriteRenderer> ().sprite.rect.width / 200;
		float bpos = (sp.sprite.rect.width / 200) + bhalfwidth + 0.1f;
		Vector3 stx = new Vector3(0,0,0);
		switch (facSt) {
		case facState.LEFT:
			stx = new Vector3(transform.position.x-bpos, transform.position.y, 0);
			break;
		case facState.RIGHT:
			stx = new Vector3(transform.position.x+bpos, transform.position.y, 0);
			break;
		}
		//instantiate peluru
		Transform bulletInstance = Instantiate(bullet, stx, transform.rotation);
		bulletInstance.transform.localScale = transform.localScale;
		//Debug.Log("Spawning Enemy: " + bullet);
		//balik ke move
		mtnSt = mtnState.MOVING;
	}
}
