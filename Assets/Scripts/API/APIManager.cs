using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIAccount;


public class APIManager : MonoBehaviour
{
	#region PUBLIC_VARIABLES

	#endregion

	#region PRIVATE_VARIABLES

	public bool debugEnabled = false;
	public static string debugString = "<b>API Messages</b>";

	#endregion

	public delegate void TipReponseReceived (string tip);

	public static event TipReponseReceived tipResposneReceived;

	public static void FireTipsResponseReceived (string tip)
	{
		if (tipResposneReceived != null)
			tipResposneReceived (tip);
	}

	public delegate void GameRulesResponseReceived (string rules);

	public static event GameRulesResponseReceived gameRulesResponseReceived;

	public static void FireGameRulesResponseReceived (string rules)
	{
		if (gameRulesResponseReceived != null)
			gameRulesResponseReceived (rules);
	}

	static APIManager Instance;

	public static APIManager GetInstance ()
	{
		if (Instance == null) {
			Instance = new GameObject ("APIManager").AddComponent<APIManager> ();
		}

		return Instance;
	}

	#region UNITY_CALLBACKS

	// Use this for initialization
	void Awake ()
	{
		Instance = this;
	}

	void OnGUI ()
	{
		if (debugEnabled)
			GUI.Label (new Rect (10, 35, 500, 200), debugString);
	}

	#endregion

	#region PUBLIC_METHODS

	/// <summary>
	/// Sends the login info.
	/// </summary>
	/// <param name="id">Identifier.</param>
	/// <param name="password">Password.</param>
	public void SendLoginInfo (string id, string password)
	{
		if (HasInternetConnection ()) {
			UIManager.Instance.loader.gameObject.SetActive (true);
			StartCoroutine (SendLoginInfoToServer (id, password));
		}
	}

	/// <summary>
	/// Gets the player info.
	/// </summary>
	/// <param name="token">Token.</param>
	public void GetPlayerInfo ()
	{
		if (HasInternetConnection ())
			StartCoroutine (SendPlayerInfoRequestToServer ());
	}

	/// <summary>
	/// Gets the profile image.
	/// </summary>
	/// <param name="url">URL.</param>
	public void GetProfileImage (string url)
	{
		if (HasInternetConnection ())
			StartCoroutine (GetPlayerImageFromServer (url));
	}

	/// <summary>
	/// Gets the tips.
	/// </summary>
	public void GetTips ()
	{
		if (HasInternetConnection ())
			StartCoroutine (GetTipsFromServer ());
	}

	public void GetGameRules ()
	{
		if (HasInternetConnection ())
			StartCoroutine (GetGameRulesFromServer ());
	}

	public void StartUpdatingLoginStatus ()
	{
		InvokeRepeating ("UpdateStatus", 0f, Constants.UPDATE_LOGIN_STATUS_INTERVAL);
	}

	private void UpdateStatus ()
	{
		if (HasInternetConnection ()) {
			StartCoroutine (UpdateLoginStatus ());
		}
	}

	public void StopUpdatingLoginStauts ()
	{
		CancelInvoke ("UpdateStatus");
	}

	public void Logout (System.Action<bool> action)
	{
		StartCoroutine (RequestLogout (action));
	}

	public void GetAvailableGame (string affiliateId, System.Action <WWW> action)
	{
		StartCoroutine (GetAvailableGameRequest (affiliateId, action));
	}
	public void lastHourAchivement ()
	{
		StopCoroutine ("GetlastHourAchivement");
		StartCoroutine ("GetlastHourAchivement");
	}
//	GetlastHourAchivement

	private IEnumerator GetlastHourAchivement ()//Dictionary<string, string> dict)
	{
		string url =  UIAccount.Constants.URL_PLAYER_ChipsUpdate;
		url = url.Replace (UIAccount.Constants.FIELD_PLAYER_TOKEN_URL, APIConstants.PLAYER_TOKEN);

		Debug.Log ("Url Call for hourly => " + url);
		WWW www = new WWW (url);

		yield return www;

		OnGetlastHourAchivementReceived (www);
	}
	void OnGetlastHourAchivementReceived(WWW www)
	{
		
	debugString += "\n" + www.text;
	if (www.error != null) {
		DebugLog.Log (www.error);
		return;
	}

		GetlastHourAchivement GetlastHourAchivementresp = JsonUtility.FromJson<GetlastHourAchivement> (www.text);

		UIManager.Instance.PanelHourlymessage.OpenMessagePanel (GetlastHourAchivementresp);
	DebugLog.LogWarning (www.text);
}
	#endregion

	#region PRIVATE_METHODS

