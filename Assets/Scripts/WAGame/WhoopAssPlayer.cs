using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System.Collections.Generic;

public class WhoopAssPlayer : MonoBehaviour
{
	#region PUBLIC_VARIABLES

	public string playerID;
	public int seatIndex;

	public bool isOwnPlayer;

	public double buyInAmount;
	public double totalChips;
	public double totalRealMoney;
	public double blindAmount;
	public double betAmount;
	public double betAmountInPot;
	public double whoopAssCardAmount;

	public Transform txtBetAmountPosition;

	public string card1;
	public string card2;

	public PlayerInfo playerInfo;

	public bool isDealer;
	public bool isSmallBlind;
	public bool isBigBlind;

	public Image imgProfile;

	public Image imgTurnDisplayer;

	public Text txtPlayerName;
	public Text txtTotalChips;
	public Text txtBetAmount;

	public Transform whoopAssCardUpPosition;
	public Transform whoopAssCardDownPosition;

	public Transform card1Position;
	public Transform card2Position;

	public Button btnUp;
	public Button btnDown;

	public Sprite ownPlayerSprite;
	public Sprite otherPlayerSprite;

	public Outline playerTurnBorder;

	public Text txtWinnerRank;

	public Transform whoopAssUpCardInitialPos;
	public Transform whoopAssDownCardInitialPos;

	public GameObject dealerObject;
	public Image imgAbsentPlayer;
	public Sprite spAbsent;
	public Sprite spSitout;

	public Image imgBetAmountBG;
	public Image imgWinningBetAmountBG;

	public GameObject chipPrefab;
	private int totalChipObject;
	private int totalRedChipObject;
	private int totalGreenChipObject;
	private int totalBlueChipObject;

	public GameObject redChipPrefab;
	public GameObject greenChipPrefab;
	public GameObject blueChipPrefab;

	public Transform redInitialChip;
	public Transform greenInitialChip;
	public Transform blueInitialChip;

	public List<Transform> betChipsPositionList;
	public List<Transform> betAmountPositionsList;
	public GameObject betAmountParentObj;

	public List<GameObject> chipsDisplayedList;

	public bool isEliminated;

	#endregion

	#region PRIVATE_VARIABLES

	private List<GameObject> redChipsDisplayedList;
	private List<GameObject> greenChipsDisplayedList;
	private List<GameObject> blueChipsDisplayedList;

	private bool isProfilePicLoaded = false;

	#endregion

	#region UNITY_CALLBACKS

	// Use this for initialization
	void OnEnable ()
	{
		HandleOnResetData ();
		StartCoroutine (SetPlayerData ());

		NetworkManager.onActionResponseReceived += HandleOnActionResponseReceived;
		WhoopAssGame.resetData += HandleOnResetData;
		NetworkManager.onMoveCompletedByPlayer += HandleOnMoveCompletedByPlayer;
		NetworkManager.onRoundComplete += HandleOnRoundComplete;
		NetworkManager.onWinnerInfoReceived += HandleOnWinnerInfoReceived;
		NetworkManager.onActionHistoryReceived += HandleOnActionHistoryReceived;
		NetworkManager.onRebuyActionResponseReceived += HandleOnRebuyActionResponseReceived;
		NetworkManager.onBlindPlayerResponseReceived += HandleOnBlindPlayerResponseReceived;
		RoundController.collectBlindAmount += HandleOnCollectBlindAmount;
		NetworkManager.onWinnerInfoReceived += HandleOnRoundComplete;       //  To transfer bet amount to pot
		NetworkManager.playerRequestedSitout += HandlePlayerReqestedSitout;
		NetworkManager.playerRequestedBackToGame += HandlePlayerReqestedBackToGame;
		RoundController.cardDistributionFinished += HandleCardDistributionFinished;
		NetworkManager.onGameStartedByPlayer += HandleOnGameStartedByPlayer;
	}

	void OnDisable ()
	{
		NetworkManager.onActionResponseReceived -= HandleOnActionResponseReceived;
		WhoopAssGame.resetData -= HandleOnResetData;
		NetworkManager.onMoveCompletedByPlayer -= HandleOnMoveCompletedByPlayer;
		NetworkManager.onRoundComplete -= HandleOnRoundComplete;
		NetworkManager.onWinnerInfoReceived -= HandleOnWinnerInfoReceived;
		NetworkManager.onActionHistoryReceived -= HandleOnActionHistoryReceived;
		NetworkManager.onRebuyActionResponseReceived -= HandleOnRebuyActionResponseReceived;
		NetworkManager.onBlindPlayerResponseReceived -= HandleOnBlindPlayerResponseReceived;
		RoundController.collectBlindAmount -= HandleOnCollectBlindAmount;
		NetworkManager.onWinnerInfoReceived -= HandleOnRoundComplete;
		NetworkManager.playerRequestedSitout -= HandlePlayerReqestedSitout;
		NetworkManager.playerRequestedBackToGame -= HandlePlayerReqestedBackToGame;
		RoundController.cardDistributionFinished -= HandleCardDistributionFinished;
		NetworkManager.onGameStartedByPlayer -= HandleOnGameStartedByPlayer;

		DestroyCards ();
	}

