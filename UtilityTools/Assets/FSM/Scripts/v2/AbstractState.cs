using UnityEngine;

/// <summary>
/// State of FSM Class.
/// </summary>
public abstract class AbstractState : MonoBehaviour
{
	/// <summary>
	/// StateMachine owner.
	/// </summary>
	protected AbstractStateMachine stateMachine;

	/// <summary>
	/// Get FSM that own this state.
	/// Set FSM when there is no FSM owner.
	/// </summary>
	public AbstractStateMachine machine
	{
		get { return stateMachine; }
		set { if (stateMachine == null) stateMachine = value; }
	}

	/// <summary>
	/// Execute a routine when machine enter in this state.
	/// </summary>
	public virtual void OnEnter() { }

	/// <summary>
	/// Execute a routine when machine leave this state.
	/// </summary>
	public virtual void OnExit() { }
}