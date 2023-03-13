using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class of Object Pooling. Each instance contains a different pool.
/// </summary>
public class ObjectPooler<T> where T : MonoBehaviour, IPoolableObject
{
	Dictionary<int, T> pool = new Dictionary<int, T>();

	/// <summary>
	/// Prefab to be cloned.
	/// </summary>
	public T prefab { get; private set; }
	/// <summary>
	/// Maximum number of elements in pool.
	/// </summary>
	public int maxSize { get; private set; }
	/// <summary>
	/// Current number of elements in pool.
	/// </summary>
	public int currentSize { get { return pool.Count; } }
	/// <summary>
	/// Can pool increase in size?
	/// </summary>
	public bool canResize { get; private set; }

	/// <summary>
	/// Constructor for later startup of the pool.
	/// </summary>
	/// <param name="prefab">Prefab to be cloned.</param>
	/// <param name="maxSize">Initial pool limit (min. value is 1).</param>
	/// <param name="canResize">Pool could grow in the future.</param>
	public ObjectPooler(T prefab, int maxSize, bool canResize = false)
	{
		this.prefab = prefab;
		this.maxSize = (maxSize < 1) ? 1 : maxSize;
		this.canResize = canResize;
	}

	/// <summary>
	/// Constructor to initialize pool immediately.
	/// </summary>
	/// <param name="prefab">Prefab to be cloned.</param>
	/// <param name="initSize">Initial pool size (min. value is 1).</param>
	/// <param name="maxSize">Initial pool limit (min. value is 1).</param>
	/// <param name="canResize">Pool could grow in the future.</param>
	public ObjectPooler(T prefab, int initSize, int maxSize, bool canResize = false)
	{
		this.prefab = prefab;
		this.maxSize = (maxSize < 1) ? 1 : maxSize;
		this.canResize = canResize;
		InitializePool(initSize);
	}

	/// <summary>
	/// Initialize objects, within a clear pool, out of screen.
	/// </summary>
	/// <param name="size">Number of objects to initialize. Can't be greater than "maxSize".</param>
	public void InitializePool(int size)
	{
		if (currentSize > 0)
		{ PrintConsole.Error("Can't initialize a pool already in use"); return; }

		size = (size < 1) ? 1 :
			(size > maxSize) ? maxSize : size;

		T n;
		for (int i = 0; i < size; ++i)
		{
			n = MonoBehaviour.Instantiate(prefab, Vector2.down * 5000, Quaternion.identity);
			n.DeActivate();
			n.poolIndex = pool.Count;
			pool.Add(n.poolIndex, n);
		}
	}

	/// <summary>
	/// Destroys all elements and clear the pool.
	/// </summary>
	public void DeletePool()
	{
		T t = null;
		for (int i = pool.Count; i >= 0; --i)
		{
			if (pool.TryGetValue(i, out t))
			{
				MonoBehaviour.Destroy(t.gameObject);
			}
		}
		pool.Clear();
	}

	/// <summary>
	/// Add new element, considering an expansion of the pool.
	/// </summary>
	/// <returns>Returns the new element.</returns>
	T Add()
	{
		if (currentSize == maxSize)
		{ if (!canResize) { return null; } }

		if (currentSize > maxSize)
		{ ++maxSize; }

		T n = MonoBehaviour.Instantiate(prefab);
		n.poolIndex = pool.Count;
		pool.Add(n.poolIndex, n);
		n.DeActivate();
		return n;
	}

	/// <summary>
	/// Get first available element in the pool.
	/// </summary>
	/// <returns></returns>
	public T GetObject()
	{
		if (currentSize == 0)
		{ PrintConsole.Error("Empty pool"); return null; }

		T t = null;
		for (int i = 0; i < pool.Count; ++i)
		{
			if (!pool[i].activeInScene)
			{ t = pool[i]; break; }
		}

		if (t == null)
		{ t = Add(); }
		return t;
	}

	/// <summary>
	/// Get a specific element from the pool, if it's available.
	/// </summary>
	/// <param name="index">Element index in the pool.</param>
	/// <returns></returns>
	public T GetObject(int index)
	{
		if (index < 0 || index > currentSize - 1)
		{ PrintConsole.Error("Index out of range"); return null; }
		if (currentSize == 0)
		{ PrintConsole.Error("Empty pool"); return null; }

		T t;
		if (pool.ContainsKey(index))
		{
			if (pool.TryGetValue(index, out t))
			{
				return t;
			}
		}
		PrintConsole.Error("Enable to find index/object");
		return null;
	}
}