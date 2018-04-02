using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PacmanPlayer : MonoBehaviour {

	private MazeGenerator _mazeGenerator;

	void Start ()
	{
		_mazeGenerator = GameObject.FindObjectOfType<MazeGenerator>();
		
		var collisionTrigger=GetComponent<Collider>().OnTriggerEnterAsObservable();
		collisionTrigger.Where(collider1 => collider1.CompareTag("Point")).Subscribe(OnPointTouched);
		collisionTrigger.Where(collider1 => collider1.CompareTag("PowerUp")).Subscribe(OnPowerUpTouched);
		collisionTrigger.Where(collider => collider.CompareTag("Ghost")).Subscribe(OnGhostHit);
	}

	private void OnPowerUpTouched(Collider collider)
	{
		_mazeGenerator.OnScoreAPoint(2, collider.gameObject);
	}

	private void OnPointTouched(Collider collider)
	{
		_mazeGenerator.OnScoreAPoint(1, collider.gameObject);
	}

	private void OnGhostHit(Collider collider) {
		_mazeGenerator.OnGhostHit(collider.gameObject);
	}

	void LateUpdate () {
		transform.rotation=Quaternion.identity;
	}
	
	
	
}