	#endregion

	#region DELEGATE_CALLBACKS

	private void HandleOnActionResponseReceived (string sender, string response)
	{
		HideTurnTimer ();
		ActionResponse ar = JsonUtility.FromJson<ActionResponse> (response);

		if (playerID.Equals (ar.Player_Name)) {
			buyInAmount = ar.Player_BuyIn_Chips;
			totalChips = ar.Player_Total_Play_Chips;
			totalRealMoney = ar.Player_Total_Real_Chips;
			DisplayTotalChips ();

			betAmount += ar.Bet_Amount;
			DisplayBetAmount ();

			if (ar.Action == (int)PLAYER_ACTION.CHECK)
				SoundManager.Instance.PlayCheckActionSound (Camera.main.transform.position);
			else if (ar.Action == (int)PLAYER_ACTION.BET || ar.Action == (int)PLAYER_ACTION.CALL)
				SoundManager.Instance.PlayBetCallSound (Camera.main.transform.position);

			if (ar.Action == (int)PLAYER_ACTION.ACTION_WA_UP) {
				whoopAssCardAmount = ar.Bet_Amount;
				betAmount -= whoopAssCardAmount;
				DisplayUpWhoopAssCard ();
				//btnDown.gameObject.SetActive(false);
				btnDown.transform.GetChild (0).gameObject.SetActive (false);

				SoundManager.Instance.PlayBetCallSound (Camera.main.transform.position);
			} else if (ar.Action == (int)PLAYER_ACTION.ACTION_WA_DOWN) {
				whoopAssCardAmount = ar.Bet_Amount;
				betAmount -= whoopAssCardAmount;
				DisplayDownWhoopAssCard ();
				//btnUp.gameObject.SetActive(false);
				btnUp.transform.GetChild (0).gameObject.SetActive (false);

				SoundManager.Instance.PlayBetCallSound (Camera.main.transform.position);
			} else if (ar.Action == (int)PLAYER_ACTION.ACTION_WA_NO) {
				btnUp.transform.GetChild (0).gameObject.SetActive (false);
				btnDown.transform.GetChild (0).gameObject.SetActive (false);

				SoundManager.Instance.PlayCheckActionSound (Camera.main.transform.position);
			}

			if (ar.Action == (int)PLAYER_ACTION.TIMEOUT) {
				GetComponent<CanvasGroup> ().alpha = .4f;
				if (playerInfo.Player_Status != (int)PLAYER_STATUS.SIT_OUT)
					playerInfo.Player_Status = (int)PLAYER_ACTION.TIMEOUT;

				SoundManager.Instance.PlayFoldActionSound (Camera.main.transform.position);
			} else if (ar.Action == (int)PLAYER_ACTION.FOLD) {
				GetComponent<CanvasGroup> ().alpha = .4f;
				if (playerInfo.Player_Status != (int)PLAYER_STATUS.SIT_OUT)
					playerInfo.Player_Status = (int)PLAYER_ACTION.FOLD;

				SoundManager.Instance.PlayFoldActionSound (Camera.main.transform.position);
			} else if (ar.Action == (int)PLAYER_ACTION.ALLIN) {
				if (playerInfo.Player_Status != (int)PLAYER_STATUS.SIT_OUT)
					playerInfo.Player_Status = (int)PLAYER_ACTION.ALLIN;

				SoundManager.Instance.PlayAllinActionSound (Camera.main.transform.position);
			} else if (ar.Action == (int)PLAYER_ACTION.RAISE) {
				WhoopAssGame.Instance.raisePerRoundCounter++;

				SoundManager.Instance.PlayBetCallSound (Camera.main.transform.position);
			}

			txtPlayerName.text = GetPlayerActionInString (ar.Action);
			txtPlayerName.color = ar.Action == (int)PLAYER_ACTION.FOLD ? Color.red : Color.white;
			txtTotalChips.text = Utility.GetAmount (betAmount);
			txtTotalChips.color = Color.white;

			//			playerInfo.Player_Status = ar.Action;

			HistoryManager.GetInstance ().AddHistory (playerID, "name", RoundController.GetInstance ().currentWhoopAssGameRound, ar.Bet_Amount, betAmount, GetPlayerAction (ar.Action));
		} else {
			if (playerInfo.Player_Status == (int)PLAYER_STATUS.ACTIVE)
				DisplayPlayerData ();
		}
	}

