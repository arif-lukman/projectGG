using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class stageHandler : MonoBehaviour {

	//ambil UI Text
	public Text textUI;

	//Init variabel
	//Jenis stage
	//0 : Normal
	//1 : Boss
	public int jenisStage = 1;

	//Timer
	//dalam detik
	public float timer;

	//Jeda spawner
	public float jedaSpawn;

	//HP boss
	public int bossHP;

	// Use this for initialization
	void Start () {
		if(jenisStage == 0){
			//Normal stage
			//tampilin timer
			setTextUI();
		}
		else{
			//Boss stage
			//spawn boss
		}
	}
	
	// Update is called once per frame
	void Update () {
		//jika normal stage dan timer habis
		if ((jenisStage == 0 && timer <= 0) || (jenisStage == 1 && bossHP <= 0)) {
			//akhiri game
			textUI.text = "Game Over.";
		}
		else if (jenisStage == 0) {
			//kurangi timer
			timer -= Time.deltaTime;
			//tampilin di gui
			setTextUI();
		}
		else {
			//tampilin hp boss
			textUI.text = "Boss' HP = " + bossHP.ToString();
		}
	}

	void setTextUI(){
		textUI.text = "Time left : " + timer.ToString();
	}
}
