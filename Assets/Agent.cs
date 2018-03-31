using System;
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
				agent.SetDestination(followObject.position);
			});
		AsyncCall();
	}
	
	public async void AsyncCall()
	{
		var response = await WebRequest.CreateHttp("http://www.google.com").GetResponseAsync();
		var streamreader = new StreamReader(response.GetResponseStream());
		Debug.Log(await streamreader.ReadToEndAsync());
	}
}
