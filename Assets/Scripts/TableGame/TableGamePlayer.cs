using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using com.shephertz.app42.gaming.multiplayer.client.events;

public class TableGamePlayer : MonoBehaviour
{
	public string playerID;

	public int seatIndex;

	public double buyinChips;
	public double totalChips;
	public double totalRealMoney;
	public double betAmount;

	public string card1;
	public string card2;

	public PlayerInfo playerInfo;

	public Image imgProfile;

	public Image imgTurnDisplayer;

	public Text txtPlayerName;
	public Text txtTotalChips;
	public Text txtBetAmount;

	public Transform whoopAssCardUpPosition;
	public Transform whoopAssCardDownPosition;
	
	public Transform card1Position;
	public Transform card2Position;

	public GameObject chipPrefab;

	public Transform anteImagePosition;
	public Transform straightOrBetterImagePosition;
	public Transform blindImagePosition;
	public Transform bet1ImagePosition;
	public Transform bet2ImagePosition;
	public Transform bet3ImagePosition;
	public Transform playImagePosition;
	public Button btnUp;
	public Button btnDown;

	public Image imgSitout;

	public List<GameObject> chipDisplayedList;

	public GameObject playerFoldParent;

	// Use this for initialization
	void OnEnable ()
	{
		StartCoroutine (SetPlayerDetail ());

		NetworkManager.onActionResponseReceived += HandleOnActionResponseReceived;
		GameManager.resetData += HandleOnResetData;
		NetworkManager.onMoveCompletedByPlayer += HandleOnMoveCompletedByPlayer;
		NetworkManager.onRoundComplete += HandleOnRoundComplete;
		NetworkManager.onWinnerInfoReceived += HandleOnWinnerInfoReceived;
		NetworkManager.onActionHistoryReceived += HandleOnActionHistoryReceived;
		NetworkManager.onRebuyActionResponseReceived += HandleOnRebuyActionResponseReceived;
		NetworkManager.playerRequestedSitout += HandlePlayerReqestedSitout;
		NetworkManager.playerRequestedBackToGame += HandlePlayerReqestedBackToGame;
		NetworkManager.anteAndBlindRequestReceived += HandleAnteAndBlindRequestReceived;
		NetworkManager.addChipsResponseReceived += HandleOnAddChipsResponseReceived;
	}

	void OnDisable ()
	{
		NetworkManager.onActionResponseReceived -= HandleOnActionResponseReceived;
		GameManager.resetData -= HandleOnResetData;
		NetworkManager.onMoveCompletedByPlayer -= HandleOnMoveCompletedByPlayer;
		NetworkManager.onRoundComplete -= HandleOnRoundComplete;
		NetworkManager.onWinnerInfoReceived -= HandleOnWinnerInfoReceived;
		NetworkManager.onActionHistoryReceived -= HandleOnActionHistoryReceived;
		NetworkManager.onRebuyActionResponseReceived -= HandleOnRebuyActionResponseReceived;
		NetworkManager.playerRequestedSitout -= HandlePlayerReqestedSitout;
		NetworkManager.playerRequestedBackToGame -= HandlePlayerReqestedBackToGame;
		NetworkManager.anteAndBlindRequestReceived -= HandleAnteAndBlindRequestReceived;
		NetworkManager.addChipsResponseReceived -= HandleOnAddChipsResponseReceived;
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
		yield return new WaitForEndOfFrame ();

		DisplayTotalChips ();
		SetPlayerName ();

		StartCoroutine (GetProfilePic ());
	}

	public void SetChipsPosition ()
	{
		if (transform.parent.position.x > GameManager.Instance.transform.position.x) {
			Vector3 pos = txtBetAmount.transform.parent.localPosition;
			pos.x = -pos.x;
			txtBetAmount.transform.parent.localPosition = pos;
		}
	}

