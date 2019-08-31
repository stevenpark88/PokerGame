using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using com.shephertz.app42.gaming.multiplayer.client.command;
using System.Text.RegularExpressions;

public class LoginScript : MonoBehaviour
{
	public InputField ifUserName;
	public InputField ifPassword;
	public GameObject goLogin;
	public GameObject goRoomList;
	public GameObject goMainMenuPanel;
	public GameObject registerPanel;
	public GameObject loginInputView;
	public GameObject texasGameRoom;

	public GameObject objLogo;
	public Image imgBackground;

	public GameObject loginMenuParent;

	public InputField inputFieldIPAddress;

	public InputField inputFieldGameID;
	public InputField inputFieldTableNumber;

	public Text txtError;

	public static WebGLLogin loginDetails;

	public static API_LoginPlayerInfo loggedInPlayer;

	public Text txtRoomSearchCounter;

	string playerID;
	public bool debugLog;
	private string debug;

	private static int roomSearchCounter = 0;
	private static int maxTimerForRoomSearch;

	public Dropdown dropDownWinningRank;

	public GameObject registrationHeader;

	void Awake ()
	{
		roomSearchCounter = 0;

//		if (Application.platform != RuntimePlatform.WindowsEditor)
		#if UNITY_WEBGL
		if (!UIManager.Instance.isAffiliate)
			Application.ExternalCall ("callbackunityTest", "The game says hello!");
		#endif

		//getDataFormPHP("{\"GameRoomID\":\"1\",\"user_name\":\"Player 1\",\"game_id\":\"1\",\"buyin\":\"500\",\"stake\":\"0\\/0\",\"speed\":\"regular\",\"game_type\":\"Play Money\",\"max_player\":\"3\",\"balance\":\"9500\",\"GameType\":\"Table\",\"isLimit\":\"1\",\"TableNumber\":\"1\"}");
		//getDataFormPHP("{\"GameRoomID\":\"16\",\"user_name\":\"Player 1\",\"game_id\":\"16\",\"buyin\":\"5570\",\"stake\":\"0\\/0\",\"speed\":\"regular\",\"game_type\":\"Play Money\",\"max_player\":\"3\",\"balance\":\"5570\",\"GameType\":\"Table\",\"isLimit\":\"1\",\"TableNumber\":\"1\"}");
//		getDataFormPHP("{\"GameRoomID\":1,\"user_name\":\"Player 4\",\"game_id\":1,\"real_money\":\"98900\",\"play_money\":\"121822\",\"buyin\":\"200.00\",\"stake\":\"10.00\\/20.00\",\"speed\":\"regular\",\"game_type\":\"play money\",\"max_player\":\"9\",\"GameType\":\"TH\",\"isLimit\":\"0\",\"TableNumber\":\"1\"}");
//		getDataFormPHP("{\"GameRoomID\":7,\"user_name\":\"Chirag\",\"game_id\":7,\"real_money\":\"0\",\"play_money\":\"46978\",\"buyin\":\"1000.00\",\"stake\":\"640.00\\/1280.00\",\"speed\":\"regular\",\"game_type\":\"play money\",\"max_player\":10,\"GameType\":\"WA_REGULAR\",\"isLimit\":\"0\",\"TableNumber\":2}");
//		getDataFormPHP ("{\"GameRoomID\":276,\"user_name\":\"Plr55\",\"game_id\":276,\"real_money\":\"1000.0\",\"play_money\":\"100035.0\",\"buyin\":\"60.00\",\"stake\":\"0.00\\/0.00\",\"speed\":\"regular\",\"game_type\":\"play money\",\"max_player\":3,\"GameType\":\"TABLE\",\"isLimit\":\"1\",\"TableNumber\":1}");

		inputFieldIPAddress.text = PlayerPrefs.GetString ("LocalC_IP", "");

//		string a = "{\"event\":\"onTournamentStartedEvent\",\"data\":\"{\\\"game_id\\\":50}\"}";
//		JSONObject o = new JSONObject (a);
//		Debug.Log (o.ToString ());
//
//		string e = o.GetField ("event").ToString ();
//		string d = o.GetField ("data").ToString ().Replace("\\", "");
//		d = d.Remove (0, 1);			//	Remove first double quote "
//		d = d.Remove (d.Length - 1, 1);	//	Remove last double quote "
//
//		JSON_Object dObj = new JSON_Object (d);
//
//		Debug.Log (e);
//		Debug.Log (d);
//		Debug.Log (dObj);
//		Debug.Log (dObj.getString ("game_id"));

		if (UIManager.Instance.isStaticHand)
			dropDownWinningRank.gameObject.SetActive (true);
		else
			dropDownWinningRank.gameObject.SetActive (false);

		SetWinningRankDropdown ();

		UIManager.Instance.isRealMoney = false;
	}

