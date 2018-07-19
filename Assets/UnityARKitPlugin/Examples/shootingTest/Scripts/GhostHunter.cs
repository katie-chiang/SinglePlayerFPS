using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParticleBulletSystem;
using UnityEngine.UI;


public class GhostHunter : GamePlayer {

	//public GameObject panelInFront;
	public Image image;

	public int number = 1;
	private float speed = 5;

	private float size = 0.1f;

	// Use this for initialization
	void Start () {
		// if(panelInFront == null){
		// 	Debug.Log("no panel");
		// }
		// image = panelInFront.GetComponent<Image>();
		// if(image == null){
		// 	Debug.Log("no panel");
		// }
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			shoot();
		}
		if(Input.touchCount < 1){
			return;
		}

		Touch touch = Input.GetTouch(0);

		if(touch.phase == TouchPhase.Began){
            shoot();
		}
	}

	override protected void shoot() {

		//code for the first particle system
		// Ray raycast = Camera.main.ScreenPointToRay(touch.position);
		// Vector3 startPoint = raycast.GetPoint(startDist);
		// ParticleManager.manager.Emit(number, count, startPoint, Camera.main.transform.rotation);
		// Transform pt = ParticleManager.manager.particle[number].transform;
		// pt.position = Vector3.zero;
		// pt.eulerAngles = Vector3.zero;


		//code for second particle system
		// Ray ray = Camera.main.ScreenPointToRay(touch.position);
		// Vector3 startPoint = ray.GetPoint(startDist);

		//shoot from camera
		ParticleManager.manager.Emit_OneShot(number, Camera.main.transform, speed, size);

	}

	override protected void onHpChanged(int hp) {
		// TODO later 
		var tempColor = image.color;
        tempColor.a = (1 - ((float)hp/maxHp))/2;
        image.color = tempColor;
	}
}
