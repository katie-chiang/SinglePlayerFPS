using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParticleBulletSystem;

public class Ghost : GamePlayer {
	// Update is called once per frame
	public TextMesh textHp;

	//private float repeatTimer = 4;
    private float timerCount = 0;

	//the number represents the particle used (using 9)
	public int particleId = 9;

	private float speed = 3;

	private float size = 0.1f;

	//event for death
	public delegate void ghostIsDead(GameObject deadGhost);
	public static event ghostIsDead ghostIsDeadEvent;

	private Transform homingTarget;

	// Use this for initialization
	void Start () {
		homingTarget = Camera.main.transform;
		// paster.Add(homingTarget);

	}
	
	// Update is called once per frame
	void Update () {

		timerCount -= Time.deltaTime;
		
		if (timerCount <= 0) {
			shoot();
			timerCount = Random.Range(3, 10);
		}

		//check death
		if(isDead()){
			if(ghostIsDeadEvent != null){
				ghostIsDeadEvent(gameObject);
			}
		}
	}

	override protected void shoot() {

		Transform pt = ParticleManager.manager.particle[particleId].transform;
		pt.position = Vector3.zero;
		pt.eulerAngles = Vector3.zero;
		
		ParticleManager.manager.Emit_OneShot(particleId, transform, speed, size);
		ParticleManager.manager.changeVelocity(particleId, transform.position, homingTarget.position, speed);
	}

	override protected void onHpChanged(int hp) {
		textHp.text = hp + "/" + maxHp;
	}
}
