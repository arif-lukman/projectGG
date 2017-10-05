using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAIMelee : MonoBehaviour {

	//box collider
	private BoxCollider2D bc2d;

	//rigidbody
	private Rigidbody2D rb2d;

	//motion state
	//1. moving randomly
	//2. aggro
	//2. attacking
	//3. knockbacked
	public enum mtnState {IDLETOP, IDLEBOT, AGGROTOP, AGGROBOT, ATTACKING, KNOCKBACKED};

	//facing state
	//1. left
	//2. right
	public enum facState {LEFT, RIGHT};

	//kecepatan loncat
	public float jmpSpd;

	//kecepatan gerak
	public float movSpd;

	//hp
	public int hp;

	//damage
	public int dmg;

	//jeda serangan
	public int atkDly;

	//jarak aggro
	public float aggDst;

	//batas kecepatan
	public float topSpd;

	//melee swing
	public Transform swing;

	//atk distance
	public float atkDst;

	//arah
	private Vector2 dir;

	//sprite renderer
	private SpriteRenderer sp;

	//set state
	private facState facSt;
	private mtnState mtnSt;

	//set jumpstate
	private bool canJump;

	//set attackstate
	private bool canAtk;

	//atk delay tersisa
	private float atkDlyLeft;

	//set objek player
	private GameObject player;

	//set script player
	private playerHandler playerScript;

	//objek platform
	private GameObject top;
	private GameObject bot;
	private GameObject[] enemies;

	void Start(){
		atkDlyLeft = atkDly;
		canAtk = true;
		player = GameObject.FindWithTag ("Player");
		playerScript = player.GetComponent<playerHandler> ();
		top = GameObject.FindWithTag ("Upper Platform");
		rb2d = GetComponent<Rigidbody2D> ();
		bc2d = GetComponent<BoxCollider2D> ();
		sp = GetComponent<SpriteRenderer> ();
		rb2d.freezeRotation = true;
	}

	void FixedUpdate(){
		//Debug.Log ("State = " +state);
		//ambil jarak antara player dan musuh
		float dist = Vector2.Distance (transform.position, player.transform.position);
		//Debug.Log( "Jarak = " + dist );

		//kalau masuk jarak, aggro
		if (dist <= aggDst) {
			if (mtnSt == mtnState.IDLETOP) {
				//aggro upper platform
				mtnSt = mtnState.AGGROTOP;
			} else if (mtnSt == mtnState.IDLEBOT) {
				//aggro lower platform
				mtnSt = mtnState.AGGROBOT;
			}
		} else {
			if (mtnSt == mtnState.AGGROTOP) {
				//aggro upper platform
				mtnSt = mtnState.IDLETOP;
			} else if (mtnSt == mtnState.AGGROBOT) {
				//aggro lower platform
				mtnSt = mtnState.IDLEBOT;
			}
		}

		enemies = GameObject.FindGameObjectsWithTag ("Monster");
		foreach (GameObject e in enemies) {
			Physics2D.IgnoreCollision (bc2d, e.GetComponent<BoxCollider2D> (), true);
		}

		//Debug.Log (IsPlayerAttackable());
		if (IsPlayerAttackable()) {
			mtnSt = mtnState.ATTACKING;
		}
		//Debug.Log (aggSt);
		//panggil fungsi state handling
		StateHandling();
		//fungsi cek darah
		HPChecker ();
		//fungsi cooldown
		CooldownHandling ();
	}

	void OnCollisionStay2D(Collision2D collision){
		//cek collision antara objek ini dengan lantai
		if (collision.gameObject.tag == "Upper Platform" || collision.gameObject.tag == "Lower Platform") {
			canJump = true;
		}
		//Debug.Log ("Can Jump = " + canJump);

		if (collision.gameObject.tag == "Upper Platform") {
			mtnSt = mtnState.IDLETOP;
		}

		if (collision.gameObject.tag == "Lower Platform") {
			mtnSt = mtnState.IDLEBOT;
		}
	}

	void OnCollisionExit2D( Collision2D collision ){
		//cek collision antara objek ini dengan lantai
		if (collision.gameObject.tag == "Upper Platform" || collision.gameObject.tag == "Lower Platform") {
			canJump = false;
		}
	}

	//fungsi cooldown handling
	void CooldownHandling(){
		if (!canAtk) {
			CooldownAttack ();
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

		//aggro state
		/*
		switch (aggSt) {
		case aggState.AGGROTOP:
			//panggil fungsi aggrotop
			Aggro("TOP");
			break;
		case aggState.AGGROBOT:
			//panggil fungsi aggrobot
			Aggro("BOT");
			break;
		case aggState.NOAGGRO:
			break;
		}
		*/

		//movement state
		switch (mtnSt) {
		case mtnState.IDLETOP:
			//panggil fungsi gerak random
			break;
		case mtnState.IDLEBOT:
			//panggil fungsi gerak random
			break;
		case mtnState.AGGROTOP:
			//fungsi aggro top
			Aggro("TOP");
			break;
		case mtnState.AGGROBOT:
			Aggro("BOT");
			break;
		case mtnState.ATTACKING:
			Attacking ();
			break;
		case mtnState.KNOCKBACKED:
			//Knockbacked (0.2f);
			break;
		}
	}

	//fungsi apply damage
	void ApplyDamage(int dmg){
		//if (state != enemyState.KNOCKBACKED) {
		hp -= dmg;
		//aggSt = aggState.NOAGGRO;
		mtnSt = mtnState.KNOCKBACKED;
		//}
	}

	void CheckFacing(){
		//if (mtnSt == mtnState.MOVING) {
		if (rb2d.velocity.x >= 0) {
			facSt = facState.RIGHT;
		} else {
			facSt = facState.LEFT;
		}
		//}
	}

	//fungsi cek HP
	void HPChecker(){
		if (hp <= 0) {
			Destroy (gameObject);
		}
	}

	//fungsi isplayerattackable
	bool IsPlayerAttackable(){
		//cek facing dan posisi player
		if ((transform.position.x < player.transform.position.x && player.transform.position.x < transform.position.x+atkDst &&
		    facSt == facState.RIGHT) ||
			(transform.position.x > player.transform.position.x && player.transform.position.x > transform.position.x-atkDst &&
				facSt == facState.LEFT) && transform.position.y == player.transform.position.y) {
			return true;
		} else {
			return false;
		}
	}

	//fungsi attack
	void Attacking(){
		//ambil lebar sprite
		//Debug.Log("Attacking");
		if (canAtk) {
			float bhalfwidth = swing.GetComponent<SpriteRenderer> ().sprite.rect.width / 200;
			float bpos = (sp.sprite.rect.width / 200) + bhalfwidth + 0.1f;
			Vector3 stx = new Vector3 (0, 0, 0);
			switch (facSt) {
			case facState.LEFT:
				stx = new Vector3 (transform.position.x - bpos, transform.position.y, 0);
				break;
			case facState.RIGHT:
				stx = new Vector3 (transform.position.x + bpos, transform.position.y, 0);
				break;
			}
			//instantiate swing
			Transform swingInstance = Instantiate (swing, stx, transform.rotation);
			swingInstance.transform.localScale = transform.localScale;
			//Debug.Log ("Spawning Enemy: " + swing);
			canAtk = false;
		}
		//balik ke move
		//mtnSt = mtnState.MOVING;
	}

	//atk delay
	void CooldownAttack(){
		if (atkDlyLeft <= 0) {
			atkDlyLeft = atkDly;
			canAtk = true;
		} else {
			atkDlyLeft -= Time.deltaTime;
			Debug.Log ("Next attack in " + atkDlyLeft + "s");
		}
	}

	/*fungsi knockbacked kampret
	void Knockbacked(float dur){
		//if (dur <= 0f) {
			//aggSt = _agg;
		//	movSt = movState.MOVING;
			//facSt = _fst;
		//} else {
			//aggSt = aggState.NOAGGRO;
		//	dur -= Time.deltaTime;
		//	Debug.Log (dur);
		//}
		//new WaitForSeconds(dur);
		//movSt = movState.MOVING;
		//aggSt = aggState.NOAGGRO;

		//for (float i = dur; i <= 0f; i -= Time.deltaTime) {
		//}
		//float i;
		//i = dur;
		//while (dur > 0f) {
		//	dur -= Time.deltaTime;
		//	Debug.Log (dur);
		//}
		//if (dur <= 0f) {
		//	movSt = movState.MOVING;
		//}
	}*/

	//fungsi facing
	void Facing(Vector3 _scaling){
		transform.localScale = _scaling;
	}

	//fungsi aggro
	void Aggro(string _platform){
		//cek posisi x player
		if (transform.position.x < player.transform.position.x) {
			//gerak ke kanan
			//Debug.Log ("KANAN");
			Move ("RIGHT");
		}
		else if (transform.position.x > player.transform.position.x) {
			//gerak ke kiri
			//Debug.Log ("KIRI");
			Move ("LEFT");
		}
		//cek posisi y
		switch(_platform){
		case "TOP":
			AggroTop ();
			break;
		case "BOT":
			AggroBot ();
			break;
		}
	}

	void AggroTop(){
		if (playerScript.posSt == playerHandler.posState.BOT) {
			//Debug.Log ("BAWAH");
			Jump ("DOWN");
		}
	}

	void AggroBot(){
		if (playerScript.posSt == playerHandler.posState.TOP) {
			//Debug.Log ("ATAS");
			Jump ("UP");
		}
	}

	//fungsi gerak
	void Move(string _dir){
		//gerak kanan
		if (_dir == "RIGHT") {
			dir = new Vector2 (movSpd, 0f);
		} 
		//gerak kiri
		else if (_dir == "LEFT") {
			dir = new Vector2 (-movSpd, 0f);
		}
		//batasin movespeednya
		if (rb2d.velocity.x >= -topSpd && rb2d.velocity.x <= topSpd) {
			rb2d.AddForce (dir);
			//mtnSt = mtnState.MOVING;
		}
		//panggil fungsi cek facing
		CheckFacing();
	}

	//fungsi loncat
	void Jump(string _dir){
		//loncat keatas
		if (_dir == "UP") {
			dir = new Vector2 (0f, jmpSpd);
		} 
		//loncat kebawah
		else if (_dir == "DOWN") {
			dir = new Vector2 (0f, 0f);
		}
		if (canJump) {
			Physics2D.IgnoreCollision (bc2d, top.GetComponent<BoxCollider2D> (), true);
			//bc2d.enabled = false;
			//Debug.Log (dir);
			rb2d.AddForce (dir);
		} else if(!canJump && transform.position.y > top.transform.position.y && _dir == "UP"){
			Physics2D.IgnoreCollision (bc2d, top.GetComponent<BoxCollider2D> (), false);
		}
	}
}
