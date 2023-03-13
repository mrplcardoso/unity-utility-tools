using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
	#region Canvas

	/// <summary>
	/// Reference for canvas object that represents the loading screen.
	/// </summary>
	protected GameObject loadingCanvas;
	/// <summary>
	/// Canvas object that represents the loading screen. 
	/// Set through children objects.
	/// </summary>
	public GameObject LoadingCanvas
	{ get { return loadingCanvas; } }

	#endregion Canvas

	#region Settings

	public enum LoadProgress
	{
		OnlyBar,
		OnlyProgressText,
		ProgressBarText
	}
	[Tooltip("Select which children object will output loading progress. Remember to add them.")]
	[SerializeField]
	protected LoadProgress loadProgressType;

	/// <summary>
	/// Text object which shows progress message.
	/// </summary>
	protected Text progressText;
	/// <summary>
	/// Slider object that will show progress in form of bar.
	/// </summary>
	protected Slider progressBar;

	[Tooltip("If true, progress Text shows 'Loading' message.")]
	[SerializeField]
	protected bool showLoading;

	[Tooltip("If true, progress Text shows loading porcentage.")]
	[SerializeField]
	protected bool showPorcentage;

	[Tooltip("If true, logs the progress through console.")]
	[SerializeField]
	protected bool debugProgress;

	[Tooltip("If true, logs the progress through console.")]
	[SerializeField]
	protected bool waitForInput;

	[Tooltip("Minimun time the screen will be playing.")]
	[SerializeField]
	protected float minDuration;

	#endregion Settings

	#region OnLoadAction

	/// <summary>
	/// List of actions to perform after loading a scene
	/// </summary>
	protected List<UnityAction<Scene, LoadSceneMode>> loadActionList;
	/// <summary>
	/// Allows you to add methods to execute when 
	/// the new scene has finished loading.
	/// <br>Default action disables loading screen. 
	/// Set to null to remove all actions.</br>
	/// </summary>
	public UnityAction<Scene, LoadSceneMode> addLoadAction
	{
		set
		{
			if (value != null)
			{
				if (!loadActionList.Contains(value))
				{ 
					loadActionList.Add(value);
					SceneManager.sceneLoaded += value;
				}
			}
			else 
			{
				for(int i = 0; i < loadActionList.Count; ++i)
				{ SceneManager.sceneLoaded -= loadActionList[i];	}
				loadActionList.Clear();
			}
		}
	}

	#endregion OnLoadAction

	#region Loading Operation

	/// <summary>
	/// Operation of loading a scene asynchronously.
	/// </summary>
	protected AsyncOperation loadingOperation;
	/// <summary>
	/// Sinalizes if a scene is loading.
	/// </summary>
	public bool isLoading
	{ get; protected set; }
	/// <summary>
	/// Time the loading screen will be in scene.
	/// </summary>
	private float loadingDuration;

	#endregion Loading Operation

	#region Start Methods

	protected virtual void Start()
	{
		DontDestroyOnLoad(this.gameObject);
		waitForInput = false;

		//Canvas
		SetCanvas();

		//After loading action
		loadActionList = new List<UnityAction<Scene, LoadSceneMode>>();
		addLoadAction = DefaultLoadAction;

		//Progress Type
		SetProgressStyle();
	}

	
	void SetCanvas()
	{
		loadingCanvas = GetComponentInChildren<Canvas>(true).gameObject;
		if (loadingCanvas == null) { Debug.LogError("No loading canvas found!"); }
		else { loadingCanvas.SetActive(false); }
	}

	//Wrap Progress Type switch-case
	void SetProgressStyle()
	{
		switch (loadProgressType)
		{
			case LoadProgress.OnlyBar:
			{
				progressBar = GetComponentInChildren<Slider>(true);
				if (progressBar == null) { Debug.LogError("No slider bar found!"); }
				else
				{
					progressBar.value = 0;
					progressBar.gameObject.SetActive(false);
				}
				break;
			}
			case LoadProgress.OnlyProgressText:
			{
				progressText = GetComponentInChildren<Text>(true);
				if (progressText == null) { Debug.LogError("No progress text found!"); }
				else
				{
					SetLoadingText(0);

					progressText.gameObject.SetActive(false);
				}
				break;
			}
			case LoadProgress.ProgressBarText:
			{
				progressBar = GetComponentInChildren<Slider>(true);
				if (progressBar == null) { Debug.LogError("No slider bar found!"); }
				else
				{
					progressBar.value = 0;
					progressBar.gameObject.SetActive(false);
				}

				progressText = GetComponentInChildren<Text>(true);
				if (progressText == null) { Debug.LogError("No progress text found!"); }
				else
				{
					SetLoadingText(0);

					progressText.gameObject.SetActive(false);
				}
				break;
			}
		}
	}

	#endregion Start Methods

	void SetLoadingText(float value)
	{
		string txt = "";
		if (showLoading && showPorcentage) { txt = "Loading: " + (value * 100) + "%"; }
		else if (showLoading) { txt = "Loading..."; }
		else if (showPorcentage) { txt = (value * 100) + "%"; }
		progressText.text = txt;
	}

	/// <summary>
	/// Turn the visibility of canvas on/off. Clear progress.
	/// </summary>
	/// <param name="activate"></param>
	/// <param name="VisualProgressUpdate">Method that update the visual progress feedback.</param>
	public virtual void SetScreenActive(bool activate, Action<float> VisualProgressUpdate = null)
	{
		if (VisualProgressUpdate != null)
		{ VisualProgressUpdate(0f); }

		switch (loadProgressType)
		{
			case LoadProgress.OnlyBar:
			{
				progressBar.gameObject.SetActive(activate);
				break;
			}
			case LoadProgress.OnlyProgressText:
			{
				progressText.gameObject.SetActive(activate);
				break;
			}
			case LoadProgress.ProgressBarText:
			{
				progressBar.gameObject.SetActive(activate);
				progressText.gameObject.SetActive(activate);
				break;
			}
		}
		loadingCanvas.SetActive(activate);
	}

	/// <summary>
	/// Update the visual progress feedback.
	/// </summary>
	/// <param name="value">Value of progress to be set, in 0.0 - 1.0 range.</param>
	protected virtual void UpdateProgress(float value)
	{
		switch (loadProgressType)
		{
			case LoadProgress.OnlyBar:
			{
				progressBar.value = value;
				break;
			}
			case LoadProgress.OnlyProgressText:
			{
				SetLoadingText(value);
				break;
			}
			case LoadProgress.ProgressBarText:
			{
				progressBar.value = value;
				SetLoadingText(value);
				break;
			}
		}
		if (debugProgress)
		{ print("Progress: " + value); }
	}

	/// <summary>
	/// Default action to perform after loading is complete
	/// </summary>
	/// <param name="s"></param>
	/// <param name="mode"></param>
	protected virtual void DefaultLoadAction(Scene s, LoadSceneMode mode)
	{
		UpdateProgress(1f);
		if (!waitForInput)
		{ SetScreenActive(false); }
	}

	/// <summary>
	/// Starts scene changing.
	/// </summary>
	/// <param name="sceneName">Target scene name.</param>
	/// <param name="tryFreeMemory">If true, UnloadUnusedAssets() and GC.Collect() are called.</param>
	public virtual void ChangeScene(string sceneName, bool tryFreeMemory = false)
	{
		StartCoroutine(CallScene(sceneName, tryFreeMemory));
	}

	public virtual void WaitChangeScene(string sceneName, string axis, bool tryFreeMemory = false)
	{
		SceneManager.sceneLoaded -= DefaultLoadAction;
		SceneManager.sceneLoaded += DefaultLoadAction;
		StartCoroutine(CallScene(sceneName, tryFreeMemory));
		StartCoroutine(WaitInput(axis));
	}

	IEnumerator WaitInput(string axis)
	{
		waitForInput = true;
		print(isLoading);
		print(Input.GetAxisRaw(axis));
		if (!progressText.gameObject.activeSelf) { progressText.gameObject.SetActive(true); }

		yield return new WaitWhile(() => isLoading);
		progressText.text = "Press " + axis + " axis to play";
		yield return new WaitWhile(() => Input.GetAxisRaw(axis) == 0);
		SetScreenActive(false);
	}

	/// <summary>
	/// Coroutine that starts scene changing.
	/// </summary>
	/// <param name="sceneName">Target scene name.</param>
	/// <param name="tryFreeMemory">If true, UnloadUnusedAssets() and GC.Collect() are called.</param>
	/// <returns></returns>
	protected virtual IEnumerator CallScene(string sceneName, bool tryFreeMemory = false)
	{
		//Activate loading screen and wait 
		//a short time before initialize loading operation
		SetScreenActive(true, UpdateProgress);
		isLoading = true;

		if (tryFreeMemory) { Resources.UnloadUnusedAssets(); }
		yield return new WaitForSeconds(0.5f);

		loadingDuration = Time.time + minDuration;
		loadingOperation = SceneManager.LoadSceneAsync(sceneName);
		loadingOperation.allowSceneActivation = false;

		while (isLoading || loadingDuration > Time.time)
		{
			UpdateProgress(loadingOperation.progress);

			if(tryFreeMemory)
			{ GC.Collect(); tryFreeMemory = false; }

			if (loadingOperation.progress >= 0.9f)
			{
				isLoading = false;
				loadingOperation.allowSceneActivation = true;
			}
			yield return null;
		}
	}
}
