using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BarCreator : MonoBehaviour
{
	[Tooltip("Holds all bar holders created")]
	public List<GameObject> holderList = new List<GameObject>();
	[Tooltip("List of user's configured bar prefabs")]
	public List<GameObject> holderPrefabs = new List<GameObject>();

	//Cria o BarHolder, que por sua vez cria as estruturas internas
	public void CreateBar(int prefabId, int num)
	{
		if (prefabId < 0 || num < 1) return;

		GameObject holder = Instantiate(holderPrefabs[prefabId], 
			this.transform.localPosition, Quaternion.identity, this.transform);
		holder.name = "Bar Holder [" + prefabId + "]" + "[" + holderList.Count + "]";
		
		if (!holder.GetComponent<BarHolder>().CreateBarStructure(num))
		{ DestroyImmediate(holder); }
		else
		{ holderList.Add(holder); }
	}

	public void Clear()
	{
		foreach(GameObject g in holderList)
		{ DestroyImmediate(g); }
		holderList.Clear();
	}
}
