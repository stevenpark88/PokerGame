using UnityEngine;
using System.Collections;
using BestHTTP.SocketIO;
using System;
using System.Collections.Generic;

public class PokerSocketManager : MonoBehaviour
{
	#region PUBLIC_VARIABLES

	public SocketManager socket;

	#endregion

	#region PRIVATE_VARIABLES

	#endregion

	#region DELEGATES

	public delegate void OnJoinCashGame (Packet joinGameInfo);

	public static event OnJoinCashGame onJoinCashGame;

	public static void FireJoinCashGame (Packet joinGameInfo)
	{
		if (onJoinCashGame != null && UIManager.Instance.lobbyPanel.gameObject.activeSelf)
			onJoinCashGame (joinGameInfo);
	}

	public delegate void OnCashGameStackUpdated (Packet stackUpdateInfo);

	public static event OnCashGameStackUpdated onCashGameStackUpdated;

	public static void FireCashGameStackUpdated (Packet stackUpdateInfo)
	{
		if (onCashGameStackUpdated != null && UIManager.Instance.lobbyPanel.gameObject.activeSelf)
			onCashGameStackUpdated (stackUpdateInfo);
	}

	public delegate void OnLeaveCashGame (Packet leaveGameInfo);

	public static event OnLeaveCashGame onLeaveCashGame;

	public static void FireLeaveCashGame (Packet leaveGameInfo)
	{
		if (onLeaveCashGame != null && UIManager.Instance.lobbyPanel.gameObject.activeSelf)
			onLeaveCashGame (leaveGameInfo);
	}

	public delegate void OnPlayerEliminate (Packet playerEliminateInfo);

	public static event OnPlayerEliminate onPlayerEliminate;

	public static void FirePlayerEliminate (Packet playerEliminateInfo)
	{
		if (onPlayerEliminate != null && UIManager.Instance.lobbyPanel.gameObject.activeSelf)
			onPlayerEliminate (playerEliminateInfo);
	}

	public delegate void OnSnGLevelUpdated (Packet levelInfo);

	public static event OnSnGLevelUpdated onSnGLevelUpdated;

	public static void FireSnGLevelUpdated (Packet levelInfo)
	{
		if (onSnGLevelUpdated != null && UIManager.Instance.lobbyPanel.gameObject.activeSelf)
			onSnGLevelUpdated (levelInfo);
	}

	public delegate void OnSnGGameStackUpdated (Packet stackUpdateInfo);

	public static event OnSnGGameStackUpdated onSnGStackUpdated;

	public static void FireSnGStackUpdated (Packet stackUpdateInfo)
	{
		if (onSnGStackUpdated != null && UIManager.Instance.lobbyPanel.gameObject.activeSelf)
			onSnGStackUpdated (stackUpdateInfo);
	}

	public delegate void OnPricePoolUpdated (Packet pricePoolInfo);

	public static event OnPricePoolUpdated onPrizePoolUpdated;

	public static void FirePricePoolUpdated (Packet pricePoolInfo)
	{
		if (onPrizePoolUpdated != null && UIManager.Instance.lobbyPanel.gameObject.activeSelf)
			onPrizePoolUpdated (pricePoolInfo);
	}

	public delegate void OnRegularTournamentStarted (Packet tournamentInfo);

	public static event OnRegularTournamentStarted onRegularTournamentStarted;

	public static void FireRegularTournamentStarted (Packet tournamentInfo)
	{
		if (onRegularTournamentStarted != null && UIManager.Instance.lobbyPanel.gameObject.activeSelf)
			onRegularTournamentStarted (tournamentInfo);
	}

	public delegate void OnRegularTournamentFinished (Packet tournamentInfo);

	public static event OnRegularTournamentFinished onRegularTournamentFinished;

	public static void FireRegularTournamentFinished (Packet tournamentInfo)
	{
		if (onRegularTournamentFinished != null && UIManager.Instance.lobbyPanel.gameObject.activeSelf)
			onRegularTournamentFinished (tournamentInfo);
	}

