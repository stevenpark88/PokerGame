using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using com.shephertz.app42.gaming.multiplayer.client.events;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
	#region PUBLIC_VARIABLES

	public Text txtTitle;

	public Button btnNote;
	public Button btnStats;
	public Button btnInfo;

	public double anteBetAmount = 5;

	public PlayerInfo dealerInfo;

	public Sprite spButtonSelected;
	public Sprite spButtonDeselected;

	public ScrollRect scrollNote;
	public ScrollRect scrollStats;
	public ScrollRect scrollInfo;
	public ScrollRect scrollChat;

	public Scrollbar leftScrollBar;
	public Scrollbar chatScrollBar;

	public Text txtGameLog;
	public Text txtChat;

	public GameObject whoopAssPlayerPrefab;

	public List<Transform> playerPositions;
	public List<Transform> playerSeats;

	public TableGamePlayer ownTablePlayer;
	public List<TableGamePlayer> allTableGamePlayers;

	public RebuyPanel rebuyPanel;

	public Image dealerFlop1Card1;
	public Image dealerFlop1Card2;

	public Image dealerFlop2Card1;
	public Image dealerFlop2Card2;

	public Image dealerFlop3Card1;
	public Image dealerFlop3Card2;

	public Text txtPlayerChips;

	public Image dealerCard1;
	public Image dealerCard2;
	public Image dealerWhoopAssCard;

	public Text txtWin;

	public CardFlipAnimation dealerFirstCard;
	public CardFlipAnimation dealerSecondCard;

	public Button btnFold;
	public Button btnAddChips;
	public Button btnSitoutNextHand;
	public Button btnBackToGame;

	public InputField ifChat;

	public int timeoutCounter;

	public bool isGameCompleted = false;

	public string currentTurnPlayerID;
	public DefaultCards defaultCards;

	public Image imgWALogo;

	public double anteAmount = 0;
	public double bet1Amount = 0;
	public double bet2Amount = 0;
	public double bet3Amount = 0;
	public double bet4Amount = 0;
	public double waCardAmount = 0;

	public double straightAmount = 0;
	public double blindAmount = 0;

	public ChatTemplates chatTemplatesPanel;

	public Button btnChatTitle;
	public Transform chatParent;
	public Transform chatOpenedPosition;
	public Transform chatClosedPosition;

	public Text GameTimer;
	public int GamePlaytime =1;
	DateTime currentDateupdate;

	public bool isSitoutForInsufficientChips;

	#endregion

	#region PRIVATE_VARIABLES

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


	public static GameManager Instance;


	#region UNITY_CALLBACKS

	// Use this for initialization
	void Awake ()
	{
		Instance = this;
		
		allTableGamePlayers = new List<TableGamePlayer> ();

		btnFold.interactable = false;
		btnAddChips.interactable = false;

		btnSitoutNextHand.interactable = false;
		btnBackToGame.interactable = false;
	}

	void OnEnable ()
	{
		timeoutCounter = 0;

		NetworkManager.Instance.JoinRoom (NetworkManager.Instance.joinedRoomID);

		NetworkManager.onPlayerInfoReceived += HandleOnPlayerInfoReceived;
		NetworkManager.onGameStartedByPlayer += HandleOnGameStartedByPlayer;
		NetworkManager.onMoveCompletedByPlayer += HandleOnMoveCompletedByPlayer;
		NetworkManager.onWinnerInfoReceived += HandleOnWinnerInfoReceived;
		NetworkManager.onPlayerLeftRoom += HandleOnPlayerLeftRoom;
		NetworkManager.onPlayerConnected += HandleOnPlayerConnected;
		NetworkManager.onChatMessageReceived += HandleOnChatMessageReceived;
		NetworkManager.onPlayerTimeoutResponseReceived += HandleOnPlayerTimoutResponseReceived;
		NetworkManager.onTableGameStarted += HandleOnTableGameStarted;
		NetworkManager.playerRequestedSitout += HandlePlayerRequestedSitout;
		NetworkManager.playerRequestedBackToGame += HandlePlayerRequestedBackToGame;

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
		NetworkManager.onTableGameStarted -= HandleOnTableGameStarted;
		NetworkManager.playerRequestedSitout -= HandlePlayerRequestedSitout;
		NetworkManager.playerRequestedBackToGame -= HandlePlayerRequestedBackToGame;

		UIManager.Instance.isRealMoney = false;

		NetworkManager.Instance.LeaveGame ();
		HideBetPanel ();
		UIManager.Instance.winReportPanel.gameObject.SetActive (false);

		foreach (TableGamePlayer tp in allTableGamePlayers)
			Destroy (tp.gameObject);
		allTableGamePlayers = new List<TableGamePlayer> ();

		ResetPlayerSeats ();

		FireResetData ();

		UIManager.Instance.winReportPanel.gameObject.SetActive (false);
		CancelInvoke("GetPerfectClock");
	}

	#endregion

	#region DELEGATE_CALLBACKS

	private void HandleOnPlayerInfoReceived (string sender, string info)
	{
		Debug.Log (info);

		btnSitoutNextHand.interactable = false;
		PlayerInfo playerInfo = JsonUtility.FromJson<PlayerInfo> (info);

		//  Need to decrease one index. Seat index from server is started from 1
		playerInfo.Player_Position--;

		if (playerInfo.Player_Name.Equals (Constants.FIELD_PLAYER_NAME_DEALER)) {
			dealerInfo = playerInfo;
		} else {
			//	Need to decrease one index. Dealer is always on first seat.
			playerInfo.Player_Position--;

			TableGamePlayer player = GetPlayerByID (playerInfo.Player_Name);
			if (player) {
				player.playerID = playerInfo.Player_Name;
				player.buyinChips = playerInfo.Player_BuyIn_Chips;
				player.card1 = playerInfo.Card1;
				player.card2 = playerInfo.Card2;
				player.playerInfo = playerInfo;
				player.seatIndex = playerInfo.Player_Position;
				player.totalChips = playerInfo.Player_Total_Play_Chips;
				player.totalRealMoney = playerInfo.Player_Total_Real_Chips;
				player.SetPlayerName ();

				if (playerInfo.Player_Status == (int)PLAYER_STATUS.SIT_OUT) {
					player.imgSitout.gameObject.SetActive (true);
				}

				player.DisplayTotalChips ();
				return;
			}

			if (playerInfo.Player_Status == (int)PLAYER_STATUS.ELIMINATED &&
			    playerInfo.Player_Name.Equals (NetworkManager.Instance.playerID)) {
//				if (Application.platform == RuntimePlatform.WebGLPlayer)
//					UIManager.Instance.BackToLobby ();
//				else {
//					LoginScript.loginDetails = null;
//					SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
//				}

				UIManager.Instance.backConfirmationPanel.OnYesButtonTap ();

				return;
			}

			GameObject obj = Instantiate (whoopAssPlayerPrefab, playerSeats [playerInfo.Player_Position].position, Quaternion.identity) as GameObject;
			TableGamePlayer tableGamePlayer = obj.GetComponent<TableGamePlayer> ();

			tableGamePlayer.playerID = playerInfo.Player_Name;
			tableGamePlayer.buyinChips = playerInfo.Player_BuyIn_Chips;
			tableGamePlayer.card1 = playerInfo.Card1;
			tableGamePlayer.card2 = playerInfo.Card2;
			tableGamePlayer.playerInfo = playerInfo;
			tableGamePlayer.seatIndex = playerInfo.Player_Position;
			tableGamePlayer.totalChips = playerInfo.Player_Total_Play_Chips;
			tableGamePlayer.totalRealMoney = playerInfo.Player_Total_Real_Chips;
			tableGamePlayer.SetPlayerName ();
			tableGamePlayer.GetComponent<CanvasGroup> ().alpha = 0f;

			if (playerInfo.Player_Status == (int)PLAYER_STATUS.SIT_OUT) {
				tableGamePlayer.imgSitout.gameObject.SetActive (true);
			}

			obj.transform.SetParent (playerSeats [playerInfo.Player_Position]);
			obj.transform.localScale = Vector3.one;
			allTableGamePlayers.Add (tableGamePlayer);

			if (playerInfo.Player_Name.Equals (NetworkManager.Instance.playerID)) {
				ownTablePlayer = tableGamePlayer;
				CentralizeOwnPlayer ();
				btnAddChips.interactable = true;
			} else {
				tableGamePlayer.SetChipsPosition ();
			}

			if (tableGamePlayer.buyinChips <= 0)
				tableGamePlayer.GetComponent<CanvasGroup> ().alpha = .4f;
			else
				tableGamePlayer.GetComponent<CanvasGroup> ().alpha = 1f;

			HandleWaitingPlayer (playerInfo);
		}
	}

	private void HandleOnGameStartedByPlayer (string sender, string gameStarter)
	{
		currentTurnPlayerID = gameStarter;

		if (gameStarter.Equals (NetworkManager.Instance.playerID)) {
			//UIManager.Instance.anteAndBlindBetPanel.gameObject.SetActive (true);
			DisplayAppropriateBetPanel ();
		} else
			HideBetPanel ();

		foreach (TableGamePlayer tp in allTableGamePlayers) {
			if (tp.playerID.Equals (gameStarter) && tp.playerInfo.Player_Status != (int)PLAYER_STATUS.SIT_OUT)
				tp.DisplayTurnTimer ();
			else
				tp.HideTurnTimer ();
		}
	}

	private void HandleOnMoveCompletedByPlayer (MoveEvent move)
	{
		currentTurnPlayerID = move.getNextTurn ();
	}

	private void HandleOnWinnerInfoReceived (string sender, string winnerInfo)
	{
		StartCoroutine (ResetGameAfterSomeTime ());
		
		if (!btnBackToGame.interactable)
			btnSitoutNextHand.interactable = true;

		if (ownTablePlayer != null && ownTablePlayer.playerInfo.Player_Status == (int)PLAYER_STATUS.SIT_OUT)
			sitOutCounter++;

		isGameCompleted = true;
		currentTurnPlayerID = "";

		HideBetPanel ();

		btnAddChips.interactable = true;

		if (winnerInfo == null)
			return;

		WinnerReport winReport = new WinnerReport (winnerInfo);

		if (ownTablePlayer.playerInfo.Player_Status != (int)PLAYER_ACTION.FOLD) {
			UIManager.Instance.winReportPanel.DisplayWinReport (winReport);
//			if (winReport.Winner.Winner_Name.Equals (ownTablePlayer.playerID)) {
//				ownTablePlayer.buyinChips += winReport.Winner.winner_balance;
//				ownTablePlayer.totalChips += winReport.Winner.winner_balance;
//			}
//			ownTablePlayer.DisplayTotalChips ();
		}

		DisplayWinnerAnimation ();

		if (dealerFirstCard)
			dealerFirstCard.DisplayCardWithoutAnimation (dealerInfo.Card1);
		if (dealerSecondCard)
			dealerSecondCard.DisplayCardWithoutAnimation (dealerInfo.Card2);

		dealerWhoopAssCard.GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (dealerInfo.WACard);
	}

	private void HandleOnPlayerLeftRoom (RoomData roomData, string playerID)
	{
		if (NetworkManager.Instance.joinedRoomID.Equals (roomData.getId ())) {
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
//				UIManager.Instance.gamePlayPanel.gameObject.SetActive (false);
//				UIManager.Instance.roomsPanel.gameObject.SetActive (true);
//			}

			UIManager.Instance.backConfirmationPanel.OnYesButtonTap ();
		}
	}

	private void HandleOnTableGameStarted ()
	{
		isGameCompleted = false;
		RoundController.GetInstance ().currentTableGameRound = TABLE_GAME_ROUND.START;
	}

	private void HandlePlayerRequestedSitout (string playerID)
	{
		if (playerID == "")
			return;

		TableGamePlayer player = GetPlayerByID (playerID);
		if (player != null) {
			txtGameLog.text += "\n" + playerID + Constants.MESSAGE_PLAYER_IS_SITOUT;
			Canvas.ForceUpdateCanvases ();
			if (scrollNote.gameObject.activeSelf)
				scrollNote.verticalScrollbar.value = 0;

			if (player.playerID.Equals (NetworkManager.Instance.playerID)) {
				DebugLog.Log ("Back to game button enabled");
				btnBackToGame.interactable = true;
			}
		}
	}

	private void HandlePlayerRequestedBackToGame (string playerID)
	{
		if (playerID == "")
			return;

		TableGamePlayer player = GetPlayerByID (playerID);

		if (player != null) {
			txtGameLog.text += "\n" + playerID + Constants.MESSAGE_PLAYER_IS_BACK_TO_GAME;
			Canvas.ForceUpdateCanvases ();
			if (scrollNote.gameObject.activeSelf)
				scrollNote.verticalScrollbar.value = 0;

			if (isGameCompleted &&
			    playerID.Equals (NetworkManager.Instance.playerID)) {
				btnSitoutNextHand.interactable = true;
				sitOutCounter = 0;
			}
		}
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

		btnBackToGame.interactable = false;
//		if (isGameCompleted)
//			btnSitoutNextHand.interactable = true;
//		else
//			btnSitoutNextHand.interactable = false;
		NetworkManager.Instance.SendRequestToServer (Constants.REQUEST_FOR_BACK_TO_GAME);
	}

	public void OnSitOutNextHandButtonTap ()
	{
		DebugLog.Log ("Sit out next hand button tapped");

		btnSitoutNextHand.interactable = false;

		NetworkManager.Instance.SendRequestToServer (Constants.REQUEST_FOR_SIT_OUT);
	}

	public void OnFoldButtonTap ()
	{
		DebugLog.Log ("Fold button tapped");

		PlayerAction action = new PlayerAction ();
		action.Action = (int)PLAYER_ACTION.FOLD;
		action.Player_Name = NetworkManager.Instance.playerID;

		ownTablePlayer.playerInfo.Player_Status = (int)PLAYER_STATUS.FOLDED;

		NetworkManager.Instance.SendPlayerAction (action);

		HideBetPanel ();
//		btnSitoutNextHand.interactable = true;
	}

	public void OnAddChipsButtonTap ()
	{
		DebugLog.Log ("Add chips button tapped");

		if (UIManager.Instance.isRealMoney)
			rebuyPanel.DisplayRebuyPanel (ownTablePlayer.totalRealMoney - ownTablePlayer.buyinChips, Constants.TABLE_GAME_REAL_MIN_MONEY);
		else
			rebuyPanel.DisplayRebuyPanel (ownTablePlayer.totalChips - ownTablePlayer.buyinChips, Constants.TABLE_GAME_PLAY_MIN_CHIPS);
//		NetworkManager.Instance.SendRequestToServer (Constants.REQUEST_FOR_ADD_CHIPS + 10000);
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
//		if (chatTemplatesPanel.gameObject.activeSelf)
//			chatTemplatesPanel.gameObject.SetActive (false);
//		else
//			chatTemplatesPanel.gameObject.SetActive (true);
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

	public TableGamePlayer GetPlayerByID (string playerID)
	{
		foreach (TableGamePlayer p in allTableGamePlayers) {
			if (p.playerID.Equals (playerID))
				return p;
		}

		return null;
	}

	public void DisplayAppropriateBetPanel ()
	{
		HideBetPanel ();

		RoundController roundController = RoundController.GetInstance ();
		switch (roundController.currentTableGameRound) {
		case TABLE_GAME_ROUND.FIRST_BET:
			UIManager.Instance.bet1RoundSelectionPanel.SetButtons (anteBetAmount);
			break;
		case TABLE_GAME_ROUND.PLAY:
			UIManager.Instance.bet4RoundSelectionPanel.SetButtons (anteBetAmount);
			break;
		case TABLE_GAME_ROUND.SECOND_BET:
			UIManager.Instance.bet2RoundSelectionPanel.SetButtons (anteBetAmount);
			break;
		case TABLE_GAME_ROUND.START:
			UIManager.Instance.anteAndBlindBetPanel.gameObject.SetActive (true);
			break;
		case TABLE_GAME_ROUND.THIRD_BET:
			UIManager.Instance.bet3RoundSelectionPanel.SetButtons (anteBetAmount);
			break;
		case TABLE_GAME_ROUND.WHOOPASS:
			UIManager.Instance.whoopAssCardRoundPanel.SetTitle (anteBetAmount);
			break;
		}

		btnFold.interactable = true;

		if (ownTablePlayer.totalChips - ownTablePlayer.buyinChips >= Constants.TABLE_GAME_PLAY_MIN_CHIPS)
			btnAddChips.interactable = true;
		else
			btnAddChips.interactable = false;
	}

	public void HideBetPanel ()
	{
		UIManager.Instance.anteAndBlindBetPanel.gameObject.SetActive (false);
		UIManager.Instance.bet1RoundSelectionPanel.gameObject.SetActive (false);
		UIManager.Instance.bet2RoundSelectionPanel.gameObject.SetActive (false);
		UIManager.Instance.bet3RoundSelectionPanel.gameObject.SetActive (false);
		UIManager.Instance.whoopAssCardRoundPanel.gameObject.SetActive (false);
		UIManager.Instance.bet4RoundSelectionPanel.gameObject.SetActive (false);
		UIManager.Instance.straightOrBetterBetPanel.gameObject.SetActive (false);

		btnFold.interactable = false;
	}

	public void DisplayPlayerTotalChips (double chips)
	{
		if (UIManager.Instance.isRealMoney)
			txtPlayerChips.text = Utility.GetCommaSeperatedAmount (chips, true);
		else
			txtPlayerChips.text = Utility.GetCommaSeperatedAmount (chips);
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
			//Debug.Log ("TICK_TIME = > "+PlayerPrefs.GetString ("TICK_TIME"));
		} else {
			//Debug.Log ("TICK_TIME = >222 "+PlayerPrefs.GetString ("TICK_TIME"));
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
		//NetworkManager.Instance.SendRequestToServer("");
	}

	private void DisplayWinnerAnimation ()
	{
		//		iTween.ShakePosition (txtWin.gameObject, new Vector3(3,3,0), 2f);
	}

	private void HandleWaitingPlayer (PlayerInfo playerInfo)
	{
		if (playerInfo.Player_Name.Equals (NetworkManager.Instance.playerID)) {
			RoundController.GetInstance ().currentTableGameRound = GetRound (playerInfo.Current_Round);

			if (playerInfo.Game_Status == (int)GAME_STATUS.RUNNING ||
			    playerInfo.Game_Status == (int)GAME_STATUS.CARD_DISTRIBUTE) {
				if (playerInfo.Player_Status == (int)PLAYER_STATUS.WAITING ||
				    playerInfo.Player_Status == (int)PLAYER_ACTION.ACTION_WAITING_FOR_GAME) {
					GeneratePlayerCardsForWaitingPlayer ();
					GenerateDealerFlopCardsForWaitingPlayer ();
				}
			}
		}
	}

	private TABLE_GAME_ROUND GetRound (int round)
	{
		switch (round) {
		case (int)TABLE_GAME_ROUND.FIRST_BET:
			return TABLE_GAME_ROUND.FIRST_BET;
		case (int)TABLE_GAME_ROUND.SECOND_BET:
			return TABLE_GAME_ROUND.SECOND_BET;
		case (int)TABLE_GAME_ROUND.START:
			return TABLE_GAME_ROUND.START;
		case (int)TABLE_GAME_ROUND.THIRD_BET:
			return TABLE_GAME_ROUND.THIRD_BET;
		case (int)TABLE_GAME_ROUND.WHOOPASS:
			return TABLE_GAME_ROUND.WHOOPASS;
		case (int)TABLE_GAME_ROUND.PLAY:
			return TABLE_GAME_ROUND.PLAY;
		}
		return TABLE_GAME_ROUND.START;
	}

	private void GeneratePlayerCardsForWaitingPlayer ()
	{
		RoundController.GetInstance ().GenerateTablePlayerCardsForWaitingPlayer ();
	}

	private void GenerateDealerFlopCardsForWaitingPlayer ()
	{
		RoundController.GetInstance ().GenerateDealerFlopCardsForWaitingPlayer ();
	}

	private void DestroyPlayer (string playerID)
	{
		TableGamePlayer p = GetPlayerByID (playerID);

		if (p) {
			allTableGamePlayers.Remove (p);
			Destroy (p.gameObject);
		}
	}

	private void CentralizeOwnPlayer ()
	{
		int posToMove = GetOwnPlayerPositionToMoveCenter ();

		if (posToMove >= Constants.TABLE_GAME_MAX_PLAYERS) {
			return;
		}

		for (int i = 0; i < playerSeats.Count; i++) {
			int posIndex = i + posToMove;

			if (posIndex > Constants.TABLE_GAME_MAX_PLAYERS - 1)
				posIndex = posIndex - Constants.TABLE_GAME_MAX_PLAYERS;

			playerSeats [i].transform.SetParent (playerPositions [posIndex]);

			playerSeats [i].position = playerPositions [posIndex].position;
		}

		foreach (TableGamePlayer p in allTableGamePlayers)
			p.SetChipsPosition ();
	}

	private int GetOwnPlayerPositionToMoveCenter ()
	{
		int movePos = Constants.TABLE_GAME_MAX_PLAYERS - ownTablePlayer.playerInfo.Player_Position;
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

	private void CheckForLowChips ()
	{
		if (UIManager.Instance.isRealMoney) {
			if (ownTablePlayer.buyinChips < Constants.TABLE_GAME_REAL_MIN_MONEY) {
				if (ownTablePlayer.totalChips > Constants.TABLE_GAME_REAL_MIN_MONEY) {
					isSitoutForInsufficientChips = true;
					OnSitOutNextHandButtonTap ();
					UIManager.Instance.DisplayNotEnoughChipsPanel (true);
				} else
					UIManager.Instance.DisplayNotEnoughChipsPanel (false);
			}
		} else {
			if (ownTablePlayer.buyinChips < Constants.TABLE_GAME_PLAY_MIN_CHIPS) {
				if (ownTablePlayer.totalChips > Constants.TABLE_GAME_PLAY_MIN_CHIPS) {
					isSitoutForInsufficientChips = true;
					OnSitOutNextHandButtonTap ();
					UIManager.Instance.DisplayNotEnoughChipsPanel (true);
				} else
					UIManager.Instance.DisplayNotEnoughChipsPanel (false);
			}
		}
	}

	#endregion

	#region COROUTINES

	private IEnumerator ResetGameAfterSomeTime ()
	{
		yield return new WaitForSeconds (Constants.TABLE_GAME_RESET_TIMER);

		CheckForLowChips ();

		if (ownTablePlayer.totalChips >= Constants.TABLE_GAME_PLAY_MIN_CHIPS)
			btnAddChips.interactable = true;
		else
			btnAddChips.interactable = false;

		FireResetData ();
		UIManager.Instance.winReportPanel.gameObject.SetActive (false);

		foreach (Transform t in dealerCard1.transform) {
			Destroy (t.gameObject);
		}
		foreach (Transform t in dealerCard2.transform) {
			Destroy (t.gameObject);
		}
		dealerWhoopAssCard.GetComponent<CardFlipAnimation> ().ResetImage ();
		dealerWhoopAssCard.gameObject.SetActive (false);

		if (txtGameLog.text.Length > 50000) {
			txtGameLog.text = "";
			Canvas.ForceUpdateCanvases ();
			if (scrollNote.gameObject.activeSelf)
				scrollNote.verticalScrollbar.value = 0;
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
}