	private string JsonToString (string target, string s)
	{
		string[] newString = Regex.Split (target, s);

		return newString [1];
	}

	// Use this for initialization
	void Start ()
	{
		NetworkManager.Instance.playerID = playerID = System.DateTime.UtcNow.Ticks.ToString ();
//		NetworkManager.Instance.playerID = playerID = "Player 1";

//		ifUserName.text = "user@gmail.com";
//		ifUserName.text = NetworkManager.Instance.playerID;
//		ifPassword.text = "12345678";

		if (Application.platform == RuntimePlatform.WindowsEditor) {
			ifUserName.text = "plr41@gmail.com";
			ifPassword.text = "123456";
		} else {
			if (UIManager.Instance.isTestingExe) {
				ifUserName.text = "plr51@gmail.com";
				ifPassword.text = "123456";
			}
		}

		if (loginDetails != null) {
			UIManager.Instance.isWebGLBuild = true;
			loginMenuParent.SetActive (false);
			SetGameType (loginDetails.GameType);

			UIManager.Instance.isLimitGame = loginDetails.isLimit.Equals ("1");
			UIManager.Instance.isRealMoney = loginDetails.game_type.Equals ("real money");

			NetworkManager.Instance.InitializeServer (UIManager.Instance.gameType);
			UIManager.Instance.DisplayLoader ();
			NetworkManager.Instance.Connect (loginDetails.user_name);
			objLogo.SetActive (false);
//			imgBackground.gameObject.SetActive (false);
		} else {
			UIManager.Instance.isWebGLBuild = false;
			loginMenuParent.SetActive (true);
			objLogo.SetActive (true);
//			imgBackground.gameObject.SetActive (true);
		}

		txtRoomSearchCounter.text = "";
	}

	void OnEnable ()
	{
		NetworkManager.onPlayerConnected += HandleOnConnectSuccessful;
		NetworkManager.onRoomListFetched += HandleOnRoomListFetched;
		NetworkManager.onRoomCreationDone += HandleOnRoomCreationDone;
		NetworkManager.onPlayerSubscribedRoom += HandleOnPlayerSubscribedRoom;
		NetworkManager.onPlayerJoinedRoom += HandleOnPlayerJoinedRoom;

		loginMenuParent.SetActive (true);
		objLogo.SetActive (true);

		registrationHeader.SetActive (false);
//		imgBackground.gameObject.SetActive (true);
	}

	void OnDisable ()
	{
		NetworkManager.onPlayerConnected -= HandleOnConnectSuccessful;
		NetworkManager.onRoomListFetched -= HandleOnRoomListFetched;
		NetworkManager.onRoomCreationDone -= HandleOnRoomCreationDone;
		NetworkManager.onPlayerSubscribedRoom -= HandleOnPlayerSubscribedRoom;
		NetworkManager.onPlayerJoinedRoom -= HandleOnPlayerJoinedRoom;
	}