	public delegate void OnGameCreated (Packet gameInfo);

	public static event OnGameCreated onGameCreated;

	public static void FireGameCreated (Packet gameInfo)
	{
		if (onGameCreated != null && UIManager.Instance.lobbyPanel.gameObject.activeSelf)
			onGameCreated (gameInfo);
	}

	public delegate void OnSnGTournamentStarted (Packet tournamentInfo);

	public static event OnSnGTournamentStarted onSnGTournamentStarted;

	public static void FireSnGTournamentStarted (Packet tournamentInfo)
	{
		if (onSnGTournamentStarted != null && UIManager.Instance.lobbyPanel.gameObject.activeSelf)
			onSnGTournamentStarted (tournamentInfo);
	}

	public delegate void OnPlayerJoinedCashGame (Packet playerInfo);

	public static event OnPlayerJoinedCashGame onPlayerJoinedCashGame;

	public static void FirePlayerJoinedCashGame (Packet playerInfo)
	{
		if (onPlayerJoinedCashGame != null && UIManager.Instance.lobbyPanel.gameObject.activeSelf)
			onPlayerJoinedCashGame (playerInfo);
	}

	public delegate void OnPlayerJoinedRegularTournament (Packet playerInfo);

	public static event OnPlayerJoinedRegularTournament onPlayerJoinedRegularTournament;

	public static void FirePlayerJoinedRegularTournament (Packet playerInfo)
	{
		if (onPlayerJoinedRegularTournament != null && UIManager.Instance.lobbyPanel.gameObject.activeSelf)
			onPlayerJoinedRegularTournament (playerInfo);
	}

	public delegate void OnPlayerJoinedSnGTournament (Packet playerInfo);

	public static event OnPlayerJoinedSnGTournament onPlayerJoinedSnGTournament;

	public static void FirePlayerJoinedSnGTournament (Packet playerInfo)
	{
		if (onPlayerJoinedSnGTournament != null && UIManager.Instance.lobbyPanel.gameObject.activeSelf)
			onPlayerJoinedSnGTournament (playerInfo);
	}

	#endregion

	public static PokerSocketManager Instance;

	#region UNITY_CALLBACKS

	// Use this for initialization
	void Awake ()
	{
		Instance = this;

		// Change an option to show how it should be done
		TimeSpan miliSecForReconnect = TimeSpan.FromMilliseconds (1000);

		SocketOptions options = new SocketOptions ();
		options.ReconnectionAttempts = 3;
		options.AutoConnect = true;
		options.ReconnectionDelay = miliSecForReconnect;
		options.ConnectWith = BestHTTP.SocketIO.Transports.TransportTypes.WebSocket;

		// Create the Socket.IO manager
//		url = APIConstants.SOCKET_BASE_URL + "/socket.io/?EIO=3&transport=websocket";
		BestHTTP.HTTPManager.Setup ();
		socket = new SocketManager (new Uri (APIConstants.SOCKET_BASE_URL + "/socket.io/"), options);
		BestHTTP.HTTPManager.Setup ();

		socket.Socket.On ("connect", OnUserConnected);
		socket.Socket.On ("disconnect", OnUserDisconnected);

		socket.Socket.On ("poker", OnPokerUpdateRequest);
	}

	void OnDestroy ()
	{
		if (socket != null) {
			socket.Close ();
		}
	}

