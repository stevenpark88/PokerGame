using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using com.shephertz.app42.gaming.multiplayer.client.events;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class WhoopAssGame : MonoBehaviour
{
	#region PUBLIC_VARIABLES

	public Text txtTableMessage;

	public GAME_STATUS currentGameStatus;

	public Text txtTitle;

	public bool canRebuy = true;

	public Button btnNote;
	public Button btnStats;
	public Button btnInfo;

	public GameObject whoopassPlayerPrefab;

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

	public WhoopAssPlayer ownWhoopAssPlayer;
	public List<WhoopAssPlayer> allWhoopAssPlayers;

	private double tableTotalAmount;
	private double waPotAmount;
	public Text txtTableTotalAmount;
	public Text txtWAPotAmount;

	public Text txtPlayerChips;

	public Button btnCheck;
	public Button btnRaise;
	public Button btnFold;
	public Button btnCall;
	public Button btnBet;
	public Button btnAllin;
	public Button btnRebuy;
	public Button btnAddChips;
	public Button btnSitOutNextHand;
	public Button btnBackToGame;

	public Slider betSlider;
	public Text txtBetSliderValue;

	public InputField ifChat;

	public CanvasGroup gameButtonsCanvasGroup;
	public CanvasGroup noTurnCheckBoxesCanvasGroup;

	public List<Transform> defaultTableCardsList;

	public DefaultCards whoopAssGameDefaultCards;

	public bool isGameCompleted = true;

	public string currentTurnPlayerID;

	public double minimumAmountToBet = 0;

	public int raisePerRoundCounter = 0;

	public double smallBlindAmount;
	public double bigBlindAmount;

	public GameObject tableChipObject;
	public GameObject initialChipPosition;

	public GameObject chipPrefab;

	public Text txtMessage;
	public int resetGameAfterTimer = 10;

	public Image imgPotAmountBG;
	public Image imgWAPotAmountBG;

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

	public Transform objectsGenerateHere;

	public Text GameTimer;
	public int GamePlaytime =1;
	DateTime currentDateupdate;

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

	private int totalRedChips = 0;
	private int totalGreenChips = 0;
	private int totalBlueChips = 0;

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

	public static WhoopAssGame Instance;


	#region UNITY_CALLBACKS

	void Awake ()
	{
		Instance = this;

		allWhoopAssPlayers = new List<WhoopAssPlayer> ();

		btnFold.interactable = false;

		chipsDisplayedList = new List<GameObject> ();
		redChipsDisplayedList = new List<GameObject> ();
		greenChipsDisplayedList = new List<GameObject> ();
		blueChipsDisplayedList = new List<GameObject> ();

		btnSitOutNextHand.gameObject.SetActive (false);
		btnBackToGame.gameObject.SetActive (false);
	}

	void OnEnable ()
	{
		timeoutCounter = 0;
		txtMessage.text = "";
		initialBuyinAmountForTournament = 0;
		if (LoginScript.loginDetails != null)
			initialBuyinAmountForTournament = double.Parse (LoginScript.loginDetails.buyin);

		SetGameTitle ();

		RoundController.isBlindAmountCollected = false;
		recentlyBackToGame = false;

		NetworkManager.Instance.JoinRoom (NetworkManager.Instance.joinedRoomID);

		NetworkManager.onPlayerInfoReceived += HandleOnPlayerInfoReceived;
		NetworkManager.onGameStartedByPlayer += HandleOnGameStartedByPlayer;
		NetworkManager.onMoveCompletedByPlayer += HandleOnMoveCompletedByPlayer;
		NetworkManager.onWinnerInfoReceived += HandleOnWinnerInfoReceived;
		NetworkManager.onPlayerLeftRoom += HandleOnPlayerLeftRoom;
		NetworkManager.onPlayerConnected += HandleOnPlayerConnected;
		NetworkManager.onChatMessageReceived += HandleOnChatMessageReceived;
		NetworkManager.onPlayerTimeoutResponseReceived += HandleOnPlayerTimoutResponseReceived;
		NetworkManager.onRoundComplete += HandleOnRoundComplete;
		NetworkManager.onBlindPlayerResponseReceived += HandleOnBlindPlayerResponseReceived;
		NetworkManager.onBreakTimeResponseReceived += HandleOnBreakTimeResponseReceived;
		NetworkManager.onResponseForRebuyReceived += HandleOnResponseForRebuyReceived;
		NetworkManager.onTournamentWinnerInfoReceived += HandleOnTournamentWinnerInfoReceived;
		NetworkManager.onNotRegisteredInTournamentResponseReceived += HandleOnNotRegisteredInTournamentResponseReceived;
		NetworkManager.onLeaveRoomSuccess += HandleOnLeaveRoomSuccess;
		NetworkManager.onActionHistoryReceived += HandleOnActionHistoryReceived;
		NetworkManager.playerRequestedSitout += HandlePlayerRequestedSitout;
		NetworkManager.playerRequestedBackToGame += HandlePlayerRequestedBackToGame;
		NetworkManager.playerEliminated += HandleOnPlayerEliminated;
		NetworkManager.onRestartGameRequestReceived += HandleOnRestartGameRequestReceived;
		NetworkManager.maxSitoutResponseReceived += HandleMaxSitoutResponseReceived;
		NetworkManager.rebuyInTournamentResponseReceived += HandleRebuyInTournamentResponseReceived;
		RoundController.cardDistributionFinished += HandleCardDistributionFinished;
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
		NetworkManager.onRoundComplete -= HandleOnRoundComplete;
		NetworkManager.onBlindPlayerResponseReceived -= HandleOnBlindPlayerResponseReceived;
		NetworkManager.onBreakTimeResponseReceived -= HandleOnBreakTimeResponseReceived;
		NetworkManager.onResponseForRebuyReceived -= HandleOnResponseForRebuyReceived;
		NetworkManager.onTournamentWinnerInfoReceived -= HandleOnTournamentWinnerInfoReceived;
		NetworkManager.onNotRegisteredInTournamentResponseReceived -= HandleOnNotRegisteredInTournamentResponseReceived;
		NetworkManager.onLeaveRoomSuccess -= HandleOnLeaveRoomSuccess;
		NetworkManager.onActionHistoryReceived -= HandleOnActionHistoryReceived;
		NetworkManager.playerRequestedSitout -= HandlePlayerRequestedSitout;
		NetworkManager.playerRequestedBackToGame -= HandlePlayerRequestedBackToGame;
		NetworkManager.playerEliminated -= HandleOnPlayerEliminated;
		NetworkManager.onRestartGameRequestReceived -= HandleOnRestartGameRequestReceived;
		NetworkManager.maxSitoutResponseReceived -= HandleMaxSitoutResponseReceived;
		NetworkManager.rebuyInTournamentResponseReceived -= HandleRebuyInTournamentResponseReceived;
		RoundController.cardDistributionFinished -= HandleCardDistributionFinished;
		NetworkManager.collectBlindOnBackToGame -= HandleCollectBlindOnBackToGame;

		UIManager.Instance.isRealMoney = false;

		NetworkManager.Instance.LeaveGame ();
		DestroyAllPlayers ();

		ResetPlayerSeats ();
		FireResetData ();

		DestroyAllInstantiatedObjects ();
		CancelInvoke("GetPerfectClock");

	}

	#endregion

	#region DELEGATE_CALLBACKS

	private void HandleOnPlayerInfoReceived (string sender, string info)
	{
		txtTableMessage.text = "";

		if (ownWhoopAssPlayer == null)
			UIManager.Instance.breakTimePanel.gameObject.SetActive (false);

//		Debug.Log ("CD : Player Info : "+info);
		btnSitOutNextHand.gameObject.SetActive (false);
		btnAddChips.gameObject.SetActive (false);

		PlayerInfo playerInfo = JsonUtility.FromJson<PlayerInfo> (info);
		SetGameStatus (playerInfo.Game_Status);
		resetGameAfterTimer = playerInfo.Restart_Time;

		//  Need to decrease one index. Seat index from server is started from 1.
		playerInfo.Player_Position--;

		WhoopAssPlayer player = GetPlayerByID (playerInfo.Player_Name);
		if (player) {
			player.playerID = playerInfo.Player_Name;
			player.buyInAmount = playerInfo.Player_BuyIn_Chips;
			player.card1 = playerInfo.Card1;
			player.card2 = playerInfo.Card2;
			player.playerInfo = playerInfo;
			player.seatIndex = playerInfo.Player_Position;
			player.totalChips = playerInfo.Player_Total_Play_Chips;
			player.totalRealMoney = playerInfo.Player_Total_Real_Chips;
			player.isEliminated = playerInfo.Player_Status.Equals (PLAYER_STATUS.ELIMINATED);

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

			if (playerInfo.Player_Status == (int)PLAYER_STATUS.ELIMINATED) {
				player.txtPlayerName.text = "<color=red>ELIMINATED</color>";
				player.DestroyCards ();
			}

			if (player.buyInAmount <= 0)
				player.GetComponent<CanvasGroup> ().alpha = .4f;
			else
				player.GetComponent<CanvasGroup> ().alpha = 1f;

			player.DisplayTotalChips ();
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

		GameObject obj = Instantiate (whoopassPlayerPrefab, playerSeats [playerInfo.Player_Position].position, Quaternion.identity) as GameObject;
//		GameObject obj = players[playerInfo.Player_Position].gameObject;
//		obj.SetActive (true);
		WhoopAssPlayer whoopAssPlayer = obj.GetComponent<WhoopAssPlayer> ();

		whoopAssPlayer.playerID = playerInfo.Player_Name;
		whoopAssPlayer.buyInAmount = playerInfo.Player_BuyIn_Chips;
		whoopAssPlayer.card1 = playerInfo.Card1;
		whoopAssPlayer.card2 = playerInfo.Card2;
		whoopAssPlayer.playerInfo = playerInfo;
		whoopAssPlayer.seatIndex = playerInfo.Player_Position;
		whoopAssPlayer.totalChips = playerInfo.Player_Total_Play_Chips;
		whoopAssPlayer.totalRealMoney = playerInfo.Player_Total_Real_Chips;
		whoopAssPlayer.isEliminated = playerInfo.Player_Status.Equals (PLAYER_STATUS.ELIMINATED);
		whoopAssPlayer.GetComponent<CanvasGroup> ().alpha = 0f;

		if (playerInfo.Player_Status == (int)PLAYER_STATUS.ABSENT) {
			whoopAssPlayer.imgAbsentPlayer.sprite = whoopAssPlayer.spAbsent;
			whoopAssPlayer.imgAbsentPlayer.gameObject.SetActive (true);
			whoopAssPlayer.imgAbsentPlayer.color = Color.yellow;
		} else if (playerInfo.Player_Status == (int)PLAYER_STATUS.SIT_OUT) {
			whoopAssPlayer.imgAbsentPlayer.sprite = whoopAssPlayer.spSitout;
			whoopAssPlayer.imgAbsentPlayer.gameObject.SetActive (true);
			whoopAssPlayer.imgAbsentPlayer.color = Color.red;
		} else {
			whoopAssPlayer.imgAbsentPlayer.gameObject.SetActive (false);
		}

		if (playerInfo.Player_Status == (int)PLAYER_STATUS.ELIMINATED) {
			whoopAssPlayer.txtPlayerName.text = "<color=red>ELIMINATED</color>";
			whoopAssPlayer.DestroyCards ();
		}

		obj.transform.SetParent (playerSeats [playerInfo.Player_Position]);
		obj.transform.localScale = Vector3.one;

		allWhoopAssPlayers.Add (whoopAssPlayer);

		if (playerInfo.Player_Name.Equals (NetworkManager.Instance.playerID)) {
			ownWhoopAssPlayer = whoopAssPlayer;
			CentralizeOwnPlayer ();

			if (initialBuyinAmountForTournament == 0)
				initialBuyinAmountForTournament = playerInfo.Player_BuyIn_Chips;
		}

		if (whoopAssPlayer.buyInAmount <= 0)
			whoopAssPlayer.GetComponent<CanvasGroup> ().alpha = .4f;
		else
			whoopAssPlayer.GetComponent<CanvasGroup> ().alpha = 1f;

		SetBetParentPositions ();

		HandleWaitingPlayer (playerInfo);
	}

	private void HandleOnGameStartedByPlayer (string sender, string gameStarter)
	{
		if (gameStarter.Equals (NetworkManager.Instance.playerID)) {
			OpenBetPanel ();
		} else {
			HideBetPanel ();

			gameButtonsCanvasGroup.interactable = false;


			///
			if (RoundController.GetInstance ().currentWhoopAssGameRound == WHOOPASS_GAME_ROUND.WHOOPASS_CARD) {
				///
				gameButtonsCanvasGroup.gameObject.SetActive (true);
				gameButtonsCanvasGroup.interactable = false;
				///
				noTurnCheckBoxesCanvasGroup.gameObject.SetActive (false);
				noTurnCheckBoxesCanvasGroup.interactable = false;
			} else {
				//
				if (ownWhoopAssPlayer.playerInfo.Player_Status == (int)PLAYER_STATUS.ACTIVE) {
					gameButtonsCanvasGroup.gameObject.SetActive (false);

					noTurnCheckBoxesCanvasGroup.interactable = true;
					noTurnCheckBoxesCanvasGroup.gameObject.SetActive (true);

					noTurnCheckBoxesCanvasGroup.GetComponent<NoTurnPanel> ().DisplayCheckboxes ();
				}
			}
		}

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
				if (ownWhoopAssPlayer.playerInfo.Player_Status == (int)PLAYER_ACTION.ACTION_WAITING_FOR_GAME)
					PlayerIsWaiting ();
				else if (ownWhoopAssPlayer.playerInfo.Player_Status == (int)PLAYER_ACTION.FOLD)
					PlayerIsFolded ();
				else if (ownWhoopAssPlayer.playerInfo.Player_Status == (int)PLAYER_ACTION.TIMEOUT)
					PlayerIsTimeout ();
				else if (ownWhoopAssPlayer.playerInfo.Player_Status == (int)PLAYER_ACTION.ALLIN)
					PlayerIsAllIn ();
				else
					OpenBetPanel ();
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
			btnSitOutNextHand.gameObject.SetActive (true);

		if (ownWhoopAssPlayer.playerInfo.Player_Status == (int)PLAYER_STATUS.SIT_OUT)// &&
//			!UIManager.Instance.isRegularTournament &&
//			!UIManager.Instance.isSitNGoTournament)
            sitOutCounter++;

		int totalWinner = 0;

		HideBetPanel ();


		///
		gameButtonsCanvasGroup.interactable = false;
		gameButtonsCanvasGroup.gameObject.SetActive (true);

		noTurnCheckBoxesCanvasGroup.interactable = false;
		noTurnCheckBoxesCanvasGroup.gameObject.SetActive (false);


		if (winnerInfo == null) {
			StartCoroutine (ResetGameAfterSomeTime (1));
			return;
		}

		//      Handle winner Info here
		//WA_Game_Winner gw = JsonUtility.FromJson<WA_Game_Winner>(winnerInfo);
		List<WA_GW_Table_Pot> tablePotWinnerList = new List<WA_GW_Table_Pot> ();
		List<WA_GW_WA_Pot> waPotWinnerList = new List<WA_GW_WA_Pot> ();

		JSON_Object obj = new JSON_Object (winnerInfo);

		JSONArray arrTablePot = new JSONArray (obj.getString ("Table_Pot"));
		totalWinner = arrTablePot.Count ();
		//  Table Pot Winner
		for (int i = 0; i < arrTablePot.Count (); i++) {
			WA_GW_Table_Pot tp = JsonUtility.FromJson<WA_GW_Table_Pot> (arrTablePot.getString (i));
			tablePotWinnerList.Add (tp);
			TableTotalAmount = tp.Total_Table_Amount;
		}

		WAPotAmount = 0;
		JSONArray arrWAPot = new JSONArray (obj.getString ("WA_Pot"));
		//  WA Pot Winner
		for (int i = 0; i < arrWAPot.Count (); i++) {
			WA_GW_WA_Pot wp = JsonUtility.FromJson<WA_GW_WA_Pot> (arrWAPot.getString (i));
			waPotWinnerList.Add (wp);
			TableTotalAmount += wp.Winning_Amount;
		}

		StartCoroutine (DisplayWinnerAnimation (tablePotWinnerList, waPotWinnerList));

		/*
		if (winnerInfo != null) {
			Debug.LogWarning (winnerInfo);
			JSONArray arr = new JSONArray (winnerInfo);

			List<GameWinner> winnerList = new List<GameWinner> ();
			for (int i = 0; i < arr.Count (); i++) {
				GameWinner win = JsonUtility.FromJson<GameWinner> (arr.getString (i));
				winnerList.Add (win);
			}

			if (allWhoopAssPlayers.Count > 1)
				StartCoroutine (DisplayWinnerAnimation (winnerList));
		}
        */
		StartCoroutine (ResetGameAfterSomeTime (totalWinner));

//		if (ownWhoopAssPlayer.buyInAmount <= bigBlindAmount &&
		if (!UIManager.Instance.isRegularTournament &&
		    !UIManager.Instance.isSitNGoTournament) {
			if (UIManager.Instance.isRealMoney) {
				if (ownWhoopAssPlayer.totalRealMoney > 0) {
					if (!btnRebuy.gameObject.activeSelf)
						btnAddChips.gameObject.SetActive (true);
				}
			} else {
				if (ownWhoopAssPlayer.totalChips > 0) {
					if (!btnRebuy.gameObject.activeSelf)
						btnAddChips.gameObject.SetActive (true);
				}
			}
		} else {
			if (UIManager.Instance.isRegularTournament) {
				if (UIManager.Instance.isRealMoney) {
					if (ownWhoopAssPlayer.buyInAmount < 0 && ownWhoopAssPlayer.totalChips >= initialBuyinAmountForTournament && canRebuy)
						btnRebuy.gameObject.SetActive (true);
				} else {
					if (ownWhoopAssPlayer.buyInAmount < 0 && ownWhoopAssPlayer.totalRealMoney >= initialBuyinAmountForTournament && canRebuy)
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
				WhoopAssPlayer whoopAssPlayer = GetPlayerByID (playerID);
				if (whoopAssPlayer) {
					whoopAssPlayer.playerInfo.Player_Status = (int)PLAYER_STATUS.ABSENT;
					whoopAssPlayer.imgAbsentPlayer.sprite = whoopAssPlayer.spAbsent;
					whoopAssPlayer.imgAbsentPlayer.gameObject.SetActive (true);
					whoopAssPlayer.imgAbsentPlayer.color = Color.yellow;
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

	private void HandleOnRoundComplete (string sender, string roundInfo)
	{
		raisePerRoundCounter = 0;

		GameRound round = JsonUtility.FromJson<GameRound> (roundInfo);
		TableTotalAmount = round.Total_Table_Amount + round.Total_WA_Pot_Amount;
//        WAPotAmount = round.Total_WA_Pot_Amount;
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

	private void HandleOnBreakTimeResponseReceived (string sender, int breakTimer, int totalTables = 1)
	{
		UIManager.Instance.isBreakTime = true;
		UIManager.Instance.breakTimeTillSeconds = breakTimer;
		UIManager.Instance.noTablesAtBreaktime = totalTables == 1;

		if (currentGameStatus != GAME_STATUS.RUNNING && isGameCompleted)
			UIManager.Instance.breakTimePanel.DisplayBreakTimer (breakTimer);
	}

	private void HandleOnResponseForRebuyReceived (string sender, string canRebuy)
	{
		this.canRebuy = canRebuy.Equals ("0") ? false : true;
	}

	private void HandleOnTournamentWinnerInfoReceived (string sender, string tournamentWinnerInfo)
	{
		txtTableMessage.text = "Fetching tournament winners.. Please wait..";
		UIManager.Instance.tournamentWinnerPanel.SetTournamentWinnerDetails (tournamentWinnerInfo);
	}

	private void HandleOnNotRegisteredInTournamentResponseReceived (string sender)
	{
		if (sender.Equals (Constants.WHOOPASS_SERVER_NAME)) {
			UIManager.Instance.notRegisteredWithTournamentPanel.gameObject.SetActive (true);
		}
	}

	private void HandleOnLeaveRoomSuccess (bool success)
	{
		if (success) {

		}
	}

	private void HandleOnActionHistoryReceived (string sender, string history)
	{
		//Debug.Log (history);

		JSON_Object obj = new JSON_Object (history);
		SetGameStatus (int.Parse (obj.get (Constants.FIELD_ACTION_HISTORY_GAME_STATUS).ToString ()));
		RoundController.GetInstance ().SetWhoopAssGameRound (int.Parse (obj.get (Constants.FIELD_ACTION_HISTORY_ROUND).ToString ()));

		if (currentGameStatus != GAME_STATUS.PAUSED && currentGameStatus != GAME_STATUS.RUNNING && currentGameStatus != GAME_STATUS.RESUMED)
			return;

		int roundStatus = int.Parse (obj.get (Constants.FIELD_ACTION_HISTORY_ROUND_STATUS).ToString ());

		JSONArray actionHistoryArray = obj.getJSONArray (Constants.FIELD_ACTION_HISTORY_TURNS);

		for (int i = 0; i < actionHistoryArray.Count (); i++) {
			ActionResponse ar = JsonUtility.FromJson<ActionResponse> (actionHistoryArray.get (i).ToString ());

			//Debug.LogWarning("player  : " + ar.Player_Name + "\t\tAction  : " + ar.Action + "\t\tBet amount  : " + ar.Bet_Amount + "\t\tRound  : " + RoundController.GetInstance().currentWhoopAssGameRound);

			WhoopAssPlayer p = GetPlayerByID (ar.Player_Name);
			if (p) {
				if (roundStatus != (int)ROUND_STATUS.ACTIVE)
					p.betAmountInPot += ar.Bet_Amount;
			}

			TableTotalAmount = ar.Total_Table_Amount;
			NetworkManager.FireActionResponseReceived (Constants.WHOOPASS_SERVER_NAME, actionHistoryArray.get (i).ToString ());
			//SetPlayerDetailsOnContinuePlay(ar, roundStatus);

			TableTotalAmount = ar.Total_Table_Amount;
		}

		if (roundStatus == (int)ROUND_STATUS.ACTIVE) {
			//	Generate player cards to continue playing
//            GeneratePlayerCardsToContinuePlaying();

			//SetNextPlayerTurnOnContinuePlay (lastTurnPlayerID);
		}
	}

	private void HandlePlayerRequestedSitout (string playerID)
	{
		if (playerID == "")
			return;

		WhoopAssPlayer player = GetPlayerByID (playerID);
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

		WhoopAssPlayer player = GetPlayerByID (playerID);
		if (player != null) {
			txtGameLog.text += "\n" + playerID + Constants.MESSAGE_PLAYER_IS_BACK_TO_GAME;
			Canvas.ForceUpdateCanvases ();
			if (scrollNote.gameObject.activeSelf)
				scrollNote.verticalScrollbar.value = 0;

			if ((currentGameStatus == GAME_STATUS.STOPPED ||
			    currentGameStatus == GAME_STATUS.FINISHED) &&
			    isGameCompleted &&
			    playerID.Equals (NetworkManager.Instance.playerID)) {
				btnSitOutNextHand.gameObject.SetActive (true);
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

//		WhoopAssPlayer p = GetPlayerByID (playerID);
//		if (p != null) {
//			p.isEliminated = true;
//			p.txtPlayerName.text = "<color=red>ELIMINATED</color>";
//			p.playerInfo.Player_Status = (int)PLAYER_STATUS.ELIMINATED;
//			p.DestroyCards ();
//		}

		DestroyPlayer (playerID);

		if (playerID.Equals (NetworkManager.Instance.playerID)) {
			NetworkManager.Instance.Disconnect ();
			UIManager.Instance.playerEliminatedPanel.gameObject.SetActive (true);
		}
	}

	private void HandleOnRestartGameRequestReceived (string sender)
	{
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

	private void HandleCardDistributionFinished ()
	{
//		if (recentlyBackToGame) {
//			NetworkManager.Instance.SendRequestToServer (Constants.REQUEST_PLAYER_BACK_TO_GAME_COLLECT_BLIND + bigBlindAmount);
//		}
	}

	private void HandleCollectBlindOnBackToGame (string playerID, double amount)
	{
		TableTotalAmount += amount;
		if (playerID.Equals (NetworkManager.Instance.playerID))
			recentlyBackToGame = false;
	}

	#endregion


	#region PUBLIC_METHODS

	public void OnHomeButtonTap ()
	{
		UIManager.Instance.backConfirmationPanel.gameObject.SetActive (true);
	}

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
		scrollNote.verticalScrollbar.value = 0;

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
		scrollNote.verticalScrollbar.value = 0;

		Color c = new Color ();
		ColorUtility.TryParseHtmlString (APIConstants.HEX_RED_HEADER, out c);
		btnInfo.GetComponentInChildren<Text> ().color = c;

		btnStats.GetComponentInChildren<Text> ().color = Color.white;
		btnNote.GetComponentInChildren<Text> ().color = Color.white;
	}

	public void OnFoldButtonTap ()
	{
		DebugLog.Log ("Fold button tapped");

		PlayerAction action = new PlayerAction ();
		action.Action = (int)PLAYER_ACTION.FOLD;
		action.Player_Name = NetworkManager.Instance.playerID;

		ownWhoopAssPlayer.playerInfo.Player_Status = (int)PLAYER_STATUS.FOLDED;

		NetworkManager.Instance.SendPlayerAction (action);

		SoundManager.Instance.PlayWooshSound (Camera.main.transform.position);

		HideBetPanel ();

//        btnSitOutNextHand.gameObject.SetActive(true);
	}

	public void OnAllinButtonTap ()
	{
		DebugLog.Log ("Allin button tapped");

		PlayerAction action = new PlayerAction ();
		action.Action = (int)PLAYER_ACTION.ALLIN;
		action.Bet_Amount = ownWhoopAssPlayer.buyInAmount;
		action.Player_Name = NetworkManager.Instance.playerID;

		ownWhoopAssPlayer.playerInfo.Player_Status = (int)PLAYER_ACTION.ALLIN;

		NetworkManager.Instance.SendPlayerAction (action);

		SoundManager.Instance.PlayWooshSound (Camera.main.transform.position);

		HideBetPanel ();
	}

	public void OnBetButtonTap ()
	{
		DebugLog.Log ("Bet button tapped");

		PlayerAction action = new PlayerAction ();
		action.Action = (int)PLAYER_ACTION.BET;
		action.Bet_Amount = betSlider.value.RoundTo2DigitFloatingPoint ();
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
		double pendingAmount = minimumAmountToBet - ownWhoopAssPlayer.betAmount; 
		if (pendingAmount <= 0) {
			OnCheckButtonTap ();
			return;
		}

		PlayerAction action = new PlayerAction ();
		action.Action = (int)PLAYER_ACTION.CALL;

		action.Bet_Amount = pendingAmount <= 0 ? 0 : pendingAmount;

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
			if (UIManager.Instance.isRealMoney)
				rebuyPanel.DisplayRebuyPanel (ownWhoopAssPlayer.totalRealMoney - ownWhoopAssPlayer.buyInAmount);
			else
				rebuyPanel.DisplayRebuyPanel (ownWhoopAssPlayer.totalChips - ownWhoopAssPlayer.buyInAmount);
		}
	}

	public void OnAddChipsButtonTap ()
	{
		DebugLog.Log ("Add Chips button tapped");

		SoundManager.Instance.PlayWooshSound (Camera.main.transform.position);

		btnAddChips.gameObject.SetActive (false);

		if (UIManager.Instance.isRealMoney)
			rebuyPanel.DisplayRebuyPanel (ownWhoopAssPlayer.totalRealMoney - ownWhoopAssPlayer.buyInAmount);
		else
			rebuyPanel.DisplayRebuyPanel (ownWhoopAssPlayer.totalChips - ownWhoopAssPlayer.buyInAmount);
	}

	public void OnBetSliderValueChanged ()
	{
		txtBetSliderValue.text = Utility.GetAmount ((double)betSlider.value);

		double pendingAmount = minimumAmountToBet - ownWhoopAssPlayer.betAmount;
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

		if (betSlider.value >= ownWhoopAssPlayer.buyInAmount) {
			btnAllin.gameObject.SetActive (true);
			btnRaise.gameObject.SetActive (false);
		} else {
			btnAllin.gameObject.SetActive (false);
			btnRaise.gameObject.SetActive (true);
		}

		if (raisePerRoundCounter >= Constants.MAX_TIME_RAISE_PER_ROUND)
			btnRaise.interactable = false;
	}

	public void OnSitOutNextHandButtonTap ()
	{
		btnSitOutNextHand.gameObject.SetActive (false);
		btnBackToGame.gameObject.SetActive (true);

		NetworkManager.Instance.SendRequestToServer (Constants.REQUEST_FOR_SIT_OUT);
	}

	public void OnBackToGameButtonTap ()
	{
		btnBackToGame.gameObject.SetActive (false);
		NetworkManager.Instance.SendRequestToServer (Constants.REQUEST_FOR_BACK_TO_GAME);
	}

	public void OnChatButtonTap ()
	{
		chatTemplatesPanel.gameObject.SetActive (false);
		ifChat.gameObject.SetActive (true);

		ifChat.Select ();
		ifChat.ActivateInputField ();
	}

	public void OnChatTemplateButtonTap ()
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

	public void OpenBetPanel ()
	{
		gameButtonsCanvasGroup.interactable = true;
		betSlider.wholeNumbers = !UIManager.Instance.isRealMoney;

		if (RoundController.GetInstance ().currentWhoopAssGameRound == WHOOPASS_GAME_ROUND.WHOOPASS_CARD) {
			double betAmount = RoundController.GetInstance ().GetLastCallAmountByPlayerInWhoopAssRound (NetworkManager.Instance.playerID, WHOOPASS_GAME_ROUND.SECOND_FLOP);
			if (betAmount <= 0)
				betAmount = RoundController.GetInstance ().GetLastCallAmountByPlayerInWhoopAssRound (NetworkManager.Instance.playerID, WHOOPASS_GAME_ROUND.FIRST_FLOP);
			betAmount = betAmount <= 0 ? bigBlindAmount : betAmount;
			betAmount = betAmount > ownWhoopAssPlayer.buyInAmount ? (double)ownWhoopAssPlayer.buyInAmount : (double)betAmount;

			UIManager.Instance.whoopAssCardRoundPanel.SetTitle (betAmount);

			btnCheck.interactable = false;
			btnRaise.interactable = false;
			betSlider.interactable = false;
			btnBet.interactable = false;
			btnCall.interactable = false;
			btnRebuy.gameObject.SetActive (false);

			btnFold.interactable = true;

			btnRaise.gameObject.SetActive (true);
			btnAllin.gameObject.SetActive (false);

			///
			gameButtonsCanvasGroup.gameObject.SetActive (true);
			gameButtonsCanvasGroup.interactable = true;

			///
			noTurnCheckBoxesCanvasGroup.gameObject.SetActive (false);
			noTurnCheckBoxesCanvasGroup.interactable = false;
		} else {
			///
			gameButtonsCanvasGroup.gameObject.SetActive (true);
			gameButtonsCanvasGroup.interactable = true;

			UIManager.Instance.whoopAssCardRoundPanel.gameObject.SetActive (false);
			SetBetButtons ();

			///
			noTurnCheckBoxesCanvasGroup.gameObject.SetActive (false);
			noTurnCheckBoxesCanvasGroup.interactable = false;
		}
	}

	public void HideBetPanel ()
	{
		betSlider.minValue = betSlider.maxValue = 0;
		betSlider.value = 0;
		OnBetSliderValueChanged ();
		gameButtonsCanvasGroup.interactable = false;
		UIManager.Instance.whoopAssCardRoundPanel.gameObject.SetActive (false);

		if (ownWhoopAssPlayer != null)
			ownWhoopAssPlayer.HideTurnTimer ();
	}

	public WhoopAssPlayer GetDealerPlayer ()
	{
		foreach (WhoopAssPlayer p in allWhoopAssPlayers) {
			if (p.isDealer)
				return p;
		}

		return null;
	}

	public WhoopAssPlayer GetSmallBlindPlayer ()
	{
		foreach (WhoopAssPlayer p in allWhoopAssPlayers) {
			if (p.isSmallBlind)
				return p;
		}

		return null;
	}

	public WhoopAssPlayer GetPlayerByID (string playerID)
	{
		foreach (WhoopAssPlayer p in allWhoopAssPlayers) {
			if (p.playerID.Equals (playerID))
				return p;
		}

		return null;
	}

	public void DisplayPlayerTotalChips (double chips)
	{
		txtPlayerChips.text = Utility.GetCommaSeperatedAmount (chips);
	}

	public int GetActivePlayers ()
	{
		int activePlayers = 0;

		foreach (WhoopAssPlayer p in allWhoopAssPlayers) {
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

	public void GeneratePlayerCardsToContinuePlaying ()
	{
		foreach (WhoopAssPlayer p in allWhoopAssPlayers) {
			for (int i = 0; i < 2; i++) {
				GameObject card = Instantiate (RoundController.GetInstance ().whoopAssPlayerCardPrefab, i == 0 ? p.card1Position.position : p.card2Position.position, Quaternion.identity) as GameObject;
				card.transform.SetParent (i == 0 ? p.card1Position : p.card2Position);
				card.transform.localScale = Vector3.one;

				if (i == 0) {
					if (p == ownWhoopAssPlayer)
						card.GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (p.playerInfo.Card1);
				} else {
					if (p == ownWhoopAssPlayer)
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
		allWhoopAssPlayers = allWhoopAssPlayers.OrderBy (o => o.seatIndex).ToList ();
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
		txtTitle.text = Constants.MESSAGE_WHOOPASS_GAME_TITLE;
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

	private void HandleWaitingPlayer (PlayerInfo playerInfo)
	{
		if (playerInfo.Player_Name.Equals (NetworkManager.Instance.playerID)) {
			RoundController.GetInstance ().currentWhoopAssGameRound = GetRound (playerInfo.Current_Round);

			if (playerInfo.Game_Status == (int)GAME_STATUS.RUNNING ||
			    playerInfo.Game_Status == (int)GAME_STATUS.CARD_DISTRIBUTE) {
				if (playerInfo.Player_Status == (int)PLAYER_STATUS.WAITING ||
				    playerInfo.Player_Status == (int)PLAYER_ACTION.ACTION_WAITING_FOR_GAME ||
				    playerInfo.Player_Status == (int)PLAYER_STATUS.ACTIVE ||
				    playerInfo.Player_Status == (int)PLAYER_STATUS.FOLDED ||
				    playerInfo.Player_Status == (int)PLAYER_ACTION.ACTION_FOLDED) {
					GeneratePlayerCardsForWaitingPlayer ();
					GenerateDefaultCardsForWhoopAssPlayer ();
				}
			}
		}
	}

	private void GeneratePlayerCardsForWaitingPlayer ()
	{
		RoundController.GetInstance ().GenerateWhoopAssPlayerCardsForWaitingPlayer ();
	}

	private void GenerateDefaultCardsForWhoopAssPlayer ()
	{
		RoundController.GetInstance ().GenerateDefaultCardsForWhoopAssPlayer ();
	}

	private WHOOPASS_GAME_ROUND GetRound (int round)
	{
		switch (round) {
		case (int) WHOOPASS_GAME_ROUND.THIRD_FLOP:
			return WHOOPASS_GAME_ROUND.THIRD_FLOP;
		case (int) WHOOPASS_GAME_ROUND.WHOOPASS_CARD:
			return WHOOPASS_GAME_ROUND.WHOOPASS_CARD;
		case (int) WHOOPASS_GAME_ROUND.SECOND_FLOP:
			return WHOOPASS_GAME_ROUND.SECOND_FLOP;
		case (int) WHOOPASS_GAME_ROUND.FIRST_FLOP:
			return WHOOPASS_GAME_ROUND.FIRST_FLOP;
		case (int) WHOOPASS_GAME_ROUND.START:
			return WHOOPASS_GAME_ROUND.START;
		}

		return WHOOPASS_GAME_ROUND.START;
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

	private void ResetTableCardsToInitialPosition ()
	{
		foreach (Transform t in defaultTableCardsList) {
			Vector2 pos = t.position;
			pos.y = t.parent.position.y;
			t.position = pos;
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
		if (RoundController.GetInstance ().currentWhoopAssGameRound == WHOOPASS_GAME_ROUND.WHOOPASS_CARD) {
			UIManager.Instance.whoopAssCardRoundPanel.SetTitle (0);
		} else {
			//PlayerAction action = new PlayerAction();
			//action.Player_Name = currentTurnPlayerID;
			//action.Action = (int)PLAYER_ACTION.ALLIN;

			//NetworkManager.Instance.SendPlayerAction(action);
		}
	}

	public void DestroyPlayer (string playerID)
	{
		WhoopAssPlayer p = GetPlayerByID (playerID);

		if (p) {
			allWhoopAssPlayers.Remove (p);
//            p.gameObject.SetActive(false);
			Destroy (p.gameObject);
		}
	}

	private void DestroyAllPlayers ()
	{
		foreach (WhoopAssPlayer wp in allWhoopAssPlayers) {
			wp.gameObject.SetActive (false);
		}
		allWhoopAssPlayers = new List<WhoopAssPlayer> ();
	}

	private void HandleLimitGameButtons (double pendingAmount)
	{
		if (UIManager.Instance.isLimitGame)
			betSlider.value = betSlider.maxValue;

//		if (UIManager.Instance.isLimitGame) {
//			if (betSlider.value >= pendingAmount + 1)
//				betSlider.value = betSlider.maxValue;
//			if (betSlider.value <= betSlider.maxValue - 1)
//				betSlider.value = (long)pendingAmount;
//		}
	}

	private void RaiseOrBetButton ()
	{
		double pendingAmount = minimumAmountToBet - ownWhoopAssPlayer.betAmount;
		if (RoundController.GetInstance ().currentWhoopAssGameRound == WHOOPASS_GAME_ROUND.START) {
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

	private void SetBetButtons ()
	{
		minimumAmountToBet = RoundController.GetInstance ().GetMinBetAmountInCurrentRound ();
		double pendingAmount = minimumAmountToBet - ownWhoopAssPlayer.betAmount;
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
			if (RoundController.GetInstance ().currentWhoopAssGameRound == WHOOPASS_GAME_ROUND.START ||
			    RoundController.GetInstance ().currentWhoopAssGameRound == WHOOPASS_GAME_ROUND.FIRST_FLOP) {
				double maxBet = pendingAmount + smallBlindAmount;
				betSlider.maxValue = maxBet < ownWhoopAssPlayer.buyInAmount ? (float)maxBet : (float)ownWhoopAssPlayer.buyInAmount;
			} else if (RoundController.GetInstance ().currentWhoopAssGameRound == WHOOPASS_GAME_ROUND.SECOND_FLOP ||
			           RoundController.GetInstance ().currentWhoopAssGameRound == WHOOPASS_GAME_ROUND.THIRD_FLOP) {
				double maxBet = pendingAmount + bigBlindAmount;
				betSlider.maxValue = maxBet < ownWhoopAssPlayer.buyInAmount ? (float)maxBet : (float)ownWhoopAssPlayer.buyInAmount;
			}

			//  To display bet button directly if not bet yet
//            if (pendingAmount <= 0)
			betSlider.value = betSlider.maxValue;
		} else {
			betSlider.maxValue = (float)ownWhoopAssPlayer.buyInAmount;
		}


		//	Check button
		if (pendingAmount <= 0)
			btnCheck.interactable = true;
		else
			btnCheck.interactable = false;


		//	Call button
		if (pendingAmount <= 0 || pendingAmount >= ownWhoopAssPlayer.buyInAmount)
			btnCall.interactable = false;
		else
			btnCall.interactable = true;


		//	Allin Button
		btnRebuy.gameObject.SetActive (false);
		if (pendingAmount >= ownWhoopAssPlayer.buyInAmount) {
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

	private void SetPlayerDetailsOnContinuePlay (ActionResponse ar, int roundStatus)
	{
		TableTotalAmount = ar.Total_Table_Amount;
		NetworkManager.FireActionResponseReceived (Constants.WHOOPASS_SERVER_NAME, JsonUtility.ToJson (ar));


		//WhoopAssPlayer p = GetPlayerByID(ar.Player_Name);
		//if (p)
		//{
		//    PLAYER_ACTION action = DisplayPlayerActionOnContinuePlay(p, ar);

		//    p.totalChips = ar.Player_Balance;
		//    p.DisplayTotalChips();

		//    p.betAmount += ar.Bet_Amount;
		//    if (roundStatus != (int)ROUND_STATUS.ACTIVE)
		//    {
		//        p.betAmountInPot += ar.Bet_Amount;
		//    }
		//    p.DisplayBetAmount();

		//    HistoryManager.GetInstance().AddHistory(ar.Player_Name, "name", RoundController.GetInstance().currentWhoopAssGameRound, ar.Bet_Amount, p.betAmount, action);
		//}
	}

	private void CheckForLowChips ()
	{
//		if (ownWhoopAssPlayer.buyInAmount <= bigBlindAmount &&
//			!UIManager.Instance.isRegularTournament &&
//			!UIManager.Instance.isSitNGoTournament) {
//			if (ownWhoopAssPlayer.totalChips <= bigBlindAmount) {
////				NetworkManager.Instance.Disconnect ();
//				UIManager.Instance.noEnoughChipsPanel.gameObject.SetActive (true);
//			} else {
//				if (!btnRebuy.gameObject.activeSelf)
//					btnAddChips.gameObject.SetActive (true);
//			}
//		}

		if (ownWhoopAssPlayer.buyInAmount <= 0 &&
		    !UIManager.Instance.isRegularTournament &&
		    !UIManager.Instance.isSitNGoTournament) {
			if (UIManager.Instance.isRealMoney)
				UIManager.Instance.DisplayNotEnoughChipsPanel (ownWhoopAssPlayer.totalRealMoney > 0);
			else
				UIManager.Instance.DisplayNotEnoughChipsPanel (ownWhoopAssPlayer.totalChips > 0);
		}
	}

	private void CheckForMaxSitout ()
	{
		
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
		return initialChipPosition.transform.position.y + (i * 2f);
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

	private float GetRedChipYPos (int i)
	{
		return redInitialChip.position.y + (i * 1.5f);
	}

	private float GetGreenChipYPos (int i)
	{
		return greenInitialChip.position.y + (i * 1.5f);
	}

	private float GetBlueChipYPos (int i)
	{
		return blueInitialChip.position.y + (i * 1.5f);
	}

	private int CalculateRedChips (int chips)
	{
		int t = chips / Constants.RED_CHIP_VALUE;

		return t > 10 ? 10 : t;
	}

	private int CalculateGreenChips (int chips)
	{
		int val = chips % Constants.RED_CHIP_VALUE;
		int t = val / Constants.GREEN_CHIP_VALUE;

		return t > 10 ? 10 : t;
	}

	private int CalculateBlueChips (int chips)
	{
		int val = chips % Constants.RED_CHIP_VALUE;
		int gval = val % Constants.GREEN_CHIP_VALUE;
		int t = gval / Constants.BLUE_CHIP_VALUE;

		return t > 10 ? 10 : t;
	}

	private void CentralizeOwnPlayer ()
	{
		int posToMove = GetOwnPlayerPositionToMoveCenter ();

		if (posToMove >= Constants.WHOOPASS_GAME_MAX_PLAYERS) {
			return;
		}

		for (int i = 0; i < playerSeats.Count; i++) {
			int posIndex = i + posToMove;

			if (posIndex > Constants.WHOOPASS_GAME_MAX_PLAYERS - 1)
				posIndex = posIndex - Constants.WHOOPASS_GAME_MAX_PLAYERS;

			playerSeats [i].transform.SetParent (playerPositions [posIndex]);

			playerSeats [i].position = playerPositions [posIndex].position;
		}
	}

	private int GetOwnPlayerPositionToMoveCenter ()
	{
		int movePos = Constants.WHOOPASS_GAME_MAX_PLAYERS - ownWhoopAssPlayer.playerInfo.Player_Position;
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
		foreach (WhoopAssPlayer p in allWhoopAssPlayers) {
			p.betAmountParentObj.transform.position = p.betAmountPositionsList [playerPositions.IndexOf (p.transform.parent.parent)].position;
			p.txtBetAmountPosition.position = p.betChipsPositionList [playerPositions.IndexOf (p.transform.parent.parent)].position;
		}
	}

	private void OnCallAnyAutoButton ()
	{
		DebugLog.Log ("Call button tapped");

		PlayerAction action = new PlayerAction ();
		action.Action = (int)PLAYER_ACTION.CALL;

		double pendingAmount = minimumAmountToBet - ownWhoopAssPlayer.betAmount; 
		if (pendingAmount <= 0) {
			OnCheckButtonTap ();
			return;
		} else if (pendingAmount >= ownWhoopAssPlayer.buyInAmount) {
			pendingAmount = ownWhoopAssPlayer.buyInAmount;
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
		WAPotAmount = 0;
		UIManager.Instance.winReportPanel.gameObject.SetActive (false);

		if (UIManager.Instance.breakingTablePanel.willBreakTable) {
			gameObject.SetActive (false);
			UIManager.Instance.breakingTablePanel.gameObject.SetActive (true);
		} else {
			if (UIManager.Instance.isBreakTime)// && ownWhoopAssPlayer.totalChips >= bigBlindAmount)
				UIManager.Instance.breakTimePanel.DisplayBreakTimer (UIManager.Instance.breakTimeTillSeconds);
		}

		if (txtGameLog.text.Length > 50000) {
			txtGameLog.text = "";
			Canvas.ForceUpdateCanvases ();
			if (scrollNote.gameObject.activeSelf)
				scrollNote.verticalScrollbar.value = 0;
		}
	}

	private IEnumerator DisplayWinnerAnimation (List<WA_GW_Table_Pot> tablePotWinnerList, List<WA_GW_WA_Pot> waPotWinnerList)
	{
		yield return new WaitForSeconds (1f);

		for (int i = 0; i < tablePotWinnerList.Count; i++) {
			WhoopAssPlayer p = GetPlayerByID (tablePotWinnerList [i].Winner_Name);
			if (p) {
//				if (p == ownWhoopAssPlayer) {
//					rebuyPanel.gameObject.SetActive (false);
//					btnRebuy.gameObject.SetActive (false);
//					btnAddChips.gameObject.SetActive (false);
//				}

				if (GetActivePlayers () > 1 || TournamentWinnerPanel.isTournamentWinnersDeclared) {
					p.HighlightWinnerBestCards (tablePotWinnerList [i].Winner_Best_Cards);
					p.txtWinnerRank.transform.parent.gameObject.SetActive (true);
					p.txtWinnerRank.text = GetWinningRank (tablePotWinnerList [i].Winner_Rank);
					p.txtBetAmount.text = Utility.GetAmount (tablePotWinnerList [i].Winning_Amount);

					p.imgBetAmountBG.gameObject.SetActive (true);
					p.imgWinningBetAmountBG.gameObject.SetActive (true);

					txtGameLog.text += "\nWinner -> <color=" + APIConstants.HEX_COLOR_LIST_VIEW_HEADER + ">" + p.playerID + " -> " + GetWinningRank (tablePotWinnerList [i].Winner_Rank) + "</color> -> " + Utility.GetAmount (tablePotWinnerList [i].Winning_Amount);
					Canvas.ForceUpdateCanvases ();
					if (scrollNote.gameObject.activeSelf)
						scrollNote.verticalScrollbar.value = 0;

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

					p.buyInAmount += tablePotWinnerList [i].Winning_Amount;
					StartCoroutine (MoveChipToWinnerPlayer (p));
					TableTotalAmount -= tablePotWinnerList [i].Winning_Amount;

					yield return new WaitForSeconds (1f);
					if (tablePotWinnerList [i].Rake_Amount + tablePotWinnerList [i].AffiliateCommission > 0) {
						TableTotalAmount -= (tablePotWinnerList [i].Rake_Amount + tablePotWinnerList [i].AffiliateCommission);
						StartCoroutine (MoveRakeAmountToWA (tablePotWinnerList [i].Rake_Amount + tablePotWinnerList [i].AffiliateCommission));
					}

					yield return new WaitForSeconds (7f);
					p.ResetCardsToInitialPosition ();
					ResetTableCardsToInitialPosition ();
				} else {
					p.txtBetAmount.text = Utility.GetAmount (tablePotWinnerList [i].Winning_Amount);
					p.imgBetAmountBG.gameObject.SetActive (true);
					p.imgWinningBetAmountBG.gameObject.SetActive (true);

					p.buyInAmount += tablePotWinnerList [i].Winning_Amount;
					StartCoroutine (MoveChipToWinnerPlayer (p));
					TableTotalAmount -= tablePotWinnerList [i].Winning_Amount;

					if (p.playerID.Equals (NetworkManager.Instance.playerID))
						UIManager.Instance.winnerAnimationPanel.gameObject.SetActive (true);

					txtGameLog.text += "\nWinner -> <color=" + APIConstants.HEX_COLOR_LIST_VIEW_HEADER + ">" + p.playerID + "</color> -> " + Utility.GetAmount (tablePotWinnerList [i].Winning_Amount);
					Canvas.ForceUpdateCanvases ();
					if (scrollNote.gameObject.activeSelf)
						scrollNote.verticalScrollbar.value = 0;

					yield return new WaitForSeconds (1f);
					if (tablePotWinnerList [i].Rake_Amount + tablePotWinnerList [i].AffiliateCommission > 0) {
						TableTotalAmount -= (tablePotWinnerList [i].Rake_Amount + tablePotWinnerList [i].AffiliateCommission);
						StartCoroutine (MoveRakeAmountToWA (tablePotWinnerList [i].Rake_Amount + tablePotWinnerList [i].AffiliateCommission));
					}
				}
			}
		}


		for (int i = 0; i < waPotWinnerList.Count; i++) {
			WhoopAssPlayer p = GetPlayerByID (waPotWinnerList [i].Winner_Name);
			if (p) {
				p.txtBetAmount.text = Utility.GetAmount (waPotWinnerList [i].Winning_Amount);
				p.imgBetAmountBG.gameObject.SetActive (true);
				p.imgWinningBetAmountBG.gameObject.SetActive (true);

				p.buyInAmount += waPotWinnerList [i].Winning_Amount;
				StartCoroutine (MoveChipToWinnerPlayer (p));
				TableTotalAmount -= waPotWinnerList [i].Winning_Amount;

				if (GetActivePlayers () > 1 || TournamentWinnerPanel.isTournamentWinnersDeclared)
					txtGameLog.text += "\nWA Pot Winner -> <color=" + APIConstants.HEX_COLOR_LIST_VIEW_HEADER + ">" + p.playerID + " -> " + GetWinningRank (waPotWinnerList [i].Winner_Rank) + "</color> -> " + Utility.GetAmount (waPotWinnerList [i].Winning_Amount);
				else
					txtGameLog.text += "\nWA Pot Winner -> <color=" + APIConstants.HEX_COLOR_LIST_VIEW_HEADER + ">" + p.playerID + "</color> -> " + Utility.GetAmount (waPotWinnerList [i].Winning_Amount);
					
				Canvas.ForceUpdateCanvases ();
				if (scrollNote.gameObject.activeSelf)
					scrollNote.verticalScrollbar.value = 0;
			}
		}
	}

	private IEnumerator MoveRakeAmountToWA (double rakeAmount)
	{
		GameObject rakeAmountGO = Instantiate (rakeAmountPrefab, txtWAPotAmount.transform.position, Quaternion.identity) as GameObject;
		rakeAmountGO.transform.GetChild (0).GetComponent<Text> ().text = Utility.GetAmount (rakeAmount);
		rakeAmountGO.transform.SetParent (transform);
		rakeAmountGO.transform.localScale = Vector3.one;

		Hashtable ht = new Hashtable ();
		ht.Add ("time", .75f);
		ht.Add ("easetype", iTween.EaseType.spring);
		ht.Add ("position", txtWAPotAmount.transform.position + Vector3.up * 10f);
		iTween.MoveTo (rakeAmountGO, ht);
		Destroy (rakeAmountGO, 1f);

		yield return new WaitForSeconds (1f);

		GameObject chip = Instantiate (tableChipObject, tableChipObject.transform.position, Quaternion.identity) as GameObject;
		chip.SetActive (true);
		chip.transform.SetParent (transform);
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

	private IEnumerator MoveChipToWinnerPlayer (WhoopAssPlayer player)
	{
		Vector3 toPos = player.txtTotalChips.transform.position;

		GameObject chip = Instantiate (tableChipObject, Vector3.one, Quaternion.identity) as GameObject;
		chip.SetActive (true);
		chip.transform.SetParent (transform);
		chip.transform.localScale = Vector3.one;
		chip.GetComponent<Image> ().SetNativeSize ();

		SoundManager.Instance.PlayChipsSound (Camera.main.transform.position);

		float i = 0;
		while (i < 1) {
			i += 2f * Time.deltaTime;

			chip.transform.position = Vector3.Lerp (tableChipObject.transform.position, toPos, i);
			yield return 0;
		}

		player.DisplayTotalChips ();

		Destroy (chip, .5f);
	}

	private IEnumerator DisplayTableChips ()
	{
		yield return new WaitForEndOfFrame ();


		int chipsAmount = (int)(TableTotalAmount);

		if (chipsAmount > 0) {
			int totalChipObjectToGenerate = CalculateTotalChipsToGenerate (chipsAmount);
			totalChipObjectToGenerate -= totalChipObject;

			if (totalChipObjectToGenerate > chipsDisplayedList.Count) {
				for (int i = 0; i < totalChipObjectToGenerate; i++) {
//					GameObject chip = Instantiate(chipPrefab, new Vector2(initialChipPosition.transform.position.x, GetYPos(++totalChipObject)), Quaternion.identity) as GameObject;
//					chipsDisplayedList.Add(chip);
//					chip.transform.SetParent(transform);
//					chip.transform.localScale = Vector3.one;

					yield return new WaitForEndOfFrame ();

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
			tableTotalAmount = value;
		}
	}

	public double WAPotAmount {
		get {
			return waPotAmount;
		}
		set {
			if (System.Math.Round (value, 2) <= 0) {
				txtWAPotAmount.text = "";
				imgWAPotAmountBG.gameObject.SetActive (false);
			} else {
				txtWAPotAmount.text = Utility.GetAmount (value);
				imgWAPotAmountBG.gameObject.SetActive (true);
			}
			waPotAmount = value;
		}
	}

	#endregion
}