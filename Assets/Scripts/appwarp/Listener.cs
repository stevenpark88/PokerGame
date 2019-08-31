using UnityEngine;

using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using com.shephertz.app42.gaming.multiplayer.client.listener;
using com.shephertz.app42.gaming.multiplayer.client.command;
using com.shephertz.app42.gaming.multiplayer.client.message;
using com.shephertz.app42.gaming.multiplayer.client.transformer;
using System;
using System.Collections.Generic;

using System.Collections;

namespace AssemblyCSharp
{
	public class Listener : MonoBehaviour, ConnectionRequestListener, LobbyRequestListener, ZoneRequestListener, RoomRequestListener, ChatRequestListener, UpdateRequestListener, NotifyListener,TurnBasedRoomListener
	{
		public bool debugLog;
		string debug = "";
		private appwarp m_apppwarp;
		private JSON_Object jsonResponce;
		//GamePlayScript roomScript;
		//	private GamePlayScript gamePlay;
		TexassGameManager texassGameManager;
		WAGameManager waGameManager;
		bool isGameRunning = false;

		public LoginScript loginScript;

		private void Log (string msg)
		{
			debug = msg + "\n" + debug;
			//Debug.Log(msg);
		}

		public string getDebug ()
		{
			return debug;
		}
		
		//Mono Behaviour
		
		void Start ()
		{
			m_apppwarp = GetComponent<appwarp> ();
			texassGameManager = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<TexassGameManager> ();
			waGameManager = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<WAGameManager> ();
//			generateWinnerPlayers (appwarp.TEXASS_SERVER_NAME,"");
		}
		
		//ConnectionRequestListener
		public void onConnectDone (ConnectEvent eventObj)
		{
			Debug.Log ("onConnectDone >> " + eventObj.getResult ());
		DEBUG.Log ("ConnectDone >> " + eventObj.getResult ());
			//DEBUG.Log ("onConnectDone "+eventObj.getResult());
//			GameObject camera = GameObject.FindGameObjectWithTag ("LoginPanel");

			if (eventObj.getResult () == WarpResponseResultCode.SUCCESS) {
					loginScript.onConnectedSuccessfully ();
			m_apppwarp.isConnected = true;
			}else if(eventObj.getResult()== WarpResponseResultCode .SUCCESS_RECOVERED ){
				Debug.Log ("onConnectDone >>  Recovered Done" + eventObj.getResult ());
			} else if(eventObj.getResult()== WarpResponseResultCode .CONNECTION_ERROR_RECOVERABLE ){
				Debug.Log ("onConnectDone >> Connection Recoverable" + eventObj.getResult ());
				StartCoroutine (ReconnectToServer ());
			}else if (eventObj.getResult() == WarpResponseResultCode.CONNECTION_ERR) {
				loginScript.onConnectedFail ("" + eventObj.getResult ());
				m_apppwarp.isConnected = false;
			}

		}
		IEnumerator ReconnectToServer ()
		{
			yield return new WaitForSeconds (6f);
			WarpClient.GetInstance ().RecoverConnection ();
		}
		public void onInitUDPDone (byte res)
		{
		}

		public void onLog (String message)
		{
			Log (message);
		}

		public void onDisconnectDone (ConnectEvent eventObj)
		{
			Debug.Log ("onDisconnectDone : " + eventObj.getResult ());
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

			if (eventObj.getResult () == 0) {
				Debug.Log ("onGetAllRoomsDone : " + eventObj.getRoomIds ().Length);
				if (eventObj.getRoomIds ().Length > 0) {
					foreach (string roomId in eventObj.getRoomIds()) {
//						WarpClient.GetInstance ().GetLiveRoomInfo (roomId);

					}
					//WarpClient.GetInstance().SubscribeRoom("1170889653");
				}
			}
		}

