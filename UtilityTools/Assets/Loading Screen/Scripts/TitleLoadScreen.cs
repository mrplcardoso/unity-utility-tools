using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleLoadScreen : MonoBehaviour
{
	public LoadingScreen loadingScreen;
	public string sceneName;

	protected void Start()
	{
		StartCoroutine(Change());
	}

	IEnumerator Change()
	{
		yield return new WaitForSeconds(0.1f);
		loadingScreen.addLoadAction = WaitInput;
		yield return new WaitForSeconds(3f);
		loadingScreen.WaitChangeScene(sceneName, "Jump");
	}

	public void WaitInput(Scene s, LoadSceneMode mode)
	{
		print("trocou de cena");
	}
}