	void OnGUI ()
	{
		if (!debugLog)
			return;

		if (GUI.Button (new Rect (10, 10, 50, 25), "Clear")) {
			debug = "";
		}

		GUI.contentColor = Color.white;
		GUI.Label (new Rect (10, 35, 500, Screen.height), debug);
	}

	void Update ()
	{
		if (Application.platform != RuntimePlatform.WebGLPlayer) {
			if (Input.GetKeyDown (KeyCode.Escape)) {
				if (WebViewManager.Instance.webViewObject != null || registrationHeader.activeSelf)
					OnWebviewBackButtonTap ();
				else if (!UIManager.Instance.lobbyPanel.gameObject.activeSelf &&
				         !UIManager.Instance.dashboardPanel.gameObject.activeSelf)
					Application.Quit ();
			}
		}
		if (Input.GetKeyDown (KeyCode.KeypadEnter) || Input.GetKeyDown (KeyCode.Return)) {
			onLoginButtonClicked ();
		} else if (Input.GetKeyDown (KeyCode.Tab)) {
			if (ifUserName.isFocused)
				ifPassword.ActivateInputField ();
			else
				ifUserName.ActivateInputField ();
		}
	}

	public void onLoginButtonClicked ()
	{
		string strUserName = ifUserName.text;
		string strPassword = ifPassword.text;

//		Debug.Log ("-----------");
//		int temp = int.Parse ("sadf");
		LoggedUserBean.userName = strUserName;

		SoundManager.Instance.PlayButtonTapSound ();


		//  Use code below to test the tournament
		// ---------------------------------------
		if (inputFieldIPAddress.gameObject.activeSelf){
			PlayerPrefs.SetString("LocalC_IP", inputFieldIPAddress.text);
			NetworkManager.localIPAddressC = inputFieldIPAddress.text;
		}
//		string tableNumber = "1";
//		if (strUserName.Equals ("Chirag"))
//			tableNumber = "10";
//		else if (strUserName.Equals ("Ravi"))
//			tableNumber = "2";
//		else if (strUserName.Equals ("Plr1"))
//			tableNumber = "1";
//		else if (strUserName.Equals ("Plr2"))
//			tableNumber = "4";
//		else if (strUserName.Equals ("Plr3"))
//			tableNumber = "5";
//		else if (strUserName.Equals ("Plr4"))
//			tableNumber = "6";
//		else if (strUserName.Equals ("Plr5"))
//			tableNumber = "7";
//		else if (strUserName.Equals ("Plr6"))
//			tableNumber = "8";
//		else if (strUserName.Equals ("Plr7"))
//			tableNumber = "9";
//		else if (strUserName.Equals ("Plr9"))
//			tableNumber = "10";
//
//		if (strUserName.Equals ("Plr2"))
//			tableNumber = "1";
//		else if (strUserName.Equals ("Plr7"))
//			tableNumber = "2";
//
//
////		  Uncomment below two lines for testing SnG Tournament
////		tableNumber = "1";
////		getDataFormPHP ("{\"GameRoomID\":3,\"user_name\":\"" + strUserName + "\",\"game_id\":3,\"real_money\":\"0\",\"play_money\":\"46978\",\"buyin\":\"1000.00\",\"stake\":\"640.00\\/1280.00\",\"speed\":\"regular\",\"game_type\":\"play money\",\"max_player\":10,\"GameType\":\"WA_SNG\",\"isLimit\":\"0\",\"TableNumber\":\"" + tableNumber + "\"}");
//
//
////			For testing of Regular tournament
//		string _gameID = inputFieldGameID.text;
//		string _tableNumber = inputFieldTableNumber.text;
//		tableNumber = _tableNumber;

//		getDataFormPHP ("{\"GameRoomID\":" + _gameID + ",\"user_name\":\"" + strUserName + "\",\"game_id\":" + _gameID + ",\"real_money\":\"0\",\"play_money\":\"46978\",\"buyin\":\"1000.00\",\"stake\":\"640.00\\/1280.00\",\"speed\":\"regular\",\"game_type\":\"play money\",\"max_player\":10,\"GameType\":\"WA_REGULAR\",\"isLimit\":\"0\",\"TableNumber\":\"" + tableNumber + "\"}");
//		Start ();
//		return;
		// ---------------------------------------

		// For mobile / EXE version
		if (Application.platform != RuntimePlatform.WebGLPlayer || UIManager.Instance.isAffiliate) {
			if (IsLoginDetailValid ()) {
				APIManager.GetInstance ().SendLoginInfo (strUserName, strPassword);
			}
			return;
		}
		// End code

		// TABLE
//		getDataFormPHP ("{\"GameRoomID\":" + 0 + ",\"user_name\":\"" + strUserName + "\",\"game_id\":" + 0 + ",\"real_money\":\"0\",\"play_money\":\"46978\",\"buyin\":\"1000.00\",\"stake\":\"640.00\\/1280.00\",\"speed\":\"regular\",\"game_type\":\"play money\",\"max_player\":3,\"GameType\":\"TABLE\",\"isLimit\":\"0\",\"TableNumber\":\"" + 1 + "\"}");
//		Start ();
//		return;

		if (strPassword.Equals ("")) {
			txtError.text = "<color=red>Password should not be empty.</color>";
		} else if (!strUserName.Equals ("")) {
			txtError.text = "<color=green>Connecting with server.</color>";
//			StartCoroutine(SendGameStartInfoToServer(strUserName,strPassword));
			UIManager.Instance.DisplayLoader ();

			Debug.Log (Constants.REMOVE_BEFORE_LIVE);
			NetworkManager.Instance.InitializeServer (UIManager.Instance.gameType);
			NetworkManager.Instance.Connect (strUserName);
//			NetworkManager.Instance.Connect (NetworkManager.Instance.playerID);
		} else {
			txtError.text = "<color=red>Username should not be empty.</color>";
		}
	}

