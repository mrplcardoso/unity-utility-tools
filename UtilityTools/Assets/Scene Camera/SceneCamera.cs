using System.Collections;
using UnityEngine;

namespace Utility.SceneCamera
{
	public class SceneCamera : MonoBehaviour
	{
		public static SceneCamera instance { get; private set; }
		public Camera camera { get; private set; }

		void Awake()
		{
			Camera[] g = GameObject.FindObjectsOfType<Camera>();
			if (g.Length > 1)
			{ Destroy(gameObject); return; }

			instance = this;
			camera = GetComponent<Camera>();
		}
	}
}