using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GamePlayer : MonoBehaviour {

	private int _hp;
	public int hp {
		set {
			if(value < 0){
				return;
			}
			//Debug.Log("onHpChanged = " + _hp + " -> " + value);
			if (_hp != value) {
			 	onHpChanged(value);
			}
			_hp = value;
		}
		get { return _hp; }
	}
	public int maxHp = 100;

	public bool isDead() {
		return hp <= 0;
	}

	void Awake() {
		hp = maxHp;
	}

	abstract protected void shoot();
	abstract protected void onHpChanged(int hp);
}
