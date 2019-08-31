using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using com.shephertz.app42.gaming.multiplayer.client.events;

public class TexassPlayer : MonoBehaviour
{
	public string player_stuas;
	public string playerID;
	public int seatIndex;

	public double buyInAmount;
	public double totalChips;
	public double totalRealMoney;
	public double betAmount;
	public double betAmountInPot;

	public Transform txtBetAmountPosition;

	public string card1;
	public string card2;

	public PlayerInfo playerInfo;

	public bool isDealer;
	public bool isSmallBlind;
	public bool isBigBlind;

	public double blindAmount;

	public Image imgProfile;

	public Image imgTurnDisplayer;

	public Image imgBetAmountBG;

	public Text txtPlayerName;
	public Text txtTotalChips;

	public GameObject winnerRankParentObject;
	public Text txtWinningRank;

	public Transform card1Position;
	public Transform card2Position;

	public Text txtBetAmount;
	public GameObject dealerObject;
	public Image imgAbsentPlayer;
	public Sprite spAbsent;
	public Sprite spSitout;

	public GameObject chipPrefab;
	private int totalChipObject;

	public List<Transform> betChipsPositionList;
	public List<Transform> betAmountPositionsList;
	public GameObject betAmountParentObj;

	public List<GameObject> chipsDisplayedList;

	public Outline playerTurnBorder;
	public Animator playerAnimator;

	public bool isEliminated;

	private bool isProfilePicLoaded = false;

	// Use this for initialization
	void OnEnable ()
	{
		HandleOnResetData ();
		totalChipObject = 0;
		chipsDisplayedList = new List<GameObject> ();
		StartCoroutine (SetPlayerDetail ());

		NetworkManager.onActionResponseReceived += HandleOnActionResponseReceived;
		TexassGame.resetData += HandleOnResetData;
		NetworkManager.onMoveCompletedByPlayer += HandleOnMoveCompletedByPlayer;
		NetworkManager.onRoundComplete += HandleOnRoundComplete;
		NetworkManager.onWinnerInfoReceived += HandleOnWinnerInfoReceived;
		NetworkManager.onActionHistoryReceived += HandleOnActionHistoryReceived;
		NetworkManager.onBlindPlayerResponseReceived += HandleOnBlindPlayerResponseReceived;
		NetworkManager.onRebuyActionResponseReceived += HandleOnRebuyActionResponseReceived;
		RoundController.collectBlindAmount += HandleOnCollectBlindAmount;
		NetworkManager.onWinnerInfoReceived += HandleOnRoundComplete;
		NetworkManager.onGameStartedByPlayer += HandleOnGameStartedByPlayer;
		NetworkManager.playerRequestedSitout += HandlePlayerReqestedSitout;
		NetworkManager.playerRequestedBackToGame += HandlePlayerReqestedBackToGame;
		RoundController.cardDistributionFinished += HandleCardDistributionFinished;
	}

	void OnDisable ()
	{
		NetworkManager.onActionResponseReceived -= HandleOnActionResponseReceived;
		TexassGame.resetData -= HandleOnResetData;
		NetworkManager.onMoveCompletedByPlayer -= HandleOnMoveCompletedByPlayer;
		NetworkManager.onRoundComplete -= HandleOnRoundComplete;
		NetworkManager.onWinnerInfoReceived -= HandleOnWinnerInfoReceived;
		NetworkManager.onActionHistoryReceived -= HandleOnActionHistoryReceived;
		NetworkManager.onBlindPlayerResponseReceived -= HandleOnBlindPlayerResponseReceived;
		NetworkManager.onRebuyActionResponseReceived -= HandleOnRebuyActionResponseReceived;
		RoundController.collectBlindAmount -= HandleOnCollectBlindAmount;
		NetworkManager.onWinnerInfoReceived -= HandleOnRoundComplete;
		NetworkManager.onGameStartedByPlayer -= HandleOnGameStartedByPlayer;
		NetworkManager.playerRequestedSitout -= HandlePlayerReqestedSitout;
		NetworkManager.playerRequestedBackToGame -= HandlePlayerReqestedBackToGame;
		RoundController.cardDistributionFinished -= HandleCardDistributionFinished;

		DestroyCards ();
	}

