using System.Collections;
using UnityEngine;

/// <summary>
/// State of FSM Class.
/// </summary>
public abstract class State : MonoBehaviour
{
	protected StateMachine machine;
	/// <summary>
	/// Get FSM that own this state.
	/// Set FSM when there is no FSM owner.
	/// </summary>
	public StateMachine stateMachine
	{ 
		get { return machine; } 
		set { if (machine == null) machine = value; } 
	}

	/// <summary>
	/// Cache variable for OnUpdate() IEnumerator method.
	/// </summary>
	protected Coroutine update;

	/// <summary>
	/// Execute a routine when machine enter in this state.
	/// </summary>
	public virtual void OnEnter()
	{ update = StartCoroutine(OnUpdate()); }

	/// <summary>
	/// Expose a possible Update() method for states.
	/// </summary>
	public virtual IEnumerator OnUpdate()
	{ yield return null; }

	/// <summary>
	/// Execute a routine when machine leave this state.
	/// </summary>
	public virtual void OnExit()
	{ StopCoroutine(update); }

	public virtual void OnDestroy()
	{ StopCoroutine(update); }
}
