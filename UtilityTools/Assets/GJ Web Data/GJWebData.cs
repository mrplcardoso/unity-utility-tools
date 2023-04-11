using GameJolt.API;
using GameJolt.API.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Utility.EventCommunication;

namespace Utility.WebData
{
	/// <summary>
	/// Game Jolt Web Data Manager for users and progress
	/// Place this class on an GameObject (it will persist while game is running)
	/// </summary>
	public class GJWebData : MonoBehaviour
	{
		void Awake()
		{
			if (!SetInstance()) { return; }

			Subscribing();
			currentUser = null;
			currentProgress = null;
		}

		private void OnDisable()
		{
			UnSubscribing();
		}

		#region Singleton
		public static GJWebData instance { get; private set; }
		private bool SetInstance()
		{
			GJWebData[] l = FindObjectsOfType<GJWebData>();
			if (l.Length > 1) { Destroy(gameObject); return false; }

			instance = this;
			DontDestroyOnLoad(gameObject);
			return true;
		}
		#endregion

		#region Events
		void Subscribing()
		{
			EventHub.Subscribe(EventList.LoadUser, LoadUser);
			EventHub.Subscribe(EventList.LoadProgress, LoadProgress);
			EventHub.Subscribe(EventList.SaveProgress, SaveProgress);
			EventHub.Subscribe(EventList.GetProgressValue, GetValue);
			EventHub.Subscribe(EventList.GetProgressValues, GetValues);
			EventHub.Subscribe(EventList.SetProgressValue, SetValue);
			EventHub.Subscribe(EventList.SetProgressValues, SetValues);
		}

		void UnSubscribing()
		{
			EventHub.UnSubscribe(EventList.LoadUser, LoadUser);
			EventHub.UnSubscribe(EventList.LoadProgress, LoadProgress);
			EventHub.UnSubscribe(EventList.SaveProgress, SaveProgress);
			EventHub.Subscribe(EventList.GetProgressValue, GetValue);
			EventHub.Subscribe(EventList.GetProgressValues, GetValues);
			EventHub.Subscribe(EventList.SetProgressValue, SetValue);
			EventHub.Subscribe(EventList.SetProgressValues, SetValues);
		}
		#endregion

		#region Load User
		//WebInterface.jslib's function pointers
		[DllImport("__Internal")]
		static extern string GetUser();
		/// <summary>
		/// Edit it to test with your user in Unity
		/// </summary>
		const string debugUser = "?gjapi_username=guestuser&gjapi_token=guesttoken";

		public User currentUser { get; private set; }
		public GJUserType userType { get; private set; }
		public bool finishLoadUser { get; private set; }
		public bool loadUserSuccess { get { return currentUser != null || userType == GJUserType.Guest; } }

		bool gjSignedUser { get { return GameJoltAPI.Instance.HasSignedInUser; } }
		bool gjHasUser { get { return GameJoltAPI.Instance.HasUser; } }

		void LoadUser(EventData e)
		{
			LoadUser();
		}

		public void LoadUser()
		{
			//Already signed User, no need to sign in again
			if (gjSignedUser)
			{ SignedUser(); return; }

			string linkParams = UserParams();
			//Empty user data is Guest User and can't be signed
			if (linkParams.Length == 0)
			{ GuestUser(); return; }

			//Load username/token from URL params and Sign User In
			Dictionary<string, string> data = UserData(linkParams);
			if (data.Count == 2)
			{ SignUserIn(data["gjapi_username"], data["gjapi_token"]); }
			else
			{ PrintConsole.Error("Couldn't sign user in"); }
		}

		public void LoadDebugUser()
		{
			string linkParams = UserParams();
			Dictionary<string, string> data = UserData(linkParams);
			currentUser = new User(data["gjapi_username"], data["gjapi_token"]);
			DebugProgress();
			userType = GJUserType.Debug;
			finishLoadUser = finishLoadProgress = true;
		}
		#endregion