	private void HandleOnResetData ()
	{
		//if (playerInfo.Player_Status == (int)PLAYER_STATUS.ACTIVE ||
		//    playerInfo.Player_Status == (int)PLAYER_STATUS.ABSENT ||
		//    playerInfo.Player_Status == (int)PLAYER_STATUS.SIT_OUT)
		GetComponent<CanvasGroup> ().alpha = 1f;

		whoopAssCardUpPosition.GetComponent<CardFlipAnimation> ().ResetImage ();
//        whoopAssCardUpPosition.GetComponent<Image> ().sprite = Resources.Load<Sprite> (Constants.RESOURCE_BACK_CARD);
		whoopAssCardUpPosition.gameObject.SetActive (false);
		whoopAssCardDownPosition.GetComponent<CardFlipAnimation> ().ResetImage ();
//        whoopAssCardDownPosition.GetComponent<Image>().sprite = Resources.Load<Sprite>(Constants.RESOURCE_BACK_CARD);
		whoopAssCardDownPosition.gameObject.SetActive (false);
		txtWinnerRank.transform.parent.gameObject.SetActive (false);

		betAmount = betAmountInPot = whoopAssCardAmount = 0;
		DisplayBetAmount ();
		imgWinningBetAmountBG.gameObject.SetActive (false);

		//btnUp.gameObject.SetActive(true);
		btnUp.transform.GetChild (0).gameObject.SetActive (true);

		//btnDown.gameObject.SetActive(true);
		btnDown.transform.GetChild (0).gameObject.SetActive (true);

		DestroyCards ();
		DisplayPlayerData ();

		DestroyAllChips ();

		if (isEliminated)
			txtPlayerName.text = "<color=red>ELIMINATED</color>";
	}

	private void HandleOnMoveCompletedByPlayer (MoveEvent moveEvent)
	{
//		HideTurnTimer ();
//
//		if (playerID.Equals (moveEvent.getNextTurn ()))
//			DisplayTurnTimer ();
	}

	private void HandleOnRoundComplete (string sender, string roundInfo)
	{
		betAmountInPot = betAmount;
		whoopAssCardAmount = 0;

		GameRound round = JsonUtility.FromJson<GameRound> (roundInfo);
		if (round.Round == (int)WHOOPASS_GAME_ROUND.THIRD_FLOP) {
			OpenWhoopAssCard ();
		}

		DisplayBetAmount ();

		StartCoroutine (MoveChipsToPot ());
	}

	private void HandleOnWinnerInfoReceived (string sender, string winnerInfo)
	{
		DisplayPlayerData ();

		OpenWhoopAssCard ();
		HideTurnTimer ();

		txtBetAmount.text = "";
		imgBetAmountBG.gameObject.SetActive (false);

		if (WhoopAssGame.Instance.GetActivePlayers () < Constants.MIN_PLAYER_TO_START_GAME ||
		    (playerInfo.Player_Status != (int)PLAYER_STATUS.ACTIVE &&
		    playerInfo.Player_Status != (int)PLAYER_ACTION.ALLIN &&
		    playerInfo.Player_Status != (int)PLAYER_STATUS.ABSENT) ||
		    playerID.Equals (WhoopAssGame.Instance.ownWhoopAssPlayer.playerID))
			return;

		whoopAssCardUpPosition.GetComponent<CardFlipAnimation> ().PlayAnimation (playerInfo.WACard);
		whoopAssCardDownPosition.GetComponent<CardFlipAnimation> ().PlayAnimation (playerInfo.WACard);

		card1Position.GetChild (0).GetComponent<CardFlipAnimation> ().PlayAnimation (card1);
		card2Position.GetChild (0).GetComponent<CardFlipAnimation> ().PlayAnimation (card2);
	}

	private void HandleOnActionHistoryReceived (string sender, string actionHistory)
	{
		//DebugLog.LogWarning (actionHistory);
		JSON_Object obj = new JSON_Object (actionHistory);

		int roundStatus = obj.getInt (Constants.FIELD_ACTION_HISTORY_ROUND_STATUS);
		int round = obj.getInt (Constants.FIELD_ACTION_HISTORY_ROUND);

		JSONArray turnsArray = obj.getJSONArray (Constants.FIELD_ACTION_HISTORY_TURNS);

		for (int i = 0; i < turnsArray.Count (); i++) {
			ActionResponse ar = JsonUtility.FromJson<ActionResponse> (turnsArray.getString (i));

			if (playerID.Equals (ar.Player_Name)) {
				betAmount += ar.Bet_Amount;
				buyInAmount = ar.Player_BuyIn_Chips;

				if (ar.Action == (int)PLAYER_ACTION.FOLD ||
				    ar.Action == (int)PLAYER_ACTION.TIMEOUT)
					GetComponent<CanvasGroup> ().alpha = .4f;
			}
		}

		DisplayBetAmount ();
		DisplayTotalChips ();
	}

	private void HandleOnRebuyActionResponseReceived (string sender, string rebuyInfo)
	{
		if (sender.Equals (Constants.WHOOPASS_SERVER_NAME)) {
			RebuyAction action = JsonUtility.FromJson<RebuyAction> (rebuyInfo);
			if (playerID.Equals (action.Player_Name)) {
				buyInAmount = action.Player_BuyIn_Chips;
				totalChips = action.Player_Total_Play_Chips;
				totalRealMoney = action.Player_Total_Real_Chips;
				DisplayTotalChips ();

				if (action.Player_Name.Equals (NetworkManager.Instance.playerID)) {
					WhoopAssGame.Instance.btnRebuy.gameObject.SetActive (false);
					WhoopAssGame.Instance.btnAddChips.gameObject.SetActive (false);
					WhoopAssGame.Instance.rebuyPanel.gameObject.SetActive (false);
				}
			}
		}
	}