	private IEnumerator GetProfilePic ()
	{
		yield return new WaitForEndOfFrame ();
		WWW www = new WWW (Constants.RESIZE_IMAGE_URL + playerInfo.Profile_Pic);
		yield return www;

		if (www.error != null) {
			DebugLog.LogError ("Error while downloading profile pic  : " + www.error + "\nURL  : " + www.url);
		} else {
			if (www.texture != null) {
				imgProfile.sprite = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), Vector2.zero);
			}
		}
	}

	public void DisplayBetAmount ()
	{
		if (UIManager.Instance.isRealMoney)
			txtBetAmount.text = Utility.GetAmount (betAmount);
		else
			txtBetAmount.text = Utility.GetAmount (System.Math.Round ((float)betAmount));

		txtBetAmount.transform.parent.gameObject.SetActive (betAmount > 0);
	}

	public void DisplayTotalChips ()
	{
		txtTotalChips.text = Utility.GetAmount (buyinChips);

		if (playerID.Equals (NetworkManager.Instance.playerID)) {
			if (UIManager.Instance.isRealMoney)
				GameManager.Instance.DisplayPlayerTotalChips (totalRealMoney);
			else
				GameManager.Instance.DisplayPlayerTotalChips (totalChips);
		}
	}

	public void DisplayUpWhoopAssCard ()
	{
		whoopAssCardUpPosition.gameObject.SetActive (true);
	}

	public void OpenWhoopAssCard ()
	{
		whoopAssCardUpPosition.GetComponent<Image> ().sprite = Resources.Load<Sprite> (Constants.RESOURCE_GAMECARDS + playerInfo.WACard);
	}

	public void DisplayDownWhoopAssCard ()
	{
		whoopAssCardDownPosition.gameObject.SetActive (true);

		if (playerID.Equals (NetworkManager.Instance.playerID))
			whoopAssCardDownPosition.GetComponent<Image> ().sprite = Resources.Load<Sprite> (Constants.RESOURCE_GAMECARDS + playerInfo.WACard);
	}

	private void DisplayChipOnPlayerAction (ActionResponse ar)
	{
		SoundManager.Instance.PlayBetCallSound (Camera.main.transform.position);

		switch (RoundController.GetInstance ().currentTableGameRound) {
		case TABLE_GAME_ROUND.START:
//			GenerateChipOnPos (anteImagePosition.position);
//
//			GenerateChipOnPos (blindImagePosition.position);

			if (ar.IsBetOnStraight) {
				GenerateChipOnPos (straightOrBetterImagePosition.position);
			}
			break;
		case TABLE_GAME_ROUND.FIRST_BET:
			GenerateChipOnPos (bet1ImagePosition.position);
			break;
		case TABLE_GAME_ROUND.SECOND_BET:
			GenerateChipOnPos (bet2ImagePosition.position);
			break;
		case TABLE_GAME_ROUND.THIRD_BET:
			GenerateChipOnPos (bet3ImagePosition.position);
			break;
		case TABLE_GAME_ROUND.WHOOPASS:
			if (ar.Action == (int)PLAYER_ACTION.ACTION_WA_NO)
				break;
			
			Vector3 targetPos = ar.Action == (int)PLAYER_ACTION.ACTION_WA_UP ? btnUp.transform.position : btnDown.transform.position;
			GenerateChipOnPos (targetPos);
			break;
		case TABLE_GAME_ROUND.PLAY:
			GenerateChipOnPos (playImagePosition.position);
			break;
		}
	}

	/// <summary>
	/// Displays the chip on players for waiting player.
	/// </summary>
	/// <param name="ar">Action Response.</param>
	/// <param name="round">Game Round.</param>
	private void DisplayChipOnPlayersForWaitingPlayer (ActionResponse ar, int round)
	{
		switch (round) {
		case (int) TABLE_GAME_ROUND.START:
			GenerateChipOnPos (anteImagePosition.position);

			GenerateChipOnPos (blindImagePosition.position);

			if (ar.IsBetOnStraight) {
				GenerateChipOnPos (straightOrBetterImagePosition.position);
			}
			break;
		case (int) TABLE_GAME_ROUND.FIRST_BET:
			GenerateChipOnPos (bet1ImagePosition.position);
			break;
		case (int) TABLE_GAME_ROUND.SECOND_BET:
			GenerateChipOnPos (bet2ImagePosition.position);
			break;
		case (int) TABLE_GAME_ROUND.THIRD_BET:
			GenerateChipOnPos (bet3ImagePosition.position);
			break;
		case (int) TABLE_GAME_ROUND.WHOOPASS:
			if (ar.Action == (int)PLAYER_ACTION.ACTION_WA_NO)
				break;

			Vector3 targetPos = ar.Action == (int)PLAYER_ACTION.ACTION_WA_UP ? btnUp.transform.position : btnDown.transform.position;
			GenerateChipOnPos (targetPos);
			break;
		case (int) TABLE_GAME_ROUND.PLAY:
			GenerateChipOnPos (playImagePosition.position);
			break;
		}
	}

	public void GenerateChipOnPos (Vector3 pos)
	{
		pos.y += 10f;
		
		GameObject chip = Instantiate (chipPrefab, pos, Quaternion.identity) as GameObject;
		chip.transform.SetParent (transform);
		chip.transform.localScale = Vector3.one;
		chipDisplayedList.Add (chip);

		SoundManager.Instance.PlayChipsSound (Camera.main.transform.position);
	}

	public void SetPlayerName ()
	{
		string playerName = playerID;

		if (playerName.Length > 10)
			playerName = playerName.Substring (0, 10) + "..";

		txtPlayerName.text = playerName;
		txtPlayerName.color = Color.yellow;
	}

	public void OnProfileImageButtonTap ()
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

	#region DELEGATE_CALLBACKS

	private void HandleOnActionResponseReceived (string sender, string response)
	{
		ActionResponse ar = JsonUtility.FromJson<ActionResponse> (response);

		if (playerID.Equals (ar.Player_Name)) {
			buyinChips = ar.Player_BuyIn_Chips;
			totalRealMoney = ar.Player_Total_Real_Chips;
			totalChips = ar.Player_Total_Play_Chips;
			DisplayTotalChips ();

			betAmount += ar.Bet_Amount;
			DisplayBetAmount ();

			if (ar.Bet_Amount > 0)
				DisplayChipOnPlayerAction (ar);

			if (ar.Action == (int)PLAYER_ACTION.ACTION_WA_UP)
				DisplayUpWhoopAssCard ();
			else if (ar.Action == (int)PLAYER_ACTION.ACTION_WA_DOWN)
				DisplayDownWhoopAssCard ();

			if (ar.Action == (int)PLAYER_ACTION.CHECK)
				SoundManager.Instance.PlayCheckActionSound (Camera.main.transform.position);


			//	Display dealer WA card
			if (ar.Action == (int)PLAYER_ACTION.ACTION_WA_UP ||
			    ar.Action == (int)PLAYER_ACTION.ACTION_WA_DOWN ||
			    ar.Action == (int)PLAYER_ACTION.ACTION_WA_NO) {
				if (playerID.Equals (NetworkManager.Instance.playerID))
					GameManager.Instance.dealerWhoopAssCard.gameObject.SetActive (true);

				SoundManager.Instance.PlayBetCallSound (Camera.main.transform.position);
			}

			if (ar.Action == (int)PLAYER_ACTION.TIMEOUT) {
				GetComponent<CanvasGroup> ().alpha = .4f;
				playerInfo.Player_Status = (int)PLAYER_ACTION.TIMEOUT;
				SoundManager.Instance.PlayFoldActionSound (Camera.main.transform.position);
				playerFoldParent.SetActive (true);
			} else if (ar.Action == (int)PLAYER_ACTION.FOLD) {
				GetComponent<CanvasGroup> ().alpha = .4f;
				playerInfo.Player_Status = (int)PLAYER_ACTION.FOLD;
				SoundManager.Instance.PlayFoldActionSound (Camera.main.transform.position);
				playerFoldParent.SetActive (true);
			} else if (ar.Action == (int)PLAYER_ACTION.ALLIN) {
				playerInfo.Player_Status = (int)PLAYER_ACTION.ALLIN;
				SoundManager.Instance.PlayAllinActionSound (Camera.main.transform.position);
			}
			
//			playerInfo.Player_Status = ar.Action;

			HistoryManager.GetInstance ().AddHistory (playerID, "name", RoundController.GetInstance ().currentTableGameRound, ar.Bet_Amount, betAmount, GetPlayerAction (ar.Action), ar.IsBetOnStraight);
		}
	}

	private void HandleOnResetData ()
	{
		GetComponent<CanvasGroup> ().alpha = 1f;
		playerFoldParent.SetActive (false);

		foreach (GameObject g in chipDisplayedList) {
			Destroy (g);
		}

		chipDisplayedList = new List<GameObject> ();

		whoopAssCardUpPosition.GetComponent<Image> ().sprite = Resources.Load<Sprite> (Constants.RESOURCE_BACK_CARD);
		whoopAssCardUpPosition.gameObject.SetActive (false);
		whoopAssCardDownPosition.gameObject.SetActive (false);

		betAmount = 0;
		DisplayBetAmount ();

		DestroyCards ();
	}

	private void HandleOnMoveCompletedByPlayer (MoveEvent moveEvent)
	{
		HideTurnTimer ();

		//if (playerID.Equals (moveEvent.getNextTurn ()))
		//	DisplayTurnTimer ();
	}

	private void HandleOnRoundComplete (string sender, string roundInfo)
	{
		GameRound round = JsonUtility.FromJson<GameRound> (roundInfo);
		if (round.Round == (int)TABLE_GAME_ROUND.PLAY) {
			OpenWhoopAssCard ();
		}
	}

	private void HandleOnWinnerInfoReceived (string sender, string winnerInfo)
	{
		HideTurnTimer ();
	}

	private void HandleOnActionHistoryReceived (string sender, string actionHistory)
	{
		JSON_Object obj = new JSON_Object (actionHistory);

		//int roundStatus = obj.getInt (Constants.FIELD_ACTION_HISTORY_ROUND_STATUS);
		int round = obj.getInt (Constants.FIELD_ACTION_HISTORY_ROUND);

		JSONArray turnsArray = obj.getJSONArray (Constants.FIELD_ACTION_HISTORY_TURNS);

		for (int i = 0; i < turnsArray.Count (); i++) {
			ActionResponse ar = JsonUtility.FromJson<ActionResponse> (turnsArray.getString (i));

			if (playerID.Equals (ar.Player_Name)) {
				betAmount += ar.Bet_Amount;
				buyinChips = ar.Player_BuyIn_Chips;

				if (ar.Bet_Amount > 0)
					DisplayChipOnPlayersForWaitingPlayer (ar, round);

				if (ar.Action == (int)PLAYER_ACTION.FOLD ||
				    ar.Action == (int)PLAYER_ACTION.TIMEOUT)
					GetComponent<CanvasGroup> ().alpha = .4f;
				playerFoldParent.SetActive (true);
			}
		}

		DisplayBetAmount ();
		DisplayTotalChips ();
	}

	private void HandleOnRebuyActionResponseReceived (string sender, string rebuyInfo)
	{
		if (sender.Equals (Constants.TABLEGAME_SERVER_NAME)) {
			RebuyAction action = JsonUtility.FromJson<RebuyAction> (rebuyInfo);
			if (playerID.Equals (action.Player_Name)) {
				buyinChips = action.Player_BuyIn_Chips;
				totalChips = action.Player_Total_Play_Chips;
				totalRealMoney = action.Player_Total_Real_Chips;

				DisplayTotalChips ();

				if (playerID.Equals (GameManager.Instance.currentTurnPlayerID) &&
				    playerID.Equals (NetworkManager.Instance.playerID) &&
				    !GameManager.Instance.isGameCompleted)
					GameManager.Instance.DisplayAppropriateBetPanel ();
			}

			if (action.Player_Name.Equals (NetworkManager.Instance.playerID)) {
				if (GameManager.Instance.ownTablePlayer.buyinChips >= Constants.TABLE_GAME_PLAY_MIN_CHIPS) {
					GameManager.Instance.isSitoutForInsufficientChips = false;
					if (GameManager.Instance.isSitoutForInsufficientChips)
						GameManager.Instance.OnBackToGameButtonTap ();
				}
			}
		}
	}

	private void HandlePlayerReqestedSitout (string sender)
	{
		if (playerID.Equals (sender)) {
			playerInfo.Player_Status = (int)PLAYER_STATUS.SIT_OUT;
			imgSitout.gameObject.SetActive (true);

			if (playerID.Equals (NetworkManager.Instance.playerID))
				GameManager.Instance.btnBackToGame.interactable = true;
		}
	}

	private void HandlePlayerReqestedBackToGame (string sender)
	{
		if (playerID.Equals (sender)) {
			//imgAbsentPlayer.gameObject.SetActive(false);
			imgSitout.gameObject.SetActive (false);
		}
	}

	private void HandleAnteAndBlindRequestReceived (string playerID, string abInfo)
	{
		if (playerID.Equals (this.playerID)) {
			GenerateChipOnPos (anteImagePosition.position);
			GenerateChipOnPos (blindImagePosition.position);
		}
	}

	private void HandleOnAddChipsResponseReceived (string sender, string rebuyInfo)
	{
		if (sender.Equals (Constants.TABLEGAME_SERVER_NAME)) {
			RebuyAction rebuy = JsonUtility.FromJson<RebuyAction> (rebuyInfo);

			if (rebuy.Player_Name.Equals (playerID)) {
				buyinChips = rebuy.Player_BuyIn_Chips;
				totalChips = rebuy.Player_Total_Play_Chips;
				totalRealMoney = rebuy.Player_Total_Real_Chips;
				DisplayTotalChips ();
			}
		}
	}

	private void DestroyCards ()
	{
		foreach (Transform t in card1Position) {
			Destroy (t.gameObject);
		}
		foreach (Transform t in card2Position) {
			Destroy (t.gameObject);
		}
	}

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
		}
		return PLAYER_ACTION.CHECK;
	}

	#endregion

	public void DisplayTurnTimer ()
	{
		if (!GameManager.Instance.isGameCompleted) {
			StartCoroutine ("TurnTimer");
		}
	}

	public void HideTurnTimer ()
	{
		StopCoroutine ("TurnTimer");
		StopCoroutine ("PlayTickSound");
		imgTurnDisplayer.fillAmount = 0;
	}

	private IEnumerator TurnTimer ()
	{
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
}