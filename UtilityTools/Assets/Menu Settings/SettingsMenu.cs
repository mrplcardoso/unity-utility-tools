using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
	public Dropdown dropDown;
	public bool fullScreen;
	List<string> r = new List<string>();
	Resolution[] resolutions;
	int selected;
	private void Start()
	{
		dropDown.ClearOptions();
		resolutions = Screen.resolutions;

		foreach (Resolution rs in resolutions) 
		{
			r.Add(rs.ToString()); 
		}
		dropDown.AddOptions(r);

		fullScreen = true;
		Screen.fullScreen = fullScreen;
	}

	private void Update()
	{
		/*if(//Screen.height != resolutions[selected].height) //||
			Screen.width != resolutions[selected].width)
		{
			Screen.SetResolution(Screen.width,
				Screen.height, false);
		}*/
	}

	public void ChangeOption()
	{
		selected = dropDown.value;
		Screen.SetResolution(resolutions[selected].width, resolutions[selected].height, fullScreen);
	}

	public void SetFullscreen()
	{
		fullScreen = (fullScreen) ? false : true;
		Screen.fullScreen = fullScreen;
	}
}