	private void HandleOnBlindPlayerResponseReceived (string sender, string blindPlayerInfo)
	{
		BlindPlayer blind = JsonUtility.FromJson<BlindPlayer> (blindPlayerInfo);

//		if (WhoopAssGame.Instance.currentGameStatus == GAME_STATUS.RUNNING)
//			return;

		isBigBlind = isSmallBlind = isDealer = false;

		blindAmount = 0;

		dealerObject.SetActive (false);

		if (blind.Big_Blind.Equals (this.playerID)) {
			isBigBlind = true;

			blindAmount = blind.SBAmount * 2;

			if (blindAmount > buyInAmount) {
				blindAmount = buyInAmount;
				if (playerInfo.Player_Status != (int)PLAYER_STATUS.SIT_OUT)
					playerInfo.Player_Status = (int)PLAYER_ACTION.ALLIN;
			}

//          HistoryManager.GetInstance().AddHistory(this.playerID, txtPlayerName.text, WHOOPASS_GAME_ROUND.START, blindAmount, betAmount, PLAYER_ACTION.BIG_BLIND);
		}
		if (blind.Player_Dealer.Equals (this.playerID)) {
			isDealer = true;
			dealerObject.SetActive (true);
		}
		if (blind.Small_Blind.Equals (this.playerID)) {
			isSmallBlind = true;

			blindAmount = blind.SBAmount;

			if (blindAmount > buyInAmount) {
				blindAmount = buyInAmount;
				if (playerInfo.Player_Status != (int)PLAYER_STATUS.SIT_OUT)
					playerInfo.Player_Status = (int)PLAYER_ACTION.ALLIN;
			}
            
//          HistoryManager.GetInstance().AddHistory(this.playerID, txtPlayerName.text, WHOOPASS_GAME_ROUND.START, blindAmount, betAmount, PLAYER_ACTION.SMALL_BLIND);
		}

		//		txtBetAmount.text = Utility.GetCurrencyPrefix() + betAmount;
//        DisplayTotalChips();
//        buyInAmount -= betAmount;
	}

	private void HandleOnCollectBlindAmount ()
	{
//		DestroyCards ();

		if (betAmount <= 0)
			betAmount = blindAmount;
		buyInAmount -= blindAmount;
		DisplayBetAmount ();
		txtTotalChips.text = Utility.GetAmount (buyInAmount);
		DisplayTotalChips ();

		if (isSmallBlind)
			HistoryManager.GetInstance ().AddHistory (this.playerID, txtPlayerName.text, WHOOPASS_GAME_ROUND.START, blindAmount, betAmount, PLAYER_ACTION.SMALL_BLIND);
		else if (isBigBlind)
			HistoryManager.GetInstance ().AddHistory (this.playerID, txtPlayerName.text, WHOOPASS_GAME_ROUND.START, blindAmount, betAmount, PLAYER_ACTION.BIG_BLIND);
		
		blindAmount = 0;
	}

	private void HandlePlayerReqestedSitout (string sender)
	{
		if (playerID.Equals (sender)) {
			playerInfo.Player_Status = (int)PLAYER_STATUS.SIT_OUT;
			imgAbsentPlayer.sprite = spSitout;
			imgAbsentPlayer.gameObject.SetActive (true);
			imgAbsentPlayer.color = Color.red;

			if (playerID.Equals (NetworkManager.Instance.playerID))
				WhoopAssGame.Instance.recentlyBackToGame = false;
		}
	}

	private void HandlePlayerReqestedBackToGame (string sender)
	{
		if (playerID.Equals (sender)) {
			imgAbsentPlayer.gameObject.SetActive (false);

			if (playerID.Equals (NetworkManager.Instance.playerID))
				WhoopAssGame.Instance.recentlyBackToGame = true;
		}
	}

	private void HandleCardDistributionFinished ()
	{
		if (!isProfilePicLoaded)
			StartCoroutine (GetProfilePic ());
	}

	private void HandleOnGameStartedByPlayer (string sender, string gameStarter)
	{
		if (sender.Equals (Constants.WHOOPASS_SERVER_NAME)) {
			if (playerID.Equals (gameStarter)) {
				DisplayPlayerData ();
				DisplayTurnTimer ();
			} else
				HideTurnTimer ();
		}
	}

	#endregion


	#region PUBLIC_METHODS

	public void DisplayTurnTimer ()
	{
		if (!WhoopAssGame.Instance.isGameCompleted) {
			if (playerInfo.Player_Status != (int)PLAYER_ACTION.ACTION_WAITING_FOR_GAME &&
			    playerInfo.Player_Status != (int)PLAYER_ACTION.FOLD &&
			    playerInfo.Player_Status != (int)PLAYER_ACTION.TIMEOUT &&
			    playerInfo.Player_Status != (int)PLAYER_ACTION.ALLIN) {
				StartCoroutine ("TurnTimer");
			}

			if (RoundController.GetInstance ().currentWhoopAssGameRound == WHOOPASS_GAME_ROUND.WHOOPASS_CARD &&
			    playerInfo.Player_Status == (int)PLAYER_ACTION.ALLIN) {
				StartCoroutine ("TurnTimer");
			}
		}
	}

