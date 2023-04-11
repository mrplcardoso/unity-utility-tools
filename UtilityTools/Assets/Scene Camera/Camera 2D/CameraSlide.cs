using System;
using System.Collections;
using UnityEngine;
using Utility.EasingEquations;

namespace Utility.SceneCamera.Camera2D
{
	public class CameraSlide : MonoBehaviour
	{
		public SceneCamera sceneCamera { get; private set; }
		public Camera camera { get { return sceneCamera.camera; } }

		private void Awake()
		{
			sceneCamera = GetComponent<SceneCamera>();
		}

		#region Move

		public Vector3 position { get { return transform.position; }
			set { transform.position = new Vector3(value.x, value.y, transform.position.z); } }

		[SerializeField] float speed;
		
		public void Move(Vector3 destination, float deltaTime)
		{
			position = EasingVector3Equations.Linear(position, destination, speed * deltaTime);
		}

		public IEnumerator SlidePosition(Func<Vector3, Vector3, float, Vector3> InterpMode,
		Vector3 start, Vector3 end, float speed = 1)
		{
			float t = 0;
			while (t < 1.01f)
			{
				position = InterpMode(start, end, t);
				t += speed * Time.deltaTime;
				yield return null;
			}
			position = end;
		}

		public void SetPosition(Func<Vector3, Vector3, float, Vector3> InterpMode,
		Vector3 start, Vector3 end, float value)
		{
			position = InterpMode(start, end, value);
		}
	}

	#endregion
}
