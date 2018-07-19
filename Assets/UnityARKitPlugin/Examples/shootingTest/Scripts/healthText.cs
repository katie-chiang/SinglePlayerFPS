using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthText : MonoBehaviour {

	// public int damageCount = 0;
	 // public int hitCount = 0;
	public Text textDisplayed;
	private GameManager gameManager;


	// Use this for initialization
	void Start () {
		gameManager = GameObject.FindObjectOfType<GameManager>();
	}

	// Update is called once per frame
	void Update () {
		
		var damageCount = 0;
		var hitCount = 0;
		foreach(GamePlayer gp in gameManager.getCurrentHunter()){
			damageCount = damageCount + gp.hp;
		}

		// foreach(GamePlayer gp in gameManager.getCurrentGhosts()){
		// 	hitCount = hitCount + gp.hp;
		// }
		textDisplayed.text = "Health: " + damageCount.ToString();
	}

}