		#region User Types
		void SignedUser()
		{
			currentUser = GameJoltAPI.Instance.CurrentUser;
			finishLoadUser = true;
			EventHub.Publish(EventList.UserSuccess, new EventData(currentUser));
		}

		void GuestUser()
		{
			PrintConsole.Warning("User not logged!");
			userType = GJUserType.Guest;
			finishLoadUser = true;
			EventHub.Publish(EventList.UserSuccess);
		}

		//Runs for non-guest unsigned user
		void SignUserIn(string userName, string userToken)
		{
			//Get User object from GameJolt
			if (gjHasUser && !gjSignedUser)
			{ currentUser = GameJoltAPI.Instance.CurrentUser; }
			//Create new User
			else if (!GameJoltAPI.Instance.HasUser)
			{ currentUser = new User(userName, userToken); }

			currentUser.SignIn(null, userFetchSuccess =>
			{
				if (userFetchSuccess)
				{ EventHub.Publish(EventList.UserSuccess, new EventData(currentUser)); }
				else
				{ EventHub.Publish(EventList.UserFailed); }
				finishLoadUser = true;
			}
			);
		}

		/// <summary>
		/// Change UserType from 'New' to 'Logged' in the very first New Game.
		/// </summary>
		public void LogInUserType()
		{
			if (userType != GJUserType.New)
			{ PrintConsole.Error("Only 'New' users can be set 'Logged'"); return; }

			userType = GJUserType.Logged;
		}
		#endregion

		#region User Auxiliary Methods
		string UserParams()
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			Uri uri = new Uri(GetUser());
			return uri.Query;
#elif UNITY_EDITOR
			return debugUser;
#endif
		}

		string[] SectionParams(string userParams)
		{
			//Remove 'question mark'
			if (userParams[0] == '?')
			{ userParams = userParams.Substring(1); }
			//Separate data by '&'
			return userParams.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
		}

		Dictionary<string, string> UserData(string userParams)
		{
			string[] sections = SectionParams(userParams);
			string[] split;
			Dictionary<string, string> data = new Dictionary<string, string>();
			for (int i = 0; i < sections.Length; ++i)
			{
				//Split assingment
				split = sections[i].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
				//If it contains an '=' sign, assign its key-value in the dictionary
				if (split.Length == 2)
				{ data.Add(split[0], split[1]); }
			}

			return data;
		}
		#endregion

		#region Load Progress
		/// <summary>
		/// Progress data. Edit it to match the desired type.
		/// </summary>
		Dictionary<DataKey, int> currentProgress;
		public bool finishLoadProgress { get; private set; }
		public bool loadProgressSuccess { get { return currentProgress != null; } }

		void LoadProgress(EventData data)
		{
			LoadProgress();
		}

		public void LoadProgress()
		{
			if (userType == GJUserType.Guest)
			{ GuestProgress(); return; }

			DataStore.Get(currentUser.ID.ToString(), true, (string value) =>
			{
				if (value != null)
				{
					userType = GJUserType.Logged;
					currentProgress = ReadJson(value);
					finishLoadProgress = true;
					EventHub.Publish(EventList.ProgressSuccess);
				}
				else
				{ CreateNewData(); }
			});
		}

		/// <summary>
		/// Create new user's progress data in Game Jolt's server
		/// </summary>
		void CreateNewData()
		{
			PrintConsole.Log("Creating new Data");

			currentProgress = new Dictionary<DataKey, int>();
			for (int i = 0; i < (int)DataKey.Count; ++i)
			{ currentProgress.Add((DataKey)i, 0); }

			string json = WriteJson(currentProgress);
			DataStore.Set(currentUser.ID.ToString(), json, true, (bool value) =>
			{
				if (value)
				{ userType = GJUserType.New; EventHub.Publish(EventList.ProgressSuccess); }
				else
				{ EventHub.Publish(EventList.ProgressFailed); }
			});
		}
		#endregion