	public void HideTurnTimer ()
	{
		StopCoroutine ("TurnTimer");
		StopCoroutine ("BlinkOutline");
		StopCoroutine ("PlayTickSound");
		imgTurnDisplayer.fillAmount = 0;
		playerTurnBorder.enabled = false;
	}

	public void DisplayBetAmount ()
	{
		double amt = ((betAmount + whoopAssCardAmount) - betAmountInPot);
		if (amt == 0) {
			txtBetAmount.text = "";
			imgBetAmountBG.gameObject.SetActive (false);
		} else {
			txtBetAmount.text = Utility.GetAmount (amt);
			imgBetAmountBG.gameObject.SetActive (true);
		}

		StartCoroutine ("DisplayChips");
	}

	public void DisplayTotalChips ()
	{
		txtTotalChips.text = Utility.GetAmount (buyInAmount);

		if (playerID.Equals (NetworkManager.Instance.playerID)) {
			if (UIManager.Instance.isRealMoney)
				WhoopAssGame.Instance.DisplayPlayerTotalChips (totalRealMoney);
			else
				WhoopAssGame.Instance.DisplayPlayerTotalChips (totalChips);

			//  Rebuy button
			if (buyInAmount <= 0) {
				if (playerID.Equals (NetworkManager.Instance.playerID) &&
				    WhoopAssGame.Instance.canRebuy &&
				    UIManager.Instance.isRegularTournament)
					WhoopAssGame.Instance.btnRebuy.gameObject.SetActive (true);
				else
					WhoopAssGame.Instance.btnRebuy.gameObject.SetActive (false);
			} else {
				WhoopAssGame.Instance.btnRebuy.gameObject.SetActive (false);
			}
		}
	}

	public void DisplayUpWhoopAssCard ()
	{
		whoopAssCardUpPosition.gameObject.SetActive (true);
	}

	public void OpenWhoopAssCard ()
	{
		whoopAssCardUpPosition.GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (playerInfo.WACard);
//		whoopAssCardUpPosition.GetComponent<Image>().sprite = Resources.Load<Sprite> (Constants.RESOURCE_GAMECARDS + playerInfo.WACard);
	}

	public void DisplayDownWhoopAssCard ()
	{
		whoopAssCardDownPosition.gameObject.SetActive (true);

		if (playerID.Equals (NetworkManager.Instance.playerID))
			whoopAssCardDownPosition.GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (playerInfo.WACard);
//			whoopAssCardDownPosition.GetComponent<Image> ().sprite = Resources.Load<Sprite> (Constants.RESOURCE_GAMECARDS + playerInfo.WACard);
	}

	private void UpCard (GameObject obj, float upPosition)
	{
		Hashtable ht = new Hashtable ();
		ht.Add ("time", .75f);
		ht.Add ("easetype", iTween.EaseType.spring);
		ht.Add ("position", obj.transform.position + Vector3.up * upPosition);
		iTween.MoveTo (obj, ht);
	}

