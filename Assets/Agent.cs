using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
	private NavMeshAgent agent;

	public Transform followObject;
	
	void Start ()
	{
		agent = GetComponent<NavMeshAgent>();
		this.FixedUpdateAsObservable().SubscribeOnMainThread().Subscribe(unit =>
			{
				Debug.Log("Update Destination");
				agent.SetDestination(followObject.position);
			});
	}
}