		public void onCreateRoomDone (RoomEvent eventObj)
		{
			Debug.Log ("onCreateRoomDone : " + eventObj.getResult ());

			string roomId = "";
			if (eventObj.getResult () == 0) {
				roomId = eventObj.getData ().getId ();
				WarpClient.GetInstance ().GetLiveRoomInfo (roomId);

				// This is for PHP to WebGL
				if (loginScript != null) {
//					if (loginScript.isPHPCall ()) {
//						WarpClient.GetInstance ().SubscribeRoom (roomId);
//						WarpClient.GetInstance ().JoinRoom (roomId);
//					}
				}
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
		}

		public void onGetMatchedRoomsDone (MatchedRoomsEvent eventObj)
		{
			if (eventObj.getResult () == WarpResponseResultCode.SUCCESS) {
				Log ("GetMatchedRooms event received with success status");

				foreach (var roomData in eventObj.getRoomsData()) {
					Log ("Room ID:" + roomData.getId ());
					WarpClient.GetInstance ().GetLiveRoomInfo (roomData.getId());
				}
			}
		}
		//RoomRequestListener
		public void onSubscribeRoomDone (RoomEvent eventObj)
		{
			Debug.Log ("onSubscribeRoomDone : " + eventObj.getResult ());
			string roomid = "";
			if (eventObj.getResult () == 0) {
				roomid = eventObj.getData ().getId ();
//				WarpClient.GetInstance().startGame();
//				string json = "{\"start\":\""+roomid+"\"}";
//				WarpClient.GetInstance().SendChat(json);
				//WarpClient.GetInstance().JoinRoom("1170889653");
			}
		}

		public void onUnSubscribeRoomDone (RoomEvent eventObj)
		{
			Debug.Log ("onUnSubscribeRoomDone : " + eventObj.getResult ());
		}

		public void onJoinRoomDone (RoomEvent eventObj)
		{
			Debug.Log ("On Join room done");
			// This is for PHP to WebGL
			GameObject loginPanel = GameObject.FindGameObjectWithTag ("LoginPanel");
			if (loginPanel != null) {
//				if (loginPanel.GetComponent<LoginScript> ().isPHPCall ()) {
//					loginPanel.GetComponent<LoginScript> ().startGame ();	
//				}
			} else {
				RoomListScript roomListScript = GameObject.FindGameObjectWithTag ("RoomListPanel").GetComponent<RoomListScript> ();
				roomListScript.onJoinRoomSuccessfully (eventObj);
			}
		}

		public void onLockPropertiesDone (byte result)
		{
			Debug.Log ("onLockPropertiesDone : " + result);
		}

		public void onUnlockPropertiesDone (byte result)
		{
			Debug.Log ("onUnlockPropertiesDone : " + result);
		}

		public void onLeaveRoomDone (RoomEvent eventObj)
		{
			Debug.Log ("onLeaveRoomDone : " + eventObj.getResult ());
		}

		public void onGetLiveRoomInfoDone (LiveRoomInfoEvent liveRoomInfoEvent)
		{

			//	Debug.Log ("onGetLiveRoomInfoDone " + liveRoomInfoEvent.getResult ()+"... "+liveRoomInfoEvent.getData().getId()+".. "+liveRoomInfoEvent.getData().getName());
//			MenuHandler menuHandler = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<MenuHandler> ();
//			if (menuHandler.roomListPanel == false) 
//			{
//				Debug.Log("2");
//				menuHandler.roomListPanel.SetActive(true);
//			}
//			if (menuHandler.createRoomPanel == true) 
//			{
//				Debug.Log("1");
//				menuHandler.createRoomPanel.SetActive(false);
//			}


			if (liveRoomInfoEvent.getResult () == 0) {

				if (GameObject.FindGameObjectWithTag ("RoomListPanel") != null) {
					Debug.Log ("On room list panel");
					UIManager.Instance.roomsPanel.loadAvailableRoomsOnZone (liveRoomInfoEvent);
				} else {
					Debug.Log ("On room panel");
					//					GamePlayScript roomScript = GameObject.FindGameObjectWithTag ("GamePlayPanel").GetComponent<GamePlayScript> ();
					//					roomScript.onRoomDetails (liveRoomInfoEvent);
				}
			}
		}

		public void onSetCustomRoomDataDone (LiveRoomInfoEvent eventObj)
		{
			Debug.Log ("onSetCustomRoomDataDone : " + eventObj.getResult ());
		}

		public void onUpdatePropertyDone (LiveRoomInfoEvent eventObj)
		{
			if (WarpResponseResultCode.SUCCESS == eventObj.getResult ()) {
				Debug.Log ("UpdateProperty event received with success status");
			} else {
				Debug.Log ("Update Propert event received with fail status. Status is :" + eventObj.getResult ().ToString ());
			}
		}
		
		//ChatRequestListener
		public void onSendChatDone (byte result)
		{
			Debug.Log ("onSendChatDone result : " + result);
			
		}

		public void onSendPrivateChatDone (byte result)
		{
			Debug.Log ("onSendPrivateChatDone : " + result);
		}
		
		//UpdateRequestListener
		public void onSendUpdateDone (byte result)
		{
		}
		
		//NotifyListener
		public void onRoomCreated (RoomData eventObj)
		{
			Debug.Log ("onRoomCreated");
		}

		public void onRoomDestroyed (RoomData eventObj)
		{
			Debug.Log ("onRoomDestroyed");
		}

		public void onUserLeftRoom (RoomData eventObj, string username)
		{
			Debug.Log ("onUserLeftRoom : " + username);
			if (appwarp.isTexassGame) {
				texassGameManager.leavePlayerOnTable (username);
			} else {
				waGameManager.leavePlayerOnTable (username);
			}
		
		}

		public void onUserJoinedRoom (RoomData eventObj, string username)
		{
			Debug.Log ("onUserJoinedRoom : " + username + " >> " + eventObj.getName ());
			/*if (GameObject.FindGameObjectWithTag (GameConstant.UI_PATH_GAME_PLAY) != null) {
				if (username.Equals (appwarp.username)) {

					//	roomScript.onNewUserJoined(username,jsonResponce);
				} else {
					//	roomScript.onNewOtherUserJoined(username,jsonResponce);
				}


			}*/


		}

		public void onUserLeftLobby (LobbyData eventObj, string username)
		{
			Debug.Log ("onUserLeftLobby : " + username);
		}

		public void onUserJoinedLobby (LobbyData eventObj, string username)
		{
			Debug.Log ("onUserJoinedLobby : " + username);
		}

		public void onUserChangeRoomProperty (RoomData roomData, string sender, Dictionary<string, object> properties, Dictionary<string, string> lockedPropertiesTable)
		{
			Debug.Log ("onUserChangeRoomProperty : " + sender);
		}

		public void onPrivateChatReceived (string sender, string message)
		{
			Debug.Log ("onPrivateChatReceived : " + sender);
		}

		public void onMoveCompleted (MoveEvent move)
		{
			if (appwarp.isTexassGame) {
				texassGameManager.highLightTurnPlayer (move.getNextTurn ());
			} else {
				waGameManager.highLightTurnPlayer (move.getNextTurn ());
			}
		}

		public void onChatReceived (ChatEvent eventObj)
		{

			//Log(eventObj.getSender() + " sent Message");
			//			m_apppwarp.onMsg(eventObj.getSender(), eventObj.getMessage());

			String msg = eventObj.getMessage ();
			String sender = eventObj.getSender ();

			Debug.Log ("Chat Received : " + msg + " \n Sender : " + sender);

			if (sender.Equals (appwarp.TEXASS_SERVER_NAME) || sender.Equals (appwarp.WA_SERVER_NAME)) {

				/*	// Get default cards details. Flops, River, Turn*/
				if (msg.StartsWith (GameConstant.RESPONSE_FOR_DEFAULT_CARDS)) {
					msg = msg.Replace (GameConstant.RESPONSE_FOR_DEFAULT_CARDS, "");

					Debug.Log ("Default Cards Details : " + msg);

					JSON_Object jsonResponce = new JSON_Object (msg);
					manageDefaultCards (sender, jsonResponce);

					//Get all room player infomations.
				} else if (msg.StartsWith (GameConstant.RESPONSE_FOR_PLAYERS_INFO)) {
					msg = msg.Replace (GameConstant.RESPONSE_FOR_PLAYERS_INFO, "");
					JSON_Object jsonResponce = new JSON_Object (msg);
					managePlayerCards (sender, jsonResponce);

				} else if (msg.StartsWith (GameConstant.RESPONSE_FOR_BLIEND_PLAYER)) {
					msg = msg.Replace (GameConstant.RESPONSE_FOR_BLIEND_PLAYER, "");
					JSON_Object jsonResponce = new JSON_Object (msg);
					manageBliendPlayers (sender, jsonResponce);

				} else if (msg.StartsWith (GameConstant.RESPONSE_FOR_ACTION_DONE)) {
					msg = msg.Replace (GameConstant.RESPONSE_FOR_ACTION_DONE, "");
					//managePlayerMoveActionDEBUG.Log("msg : "+ msg);
					managePlayerMoveAction (sender, msg);

				} else if (msg.StartsWith (GameConstant.RESPONSE_FOR_ROUND_COMPLETE)) {
					msg = msg.Replace (GameConstant.RESPONSE_FOR_ROUND_COMPLETE, "");
					manageRoundCompleted (sender, msg);

				} else if (msg.StartsWith (GameConstant.RESPONSE_FOR_GAME_COMPLETE)) {
					msg = msg.Replace (GameConstant.RESPONSE_FOR_GAME_COMPLETE, "");
					//				manageGameCompleted (sender, msg);
					if (sender.Equals (appwarp.TEXASS_SERVER_NAME))
						texassGameManager.gameFinished ();
					else
						waGameManager.gameFinished ();
					WarpClient.GetInstance ().stopGame ();

				} else if (msg.StartsWith (GameConstant.RESPONSE_FOR_DESTRIBUTE_CARD)) {
					//					DEBUG.Log("Start Distribution");
					if (sender.Equals (appwarp.TEXASS_SERVER_NAME))
						texassGameManager.distributeCards ();
					else
						waGameManager.distributeCards ();
				} else if (msg.StartsWith (GameConstant.RESPONSE_FOR_WINNIER_INFO)) {
					msg = msg.Replace (GameConstant.RESPONSE_FOR_WINNIER_INFO, "");

					generateWinnerPlayers (sender, msg);
				} else if (msg.StartsWith (GameConstant.RESPONSE_FOR_GAME_START)) {
					msg = msg.Replace (GameConstant.RESPONSE_FOR_GAME_START, "");
					if (sender.Equals (appwarp.TEXASS_SERVER_NAME)) {
						texassGameManager.initGamePlay ();
					} else {
						waGameManager.initGamePlay ();
					}
				} else if (msg.StartsWith (GameConstant.REQUEST_FOR_WA_POT_WINNER)) {

					msg = msg.Replace (GameConstant.REQUEST_FOR_WA_POT_WINNER, "");
					if (sender.Equals (appwarp.WA_SERVER_NAME)) {
						JSONArray jSONArray = new JSONArray (msg);
						manageWAPotAmount (jSONArray);
					}
				}
			}

		}

		public void manageReBuyChips(string sender , string msg){
			if(sender.Equals(appwarp.TEXASS_SERVER_NAME)){
				JSON_Object jSONObject = new JSON_Object (msg);
				if (sender.Equals (appwarp.TEXASS_SERVER_NAME)) {
					texassGameManager.getPlayerManager ().addBalanceToPlayer (
						jSONObject.getString (GameConstant.TAG_PLAYER_NAME), 
						jSONObject.getInt (GameConstant.TAG_PLAYER_BALANCE));
				}
			}
		}
		public void onUpdatePeersReceived (UpdateEvent eventObj)
		{
			//Log ("onUpdatePeersReceived");

			//Log("isUDP " + eventObj.getIsUdp());
		}


		private void manageGameCompleted (string sender, string msg)
		{
//			DEBUG.Log ("Game Completed : "+msg);
			JSON_Object jsonResponce = new JSON_Object (msg);
			List<string> listBestCard = new List<string> ();
			JSONArray jSONArray = jsonResponce.getJSONArray (GameConstant.TAG_WINNER_BEST_CARDS);
			for (int i = 0; i < jSONArray.Count (); i++) {
				listBestCard.Add (jSONArray.get (i).ToString ());
			}
			if (sender.Equals (appwarp.TEXASS_SERVER_NAME)) {
				texassGameManager.manageGameFinishAction (
					jsonResponce.getString (GameConstant.TAG_WINNER_NAME),
					jsonResponce.getInt (GameConstant.TAG_WINNER_RANK),
					jsonResponce.getInt (GameConstant.TAG_WINNERS_WINNING_AMOUNT),
					jsonResponce.getInt (GameConstant.TAG_WINNER_TOTAL_BALENCE), listBestCard);
			} else {
				waGameManager.manageGameFinishAction (
					jsonResponce.getString (GameConstant.TAG_WINNER_NAME),
					jsonResponce.getInt (GameConstant.TAG_WINNER_RANK),
					jsonResponce.getInt (GameConstant.TAG_WINNER_TOTAL_BALENCE), 
					jsonResponce.getInt (GameConstant.TAG_WINNERS_WINNING_AMOUNT),
					listBestCard);	//	jsonResponce.getInt (GameConstant.TAG_WINNERS_WINNING_AMOUNT),
			}
		}

		private void manageRoundCompleted (string sender, string msg)
		{
			JSON_Object jsonResponce = new JSON_Object (msg);
			if (sender.Equals (appwarp.TEXASS_SERVER_NAME)) {
				texassGameManager.moveToNextRound (
					jsonResponce.getInt (GameConstant.TAG_ROUND),
					jsonResponce.getInt (GameConstant.TAG_TABLE_AMOUNT));
			} else {
				waGameManager.moveToNextRound (
					jsonResponce.getInt (GameConstant.TAG_ROUND),
					jsonResponce.getInt (GameConstant.TAG_TABLE_AMOUNT));
			}
		}

		private void managePlayerMoveAction (string sender, string msg)
		{
			JSON_Object jsonResponce = new JSON_Object (msg);
			if (jsonResponce.getInt (GameConstant.TAG_ACTION) != GameConstant.ACTION_NO_TURN) {
				if (sender.Equals (appwarp.TEXASS_SERVER_NAME)) {
					texassGameManager.managePlayerMoveAction (
						jsonResponce.getString (GameConstant.TAG_PLAYER_NAME),
						jsonResponce.getInt (GameConstant.TAG_BET_AMOUNT),
						jsonResponce.getInt (GameConstant.TAG_TABLE_AMOUNT),
						jsonResponce.getInt (GameConstant.TAG_PLAYER_BALANCE),
						jsonResponce.getInt (GameConstant.TAG_ACTION));
				} else {
					waGameManager.managePlayerMoveAction (
						jsonResponce.getString (GameConstant.TAG_PLAYER_NAME),
						jsonResponce.getInt (GameConstant.TAG_BET_AMOUNT),
						jsonResponce.getInt (GameConstant.TAG_TABLE_AMOUNT),
						jsonResponce.getInt (GameConstant.TAG_PLAYER_BALANCE),
						jsonResponce.getInt (GameConstant.TAG_ACTION));
				}
			}
		}

		private void manageBliendPlayers (string sender, JSON_Object jsonResponce)
		{
			int smallBliendAmt = jsonResponce.getInt (GameConstant.TAG_SMALL_BLIEND_AMOUNT);
			if (sender.Equals (appwarp.TEXASS_SERVER_NAME)) {
				
				if (texassGameManager.getBlindAmount ()!= 0) {
//					smallBliendAmt = texassGameManager.getBlindAmount ();
				}
				texassGameManager.defineBlindPlayer (smallBliendAmt,
					jsonResponce.getString (GameConstant.TAG_PLAYER_DEALER),
					jsonResponce.getString (GameConstant.TAG_PLAYER_SMALL_BLIND),
					jsonResponce.getString (GameConstant.TAG_PLAYER_BIG_BLIND));
				if (texassGameManager.getPlayerManager().getDealerPlayer ().getPlayerName ().Equals (appwarp.username)) 
					sendGameType (smallBliendAmt);
			} else {
				if (waGameManager.getBlindAmount ()!= 0) {
//					smallBliendAmt = waGameManager.getBlindAmount ();
				}
				waGameManager.defineBlindPlayer (smallBliendAmt,
					jsonResponce.getString (GameConstant.TAG_PLAYER_DEALER),
					jsonResponce.getString (GameConstant.TAG_PLAYER_SMALL_BLIND),
					jsonResponce.getString (GameConstant.TAG_PLAYER_BIG_BLIND));
				if (waGameManager.getPlayerManager().getDealerPlayer ().getPlayerName ().Equals (appwarp.username)) 
					sendGameType (smallBliendAmt);
			}
		
		}

		private void manageDefaultCards (string sender, JSON_Object jsonResponce)
		{
			if (sender.Equals (appwarp.TEXASS_SERVER_NAME)) {
				texassGameManager.setDefaultTableCards (
					jsonResponce.getString (GameConstant.TAG_CARD_FLOP_1), 
					jsonResponce.getString (GameConstant.TAG_CARD_FLOP_2), 
					jsonResponce.getString (GameConstant.TAG_CARD_FLOP_3), 
					jsonResponce.getString (GameConstant.TAG_CARD_TURN), 
					jsonResponce.getString (GameConstant.TAG_CARD_RIVER));
			} else {
				waGameManager.setDefaultTableCards (
					jsonResponce.getString (GameConstant.TAG_CARD_FIRST_FLOP_1), 
					jsonResponce.getString (GameConstant.TAG_CARD_FIRST_FLOP_2), 
					jsonResponce.getString (GameConstant.TAG_CARD_SECOND_FLOP_1), 
					jsonResponce.getString (GameConstant.TAG_CARD_SECOND_FLOP_2), 
					jsonResponce.getString (GameConstant.TAG_CARD_THIRD_FLOP_1),
					jsonResponce.getString (GameConstant.TAG_CARD_THIRD_FLOP_2));
			}
		}

		private void managePlayerCards (string sender, JSON_Object jsonResponce)
		{
//			DEBUG.Log ("Plr Card : "+sender+" \n>> "+ jsonResponce.toString());
			if (sender.Equals (appwarp.TEXASS_SERVER_NAME)) {
				texassGameManager.addNewPlayerOnTable (
					1,
					jsonResponce.getString (GameConstant.TAG_PLAYER_NAME),
					jsonResponce.getInt (GameConstant.TAG_PLAYER_BALANCE), 
					jsonResponce.getString (GameConstant.TAG_CARD_PLAYER_1), 
					jsonResponce.getString (GameConstant.TAG_CARD_PLAYER_2),
					jsonResponce.getInt(GameConstant.TAG_GAME_STATUS),
					jsonResponce.getInt(GameConstant.TAG_PLAYER_STATUS),
					jsonResponce.getInt(GameConstant.TAG_CURRENT_ROUND));
			} else {
				waGameManager.addNewPlayerOnTable (
					1,
					jsonResponce.getString (GameConstant.TAG_PLAYER_NAME),
					jsonResponce.getInt (GameConstant.TAG_PLAYER_BALANCE), 
					jsonResponce.getString (GameConstant.TAG_CARD_PLAYER_1), 
					jsonResponce.getString (GameConstant.TAG_CARD_PLAYER_2),
					jsonResponce.getString (GameConstant.TAG_CARD_WA),
					jsonResponce.getInt(GameConstant.TAG_GAME_STATUS),
					jsonResponce.getInt(GameConstant.TAG_PLAYER_STATUS));
			}
		}

		public void onUserChangeRoomProperty (RoomData roomData, string sender, Dictionary<String, System.Object> properties)
		{
			Log ("Notification for User Changed Room Property received");
			Log (roomData.getId ());
			Log (sender);
			foreach (KeyValuePair<String, System.Object> entry in properties) {
				Log ("KEY:" + entry.Key);
				Log ("VALUE:" + entry.Value.ToString ());
			}
		}

		public void onUserPaused (string a, bool b, string c)
		{
		}

		public void onUserResumed (string a, bool b, string c)
		{
		}

		public void onGameStarted (string sender, string roomId, string nextTurn)
		{
			if (appwarp.isTexassGame) {
				texassGameManager.startPreFlopRound ();
				texassGameManager.highLightTurnPlayer (nextTurn);
			} else {
				waGameManager.startStartRound ();
				waGameManager.highLightTurnPlayer (nextTurn);
			}

		}

		public void onGameStopped (string a, string b)
		{
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
			Debug.Log ("onSendMoveDone");
		}

		public void onStartGameDone (byte result)
		{
//			DEBUG.Log ("onStartGameDone");
		}

		public void onStopGameDone (byte result)
		{
			Debug.Log ("onStopGameDone");
			//DEBUG.Log ("Game is completed...");
		}

		public void onGetMoveHistoryDone (byte result, MoveEvent[] history)
		{
			Debug.Log ("onGetMoveHistoryDone");
		}

		private void generateWinnerPlayers (string sender, string winnerDetails)
		{
			DEBUG.Log ("Winner Data : "+ winnerDetails);
			JSONArray jsonResponceArray;
			JSONArray waPotResponceArray = null;
			if (sender.Equals (appwarp.WA_SERVER_NAME)) {
				JSON_Object jsonObject = new JSON_Object (winnerDetails);
				jsonResponceArray = jsonObject.getJSONArray ("Table_Pot");
				waPotResponceArray = jsonObject.getJSONArray ("WA_Pot");
			} else {
				jsonResponceArray = new JSONArray (winnerDetails);
			}

			StartCoroutine (winnerBrodcast (sender, jsonResponceArray, waPotResponceArray));
		}
		/**
		 * Timer for rebuy chips in tournament
		 * */
//		private IEnumerator reBuyChipTimerOff(string sender){
//			yield return new WaitForSeconds (GameConstant.TOURNAMENT_REBUY_TIMER);
//
//			if (sender.Equals (appwarp.TEXASS_SERVER_NAME))
//				texassGameManager.canReBuy = false;
////			else
////				waGameManager.distributeCards ();
//			
//		}


		private void sendGameType(int amt){
			JSON_Object jsonObject = new JSON_Object ();
			jsonObject.put (GameConstant.TAG_SMALL_BLIEND_AMOUNT,amt);
			jsonObject.put (GameConstant.TAG_GAME_TYPE, appwarp.GAME_TYPE);
			WarpClient.GetInstance ().SendChat (GameConstant.REQUEST_FOR_BLIEND_AMOUNT+jsonObject.ToString());
		}

		IEnumerator winnerBrodcast (string sender, JSONArray jsonResponceArray, JSONArray waPotResponceArray)
		{
			//		StartCoroutine(yourFunctionName ());
			int cntr = 0;
			if (sender.Equals (appwarp.WA_SERVER_NAME) && waPotResponceArray != null) {
				manageWAPotAmount (waPotResponceArray);
			}
			while (cntr < jsonResponceArray.Count ()) {
				JSON_Object winnerJsonResponce = (JSON_Object)jsonResponceArray.get (cntr);
				manageGameCompleted (sender, winnerJsonResponce.toString ());
				cntr++;
				yield return new WaitForSeconds (GameConstant.WAITING_TIME);
			}

			// Restart game
			if (sender.Equals (appwarp.TEXASS_SERVER_NAME)) {
				texassGameManager.restartGame ();
			} else {
				waGameManager.restartGame ();
			}
		}

		private void manageWAPotAmount (JSONArray waPotResponceArray)
		{
			int cntr = 0;
			while (cntr < waPotResponceArray.Count ()) {
				JSON_Object waWinnerObject = (JSON_Object)waPotResponceArray.get (cntr);
				waGameManager.manageWACardPotAmt (
					waWinnerObject.getString (GameConstant.TAG_WINNER_NAME),
					waWinnerObject.getInt (GameConstant.TAG_WINNER_TOTAL_BALENCE),
					waWinnerObject.getInt (GameConstant.TAG_WINNERS_WINNING_AMOUNT));
				cntr++;
			}
		}
	}
}