	public void HighlightWinnerBestCards (List<string> winner)
	{
		SoundManager.Instance.PlayGameCompleteSound (Camera.main.transform.position);

		for (int i = 0; i < winner.Count; i++) {
			if (card1.Equals (winner [i])) {
//				Vector2 pos = card1Position.GetChild (0).position;
//				pos.y = card1Position.position.y + 10f;
//				card1Position.GetChild (0).position = pos;

				if (card1Position.childCount > 0)
					UpCard (card1Position.GetChild (0).gameObject, 10f);
			} else if (card2.Equals (winner [i])) {
//				Vector2 pos = card2Position.GetChild (0).position;
//                pos.y = card2Position.position.y + 10f;
//				card2Position.GetChild (0).position = pos;

				if (card2Position.childCount > 0)
					UpCard (card2Position.GetChild (0).gameObject, 10f);
			}
		}

		for (int i = 0; i < winner.Count; i++) {
			if (playerInfo.WACard.Equals (winner [i])) {
//				Vector2 upCardPos = whoopAssCardUpPosition.position;
//                upCardPos.y = whoopAssUpCardInitialPos.position.y + 10f;
//				whoopAssCardUpPosition.position = upCardPos;

				UpCard (whoopAssCardUpPosition.gameObject, 10f);

//                Vector2 downCardPos = whoopAssCardDownPosition.position;
//                downCardPos.y = whoopAssDownCardInitialPos.position.y + 10f;
//                whoopAssCardDownPosition.position = downCardPos;

				UpCard (whoopAssCardDownPosition.gameObject, 10f);
			}
		}

		for (int i = 0; i < winner.Count; i++) {
			if (winner [i].Equals (WhoopAssGame.Instance.whoopAssGameDefaultCards.FirstFlop1)) {
//				Vector2 pos = WhoopAssGame.Instance.defaultTableCardsList [0].transform.position;
//				pos.y = WhoopAssGame.Instance.defaultTableCardsList[0].transform.parent.position.y + 15f;
//				WhoopAssGame.Instance.defaultTableCardsList [0].transform.position = pos;

				UpCard (WhoopAssGame.Instance.defaultTableCardsList [0].gameObject, 15f);
			} else if (winner [i].Equals (WhoopAssGame.Instance.whoopAssGameDefaultCards.FirstFlop2)) {
//				Vector2 pos = WhoopAssGame.Instance.defaultTableCardsList [1].transform.position;
//				pos.y = WhoopAssGame.Instance.defaultTableCardsList[1].transform.parent.position.y + 15f;
//				WhoopAssGame.Instance.defaultTableCardsList [1].transform.position = pos;

				UpCard (WhoopAssGame.Instance.defaultTableCardsList [1].gameObject, 15f);
			} else if (winner [i].Equals (WhoopAssGame.Instance.whoopAssGameDefaultCards.SecondFlop1)) {
//				Vector2 pos = WhoopAssGame.Instance.defaultTableCardsList [2].transform.position;
//				pos.y = WhoopAssGame.Instance.defaultTableCardsList[2].transform.parent.position.y + 15f;
//				WhoopAssGame.Instance.defaultTableCardsList [2].transform.position = pos;

				UpCard (WhoopAssGame.Instance.defaultTableCardsList [2].gameObject, 15f);
			} else if (winner [i].Equals (WhoopAssGame.Instance.whoopAssGameDefaultCards.SecondFlop2)) {
//				Vector2 pos = WhoopAssGame.Instance.defaultTableCardsList [3].transform.position;
//				pos.y = WhoopAssGame.Instance.defaultTableCardsList[3].transform.parent.position.y + 15f;
//				WhoopAssGame.Instance.defaultTableCardsList [3].transform.position = pos;

				UpCard (WhoopAssGame.Instance.defaultTableCardsList [3].gameObject, 15f);
			} else if (winner [i].Equals (WhoopAssGame.Instance.whoopAssGameDefaultCards.ThirdFlop1)) {
//				Vector2 pos = WhoopAssGame.Instance.defaultTableCardsList [4].transform.position;
//				pos.y = WhoopAssGame.Instance.defaultTableCardsList[4].transform.parent.position.y + 15f;
//				WhoopAssGame.Instance.defaultTableCardsList [4].transform.position = pos;

				UpCard (WhoopAssGame.Instance.defaultTableCardsList [4].gameObject, 15f);
			} else if (winner [i].Equals (WhoopAssGame.Instance.whoopAssGameDefaultCards.ThirdFlop2)) {
//				Vector2 pos = WhoopAssGame.Instance.defaultTableCardsList [5].transform.position;
//				pos.y = WhoopAssGame.Instance.defaultTableCardsList[5].transform.parent.position.y + 15f;
//				WhoopAssGame.Instance.defaultTableCardsList [5].transform.position = pos;

				UpCard (WhoopAssGame.Instance.defaultTableCardsList [5].gameObject, 15f);
			}
		}

		//		BlinkPlayer ();
	}

	public void ResetCardsToInitialPosition ()
	{
		if (card1Position.childCount > 0) {
			iTween.Stop (card1Position.GetChild (0).gameObject);
			card1Position.GetChild (0).transform.position = card1Position.position;
		}
		if (card2Position.childCount > 0) {
			iTween.Stop (card2Position.GetChild (0).gameObject);
			card2Position.GetChild (0).transform.position = card2Position.position;
		}

		iTween.Stop (whoopAssCardUpPosition.gameObject);
		iTween.Stop (whoopAssCardDownPosition.gameObject);

		whoopAssCardUpPosition.position = whoopAssUpCardInitialPos.position;
		whoopAssCardDownPosition.position = whoopAssDownCardInitialPos.position;
	}

	public void SetPlayerName ()
	{
		string playerName = playerID;

		if (playerName.Length > 10)
			playerName = playerName.Substring (0, 10) + "..";

		txtPlayerName.text = playerName;
		txtPlayerName.color = Color.yellow;
	}

	public void DisplayPlayerData ()
	{
		SetPlayerName ();
		txtTotalChips.text = Utility.GetAmount (buyInAmount);
		txtTotalChips.color = Color.yellow;
	}

	public void DestroyCards ()
	{
		foreach (Transform t in card1Position) {
			Destroy (t.gameObject);
		}
		foreach (Transform t in card2Position) {
			Destroy (t.gameObject);
		}
	}

	public void DestroyCard1 ()
	{
		foreach (Transform t in card1Position) {
			Destroy (t.gameObject);
		}
	}

	public void DestroyCard2 ()
	{
		foreach (Transform t in card2Position) {
			Destroy (t.gameObject);
		}
	}

	public void OnProfileInfoButtonTap ()
	{
		List<string> availableMoneyType = UIManager.Instance.lobbyPanel.availableMoneyType;
		MoneyType moneyType = MoneyType.All;
		if (availableMoneyType.Contains ("Real Money") && availableMoneyType.Contains ("Play Money"))
			moneyType = MoneyType.All;
		else if (availableMoneyType.Contains ("Real Money"))
			moneyType = MoneyType.RealMoney;
		else if (availableMoneyType.Contains ("Play Money"))
			moneyType = MoneyType.PlayMoney;

		string playMoney = Utility.GetCommaSeperatedPlayMoneyAmount (totalChips);
		string realMoney = Utility.GetCommaSeperatedAmount (totalRealMoney, true);

		UIManager.Instance.playerDetailPanel.SetPlayerDetails (imgProfile.transform.position, imgProfile.sprite, playerInfo.Player_Name, playMoney, realMoney, moneyType);
	}

