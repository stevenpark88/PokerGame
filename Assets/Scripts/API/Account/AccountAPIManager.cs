using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UIAccount
{
	public class AccountAPIManager : MonoBehaviour
	{
		#region PUBLIC_VARIABLES
		public bool debugEnabled = false;
		#endregion

		#region PRIVATE_VARIABLES
		private static string debugString = "<b>API Messages</b>";
		#endregion

		#region DELEGATES
		public delegate void OnAccountDetailReceived (string info);
		public static event OnAccountDetailReceived onAccountDetailReceived;

		public static void FireAccountDetailReceived (string info)
		{
			if (onAccountDetailReceived != null)
				onAccountDetailReceived (info);
		}

		public delegate void OnAccountDetailUpdated (string info);
		public static event OnAccountDetailUpdated onAccountDetailUpdated;

		public static void FireAccountDetailUpdated (string info)
		{
			if (onAccountDetailUpdated != null)
				onAccountDetailUpdated (info);
		}

		public delegate void OnSelfLimitUpdated (string info);
		public static event OnSelfLimitUpdated onSelfLimitUpdated;

		public static void FireSelfLimitUpdated (string info)
		{
			if (onSelfLimitUpdated != null)
				onSelfLimitUpdated (info);
		}

		public delegate void OnSelfExclusionUpdated (string info);
		public static event OnSelfExclusionUpdated onSelfExclusionUpdated;

		public static void FireSelfExclusionUpdated (string info)
		{
			if (onSelfExclusionUpdated != null)
				onSelfExclusionUpdated (info);
		}
		#endregion

		private static AccountAPIManager instance;

		public static AccountAPIManager GetInstance()
		{
			if (instance == null) {
				instance = new GameObject ("AccountAPIManager").AddComponent<AccountAPIManager> ();
			}

			return instance;
		}

		#region UNITY_CALLBACKS

		// Use this for initialization
		void Awake ()
		{

		}

		void OnGUI()
		{
			if (debugEnabled)
				GUI.Label (new Rect (10, 35, 500, 200), debugString);
		}

		#endregion

		#region GetAccountDetail

		public void GetAccountDetail (bool displayLoader = false)
		{
			if (HasInternetConnection ()) {
				if (displayLoader)
					UIManager.Instance.DisplayLoader ();
				StartCoroutine (RequestAccountDetail ());
			}
		}

		private IEnumerator RequestAccountDetail()
		{
			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers.Add (APIConstants.FIELD_AUTHORIZATION, "Bearer " + APIConstants.PLAYER_TOKEN);
			headers.Add ("Content-Type", "application/x-www-form-urlencoded");

			WWW www = new WWW (APIConstants.USER_ACCOUNT_URL, null, headers);
			yield return www;

			OnGetAccountDetailDone (www);
		}

		private void OnGetAccountDetailDone (WWW www)
		{
			debugString += "\n" + www.text;
			if (www.error != null)
			{
				DebugLog.Log(www.error);
				Debug.LogWarning(www.error);
				return;
			}

			DebugLog.LogWarning (www.text);

			FireAccountDetailReceived (www.text);
			AccountInfo accountInfo = JsonUtility.FromJson<AccountInfo> (www.text);
			DataManager.IsSoundEnabled = accountInfo.sound;
		}

		#endregion

		#region SetAccountDetail

		public void SetAccountDetail (int username, int contact, int sound, string timezone)
		{
			if (HasInternetConnection ()) {
				UIManager.Instance.DisplayLoader ();
				StartCoroutine (UpdateAccountDetail (username, contact, sound, timezone));
			}
		}

		private IEnumerator UpdateAccountDetail (int username, int contact, int sound, string timezone)
		{
			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers.Add (APIConstants.FIELD_AUTHORIZATION, "Bearer " + APIConstants.PLAYER_TOKEN);
			headers.Add ("Content-Type", "application/x-www-form-urlencoded");

			WWWForm form = new WWWForm ();
			form.AddField (APIConstants.FIELD_USERNAME, "" + username);
			form.AddField (APIConstants.FIELD_CONTACT, "" + contact);
			form.AddField (APIConstants.FIELD_SOUND, "" + sound);
			form.AddField (APIConstants.FIELD_TIMEZONE, timezone);

			WWW www = new WWW (APIConstants.USER_ACCOUNT_URL, form.data, headers);

			yield return www;

			OnUpdateAccountDetailDone (www);
		}

		private void OnUpdateAccountDetailDone (WWW www)
		{
			debugString += "\n" + www.text;
			if (www.error != null)
			{
				DebugLog.Log(www.error);
				Debug.LogWarning(www.error);
				return;
			}

			DebugLog.LogWarning (www.text);

			FireAccountDetailUpdated (www.text);
		}

		#endregion

		#region UpdateSelfLimit

		public void SetSelfLimit (double chipsLimit)
		{
			if (HasInternetConnection ()) {
				UIManager.Instance.DisplayLoader ();
				StartCoroutine (UpdateSelfLimit (chipsLimit));
			}
		}

		private IEnumerator UpdateSelfLimit (double chipsLimit)
		{
			Dictionary<string, string> headers = new Dictionary<string, string> ();
			headers.Add (APIConstants.FIELD_AUTHORIZATION, "Bearer " + APIConstants.PLAYER_TOKEN);
			headers.Add ("Content-Type", "application/x-www-form-urlencoded");

			WWWForm form = new WWWForm ();
			form.AddField (APIConstants.FIELD_DAILY_LIMIT, "" + chipsLimit);

			WWW www = new WWW (APIConstants.UPDATE_SELF_LIMIT_URL, form.data, headers);

			yield return www;

			OnUpdateSelfLimitDone (www);
		}

		private void OnUpdateSelfLimitDone (WWW www)
		{
			debugString += "\n" + www.text;
			if (www.error != null) {
				DebugLog.Log (www.error);
				Debug.LogWarning (www.error);
				return;
			}

			DebugLog.LogWarning (www.text);

			FireSelfLimitUpdated (www.text);
		}

		#endregion

		#region UpdateSelfExclusion

		public void SetSelfExclusion (string exclusionType)
		{
			if (HasInternetConnection ()) {
				UIManager.Instance.DisplayLoader ();
				StartCoroutine (UpdateSelfExclusion (exclusionType));
			}
		}

		private IEnumerator UpdateSelfExclusion (string exclusionType)
		{
			Dictionary<string, string> headers = new Dictionary<string, string> ();
			headers.Add (APIConstants.FIELD_AUTHORIZATION, "Bearer " + APIConstants.PLAYER_TOKEN);
			headers.Add ("Content-Type", "application/x-www-form-urlencoded");

			WWWForm form = new WWWForm ();
			form.AddField (APIConstants.FIELD_EXCLUSION_TYPE, exclusionType);

			WWW www = new WWW (APIConstants.UPDATE_SELF_EXCLUSION_URL, form.data, headers);

			yield return www;

			OnUpdateSelfExclusionDone (www);
		}

		private void OnUpdateSelfExclusionDone (WWW www)
		{
			debugString += "\n" + www.text;
			if (www.error != null) {
				DebugLog.Log (www.error);
				Debug.LogWarning (www.error);
				return;
			}

			DebugLog.LogWarning (www.text);

			FireSelfExclusionUpdated (www.text);
		}

		#endregion

		#region PRIVATE_METHODS

		private bool HasInternetConnection()
		{
			if (Application.internetReachability == NetworkReachability.NotReachable) {
				UIManager.Instance.noConnectionPanel.gameObject.SetActive (true);
				return false;
			}

			return true;
		}

		#endregion
	}
}