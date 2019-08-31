using UnityEngine;
using System.Collections;

using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using com.shephertz.app42.gaming.multiplayer.client.listener;
using com.shephertz.app42.gaming.multiplayer.client.command;
using com.shephertz.app42.gaming.multiplayer.client.message;
using com.shephertz.app42.gaming.multiplayer.client.transformer;
using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using System.Collections.Generic;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour, ConnectionRequestListener, ChatRequestListener, LobbyRequestListener, NotifyListener, RoomRequestListener, UpdateRequestListener, ZoneRequestListener, TurnBasedRoomListener
{
	#region PUBLIC_VARIABLES

	public string playerID;
	public string playerName;

	public string joinedRoomID;

	public bool debugLog = false;
	#endregion

	#region PRIVATE_VARIABLES

	private string debug;

	private const string tableGameAppKey = "7b46963f-8d16-4e15-9";
	//"5cb74c74-61f1-4cc8-a";
	private const string texassGameAppKey = "b299d465-fdeb-4c5a-8";
	//"5e8278ec-cfc7-445d-8";
	private const string whoopassGameAppKey = "4318ddad-038a-409d-8";
	//"970f84e7-bb66-4499-b";

	private const string localTableAppKey = "7b46963f-8d16-4e15-9";
	private const string localTexassAppKey = "3689654b-d64f-421e-8";
	private const string localWAAppKey = "4318ddad-038a-409d-8";

	//private const string liveIPAddress = "146.247.49.61";				//"52.25.168.232";
	 private const string liveIPAddress = "whoopasspoker.com";
	//private const string liveIPAddress = "https://whoopasspoker.com";
	//private const string liveIPAddress = "http://46.51.137.196";

	//private const string liveIPAddress = "35.177.136.119";
	public static string localIPAddressC = "192.168.74.2";//"192.168.2.154";
	private const string localIPAddressR = "192.168.74.2";

	private bool isConnected;


	/*
	 * Another live server details
	 *  52.39.171.252
		App Name : WA
		AppKey : 489c778f-e649-47b5-b

		App Name : TH
		AppKey : b7b91a31-1e7f-43b6-a

		App Name : Table
		AppKey : 5421483a-c567-4684-b
	 */

	#endregion

	public static NetworkManager Instance;

	#region DELEGATES

	public delegate void OnPlayerConnected (bool success);

	public static event OnPlayerConnected onPlayerConnected;

	public static void FirePlayerConnected (bool success)
	{
		if (onPlayerConnected != null)
			onPlayerConnected (success);
	}

	public delegate void OnRoomListFetched (MatchedRoomsEvent matchedRoomsEvent);

	public static event OnRoomListFetched onRoomListFetched;

	public static void FireRoomsAvailable (MatchedRoomsEvent matchedRoomsEvent)
	{
		if (onRoomListFetched != null) {
			onRoomListFetched (matchedRoomsEvent);
		}
	}

	public delegate void OnRoomCreationSuccess (RoomEvent roomData);

	public static event OnRoomCreationSuccess onRoomCreationDone;

	public static void FireRoomCreationSuccess (RoomEvent roomData)
	{
		if (onRoomCreationDone != null)
			onRoomCreationDone (roomData);
	}

	public delegate void OnRoomInfoReveived (LiveRoomInfoEvent roomInfo);

	public static event OnRoomInfoReveived onRoomInfoReceived;

	public static void FireRoomInfoReceived (LiveRoomInfoEvent roomInfo)
	{
		if (onRoomInfoReceived != null)
			onRoomInfoReceived (roomInfo);
	}

	public delegate void OnPlayerSubscribedRoom (RoomEvent roomEvent);

	public static event OnPlayerSubscribedRoom onPlayerSubscribedRoom;

	public static void FirePlayerSubscribedRoom (RoomEvent roomEvent)
	{
		if (onPlayerSubscribedRoom != null)
			onPlayerSubscribedRoom (roomEvent);
	}

	public delegate void OnPlayerJoinedRoom (RoomEvent roomData, string playerName);

	public static event OnPlayerJoinedRoom onPlayerJoinedRoom;

	public static void FirePlayerJoinedRoom (RoomEvent roomData, string playerName)
	{
		if (onPlayerJoinedRoom != null)
			onPlayerJoinedRoom (roomData, playerName);
	}

	public delegate void OnDefaultCardDataReceived (string sender, string cardData);

	public static event OnDefaultCardDataReceived onDefaultCardDataReceived;

	public static void FireDefaultCardDataReceived (string sender, string defaultCards)
	{
		if (onDefaultCardDataReceived != null)
			onDefaultCardDataReceived (sender, defaultCards);
	}

	public delegate void OnPlayerInfoReceived (string sender, string playerInfo);

	public static event OnPlayerInfoReceived onPlayerInfoReceived;

	public static void FirePlayerInfoReceived (string sender, string playerInfo)
	{
		if (onPlayerInfoReceived != null)
			onPlayerInfoReceived (sender, playerInfo);
	}

	public delegate void OnPlayerLeftRoom (RoomData RoomData, string playerID);

	public static event OnPlayerLeftRoom onPlayerLeftRoom;

	public static void FirePlayerLeftRoom (RoomData roomData, string playerID)
	{
		if (onPlayerLeftRoom != null)
			onPlayerLeftRoom (roomData, playerID);
	}

	public delegate void OnChatMessageReceived (string playerID, string msg);

	public static event OnChatMessageReceived onChatMessageReceived;

	public static void FireChatMessageReceived (string playerID, string msg)
	{
		if (onChatMessageReceived != null)
			onChatMessageReceived (playerID, msg);
	}

	public delegate void OnBlindPlayerResponseReceived (string sender, string blindPlayers);

	public static event OnBlindPlayerResponseReceived onBlindPlayerResponseReceived;

	public static void FireBlindPlayerResponseReceived (string sender, string blindPlayers)
	{
		if (onBlindPlayerResponseReceived != null)
			onBlindPlayerResponseReceived (sender, blindPlayers);
	}

	public delegate void OnDistributeCardResponseReceived (string sender);

	public static event OnDistributeCardResponseReceived onDistributeCardResponseReceived;

	public static void FireDistributeCards (string sender)
	{
		if (onDistributeCardResponseReceived != null)
			onDistributeCardResponseReceived (sender);
	}

	public delegate void OnGameStartedByPlayer (string sender, string gameStarter);

	public static event OnGameStartedByPlayer onGameStartedByPlayer;

	public static void FireOnGameStartedByPlayer (string sender, string gameStarter)
	{
		if (onGameStartedByPlayer != null)
			onGameStartedByPlayer (sender, gameStarter);
	}

	public delegate void OnMoveCompleterByPlayer (MoveEvent move);

	public static event OnMoveCompleterByPlayer onMoveCompletedByPlayer;

	public static void FireOnMoveCompletedByPlayer (MoveEvent move)
	{
		if (onMoveCompletedByPlayer != null)
			onMoveCompletedByPlayer (move);
	}

	public delegate void OnWinnerInfoReceived (string sender, string winnerInfo);

	public static event OnWinnerInfoReceived onWinnerInfoReceived;

	public static void FireOnWinnerInfoReceived (string sender, string winnerInfo)
	{
		if (onWinnerInfoReceived != null)
			onWinnerInfoReceived (sender, winnerInfo);
	}

	public delegate void OnRoundComplete (string sender, string roundInfo);

	public static event OnRoundComplete onRoundComplete;

	public static void FireOnRoundComplete (string sender, string roundInfo)
	{
		if (onRoundComplete != null)
			onRoundComplete (sender, roundInfo);
	}

	public delegate void OnActionResponseReceived (string sender, string actionResponse);

	public static event OnActionResponseReceived onActionResponseReceived;

	public static void FireActionResponseReceived (string sender, string actionResponse)
	{
		if (onActionResponseReceived != null)
			onActionResponseReceived (sender, actionResponse);
	}

	public delegate void OnActionHistoryReceived (string sender, string history);

	public static event OnActionHistoryReceived onActionHistoryReceived;

	public static void FireActionHistoryReceived (string sender, string history)
	{
		if (onActionHistoryReceived != null)
			onActionHistoryReceived (sender, history);
	}

	public delegate void OnRebuyResponseReceived (string sender, string rebuyInfo);

	public static event OnRebuyResponseReceived onRebuyActionResponseReceived;

	public static void FireRebuyResponseReceived (string sender, string rebuyInfo)
	{
		if (onRebuyActionResponseReceived != null)
			onRebuyActionResponseReceived (sender, rebuyInfo);
	}

	public delegate void OnResponseForRebuyReceived (string sender, string canRebuy);

	public static event OnResponseForRebuyReceived onResponseForRebuyReceived;

	public static void FireResponseForRebuyReceived (string sender, string canRebuy)
	{
		if (onResponseForRebuyReceived != null)
			onResponseForRebuyReceived (sender, canRebuy);
	}

	public delegate void OnBreakTimeResponseReceived (string sender, int breakTimer, int totalTables = 1);

	public static event OnBreakTimeResponseReceived onBreakTimeResponseReceived;

	public static void FireBreakTimeResponseReceived (string sender, int breakTimer, int totalTables = 1)
	{
		if (onBreakTimeResponseReceived != null)
			onBreakTimeResponseReceived (sender, breakTimer, totalTables);
	}

	public delegate void OnPlayerTimeoutResponseReceived ();

	public static event OnPlayerTimeoutResponseReceived onPlayerTimeoutResponseReceived;

	public static void FirePlayerTimeoutResponseReceived ()
	{
		if (onPlayerTimeoutResponseReceived != null)
			onPlayerTimeoutResponseReceived ();
	}

	public delegate void OnTournamentWinnerInfoReceived (string sender, string tournamentWinnerInfo);

	public static event OnTournamentWinnerInfoReceived onTournamentWinnerInfoReceived;

	public static void FireTournamentWinnerInfoReceived (string sender, string tournamentWinnerInfo)
	{
		if (onTournamentWinnerInfoReceived != null)
			onTournamentWinnerInfoReceived (sender, tournamentWinnerInfo);
	}

	public delegate void OnNotRegisteredInTournamentResponseReceived (string sender);

	public static event OnNotRegisteredInTournamentResponseReceived onNotRegisteredInTournamentResponseReceived;

	public static void FireNotRegisteredInTournament (string sender)
	{
		if (onNotRegisteredInTournamentResponseReceived != null)
			onNotRegisteredInTournamentResponseReceived (sender);
	}

	public delegate void OnLeaveRoomSuccess (bool success);

	public static event OnLeaveRoomSuccess onLeaveRoomSuccess;

	public static void FireOnLeaveRoomDone (bool success)
	{
		if (onLeaveRoomSuccess != null)
			onLeaveRoomSuccess (success);
	}

	public delegate void PlayerRequestedSitout (string sender);

	public static event PlayerRequestedSitout playerRequestedSitout;

	public static void FirePlayerRequestedSitout (string sender)
	{
		if (playerRequestedSitout != null)
			playerRequestedSitout (sender);
	}

	public delegate void OnTableGameStarted ();

	public static event OnTableGameStarted onTableGameStarted;

	public static void FireTableGameStarted ()
	{
		if (onTableGameStarted != null)
			onTableGameStarted ();
	}

	public delegate void PlayerRequestedBackToGame (string sender);

	public static event PlayerRequestedBackToGame playerRequestedBackToGame;

	public static void FirePlayerRequestedBackToGame (string sender)
	{
		if (playerRequestedBackToGame != null)
			playerRequestedBackToGame (sender);
	}

	public delegate void BreakingTableResponseReceived (string tableNumber);

	public static event BreakingTableResponseReceived breakingTableResponseReceived;

	public static void FireBreakingTableResponseReceived (string tableNumber)
	{
		if (breakingTableResponseReceived != null)
			breakingTableResponseReceived (tableNumber);
	}

	public delegate void PlayerEliminated (string playerID);

	public static event PlayerEliminated playerEliminated;

	public static void FirePlayerEliminated (string playerID)
	{
		if (playerEliminated != null)
			playerEliminated (playerID);
	}

	public delegate void AnteAndBlindRequestReceived (string playerID, string abInfo);

	public static event AnteAndBlindRequestReceived anteAndBlindRequestReceived;

	public static void FireAnteAndBlindRequestReceived (string playerID, string abInfo)
	{
		if (anteAndBlindRequestReceived != null)
			anteAndBlindRequestReceived (playerID, abInfo);
	}

	public delegate void OnRestartGameRequestReceived (string sender);

	public static event OnRestartGameRequestReceived onRestartGameRequestReceived;

	public static void FireRestartGameRequestReceived (string sender)
	{
		if (onRestartGameRequestReceived != null)
			onRestartGameRequestReceived (sender);
	}

	public delegate void AddChipsResponseReceived (string sender, string addChipsInfo);

	public static event AddChipsResponseReceived addChipsResponseReceived;

	public static void FireAddChipsResponseReceived (string sender, string addChipsInfo)
	{
		if (addChipsResponseReceived != null)
			addChipsResponseReceived (sender, addChipsInfo);
	}

	public delegate void MaxSitoutResponseReceived (string sender, string playerID);

	public static event MaxSitoutResponseReceived maxSitoutResponseReceived;

	public static void FireMaxSitoutResponseReceived (string sender, string playerID)
	{
		if (maxSitoutResponseReceived != null)
			maxSitoutResponseReceived (sender, playerID);
	}

	public delegate void RebuyInTournamentResponseReceived (string sender);

	public static event RebuyInTournamentResponseReceived rebuyInTournamentResponseReceived;

	public static void FireRebuyInTournamentResponseReceived (string sender)
	{
		if (rebuyInTournamentResponseReceived != null)
			rebuyInTournamentResponseReceived (sender);
	}

	public delegate void CollectBlindOnBackToGame (string playerID, double amount);

	public static event CollectBlindOnBackToGame collectBlindOnBackToGame;

	public static void FireCollectBlindOnBackToGame (string playerID, double amount)
	{
		if (collectBlindOnBackToGame != null)
			collectBlindOnBackToGame (playerID, amount);
	}

	public delegate void WaitForGameFinishOnOtherTable (string sender);

	public static event WaitForGameFinishOnOtherTable waitForGameFinishOnOtherTable;

	public static void FireWaitForGameFinishOnOtherTable (string sender)
	{
		if (waitForGameFinishOnOtherTable != null)
			waitForGameFinishOnOtherTable (sender);
	}

	#endregion

	#region UNITY_CALLBACKS

	// Use this for initialization
	void Awake ()
	{
		Instance = this;
	}

	void Start ()
	{ 
		Application.runInBackground = true;
	}

	public void InitializeServer (POKER_GAME_TYPE gt)
	{
		string ip = "";
		if (UIManager.Instance.server == SERVER.LIVE)
			ip = liveIPAddress;
		else
			ip = UIManager.Instance.server == SERVER.LOCAL_C ? localIPAddressC : localIPAddressR;

		int port = 12346;
		#if UNITY_WEBGL
			port = 12347;
		#endif
		string appKey = "";
		Debug.Log (ip + " >> Port > " + port + " >> Game Type : " + gt);
		if (gt == POKER_GAME_TYPE.TABLE) {
			appKey = UIManager.Instance.server == SERVER.LIVE ? tableGameAppKey : localTableAppKey;
			Debug.Log ("Table game selected............." + appKey + " >> ip : " + ip + " >> port : " + port);
			WarpClient.initialize (appKey, ip, port);
		} else if (gt == POKER_GAME_TYPE.TEXAS) {
			appKey = UIManager.Instance.server == SERVER.LIVE ? texassGameAppKey : localTexassAppKey;
			Debug.Log ("Texas game selected............." + appKey + " >> ip : " + ip + " >> port : " + port);
			WarpClient.initialize (appKey, ip, port);
		} else {
			appKey = UIManager.Instance.server == SERVER.LIVE ? whoopassGameAppKey : localWAAppKey;
			Debug.Log ("WHPoker game selected............." + appKey + " >> ip : " + ip + " >> port : " + port);
			WarpClient.initialize (appKey, ip, port);
		}

		#if UNITY_WEBGL
		WarpClient.GetInstance ().enableSSL (true);
		#endif
		WarpClient.GetInstance ().SetAppKey (appKey);

		WarpClient.setRecoveryAllowance (Constants.RECONNECTING_TIMEOUT);

		WarpClient.GetInstance ().RemoveConnectionRequestListener (this);
		WarpClient.GetInstance ().RemoveChatRequestListener (this);
		WarpClient.GetInstance ().RemoveLobbyRequestListener (this);
		WarpClient.GetInstance ().RemoveNotificationListener (this);
		WarpClient.GetInstance ().RemoveRoomRequestListener (this);
		WarpClient.GetInstance ().RemoveUpdateRequestListener (this);
		WarpClient.GetInstance ().RemoveZoneRequestListener (this);
		WarpClient.GetInstance ().RemoveTurnBasedRoomRequestListener (this);

		WarpClient.GetInstance ().AddConnectionRequestListener (this);
		WarpClient.GetInstance ().AddChatRequestListener (this);
		WarpClient.GetInstance ().AddLobbyRequestListener (this);
		WarpClient.GetInstance ().AddNotificationListener (this);
		WarpClient.GetInstance ().AddRoomRequestListener (this);
		WarpClient.GetInstance ().AddUpdateRequestListener (this);
		WarpClient.GetInstance ().AddZoneRequestListener (this);
		WarpClient.GetInstance ().AddTurnBasedRoomRequestListener (this);
	}

	void Update ()
	{
		if (isConnected)
			WarpClient.GetInstance ().Update ();

		if (!Application.runInBackground) {
			Application.runInBackground = true;
		}
	
	}

	void OnGUI ()
	{
		if (!debugLog)
			return;

		if (GUI.Button (new Rect (10, 10, 50, 25), "Clear")) {
			debug = "";
		}

		GUI.contentColor = Color.white;
		GUI.Label (new Rect (10, 35, Screen.width, Screen.height), debug);
	}

	void OnDestroy ()
	{
		Disconnect ();
	}

	#endregion

	private void Log (string msg)
	{
		debug = msg + "\n" + debug;
		DebugLog.Log (msg);
//		DebugLog.Log ("-> " + msg);
	}

	#region PUBLIC_METHODS

	public void Connect (string playerID)
	{
		this.playerID = playerID;
		WarpClient.GetInstance ().Connect (playerID, "");
//		WarpClient.GetInstance ().RecoverConnection ();
	}

	public void Disconnect ()
	{
		if (isConnected)
			WarpClient.GetInstance ().Disconnect ();
	}

	public void GetRooms ()
	{
		Dictionary<string, object> prop = new Dictionary<string, object> ();
		prop.Add (Constants.ROOM_PROP_ROOM_NAME, LoginScript.loginDetails.game_id);
		prop.Add (Constants.ROOM_PROP_TABLE_NUMBER, LoginScript.loginDetails.TableNumber);

		WarpClient.GetInstance ().GetRoomWithProperties (prop);
	}

	public void GetRoomWithName (string roomName)
	{
		Dictionary<string, object> dict = new Dictionary<string, object> ();
		dict.Add (Constants.ROOM_PROP_ROOM_NAME, roomName);

//        DebugLog.Log(Constants.REMOVE_BEFORE_LIVE);
		dict.Add (Constants.ROOM_PROP_GAME_TYPE, UIManager.Instance.gameType);
		dict.Add (Constants.ROOM_PROP_TABLE_NUMBER, LoginScript.loginDetails.TableNumber);

		///
//		DebugLog.Log(Constants.REMOVE_BEFORE_LIVE);
//		if (!dict.ContainsKey (Constants.ROOM_PROP_GAME_ROOM_ID))
//			dict.Add (Constants.ROOM_PROP_GAME_ROOM_ID, "0");
//		if (!dict.ContainsKey (Constants.ROOM_PROP_TABLE_NUMBER))
//			dict.Add (Constants.ROOM_PROP_TABLE_NUMBER, "1");

		if (UIManager.Instance.isRegularTournament)
			dict.Add (Constants.ROOM_PROP_TOURNAMENT, (int)TOURNAMENT_GAME_TYPE.REGULAR_TOURNAMENT);
		else if (UIManager.Instance.isSitNGoTournament)
			dict.Add (Constants.ROOM_PROP_TOURNAMENT, (int)TOURNAMENT_GAME_TYPE.SIT_N_GO_TOURNAMENT);
		else
			dict.Add (Constants.ROOM_PROP_TOURNAMENT, (int)TOURNAMENT_GAME_TYPE.REGULAR);

		Log ("GetRoomWithName  : " + roomName);

		WarpClient.GetInstance ().GetRoomWithProperties (dict);
	}

	public void CreateRoom (string roomName, int maxPlayers, Dictionary<string, object> dict)
	{
		DebugLog.Log (Constants.REMOVE_BEFORE_LIVE);
		dict.Add (Constants.ROOM_PROP_GAME_TYPE, UIManager.Instance.gameType);

		DebugLog.Log (Constants.REMOVE_BEFORE_LIVE);

		if (dict.ContainsKey (Constants.ROOM_PROP_TABLE_NUMBER)) {
			if (dict [Constants.ROOM_PROP_TABLE_NUMBER].Equals ("0"))
				dict [Constants.ROOM_PROP_TABLE_NUMBER] = "1";
		} else
			dict.Add (Constants.ROOM_PROP_TABLE_NUMBER, "1");

		if (!dict.ContainsKey (Constants.ROOM_PROP_GAME_ROOM_ID)) {
			dict.Add (Constants.ROOM_PROP_GAME_ROOM_ID, "0");
		}

		if (UIManager.Instance.isStaticHand) {
			dict.Add (Constants.ROOM_PROP_STATIC_HAND, UIManager.Instance.staticHand.ToString ());
		} else {
			dict.Add (Constants.ROOM_PROP_STATIC_HAND, "NONE");
		}

		List<string> keyList = new List<string> (dict.Keys);
		foreach (string k in keyList) {
			DebugLog.Log (k + " --> " + dict [k]);
		}

		if (UIManager.Instance.isRegularTournament)
			dict.Add (Constants.ROOM_PROP_TOURNAMENT, (int)TOURNAMENT_GAME_TYPE.REGULAR_TOURNAMENT);
		else if (UIManager.Instance.isSitNGoTournament)
			dict.Add (Constants.ROOM_PROP_TOURNAMENT, (int)TOURNAMENT_GAME_TYPE.SIT_N_GO_TOURNAMENT);
		else
			dict.Add (Constants.ROOM_PROP_TOURNAMENT, (int)TOURNAMENT_GAME_TYPE.REGULAR);

		WarpClient.GetInstance ().CreateTurnRoom (roomName, playerID, maxPlayers, dict, Constants.TURN_TIME);
	}

	public void CreateRoomWithName (string name, int maxPlayers)
	{
		Dictionary<string, object> dict = new Dictionary<string, object> ();
		dict.Add ("room_name", "room_name");
		dict.Add (Constants.ROOM_PROP_ROOM_NAME, LoginScript.loginDetails.game_id);
		dict.Add ("MINCHIPS", LoginScript.loginDetails.buyin);
		dict.Add (Constants.ROOM_PROP_GAME_ROOM_ID, LoginScript.loginDetails.GameRoomID);
		dict.Add (Constants.ROOM_PROP_TABLE_NUMBER, LoginScript.loginDetails.TableNumber);

		DebugLog.Log (Constants.REMOVE_BEFORE_LIVE);
		dict.Add (Constants.ROOM_PROP_GAME_TYPE, UIManager.Instance.gameType);

		if (UIManager.Instance.isRegularTournament)
			dict.Add (Constants.ROOM_PROP_TOURNAMENT, (int)TOURNAMENT_GAME_TYPE.REGULAR_TOURNAMENT);
		else if (UIManager.Instance.isSitNGoTournament)
			dict.Add (Constants.ROOM_PROP_TOURNAMENT, (int)TOURNAMENT_GAME_TYPE.SIT_N_GO_TOURNAMENT);
		else
			dict.Add (Constants.ROOM_PROP_TOURNAMENT, (int)TOURNAMENT_GAME_TYPE.REGULAR);

		if (UIManager.Instance.isStaticHand) {
			dict.Add (Constants.ROOM_PROP_STATIC_HAND, UIManager.Instance.staticHand.ToString ());
		}

		List<string> keyList = new List<string> (dict.Keys);
		foreach (string k in keyList) {
			DebugLog.Log (k + " --> " + dict [k]);
		}
//		DebugLog.Log ("Max players --> " + maxPlayers);

		WarpClient.GetInstance ().CreateTurnRoom (LoginScript.loginDetails.game_id, LoginScript.loginDetails.game_id, maxPlayers, dict, Constants.TURN_TIME);
	}

	public void DeleteRoom (string roomID)
	{
		WarpClient.GetInstance ().DeleteRoom (roomID);
	}

	public void GetRoomInfo (string roomID)
	{
		WarpClient.GetInstance ().GetLiveRoomInfo (roomID);
	}

	public void SubscribeRoom (string roomID)
	{
		WarpClient.GetInstance ().SubscribeRoom (roomID);
	}

	public void UnsubscribeRoom (string roomID)
	{
		WarpClient.GetInstance ().UnsubscribeRoom (roomID);
	}

	public void SetPlayerCustomData ()
	{
		//		GD_PlayerDetail player = null;
		//
		//		for (int i = 0; i < AndroidNative.Instance.allPlayersDetail.Count; i++) {
		//			if (AndroidNative.Instance.allPlayersDetail [i].id.Equals (playerID))
		//				player = AndroidNative.Instance.allPlayersDetail [i];
		//		}
		//		string jsonString = JsonUtility.ToJson (player);
		//		DebugLog.Log (Constants.REMOVE_BEFORE_LIVE);
		string jsonString = "{\"id\":\"" + playerID + "}";// "\",\"name\":\"test97\",\"player_status\":\"accept\",\"number\":\"919876543210\",\"profile_pic\":\"http://pokernew.aistechnolabs.us/admin/public/uploads/919876543210.png\",\"player_chip_amt\":\"2222\"}";
		WarpClient.GetInstance ().SetCustomUserData (playerID, jsonString);
	}

	public void JoinRoom (string roomID)
	{
		WarpClient.GetInstance ().JoinRoom (roomID);
	}

	public void GetPlayerInfo (string playerID)
	{
		WarpClient.GetInstance ().GetLiveUserInfo (playerID);
	}

	public void SendChatMessage (string chatMessage)
	{
		WarpClient.GetInstance ().SendChat (Constants.RESPONSE_FOR_CHAT + chatMessage);
	}

	public void SendGameStartRequest ()
	{
		WarpClient.GetInstance ().SendChat (Constants.RESPONSE_FOR_DISTRIBUTE_CARD);
	}

	public void SendPlayerAction (PlayerAction action)
	{
		if (UIManager.Instance.gameType == POKER_GAME_TYPE.TABLE)
			GameManager.Instance.timeoutCounter = 0;
		else if (UIManager.Instance.gameType == POKER_GAME_TYPE.TEXAS)
			TexassGame.Instance.timeoutCounter = 0;
		else if (UIManager.Instance.gameType == POKER_GAME_TYPE.WHOOPASS)
			WhoopAssGame.Instance.timeoutCounter = 0;

		if (action.Bet_Amount < 0)
			action.Bet_Amount = 0;

		action.Bet_Amount = System.Math.Round (action.Bet_Amount, 2);
		string act = JsonUtility.ToJson (action);
		Debug.Log ("ACTION --->  " + act);
		WarpClient.GetInstance ().sendMove (Constants.REQUEST_FOR_ACTION + act);
	}

	public void SendRequestToServer (string request)
	{
		WarpClient.GetInstance ().SendChat (request);
	}

	public void StopGame ()
	{
		WarpClient.GetInstance ().stopGame ();
	}

	public void LeaveGame ()
	{
		WarpClient.GetInstance ().UnsubscribeRoom (joinedRoomID);
		WarpClient.GetInstance ().LeaveRoom (joinedRoomID);
	}

	#endregion

	#region APPWARP_LISTENER

	//ConnectionRequestListener
	public void onConnectDone (ConnectEvent eventObj)
	{		
		DebugLog.Log ("onConnectDone >> " + eventObj.getResult ());

		if (eventObj.getResult () == WarpResponseResultCode.SUCCESS) {
			isConnected = true;
//			Debug.Log ("=======================>> " + WarpClient.GetInstance ().in);
			FirePlayerConnected (true);
		} else if (eventObj.getResult () == WarpResponseResultCode.CONNECTION_ERROR_RECOVERABLE) {
			UIManager.Instance.DisplayReconnectingPanel ();
			StopCoroutine ("TryToReconnect");
			StartCoroutine ("TryToReconnect");
		} else if (eventObj.getResult () == WarpResponseResultCode.SUCCESS_RECOVERED) {
			StopCoroutine ("TryToReconnect");
			UIManager.Instance.HideReconnectingPanel ();
		} else {
			StopCoroutine ("TryToReconnect");
			isConnected = false;
			UIManager.Instance.HideReconnectingPanel ();
			FirePlayerConnected (false);
		}
	}

	public void onInitUDPDone (byte res)
	{
		
	}

	public void onLog (string message)
	{
		Log (message);
	}

	public void onDisconnectDone (ConnectEvent eventObj)
	{
		Log ("onDisconnectDone : " + eventObj.getResult ());
	}

	//LobbyRequestListener
	public void onJoinLobbyDone (LobbyEvent eventObj)
	{
		Log ("onJoinLobbyDone : " + eventObj.getResult ());
		if (eventObj.getResult () == 0) {

		}
	}

	public void onLeaveLobbyDone (LobbyEvent eventObj)
	{
		Log ("onLeaveLobbyDone : " + eventObj.getResult ());
	}

	public void onSubscribeLobbyDone (LobbyEvent eventObj)
	{
		Log ("onSubscribeLobbyDone : " + eventObj.getResult ());
		if (eventObj.getResult () == 0) {
			WarpClient.GetInstance ().JoinLobby ();
		}
	}

	public void onUnSubscribeLobbyDone (LobbyEvent eventObj)
	{
		Log ("onUnSubscribeLobbyDone : " + eventObj.getResult ());
	}

	public void onGetLiveLobbyInfoDone (LiveRoomInfoEvent eventObj)
	{
		Log ("onGetLiveLobbyInfoDone : " + eventObj.getResult ());
	}

	//ZoneRequestListener
	public void onDeleteRoomDone (RoomEvent eventObj)
	{
		Log ("onDeleteRoomDone : " + eventObj.getResult ());
	}

	public void onGetAllRoomsDone (AllRoomsEvent eventObj)
	{
		Log ("ongetallroomsdone : " + eventObj.getResult ());
		if (eventObj.getResult () == WarpResponseResultCode.SUCCESS) {
//			if (eventObj.getRoomIds ().Length > 0) {
//				foreach (string roomId in eventObj.getRoomIds()) {
//					WarpClient.GetInstance ().GetLiveRoomInfo (roomId);
//				}
//				//WarpClient.GetInstance().SubscribeRoom("1170889653");
//			}
		}
	}

	public void onCreateRoomDone (RoomEvent eventObj)
	{
		Log ("onCreateRoomDone : " + eventObj.getResult ());

//		string roomId = "";
		FireRoomCreationSuccess (eventObj);
		if (eventObj.getResult () == WarpResponseResultCode.SUCCESS) {
//			roomId = eventObj.getData ().getId ();
//			WarpClient.GetInstance ().GetLiveRoomInfo (roomId);
		}
	}

	public void onGetOnlineUsersDone (AllUsersEvent eventObj)
	{
		Log ("onGetOnlineUsersDone : " + eventObj.getResult ());
	}

	public void onGetLiveUserInfoDone (LiveUserInfoEvent eventObj)
	{
		Log ("onGetLiveUserInfoDone : " + eventObj.getResult ());
	}

	public void onSetCustomUserDataDone (LiveUserInfoEvent eventObj)
	{
		Log ("onSetCustomUserDataDone : " + eventObj.getResult ());

		if (eventObj.getResult () == WarpResponseResultCode.SUCCESS) {
//			UIManager.Instance.loginPanel.gameObject.SetActive (false);
			UIManager.Instance.HideLoader ();

			if (UIManager.Instance.gameType == POKER_GAME_TYPE.TABLE)
				UIManager.Instance.gamePlayPanel.gameObject.SetActive (true);
			else if (UIManager.Instance.gameType == POKER_GAME_TYPE.TEXAS)
				UIManager.Instance.texassGamePanel.gameObject.SetActive (true);
			else if (UIManager.Instance.gameType == POKER_GAME_TYPE.WHOOPASS)
				UIManager.Instance.whoopAssGamePanel.gameObject.SetActive (true);

			UIManager.Instance.loginPanel.gameObject.SetActive (false);
			UIManager.Instance.bottomBarPanel.gameObject.SetActive (false);
			UIManager.Instance.breakingTablePanel.gameObject.SetActive (false);
			UIManager.Instance.roomsPanel.gameObject.SetActive (false);
			UIManager.Instance.lobbyPanel.gameObject.SetActive (false);

//			UIManager.Instance.gamePanel.gameObject.SetActive (true);
		}
	}

	public void onGetMatchedRoomsDone (MatchedRoomsEvent eventObj)
	{
		if (eventObj.getResult () == WarpResponseResultCode.SUCCESS) {
//			Log ("GetMatchedRooms event received with success status");
			FireRoomsAvailable (eventObj);

//			foreach (var roomData in eventObj.getRoomsData()) {
//				Log ("Room ID:" + roomData.getId ());
//			}
		}
	}
	//RoomRequestListener
	public void onSubscribeRoomDone (RoomEvent eventObj)
	{
		Log ("onSubscribeRoomDone : " + eventObj.getResult ());
//		string roomid = "";
		FirePlayerSubscribedRoom (eventObj);
		if (eventObj.getResult () == WarpResponseResultCode.SUCCESS) {
			joinedRoomID = eventObj.getData ().getId ();

//			roomid = eventObj.getData ().getId ();
			//				WarpClient.GetInstance().startGame();
			//				string json = "{\"start\":\""+roomid+"\"}";
			//				WarpClient.GetInstance().SendChat(json);
			//WarpClient.GetInstance().JoinRoom("1170889653");
		}
	}

	public void onUnSubscribeRoomDone (RoomEvent eventObj)
	{
		Log ("onUnSubscribeRoomDone : " + eventObj.getResult ());
	}

	public void onJoinRoomDone (RoomEvent eventObj)
	{
		Log ("==> On Join room done : " + eventObj.getResult ());
		FirePlayerJoinedRoom (eventObj, playerID);

//		if (eventObj.getResult () == WarpResponseResultCode.SUCCESS) {
//			joinedRoomID = eventObj.getData ().getId ();
//		}

		if (eventObj.getResult () == WarpResponseResultCode.SUCCESS)
			WarpClient.GetInstance ().GetLiveRoomInfo (eventObj.getData ().getId ());

		//WarpClient.GetInstance ().sendMove("Move by Me >> ");
	}

	public void onLockPropertiesDone (byte result)
	{
		Log ("onLockPropertiesDone : " + result);
	}

	public void onUnlockPropertiesDone (byte result)
	{
		Log ("onUnlockPropertiesDone : " + result);
	}

	public void onLeaveRoomDone (RoomEvent eventObj)
	{
		Log ("onLeaveRoomDone : " + eventObj.getResult ());
	}

	public void onGetLiveRoomInfoDone (LiveRoomInfoEvent liveRoomInfoEvent)
	{
		if (liveRoomInfoEvent.getResult () == WarpResponseResultCode.SUCCESS) {
			FireRoomInfoReceived (liveRoomInfoEvent);
//			DebugLog.LogWarning (liveRoomInfoEvent.getProperties()[Constants.TURN_TIME_ROOM_PROPERTY]);
		}
	}

	public void onSetCustomRoomDataDone (LiveRoomInfoEvent eventObj)
	{
		DebugLog.Log ("onSetCustomRoomDataDone : " + eventObj.getResult ());
	}

	public void onUpdatePropertyDone (LiveRoomInfoEvent eventObj)
	{
		if (WarpResponseResultCode.SUCCESS == eventObj.getResult ()) {
			DebugLog.Log ("UpdateProperty event received with success status");
		} else {
			DebugLog.Log ("Update Propert event received with fail status. Status is :" + eventObj.getResult ().ToString ());
		}
	}

	//ChatRequestListener
	public void onSendChatDone (byte result)
	{
		DebugLog.Log ("onSendChatDone result : " + result);
	}

	public void onSendPrivateChatDone (byte result)
	{
		DebugLog.Log ("onSendPrivateChatDone : " + result);
	}

	//UpdateRequestListener
	public void onSendUpdateDone (byte result)
	{
	}

	//NotifyListener
	public void onRoomCreated (RoomData eventObj)
	{
		DebugLog.Log ("onRoomCreated");

//		FireRoomCreationSuccess (eventObj);
	}

	public void onRoomDestroyed (RoomData eventObj)
	{
		DebugLog.Log ("onRoomDestroyed");
	}

	public void onUserLeftRoom (RoomData eventObj, string username)
	{
		Log ("onUserLeftRoom : " + username);

		FirePlayerLeftRoom (eventObj, username);
	}

	public void onUserJoinedRoom (RoomData eventObj, string username)
	{
		Log ("onUserJoinedRoom : " + username + " >> " + eventObj.getName ());

//		if (username == GameManager.Instance.ownPlayerID) {
//			joinedRoomID = eventObj.getId ();
//			FirePlayerJoinedRoom (eventObj, username);
//		}	
	}

	public void onUserLeftLobby (LobbyData eventObj, string username)
	{
		DebugLog.Log ("onUserLeftLobby : " + username);
	}

	public void onUserJoinedLobby (LobbyData eventObj, string username)
	{
		DebugLog.Log ("onUserJoinedLobby : " + username);
	}

	public void onUserChangeRoomProperty (RoomData roomData, string sender, Dictionary<string, object> properties, Dictionary<string, string> lockedPropertiesTable)
	{
		DebugLog.Log ("onUserChangeRoomProperty : " + sender);
	}

	public void onPrivateChatReceived (string sender, string message)
	{
		DebugLog.Log ("onPrivateChatReceived : " + sender);
	}

	public void onMoveCompleted (MoveEvent move)
	{
		Log ("Move Completed by : " + move.getSender () + "Move data  : " + move.getMoveData ());
		
		FireOnMoveCompletedByPlayer (move);
	}

	public void onChatReceived (ChatEvent eventObj)
	{
		string msg = eventObj.getMessage ();
		string sender = eventObj.getSender ();

		Log ("Chat Received : " + msg + " \n Sender : " + sender);

		if (msg.StartsWith (Constants.RESPONSE_FOR_DEFAULT_CARDS)) {
			msg = msg.Replace (Constants.RESPONSE_FOR_DEFAULT_CARDS, "");
			FireDefaultCardDataReceived (sender, msg);
		} else if (msg.StartsWith (Constants.RESPONSE_FOR_PLAYER_INFO)) {
			msg = msg.Replace (Constants.RESPONSE_FOR_PLAYER_INFO, "");
			FirePlayerInfoReceived (sender, msg);
		} else if (msg.StartsWith (Constants.RESPONSE_FOR_ACTION_DONE)) {
			msg = msg.Replace (Constants.RESPONSE_FOR_ACTION_DONE, "");
			FireActionResponseReceived (sender, msg);
		} else if (msg.StartsWith (Constants.RESPONSE_FOR_BLIND_PLAYER)) {
			msg = msg.Replace (Constants.RESPONSE_FOR_BLIND_PLAYER, "");
			FireBlindPlayerResponseReceived (sender, msg);
		} else if (msg.StartsWith (Constants.RESPONSE_FOR_DISTRIBUTE_CARD)) {
			msg = msg.Replace (Constants.RESPONSE_FOR_DISTRIBUTE_CARD, "");
			FireDistributeCards (sender);
		} else if (msg.StartsWith (Constants.RESPONSE_FOR_GAME_COMPLETE)) {
			msg = msg.Replace (Constants.RESPONSE_FOR_GAME_COMPLETE, "");

			if (UIManager.Instance.gameType == POKER_GAME_TYPE.TABLE) {
				FireOnWinnerInfoReceived (Constants.TABLEGAME_SERVER_NAME, null);
			}
		} else if (msg.StartsWith (Constants.RESPONSE_FOR_GAME_START)) {
			msg = msg.Replace (Constants.RESPONSE_FOR_GAME_START, "");
			FireTableGameStarted ();
		} else if (msg.StartsWith (Constants.RESPONSE_FOR_ROUND_COMPLETE)) {
			msg = msg.Replace (Constants.RESPONSE_FOR_ROUND_COMPLETE, "");
			FireOnRoundComplete (sender, msg);
		} else if (msg.StartsWith (Constants.RESPONSE_FOR_WINNER_INFO)) {
			msg = msg.Replace (Constants.RESPONSE_FOR_WINNER_INFO, "");
			FireOnWinnerInfoReceived (sender, msg);
		} else if (msg.StartsWith (Constants.RESPONSE_FOR_CHAT)) {
			msg = msg.Replace (Constants.RESPONSE_FOR_CHAT, "");
			FireChatMessageReceived (sender, msg);
		} else if (msg.StartsWith (Constants.RESPONSE_FOR_HISTORY)) {
			msg = msg.Replace (Constants.RESPONSE_FOR_HISTORY, "");
			FireActionHistoryReceived (sender, msg);
		} else if (msg.StartsWith (Constants.REQUEST_FOR_REBUY)) {
			msg = msg.Replace (Constants.REQUEST_FOR_REBUY, "");
			FireRebuyResponseReceived (sender, msg);
		} else if (msg.StartsWith (Constants.RESPONSE_FOR_REBUY)) {
			msg = msg.Replace (Constants.RESPONSE_FOR_REBUY, "");
			FireResponseForRebuyReceived (sender, msg);
		} else if (msg.StartsWith (Constants.RESPONSE_FOR_BREAK_STATUS)) {
			msg = msg.Replace (Constants.RESPONSE_FOR_BREAK_STATUS, "");

			string[] arr = msg.Split ('#');

			int breakTime = 0;
			int totalTables = 1;

			if (arr.Length > 1) {
				breakTime = int.Parse (arr [0]);
				totalTables = int.Parse (arr [1]);
			} else {
				breakTime = int.Parse (msg);
			}

			FireBreakTimeResponseReceived (sender, breakTime, totalTables);
		} else if (msg.StartsWith (Constants.RESPONSE_FOR_PLAYER_TIMEOUT)) {
			FirePlayerTimeoutResponseReceived ();
		} else if (msg.StartsWith (Constants.RESPONSE_FOR_TOURNAMENT_WINNER_INFO)) {
			msg = msg.Replace (Constants.RESPONSE_FOR_TOURNAMENT_WINNER_INFO, "");
			FireTournamentWinnerInfoReceived (sender, msg);
		} else if (msg.StartsWith (Constants.RESPONSE_FOR_NOT_REGISTERED_IN_TOURNAMENT)) {
			msg = msg.Replace (Constants.RESPONSE_FOR_NOT_REGISTERED_IN_TOURNAMENT, "");
			FireNotRegisteredInTournament (sender);
		} else if (msg.StartsWith (Constants.REQUEST_FOR_SIT_OUT)) {
			msg = msg.Replace (Constants.REQUEST_FOR_SIT_OUT, "");
			FirePlayerRequestedSitout (msg);
		} else if (msg.StartsWith (Constants.REQUEST_FOR_BACK_TO_GAME)) {
			msg = msg.Replace (Constants.REQUEST_FOR_BACK_TO_GAME, "");
			FirePlayerRequestedBackToGame (msg);
		} else if (msg.StartsWith (Constants.RESPONSE_FOR_BREAK_TABLE)) {
			msg = msg.Replace (Constants.RESPONSE_FOR_BREAK_TABLE, "");

			if (UIManager.Instance.gameType == POKER_GAME_TYPE.TEXAS) {
				if (TexassGame.Instance.currentGameStatus == GAME_STATUS.RUNNING) {
					UIManager.Instance.breakingTablePanel.breakingTableNumber = msg;
					UIManager.Instance.breakingTablePanel.willBreakTable = true;
				} else {
					UIManager.Instance.texassGamePanel.gameObject.SetActive (false);
					UIManager.Instance.whoopAssGamePanel.gameObject.SetActive (false);
					UIManager.Instance.breakingTablePanel.breakingTableNumber = msg;
					UIManager.Instance.breakingTablePanel.willBreakTable = true;
					UIManager.Instance.breakingTablePanel.gameObject.SetActive (true);
				}
			} else if (UIManager.Instance.gameType == POKER_GAME_TYPE.WHOOPASS) {
				if (WhoopAssGame.Instance.currentGameStatus == GAME_STATUS.RUNNING) {
					UIManager.Instance.breakingTablePanel.breakingTableNumber = msg;
					UIManager.Instance.breakingTablePanel.willBreakTable = true;
				} else {
					UIManager.Instance.texassGamePanel.gameObject.SetActive (false);
					UIManager.Instance.whoopAssGamePanel.gameObject.SetActive (false);
					UIManager.Instance.breakingTablePanel.breakingTableNumber = msg;
					UIManager.Instance.breakingTablePanel.willBreakTable = true;
					UIManager.Instance.breakingTablePanel.gameObject.SetActive (true);
				}
			}

			LoginScript.loginDetails.TableNumber = msg;
		} else if (msg.StartsWith (Constants.RESPONSE_FOR_PLAYER_ELIMINATE)) {
			msg = msg.Replace (Constants.RESPONSE_FOR_PLAYER_ELIMINATE, "");
			FirePlayerEliminated (msg);
		} else if (msg.StartsWith (Constants.REQUEST_FOR_ANTE_AND_BLIND)) {
			msg = msg.Replace (Constants.REQUEST_FOR_ANTE_AND_BLIND, "");
			FireAnteAndBlindRequestReceived (sender, msg);
		} else if (msg.StartsWith (Constants.REQUEST_FOR_RESTART_GAME)) {
			msg = msg.Replace (Constants.REQUEST_FOR_RESTART_GAME, "");
			FireRestartGameRequestReceived (sender);
		} else if (msg.StartsWith (Constants.REQUEST_FOR_ADD_CHIPS)) {
			msg = msg.Replace (Constants.REQUEST_FOR_ADD_CHIPS, "");
			FireAddChipsResponseReceived (sender, msg);
		} else if (msg.StartsWith (Constants.REQUEST_FOR_ELIMINATION_FROM_TOURNAMENT)) {
			msg = msg.Replace (Constants.REQUEST_FOR_ELIMINATION_FROM_TOURNAMENT, "");
			FireMaxSitoutResponseReceived (sender, msg);
		} else if (msg.StartsWith (Constants.RESPONSE_FOR_REBUY_ON_ELIMINATED_IN_TOURNAMENT)) {
			msg = msg.Replace (Constants.RESPONSE_FOR_REBUY_ON_ELIMINATED_IN_TOURNAMENT, "");
			FireRebuyInTournamentResponseReceived (sender);
		} else if (msg.StartsWith (Constants.REQUEST_PLAYER_BACK_TO_GAME_COLLECT_BLIND)) {
			msg = msg.Replace (Constants.REQUEST_PLAYER_BACK_TO_GAME_COLLECT_BLIND, "");
			FireCollectBlindOnBackToGame (eventObj.getSender (), double.Parse (msg));
		}
	}

	public void onUpdatePeersReceived (UpdateEvent eventObj)
	{
		Log ("onUpdatePeersReceived");

		Log ("isUDP " + eventObj.getIsUdp ());
	}

	public void onUserChangeRoomProperty (RoomData roomData, string sender, Dictionary<string, System.Object> properties)
	{
		Log ("Notification for User Changed Room Property received");
		Log (roomData.getId ());
		Log (sender);
//		foreach (KeyValuePair<String, System.Object> entry in properties) {
//			LogMessage.Log ("KEY:" + entry.Key);
//			LogMessage.Log ("VALUE:" + entry.Value.ToString ());
//		}
	}

	public void onUserPaused (string a, bool b, string c)
	{
	}

	public void onUserResumed (string a, bool b, string c)
	{
	}

	public void onGameStarted (string sender, string roomId, string nextTurn)
	{
		//		Log ("sender  : " + sender + "\tnext turn  : " + nextTurn);
//		DebugLog.Log ("sender  : " + sender + "\tnext turn  : " + nextTurn);

		FireOnGameStartedByPlayer (sender, nextTurn);
	}

	public void onGameStopped (string a, string b)
	{
//		Log ("game stopped -- a  : " + a);
//		Log ("game stopped -- b  : " + b);

		if (UIManager.Instance.gameType == POKER_GAME_TYPE.TABLE) {
			if (GameManager.Instance.ownTablePlayer.playerInfo.Player_Status == (int)PLAYER_STATUS.WAITING)
				FireOnWinnerInfoReceived (Constants.TABLEGAME_SERVER_NAME, null);
		}
	}

	public void onInvokeZoneRPCDone (RPCEvent evnt)
	{
	}

	public void onInvokeRoomRPCDone (RPCEvent evnt)
	{
	}

	public void sendBytes (byte[] msg, bool useUDP)
	{
		if (true) {	
			if (useUDP == true)
				WarpClient.GetInstance ().SendUDPUpdatePeers (msg);
			else
				WarpClient.GetInstance ().SendUpdatePeers (msg);
		}
	}

	public void onSendMoveDone (byte result)
	{
		Log ("onSendMoveDone");
	}

	public void onStartGameDone (byte result)
	{
		Log ("onStartGameDone  : " + result + "  :  " + result.ToString ());
	}

	public void onStopGameDone (byte result)
	{
		Log ("onStopGameDone  : " + result + "  :  " + result.ToString ());
	}

	public void onGetMoveHistoryDone (byte result, MoveEvent[] history)
	{
		DebugLog.Log ("onGetMoveHistoryDone");

		if (history == null)
			return;
		
		foreach (MoveEvent me in history) {
			DebugLog.Log (me.getMoveData ());
		}
	}

	#endregion

	#region COROUTINES

	private IEnumerator TryToReconnect ()
	{
		WarpClient.GetInstance ().RecoverConnection ();
		
		yield return new WaitForSeconds (2f);
		StartCoroutine ("TryToReconnect");
	}

	#endregion
	 
}