/// <summary>
/// Interface for implementing poolable objects
/// </summary>
public interface IPoolableObject
{
	int poolIndex { get; set; }
	bool activeInScene { get; }
	/// <summary>
	/// Time left to return to the pool.
	/// </summary>
	float leftDuration { get; }
	/// <summary>
	/// Set to true to make the object stay on scene
	/// until the intervention of another object.
	/// </summary>
	bool stopTimer { get; set; }

	/// <summary></summary>
	/// <param name="duration">When minor or equal to 0, stopTimer turns to true.</param>
	void Activate(float duration);
	void DeActivate();
}