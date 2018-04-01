using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PacmanPlayer : MonoBehaviour {

	void Start ()
	{
		var collisionTrigger=GetComponent<Collider>().OnTriggerEnterAsObservable();
			collisionTrigger.Where(collider1 => collider1.CompareTag("Point")).Subscribe(OnPointTouched);
		collisionTrigger.Where(collider1 => collider1.CompareTag("PowerUp")).Subscribe(OnPowerUpTouched);
	}

	private void OnPowerUpTouched(Collider collider)
	{
		Debug.Log($"Collision with {collider.name}");
		Debug.Log("Bang");
		Destroy(collider.gameObject);
	}

	private void OnPointTouched(Collider collider)
	{
		Debug.Log($"Collision with {collider.name}");
		Debug.Log("Bang");
		Destroy(collider.gameObject);
	}

	void LateUpdate () {
		transform.rotation=Quaternion.identity;
	}
	
	
	
}
