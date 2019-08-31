using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SocketIO;

public class LobbyPanel : MonoBehaviour
{
	#region PUBLIC_VARIABLES

	public Text txtOnlinePlayers;
	public Text txtTournaments;

	public Dropdown ddGameType;
	public Dropdown ddTournamentType;
	public Dropdown ddPokerType;
	public Dropdown ddMoneyType;
	public Dropdown ddGameSpeed;
	public Dropdown ddLimitType;

	public LobbyCashGamePanel cashGamePanel;
	public LobbyRegularTournamentPanel regularTournamentPanel;
	public LobbySngTournamentPanel sngTournamentPanel;

	public string nextPageUrl;
	public string previousPageUrl;

	public Button btnNext;
	public Button btnPrevious;
	public Button btnCurrent;

	public Text txtCurrentPage;

	public GameObject backgroundLoader;

	[Range (3, 10)]
	public int RefreshGameDetailInterval = 3;

	[Space (10)]
	[Header ("Player Profile")]
	public Image imgPlayerProfile;
	public Text txtPlayerName;
	public Text txtPlayerTotalPlayChips;
	public Text txtPlayerTotalRealMoney;

	#endregion

	#region PRIVATE_VARIABLES

	private int _onlinePlayers;
	private int _tournaments;

	//	public List<string> availableGamesType = new List<string> ();
	public List<string> availablePokerType = new List<string> ();
	public List<string> availableMoneyType = new List<string> ();
	//	public List<string> availableGameSpeedType = new List<string> ();
	//	public List<string> availableLimitType = new List<string> ();
	//	public List<string> availableTournamentType = new List<string> ();

	private API_AvailableGames lastAvailableGameResponse = new API_AvailableGames ();

	#endregion

	#region UNITY_CALLBACKS

	// Use this for initialization
	void Start ()
	{
		backgroundLoader.SetActive (false);
		OnlinePlayers = Tournaments = 0;

		txtOnlinePlayers.gameObject.SetActive (Application.platform == RuntimePlatform.WebGLPlayer && UIManager.Instance.isAffiliate);
		txtTournaments.gameObject.SetActive (Application.platform == RuntimePlatform.WebGLPlayer && UIManager.Instance.isAffiliate);
	}

	void OnEnable ()
	{
		if (!UIManager.Instance.isAffiliate) {
			availablePokerType = new List<string> (){ "All", "WhoopAss", "Texas", "Table" };
			availableMoneyType = new List<string> (){ "All", "Play Money", "Real Money" };
		}

		btnNext.interactable = false;
		btnPrevious.interactable = false;
//		btnCurrent.interactable = false;
		APIManager.GetInstance ().GetPlayerInfo ();
		SetDropDownOptions ();
		GetFilteredData (true);

		SetPlayerProfile ();

		PokerSocketManager.onGameCreated += HandleOnGameCreated;
		PokerSocketManager.onJoinCashGame += HandleOnJoinCashGame;
		PokerSocketManager.onLeaveCashGame += HandleOnLeaveCashGame;
		PokerSocketManager.onPlayerJoinedCashGame += HandleOnPlayerJoinedCashGame;

		StartCoroutine (GetGameDetail ());
		UIAccount.AccountAPIManager.GetInstance ().GetAccountDetail ();
	}