	#endregion

	#region PRIVATE_METHODS

	private PLAYER_ACTION GetPlayerAction (int action)
	{
		switch (action) {
		case (int)PLAYER_ACTION.CHECK:
			return PLAYER_ACTION.CHECK;
		case (int)PLAYER_ACTION.BET:
			return PLAYER_ACTION.BET;
		case (int)PLAYER_ACTION.FOLD:
			return PLAYER_ACTION.FOLD;
		case (int)PLAYER_ACTION.TIMEOUT:
			return PLAYER_ACTION.TIMEOUT;
		case (int)PLAYER_ACTION.ACTION_WA_DOWN:
			return PLAYER_ACTION.ACTION_WA_DOWN;
		case (int)PLAYER_ACTION.ACTION_WA_UP:
			return PLAYER_ACTION.ACTION_WA_UP;
		case (int)PLAYER_ACTION.ACTION_WA_NO:
			return PLAYER_ACTION.ACTION_WA_NO;
		case (int)PLAYER_ACTION.ALLIN:
			return PLAYER_ACTION.ALLIN;
		case (int)PLAYER_ACTION.CALL:
			return PLAYER_ACTION.CALL;
		case (int)PLAYER_ACTION.RAISE:
			return PLAYER_ACTION.RAISE;
		case (int)PLAYER_ACTION.BLIND_ON_BACK_TO_GAME:
			return PLAYER_ACTION.BLIND_ON_BACK_TO_GAME;
		}
		return PLAYER_ACTION.CHECK;
	}

