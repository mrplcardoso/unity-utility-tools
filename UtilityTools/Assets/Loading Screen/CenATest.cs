using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CenATest : MonoBehaviour
{
	public LoadingScreen s;

	private void Start()
	{

		StartCoroutine(v());
	}
	
	IEnumerator v()
	{
		yield return new WaitForSeconds(0.1f);
	//	s.OnLoadAction = AfterLoad;
		yield return new WaitForSeconds(2f);
		Destroy(this.gameObject);
	}

	void AfterLoad(Scene s, LoadSceneMode mode)
	{
		print("ainda estou aqui");
	}
}
