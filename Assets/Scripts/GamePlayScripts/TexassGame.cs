using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using com.shephertz.app42.gaming.multiplayer.client.events;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class TexassGame : MonoBehaviour
{
	#region PUBLIC_VARIABLES

	public Text txtTableMessage;

	public GAME_STATUS currentGameStatus;

	public Text txtTitle;

	public bool canRebuy = true;

	public Button btnNote;
	public Button btnStats;
	public Button btnInfo;

	public GameObject texassPlayerPrefab;

	public double anteBetAmount = 5;

	public PlayerInfo dealerInfo;

	public Sprite spButtonSelected;
	public Sprite spButtonDeselected;

	public ScrollRect scrollNote;
	public ScrollRect scrollStats;
	public ScrollRect scrollInfo;
	public ScrollRect scrollChat;

	public Scrollbar chatScrollBar;
	public Scrollbar leftScrollBar;

	public Text txtGameLog;
	public Text txtChat;

	public List<Transform> playerPositions;
	public List<Transform> playerSeats;
	//	public List<Transform> players;

	public TexassPlayer ownTexassPlayer;
	public List<TexassPlayer> allTexassPlayers;

	private double tableTotalAmount;
	public Text txtTableTotalAmount;

	public Text txtPlayerChips;

	public Button btnCheck;
	public Button btnRaise;
	public Button btnFold;
	public Button btnCall;
	public Button btnBet;
	public Button btnAllin;
	public Button btnRebuy;
	public Button btnAddChips;
	public Button btnSitoutNextHand;
	public Button btnBackToGame;

	public Slider betSlider;
	public Text txtBetSliderValue;

	public InputField ifChat;

	public CanvasGroup gameButtonsCanvasGroup;
	public CanvasGroup noTurnCheckBoxesCanvasGroup;

	public TexassDefaultCards texassDefaultCards;

	public List<Transform> defaultCardsPositionList;
	public List<CardFlipAnimation> defaultCardsList;

	public bool isGameCompleted = true;

	public string currentTurnPlayerID;

	public double minimumAmountToBet = 0;

	public int raisePerRoundCounter = 0;

	public double smallBlindAmount;
	public double bigBlindAmount;

	public GameObject tableChipObject;

	public GameObject chipPrefab;

	public Text txtMessage;
	public int resetGameAfterTimer = 10;

	public Image imgPotAmountBG;

	public GameObject redChipPrefab;
	public GameObject greenChipPrefab;
	public GameObject blueChipPrefab;

	public Transform redInitialChip;
	public Transform greenInitialChip;
	public Transform blueInitialChip;

	public Image imgWALogo;

	public RebuyPanel rebuyPanel;

	public GameObject rakeAmountPrefab;

	public ChatTemplates chatTemplatesPanel;

	public Button btnChatTitle;
	public Transform chatParent;
	public Transform chatOpenedPosition;
	public Transform chatClosedPosition;

	public double initialBuyinAmountForTournament;

	public bool recentlyBackToGame = false;
	public int timeoutCounter = 0;

	public Text GameTimer;
	public int GamePlaytime =1;
	DateTime currentDateupdate;
	public Transform objectsGenerateHere;

	#endregion

	#region PRIVATE_VARIABLES

	private int totalChipObject;
	private int totalRedChipObject;
	private int totalGreenChipObject;
	private int totalBlueChipObject;
	private List<GameObject> chipsDisplayedList;
	private List<GameObject> redChipsDisplayedList;
	private List<GameObject> greenChipsDisplayedList;
	private List<GameObject> blueChipsDisplayedList;
	private int sitOutCounter = 0;

	private bool isChatParentOpened = false;

	#endregion

	#region DELEGATES

	public delegate void ResetData ();

	public static event ResetData resetData;

	public static void FireResetData ()
	{
		if (resetData != null)
			resetData ();
	}

	#endregion


	public static TexassGame Instance;


	#region UNITY_CALLBACKS

	// Use this for initialization
	void Awake ()
	{
		Instance = this;

		allTexassPlayers = new List<TexassPlayer> ();

		btnBet.interactable = false;

		chipsDisplayedList = new List<GameObject> ();
		redChipsDisplayedList = new List<GameObject> ();
		greenChipsDisplayedList = new List<GameObject> ();
		blueChipsDisplayedList = new List<GameObject> ();
		TableTotalAmount = 0;

		btnSitoutNextHand.gameObject.SetActive (false);
		btnBackToGame.gameObject.SetActive (false);
	}

	void OnEnable ()
	{
		timeoutCounter = 0;
		initialBuyinAmountForTournament = 0;
		if (LoginScript.loginDetails != null)
			initialBuyinAmountForTournament = double.Parse (LoginScript.loginDetails.buyin);

		totalChipObject = 0;
		totalRedChipObject = totalGreenChipObject = totalBlueChipObject = 0;
		sitOutCounter = 0;

		SetGameTitle ();

		RoundController.isBlindAmountCollected = false;

		NetworkManager.Instance.JoinRoom (NetworkManager.Instance.joinedRoomID);

		NetworkManager.onPlayerInfoReceived += HandleOnPlayerInfoReceived;
		NetworkManager.onGameStartedByPlayer += HandleOnGameStartedByPlayer;
		NetworkManager.onMoveCompletedByPlayer += HandleOnMoveCompletedByPlayer;
		NetworkManager.onWinnerInfoReceived += HandleOnWinnerInfoReceived;
		NetworkManager.onPlayerLeftRoom += HandleOnPlayerLeftRoom;
		NetworkManager.onPlayerConnected += HandleOnPlayerConnected;
		NetworkManager.onChatMessageReceived += HandleOnChatMessageReceived;
		NetworkManager.onPlayerTimeoutResponseReceived += HandleOnPlayerTimoutResponseReceived;
		NetworkManager.onDefaultCardDataReceived += HandleOnDefaultCardDataReceived;
		NetworkManager.onBlindPlayerResponseReceived += HandleOnBlindPlayerResponseReceived;
		NetworkManager.onResponseForRebuyReceived += HandleOnResponseForRebuyReceived;
		NetworkManager.onBreakTimeResponseReceived += HandleOnBreakTimeResponseReceived;
		NetworkManager.onRoundComplete += HandleOnRoundComplete;
		NetworkManager.onTournamentWinnerInfoReceived += HandleOnTournamentWinnerInfoReceived;
		NetworkManager.onActionHistoryReceived += HandleOnActionHistoryReceived;
		NetworkManager.onNotRegisteredInTournamentResponseReceived += HandleOnNotRegisteredInTournamentResponseReceived;
		NetworkManager.playerRequestedSitout += HandlePlayerRequestedSitout;
		NetworkManager.playerRequestedBackToGame += HandlePlayerRequestedBackToGame;
		NetworkManager.playerEliminated += HandleOnPlayerEliminated;
		NetworkManager.onRestartGameRequestReceived += HandleOnRestartGameRequestReceived;
		NetworkManager.maxSitoutResponseReceived += HandleMaxSitoutResponseReceived;
		NetworkManager.rebuyInTournamentResponseReceived += HandleRebuyInTournamentResponseReceived;
		NetworkManager.collectBlindOnBackToGame += HandleCollectBlindOnBackToGame;

		if (UIManager.Instance.isRegularTournament || UIManager.Instance.isSitNGoTournament)
			txtTableMessage.text = Constants.MessageStartingTournament;
		else
			txtTableMessage.text = "";

		ResetTimerForHourly ();

	}

	void OnDisable ()
	{
		NetworkManager.onPlayerInfoReceived -= HandleOnPlayerInfoReceived;
		NetworkManager.onGameStartedByPlayer -= HandleOnGameStartedByPlayer;
		NetworkManager.onMoveCompletedByPlayer -= HandleOnMoveCompletedByPlayer;
		NetworkManager.onWinnerInfoReceived -= HandleOnWinnerInfoReceived;
		NetworkManager.onPlayerLeftRoom -= HandleOnPlayerLeftRoom;
		NetworkManager.onPlayerConnected -= HandleOnPlayerConnected;
		NetworkManager.onChatMessageReceived -= HandleOnChatMessageReceived;
		NetworkManager.onPlayerTimeoutResponseReceived -= HandleOnPlayerTimoutResponseReceived;
		NetworkManager.onDefaultCardDataReceived -= HandleOnDefaultCardDataReceived;
		NetworkManager.onBlindPlayerResponseReceived -= HandleOnBlindPlayerResponseReceived;
		NetworkManager.onResponseForRebuyReceived -= HandleOnResponseForRebuyReceived;
		NetworkManager.onBreakTimeResponseReceived -= HandleOnBreakTimeResponseReceived;
		NetworkManager.onRoundComplete -= HandleOnRoundComplete;
		NetworkManager.onTournamentWinnerInfoReceived -= HandleOnTournamentWinnerInfoReceived;
		NetworkManager.onActionHistoryReceived -= HandleOnActionHistoryReceived;
		NetworkManager.onNotRegisteredInTournamentResponseReceived -= HandleOnNotRegisteredInTournamentResponseReceived;
		NetworkManager.playerRequestedSitout -= HandlePlayerRequestedSitout;
		NetworkManager.playerRequestedBackToGame -= HandlePlayerRequestedBackToGame;
		NetworkManager.playerEliminated -= HandleOnPlayerEliminated;
		NetworkManager.onRestartGameRequestReceived -= HandleOnRestartGameRequestReceived;
		NetworkManager.maxSitoutResponseReceived -= HandleMaxSitoutResponseReceived;
		NetworkManager.rebuyInTournamentResponseReceived -= HandleRebuyInTournamentResponseReceived;
		NetworkManager.collectBlindOnBackToGame -= HandleCollectBlindOnBackToGame;

		UIManager.Instance.isRealMoney = false;

		NetworkManager.Instance.LeaveGame ();
		DestroyAllPlayers ();

		ResetPlayerSeats ();
		FireResetData ();
		CancelInvoke("GetPerfectClock");
		DestroyAllInstantiatedObjects ();
	}

	/*void OnApplicationPause (bool paused)
	{
		if (paused)
			NetworkManager.Instance.UnsubscribeRoom (NetworkManager.Instance.joinedRoomID);
		else
			NetworkManager.Instance.SubscribeRoom (NetworkManager.Instance.joinedRoomID);
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.P)) {
			NetworkManager.Instance.UnsubscribeRoom (NetworkManager.Instance.joinedRoomID);
		} else if (Input.GetKeyDown (KeyCode.O)) {
			NetworkManager.Instance.SubscribeRoom (NetworkManager.Instance.joinedRoomID);
		}
	}*/

	#endregion

	#region DELEGATE_CALLBACKS

	private void HandleOnPlayerInfoReceived (string sender, string info)
	{
		txtTableMessage.text = "";

		if (ownTexassPlayer == null)
			UIManager.Instance.breakTimePanel.gameObject.SetActive (false);
		btnSitoutNextHand.gameObject.SetActive (false);
		btnAddChips.gameObject.SetActive (false);

		PlayerInfo playerInfo = JsonUtility.FromJson<PlayerInfo> (info);
		resetGameAfterTimer = playerInfo.Restart_Time;

		//  Need to decrease one index. Seat index from server is started from 1.
		playerInfo.Player_Position--;

		TexassPlayer player = GetPlayerByID (playerInfo.Player_Name);
		if (player) {
			player.playerID = playerInfo.Player_Name;
			player.buyInAmount = playerInfo.Player_BuyIn_Chips;
			player.card1 = playerInfo.Card1;
			player.card2 = playerInfo.Card2;
			player.txtPlayerName.text = playerInfo.Player_Name;
			player.playerInfo = playerInfo;
			player.seatIndex = playerInfo.Player_Position;
			player.totalChips = playerInfo.Player_Total_Play_Chips;
			player.totalRealMoney = playerInfo.Player_Total_Real_Chips;
			player.player_stuas = "" + playerInfo.Player_Status;
			player.isEliminated = playerInfo.Player_Status.Equals (PLAYER_STATUS.ELIMINATED);

			player.DisplayTotalChips ();
			SetGameStatus (playerInfo.Game_Status);

			if (playerInfo.Player_Status == (int)PLAYER_STATUS.ABSENT) {
				player.imgAbsentPlayer.sprite = player.spAbsent;
				player.imgAbsentPlayer.gameObject.SetActive (true);
				player.imgAbsentPlayer.color = Color.yellow;
			} else if (playerInfo.Player_Status == (int)PLAYER_STATUS.SIT_OUT) {
				player.imgAbsentPlayer.sprite = player.spSitout;
				player.imgAbsentPlayer.gameObject.SetActive (true);
				player.imgAbsentPlayer.color = Color.red;
			} else {
				player.imgAbsentPlayer.gameObject.SetActive (false);
			}

			if (playerInfo.Player_Status == (int)PLAYER_STATUS.ACTIVE ||
			    playerInfo.Player_Status == (int)PLAYER_STATUS.ABSENT)
				player.GetComponent<CanvasGroup> ().alpha = 1f;
			else if (playerInfo.Player_Status == (int)PLAYER_STATUS.ELIMINATED) {
				player.txtPlayerName.text = "<color=red>ELIMINATED</color>";
				player.DestroyCards ();
			}

			return;
		}

		if (playerInfo.Player_Status == (int)PLAYER_STATUS.ELIMINATED &&
		    playerInfo.Player_Name.Equals (NetworkManager.Instance.playerID)) {
//			if (Application.platform == RuntimePlatform.WebGLPlayer)
//				UIManager.Instance.BackToLobby ();
//			else {
//				LoginScript.loginDetails = null;
//				SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
//			}

			UIManager.Instance.backConfirmationPanel.OnYesButtonTap ();

			return;
		}

		GameObject obj = Instantiate (texassPlayerPrefab, playerSeats [playerInfo.Player_Position].position, Quaternion.identity) as GameObject;
//		GameObject obj = players [playerInfo.Player_Position].gameObject;
//		obj.SetActive (true);
		TexassPlayer texassPlayer = obj.GetComponent<TexassPlayer> ();

		texassPlayer.playerID = playerInfo.Player_Name;
		texassPlayer.buyInAmount = playerInfo.Player_BuyIn_Chips;
		texassPlayer.card1 = playerInfo.Card1;
		texassPlayer.card2 = playerInfo.Card2;
		texassPlayer.txtPlayerName.text = playerInfo.Player_Name;
		texassPlayer.playerInfo = playerInfo;
		texassPlayer.seatIndex = playerInfo.Player_Position;
		texassPlayer.totalChips = playerInfo.Player_Total_Play_Chips;
		texassPlayer.totalRealMoney = playerInfo.Player_Total_Real_Chips;
		texassPlayer.player_stuas = "" + playerInfo.Player_Status;
		texassPlayer.isEliminated = playerInfo.Player_Status.Equals (PLAYER_STATUS.ELIMINATED);
		texassPlayer.GetComponent<CanvasGroup> ().alpha = 0f;

		if (playerInfo.Player_Status == (int)PLAYER_STATUS.ABSENT) {
			texassPlayer.imgAbsentPlayer.sprite = texassPlayer.spAbsent;
			texassPlayer.imgAbsentPlayer.gameObject.SetActive (true);
			texassPlayer.imgAbsentPlayer.color = Color.yellow;
		} else if (playerInfo.Player_Status == (int)PLAYER_STATUS.SIT_OUT) {
			texassPlayer.imgAbsentPlayer.sprite = texassPlayer.spSitout;
			texassPlayer.imgAbsentPlayer.gameObject.SetActive (true);
			texassPlayer.imgAbsentPlayer.color = Color.red;
		} else
			texassPlayer.imgAbsentPlayer.gameObject.SetActive (false);

		if (playerInfo.Player_Status == (int)PLAYER_STATUS.ELIMINATED) {
			texassPlayer.txtPlayerName.text = "<color=red>ELIMINATED</color>";
			texassPlayer.DestroyCards ();
		}

		//	======================
		obj.transform.SetParent (playerSeats [playerInfo.Player_Position]);
		obj.transform.localScale = Vector3.one;
		//	======================

		allTexassPlayers.Add (texassPlayer);

		if (playerInfo.Player_Name.Equals (NetworkManager.Instance.playerID)) {
			ownTexassPlayer = texassPlayer;
			CentralizeOwnPlayer ();
			if (initialBuyinAmountForTournament == 0)
				initialBuyinAmountForTournament = playerInfo.Player_BuyIn_Chips;
		}

		if (texassPlayer.buyInAmount <= 0)
			texassPlayer.GetComponent<CanvasGroup> ().alpha = .4f;
		else
			texassPlayer.GetComponent<CanvasGroup> ().alpha = 1f;

		SetGameStatus (playerInfo.Game_Status);

		SetBetParentPositions ();

		HandleWaitingPlayer (playerInfo);
	}

	private void HandleOnGameStartedByPlayer (string sender, string gameStarter)
	{
		if (sender.Equals (Constants.TEXASS_SERVER_NAME)) {
			if (gameStarter.Equals (NetworkManager.Instance.playerID)) {
				OpenBetPanel ();
			} else {
				HideBetPanel ();

				gameButtonsCanvasGroup.interactable = false;

				//
				if (ownTexassPlayer.playerInfo.Player_Status == (int)PLAYER_STATUS.ACTIVE) {
					gameButtonsCanvasGroup.gameObject.SetActive (false);

					noTurnCheckBoxesCanvasGroup.interactable = true;
					noTurnCheckBoxesCanvasGroup.gameObject.SetActive (true);

					noTurnCheckBoxesCanvasGroup.GetComponent<NoTurnPanel> ().DisplayCheckboxes ();
				}
			}
		}

		//TexassPlayer p = GetPlayerByID (gameStarter);
		//if (p) {
		//	p.DisplayPlayerData ();
		//	p.DisplayTurnTimer ();
		//}

		currentGameStatus = GAME_STATUS.RUNNING;

		if (recentlyBackToGame) {
			NetworkManager.Instance.SendRequestToServer (Constants.REQUEST_PLAYER_BACK_TO_GAME_COLLECT_BLIND + bigBlindAmount);
		}
	}

	private void HandleOnMoveCompletedByPlayer (MoveEvent move)
	{
		currentTurnPlayerID = move.getNextTurn ();

		if (move.getNextTurn () != null && move.getNextTurn ().Equals (NetworkManager.Instance.playerID)) {
			if (!isGameCompleted) {
				if (ownTexassPlayer.playerInfo.Player_Status == (int)PLAYER_ACTION.ACTION_WAITING_FOR_GAME)
					PlayerIsWaiting ();
				else if (ownTexassPlayer.playerInfo.Player_Status == (int)PLAYER_ACTION.FOLD)
					PlayerIsFolded ();
				else if (ownTexassPlayer.playerInfo.Player_Status == (int)PLAYER_ACTION.TIMEOUT)
					PlayerIsTimeout ();
				else if (ownTexassPlayer.playerInfo.Player_Status == (int)PLAYER_ACTION.ALLIN)
					PlayerIsAllIn ();
				else if (ownTexassPlayer.playerInfo.Player_Status == (int)PLAYER_STATUS.SIT_OUT) {
					PlayerIsSitOut ();
				} else {
					OpenBetPanel ();
				}
			}
		} else {
			HideBetPanel ();
		}
	}

	private void HandleOnWinnerInfoReceived (string sender, string winnerInfo)
	{
		raisePerRoundCounter = 0;
		TournamentWinnerPanel.gameWinnerDeclaredAt = Time.time;

		isGameCompleted = true;
		currentGameStatus = GAME_STATUS.FINISHED;
		if (!btnBackToGame.gameObject.activeSelf)
			btnSitoutNextHand.gameObject.SetActive (true);

		if (ownTexassPlayer.playerInfo.Player_Status == (int)PLAYER_STATUS.SIT_OUT)// &&
//			!UIManager.Instance.isRegularTournament &&
//			!UIManager.Instance.isSitNGoTournament)
			sitOutCounter++;

		int totalWinner = 0;

		HideBetPanel ();

		gameButtonsCanvasGroup.interactable = false;


		//
		gameButtonsCanvasGroup.gameObject.SetActive (true);

		noTurnCheckBoxesCanvasGroup.interactable = false;
		noTurnCheckBoxesCanvasGroup.gameObject.SetActive (false);

		if (winnerInfo == null) {
			StartCoroutine (ResetGameAfterSomeTime (1));
			return;
		}

		JSON_Object obj = new JSON_Object (winnerInfo);
		JSONArray arr = new JSONArray (obj.getString ("Table_Pot"));

		List<GameWinner> winnerList = new List<GameWinner> ();
		totalWinner = arr.Count ();

		for (int i = 0; i < arr.Count (); i++) {
			GameWinner win = JsonUtility.FromJson<GameWinner> (arr.getString (i));
			winnerList.Add (win);
			TableTotalAmount = win.Total_Table_Amount;
		}

//		if (allTexassPlayers.Count > 1)
		StartCoroutine (DisplayWinnerAnimation (winnerList));
//		else {
//			for (int i = 0; i < winnerList.Count; i++) {
//				TexassPlayer p = GetPlayerByID (winnerList[i].Winner_Name);
//				txtGameLog.text += "\nWinner -> " + p.playerID + " -> " + GetWinningRank (winnerList [i].Winner_Rank) + " -> " + Utility.GetAmount (winnerList [i].Winning_Amount);
//				Canvas.ForceUpdateCanvases ();
//				if (scrollNote.gameObject.activeSelf)
//					scrollNote.verticalScrollbar.value = 0;
//			}
//		}

		StartCoroutine (ResetGameAfterSomeTime (totalWinner));

//		if (ownTexassPlayer.buyInAmount <= bigBlindAmount &&
		if (!UIManager.Instance.isRegularTournament &&
		    !UIManager.Instance.isSitNGoTournament) {
			if (UIManager.Instance.isRealMoney) {
				if (ownTexassPlayer.totalRealMoney > 0) {
					if (!btnRebuy.gameObject.activeSelf)
						btnAddChips.gameObject.SetActive (true);
				}
			} else {
				if (ownTexassPlayer.totalChips > 0) {
					if (!btnRebuy.gameObject.activeSelf)
						btnAddChips.gameObject.SetActive (true);
				}
			}
		} else {
			if (UIManager.Instance.isRegularTournament) {
				if (UIManager.Instance.isRealMoney) {
					if (ownTexassPlayer.buyInAmount < 0 && ownTexassPlayer.totalRealMoney >= initialBuyinAmountForTournament && canRebuy)
						btnRebuy.gameObject.SetActive (true);
				} else {
					if (ownTexassPlayer.buyInAmount < 0 && ownTexassPlayer.totalChips >= initialBuyinAmountForTournament && canRebuy)
						btnRebuy.gameObject.SetActive (true);
				}
			}
		}
	}

	private void HandleOnPlayerLeftRoom (RoomData roomData, string playerID)
	{
		if (NetworkManager.Instance.joinedRoomID.Equals (roomData.getId ())) {
			if (UIManager.Instance.isRegularTournament ||
			    UIManager.Instance.isSitNGoTournament) {
				TexassPlayer texassPlayer = GetPlayerByID (playerID);
				if (texassPlayer) {
					texassPlayer.playerInfo.Player_Status = (int)PLAYER_STATUS.ABSENT;
					texassPlayer.imgAbsentPlayer.sprite = texassPlayer.spAbsent;
					texassPlayer.imgAbsentPlayer.gameObject.SetActive (true);
					texassPlayer.imgAbsentPlayer.color = Color.yellow;
				}
			} else
				DestroyPlayer (playerID);
		}
	}

	private void HandleOnPlayerConnected (bool success)
	{
		if (!success) 
			UIManager.Instance.connectionFailPanel.gameObject.SetActive (true);
		
	}

	private void HandleOnChatMessageReceived (string playerID, string chatMessage)
	{
		PlayerChat chat = JsonUtility.FromJson<PlayerChat> (chatMessage);

		string msg = txtChat.text;
		txtChat.text = "\n<color=" + APIConstants.HEX_COLOR_LIST_VIEW_HEADER + ">" + playerID + "</color> : " + chat.chatMessage + msg;
		Canvas.ForceUpdateCanvases ();
		scrollChat.verticalScrollbar.value = 1;

		iTween.Stop (chatParent.gameObject);
		iTween.MoveTo (chatParent.gameObject, chatOpenedPosition.position, .5f);
		btnChatTitle.transform.SetAsFirstSibling ();

		StopCoroutine ("CloseChatParentAfterSometime");
		StartCoroutine ("CloseChatParentAfterSometime");
	}

	private void HandleOnPlayerTimoutResponseReceived ()
	{
		timeoutCounter++;

		if (timeoutCounter > Constants.MAX_TIMEOUT_ALLOW) {
//			if (Application.platform == RuntimePlatform.WebGLPlayer) {
//				UIManager.Instance.BackToLobby ();
//			} else {
//				HideBetPanel ();
//
//				gameObject.SetActive (false);
//				UIManager.Instance.roomsPanel.gameObject.SetActive (true);
//			}

			UIManager.Instance.backConfirmationPanel.OnYesButtonTap ();
		}
	}

	private void HandleOnDefaultCardDataReceived (string sender, string defaultCardsInfo)
	{
		if (sender.Equals (Constants.TEXASS_SERVER_NAME)) {
			texassDefaultCards = JsonUtility.FromJson<TexassDefaultCards> (defaultCardsInfo);
		}
	}

	private void HandleOnBlindPlayerResponseReceived (string sender, string blindPlayers)
	{
		BlindPlayer blind = JsonUtility.FromJson<BlindPlayer> (blindPlayers);

		smallBlindAmount = blind.SBAmount;
		bigBlindAmount = blind.SBAmount * 2;

		if (NetworkManager.Instance.playerID.Equals (blind.Player_Dealer)) {
			if (UIManager.Instance.isSitNGoTournament) {
				JSON_Object obj = new JSON_Object ();
				obj.put (Constants.SBAMOUNT_FIELD_SEND_GAME_TYPE, blind.SBAmount);
				obj.put (Constants.GAME_TYPE_FIELD_SEND_GAME_TYPE, (int)TOURNAMENT_GAME_TYPE.SIT_N_GO_TOURNAMENT);
				NetworkManager.Instance.SendRequestToServer (Constants.REQUEST_FOR_BLIND_AMOUNT + obj.ToString ());
			} else if (UIManager.Instance.isRegularTournament) {
				JSON_Object obj = new JSON_Object ();
				obj.put (Constants.SBAMOUNT_FIELD_SEND_GAME_TYPE, blind.SBAmount);
				obj.put (Constants.GAME_TYPE_FIELD_SEND_GAME_TYPE, (int)TOURNAMENT_GAME_TYPE.REGULAR_TOURNAMENT);
				NetworkManager.Instance.SendRequestToServer (Constants.REQUEST_FOR_BLIND_AMOUNT + obj.ToString ());
			}
		}
	}

	private void HandleOnResponseForRebuyReceived (string sender, string canRebuy)
	{
		this.canRebuy = canRebuy.Equals ("0") ? false : true;
	}

	private void HandleOnBreakTimeResponseReceived (string sender, int breakTimer, int totalTables = 1)
	{
		Debug.LogWarning ("-=-=-=-=- " + breakTimer);
		UIManager.Instance.isBreakTime = true;
		UIManager.Instance.breakTimeTillSeconds = breakTimer;
		UIManager.Instance.noTablesAtBreaktime = totalTables == 1;

		if (currentGameStatus != GAME_STATUS.RUNNING && isGameCompleted)
			UIManager.Instance.breakTimePanel.DisplayBreakTimer (breakTimer);
	}

	private void HandleOnRoundComplete (string sender, string roundInfo)
	{
		raisePerRoundCounter = 0;

		GameRound gr = JsonUtility.FromJson<GameRound> (roundInfo);
		TableTotalAmount = gr.Total_Table_Amount;
	}

	private void HandleOnTournamentWinnerInfoReceived (string sender, string tournamentWinnerInfo)
	{
		txtTableMessage.text = "Fetching tournament winners.. Please wait..";
		UIManager.Instance.tournamentWinnerPanel.SetTournamentWinnerDetails (tournamentWinnerInfo);
	}

	private void HandleOnActionHistoryReceived (string sender, string history)
	{
		//Debug.Log (history);

		JSON_Object obj = new JSON_Object (history);
		SetGameStatus (int.Parse (obj.get (Constants.FIELD_ACTION_HISTORY_GAME_STATUS).ToString ()));
		RoundController.GetInstance ().SetTexassGameRound (int.Parse (obj.get (Constants.FIELD_ACTION_HISTORY_ROUND).ToString ()));

		if (currentGameStatus != GAME_STATUS.PAUSED && currentGameStatus != GAME_STATUS.RUNNING && currentGameStatus != GAME_STATUS.RESUMED)
			return;

		int roundStatus = int.Parse (obj.get (Constants.FIELD_ACTION_HISTORY_ROUND_STATUS).ToString ());

		JSONArray actionHistoryArray = obj.getJSONArray (Constants.FIELD_ACTION_HISTORY_TURNS);

		for (int i = 0; i < actionHistoryArray.Count (); i++) {
			ActionResponse ar = JsonUtility.FromJson<ActionResponse> (actionHistoryArray.get (i).ToString ());

			//Debug.LogWarning("player  : " + ar.Player_Name + "\t\tAction  : " + ar.Action + "\t\tBet amount  : " + ar.Bet_Amount + "\t\tRound  : " + RoundController.GetInstance().currentWhoopAssGameRound);

			TexassPlayer p = GetPlayerByID (ar.Player_Name);
			if (p) {
				if (roundStatus != (int)ROUND_STATUS.ACTIVE)
					p.betAmountInPot += ar.Bet_Amount;
			}

			TableTotalAmount = ar.Total_Table_Amount;
			NetworkManager.FireActionResponseReceived (Constants.TEXASS_SERVER_NAME, actionHistoryArray.get (i).ToString ());
			//SetPlayerDetailsOnContinuePlay(ar, roundStatus);

			TableTotalAmount = ar.Total_Table_Amount;
		}

		if (roundStatus == (int)ROUND_STATUS.ACTIVE) {
			//	Generate player cards to continue playing
			GeneratePlayerCardsToContinuePlaying ();

			//SetNextPlayerTurnOnContinuePlay (lastTurnPlayerID);
		}
	}

	private void HandleOnNotRegisteredInTournamentResponseReceived (string sender)
	{
		if (sender.Equals (Constants.TEXASS_SERVER_NAME)) {
			UIManager.Instance.notRegisteredWithTournamentPanel.gameObject.SetActive (true);
		}
	}

	private void HandlePlayerRequestedSitout (string playerID)
	{
		if (playerID == "")
			return;

		TexassPlayer player = GetPlayerByID (playerID);
		if (player != null) {
			txtGameLog.text += "\n" + playerID + Constants.MESSAGE_PLAYER_IS_SITOUT;
			Canvas.ForceUpdateCanvases ();
			if (scrollNote.gameObject.activeSelf)
				scrollNote.verticalScrollbar.value = 0;
		}
	}

	private void HandlePlayerRequestedBackToGame (string playerID)
	{
		if (playerID == "")
			return;
		
		TexassPlayer player = GetPlayerByID (playerID);
		if (player != null) {
			txtGameLog.text += "\n" + playerID + Constants.MESSAGE_PLAYER_IS_BACK_TO_GAME;
			Canvas.ForceUpdateCanvases ();
			if (scrollNote.gameObject.activeSelf)
				scrollNote.verticalScrollbar.value = 0;

			if ((currentGameStatus == GAME_STATUS.STOPPED ||
			    currentGameStatus == GAME_STATUS.FINISHED) &&
			    isGameCompleted &&
			    playerID.Equals (NetworkManager.Instance.playerID)) {
				btnSitoutNextHand.gameObject.SetActive (true);
				sitOutCounter = 0;
			}
		}
	}

	private void HandleOnPlayerEliminated (string playerID)
	{
		txtGameLog.text += "\n" + playerID + Constants.MESSAGE_PLAYER_IS_ELIMINATED;
		Canvas.ForceUpdateCanvases ();
		if (scrollNote.gameObject.activeSelf)
			scrollNote.verticalScrollbar.value = 0;

//		TexassPlayer p = GetPlayerByID (playerID);
//		if (p != null) {
//			p.isEliminated = true;
//			p.txtPlayerName.text = "<color=red>ELIMINATED</color>";
//			p.playerInfo.Player_Status = (int)PLAYER_STATUS.ELIMINATED;
//			p.DestroyCards ();
//		}

		DestroyPlayer (playerID);

		if (playerID.Equals (NetworkManager.Instance.playerID)) {
			UIManager.Instance.playerEliminatedPanel.gameObject.SetActive (true);
		}
	}

	private void HandleOnRestartGameRequestReceived (string sender)
	{
		isGameCompleted = true;
		FireResetData ();

		DestroyAllInstantiatedObjects ();
	}

	private void HandleMaxSitoutResponseReceived (string sender, string playerID)
	{
		if (playerID.Equals (NetworkManager.Instance.playerID)) {
			UIManager.Instance.maxSitoutPanel.gameObject.SetActive (true);
			NetworkManager.Instance.Disconnect ();
		}
	}

	private void HandleRebuyInTournamentResponseReceived (string sender)
	{
		UIManager.Instance.insufficientChipsEliminationPanel.gameObject.SetActive (true);
	}

	private void HandleCollectBlindOnBackToGame (string playerID, double amount)
	{
		TableTotalAmount += amount;
		if (playerID.Equals (NetworkManager.Instance.playerID))
			recentlyBackToGame = false;
	}

	#endregion


	#region PUBLIC_METHODS

	public void OnNoteButtonTap ()
	{
		btnNote.image.sprite = spButtonSelected;
		btnStats.image.sprite = spButtonDeselected;
		btnInfo.image.sprite = spButtonDeselected;

		scrollNote.gameObject.SetActive (true);
		scrollStats.gameObject.SetActive (false);
		scrollInfo.gameObject.SetActive (false);
		scrollNote.verticalScrollbar = leftScrollBar;
		scrollNote.verticalScrollbar.value = 0;

		Color c = new Color ();
		ColorUtility.TryParseHtmlString (APIConstants.HEX_RED_HEADER, out c);
		btnNote.GetComponentInChildren<Text> ().color = c;

		btnStats.GetComponentInChildren<Text> ().color = Color.white;
		btnInfo.GetComponentInChildren<Text> ().color = Color.white;
	}

	public void OnStatsButtonTap ()
	{
		btnNote.image.sprite = spButtonDeselected;
		btnStats.image.sprite = spButtonSelected;
		btnInfo.image.sprite = spButtonDeselected;

		scrollNote.gameObject.SetActive (false);
		scrollStats.gameObject.SetActive (true);
		scrollInfo.gameObject.SetActive (false);
		scrollStats.verticalScrollbar = leftScrollBar;
		scrollStats.verticalScrollbar.value = 0;

		Color c = new Color ();
		ColorUtility.TryParseHtmlString (APIConstants.HEX_RED_HEADER, out c);
		btnStats.GetComponentInChildren<Text> ().color = c;

		btnNote.GetComponentInChildren<Text> ().color = Color.white;
		btnInfo.GetComponentInChildren<Text> ().color = Color.white;
	}

	public void OnInfoButtonTap ()
	{
		btnNote.image.sprite = spButtonDeselected;
		btnStats.image.sprite = spButtonDeselected;
		btnInfo.image.sprite = spButtonSelected;

		scrollNote.gameObject.SetActive (false);
		scrollStats.gameObject.SetActive (false);
		scrollInfo.gameObject.SetActive (true);
		scrollInfo.verticalScrollbar = leftScrollBar;
		scrollInfo.verticalScrollbar.value = 0;

		Color c = new Color ();
		ColorUtility.TryParseHtmlString (APIConstants.HEX_RED_HEADER, out c);
		btnInfo.GetComponentInChildren<Text> ().color = c;

		btnStats.GetComponentInChildren<Text> ().color = Color.white;
		btnNote.GetComponentInChildren<Text> ().color = Color.white;
	}

	public void OnHomeButtonTap ()
	{
		DebugLog.Log ("Home button tapped");
		UIManager.Instance.backConfirmationPanel.gameObject.SetActive (true);
	}

	public void OnBackToGameButtonTap ()
	{
		DebugLog.Log ("Back to Game button tapped");

		btnBackToGame.gameObject.SetActive (false);
		NetworkManager.Instance.SendRequestToServer (Constants.REQUEST_FOR_BACK_TO_GAME);
	}

	public void OnSitOutNextHandButtonTap ()
	{
		DebugLog.Log ("Sit out next hand button tapped");

		btnSitoutNextHand.gameObject.SetActive (false);
//		btnBackToGame.gameObject.SetActive (true);

		NetworkManager.Instance.SendRequestToServer (Constants.REQUEST_FOR_SIT_OUT);
	}

	public void OnFoldButtonTap ()
	{
		DebugLog.Log ("Fold button tapped");

		PlayerAction action = new PlayerAction ();
		action.Action = (int)PLAYER_ACTION.FOLD;
		action.Player_Name = NetworkManager.Instance.playerID;

		ownTexassPlayer.playerInfo.Player_Status = (int)PLAYER_STATUS.FOLDED;

		NetworkManager.Instance.SendPlayerAction (action);

		SoundManager.Instance.PlayWooshSound (Camera.main.transform.position);

		HideBetPanel ();

//		btnSitoutNextHand.gameObject.SetActive (true);
	}

	public void OnAllinButtonTap ()
	{
		DebugLog.Log ("Allin button tapped");

		PlayerAction action = new PlayerAction ();
		action.Action = (int)PLAYER_ACTION.ALLIN;
		action.Bet_Amount = ownTexassPlayer.buyInAmount - ownTexassPlayer.blindAmount;
		action.Player_Name = NetworkManager.Instance.playerID;

		ownTexassPlayer.playerInfo.Player_Status = (int)PLAYER_ACTION.ALLIN;

		NetworkManager.Instance.SendPlayerAction (action);

		SoundManager.Instance.PlayWooshSound (Camera.main.transform.position);

		HideBetPanel ();
	}

	public void OnBetButtonTap ()
	{
		DebugLog.Log ("Bet button tapped");

		PlayerAction action = new PlayerAction ();
		action.Action = (int)PLAYER_ACTION.BET;
		action.Bet_Amount = (float)betSlider.value;
		action.Player_Name = NetworkManager.Instance.playerID;

		NetworkManager.Instance.SendPlayerAction (action);

		SoundManager.Instance.PlayWooshSound (Camera.main.transform.position);

		HideBetPanel ();
	}

	public void OnCheckButtonTap ()
	{
		DebugLog.Log ("Check button tapped");

		PlayerAction action = new PlayerAction ();
		action.Action = (int)PLAYER_ACTION.CHECK;
		action.Player_Name = NetworkManager.Instance.playerID;

		NetworkManager.Instance.SendPlayerAction (action);

		SoundManager.Instance.PlayWooshSound (Camera.main.transform.position);

		HideBetPanel ();
	}

	public void OnCallButtonTap ()
	{
		DebugLog.Log ("Call button tapped");

		double pendingAmount = minimumAmountToBet - ownTexassPlayer.betAmount; 
		if (pendingAmount <= 0) {
			OnCheckButtonTap ();
			return;
		}

		PlayerAction action = new PlayerAction ();
		action.Action = (int)PLAYER_ACTION.CALL;

		action.Bet_Amount = pendingAmount;

		action.Player_Name = NetworkManager.Instance.playerID;

		NetworkManager.Instance.SendPlayerAction (action);

		SoundManager.Instance.PlayWooshSound (Camera.main.transform.position);

		HideBetPanel ();
	}

	public void OnRaiseButtonTap ()
	{
		DebugLog.Log ("Check button tapped");

		PlayerAction action = new PlayerAction ();
		action.Action = (int)PLAYER_ACTION.RAISE;
		if (UIManager.Instance.isLimitGame)
			action.Bet_Amount = (float)betSlider.maxValue;
		else
			action.Bet_Amount = (float)betSlider.value;
		action.Player_Name = NetworkManager.Instance.playerID;

		NetworkManager.Instance.SendPlayerAction (action);

		SoundManager.Instance.PlayWooshSound (Camera.main.transform.position);

		HideBetPanel ();
	}

	public void OnRebuyButtonTap ()
	{
		DebugLog.Log ("Rebuy button tapped");

		//		NetworkManager.Instance.SendRequestToServer (Constants.REQUEST_FOR_REBUY);
		//
		SoundManager.Instance.PlayWooshSound (Camera.main.transform.position);
		//
		btnRebuy.gameObject.SetActive (false);
		if (UIManager.Instance.isRegularTournament) {
			UIManager.Instance.rebuyDetailPanel.DisplayRebuyMessage (initialBuyinAmountForTournament);
		} else {
			rebuyPanel.DisplayRebuyPanel (ownTexassPlayer.totalChips - ownTexassPlayer.buyInAmount);
		}
	}

	public void OnAddChipsButtonTap ()
	{
		DebugLog.Log ("Add Chips button tapped");

		SoundManager.Instance.PlayWooshSound (Camera.main.transform.position);

//		btnAddChips.gameObject.SetActive(false);
		if (UIManager.Instance.isRealMoney)
			rebuyPanel.DisplayRebuyPanel (ownTexassPlayer.totalRealMoney - ownTexassPlayer.buyInAmount);
		else
			rebuyPanel.DisplayRebuyPanel (ownTexassPlayer.totalChips - ownTexassPlayer.buyInAmount);
	}

	public void OnBetSliderValueChanged ()
	{
		txtBetSliderValue.text = Utility.GetAmount ((double)betSlider.value);

		double pendingAmount = minimumAmountToBet - ownTexassPlayer.betAmount;
		if (betSlider.value < pendingAmount)
			betSlider.value = (float)pendingAmount;

		HandleLimitGameButtons (pendingAmount);

		if (betSlider.value == pendingAmount ||
		    betSlider.value == betSlider.minValue) {
			if (!UIManager.Instance.isLimitGame)
				btnRaise.interactable = false;
			btnBet.interactable = false;
		} else {
			RaiseOrBetButton ();
		}

		if (betSlider.value >= ownTexassPlayer.buyInAmount) {
			btnAllin.gameObject.SetActive (true);
			btnRaise.gameObject.SetActive (false);
		} else {
			btnAllin.gameObject.SetActive (false);
			btnRaise.gameObject.SetActive (true);
		}

		if (raisePerRoundCounter >= Constants.MAX_TIME_RAISE_PER_ROUND)
			btnRaise.interactable = false;
	}

	public void OnChatButtonTap ()
	{
		chatTemplatesPanel.gameObject.SetActive (false);
		ifChat.gameObject.SetActive (true);

		ifChat.Select ();
		ifChat.ActivateInputField ();
	}

	public void OnChatTemplatesButtonTap ()
	{
		if (chatTemplatesPanel.gameObject.activeSelf)
			chatTemplatesPanel.gameObject.SetActive (false);
		else
			chatTemplatesPanel.gameObject.SetActive (true);
	}

	public void OnEndChatInput ()
	{
		if (!string.IsNullOrEmpty (ifChat.text) && ifChat.text.Trim ().Length != 0) {
			PlayerChat chat = new PlayerChat ();
			chat.toPlayerID = "";

			//  Cut the message if more than specified length
			string msg = ifChat.text.Trim ();
			if (msg.Length > Constants.MAX_CHAT_MESSAGE_LENGTH) {
				msg = msg.Substring (0, Constants.MAX_CHAT_MESSAGE_LENGTH);
				msg += "...";
			}
			chat.chatMessage = msg;

			NetworkManager.Instance.SendChatMessage (JsonUtility.ToJson (chat));
		}

		ifChat.text = "";

		ifChat.gameObject.SetActive (false);
	}

	public int GetActivePlayers ()
	{
		int activePlayers = 0;

		foreach (TexassPlayer p in allTexassPlayers) {
			if ((p.playerInfo.Player_Status == (int)PLAYER_STATUS.ACTIVE ||
			    p.playerInfo.Player_Status == (int)PLAYER_ACTION.ALLIN ||
			    p.playerInfo.Player_Status == (int)PLAYER_STATUS.ABSENT ||
			    p.playerInfo.Player_Status == (int)PLAYER_STATUS.ELIMINATED) &&
			    p.playerInfo.Player_Status != (int)PLAYER_STATUS.FOLDED &&
			    p.playerInfo.Player_Status != (int)PLAYER_ACTION.TIMEOUT &&
			    p.playerInfo.Player_Status != (int)PLAYER_STATUS.WAITING)
				activePlayers++;
		}

		return activePlayers;
	}

	public TexassPlayer GetPlayerByID (string playerID)
	{
		foreach (TexassPlayer p in allTexassPlayers) {
			if (p.playerID.Equals (playerID))
				return p;
		}

		return null;
	}

	public void OpenBetPanel ()
	{
		///
		gameButtonsCanvasGroup.gameObject.SetActive (true);
		gameButtonsCanvasGroup.interactable = true;

		SetBetButtons ();

		///
		noTurnCheckBoxesCanvasGroup.gameObject.SetActive (false);
		noTurnCheckBoxesCanvasGroup.interactable = false;
	}

	public void DisplayPlayerTotalChips (double chips)
	{
		txtPlayerChips.text = Utility.GetCommaSeperatedAmount (chips, UIManager.Instance.isRealMoney);
	}

	public void HideBetPanel ()
	{
		betSlider.minValue = betSlider.maxValue = 0;
		betSlider.value = 0;
		OnBetSliderValueChanged ();

		gameButtonsCanvasGroup.interactable = false;

		if (ownTexassPlayer != null)
			ownTexassPlayer.HideTurnTimer ();
	}

	public TexassPlayer GetDealerPlayer ()
	{
		foreach (TexassPlayer p in allTexassPlayers) {
			if (p.isDealer)
				return p;
		}

		return null;
	}

	public TexassPlayer GetSmallBlindPlayer ()
	{
		foreach (TexassPlayer p in allTexassPlayers) {
			if (p.isSmallBlind)
				return p;
		}

		return null;
	}

	public void GeneratePlayerCardsToContinuePlaying ()
	{
		foreach (TexassPlayer p in allTexassPlayers) {
			for (int i = 0; i < 2; i++) {
				GameObject card = Instantiate (RoundController.GetInstance ().cardPrefab, i == 0 ? p.card1Position.position : p.card2Position.position, Quaternion.identity) as GameObject;
				card.transform.SetParent (i == 0 ? p.card1Position : p.card2Position);
				card.transform.localScale = Vector3.one;

				if (i == 0) {
					if (p == ownTexassPlayer)
						card.GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (p.playerInfo.Card1);
				} else {
					if (p == ownTexassPlayer)
						card.GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (p.playerInfo.Card2);
				}
			}
		}
	}

	public void OnChatTitleButtonTap ()
	{
		isChatParentOpened = !isChatParentOpened;

		if (isChatParentOpened) {
			iTween.Stop (chatParent.gameObject);
			iTween.MoveTo (chatParent.gameObject, chatOpenedPosition.position, .5f);
			//			btnChatTitle.transform.SetAsFirstSibling ();

			StopCoroutine ("CloseChatParentAfterSometime");
			StartCoroutine ("CloseChatParentAfterSometime");

			chatTemplatesPanel.gameObject.SetActive (true);
		} else {
			iTween.Stop (chatParent.gameObject);
			iTween.MoveTo (chatParent.gameObject, chatClosedPosition.position, .5f);

			chatTemplatesPanel.gameObject.SetActive (false);

			//			btnChatTitle.transform.SetAsLastSibling ();
		}
	}

	public void SortPlayerBySeatIndex ()
	{
		allTexassPlayers = allTexassPlayers.OrderBy (o => o.seatIndex).ToList ();
	}
	public void buttoncallhourly()
	{
		CancelInvoke("GetPerfectClock");
		APIManager.GetInstance ().lastHourAchivement ();
	}
	public void ResetTimerForHourly()
	{
		PlayerPrefs.SetString ("TICK_TIME","");
		InvokeRepeating("GetPerfectClock", 1f, 1f);
	}
	#endregion

	#region PRIVATE_METHODS
	void StoreTimeNow ()
	{
		PlayerPrefs.SetString ("TICK_TIME", System.DateTime.Now.ToBinary ().ToString ());
	}
	void GetPerfectClock ()
	{
		if (PlayerPrefs.GetString ("TICK_TIME").Equals ("")) {
			StoreTimeNow ();
//			Debug.Log ("TICK_TIME = > "+PlayerPrefs.GetString ("TICK_TIME"));
		} else {
//			Debug.Log ("TICK_TIME = >222 "+PlayerPrefs.GetString ("TICK_TIME"));
			long temp = Convert.ToInt64 (PlayerPrefs.GetString ("TICK_TIME"));
			DateTime oldDate = DateTime.FromBinary (temp);
			DateTime oldDateupdate = oldDate.AddHours (1);

			currentDateupdate = System.DateTime.Now;
			TimeSpan difference = currentDateupdate.Subtract (oldDateupdate);

			int _hours = difference.Hours;
			int _minute = difference.Minutes;
			int _second = difference.Seconds;

			if (_hours >= 0 && _minute >= 0 && _second >= 0) {
				// Bonus Availabe btn On
				GameTimer.text = "";
				//CancelInvoke("GetPerfectClock");
				buttoncallhourly ();

			} else {

				if (_hours < 0) {
					_hours = _hours * (-1);
				}
				if (_minute < 0) {
					_minute = _minute * (-1);
				}
				if (_second < 0) {
					_second = _second * (-1);
				}

				// Bonus Text

				GameTimer.text = _hours.ToString ("00") + ":" + _minute.ToString ("00") + ":" + _second.ToString ("00");
			}

		}
	}
	private void SetGameTitle ()
	{
		txtTitle.text = Constants.MESSAGE_TEXASS_GAME_TITLE;
		if (UIManager.Instance.isRegularTournament) {
			if (LoginScript.loginDetails != null)
				txtTitle.text += " " + Constants.MESSAGE_REGULAR_TOURNAMENT_TITLE + " (Table: " + LoginScript.loginDetails.TableNumber + ")";
			else
				txtTitle.text += " " + Constants.MESSAGE_REGULAR_TOURNAMENT_TITLE;
		} else if (UIManager.Instance.isSitNGoTournament)
			txtTitle.text += " " + Constants.MESSAGE_SnG_TOURNAMENT_TITLE;
		if (UIManager.Instance.isLimitGame)
			txtTitle.text += " - " + Constants.MESSAGE_LIMIT_GAME_TITLE;
		else
			txtTitle.text += " - " + Constants.MESSAGE_NO_LIMIT_GAME_TITLE;
	}

	private void HandleLimitGameButtons (double pendingAmount)
	{
		if (UIManager.Instance.isLimitGame)
			betSlider.value = betSlider.maxValue;

//		if (UIManager.Instance.isLimitGame) {
//			if (betSlider.value > pendingAmount)
//				betSlider.value = betSlider.maxValue;
//			if (betSlider.value < betSlider.maxValue)
//				betSlider.value = (long) pendingAmount;
//		}
	}

	private void RaiseOrBetButton ()
	{
		double pendingAmount = minimumAmountToBet - ownTexassPlayer.betAmount;
		if (RoundController.GetInstance ().currentTexassGameRound == TEXASS_GAME_ROUND.PREFLOP) {
			btnRaise.interactable = true;
			btnBet.interactable = false;
		} else {
			if (pendingAmount <= 0) {
				btnRaise.interactable = false;
				btnBet.interactable = true;
			} else {
				btnRaise.interactable = true;
				btnBet.interactable = false;
			}
		}
	}

	private void PlayerIsWaiting ()
	{
		//PlayerAction action = new PlayerAction ();
		//action.Player_Name = currentTurnPlayerID;
		//action.Action = (int)PLAYER_ACTION.ACTION_WAITING_FOR_GAME;

		//NetworkManager.Instance.SendPlayerAction (action);
	}

	private void PlayerIsFolded ()
	{
		//PlayerAction action = new PlayerAction ();
		//action.Player_Name = currentTurnPlayerID;
		//action.Action = (int)PLAYER_ACTION.ACTION_FOLDED;

		//NetworkManager.Instance.SendPlayerAction (action);
	}

	private void PlayerIsTimeout ()
	{
		//PlayerAction action = new PlayerAction ();
		//action.Player_Name = currentTurnPlayerID;
		//action.Action = (int)PLAYER_ACTION.TIMEOUT;

		//NetworkManager.Instance.SendPlayerAction (action);
	}

	private void PlayerIsAllIn ()
	{
		//PlayerAction action = new PlayerAction ();
		//action.Player_Name = currentTurnPlayerID;
		//action.Action = (int)PLAYER_ACTION.ALLIN;

		//NetworkManager.Instance.SendPlayerAction (action);
	}

	private void PlayerIsSitOut ()
	{
		NetworkManager.Instance.SendRequestToServer ("");
	}

	private void DestroyPlayer (string playerID)
	{
		TexassPlayer p = GetPlayerByID (playerID);

		if (p) {
			allTexassPlayers.Remove (p);
//			p.gameObject.SetActive (false);
			Destroy (p.gameObject);
		}
	}

	private void SetGameStatus (int status)
	{
		switch (status) {
		case (int)GAME_STATUS.CARD_DISTRIBUTE:
			currentGameStatus = GAME_STATUS.CARD_DISTRIBUTE;
			break;
		case (int)GAME_STATUS.FINISHED:
			currentGameStatus = GAME_STATUS.FINISHED;
			break;
		case (int)GAME_STATUS.PAUSED:
			currentGameStatus = GAME_STATUS.PAUSED;
			break;
		case (int)GAME_STATUS.RESTART:
			currentGameStatus = GAME_STATUS.RESTART;
			break;
		case (int)GAME_STATUS.RESUMED:
			currentGameStatus = GAME_STATUS.RESUMED;
			break;
		case (int)GAME_STATUS.RUNNING:
			currentGameStatus = GAME_STATUS.RUNNING;
			break;
		case (int)GAME_STATUS.STOPPED:
			currentGameStatus = GAME_STATUS.STOPPED;
			break;
		}
	}

	private void SetBetButtons ()
	{
		betSlider.wholeNumbers = !UIManager.Instance.isRealMoney;

		minimumAmountToBet = RoundController.GetInstance ().GetMinBetAmountInCurrentRound ();
		double pendingAmount = minimumAmountToBet - ownTexassPlayer.betAmount;
		pendingAmount = pendingAmount <= 0 ? 0 : pendingAmount;

		btnFold.interactable = true;
		btnBet.interactable = false;
		btnRaise.interactable = false;

		if (raisePerRoundCounter >= Constants.MAX_TIME_RAISE_PER_ROUND) {
			betSlider.interactable = false;
		} else {
			betSlider.interactable = true;
		}

		betSlider.minValue = 0;
		betSlider.value = (float)pendingAmount;

		if (UIManager.Instance.isLimitGame) {
			if (RoundController.GetInstance ().currentTexassGameRound == TEXASS_GAME_ROUND.PREFLOP ||
			    RoundController.GetInstance ().currentTexassGameRound == TEXASS_GAME_ROUND.FLOP) {
				double maxBet = pendingAmount + smallBlindAmount;
				betSlider.maxValue = maxBet < ownTexassPlayer.buyInAmount ? (float)maxBet : (float)ownTexassPlayer.buyInAmount;
			} else if (RoundController.GetInstance ().currentTexassGameRound == TEXASS_GAME_ROUND.TURN ||
			           RoundController.GetInstance ().currentTexassGameRound == TEXASS_GAME_ROUND.RIVER) {
				double maxBet = pendingAmount + bigBlindAmount;
				betSlider.maxValue = maxBet < ownTexassPlayer.buyInAmount ? (float)maxBet : (float)ownTexassPlayer.buyInAmount;
			}

			//  To display bet button directly if not bet yet
			//            if (pendingAmount <= 0)
			betSlider.value = betSlider.maxValue;
		} else {
			betSlider.maxValue = (float)ownTexassPlayer.buyInAmount;
		}


		//	Check button
		if (pendingAmount <= 0)
			btnCheck.interactable = true;
		else
			btnCheck.interactable = false;


		//	Call button
		if (pendingAmount <= 0 || pendingAmount >= ownTexassPlayer.buyInAmount)
			btnCall.interactable = false;
		else
			btnCall.interactable = true;


		//	Allin Button
		btnRebuy.gameObject.SetActive (false);
		if (pendingAmount >= ownTexassPlayer.buyInAmount) {
			btnRaise.gameObject.SetActive (false);
			btnAllin.gameObject.SetActive (true);

			if (UIManager.Instance.isRegularTournament && canRebuy) {
//                btnRebuy.gameObject.SetActive(true);
			}
		} else {
			btnAllin.gameObject.SetActive (false);
		}

		betSlider.value = (float)pendingAmount;
		OnBetSliderValueChanged ();

		///
		NoTurnPanel noTurnPanel = noTurnCheckBoxesCanvasGroup.GetComponent<NoTurnPanel> ();
		OFF_TURN_ACTION offTurnAction = noTurnPanel.GetSelectedAction ();
		if (offTurnAction != OFF_TURN_ACTION.NONE) {
			if (offTurnAction == OFF_TURN_ACTION.FOLD)
				OnFoldButtonTap ();
			else if (offTurnAction == OFF_TURN_ACTION.CALL_ANY &&
			         noTurnPanel.cgCallAny.interactable)
				OnCallAnyAutoButton ();
			else if (offTurnAction == OFF_TURN_ACTION.CALL &&
			         noTurnPanel.cgCall.interactable)
				OnCallAnyAutoButton ();
			else if (offTurnAction == OFF_TURN_ACTION.CHECK &&
			         pendingAmount <= 0)
				OnCheckButtonTap ();
		}
	}

	private void ResetTableCardsToInitialPosition ()
	{
		for (int i = 0; i < defaultCardsList.Count; i++) {
			defaultCardsList [i].transform.position = defaultCardsPositionList [i].position;
		}
	}

	private void HandleWaitingPlayer (PlayerInfo playerInfo)
	{
		if (playerInfo.Player_Name.Equals (NetworkManager.Instance.playerID)) {
			RoundController.GetInstance ().currentTexassGameRound = GetTexassGameRound (playerInfo.Current_Round);

			if (playerInfo.Game_Status == (int)GAME_STATUS.RUNNING ||
			    playerInfo.Game_Status == (int)GAME_STATUS.CARD_DISTRIBUTE) {
				if (playerInfo.Player_Status == (int)PLAYER_STATUS.WAITING ||
				    playerInfo.Player_Status == (int)PLAYER_ACTION.ACTION_WAITING_FOR_GAME ||
				    playerInfo.Player_Status == (int)PLAYER_STATUS.ACTIVE ||
				    playerInfo.Player_Status == (int)PLAYER_STATUS.FOLDED ||
				    playerInfo.Player_Status == (int)PLAYER_ACTION.ACTION_FOLDED) {
					GeneratePlayerCardsForWaitingPlayer ();
					RoundController.GetInstance ().GenerateTexassTableCardsForWaitingPlayer ();
				}
			}
		}
	}

	private TEXASS_GAME_ROUND GetTexassGameRound (int round)
	{
		switch (round) {
		case (int)TEXASS_GAME_ROUND.RIVER:
			return TEXASS_GAME_ROUND.RIVER;
		case (int)TEXASS_GAME_ROUND.TURN:
			return TEXASS_GAME_ROUND.TURN;
		case (int)TEXASS_GAME_ROUND.FLOP:
			return TEXASS_GAME_ROUND.FLOP;
		case (int)TEXASS_GAME_ROUND.PREFLOP:
			return TEXASS_GAME_ROUND.PREFLOP;
		}
		return TEXASS_GAME_ROUND.PREFLOP;
	}

	private void GeneratePlayerCardsForWaitingPlayer ()
	{
		RoundController.GetInstance ().GenerateTexassPlayerCardsForWaitingPlayer ();
	}

	private void GenerateDealerFlopCardsForWaitingPlayer ()
	{
		Debug.Log ("--> GenerateDealerFlopCardsForWaitingPlayer <--");
		RoundController.GetInstance ().GenerateDealerFlopCardsForWaitingPlayer ();
	}

	/// <summary>
	/// Sets the winning rank
	/// </summary>
	private string GetWinningRank (int winningRank)
	{
		switch (winningRank) {
		case (int) WINNING_RANK.FLUSH:
			return "FLUSH";
		case (int) WINNING_RANK.FOUR_OF_A_KIND:
			return "FOUR OF A KIND";
		case (int) WINNING_RANK.FULL_HOUSE:
			return "FULL HOUSE";
		case (int) WINNING_RANK.HIGH_CARD:
			return "HIGH CARD";
		case (int) WINNING_RANK.ONE_PAIR:
			return "ONE PAIR";
		case (int) WINNING_RANK.ROYAL_FLUSH:
			return "ROYAL FLUSH";
		case (int) WINNING_RANK.STRAIGHT:
			return "STRAIGHT";
		case (int) WINNING_RANK.STRAIGHT_FLUSH:
			return "STRAIGHT FLUSH";
		case (int) WINNING_RANK.THREE_OF_A_KIND:
			return "THREE OF A KIND";
		case (int) WINNING_RANK.TWO_PAIR:
			return "TWO PAIR";
		}
		return "";
	}

	private int CalculateTotalChipsToGenerate (int chipAmount)
	{
		int d = (GetActivePlayers () * 1000);
		d = d < 1 ? 0 : d;

		int chip = (chipAmount * 10) / d;
		if (chip < Constants.MINIMUM_CHIP_DISPLAY)
			chip = Constants.MINIMUM_CHIP_DISPLAY;
		else if (chip > Constants.MAXIMUM_CHIPS_DISPLAY)
			chip = Constants.MAXIMUM_CHIPS_DISPLAY;
		return chip;
	}

	private float GetYPos (int i)
	{
		return tableChipObject.transform.position.y + (i * 3f);
	}

	private float GetRedChipYPos (int i)
	{
		return redInitialChip.position.y + (i * 1f);
	}

	private float GetGreenChipYPos (int i)
	{
		return greenInitialChip.position.y + (i * 1f);
	}

	private float GetBlueChipYPos (int i)
	{
		return blueInitialChip.position.y + (i * 1f);
	}

	private void DestroyAllChips ()
	{
		foreach (Chip c in GetComponentsInChildren<Chip>()) {
			Destroy (c.gameObject);
		}
		totalChipObject = 0;
		totalRedChipObject = totalGreenChipObject = totalBlueChipObject = 0;
		chipsDisplayedList = new List<GameObject> ();
		redChipsDisplayedList = new List<GameObject> ();
		greenChipsDisplayedList = new List<GameObject> ();
		blueChipsDisplayedList = new List<GameObject> ();
	}

	private void CheckForLowChips ()
	{
//		if (ownTexassPlayer.buyInAmount <= bigBlindAmount &&
//		    !UIManager.Instance.isRegularTournament &&
//		    !UIManager.Instance.isSitNGoTournament) {
//			if (ownTexassPlayer.totalChips <= bigBlindAmount) {
////				NetworkManager.Instance.Disconnect ();
//				UIManager.Instance.noEnoughChipsPanel.gameObject.SetActive (true);
//			} else {
//				if (!btnRebuy.gameObject.activeSelf)
//					btnAddChips.gameObject.SetActive (true);
//			}
//		}

		if (ownTexassPlayer.buyInAmount <= 0 &&
		    !UIManager.Instance.isRegularTournament &&
		    !UIManager.Instance.isSitNGoTournament) {
			if (UIManager.Instance.isRealMoney)
				UIManager.Instance.DisplayNotEnoughChipsPanel (ownTexassPlayer.totalRealMoney > 0);
			else
				UIManager.Instance.DisplayNotEnoughChipsPanel (ownTexassPlayer.totalChips > 0);
		}
	}

	private void CheckForMaxSitout ()
	{
		
	}

	private void DestroyAllPlayers ()
	{
		foreach (TexassPlayer wp in allTexassPlayers) {
			wp.gameObject.SetActive (false);
		}
		allTexassPlayers = new List<TexassPlayer> ();
	}

	private void CentralizeOwnPlayer ()
	{
		int posToMove = GetOwnPlayerPositionToMoveCenter ();

		if (posToMove >= Constants.TEXASS_GAME_MAX_PLAYERS) {
			return;
		}

		for (int i = 0; i < playerSeats.Count; i++) {
			int posIndex = i + posToMove;

			if (posIndex > Constants.TEXASS_GAME_MAX_PLAYERS - 1)
				posIndex = posIndex - Constants.TEXASS_GAME_MAX_PLAYERS;

			playerSeats [i].transform.SetParent (playerPositions [posIndex]);

			playerSeats [i].position = playerPositions [posIndex].position;
		}
	}

	private int GetOwnPlayerPositionToMoveCenter ()
	{
		int movePos = Constants.TEXASS_GAME_MAX_PLAYERS - ownTexassPlayer.playerInfo.Player_Position;
		return movePos;
	}

	/// <summary>
	/// Resets player seats to initial positions.
	/// </summary>
	private void ResetPlayerSeats ()
	{
		for (int i = 0; i < playerPositions.Count; i++) {
			playerSeats [i].transform.SetParent (playerPositions [i]);
			playerSeats [i].position = playerPositions [i].position;
		}
	}

	private void SetBetParentPositions ()
	{
//		foreach (Transform go in players) {
//			TexassPlayer p = go.GetComponent<TexassPlayer> ();
//			p.betAmountParentObj.transform.position = p.betAmountPositionsList [playerPositions.IndexOf (go.parent.parent)].position;
//			p.txtBetAmountPosition.position = p.betChipsPositionList [playerPositions.IndexOf (go.parent.parent)].position;
//		}

		foreach (TexassPlayer p in allTexassPlayers) {
			int index = playerPositions.IndexOf (p.transform.parent.parent);

			p.betAmountParentObj.transform.position = p.betAmountPositionsList [index].position;
			p.txtBetAmountPosition.position = p.betChipsPositionList [index].position;

			p.dealerObject.transform.position = new Vector3 (p.dealerObject.transform.position.x, p.betAmountPositionsList [index].position.y, 0);
		}
	}

	private void OnCallAnyAutoButton ()
	{
		DebugLog.Log ("Call button tapped");

		PlayerAction action = new PlayerAction ();
		action.Action = (int)PLAYER_ACTION.CALL;

		double pendingAmount = minimumAmountToBet - ownTexassPlayer.betAmount; 
		if (pendingAmount <= 0) {
			OnCheckButtonTap ();
			return;
		} else if (pendingAmount >= ownTexassPlayer.buyInAmount) {
			pendingAmount = ownTexassPlayer.buyInAmount;
			action.Action = (int)PLAYER_ACTION.ALLIN;
		}

		action.Bet_Amount = pendingAmount;
		action.Player_Name = NetworkManager.Instance.playerID;

		NetworkManager.Instance.SendPlayerAction (action);

		SoundManager.Instance.PlayWooshSound (Camera.main.transform.position);
		HideBetPanel ();
	}

	private void DestroyAllInstantiatedObjects ()
	{
		foreach (Transform child in objectsGenerateHere) {
			Destroy (child.gameObject);
		}
	}

	#endregion

	#region COROUTINES

	private IEnumerator ResetGameAfterSomeTime (int totalWinner)
	{
		int waitTime = (resetGameAfterTimer * totalWinner) - 2;
		yield return new WaitForSeconds (waitTime);

		CheckForLowChips ();
		CheckForMaxSitout ();

		FireResetData ();
		DestroyAllInstantiatedObjects ();
		TableTotalAmount = 0;

		UIManager.Instance.winnerAnimationPanel.gameObject.SetActive (false);

		if (UIManager.Instance.breakingTablePanel.willBreakTable) {
			gameObject.SetActive (false);
			UIManager.Instance.breakingTablePanel.gameObject.SetActive (true);
		} else {
			if (UIManager.Instance.isBreakTime)// && ownTexassPlayer.totalChips >= bigBlindAmount)
				UIManager.Instance.breakTimePanel.DisplayBreakTimer (UIManager.Instance.breakTimeTillSeconds);
		}

		if (txtGameLog.text.Length > 50000) {
			txtGameLog.text = "";
			Canvas.ForceUpdateCanvases ();
			if (scrollNote.gameObject.activeSelf)
				scrollNote.verticalScrollbar.value = 0;
		}
	}

	private IEnumerator DisplayWinnerAnimation (List<GameWinner> winnerList)
	{
		//List<GameWinner> gameWinnerList = new List<GameWinner> ();

		yield return new WaitForSeconds (1f);

		for (int i = 0; i < winnerList.Count; i++) {

			TexassPlayer p = GetPlayerByID (winnerList [i].Winner_Name);
			if (p) {
				p.winnerRankParentObject.SetActive (true);

//				if (p == ownTexassPlayer) {
//					rebuyPanel.gameObject.SetActive (false);
//					btnRebuy.gameObject.SetActive (false);
//					btnAddChips.gameObject.SetActive (false);
//				}

				if (GetActivePlayers () > 1 || TournamentWinnerPanel.isTournamentWinnersDeclared) {
					p.HighlightWinnerBestCards (winnerList [i]);
					p.txtWinningRank.color = Color.white;
					p.txtWinningRank.text = GetWinningRank (winnerList [i].Winner_Rank) + "\n" + Utility.GetAmount (winnerList [i].Winning_Amount);

					p.txtPlayerName.GetComponent<CanvasGroup> ().ignoreParentGroups = false;
					for (int j = 0; j < 5; j++) {
						yield return new WaitForSeconds (.175f);
						p.GetComponent<CanvasGroup> ().alpha = 0;
						yield return new WaitForSeconds (.175f);
						p.GetComponent<CanvasGroup> ().alpha = 1f;
					}
					p.txtPlayerName.GetComponent<CanvasGroup> ().ignoreParentGroups = true;

					if (p.playerID.Equals (NetworkManager.Instance.playerID))
						UIManager.Instance.winnerAnimationPanel.gameObject.SetActive (true);

					p.playerAnimator.SetTrigger (Constants.PLAYER_WINNER_ANIMATION_TRIGGER);

					txtGameLog.text += "\nWinner -> <color=" + APIConstants.HEX_COLOR_LIST_VIEW_HEADER + ">" + p.playerID + " -> " + GetWinningRank (winnerList [i].Winner_Rank) + "</color> -> " + Utility.GetAmount (winnerList [i].Winning_Amount);
					Canvas.ForceUpdateCanvases ();
					if (scrollNote.gameObject.activeSelf)
						scrollNote.verticalScrollbar.value = 0;

					StartCoroutine (MoveChipToWinnerPlayer (p.txtTotalChips.transform.position));
					TableTotalAmount -= winnerList [i].Winning_Amount;
					p.buyInAmount = winnerList [i].winner_balance;
					p.DisplayTotalChips ();

					yield return new WaitForSeconds (1f);
					if (winnerList [i].Rake_Amount + winnerList [i].AffiliateCommission > 0) {
						TableTotalAmount -= (winnerList [i].Rake_Amount + winnerList [i].AffiliateCommission);
						StartCoroutine (MoveRakeAmountToWA (winnerList [i].Rake_Amount + winnerList [i].AffiliateCommission));
					}

					yield return new WaitForSeconds (3f);
					p.ResetCardsToInitialPosition ();
					ResetTableCardsToInitialPosition ();
					UIManager.Instance.winnerAnimationPanel.gameObject.SetActive (false);
				} else {
					p.txtWinningRank.color = Color.white;
					p.txtWinningRank.text = Utility.GetAmount (winnerList [i].Winning_Amount);
					p.buyInAmount = winnerList [i].winner_balance;

					yield return new WaitForSeconds (2f);
					StartCoroutine (MoveChipToWinnerPlayer (p.txtTotalChips.transform.position));
					TableTotalAmount -= winnerList [i].Winning_Amount;

					txtGameLog.text += "\nWinner -> <color=" + APIConstants.HEX_COLOR_LIST_VIEW_HEADER + ">" + p.playerID + " -> " + GetWinningRank (winnerList [i].Winner_Rank) + "</color> -> " + Utility.GetAmount (winnerList [i].Winning_Amount);
					Canvas.ForceUpdateCanvases ();
					if (scrollNote.gameObject.activeSelf)
						scrollNote.verticalScrollbar.value = 0;

					yield return new WaitForSeconds (1f);
					if (winnerList [i].Rake_Amount + winnerList [i].AffiliateCommission > 0) {
						TableTotalAmount -= (winnerList [i].Rake_Amount + winnerList [i].AffiliateCommission);
						StartCoroutine (MoveRakeAmountToWA (winnerList [i].Rake_Amount + winnerList [i].AffiliateCommission));
					}
				}
			}
		}

		if (ownTexassPlayer.buyInAmount <= 0 &&
		    ownTexassPlayer.totalChips > 0 &&
		    canRebuy) {
			btnRebuy.gameObject.SetActive (true);

			if (UIManager.Instance.isRegularTournament) {
				if (UIManager.Instance.isRealMoney)
					btnRebuy.gameObject.SetActive (ownTexassPlayer.totalRealMoney >= initialBuyinAmountForTournament && canRebuy);
				else
					btnRebuy.gameObject.SetActive (ownTexassPlayer.totalChips >= initialBuyinAmountForTournament && canRebuy);
			}
		} else
			btnRebuy.gameObject.SetActive (false);
	}

	private IEnumerator MoveRakeAmountToWA (double rakeAmount)
	{
		GameObject rakeAmountGO = Instantiate (rakeAmountPrefab, txtTableTotalAmount.transform.position, Quaternion.identity) as GameObject;
		rakeAmountGO.transform.GetChild (0).GetComponent<Text> ().text = Utility.GetAmount (rakeAmount);
		rakeAmountGO.transform.SetParent (objectsGenerateHere);
		rakeAmountGO.transform.localScale = Vector3.one;

		Hashtable ht = new Hashtable ();
		ht.Add ("time", .75f);
		ht.Add ("easetype", iTween.EaseType.spring);
		ht.Add ("position", txtTableTotalAmount.transform.position + Vector3.up * 10f);
		iTween.MoveTo (rakeAmountGO, ht);
		Destroy (rakeAmountGO, 1f);

		yield return new WaitForSeconds (1f);

		GameObject chip = Instantiate (tableChipObject, tableChipObject.transform.position, Quaternion.identity) as GameObject;
		chip.SetActive (true);
		chip.transform.SetParent (objectsGenerateHere);
		chip.transform.localScale = Vector3.one;
		chip.GetComponent<Image> ().SetNativeSize ();

		SoundManager.Instance.PlayChipsSound (Camera.main.transform.position);

		float i = 0;
		while (i < 1) {
			i += 2f * Time.deltaTime;

			chip.transform.position = Vector3.Lerp (tableChipObject.transform.position, imgWALogo.transform.position, i);
			yield return 0;
		}

		Destroy (chip, .5f);
	}

	private IEnumerator MoveChipToWinnerPlayer (Vector3 toPos)
	{
		GameObject chip = Instantiate (tableChipObject, tableChipObject.transform.position, Quaternion.identity) as GameObject;
		chip.SetActive (true);
		chip.transform.SetParent (objectsGenerateHere);
		chip.transform.localScale = Vector3.one;
		chip.GetComponent<Image> ().SetNativeSize ();

		SoundManager.Instance.PlayChipsSound (Camera.main.transform.position);

		float i = 0;
		while (i < 1) {
			i += 2f * Time.deltaTime;

			chip.transform.position = Vector3.Lerp (tableChipObject.transform.position, toPos, i);
			yield return 0;
		}

		Destroy (chip, .5f);
	}

	private IEnumerator DisplayTableChips ()
	{
		//		totalChipObject = 0;
		int chipsAmount = (int)(TableTotalAmount);

		if (chipsAmount > 0) {
			int totalChipObjectToGenerate = CalculateTotalChipsToGenerate (chipsAmount);
			totalChipObjectToGenerate -= totalChipObject;

			if (totalChipObjectToGenerate > chipsDisplayedList.Count) {
				for (int i = 0; i < totalChipObjectToGenerate; i++) {
//                    GameObject chip = Instantiate(chipPrefab, new Vector2(tableChipObject.transform.position.x, GetYPos(++totalChipObject)), Quaternion.identity) as GameObject;
//                    chipsDisplayedList.Add(chip);
//                    chip.transform.SetParent(transform);
//                    chip.transform.localScale = Vector3.one;

					yield return 0;

					GameObject redChip = Instantiate (redChipPrefab, new Vector2 (redInitialChip.transform.position.x, GetRedChipYPos (++totalRedChipObject)), Quaternion.identity) as GameObject;
					GameObject greenChip = Instantiate (greenChipPrefab, new Vector2 (greenInitialChip.transform.position.x, GetGreenChipYPos (++totalGreenChipObject)), Quaternion.identity) as GameObject;
					GameObject blueChip = Instantiate (blueChipPrefab, new Vector2 (blueInitialChip.transform.position.x, GetBlueChipYPos (++totalBlueChipObject)), Quaternion.identity) as GameObject;

					redChipsDisplayedList.Add (redChip);
					greenChipsDisplayedList.Add (greenChip);
					blueChipsDisplayedList.Add (blueChip);

					//-------------------------------
					chipsDisplayedList.Add (redChip);
					totalChipObject++;
					//-------------------------------

					redChip.transform.SetParent (redInitialChip);
					greenChip.transform.SetParent (greenInitialChip);
					blueChip.transform.SetParent (blueInitialChip);

					redChip.transform.localScale = Vector3.one;
					greenChip.transform.localScale = Vector3.one;
					blueChip.transform.localScale = Vector3.one;
				}
			}
		}
	}

	private IEnumerator CloseChatParentAfterSometime ()
	{
		yield return new WaitForSeconds (10f);

		iTween.Stop (chatParent.gameObject);
		iTween.MoveTo (chatParent.gameObject, chatClosedPosition.position, .5f);
		btnChatTitle.transform.SetAsLastSibling ();

		chatTemplatesPanel.gameObject.SetActive (false);
	}

	#endregion

	#region GETTER_SETTER

	public double TableTotalAmount {
		get { 
			return tableTotalAmount;
		}
		set {
			tableTotalAmount = value;
			if (System.Math.Round (value, 2) <= 0) {
				//tableChipObject.transform.parent.gameObject.SetActive(false);
				txtTableTotalAmount.text = "";
				imgPotAmountBG.gameObject.SetActive (false);
				DestroyAllChips ();
			} else {
				//tableChipObject.transform.parent.gameObject.SetActive(true);
				txtTableTotalAmount.text = Utility.GetAmount (value);
				imgPotAmountBG.gameObject.SetActive (true);
				StartCoroutine (DisplayTableChips ());
			}
		}
	}

	 
	#endregion
}