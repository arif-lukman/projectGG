using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerHandler : MonoBehaviour {

	//var rigidbody2d player
	public Rigidbody2D rb2d;

	//var boxcollider2d player
	public BoxCollider2D bc2d;

	//var UI debug
	public Text textUI;

	//var debug
	private Vector2 move;

	//var kecepatan gerak
	private float mvSpd = 0.5f;

	//var kecepatan loncat
	private float jmpSpd = 30f;

	//var boleh loncat apa engga
	private bool canJump;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D> ();
		bc2d = GetComponent<BoxCollider2D> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//var buat ambil input horizontal
		float hor = 0.0f;
		if (Input.GetKey (KeyCode.RightArrow)) {
			//loncat
			hor = mvSpd;
		}
		else if (Input.GetKey (KeyCode.LeftArrow)) {
			//turun
			hor = -mvSpd;
		}

		//var buat ambil input up down
		float ver = 0.0f;
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			//loncat
			ver = jmpSpd;
		}
		else if (Input.GetKeyDown (KeyCode.DownArrow)) {
			//turun
			ver = -jmpSpd;
		}

		//set vector perubahan
		move = new Vector2(hor, ver);

		//tampilin debug
		tampilDebug();

		//cek keyboard
		//panah kanan = gerak ke kanan
		//panah kiri = gerak ke kiri
		rb2d.AddForce (move);

		//panah atas = loncat
		//panah bawah kalo di platform kedua = loncat ke bawah
	}

	//Nampilin debug
	void tampilDebug(){
		textUI.text = "move = " + move.ToString ();
	}
}
