using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LobbyAPIManager : MonoBehaviour
{
	#region PUBLIC_VARIABLES

	#endregion

	#region PRIVATE_VARIABLES

	public bool debugEnabled = false;
	public static string debugString = "<b>API Messages</b>";

	#endregion

	public delegate void SingleGameInfoReceived (API_SingleGameInfo gameInfo);

	public static event SingleGameInfoReceived singleGameInfoReceived;

	public static void FireSingleGameInfoReceived (API_SingleGameInfo gameInfo)
	{
		if (singleGameInfoReceived != null)
			singleGameInfoReceived (gameInfo);
	}

	public delegate void TournamentAwardInfoReceived (API_TournamentAwardInfo awardInfo);

	public static event TournamentAwardInfoReceived tournamentAwardInfoReceived;

	public static void FireTournamentAwardInfoReceived (API_TournamentAwardInfo awardInfo)
	{
		if (tournamentAwardInfoReceived != null)
			tournamentAwardInfoReceived (awardInfo);
	}

	public delegate void RegisteredPlayersInfoReceived (API_TournamentRegPlayers regPlayers);

	public static event RegisteredPlayersInfoReceived registeredPlayerInfoReceived;

	public static void FireRegisteredPlayersInfoReceived (API_TournamentRegPlayers regPlayers)
	{
		if (registeredPlayerInfoReceived != null)
			registeredPlayerInfoReceived (regPlayers);
	}

	public delegate void BuyinAmountSetDone (bool success);

	public static event BuyinAmountSetDone buyInAmountSetDone;

	public static void FireBuyinAmountSetDone (bool success)
	{
		if (buyInAmountSetDone != null)
			buyInAmountSetDone (success);
	}

	public delegate void RegisterInTournamentDone (bool success);

	public static event RegisterInTournamentDone registerInTournamentDone;

	public static void FireRegisterInTournamentDone (bool success)
	{
		if (registerInTournamentDone != null)
			registerInTournamentDone (success);
	}

	static LobbyAPIManager Instance;

	public static LobbyAPIManager GetInstance ()
	{
		if (Instance == null) {
			Instance = new GameObject ("LobbyAPIManager").AddComponent<LobbyAPIManager> ();
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
		if (debugEnabled) {
			if (GUI.Button (new Rect (10, 10, 50, 25), "Clear")) {
				debugString = "";
			}

			GUI.Label (new Rect (10, 35, 500, 200), debugString);
		}
	}

	#endregion

	#region PUBLIC_METHODS

	public void GetGames (Dictionary<string, string> dict)
	{
		StopCoroutine ("GetGamesFromServer");
		StartCoroutine ("GetGamesFromServer", dict);
	}

	public void GetSingleGameInfo (string gameID)
	{
		StopCoroutine ("GetSingleGameInfoFromServer");
		StartCoroutine ("GetSingleGameInfoFromServer", gameID);
	}

	public void GetTournamentInfo (string gameId, Action<WWW> resp)
	{
		StartCoroutine (GetTournamentInfoFromServer (gameId, resp));
	}

	public void GetTournamentAwardInfo (string gameID)
	{
		StopCoroutine ("GetTournamentAwardInfoFromServer");
		StartCoroutine ("GetTournamentAwardInfoFromServer", gameID);
	}

	public void GetGamesByNavigationUrl (Dictionary<string, string> dict)
	{
		StopCoroutine ("GetGamesByURL");
		StopCoroutine ("GetGamesFromServer");

		StartCoroutine ("GetGamesByURL", dict);
	}

	public void GetTournamentRegisteredPlayers (string gameID)
	{
		StartCoroutine (GetTournamentRegisteredPlayersFromServer (gameID));
	}

	public void SetBuyin (string gameID, double buyInAmount)
	{
		StartCoroutine (SetBuyinAmount (gameID, buyInAmount));
	}

	public void RegisterInTournament (string gameID, string tourType)
	{
		StartCoroutine (RegisterPlayerInTournament (gameID, tourType));
	}

	public void GetGameDetail (Action <WWW> response)
	{
		StartCoroutine (GetGameDetailRequest (response));
	}

	#endregion

	#region PRIVATE_METHODS

	#endregion

	#region COROUTINES

	private IEnumerator GetGamesFromServer (Dictionary<string, string> dict)
	{
		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add (APIConstants.FIELD_AUTHORIZATION, "Bearer " + APIConstants.PLAYER_TOKEN);
		headers.Add ("Content-Type", "application/x-www-form-urlencoded");

//		Debug.Log (APIConstants.FIELD_AUTHORIZATION + "\t\t" + "Bearer " + APIConstants.PLAYER_TOKEN);
		WWWForm form = new WWWForm ();
		form.AddField (APIConstants.GAME_FILTER_GAME_TYPE, dict [APIConstants.FIELD_GAME_TYPE]);
		form.AddField (APIConstants.GAME_FILTER_POKER_TYPE, dict [APIConstants.FIELD_POKER_TYPE]);
		form.AddField (APIConstants.GAME_FILTER_MONEY_TYPE, dict [APIConstants.FIELD_MONEY_TYPE]);
		form.AddField (APIConstants.GAME_FILTER_GAME_SPEED, dict [APIConstants.FIELD_GAME_SPEED]);
		form.AddField (APIConstants.GAME_FILTER_LIMIT, dict [APIConstants.FIELD_LIMIT]);
		form.AddField (APIConstants.FIELD_PER_PAGE, APIConstants.GAMES_PER_TABLE.ToString ());

		if (dict.ContainsKey (APIConstants.FIELD_TOUR_TYPE)) {
			form.AddField (APIConstants.GAME_FILTER_TOUR_TYPE, dict [APIConstants.FIELD_TOUR_TYPE]);
//			Debug.Log (APIConstants.GAME_FILTER_TOUR_TYPE + "\t\t" + dict [APIConstants.FIELD_TOUR_TYPE]);
		}

//		Debug.Log (APIConstants.GAME_FILTER_GAME_TYPE + "\t\t" + dict [APIConstants.FIELD_GAME_TYPE]);
//		Debug.Log (APIConstants.GAME_FILTER_POKER_TYPE + "\t\t" + dict [APIConstants.FIELD_POKER_TYPE]);
//		Debug.Log (APIConstants.GAME_FILTER_MONEY_TYPE + "\t\t" + dict [APIConstants.FIELD_MONEY_TYPE]);
//		Debug.Log (APIConstants.GAME_FILTER_GAME_SPEED + "\t\t" + dict [APIConstants.FIELD_GAME_SPEED]);
//		Debug.Log (APIConstants.GAME_FILTER_LIMIT + "\t\t" + dict [APIConstants.FIELD_LIMIT]);
//		Debug.Log (APIConstants.FIELD_PER_PAGE + "\t\t" + APIConstants.GAMES_PER_TABLE.ToString ());

		byte[] rawData = form.data;

		WWW www = new WWW (APIConstants.GAMES_URL, rawData, headers);

		yield return www;

		OnGamesListReceived (www);
	}

	private IEnumerator GetSingleGameInfoFromServer (string gameID)
	{
		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add (APIConstants.FIELD_AUTHORIZATION, "Bearer " + APIConstants.PLAYER_TOKEN);
		headers.Add ("Content-Type", "application/x-www-form-urlencoded");

		WWW www = new WWW (APIConstants.GAMES_URL + "/" + gameID, null, headers);

		yield return www;

		OnSingleGameInfoReceived (www);
	}

	private IEnumerator GetTournamentInfoFromServer (string gameId, Action<WWW> resp)
	{
		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add (APIConstants.FIELD_AUTHORIZATION, "Bearer " + APIConstants.PLAYER_TOKEN);
		headers.Add ("Content-Type", "application/x-www-form-urlencoded");

		WWW www = new WWW (APIConstants.GAMES_URL + "/" + gameId + "/" + "info", null, headers);

		yield return www;

		resp (www);
	}

	private IEnumerator GetTournamentAwardInfoFromServer (string gameID)
	{
		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add (APIConstants.FIELD_AUTHORIZATION, "Bearer " + APIConstants.PLAYER_TOKEN);
		headers.Add ("Content-Type", "application/x-www-form-urlencoded");

		WWW www = new WWW (APIConstants.GAMES_URL + "/" + gameID + APIConstants.TOURNAMENT_AWARD_URL_KEY, null, headers);

		yield return www;

		OnTournamentAwardInfoReceived (www);
	}

	private IEnumerator GetGamesByURL (Dictionary<string, string> dict)
	{
		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add (APIConstants.FIELD_AUTHORIZATION, "Bearer " + APIConstants.PLAYER_TOKEN);
		headers.Add ("Content-Type", "application/x-www-form-urlencoded");

		WWWForm form = new WWWForm ();
		form.AddField (APIConstants.GAME_FILTER_GAME_TYPE, dict [APIConstants.FIELD_GAME_TYPE]);
		form.AddField (APIConstants.GAME_FILTER_POKER_TYPE, dict [APIConstants.FIELD_POKER_TYPE]);
		form.AddField (APIConstants.GAME_FILTER_MONEY_TYPE, dict [APIConstants.FIELD_MONEY_TYPE]);
		form.AddField (APIConstants.GAME_FILTER_GAME_SPEED, dict [APIConstants.FIELD_GAME_SPEED]);
		form.AddField (APIConstants.GAME_FILTER_LIMIT, dict [APIConstants.FIELD_LIMIT]);
		form.AddField (APIConstants.FIELD_PER_PAGE, APIConstants.GAMES_PER_TABLE.ToString ());
		if (dict.ContainsKey (APIConstants.FIELD_TOUR_TYPE))
			form.AddField (APIConstants.GAME_FILTER_TOUR_TYPE, dict [APIConstants.FIELD_TOUR_TYPE]);

		byte[] rawData = form.data;

		WWW www = new WWW (dict [APIConstants.FIELD_URL], rawData, headers);

		yield return www;

		OnGamesListReceived (www);
	}

	private IEnumerator GetTournamentRegisteredPlayersFromServer (string gameID)
	{
		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add (APIConstants.FIELD_AUTHORIZATION, "Bearer " + APIConstants.PLAYER_TOKEN);
		headers.Add ("Content-Type", "application/x-www-form-urlencoded");

		WWW www = new WWW (APIConstants.GAMES_URL + "/" + gameID + APIConstants.TOURNAMENT_REGISTERED_PLAYERS_URL_KEY, null, headers);

		yield return www;

		OnRegisteredPlayersInfoReceived (www);
	}

	private IEnumerator SetBuyinAmount (string gameID, double buyInAmount)
	{
		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add (APIConstants.FIELD_AUTHORIZATION, "Bearer " + APIConstants.PLAYER_TOKEN);
		headers.Add ("Content-Type", "application/x-www-form-urlencoded");

		WWWForm form = new WWWForm ();
		form.AddField (APIConstants.FIELD_BUY_IN, buyInAmount.ToString ());

		byte[] rawData = form.data;

		WWW www = new WWW (APIConstants.GAMES_URL + "/" + gameID + APIConstants.JOIN_CASH_GAME_URL_KEY, rawData, headers);

		yield return www;

		OnSetBuyinAmountDone (www);
	}

	private IEnumerator RegisterPlayerInTournament (string gameID, string tourType)
	{
		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add (APIConstants.FIELD_AUTHORIZATION, "Bearer " + APIConstants.PLAYER_TOKEN);
		headers.Add ("Content-Type", "application/x-www-form-urlencoded");

		string tournamentURL = APIConstants.GAMES_URL + "/" + gameID + APIConstants.JOIN_SNG_TOUR_URL_KEY;
		if (tourType.Equals (APIConstants.SNG_TOUR_GAME_TYPE))
			tournamentURL = APIConstants.GAMES_URL + "/" + gameID + APIConstants.JOIN_SNG_TOUR_URL_KEY;
		else
			tournamentURL = APIConstants.GAMES_URL + "/" + gameID + APIConstants.JOIN_REGULAR_TOUR_URL_KEY;

		WWW www = new WWW (tournamentURL, null, headers);

		yield return www;

		OnRegisterInTournamentDone (www);
	}

	private IEnumerator GetGameDetailRequest (Action <WWW> response)
	{
		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add (APIConstants.FIELD_AUTHORIZATION, "Bearer " + APIConstants.PLAYER_TOKEN);
		headers.Add ("Content-Type", "application/x-www-form-urlencoded");

		string gameDetailURL = APIConstants.URL_GET_GAME_DETAIL;
		WWW www = new WWW (gameDetailURL, null, headers);

		yield return www;

		response (www);
	}

	#endregion

	#region API_CALLBACKS

	private void OnGamesListReceived (WWW www)
	{
		UIManager.Instance.lobbyPanel.backgroundLoader.SetActive (false);

		debugString += "\n" + www.text;
		if (www.error != null) {
			DebugLog.Log (www.error);
			return;
		}

		DebugLog.LogWarning (www.text);

		API_GameInfo gameInfo = new API_GameInfo (www.text);
		if (UIManager.Instance.lobbyPanel.cashGamePanel.gameObject.activeSelf) {
			UIManager.Instance.lobbyPanel.cashGamePanel.loaderPanel.gameObject.SetActive (false);
			UIManager.Instance.lobbyPanel.cashGamePanel.SetCashGameObjects (gameInfo);
		} else if (UIManager.Instance.lobbyPanel.regularTournamentPanel.gameObject.activeSelf) {
			UIManager.Instance.lobbyPanel.regularTournamentPanel.loaderPanel.gameObject.SetActive (false);
			UIManager.Instance.lobbyPanel.regularTournamentPanel.SetRegularTournamentObjects (gameInfo);
		} else if (UIManager.Instance.lobbyPanel.sngTournamentPanel.gameObject.activeSelf) {
			UIManager.Instance.lobbyPanel.sngTournamentPanel.loaderPanel.gameObject.SetActive (false);
			UIManager.Instance.lobbyPanel.sngTournamentPanel.SetSngTournamentObjects (gameInfo);
		}

		if (gameInfo.next_page_url != null && gameInfo.next_page_url.Length > 0) {
			UIManager.Instance.lobbyPanel.nextPageUrl = gameInfo.next_page_url;
			UIManager.Instance.lobbyPanel.btnNext.interactable = true;
		} else {
			UIManager.Instance.lobbyPanel.nextPageUrl = "";
			UIManager.Instance.lobbyPanel.btnNext.interactable = false;
		}

		if (gameInfo.prev_page_url != null && gameInfo.prev_page_url.Length > 0) {
			UIManager.Instance.lobbyPanel.previousPageUrl = gameInfo.prev_page_url;
			UIManager.Instance.lobbyPanel.btnPrevious.interactable = true;
		} else {
			UIManager.Instance.lobbyPanel.previousPageUrl = "";
			UIManager.Instance.lobbyPanel.btnPrevious.interactable = false;
		}

		if (gameInfo.last_page != 0)
			UIManager.Instance.lobbyPanel.txtCurrentPage.text = gameInfo.current_page + "/" + gameInfo.last_page;
		else
			UIManager.Instance.lobbyPanel.txtCurrentPage.text = "0/" + gameInfo.last_page;
	}

	private void OnSingleGameInfoReceived (WWW www)
	{
		debugString += "\n" + www.text;
		if (www.error != null) {
			DebugLog.Log (www.error);
			return;
		}

		DebugLog.LogWarning (www.text);

//		API_SingleGameInfo singleGameInfo = new API_SingleGameInfo (www.text);
		FireSingleGameInfoReceived (JsonUtility.FromJson <API_SingleGameInfo> (www.text));
	}

	private void OnTournamentAwardInfoReceived (WWW www)
	{
		debugString += "\n" + www.text;
		if (www.error != null) {
			DebugLog.Log (www.error);
			return;
		}

		DebugLog.LogWarning (www.text);

		API_TournamentAwardInfo awardInfo = new API_TournamentAwardInfo (www.text);
		FireTournamentAwardInfoReceived (awardInfo);
	}

	private void OnRegisteredPlayersInfoReceived (WWW www)
	{
		debugString += "\n" + www.text;
		if (www.error != null) {
			DebugLog.Log (www.error);
			return;
		}

		DebugLog.LogWarning (www.text);

		API_TournamentRegPlayers regPlayers = new API_TournamentRegPlayers (www.text);
		FireRegisteredPlayersInfoReceived (regPlayers);
	}

	private void OnSetBuyinAmountDone (WWW www)
	{
		debugString += "\n" + www.text;
		if (www.error != null) {
			DebugLog.Log (www.error);
			FireBuyinAmountSetDone (false);
			return;
		}

		DebugLog.LogWarning (www.text);

		if (www.text.Equals ("ok"))
			FireBuyinAmountSetDone (true);
		else
			FireBuyinAmountSetDone (false);
	}

	private void OnRegisterInTournamentDone (WWW www)
	{
		debugString += "\n" + www.text;
		if (www.error != null) {
			DebugLog.Log (www.error);
			FireRegisterInTournamentDone (false);
			UIManager.Instance.DisplayMessagePanel ("Tournament Registration", Constants.MESSAGE_SOMETHING_WENT_WRONG);
			return;
		}

		DebugLog.LogWarning (www.text);

		if (www.text.Equals ("ok")) {
			FireRegisterInTournamentDone (true);
		} else {
			FireRegisterInTournamentDone (false);
			JSON_Object obj = new JSON_Object (www.text);
			UIManager.Instance.DisplayMessagePanel ("Tournament Registration", obj.getString ("msg"));
		}
	}

	#endregion
}