	void OnDisable ()
	{
		UIManager.Instance.playerDetailPanel.gameObject.SetActive (false);

		PokerSocketManager.onGameCreated -= HandleOnGameCreated;
		PokerSocketManager.onJoinCashGame -= HandleOnJoinCashGame;
		PokerSocketManager.onLeaveCashGame -= HandleOnLeaveCashGame;
		PokerSocketManager.onPlayerJoinedCashGame -= HandleOnPlayerJoinedCashGame;

		APIManager.GetInstance ().StopUpdatingLoginStauts ();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape) && !UIManager.Instance.loader.gameObject.activeSelf) {
			if (!cashGamePanel.cashGameDetailPanel.gameObject.activeSelf &&
			    !regularTournamentPanel.tournamentDetailPanel.gameObject.activeSelf &&
			    !sngTournamentPanel.tournamentDetailPanel.gameObject.activeSelf) {
				if (UIManager.Instance.playerDetailPanel.gameObject.activeSelf)
					UIManager.Instance.playerDetailPanel.gameObject.SetActive (false);
				else if (UIManager.Instance.logoutConfirmationPanel.gameObject.activeSelf)
					UIManager.Instance.logoutConfirmationPanel.gameObject.SetActive (false);
				else
					UIManager.Instance.logoutConfirmationPanel.gameObject.SetActive (true);

				SoundManager.Instance.PlayButtonTapSound ();
			}
		}
	}

	#endregion

	#region SOCKET_DELEGATE_CALLBACKS

	private void HandleOnGameCreated (Packet gameInfo)
	{
		Debug.Log (gameInfo.ToString ());

		string selectedGameType = ddGameType.captionText.text;
		string selectedPokerType = ddPokerType.captionText.text;
		string selectedMoneyType = ddMoneyType.captionText.text;
		int selectedGameSpeed = ddGameSpeed.value;
		int selectedLimitType = ddLimitType.value;
		string selectedTournamentType = ddTournamentType.captionText.text;

		JSONArray arr = new JSONArray (gameInfo.ToString ());
		JSON_Object dataObj = arr.getJSONObject (1).getJSONObject ("data");
		JSON_Object gameObj = dataObj.getJSONObject ("game");

		string gameType = gameObj.getString (APIConstants.FIELD_GAME_TYPE);
		string pokerType = gameObj.getString (APIConstants.FIELD_POKER_TYPE);
		string moneyType = gameObj.getString (APIConstants.FIELD_MONEY_TYPE);
		string gameSpeedType = gameObj.getString (APIConstants.FIELD_GAME_SPEED);
		string limitType = gameObj.getString (APIConstants.FIELD_LIMIT);
//		string tournamentType = gameObj.getString (APIConstants.FIELD_TOUR_TYPE);

		if (selectedGameType.Equals (GetCreatedGameType (gameType)) &&
		    (selectedPokerType.Equals ("All") || selectedPokerType.Equals (GetCreatedPokerType (pokerType))) ||
		    (selectedMoneyType.Equals ("All") || selectedMoneyType.Equals (GetCreatedMoneyType (moneyType))) ||
		    (selectedGameSpeed.Equals ("All") || selectedGameSpeed == GetCreatedGameSpeedType (gameSpeedType)) ||
		    (selectedLimitType.Equals ("All") || selectedLimitType == GetCreatedGameLimitType (limitType))) {
			backgroundLoader.SetActive (true);
			GetFilteredData (false);
		}
	}

	private void HandleOnJoinCashGame (Packet gameInfo)
	{
		Debug.Log (gameInfo.ToString ());
	}

	private void HandleOnLeaveCashGame (Packet gameInfo)
	{
		Debug.Log (gameInfo.ToString ());
	}

	private void HandleOnPlayerJoinedCashGame (Packet gameInfo)
	{
		Debug.Log (gameInfo.ToString ());
	}

	#endregion

	#region PUBLIC_METHODS

	public void OnCashGameButtonTap ()
	{
		cashGamePanel.gameObject.SetActive (true);
		regularTournamentPanel.gameObject.SetActive (false);
		sngTournamentPanel.gameObject.SetActive (false);
	}

	public void OnRegularButtonTap ()
	{
		cashGamePanel.gameObject.SetActive (false);
		regularTournamentPanel.gameObject.SetActive (true);
		sngTournamentPanel.gameObject.SetActive (false);
	}

	public void OnSnGButtonTap ()
	{
		cashGamePanel.gameObject.SetActive (false);
		regularTournamentPanel.gameObject.SetActive (false);
		sngTournamentPanel.gameObject.SetActive (true);
	}

	public void OnGameTypeValueChanged ()
	{
		if (ddGameType.captionText.text.Equals ("Games") && !cashGamePanel.gameObject.active) {
			ddTournamentType.gameObject.SetActive (false);

			ddPokerType.ClearOptions ();
			List<Dropdown.OptionData> pokerTypeOptions = new List<Dropdown.OptionData> ();

			if (availablePokerType.Contains ("All"))
				pokerTypeOptions.Add (new Dropdown.OptionData (){ text = "All" });
			if (availablePokerType.Contains ("WhoopAss"))
				pokerTypeOptions.Add (new Dropdown.OptionData (){ text = "WhoopAss" });
			if (availablePokerType.Contains ("Texas"))
				pokerTypeOptions.Add (new Dropdown.OptionData (){ text = "Texas" });
			if (availablePokerType.Contains ("Table"))
				pokerTypeOptions.Add (new Dropdown.OptionData (){ text = "Table" });
			ddPokerType.AddOptions (pokerTypeOptions);

			GetFilteredData (true);
		} else {
			ddTournamentType.gameObject.SetActive (true);

			ddPokerType.ClearOptions ();
			List<Dropdown.OptionData> pokerTypeOptions = new List<Dropdown.OptionData> ();
			if (availablePokerType.Contains ("All"))
				pokerTypeOptions.Add (new Dropdown.OptionData (){ text = "All" });
			if (availablePokerType.Contains ("WhoopAss"))
				pokerTypeOptions.Add (new Dropdown.OptionData (){ text = "WhoopAss" });
			if (availablePokerType.Contains ("Texas"))
				pokerTypeOptions.Add (new Dropdown.OptionData (){ text = "Texas" });
			ddPokerType.AddOptions (pokerTypeOptions);

			int val = ddPokerType.value;
			if (val == 3)
				val--;
			ddPokerType.value = val;

			GetFilteredData (true);
		}
	}

	public void OnTournamentValueChanged ()
	{
		GetFilteredData (true);
	}

	public void OnPokerTypeValueChanged ()
	{
		GetFilteredData (true);
	}

	public void OnMoneyTypeValueChanges ()
	{
		GetFilteredData (true);
	}

	public void OnGameSpeedValueChanged ()
	{
		GetFilteredData (true);
	}

	public void OnLimitTypeValueChanged ()
	{
		GetFilteredData (true);
	}

	public void OnNextButtonTap ()
	{
//		cashGamePanel.gameObject.SetActive (false);
//		regularTournamentPanel.gameObject.SetActive (false);
//		sngTournamentPanel.gameObject.SetActive (false);

		btnNext.interactable = false;
		GetFilteredData (true, nextPageUrl);
//		LobbyAPIManager.GetInstance ().GetGamesByNavigationUrl (nextPageUrl);
	}

	public void OnPreviousButtonTap ()
	{
//		cashGamePanel.gameObject.SetActive (false);
//		regularTournamentPanel.gameObject.SetActive (false);
//		sngTournamentPanel.gameObject.SetActive (false);

		btnPrevious.interactable = false;
		GetFilteredData (true, previousPageUrl);
//		LobbyAPIManager.GetInstance ().GetGamesByNavigationUrl (previousPageUrl);
	}

	public void GetFilteredData (bool displayLoader, string url = null)
	{
//		cashGamePanel.gameObject.SetActive (false);
//		regularTournamentPanel.gameObject.SetActive (false);
//		sngTournamentPanel.gameObject.SetActive (false);

		btnNext.interactable = btnPrevious.interactable = false;

		string gameType = "";
		string pokerType = "";
		string moneyType = "";
		string gameSpeed = "";
		string limitType = "";
		string tournamentType = "";

		switch (ddGameType.captionText.text) {
//		case 0:
//			gameType = "all";
//			cashGamePanel.gameObject.SetActive (false);
//			regularTournamentPanel.gameObject.SetActive (false);
//			sngTournamentPanel.gameObject.SetActive (false);
//			break;
		case "Games":
			gameType = "cash game";
			cashGamePanel.gameObject.SetActive (true);
			regularTournamentPanel.gameObject.SetActive (false);
			sngTournamentPanel.gameObject.SetActive (false);

			if (displayLoader)
				cashGamePanel.loaderPanel.gameObject.SetActive (true);
			break;
		case "Tournament":
			gameType = "tournament";
			cashGamePanel.gameObject.SetActive (false);
			regularTournamentPanel.gameObject.SetActive (false);
			sngTournamentPanel.gameObject.SetActive (false);
			break;
//		default:
//			gameType = "cash game";
//			cashGamePanel.gameObject.SetActive (true);
//			regularTournamentPanel.gameObject.SetActive (false);
//			sngTournamentPanel.gameObject.SetActive (false);
//
//			if (displayLoader)
//				cashGamePanel.loaderPanel.gameObject.SetActive (true);
//			break;
		}

		switch (ddTournamentType.captionText.text) {
		case "Regular":
			tournamentType = "regular";
			if (ddTournamentType.gameObject.activeSelf) {
				cashGamePanel.gameObject.SetActive (false);
				regularTournamentPanel.gameObject.SetActive (true);
				sngTournamentPanel.gameObject.SetActive (false);

				if (displayLoader)
					regularTournamentPanel.loaderPanel.gameObject.SetActive (true);
			}
			break;
		case "SnG":
			tournamentType = "sng";
			if (ddTournamentType.gameObject.activeSelf) {
				cashGamePanel.gameObject.SetActive (false);
				regularTournamentPanel.gameObject.SetActive (false);
				sngTournamentPanel.gameObject.SetActive (true);

				if (displayLoader)
					sngTournamentPanel.loaderPanel.gameObject.SetActive (true);
			}
			break;
		}

//		return;

		switch (ddPokerType.captionText.text) {
		case "All":
			pokerType = "all";
			break;
		case "WhoopAss":
			pokerType = "wa";
			break;
		case "Texas":
			pokerType = "th";
			break;
		case "Table":
			pokerType = "table";
			break;
//		default:
//			pokerType = "all";
//			break;
		}

		switch (ddMoneyType.captionText.text) {
		case "All":
			moneyType = "all";
			break;
		case "Play Money":
			moneyType = "play money";
			break;
		case "Real Money":
			moneyType = "real money";
			break;
//		default:
//			moneyType = "all";
//			break;
		}

		switch (ddGameSpeed.captionText.text) {
		case "All":
			gameSpeed = "all";
			break;
		case "Regular":
			gameSpeed = "regular";
			break;
		case "Turbo":
			gameSpeed = "turbo";
			break;
		case "Hyper Turbo":
			gameSpeed = "hyper turbo";
			break;
//		default:
//			gameSpeed = "all";
//			break;
		}

		switch (ddLimitType.captionText.text) {
		case "All":
			limitType = "all";
			break;
		case "Limit":
			limitType = "yes";
			break;
		case "No Limit":
			limitType = "no";
			break;
//		default:
//			limitType = "all";
//			break;
		}

		Dictionary<string, string> dict = new Dictionary<string, string> ();
		dict.Add (APIConstants.FIELD_GAME_TYPE, gameType);
		dict.Add (APIConstants.FIELD_POKER_TYPE, pokerType);
		dict.Add (APIConstants.FIELD_MONEY_TYPE, moneyType);
		dict.Add (APIConstants.FIELD_GAME_SPEED, gameSpeed);
		dict.Add (APIConstants.FIELD_LIMIT, limitType);

		if (ddTournamentType.gameObject.activeSelf)
			dict.Add (APIConstants.FIELD_TOUR_TYPE, tournamentType);

		if (url == null)
			LobbyAPIManager.GetInstance ().GetGames (dict);
		else {
			dict.Add (APIConstants.FIELD_URL, url);
			LobbyAPIManager.GetInstance ().GetGamesByNavigationUrl (dict);
		}

		SoundManager.Instance.PlayButtonTapSound ();
	}

	public void OnDashboardButtonTap ()
	{
		UIManager.Instance.dashboardPanel.gameObject.SetActive (true);
		gameObject.SetActive (false);
		SoundManager.Instance.PlayButtonTapSound ();
	}

	public void OnLogoutButtonTap ()
	{
		UIManager.Instance.logoutConfirmationPanel.gameObject.SetActive (true);
		SoundManager.Instance.PlayButtonTapSound ();
	}

	public void OnPlayerProfileButtonTap ()
	{
		string playMoney = Utility.GetCommaSeperatedPlayMoneyAmount (LoginScript.loggedInPlayer.balance_chips);
		string realMoney = Utility.GetCommaSeperatedAmount (LoginScript.loggedInPlayer.balance_cash, true);

		MoneyType moneyType = MoneyType.All;
		if (availableMoneyType.Contains ("Real Money") && availableMoneyType.Contains ("Play Money"))
			moneyType = MoneyType.All;
		else if (availableMoneyType.Contains ("Real Money"))
			moneyType = MoneyType.RealMoney;
		else if (availableMoneyType.Contains ("Play Money"))
			moneyType = MoneyType.PlayMoney;

		UIManager.Instance.playerDetailPanel.SetPlayerDetails (imgPlayerProfile.transform.position, imgPlayerProfile.sprite, LoginScript.loggedInPlayer.name, playMoney, realMoney, moneyType);
	}

	#endregion

	#region PRIVATE_METHODS

	private void SetDropDownOptions ()
	{
		ddGameType.ClearOptions ();
		ddTournamentType.ClearOptions ();
		ddPokerType.ClearOptions ();
		ddMoneyType.ClearOptions ();
		ddGameSpeed.ClearOptions ();
		ddLimitType.ClearOptions ();

		List<Dropdown.OptionData> gameTypeOptions = new List<Dropdown.OptionData> ();
//		gameTypeOptions.Add ("All");

		gameTypeOptions.Add (new Dropdown.OptionData (){ text = "Games" });
		if (availablePokerType.Count == 1 && availablePokerType [0].Equals ("Table")) {
		} else
			gameTypeOptions.Add (new Dropdown.OptionData (){ text = "Tournament" });
		
		ddGameType.AddOptions (gameTypeOptions);

		List<Dropdown.OptionData> tourTypeOptions = new List<Dropdown.OptionData> ();
		tourTypeOptions.Add (new Dropdown.OptionData () { text = "Regular" });
		tourTypeOptions.Add (new Dropdown.OptionData (){ text = "SnG" });
		ddTournamentType.AddOptions (tourTypeOptions);

		List<Dropdown.OptionData> pokerTypeOptions = new List<Dropdown.OptionData> ();
		if (availablePokerType.Contains ("All"))
			pokerTypeOptions.Add (new Dropdown.OptionData (){ text = "All" });
		if (availablePokerType.Contains ("WhoopAss"))
			pokerTypeOptions.Add (new Dropdown.OptionData (){ text = "WhoopAss" });
		if (availablePokerType.Contains ("Texas"))
			pokerTypeOptions.Add (new Dropdown.OptionData (){ text = "Texas" });
		if (availablePokerType.Contains ("Table"))
			pokerTypeOptions.Add (new Dropdown.OptionData (){ text = "Table" });
		ddPokerType.AddOptions (pokerTypeOptions);

		List<Dropdown.OptionData> moneyTypeOptions = new List<Dropdown.OptionData> ();
		if (availableMoneyType.Contains ("All"))
			moneyTypeOptions.Add (new Dropdown.OptionData (){ text = "All" });
		if (availableMoneyType.Contains ("Play Money"))
			moneyTypeOptions.Add (new Dropdown.OptionData (){ text = "Play Money" });
		if (availableMoneyType.Contains ("Real Money"))
			moneyTypeOptions.Add (new Dropdown.OptionData (){ text = "Real Money" });
		ddMoneyType.AddOptions (moneyTypeOptions);

		List<Dropdown.OptionData> gameSpeedOptions = new List<Dropdown.OptionData> ();
		gameSpeedOptions.Add (new Dropdown.OptionData (){ text = "All" });
		gameSpeedOptions.Add (new Dropdown.OptionData (){ text = "Regular" });
		gameSpeedOptions.Add (new Dropdown.OptionData (){ text = "Turbo" });
		gameSpeedOptions.Add (new Dropdown.OptionData (){ text = "Hyper Turbo" });
		ddGameSpeed.AddOptions (gameSpeedOptions);

		List<Dropdown.OptionData> limitTypeOptions = new List<Dropdown.OptionData> ();
		limitTypeOptions.Add (new Dropdown.OptionData (){ text = "All" });
		limitTypeOptions.Add (new Dropdown.OptionData (){ text = "Limit" });
		limitTypeOptions.Add (new Dropdown.OptionData (){ text = "No Limit" });
		ddLimitType.AddOptions (limitTypeOptions);
	}

	private void SetPlayerProfile ()
	{
		txtPlayerName.text = LoginScript.loggedInPlayer.name;

		if (availableMoneyType.Contains ("Play Money"))
			txtPlayerTotalPlayChips.text = "Total Play Chips: <color=" + APIConstants.HEX_COLOR_YELLOW + ">" + Utility.GetCommaSeperatedAmount (LoginScript.loggedInPlayer.balance_chips) + "</color>     ";
		else
			txtPlayerTotalPlayChips.text = "";
		
		if (availableMoneyType.Contains ("Real Money"))
			txtPlayerTotalRealMoney.text = "Total Real Money: <color=" + APIConstants.HEX_COLOR_YELLOW + ">" + Utility.GetCommaSeperatedAmount (LoginScript.loggedInPlayer.balance_cash, true) + "</color>";
		else
			txtPlayerTotalRealMoney.text = "";

		StartCoroutine (GetProfilePic (LoginScript.loggedInPlayer.avtar));
	}

	private string GetCreatedGameType (string gameType)
	{
		if (gameType.Equals (APIConstants.CASH_GAME_GAME_TYPE))
			return "Games";

		return "Tournament";
	}

	private string GetCreatedPokerType (string pokerType)
	{
		if (pokerType.Equals (APIConstants.KEY_WHOOPASS))
			return "WhoopAss";
		else if (pokerType.Equals (APIConstants.KEY_TEXASS))
			return "Texas";
		else if (pokerType.Equals (APIConstants.KEY_TABLE))
			return "Table";

		return "All";
	}

	private string GetCreatedMoneyType (string moneyType)
	{
		if (moneyType.Equals (APIConstants.KEY_PLAY_MONEY))
			return "Play Money";
		else if (moneyType.Equals (APIConstants.KEY_REAL_MONEY))
			return "Real Money";

		return "All";
	}

	private int GetCreatedGameSpeedType (string speedType)
	{
		if (speedType.Equals ("regular"))
			return 1;
		else if (speedType.Equals ("turbo"))
			return 2;
		else if (speedType.Equals ("hyper turbo"))
			return 3;

		return 0;
	}

	private int GetCreatedGameLimitType (string limitType)
	{
		if (limitType.Equals ("yes"))
			return 1;
		else if (limitType.Equals ("no"))
			return 2;

		return 0;
	}

	public void RefreshForAffiliateGameUpdate (API_AvailableGames ag)
	{
		availableMoneyType = ag.moneyType;
		availablePokerType = ag.gameType;

		if (!JsonUtility.ToJson (lastAvailableGameResponse).Equals (JsonUtility.ToJson (ag))) {
			if (gameObject.activeSelf) {
				SetDropDownOptions ();
				GetFilteredData (true);
			}

			UIManager.Instance.dashboardPanel.buyChipsPanel.UpdateBuyButtons ();
		}

		lastAvailableGameResponse = ag;
	}

	#endregion

	#region COROUTINES

	private IEnumerator GetProfilePic (string url)
	{
		WWW www = new WWW (Constants.RESIZE_IMAGE_URL + url);
		yield return www;

		if (www.error != null) {
			Debug.LogError ("Error while downloading profile pic  : " + www.error);
		} else {
			if (www.texture != null) {
				imgPlayerProfile.sprite = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), Vector2.zero);
			}
		}
	}

	private IEnumerator GetGameDetail ()
	{
		if (Application.platform == RuntimePlatform.WebGLPlayer && UIManager.Instance.isAffiliate) {
			while (true) {
				LobbyAPIManager.GetInstance ().GetGameDetail ((www) => {
					if (www.error == null) {
						DebugLog.LogError ("GetGameDetail  : " + www.text);
						GameDetailResponse ggd = JsonUtility.FromJson<GameDetailResponse> (www.text);

						OnlinePlayers = ggd.data.online_players;
						Tournaments = ggd.data.tournaments;
					}
				});
				
				yield return new WaitForSeconds (RefreshGameDetailInterval);
			}
		}

		yield return 0;
	}

	#endregion

	#region GETTER_SETTER

	public int OnlinePlayers {
		get {
			return _onlinePlayers;
		}
		set {
			_onlinePlayers = value;
			txtOnlinePlayers.text = "Online: " + _onlinePlayers + " Players";
		}
	}

	public int Tournaments {
		get {
			return _tournaments;
		}
		set {
			_onlinePlayers = value;
			txtTournaments.text = _tournaments + " Tournaments";
		}
	}

	#endregion
}