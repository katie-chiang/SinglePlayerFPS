using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionDetection : MonoBehaviour {

	GameObject textDisplayed = null;
	healthText healthScript = null;

	// Use this for initialization
	void Start () {
		
		textDisplayed = GameObject.FindWithTag("text");
		healthScript = textDisplayed.GetComponent<healthText>();
	}
	
	// Update is called once per frame
	void Update () {

	}

	//enter when particle hits a gameobject, and the hit object is the parameter passed in
	void OnParticleCollision(GameObject other)
	{	
		// if(healthScript == null){
		// 	return;
		// }
		var player = other.GetComponent<GamePlayer>();
		if (player != null) {
			// TODO solve concurrency problem
			player.hp = player.hp - 1;
		}
		// if(other.tag == "MainCamera"){
		// 	// find hunter
		// 	Debug.Log("Player is hit!!");
		// 	healthScript.damageCount++;
		// }else if (other.tag == "enemy"){
		// 	Debug.Log("ghost is hit!!");
		// 	healthScript.hitCount++;
		// }
	}
}