	private string GetPlayerActionInString (int action)
	{
		switch (action) {
		case (int)PLAYER_ACTION.CHECK:
			return "CHECK";
		case (int)PLAYER_ACTION.BET:
			return "BET";
		case (int)PLAYER_ACTION.FOLD:
			return "FOLD";
		case (int)PLAYER_ACTION.TIMEOUT:
			return "TIMEOUT";
		case (int)PLAYER_ACTION.ACTION_WA_DOWN:
			return "DOWN";
		case (int)PLAYER_ACTION.ACTION_WA_UP:
			return "UP";
		case (int)PLAYER_ACTION.ACTION_WA_NO:
			return "NO";
		case (int)PLAYER_ACTION.ALLIN:
			return "ALLIN";
		case (int)PLAYER_ACTION.CALL:
			return "CALL";
		case (int)PLAYER_ACTION.RAISE:
			return "RAISE";
		}
		return "CHECK";
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

	private int CalculateTotalChipsToGenerate (int chipAmount)
	{
		int chip = (chipAmount * 10) / 1000;
		if (chip < Constants.MINIMUM_CHIP_DISPLAY)
			chip = Constants.MINIMUM_CHIP_DISPLAY;
		else if (chip > Constants.MAXIMUM_CHIPS_DISPLAY)
			chip = Constants.MAXIMUM_CHIPS_DISPLAY;
		return chip;
	}

	private float GetYPos (int i)
	{
		return txtBetAmountPosition.transform.position.y + (i * 2f);
	}

	private float GetRedChipYPos (int i)
	{
		return redInitialChip.position.y + (i * 3f);
	}

	private float GetGreenChipYPos (int i)
	{
		return greenInitialChip.position.y + (i * 3f);
	}

	private float GetBlueChipYPos (int i)
	{
		return blueInitialChip.position.y + (i * 3f);
	}

	#endregion

	#region COROUTINES

	private IEnumerator SetPlayerData ()
	{
		yield return new WaitForEndOfFrame ();

		isOwnPlayer = playerID.Equals (NetworkManager.Instance.playerID);

		if (isOwnPlayer)
			GetComponent<Image> ().sprite = ownPlayerSprite;
		else
			GetComponent<Image> ().sprite = otherPlayerSprite;

		DisplayTotalChips ();
		DestroyAllChips ();

		if (playerInfo.Player_Status != (int)PLAYER_STATUS.ACTIVE &&
		    playerInfo.Player_Status != (int)PLAYER_STATUS.ABSENT &&
		    playerInfo.Player_Status != (int)PLAYER_STATUS.SIT_OUT)
			GetComponent<CanvasGroup> ().alpha = .4f;
		else
			GetComponent<CanvasGroup> ().alpha = 1f;
		
		txtPlayerName.GetComponent<CanvasGroup> ().ignoreParentGroups = true;

		whoopAssCardUpPosition.position = whoopAssUpCardInitialPos.position;
		whoopAssCardDownPosition.position = whoopAssDownCardInitialPos.position;

		DisplayPlayerData ();
		HideTurnTimer ();

		if (playerInfo.Player_Status == (int)PLAYER_STATUS.ELIMINATED) {
			txtPlayerName.text = "<color=red>ELIMINATED</color>";
			DestroyCards ();
		}

		StartCoroutine (GetProfilePic ());
	}

	private IEnumerator GetProfilePic ()
	{
		yield return new WaitForEndOfFrame ();
		playerInfo.Profile_Pic = playerInfo.Profile_Pic.Replace (".info", ".com");
		DebugLog.Log ("Profile Pic : " + playerInfo.Profile_Pic);
		WWW www = new WWW (Constants.RESIZE_IMAGE_URL + playerInfo.Profile_Pic);
		yield return www;

		if (www.error != null) {
			DebugLog.LogError ("Error while downloading profile pic  : " + www.error + "\nURL  : " + www.url);
		} else {
			if (www.texture != null) {
				isProfilePicLoaded = true;
				imgProfile.sprite = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), Vector2.zero);
			}
		}
	}

	private IEnumerator TurnTimer ()
	{
		StartCoroutine ("BlinkOutline");
		if (playerID.Equals (NetworkManager.Instance.playerID))
			StartCoroutine ("PlayTickSound");

		float i = 0;
		while (i < 1) {
			i += 1f / playerInfo.Turn_Time * Time.deltaTime;

			imgTurnDisplayer.fillAmount = Mathf.Lerp (0, 1, i);

			yield return 0;
		}

		imgTurnDisplayer.fillAmount = 0;
	}

	private IEnumerator BlinkOutline ()
	{
		while (true) {
			playerTurnBorder.enabled = true;
			yield return new WaitForSeconds (.5f);

			playerTurnBorder.enabled = false;
			yield return new WaitForSeconds (.5f);
		}
	}

	private IEnumerator PlayTickSound ()
	{
		while (true) {
			yield return new WaitForSeconds (1f);

			SoundManager.Instance.PlayTickSound (Camera.main.transform.position);
		}
	}

	private IEnumerator DisplayChips ()
	{
		yield return new WaitForEndOfFrame ();
		//		totalChipObject = 0;
		int chipsAmount = (int)(betAmount - betAmountInPot);

		if (chipsAmount > 0) {
			SoundManager.Instance.PlayChipsSound (Camera.main.transform.position);

			int totalChipObjectToGenerate = CalculateTotalChipsToGenerate (chipsAmount);

			if (totalChipObjectToGenerate > chipsDisplayedList.Count) {
				totalChipObjectToGenerate -= chipsDisplayedList.Count;

				for (int i = 0; i < totalChipObjectToGenerate; i++) {
					GameObject chip = Instantiate (chipPrefab, new Vector2 (txtBetAmountPosition.transform.position.x, GetYPos (++totalChipObject)), Quaternion.identity) as GameObject;
					chipsDisplayedList.Add (chip);
					chip.transform.SetParent (transform);
					chip.transform.localScale = Vector3.one;

					yield return new WaitForEndOfFrame ();

//					GameObject redChip = Instantiate(redChipPrefab, new Vector2(redInitialChip.transform.position.x, GetRedChipYPos(++totalRedChipObject)), Quaternion.identity) as GameObject;
//					GameObject greenChip = Instantiate(greenChipPrefab, new Vector2(greenInitialChip.transform.position.x, GetGreenChipYPos(++totalGreenChipObject)), Quaternion.identity) as GameObject;
//					GameObject blueChip = Instantiate(blueChipPrefab, new Vector2(blueInitialChip.transform.position.x, GetBlueChipYPos(++totalBlueChipObject)), Quaternion.identity) as GameObject;
//
//					redChipsDisplayedList.Add(redChip);
//					greenChipsDisplayedList.Add(greenChip);
//					blueChipsDisplayedList.Add(blueChip);
//
//					//-------------------------------
//					chipsDisplayedList.Add (redChip);
//					chipsDisplayedList.Add (greenChip);
//					chipsDisplayedList.Add (blueChip);
//					totalChipObject++;
//					//-------------------------------
//
//					redChip.transform.SetParent(redInitialChip);
//					greenChip.transform.SetParent(greenInitialChip);
//					blueChip.transform.SetParent(blueInitialChip);
//
//					redChip.transform.localScale = Vector3.one;
//					greenChip.transform.localScale = Vector3.one;
//					blueChip.transform.localScale = Vector3.one;
				}
			}
		}
	}

	private IEnumerator MoveChipsToPot ()
	{
		StopCoroutine ("DisplayChips");

		GameObject go = new GameObject ("ChipsParent");
		go.transform.position = txtBetAmountPosition.position;
		go.transform.SetParent (transform);

		if (chipsDisplayedList.Count > 0)
			SoundManager.Instance.PlayChipsSound (Camera.main.transform.position);

		totalChipObject = 0;
		totalRedChipObject = totalGreenChipObject = totalBlueChipObject = 0;
		foreach (GameObject g in chipsDisplayedList) {
			g.transform.SetParent (go.transform);
		}
		chipsDisplayedList = new List<GameObject> ();
		redChipsDisplayedList = new List<GameObject> ();
		greenChipsDisplayedList = new List<GameObject> ();
		blueChipsDisplayedList = new List<GameObject> ();

		Vector3 fromPos = go.transform.position;
		Vector3 toPos = WhoopAssGame.Instance.initialChipPosition.transform.position;

		float i = 0;
		while (i < 1) {
			i += 3 * Time.deltaTime;
			go.transform.position = Vector3.Lerp (fromPos, toPos, i);
			yield return 0;
		}

		Destroy (go, .15f);
	}

	#endregion
}