	public void OnUpButtonTap ()
	{
		DebugLog.Log ("Up button tap");
	}

	public void OnDownButtonTap ()
	{
		DebugLog.Log ("Down button tap");
	}

	private IEnumerator SetPlayerDetail ()
	{
		winnerRankParentObject.SetActive (false);
		betAmount = betAmountInPot = 0;
		DestroyAllChips ();

		yield return new WaitForSeconds (.1f);

		DisplayPlayerData ();
		DisplayTotalChips ();

		if (playerInfo.Player_Status != (int)PLAYER_STATUS.ACTIVE &&
		    playerInfo.Player_Status != (int)PLAYER_STATUS.ABSENT &&
		    playerInfo.Player_Status != (int)PLAYER_STATUS.SIT_OUT)
			GetComponent<CanvasGroup> ().alpha = .4f;
		else
			GetComponent<CanvasGroup> ().alpha = 1f;
		
		txtPlayerName.GetComponent<CanvasGroup> ().ignoreParentGroups = true;

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
		WWW www = new WWW (Constants.RESIZE_IMAGE_URL + playerInfo.Profile_Pic);
		yield return www;

		if (www.error != null) {
			Debug.LogError ("Error while downloading profile pic  : " + www.error + "\nURL  : " + www.url);
		} else {
			if (www.texture != null) {
				isProfilePicLoaded = true;
				imgProfile.sprite = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), Vector2.zero);
			}
		}
	}

	public void DisplayBetAmount ()
	{
		double amt = (betAmount - betAmountInPot);
		if (amt == 0) {
			txtBetAmount.text = "";
			imgBetAmountBG.gameObject.SetActive (false);
		} else {
			txtBetAmount.text = Utility.GetAmount (amt);
			imgBetAmountBG.gameObject.SetActive (true);
		}

		StartCoroutine ("DisplayChips");
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

	public void DisplayTotalChips ()
	{
		if (playerID.Equals (NetworkManager.Instance.playerID)) {
			if (UIManager.Instance.isRealMoney)
				TexassGame.Instance.DisplayPlayerTotalChips (totalRealMoney);
			else
				TexassGame.Instance.DisplayPlayerTotalChips (totalChips);
		}

		//  Rebuy button
		if (buyInAmount <= 0) {
			if (playerID.Equals (NetworkManager.Instance.playerID) &&
			    TexassGame.Instance.canRebuy &&
			    UIManager.Instance.isRegularTournament && TexassGame.Instance.isGameCompleted) {
				TexassGame.Instance.btnRebuy.gameObject.SetActive (true);
				TexassGame.Instance.btnSitoutNextHand.gameObject.SetActive (false);
			} else
				TexassGame.Instance.btnRebuy.gameObject.SetActive (false);
		} else {
			TexassGame.Instance.btnRebuy.gameObject.SetActive (false);
		}
	}

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

			if (ar.Action == (int)PLAYER_ACTION.CHECK)
				SoundManager.Instance.PlayCheckActionSound (Camera.main.transform.position);
			else if (ar.Action == (int)PLAYER_ACTION.BET || ar.Action == (int)PLAYER_ACTION.CALL)
				SoundManager.Instance.PlayBetCallSound (Camera.main.transform.position);
                
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
				TexassGame.Instance.raisePerRoundCounter++;

				SoundManager.Instance.PlayBetCallSound (Camera.main.transform.position);
			}

			txtPlayerName.text = GetPlayerAction (ar.Action).ToString ();
			txtPlayerName.color = ar.Action == (int)PLAYER_ACTION.FOLD ? Color.red : Color.white;
			txtTotalChips.text = Utility.GetAmount (betAmount);
			txtTotalChips.color = Color.white;
			DisplayBetAmount ();

			//			playerInfo.Player_Status = ar.Action;

			HistoryManager.GetInstance ().AddHistory (playerID, "name", RoundController.GetInstance ().currentTexassGameRound, ar.Bet_Amount, betAmount, GetPlayerAction (ar.Action));
		} else {
			if (playerInfo.Player_Status == (int)PLAYER_STATUS.ACTIVE)
				DisplayPlayerData ();
		}
	}

	public void HandleOnResetData ()
	{
//        if (playerInfo.Player_Status == (int)PLAYER_STATUS.ACTIVE ||
//            playerInfo.Player_Status == (int)PLAYER_STATUS.ABSENT ||
//            playerInfo.Player_Status == (int)PLAYER_STATUS.SIT_OUT)
		GetComponent<CanvasGroup> ().alpha = 1f;

		betAmount = betAmountInPot = 0;
		DestroyCards ();
		DisplayBetAmount ();
		winnerRankParentObject.SetActive (false);
		txtTotalChips.text = Utility.GetAmount (buyInAmount);

		DestroyAllChips ();

		if (isEliminated)
			txtPlayerName.text = "<color=red>ELIMINATED</color>";
	}

	private void HandleOnMoveCompletedByPlayer (MoveEvent moveEvent)
	{
		HideTurnTimer ();

		if (playerID.Equals (moveEvent.getNextTurn ())) {
			if (playerInfo.Player_Status != (int)PLAYER_ACTION.ACTION_WAITING_FOR_GAME &&
			    playerInfo.Player_Status != (int)PLAYER_ACTION.FOLD &&
			    playerInfo.Player_Status != (int)PLAYER_ACTION.TIMEOUT &&
			    playerInfo.Player_Status != (int)PLAYER_ACTION.ALLIN) {

				DisplayPlayerData ();
				DisplayTurnTimer ();
			}
		}
	}

	private void HandleOnRoundComplete (string sender, string roundInfo)
	{
		//if (playerInfo.Player_Status == (int)PLAYER_STATUS.ACTIVE)
		//{
		//    card1Position.GetChild(0).GetComponent<CardFlipAnimation>().DisplayCardWithoutAnimation(card1);
		//    card2Position.GetChild(0).GetComponent<CardFlipAnimation>().DisplayCardWithoutAnimation(card2);
		//}

		betAmountInPot = betAmount;
		blindAmount = 0;

		//GameRound round = JsonUtility.FromJson<GameRound> (roundInfo);

		DisplayBetAmount ();

		StartCoroutine (MoveChipsToPot ());
	}

	private void HandleOnGameStartedByPlayer (string sender, string gameStarter)
	{
		if (sender.Equals (Constants.TEXASS_SERVER_NAME)) {
			if (playerID.Equals (gameStarter)) {
				DisplayPlayerData ();
				DisplayTurnTimer ();
			} else
				HideTurnTimer ();
		}
	}

	private void HandleOnWinnerInfoReceived (string sender, string winnerInfo)
	{
		DisplayPlayerData ();

		betAmountInPot = betAmount;
		DisplayBetAmount ();

		HideTurnTimer ();
		DisplayPlayerData ();

		txtBetAmount.text = "";
		imgBetAmountBG.gameObject.SetActive (false);

		if (TexassGame.Instance.GetActivePlayers () < Constants.MIN_PLAYER_TO_START_GAME ||
		    (playerInfo.Player_Status != (int)PLAYER_STATUS.ACTIVE &&
		    playerInfo.Player_Status != (int)PLAYER_ACTION.ALLIN &&
		    playerInfo.Player_Status != (int)PLAYER_STATUS.ABSENT) ||
		    playerID.Equals (TexassGame.Instance.ownTexassPlayer.playerID))
			return;

		card1Position.GetChild (0).GetComponent<CardFlipAnimation> ().PlayAnimation (card1);
		card2Position.GetChild (0).GetComponent<CardFlipAnimation> ().PlayAnimation (card2);
	}

	private void UpCard (GameObject obj, float upPosition)
	{
		Hashtable ht = new Hashtable ();
		ht.Add ("time", .75f);
		ht.Add ("easetype", iTween.EaseType.spring);
		ht.Add ("position", obj.transform.position + Vector3.up * upPosition);
		iTween.MoveTo (obj, ht);
	}

	public void HighlightWinnerBestCards (GameWinner winner)
	{
		SoundManager.Instance.PlayGameCompleteSound (Camera.main.transform.position);

		for (int i = 0; i < winner.Winner_Best_Cards.Count; i++) {
			if (card1.Equals (winner.Winner_Best_Cards [i])) {
//                Vector2 pos = card1Position.GetChild (0).position;
//				pos.y += 10f;
//				card1Position.GetChild (0).position = pos;

				if (card1Position.childCount > 0)
					UpCard (card1Position.GetChild (0).gameObject, 10f);
			} else if (card2.Equals (winner.Winner_Best_Cards [i])) {
//                Vector2 pos = card2Position.GetChild (0).position;
//				pos.y += 10f;
//				card2Position.GetChild (0).position = pos;

				if (card2Position.childCount > 0)
					UpCard (card2Position.GetChild (0).gameObject, 10f);
			}
		}

		for (int i = 0; i < winner.Winner_Best_Cards.Count; i++) {
			if (TexassGame.Instance.defaultCardsPositionList [i].childCount == 0)
				continue;

			if (winner.Winner_Best_Cards [i].Equals (TexassGame.Instance.texassDefaultCards.Flop1)) {
//				Vector2 pos = TexassGame.Instance.defaultCardsPositionList [0].GetChild (0).transform.position;
//				pos.y += 15f;
//				TexassGame.Instance.defaultCardsPositionList [0].GetChild (0).transform.position = pos;

				if (TexassGame.Instance.defaultCardsPositionList [0].childCount > 0)
					UpCard (TexassGame.Instance.defaultCardsPositionList [0].GetChild (0).gameObject, 15f);
			} else if (winner.Winner_Best_Cards [i].Equals (TexassGame.Instance.texassDefaultCards.Flop2)) {
//				Vector2 pos = TexassGame.Instance.defaultCardsPositionList [1].GetChild (0).transform.position;
//				pos.y += 15f;
//				TexassGame.Instance.defaultCardsPositionList [1].GetChild (0).transform.position = pos;

				if (TexassGame.Instance.defaultCardsPositionList [1].childCount > 0)
					UpCard (TexassGame.Instance.defaultCardsPositionList [1].GetChild (0).gameObject, 15f);
			} else if (winner.Winner_Best_Cards [i].Equals (TexassGame.Instance.texassDefaultCards.Flop3)) {
//				Vector2 pos = TexassGame.Instance.defaultCardsPositionList [2].GetChild (0).transform.position;
//				pos.y += 15f;
//				TexassGame.Instance.defaultCardsPositionList [2].GetChild (0).transform.position = pos;

				if (TexassGame.Instance.defaultCardsPositionList [2].childCount > 0)
					UpCard (TexassGame.Instance.defaultCardsPositionList [2].GetChild (0).gameObject, 15f);
			} else if (winner.Winner_Best_Cards [i].Equals (TexassGame.Instance.texassDefaultCards.Turn)) {
//				Vector2 pos = TexassGame.Instance.defaultCardsPositionList [3].GetChild (0).transform.position;
//				pos.y += 15f;
//				TexassGame.Instance.defaultCardsPositionList [3].GetChild (0).transform.position = pos;

				if (TexassGame.Instance.defaultCardsPositionList [3].childCount > 0)
					UpCard (TexassGame.Instance.defaultCardsPositionList [3].GetChild (0).gameObject, 15f);
			} else if (winner.Winner_Best_Cards [i].Equals (TexassGame.Instance.texassDefaultCards.River)) {
//				Vector2 pos = TexassGame.Instance.defaultCardsPositionList [4].GetChild (0).transform.position;
//				pos.y += 15f;
//				TexassGame.Instance.defaultCardsPositionList [4].GetChild (0).transform.position = pos;

				if (TexassGame.Instance.defaultCardsPositionList [4].childCount > 0)
					UpCard (TexassGame.Instance.defaultCardsPositionList [4].GetChild (0).gameObject, 15f);
			}
		}

//		BlinkPlayer ();
	}

	public void ResetCardsToInitialPosition ()
	{
		if (card1Position.childCount > 0) {
			iTween.Stop (card1Position.GetChild (0).gameObject);
			card1Position.GetChild (0).position = card1Position.position;
		}
		if (card2Position.childCount > 0) {
			iTween.Stop (card2Position.GetChild (0).gameObject);
			card2Position.GetChild (0).position = card2Position.position;
		}
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

	public void BlinkPlayer ()
	{
		StartCoroutine (BlinkWinnerPlayer ());
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

	private IEnumerator BlinkWinnerPlayer ()
	{
		for (int i = 0; i < 5; i++) {
			yield return new WaitForSeconds (.25f);
			GetComponent<CanvasGroup> ().alpha = 0;
			yield return new WaitForSeconds (.25f);
			GetComponent<CanvasGroup> ().alpha = 1f;
		}
	}

	private void HandleOnActionHistoryReceived (string sender, string actionHistory)
	{
		Debug.Log (Constants.HANDLE_MULTI_GAME_HERE + " --> " + sender);

		Debug.LogWarning (actionHistory);
		JSON_Object obj = new JSON_Object (actionHistory);

		//int roundStatus = obj.getInt (Constants.FIELD_ACTION_HISTORY_ROUND_STATUS);
		//int round = obj.getInt (Constants.FIELD_ACTION_HISTORY_ROUND);

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

	private void HandleOnBlindPlayerResponseReceived (string sender, string blindPlayerInfo)
	{
		BlindPlayer blind = JsonUtility.FromJson<BlindPlayer> (blindPlayerInfo);


		if (TexassGame.Instance.currentGameStatus == GAME_STATUS.RUNNING)
			return;
		
		isBigBlind = isSmallBlind = isDealer = false;

		blindAmount = betAmount = 0;

		dealerObject.SetActive (false);

		if (blind.Big_Blind.Equals (this.playerID)) {
			isBigBlind = true;

			blindAmount = betAmount = blind.SBAmount * 2;

			if (blindAmount > buyInAmount) {
				blindAmount = betAmount = buyInAmount;
				if (playerInfo.Player_Status != (int)PLAYER_STATUS.SIT_OUT)
					playerInfo.Player_Status = (int)PLAYER_ACTION.ALLIN;
			}

//			HistoryManager.GetInstance ().AddHistory (this.playerID, txtPlayerName.text, TEXASS_GAME_ROUND.PREFLOP, blindAmount, betAmount, PLAYER_ACTION.BIG_BLIND);
		}
		if (blind.Player_Dealer.Equals (this.playerID)) {
			isDealer = true;
			dealerObject.SetActive (true);
		}
		if (blind.Small_Blind.Equals (this.playerID)) {
			isSmallBlind = true;

			blindAmount = betAmount = blind.SBAmount;

			if (blindAmount > buyInAmount) {
				blindAmount = betAmount = buyInAmount;
				if (playerInfo.Player_Status != (int)PLAYER_STATUS.SIT_OUT)
					playerInfo.Player_Status = (int)PLAYER_ACTION.ALLIN;
			}


//			HistoryManager.GetInstance ().AddHistory (this.playerID, txtPlayerName.text, TEXASS_GAME_ROUND.PREFLOP, blindAmount, betAmount, PLAYER_ACTION.SMALL_BLIND);
		}

//		txtBetAmount.text = Utility.GetCurrencyPrefix() + betAmount;
//		DisplayPlayerData ();
//		buyInAmount -= betAmount;
	}

	private void HandleOnRebuyActionResponseReceived (string sender, string rebuyInfo)
	{
		if (sender.Equals (Constants.TEXASS_SERVER_NAME)) {
			RebuyAction rebuy = JsonUtility.FromJson<RebuyAction> (rebuyInfo);

			if (rebuy.Player_Name.Equals (playerID)) {
				buyInAmount = rebuy.Player_BuyIn_Chips;
				totalChips = rebuy.Player_Total_Play_Chips;
				totalRealMoney = rebuy.Player_Total_Real_Chips;
				DisplayTotalChips ();

				if (rebuy.Player_Name.Equals (NetworkManager.Instance.playerID)) {
					TexassGame.Instance.btnRebuy.gameObject.SetActive (false);
					TexassGame.Instance.btnAddChips.gameObject.SetActive (false);
					TexassGame.Instance.rebuyPanel.gameObject.SetActive (false);
				}
			}
		}
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
			HistoryManager.GetInstance ().AddHistory (this.playerID, txtPlayerName.text, TEXASS_GAME_ROUND.PREFLOP, blindAmount, betAmount, PLAYER_ACTION.SMALL_BLIND);
		else if (isBigBlind)
			HistoryManager.GetInstance ().AddHistory (this.playerID, txtPlayerName.text, TEXASS_GAME_ROUND.PREFLOP, blindAmount, betAmount, PLAYER_ACTION.BIG_BLIND);
		
		blindAmount = 0;
	}

	private void HandlePlayerReqestedSitout (string sender)
	{
		if (playerID.Equals (sender)) {
			playerInfo.Player_Status = (int)PLAYER_STATUS.SIT_OUT;
			imgAbsentPlayer.sprite = spSitout;
			imgAbsentPlayer.gameObject.SetActive (true);
			imgAbsentPlayer.color = Color.red;

			if (playerID.Equals (NetworkManager.Instance.playerID)) {
				TexassGame.Instance.btnBackToGame.gameObject.SetActive (true);
				TexassGame.Instance.recentlyBackToGame = false;
			}
		}
	}

	private void HandlePlayerReqestedBackToGame (string sender)
	{
		if (playerID.Equals (sender)) {
			imgAbsentPlayer.gameObject.SetActive (false);

			if (playerID.Equals (NetworkManager.Instance.playerID))
				TexassGame.Instance.recentlyBackToGame = true;
		}
	}

	private void HandleCardDistributionFinished ()
	{
		if (!isProfilePicLoaded)
			StartCoroutine (GetProfilePic ());
	}

	private PLAYER_ACTION GetPlayerAction (int action)
	{
		switch (action) {
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
		case (int)PLAYER_ACTION.RAISE:
			return PLAYER_ACTION.RAISE;
		case (int) PLAYER_ACTION.CALL:
			return PLAYER_ACTION.CALL;
		}
		return PLAYER_ACTION.CHECK;
	}

	#endregion

	public void DisplayTurnTimer ()
	{
		if (!TexassGame.Instance.isGameCompleted) {
			StartCoroutine ("TurnTimer");
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

	private IEnumerator PlayTickSound ()
	{
		while (true) {
			yield return new WaitForSeconds (1f);

			SoundManager.Instance.PlayTickSound (Camera.main.transform.position);
		}
	}

	public void DisplayPlayerData ()
	{
		txtPlayerName.text = playerInfo.Player_Name;
		txtPlayerName.color = Color.yellow;
		txtTotalChips.text = Utility.GetAmount (buyInAmount);
		txtTotalChips.color = Color.yellow;
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

	private IEnumerator DisplayChips ()
	{
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

					yield return 0;
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
		foreach (GameObject g in chipsDisplayedList) {
			g.transform.SetParent (go.transform);
		}
		chipsDisplayedList = new List<GameObject> ();

		Vector3 fromPos = go.transform.position;
		Vector3 toPos = TexassGame.Instance.tableChipObject.transform.position;

		float i = 0;
		while (i < 1) {
			i += 3 * Time.deltaTime;
			go.transform.position = Vector3.Lerp (fromPos, toPos, i);
			yield return 0;
		}

		Destroy (go, .15f);
	}

	private void DestroyAllChips ()
	{
		foreach (Chip c in GetComponentsInChildren<Chip>()) {
			Destroy (c.gameObject);
		}
		totalChipObject = 0;
		chipsDisplayedList = new List<GameObject> ();
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
}