using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : State
{
	protected PlayerController player;
	protected PlayerSM playerMachine
	{ get { return (PlayerSM)stateMachine; } }
}
