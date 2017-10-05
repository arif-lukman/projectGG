using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeSwingHandler : MonoBehaviour {

	//box collider
	public BoxCollider2D bc2d;

	//delay swing
	//private float swgDly;

	// Use this for initialization
	void Start () {
		//swgDly = 0.05f;
		bc2d = GetComponent<BoxCollider2D> ();
		//new WaitForSeconds (0.05f);
		//if (swgDly <= 0f) {
		//	Destroy (gameObject);
		//} else {
		//	swgDly -= Time.deltaTime;
		//}
		//Destroy(gameObject);
		StartCoroutine(destroyObject());
	}
	
	void OnTriggerEnter2D(Collider2D collider){
		if (collider.gameObject.tag == "Player") {
			collider.gameObject.SendMessage ("ApplyDamage", 10);
		}
	}

	IEnumerator destroyObject(){
		yield return new WaitForSeconds (0.1f);
		Destroy(gameObject);
	}
}