	public bool IsLoginDetailValid ()
	{
		txtError.text = "";

		if (string.IsNullOrEmpty (ifUserName.text)) {
			txtError.text = "<color=yellow>Email should not be empty.</color>";
			return false;
		} else {
			string email = ifUserName.text;
			Regex regex = new Regex (@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
			Match match = regex.Match (email);

			if (!match.Success) {
				txtError.text = "<color=yellow>Email not valid</color>";
				return false;
			}
		}

		if (string.IsNullOrEmpty (ifPassword.text)) {
			txtError.text = "<color=yellow>Password should not be empty.</color>";
			return false;
		} else {
			if (ifPassword.text.Length < 6) {
				txtError.text = "<color=yellow>Password must contain minimum 8 characters with minimum 1 lowercase, uppercase, special and a numerical character.</color>";
				return false;
			}
		}

		return true;
	}

	public void onConnectedSuccessfully ()
	{
		UIManager.Instance.HideLoader ();
		txtError.text = "<color=green>Logged in successfully.</color>";

		goLogin.SetActive (false);

		goRoomList.SetActive (true);
		UIManager.Instance.bottomBarPanel.gameObject.SetActive (true);
		UIManager.Instance.somethingWrongPanel.gameObject.SetActive (false);
	}

	public void OnPlayerDropDownValueChanged (Dropdown dd)
	{
		if (dd.value == 0)
			ifUserName.text = "Player 1";
		else if (dd.value == 1)
			ifUserName.text = "Player 2";
		else if (dd.value == 2)
			ifUserName.text = "Player 3";
		else if (dd.value == 3)
			ifUserName.text = "Player 4";
		else if (dd.value == 4)
			ifUserName.text = "Player 5";
		else if (dd.value == 5)
			ifUserName.text = "Player 6";
		else if (dd.value == 6)
			ifUserName.text = "Player 7";
		else if (dd.value == 7)
			ifUserName.text = "Player 8";
		else if (dd.value == 8)
			ifUserName.text = "Player 9";
		else if (dd.value == 9)
			ifUserName.text = "Player 10";
	}

	public void OnTimestampButtonTap ()
	{
		ifUserName.text = System.DateTime.UtcNow.Ticks.ToString ();
	}

	public void onConnectedFail (string errorCode)
	{
		UIManager.Instance.HideLoader ();
		txtError.color = Color.red;
		txtError.text = errorCode;

		if (Application.platform == RuntimePlatform.WebGLPlayer || UIManager.Instance.isWebGLBuild) {
			UIManager.Instance.somethingWrongPanel.gameObject.SetActive (true);
		} else {
			loginMenuParent.SetActive (true);
			objLogo.SetActive (true);
		}
	}

	public void OnRegisterBtnClick ()
	{
		goLogin.SetActive (false);
		registerPanel.SetActive (true);
	}

	private IEnumerator Countdown (int time)
	{
		while (time > 0) {
			Debug.Log (time--);
			yield return new WaitForSeconds (1);
		}
		Debug.Log ("Countdown Complete!");
	}

	public void getDataFormPHP (string message)
	{
		Debug.Log (message);

		debug += "\n" + message;

		loginMenuParent.SetActive (false);

		WebGLLogin loginData = JsonUtility.FromJson<WebGLLogin> (message);
		loginDetails = loginData;
	}

	public void StartMobileGame (string message)
	{
		getDataFormPHP (message);

		loginMenuParent.SetActive (false);
		SetGameType (loginDetails.GameType);

		UIManager.Instance.isLimitGame = loginDetails.isLimit.Equals ("1");
		UIManager.Instance.isRealMoney = loginDetails.game_type.Equals ("real money");

		NetworkManager.Instance.InitializeServer (UIManager.Instance.gameType);
		UIManager.Instance.DisplayLoader ();
		NetworkManager.Instance.Connect (loginDetails.user_name);
		objLogo.SetActive (false);
//		imgBackground.gameObject.SetActive (false);
		UIManager.Instance.DisplayLoader ();
	}

	public void OnWinningRankDropdownValueChanged ()
	{
		if (dropDownWinningRank.captionText.text.Equals ("RANDOM")) {
			UIManager.Instance.isStaticHand = false;
		} else {
			UIManager.Instance.staticHand = (WINNING_RANK)System.Enum.Parse (typeof(WINNING_RANK), dropDownWinningRank.captionText.text);
			UIManager.Instance.isStaticHand = true;
		}
	}

	private void SetGameType (string GameType)
	{
		if (GameType.Equals (Constants.WGL_TABLE_GAME))
			UIManager.Instance.gameType = POKER_GAME_TYPE.TABLE;
		else if (GameType.Equals (Constants.WGL_TEXASS_GAME) || GameType.Equals (Constants.WGL_TH_SNG_TOURNAMENT) || GameType.Equals (Constants.WGL_TH_REGULAR_TOURNAMENT))
			UIManager.Instance.gameType = POKER_GAME_TYPE.TEXAS;
		else if (GameType.Equals (Constants.WGL_WHOOPASS_GAME) || GameType.Equals (Constants.WGL_WA_SNG_TOURNAMENT) || GameType.Equals (Constants.WGL_WA_REGULAR_TOURNAMENT))
			UIManager.Instance.gameType = POKER_GAME_TYPE.WHOOPASS;

		if (GameType.Equals (Constants.WGL_TH_SNG_TOURNAMENT) || GameType.Equals (Constants.WGL_WA_SNG_TOURNAMENT)) {
			UIManager.Instance.isSitNGoTournament = true;
			UIManager.Instance.isRegularTournament = false;
		} else if (GameType.Equals (Constants.WGL_TH_REGULAR_TOURNAMENT) || GameType.Equals (Constants.WGL_WA_REGULAR_TOURNAMENT)) {
			UIManager.Instance.isSitNGoTournament = false;
			UIManager.Instance.isRegularTournament = true;
		}
	}

	public void startGame ()
	{
		txtError.text = "Game Started";
		goLogin.SetActive (false);
		texasGameRoom.SetActive (true);
	}

	private void HandleOnConnectSuccessful (bool success)
	{
		Debug.LogError ("HandleOnConnectSuccessful");

		if (success) {
			if (loginDetails != null) {
				maxTimerForRoomSearch = Random.Range (2, 10);
				txtRoomSearchCounter.text = "Room Search  : " + maxTimerForRoomSearch;
//				Debug.Log ("Max time for room search  : " + maxTimerForRoomSearch);

				StartCoroutine (GetRooms ());
//				NetworkManager.Instance.GetRoomWithName (loginDetails.game_id);
			} else
				onConnectedSuccessfully ();
		} else {
			onConnectedFail ("Something went wrong. Try again.");
		}
	}

	private void HandleOnRoomListFetched (MatchedRoomsEvent matchedRoom)
	{
		Debug.LogError ("HandleOnRoomListFetched");

		if (matchedRoom.getRoomsData () != null) {
			if (matchedRoom.getRoomsData ().Length == 0) {
				if (roomSearchCounter < maxTimerForRoomSearch) {
					roomSearchCounter++;

					StartCoroutine (GetRooms ());
				} else {
					roomSearchCounter = 0;
					NetworkManager.Instance.CreateRoomWithName (loginDetails.game_id + "_" + loginDetails.TableNumber, int.Parse (loginDetails.max_player));
				}
			} else {
				Constants.GameID = matchedRoom.getRoomsData () [0].getName ();
				NetworkManager.Instance.SubscribeRoom (matchedRoom.getRoomsData () [0].getId ());
			}
		}
	}

	private IEnumerator GetRooms ()
	{
		yield return new WaitForSeconds (Random.Range (.5f, 1f));

		NetworkManager.Instance.GetRoomWithName (loginDetails.game_id);
	}

	private void HandleOnRoomCreationDone (RoomEvent roomEvent)
	{
		Debug.LogError ("HandleOnRoomCreationDone");

		if (roomEvent.getResult () == WarpResponseResultCode.SUCCESS) {
			Constants.GameID = roomEvent.getData ().getName ();
			NetworkManager.Instance.SubscribeRoom (roomEvent.getData ().getId ());
		} else if (roomEvent.getResult ().ToString ().Equals ("100")) {
			StartCoroutine (GetRooms ());
		} else {
			UIManager.Instance.somethingWrongPanel.gameObject.SetActive (false);
			NetworkManager.Instance.Disconnect ();
//			StartCoroutine (GetRooms ());
		}
	}

	private void HandleOnPlayerSubscribedRoom (RoomEvent roomEvent)
	{
		Debug.Log ("==> HandleOnPlayerSubscribedRoom  : " + roomEvent.getResult ());

		if (roomEvent.getResult () == WarpResponseResultCode.SUCCESS) {
			NetworkManager.Instance.SetPlayerCustomData ();
//			UIManager.Instance.HideLoader ();
		} else if (roomEvent.getResult ().ToString ().Equals ("100")) {
			UIManager.Instance.HideLoader ();
			if (Application.platform == RuntimePlatform.WebGLPlayer && !UIManager.Instance.isAffiliate) {
				UIManager.Instance.DisplayMessagePanel ("Room is Full", "This Game Room is Full. You may join other Game Rooms or Create Your Own Game Room.", "Lobby",
					UIManager.Instance.BackToLobbyOnWebsite);
			} else {
				UIManager.Instance.DisplayMessagePanel ("Room is Full", "This Game Room is Full. You may join other Game Rooms.", "Lobby", () => {
					UIManager.Instance.lobbyPanel.cashGamePanel.cashGameDetailPanel.gameObject.SetActive (false);
					UIManager.Instance.lobbyPanel.regularTournamentPanel.tournamentDetailPanel.gameObject.SetActive (false);
					UIManager.Instance.lobbyPanel.sngTournamentPanel.tournamentDetailPanel.gameObject.SetActive (false);
				});
			}
			NetworkManager.Instance.Disconnect ();
		} else
			UIManager.Instance.somethingWrongPanel.gameObject.SetActive (true);
	}

	private void HandleOnPlayerJoinedRoom (RoomEvent roomEvent, string playerName)
	{
		Debug.Log ("==> HandleOnPlayerJoinedRoom  : " + roomEvent.getResult ());

//		if (roomEvent.getResult () != WarpResponseResultCode.SUCCESS) {
////			NetworkManager.Instance.CreateRoomWithName (loginDetails.game_id + "_" + loginDetails.TableNumber, int.Parse (loginDetails.max_player));
//		}

		if (roomEvent.getResult () != WarpResponseResultCode.SUCCESS) {
			UIManager.Instance.HideLoader ();
			UIManager.Instance.somethingWrongPanel.gameObject.SetActive (true);
			NetworkManager.Instance.Disconnect ();
		}
	}

	private void SetWinningRankDropdown ()
	{
		List<Dropdown.OptionData> data = new List<Dropdown.OptionData> ();
		data.Add (new Dropdown.OptionData ("RANDOM"));
		foreach (WINNING_RANK rank in System.Enum.GetValues(typeof(WINNING_RANK))) {
			data.Add (new Dropdown.OptionData (rank.ToString ()));
		}

		dropDownWinningRank.AddOptions (data);
		dropDownWinningRank.value = 0;
		UIManager.Instance.isStaticHand = false;
	}

	public void OnRegularGameToggleValueChanged (Toggle t)
	{
		UIManager.Instance.isRegularTournament = UIManager.Instance.isSitNGoTournament = false;
	}

	public void OnRegularTournamentValueChanged (Toggle t)
	{
		UIManager.Instance.isRegularTournament = t.isOn;
	}

	public void OnSnGTournamentValueChanged (Toggle t)
	{
		UIManager.Instance.isSitNGoTournament = t.isOn;
	}

	public void OnTableGameToggleValueChanged (Toggle t)
	{
		UIManager.Instance.gameType = POKER_GAME_TYPE.TABLE;
	}

	public void OnTexassGameToggleValueChanged (Toggle t)
	{
		UIManager.Instance.gameType = POKER_GAME_TYPE.TEXAS;
	}

	public void OnWhoopAssGameToggleValueChanged (Toggle t)
	{
		UIManager.Instance.gameType = POKER_GAME_TYPE.WHOOPASS;
	}

	public void OnLiveServerTypeToggleValueChanged (Toggle t)
	{
		UIManager.Instance.server = SERVER.LIVE;
	}

	public void OnLocalCTypeToggleValueChanged (Toggle t)
	{
		UIManager.Instance.server = SERVER.LOCAL_C;
	}

	public void OnLocalRTypeToggleValueChanged (Toggle t)
	{
		UIManager.Instance.server = SERVER.LOCAL_R;
	}

	public void OnRegisterButtonTap ()
	{
		if ((Application.platform == RuntimePlatform.Android ||
		    Application.platform == RuntimePlatform.IPhonePlayer))
			registrationHeader.SetActive (true);

//		if (Application.platform == RuntimePlatform.WebGLPlayer && UIManager.Instance.isAffiliate)
//			WebViewManager.Instance.OpenWebviewWithURL (UIAccount.Constants.URL_PLAYER_REGISTRATION);
//		else
//			Application.OpenURL (UIAccount.Constants.URL_PLAYER_REGISTRATION);
		WebViewManager.Instance.OpenWebviewWithURL (UIAccount.Constants.URL_PLAYER_REGISTRATION, true);
	}

	public void OnWebviewBackButtonTap ()
	{
		registrationHeader.SetActive (false);
		WebViewManager.Instance.CloseWebview ();
	}
}