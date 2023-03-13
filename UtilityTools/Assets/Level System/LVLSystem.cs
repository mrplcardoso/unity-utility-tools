using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class LVLSystem : MonoBehaviour
{
	public int currentLvl;
	int maxLvl;
	public int currentXP;
	int maxXP;
	public int baseXP;

	private void Start()
	{
		currentXP = 0;
		currentLvl = 0;
		maxLvl = 99;
		maxXP = 99999;
		print(((float)maxLvl / (float)maxXP));
		baseXP = (int)(((float)maxLvl / (float)maxXP) * 10000);
		StartCoroutine(nextLevel());
	}

	//f(x) = ((x^e) / d) + b
	float NextXPCurve(int lvl, int init)
	{
		return (Mathf.Pow(lvl, 2.5f) / 4) + init;
	}

	IEnumerator nextLevel()
	{
		yield return new WaitForSeconds(3f);
		while (currentLvl < maxLvl)
		{
			print(currentLvl);
			float c = NextXPCurve(currentLvl, baseXP);
			print(c);
			currentXP += Mathf.FloorToInt(c);
			print(currentXP);
			currentLvl++;
			yield return new WaitForSeconds(0.1f);
		}
	}

	//EarnedXP g(x) = -(log(e, x)/d) + b
}
