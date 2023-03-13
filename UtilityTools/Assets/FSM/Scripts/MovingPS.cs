using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPS : PlayerState
{
	public override void OnEnter()
	{
		player = playerMachine.GetComponent<PlayerController>();
		base.OnEnter();
	}
	public override IEnumerator OnUpdate()
	{
		while (true)
		{
			if (Input.GetAxisRaw("Fire1") != 0)
			{
				if (playerMachine.axisInUse == false)
				{
					playerMachine.axisInUse = true;
					player.agent.isStopped = true;
					player.agent.Warp(player.agent.pathEndPosition);
					playerMachine.ChangeState<SetDestinyPS>();
				}
			}
			else if (Input.GetAxisRaw("Fire1") == 0)
			{
				playerMachine.axisInUse = false;
			}

			if (Input.GetAxisRaw("Fire2") != 0)
			{
				if (playerMachine.axisInUse == false)
				{
					player.agent.isStopped = true;
					playerMachine.axisInUse = true;
					playerMachine.ChangeState<SetDestinyPS>();
				}
			}
			if (Input.GetAxisRaw("Fire2") == 0)
			{
				//playerMachine.axisInUse = false;
			}
			yield return null;
		}
	}

	public override void OnExit()
	{
		player.agent.ResetPath();
		base.OnExit();
	}
}
