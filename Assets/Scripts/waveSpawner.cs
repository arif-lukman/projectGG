using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveSpawner : MonoBehaviour {

	public enum spawnState { SPAWNING, WAITING, COUNTING };

	[System.Serializable]
	public class Wave{
		public string name;
		public Transform enemy;
		public int count;
		public float rate;
	}

	public Wave[] waves;
	private int nextWave = 0;

	public Transform[] spawnPoints;

	public float timeBetweenWaves = 5f;
	private float waveCountdown;

	private float searchCountdown = 1f;

	public spawnState state = spawnState.COUNTING;

	void Start(){
		if (spawnPoints.Length == 0) {
			Debug.LogError ("No spawn points referenced.");
		}
		waveCountdown = timeBetweenWaves;
	}

	void Update(){
		if (state == spawnState.WAITING) {
			if (!EnemyIsAlive ()) {
				WaveCompleted();
			} else {
				return;
			}
		}

		if (waveCountdown <= 0f) {
			if (state != spawnState.SPAWNING) {
				StartCoroutine (SpawnWave (waves [nextWave]));
			}
		} else {
			waveCountdown -= Time.deltaTime;
		}
	}

	void WaveCompleted(){
		//Debug.Log ("Wave Completed!");

		state = spawnState.COUNTING;
		waveCountdown = timeBetweenWaves;

		if (nextWave + 1 > waves.Length - 1) {
			nextWave = 0;
			//Debug.Log ("ALL WAVES COMPLETED! Looping...");
		} else {
			nextWave++;
		}
	}

	bool EnemyIsAlive(){
		if (searchCountdown <= 0f) {
			//Debug.Log ("Cek musuh");
			searchCountdown = 1f;
			if (GameObject.FindGameObjectWithTag ("Monster") == null) {
				return false;
			}
		} else {
			searchCountdown -= Time.deltaTime;
		}
		return true;
	}

	IEnumerator SpawnWave (Wave _wave){
		//Debug.Log ("Spawning Wave: " + _wave.name);
		state = spawnState.SPAWNING;

		for (int i = 0; i < _wave.count; i++) {
			SpawnEnemy (_wave.enemy);
			yield return new WaitForSeconds (1f / _wave.rate);
		}

		state = spawnState.WAITING;
		yield break;
	}

	void SpawnEnemy(Transform _enemy){
		Transform _sp = spawnPoints[ Random.Range(0, spawnPoints.Length) ];
		Instantiate(_enemy, _sp.position, _sp.rotation);
		//Debug.Log("Spawning Enemy: " + _enemy.name);
	}
}