	private bool HasInternetConnection ()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			UIManager.Instance.noConnectionPanel.gameObject.SetActive (true);
			return false;
		}

		return true;
	}

	#endregion

	public string affiliateId = "0";

	#region COROUTINES

	private IEnumerator SendLoginInfoToServer (string id, string password)
	{
		DebugLog.Log ("Login : " + id + " >> " + password);
		WWWForm form = new WWWForm ();
		form.AddField (APIConstants.FIELD_EMAIL, id);
		form.AddField (APIConstants.FIELD_PASSWORD, password);

		if (Application.platform != RuntimePlatform.WebGLPlayer)
			form.AddField (APIConstants.FIELD_AFFILIATE_DATA, "");
		else
			form.AddField (APIConstants.FIELD_AFFILIATE_DATA, affiliateId);

		Debug.LogWarning ("===> Affiliate ID  : " + affiliateId);

		WWW www = new WWW (APIConstants.LOGIN_URL, form);

		yield return www;

		OnLoginResponseReceived (www);
	}

	private IEnumerator SendPlayerInfoRequestToServer ()
	{
		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add (APIConstants.FIELD_AUTHORIZATION, "Bearer " + APIConstants.PLAYER_TOKEN);
		headers.Add ("Content-Type", "application/x-www-form-urlencoded");

		WWW www = new WWW (APIConstants.LOGIN_PLAYER_DETAIL, null, headers);
		yield return www;

		OnPlayerInfoResponseReceived (www);
	}

	private IEnumerator GetPlayerImageFromServer (string url)
	{
		WWW www = new WWW (url);

		yield return www;
		OnPlayerImageReceived (www);
	}

	private IEnumerator GetTipsFromServer ()
	{
		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add (APIConstants.FIELD_AUTHORIZATION, "Bearer " + APIConstants.PLAYER_TOKEN);
		headers.Add ("Content-Type", "application/x-www-form-urlencoded");

		WWW www = new WWW (APIConstants.URL_TIPS, null, headers);
		yield return www;

		OnTipsResponseReceived (www);
	}

	private IEnumerator GetGameRulesFromServer ()
	{
		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add (APIConstants.FIELD_AUTHORIZATION, "Bearer " + APIConstants.PLAYER_TOKEN);
		headers.Add ("Content-Type", "application/x-www-form-urlencoded");

		WWW www = new WWW (APIConstants.URL_GAME_RULES, null, headers);
		yield return www;

		OnGameRulesResponseReceived (www);
	}


	private IEnumerator UpdateLoginStatus ()
	{
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add (APIConstants.FIELD_AUTHORIZATION, "Bearer " + APIConstants.PLAYER_TOKEN);
		headers.Add ("Content-Type", "application/x-www-form-urlencoded");

		WWW www = new WWW (APIConstants.URL_UPDATE_LOGIN_STATUS, null, headers);
		yield return www;

		OnUpdateLoginStatusDone (www);
	}

	private IEnumerator RequestLogout (System.Action<bool> action)
	{
		Dictionary <string, string> headers = new Dictionary<string, string> ();
		headers.Add (APIConstants.FIELD_AUTHORIZATION, "Bearer " + APIConstants.PLAYER_TOKEN);
		headers.Add ("Content-Type", "application/x-www-form-urlencoded");

		WWW www = new WWW (APIConstants.URL_UPDATE_LOGOUT, null, headers);
		yield return www;

		OnLogoutDone (www, action);
	}

	private IEnumerator GetAvailableGameRequest (string affiliateId, System.Action <WWW> action)
	{
		WWWForm form = new WWWForm ();
		form.AddField (APIConstants.FIELD_AFFILIATE_ID, affiliateId);

		WWW www = new WWW (APIConstants.URL_AVAILABLE_GAMES, form);

		yield return www;

		action (www);
	}

	#endregion

	#region API_CALLBACKS

	/// <summary>
	/// Raises the login response received event.
	/// </summary>
	/// <param name="www">Www.</param>
	private void OnLoginResponseReceived (WWW www)
	{
		UIManager.Instance.loader.gameObject.SetActive (false);
		debugString += "\n" + www.text;
		if (www.error != null) {
			DebugLog.LogError (www.error);
			UIManager.Instance.loginPanel.txtError.text = "<color=yellow>Something went wrong.</color>";
			DebugLog.Log (www.text);
			JSON_Object errorObj = new JSON_Object (www.text);
			if (errorObj.has ("messages")) {
				JSONArray arr = errorObj.getJSONArray ("messages");
				if (arr.Count () > 0)
					UIManager.Instance.loginPanel.txtError.text = "<color=yellow>" + arr.getString (0) + "</color>";
			}

			return;
		}

		DebugLog.LogWarning (www.text);

		JSON_Object obj = new JSON_Object (www.text);

		if (obj.getString ("status").Equals (APIConstants.STATUS_AUTHORIZED)) {
			API_LoginPlayerInfo loggedInPlayerInfo = new API_LoginPlayerInfo (www.text);
			LoginScript.loggedInPlayer = loggedInPlayerInfo;

//			GetProfileImage (loggedInPlayerInfo.avtar);
			APIConstants.PLAYER_TOKEN = loggedInPlayerInfo.token;

//			UIManager.Instance.loginPanel.gameObject.SetActive (false);
			UIManager.Instance.lobbyPanel.gameObject.SetActive (true);

			GetPlayerInfo ();

			StartUpdatingLoginStatus ();
		} else {
			UIManager.Instance.loginPanel.txtError.text = "<color=red>Something went wrong.</color>";
		}
	}

	/// <summary>
	/// Raises the player info response received event.
	/// </summary>
	/// <param name="www">Www.</param>
	private void OnPlayerInfoResponseReceived (WWW www)
	{
		debugString += "\n" + www.text;
		if (www.error != null) {
			DebugLog.Log (www.error);
			DebugLog.LogWarning (www.error);
			return;
		}
//		DebugLog.Log ("CD <<OnPlayerInfoResponseReceived>> "+www.text);
		DebugLog.LogWarning (www.text);

		API_LoginPlayerInfo loggedInPlayerInfo = new API_LoginPlayerInfo (www.text);
		LoginScript.loggedInPlayer = loggedInPlayerInfo;

		LobbyPanel lobbyPanel = UIManager.Instance.lobbyPanel;

		if (lobbyPanel.availableMoneyType.Contains ("Play Money"))
			lobbyPanel.txtPlayerTotalPlayChips.text = "Total Play Chips: <color=" + APIConstants.HEX_COLOR_YELLOW + ">" + Utility.GetCommaSeperatedAmount (LoginScript.loggedInPlayer.balance_chips) + "</color>     ";
		else
			lobbyPanel.txtPlayerTotalPlayChips.text = "";

		if (lobbyPanel.availableMoneyType.Contains ("Real Money"))
			lobbyPanel.txtPlayerTotalRealMoney.text = "Total Real Money: <color=" + APIConstants.HEX_COLOR_YELLOW + ">" + Utility.GetCommaSeperatedAmount (LoginScript.loggedInPlayer.balance_cash, true) + "</color>";
		else
			lobbyPanel.txtPlayerTotalRealMoney.text = "";
	}

	/// <summary>
	/// Raises the player image received event.
	/// </summary>
	/// <param name="www">Www.</param>
	private void OnPlayerImageReceived (WWW www)
	{
		debugString += "\n" + www.text;
		if (www.error != null) {
			DebugLog.Log (www.error);
			DebugLog.LogWarning (www.error);
			return;
		}

		DebugLog.LogWarning (www.text);
	}

	/// <summary>
	/// Raises the tips response received event.
	/// </summary>
	/// <param name="www">Www.</param>
	private void OnTipsResponseReceived (WWW www)
	{
		debugString += "\n" + www.text;
		if (www.error != null) {
			DebugLog.Log (www.error);
			DebugLog.LogWarning (www.error);
			return;
		}

		DebugLog.LogWarning (www.text);

		FireTipsResponseReceived (www.text);
	}

	/// <summary>
	/// Raises the game rules response received event.
	/// </summary>
	/// <param name="www">Www.</param>
	private void OnGameRulesResponseReceived (WWW www)
	{
		debugString += "\n" + www.text;
		if (www.error != null) {
			DebugLog.Log (www.error);
			DebugLog.LogWarning (www.error);
			return;
		}

		DebugLog.LogWarning (www.text);

		FireGameRulesResponseReceived (www.text);
	}

	/// <summary>
	/// Raises the update login status done event.
	/// </summary>
	/// <param name="www">Www.</param>
	private void OnUpdateLoginStatusDone (WWW www)
	{
		debugString += "\n" + www.text;
		if (www.error != null) {
			DebugLog.Log (www.error);
			DebugLog.LogWarning (www.error);
			return;
		}

		DebugLog.LogWarning (www.text);
	}

	/// <summary>
	/// Raises the logout done event.
	/// </summary>
	/// <param name="www">Www.</param>
	private void OnLogoutDone (WWW www, System.Action<bool> action)
	{
		debugString += "\n" + www.text;
		if (www.error != null) {
			DebugLog.Log (www.error);
			DebugLog.LogWarning (www.error);
			action (false);
			return;
		}

		action (true);
		DebugLog.LogWarning (www.text);
	}

	#endregion
}
[SerializeField]
public class GetlastHourAchivement
{
	public string winning_amount ;
	public string lose_amount ;
}