	void OnApplicationQuit ()
	{
		if (socket != null) {
			socket.Close ();
		}
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.F)) {
			string aaa = "{\"game\":{\"id\":2,\"name\":\"WhoopAss No Limit\",\"users\":[\"76\",\"37\",\"78\",\"40\",\"41\",\"42\"]}}";
			Debug.Log (aaa);
			PokerSocketManager.TournamentStartResp tsr = JsonUtility.FromJson<PokerSocketManager.TournamentStartResp> (aaa);
			if (tsr.game.users.Contains (LoginScript.loggedInPlayer.id)) {
				UIManager.Instance.tournamentStartingPanel.DisplayTournamentStarting (tsr.game.id, "TOURNAMENT STARTING", tsr.game.name + " Tournament is going to start");
			}
		}
	}

	#endregion

	#region DELEGATE_CALLBACKS

	#endregion

	#region SOCKET_CALLBACKS

	private void OnUserConnected (Socket socket, Packet packet, params object[] args)
	{
		Debug.Log ("---------- Player Connected ---------- " + packet.ToString ());
	}

	private void OnUserDisconnected (Socket socket, Packet packet, params object[] args)
	{
		Debug.Log ("---------- Player Disconnected ---------- " + packet.ToString ());
	}

	private void OnPokerUpdateRequest (Socket socket, Packet packet, params object[] args)
	{
		JSONArray arr = new JSONArray (packet.ToString ());
		JSON_Object eventObj = arr.getJSONObject (1);
		string eventFired = eventObj.getString ("event");
		string dataReceived = eventObj.getString ("data");

		Debug.Log ("Event  : " + eventFired + " --> Data  : " + dataReceived);

		switch (eventFired) {
		case Constants.SOCKET_EVENT_CASH_GAME_JOIN:
			FireJoinCashGame (packet);
			break;
		case Constants.SOCKET_EVENT_CASH_GAME_LEAVE:
			FireLeaveCashGame (packet);
			break;
		case Constants.SOCKET_EVENT_SNG_ELIMINATE_PLAYER:
			FirePlayerEliminate (packet);
			break;
		case Constants.SOCKET_EVENT_SNG_LEVEL_UPDATE:
			FireSnGLevelUpdated (packet);
			break;
		case Constants.SOCKET_EVENT_STACK_UPDATE:
			FireCashGameStackUpdated (packet);
			FireSnGStackUpdated (packet);
			break;
		case Constants.SOCKET_EVENT_REGULAR_TOUR_PRICE_POOL_UPDATE:
			FirePricePoolUpdated (packet);
			break;
		case Constants.SOCKET_EVENT_REGULAR_TOUR_START:
			FireRegularTournamentStarted (packet);
			break;
		case Constants.SOCKET_EVENT_REGULAR_TOUR_FINISH:
			FireRegularTournamentFinished (packet);
			break;
		case Constants.SOCKET_EVENT_GAME_CREATED:
			FireGameCreated (packet);
			break;
		case Constants.SOCKET_EVENT_START_SNG_GAME:
			FireSnGTournamentStarted (packet);
			HandleSnGTournamentStart (packet);
			break;
		case Constants.SOCKET_EVENT_USER_JOINED_CASH_GAME:
			FirePlayerJoinedCashGame (packet);
			break;
		case Constants.SOCKET_EVENT_USER_JOINED_REGULAR_TOUR:
			FirePlayerJoinedRegularTournament (packet);
			break;
		case Constants.SOCKET_EVENT_USER_JOINED_SNG_TOUR:
			FirePlayerJoinedSnGTournament (packet);
			break;
		}
	}

	#endregion

	#region PUBLIC_METHODS

	#endregion

	#region PRIVATE_METHODS

	private void HandleSnGTournamentStart (Packet packet)
	{
		Debug.LogWarning ("=====> " + packet.ToString () + " <======");

		JSONArray arr = new JSONArray (packet.ToString ());
		JSON_Object obj = arr.getJSONObject (1).getJSONObject ("data");

		TournamentStartResp tsr = JsonUtility.FromJson<TournamentStartResp> (obj.toString ());
		if (tsr.game.users.Contains (LoginScript.loggedInPlayer.id)) {
			if (tsr.game.users.Contains (LoginScript.loggedInPlayer.id)) {
				UIManager.Instance.tournamentStartingPanel.DisplayTournamentStarting (tsr.game.id, "TOURNAMENT STARTING", tsr.game.name + " Tournament is going to start");
			}
		}
	}

	#endregion

	#region COROUTINES

	#endregion

	[Serializable]
	public class TournamentStartResp
	{
		public Tour game;

		[Serializable]
		public class Tour
		{
			public string id;
			public string name;
			public List<string> users;
		}
	}
}