using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerHandler : MonoBehaviour {

	//enum player state
	//player states
	//1. top
	//2. bot
	public enum playerState {TOP, BOT, DOWN};

	//var boxcollider2d player
	public BoxCollider2D bc2d;

	//var rigidbody2d player
	public Rigidbody2D rb2d;

	//var UI debug
	public Text textUI;

	//var kecepatan gerak
	public float movSpd;

	//var kecepatan loncat
	public float jmpSpd;

	//var boleh loncat apa engga
	private bool canJump;

	//set state
	public playerState state;

	//objek upper platform
	private GameObject top;

	//nyimpen vektor2 arah
	private Vector2 dir;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D> ();
		bc2d = GetComponent<BoxCollider2D> ();
		top = GameObject.FindWithTag ("Upper Platform");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//input horizontal
		if (Input.GetKey (KeyCode.RightArrow)) {
			//kanan
			Move("RIGHT");
		}
		else if (Input.GetKey (KeyCode.LeftArrow)) {
			//kiri
			Move("LEFT");
		}
		//input vertikal
		if (Input.GetKeyDown (KeyCode.UpArrow) && state == playerState.TOP) {
			//loncat
			Jump("UPTOP");
		}
		else if (Input.GetKeyDown (KeyCode.UpArrow) && state == playerState.BOT) {
			//loncat
			Jump("UPBOT");
		}
		else if (Input.GetKeyDown (KeyCode.DownArrow) && state == playerState.TOP) {
			//turun
			Jump("DOWN");
			state = playerState.DOWN;
		}
		if(!canJump && transform.position.y > top.transform.position.y && state != playerState.DOWN){
			Physics2D.IgnoreCollision (bc2d, top.GetComponent<BoxCollider2D> (), false);
		}
		//panah atas = loncat
		//panah bawah kalo di platform kedua = loncat ke bawah
	}

	void OnCollisionStay2D(Collision2D collision){
		//cek collision antara objek ini dengan lantai
		if (collision.gameObject.tag == "Upper Platform" || collision.gameObject.tag == "Lower Platform") {
			canJump = true;
		}
		//Debug.Log ("Can Jump = " + canJump);
		if (collision.gameObject.tag == "Upper Platform") {
			state = playerState.TOP;
		}

		if (collision.gameObject.tag == "Lower Platform") {
			state = playerState.BOT;
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
}
