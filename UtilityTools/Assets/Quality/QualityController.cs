using UnityEngine;
using Utility.EventCommunication;
using Utility.WebData;

namespace Utility.GraphicQuality
{
	public class QualityController : MonoBehaviour
	{
		public static QualityController instance { get; private set; }
		[SerializeField] string qualitySet = "High";
		public int qualityLevel
		{
			get
			{
				switch (qualitySet)
				{
					case "Low":
						return 1;
					case "High":
					default:
						return 4;
				}
			}
		}

		private void Awake()
		{
			QualityController[] g = GameObject.FindObjectsOfType<QualityController>();
			if (g.Length > 1) { Destroy(gameObject); return; }
			instance = this; DontDestroyOnLoad(gameObject);

			EventHub.Subscribe(EventList.SetQuality, SetQuality);
		}

		void Start()
		{
			GJWebData.instance.SetValue(DataKey.QualityLevel, qualityLevel);
		}

		void SetQuality(EventData data)
		{
			qualitySet = (string)data.eventInformation;
			QualitySettings.SetQualityLevel(qualityLevel);
		}

		public void OnDestroy()
		{
			EventHub.UnSubscribe(EventList.SetQuality, SetQuality);
		}
	}
}