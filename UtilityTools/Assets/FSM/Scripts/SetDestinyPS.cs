using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDestinyPS : PlayerState
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
					player.agent.SetDestination(player.mousePosition);
					playerMachine.ChangeState<MovingPS>();
				}
			}
			if (Input.GetAxisRaw("Fire1") == 0)
			{
				playerMachine.axisInUse = false;
			}
			yield return null;
		}
	}
}
