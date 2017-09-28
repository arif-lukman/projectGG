using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAIMelee : MonoBehaviour {

	//box collider
	private BoxCollider2D bc2d;

	//rigidbody
	private Rigidbody2D rb2d;

	//states
	//1. idle top
	//2. idle bot
	//3. aggro top
	//4. aggro bot
	//5. attacking
	//6. jump up
	//7. jump down
	//8. can jump
	//9. cannot jump
	public enum enemyState {IDLETOP, IDLEBOT, AGGROTOP, AGGROBOT, ATTACK, JUMPUP, JUMPDOWN};

	//kecepatan loncat
	public float jmpSpd;

	//kecepatan gerak
	public float movSpd;

	//HP
	public int hp;

	//damage
	public int dmg;

	//jeda serangan
	public int atkDly;

	//jarak aggro
	public float aggDst;

	//arah
	private Vector2 dir;

	//set state
	private enemyState state;

	//set jumpstate
	private bool canJump;

	//set objek player
	private GameObject player;

	//set script player
	private playerHandler playerScript;

	//objek platform
	private GameObject top;
	private GameObject bot;

	void Start(){
		player = GameObject.FindWithTag ("Player");
		playerScript = player.GetComponent<playerHandler> ();
		top = GameObject.FindWithTag ("Upper Platform");
		rb2d = GetComponent<Rigidbody2D> ();
		bc2d = GetComponent<BoxCollider2D> ();
	}

	void FixedUpdate(){
		//Debug.Log ("State = " +state);
		//ambil jarak antara player dan musuh
		float dist = Vector2.Distance (transform.position, player.transform.position);
		//Debug.Log( "Jarak = " + dist );

		//kalau masuk jarak, aggro
		if ( dist <= aggDst ) {
			if (state == enemyState.IDLETOP) {
				//aggro upper platform
				state = enemyState.AGGROTOP;
			}
			else if (state == enemyState.IDLEBOT) {
				//aggro lower platform
				state = enemyState.AGGROBOT;
			}
		}

		//panggil fungsi state handling
		StateHandling();
	}

	void OnCollisionStay2D(Collision2D collision){
		//cek collision antara objek ini dengan lantai
		if (collision.gameObject.tag == "Upper Platform" || collision.gameObject.tag == "Lower Platform") {
			canJump = true;
		}
		//Debug.Log ("Can Jump = " + canJump);

		if (collision.gameObject.tag == "Upper Platform") {
			state = enemyState.IDLETOP;
		}

		if (collision.gameObject.tag == "Lower Platform") {
			state = enemyState.IDLEBOT;
		}
	}

	void OnCollisionExit2D( Collision2D collision ){
		//cek collision antara objek ini dengan lantai
		if (collision.gameObject.tag == "Upper Platform" || collision.gameObject.tag == "Lower Platform") {
			canJump = false;
		}
	}

	//fungsi state handling
	void StateHandling(){
		switch (state) {
		case enemyState.AGGROTOP:
			//panggil fungsi aggrotop
			Aggro("TOP");
			break;
		case enemyState.AGGROBOT:
			//panggil fungsi aggrobot
			Aggro("BOT");
			break;
		}
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
		if (playerScript.state == playerHandler.playerState.BOT) {
			//Debug.Log ("BAWAH");
			Jump ("DOWN");
		}
	}

	void AggroBot(){
		if (playerScript.state == playerHandler.playerState.TOP) {
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
		rb2d.AddForce (dir);
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
