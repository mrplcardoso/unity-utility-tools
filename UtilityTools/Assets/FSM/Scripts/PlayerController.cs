using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
	public NavMeshAgent agent;
	public Vector3 mousePosition
	{ 
		get
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit h;
			Physics.Raycast(ray, out h);
			return h.point;
		}
	}

	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
	}
}
