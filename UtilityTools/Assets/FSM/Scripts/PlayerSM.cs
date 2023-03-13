using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSM : StateMachine
{
	[SerializeField]
	public bool axisInUse = false;
	protected void Start()
	{
		ChangeState<SetDestinyPS>();
	}
}
