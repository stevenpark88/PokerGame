using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//using com.shephertz.app42.gaming.multiplayer.client;
//using com.shephertz.app42.gaming.multiplayer.client.events;
//using com.shephertz.app42.gaming.multiplayer.client.listener;
//using com.shephertz.app42.gaming.multiplayer.client.command;
//using com.shephertz.app42.gaming.multiplayer.client.message;
//using com.shephertz.app42.gaming.multiplayer.client.transformer;
//using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;

//using UnityEditor;

using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;

public class appwarp : MonoBehaviour
{
	public const bool isTexassGame = true;
	public static bool isLimitGame= false;

//	public static bool isTournament = true;
//	public static bool isSitNGoTournament = true;

	public static int GAME_TYPE = GameConstant.GAME_TYPE_REGULAR;

//	private string appKey = "b597b498-e9c6-455e-b";//  Texass Game Key for local
	private string appKey = "2e267e02-dc27-4452-8";// PokerUP Texass Game Key for local
//	private string appKey = "5e8278ec-cfc7-445d-8";// PokerUP Texass Game Key for live
	private string WAAppKey = "4318ddad-038a-409d-8";// WA game key for local

//	private string appKey = "6be0f405-c805-45c2-9"; // Texass game key for live
//	private string WAAppKey = "970f84e7-bb66-4499-b";// WA game key for Live
//	private string ipAddress = "192.168.1.200";//Office
//	private string ipAddress = "192.168.1.102";//Home

//--------- Live Server Details-------------
	private string ipAddress = "52.40.28.151";// PokerUP
//	private string ipAddress = "52.25.168.232";

	public static string username = "";
	public static string currentRoomId ="";
	public const string TEXASS_SERVER_NAME = "TexassAppWarpS2";
	public const string WA_SERVER_NAME = "WAAppWarpS2";
	public static int MAXIMUM_PLAYERS = 9;
	public static int MINIMUM_PLAYERS_TO_START_GAME = 2;
	public static int TURN_TIME = 10;


	Listener listen;

	public bool isConnected = false;

	void Start () {
		Application.runInBackground = true;
		if(!isTexassGame){
			appKey = WAAppKey;
		}

		if (UIManager.Instance.gameType != POKER_GAME_TYPE.TABLE) {
			Debug.Log ("AppWarp start");
			WarpClient.initialize (appKey, ipAddress, 12346);
			listen = GetComponent<Listener> ();
			WarpClient.GetInstance ().AddConnectionRequestListener (listen);
			WarpClient.GetInstance ().AddChatRequestListener (listen);
			WarpClient.GetInstance ().AddLobbyRequestListener (listen);
			WarpClient.GetInstance ().AddNotificationListener (listen);
			WarpClient.GetInstance ().AddRoomRequestListener (listen);
			WarpClient.GetInstance ().AddUpdateRequestListener (listen);
			WarpClient.GetInstance ().AddZoneRequestListener (listen);
			WarpClient.GetInstance ().AddTurnBasedRoomRequestListener (listen);
		}
	}
}
