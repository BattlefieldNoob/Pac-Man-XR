﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UniRx;
using UniRx.Triggers;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
	private NavMeshAgent agent;

	public Transform followObject;
	
	void Start ()
	{
		agent = GetComponent<NavMeshAgent>();
		this.FixedUpdateAsObservable().Subscribe(unit =>
			{
				//Debug.Log("Update Destination");
				NavMeshHit myNavHit;
				if(NavMesh.SamplePosition(followObject.position, out myNavHit, 100 , -1))
				{
					agent.SetDestination(myNavHit.position);
				}
				
			});
		//AsyncCall();
	}

	public void TeleportTo(Vector3 position) {
		transform.position = position;
	}
	
	public async void AsyncCall()
	{
		var response = await WebRequest.CreateHttp("http://www.google.com").GetResponseAsync();
		var streamreader = new StreamReader(response.GetResponseStream());
		Debug.Log(await streamreader.ReadToEndAsync());
	}
}
