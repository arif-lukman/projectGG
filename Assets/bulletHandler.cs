using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletHandler : MonoBehaviour {

	public Rigidbody2D rb2d;
	public BoxCollider2D bc2d;

	public float spd;

	void Start () {
		rb2d = GetComponent<Rigidbody2D> ();
		rb2d.freezeRotation = true;
		rb2d.AddForce (new Vector2(spd*transform.localScale.x,0));
	}

	//cek collision, kalau bentur tembok atau musuh, hancurkan instance
	void OnCollisionEnter2D(Collision2D collision){
		if (collision.gameObject.tag == "Background") {
			Destroy (gameObject);
		}
		if (collision.gameObject.tag == "Monster") {
			collision.gameObject.SendMessage ("ApplyDamage", 10);
			Destroy (gameObject);
		}
	}
}
