using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public int numOfGhosts = 2;

	public List<GamePlayer> mHunters = new List<GamePlayer>();
	public List<GamePlayer> mGhost = new List<GamePlayer>();

	public GameObject enemyPrefab;

	// Use this for initialization
	void Start () {
		var hunter = GameObject.FindGameObjectWithTag("MainCamera");
		mHunters.Add(hunter.GetComponent<GhostHunter>());

		for (var i = 0 ; i < numOfGhosts ; i++) {
			var enemyObject = Instantiate(enemyPrefab, Camera.main.transform.position + Vector3.forward * 6f, Quaternion.identity);
			var ghost = enemyObject.GetComponentInChildren<Ghost>();
			// ghost.particleId = Random.Range(0, 10);
			ghost.particleId = 9;
			mGhost.Add(ghost);
		}

		Ghost.ghostIsDeadEvent += destroyGhost;
	}

	void destroyGhost(GameObject ghostToDestroy){
		
		GameObject parentObj = ghostToDestroy.transform.parent.gameObject;
		
		for(int i = 0; i < parentObj.transform.childCount; i++){
			Destroy(parentObj.transform.GetChild(i).gameObject);
		}

		Destroy(parentObj);
	}

	
	// Update is called once per frame
	void Update () {

		
	}

	public List<GamePlayer> getCurrentHunter() {
		return mHunters;
	}

	public List<GamePlayer> getCurrentGhosts() {
		return mGhost;
	}

	//manage the ghosts in the scene and destroy if the ghost is dead
	void checkGhostDeath(){

	}
}