		#region Save Progress
		void SaveProgress(EventData data)
		{
			SaveProgress();
		}

		public void SaveProgress()
		{
			string json = WriteJson(currentProgress);
			DataStore.Set(currentUser.ID.ToString(), json, true, (bool value) =>
			{
				if (value)
				{ EventHub.Publish(EventList.SaveSuccess); }
				else
				{ EventHub.Publish(EventList.SaveFailed); }
			});
		}
		#endregion

		#region Defined Progress
		/// <summary>
		/// Edit it to test progress without GJ's Server
		/// </summary>
		void DebugProgress()
		{
			userType = GJUserType.Logged;
			string json = "{ 'Data 1': 'value 1' }";
			currentProgress = ReadJson(json);
			EventHub.Publish(EventList.ProgressSuccess);
		}

		void GuestProgress()
		{
			string json = "{ 'Data 1': 'value 1' }";
			currentProgress = ReadJson(json);
			EventHub.Publish(EventList.ProgressSuccess);
		}
		#endregion

		#region Json
		/// <summary>
		/// Convert Json to Progress. Edit it to match progress type.
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		Dictionary<DataKey, int> ReadJson(string json)
		{
			PrintConsole.Log("Loading Data");
			return JsonConvert.DeserializeObject<Dictionary<DataKey, int>>(json);
		}

		/// <summary>
		/// Convert Progress to Json. Edit it to match progress type.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		string WriteJson(Dictionary<DataKey, int> data)
		{
			Dictionary<DataKey, int> newData = new Dictionary<DataKey, int>();
			for (int i = 0; i < (int)DataKey.Count; ++i)
			{ newData.Add((DataKey)i, data[(DataKey)i]); }

			return JsonConvert.SerializeObject(newData, Formatting.Indented);
		}
		#endregion

		#region Values

		void GetValue(EventData data)
		{
			int value = GetValue((DataKey)data.eventInformation);
			EventHub.Publish(EventList.SendProgressValue, new EventData(value));
		}

		/// <summary>
		/// Get a progress value. Edit it to match progress type.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public int GetValue(DataKey key)
		{
			return currentProgress[key];
		}

		void GetValues(EventData data)
		{
			DataKey[] keys = (DataKey[])data.eventInformation;
			Dictionary<DataKey, int> values = GetValues(keys);
			EventHub.Publish(EventList.SendProgressValues, new EventData(values));
		}

		/// <summary>
		/// Get selected progress value. Edit it to match progress type.
		/// </summary>
		/// <param name="keys"></param>
		/// <returns></returns>
		public Dictionary<DataKey, int> GetValues(DataKey[] keys)
		{
			Dictionary<DataKey, int> values = new Dictionary<DataKey, int>();
			for (int i = 0; i < keys.Length; ++i)
			{ values.Add(keys[i], currentProgress[keys[i]]); }

			return values;
		}

		void SetValue(EventData data)
		{
			int[] d = (int[])data.eventInformation;
			SetValue((DataKey)d[0], d[1]);
		}

		/// <summary>
		/// Set a value. Edit it to match progress type.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void SetValue(DataKey key, int value)
		{
			currentProgress[key] = value;
		}

		void SetValues(EventData data)
		{
			int[][] d = (int[][])data.eventInformation;
			DataKey[] keys = new DataKey[d[0].Length];
			int[] values = new int[d[1].Length];
			for (int i = 0; i < d[0].Length; ++i)
			{
				keys[i] = (DataKey)d[0][i];
				values[i] = d[1][i];
			}
			SetValues(keys, values);
		}

		/// <summary>
		/// Set selected progress values. Edit it to match progress type.
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="values"></param>
		public void SetValues(DataKey[] keys, int[] values)
		{
			for (int i = 0; i < keys.Length; ++i)
			{
				currentProgress[keys[i]] = values[i];
			}
		}
		#endregion
	}
}
