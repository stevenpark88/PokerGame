using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoundController : MonoBehaviour
{
	[Header ("Table")]
	#region TABLE_GAME
	public TABLE_GAME_ROUND currentTableGameRound;
	public GameObject cardPrefab;
	public GameObject dealerCardPrefab;
	public Transform dealerPosition;

	#endregion

	[Header ("Texass")]
	#region TEXASS_GAME
	public TEXASS_GAME_ROUND currentTexassGameRound;
	public GameObject texassGameDefaultCardPrefab;

	#endregion

	[Header ("WhoopAss")]
	#region WHOOPASS_GAME
	public WHOOPASS_GAME_ROUND currentWhoopAssGameRound;
	public GameObject whoopAssDefaultCardPrefab;
	public GameObject whoopAssPlayerCardPrefab;

	#endregion

	public static bool isBlindAmountCollected = false;

	#region DELEGATES

	public delegate void CollectBlindAmount ();

	public static event CollectBlindAmount collectBlindAmount;

	public static void FireCollectBlindAmount ()
	{
		if (collectBlindAmount != null && !isBlindAmountCollected) {
			isBlindAmountCollected = true;
			collectBlindAmount ();
		}
	}

	public delegate void CardDistributionFinished ();

	public static event CardDistributionFinished cardDistributionFinished;

	public static void FireCardDistributionFinished ()
	{
		if (cardDistributionFinished != null)
			cardDistributionFinished ();
	}

	#endregion

	static RoundController Instance;

	public static RoundController GetInstance ()
	{
		if (Instance == null) {
			Instance = new GameObject ("RoundController").AddComponent<RoundController> ();
		}

		return Instance;
	}

	void Awake ()
	{
		Instance = this;
	}

	void OnEnable ()
	{
		NetworkManager.onRoundComplete += HandleOnRoundComplete;
		GameManager.resetData += HandleOnResetData;
		TexassGame.resetData += HandleOnResetData;
		WhoopAssGame.resetData += HandleOnResetData;
		NetworkManager.onDefaultCardDataReceived += HandleOnDefaultCardsDataReceived;
		NetworkManager.onWinnerInfoReceived += HandleOnWinnerInfoReceived;
		NetworkManager.onDistributeCardResponseReceived += HandleOnDistributeCardResponseReceived;
		NetworkManager.onBlindPlayerResponseReceived += HandleOnBlindPlayerResponseReceived;
		NetworkManager.onGameStartedByPlayer += HandleOnGameStartedByPlayer;
	}

	void OnDisable ()
	{
		NetworkManager.onRoundComplete -= HandleOnRoundComplete;
		GameManager.resetData -= HandleOnResetData;
		TexassGame.resetData -= HandleOnResetData;
		WhoopAssGame.resetData -= HandleOnResetData;
		NetworkManager.onDefaultCardDataReceived -= HandleOnDefaultCardsDataReceived;
		NetworkManager.onWinnerInfoReceived -= HandleOnWinnerInfoReceived;
		NetworkManager.onDistributeCardResponseReceived -= HandleOnDistributeCardResponseReceived;
		NetworkManager.onBlindPlayerResponseReceived -= HandleOnBlindPlayerResponseReceived;
		NetworkManager.onGameStartedByPlayer -= HandleOnGameStartedByPlayer;
	}

	public void DistributeTableGameCards ()
	{
		for (int i = 0; i < GameManager.Instance.allTableGamePlayers.Count; i++) {
			TableGamePlayer p = GameManager.Instance.allTableGamePlayers [i];
			if (p.playerInfo.Player_Status != (int)PLAYER_STATUS.ACTIVE &&
			    p.playerInfo.Player_Status != (int)PLAYER_STATUS.ABSENT &&
			    p.playerInfo.Player_Status != (int)PLAYER_ACTION.ALLIN)
				continue;

			for (int j = 0; j < 2; j++) {
				GameObject card = Instantiate (cardPrefab, dealerPosition.position, Quaternion.identity) as GameObject;
				card.transform.SetParent (j == 0 ? p.card1Position : p.card2Position);
				card.transform.localScale = Vector3.one;

				if (p.playerID.Equals (NetworkManager.Instance.playerID))
					card.GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (j == 0 ? p.card1 : p.card2);

				Vector3 targetPos = j == 0 ? p.card1Position.position : p.card2Position.position;
				StartCoroutine (MoveCardTo (card.transform, targetPos));
			}
		}


		//	Generate cards for dealer
		for (int i = 0; i < 2; i++) {
			GameObject card = Instantiate (dealerCardPrefab, dealerPosition.position, Quaternion.identity) as GameObject;
			card.transform.SetParent (i == 0 ? GameManager.Instance.dealerCard1.transform : GameManager.Instance.dealerCard2.transform);
			card.transform.localScale = Vector3.one;

			if (i == 0)
				GameManager.Instance.dealerFirstCard = card.GetComponent<CardFlipAnimation> ();
			else
				GameManager.Instance.dealerSecondCard = card.GetComponent<CardFlipAnimation> ();

			Vector3 targetPos = i == 0 ? GameManager.Instance.dealerCard1.transform.position : GameManager.Instance.dealerCard2.transform.position;
			StartCoroutine (MoveCardTo (card.transform, targetPos));

			SoundManager.Instance.PlayCardSuffleSound (Camera.main.transform.position);
		}
	}

	public IEnumerator DistributeTexassGameCards ()
	{
		yield return new WaitForSeconds (1f);
		FireCollectBlindAmount ();

//		foreach (TexassPlayer p in TexassGame.Instance.allTexassPlayers)
//			p.DestroyCards ();

		TexassGame.Instance.SortPlayerBySeatIndex ();

		yield return new WaitForSeconds (1);

		Vector3 cardsFromPosition = TexassGame.Instance.playerPositions [0].position;
		TexassPlayer dealerPlayer = TexassGame.Instance.GetDealerPlayer ();
		if (dealerPlayer) {
			cardsFromPosition = dealerPlayer.transform.position;
		}

		int cardDistributionStartFrom = 0;
		TexassPlayer smallBlindPlayer = TexassGame.Instance.GetSmallBlindPlayer ();
		if (smallBlindPlayer)
			cardDistributionStartFrom = TexassGame.Instance.allTexassPlayers.IndexOf (smallBlindPlayer);

		for (int j = 0; j < 2; j++) {
			foreach (TexassPlayer p in TexassGame.Instance.allTexassPlayers) {
				TexassPlayer player = TexassGame.Instance.allTexassPlayers [cardDistributionStartFrom++];

				if (cardDistributionStartFrom >= TexassGame.Instance.allTexassPlayers.Count)
					cardDistributionStartFrom = 0;

				if (UIManager.Instance.isRegularTournament || UIManager.Instance.isSitNGoTournament) {
					if (player.playerInfo.Player_Status != (int)PLAYER_STATUS.ACTIVE &&
					    player.playerInfo.Player_Status != (int)PLAYER_STATUS.ABSENT &&
					    player.playerInfo.Player_Status != (int)PLAYER_STATUS.SIT_OUT &&
					    player.playerInfo.Player_Status != (int)PLAYER_ACTION.ALLIN)
						continue;
				} else {
					if (player.playerInfo.Player_Status != (int)PLAYER_STATUS.ACTIVE &&
					    player.playerInfo.Player_Status != (int)PLAYER_STATUS.ABSENT &&
					    player.playerInfo.Player_Status != (int)PLAYER_ACTION.ALLIN)
						continue;
				}


				if (j == 0 && player.card1Position.childCount > 0 && player.playerID.Equals (NetworkManager.Instance.playerID)) {
					player.card1Position.GetChild (0).GetComponent<CardFlipAnimation> ().PlayAnimation (j == 0 ? player.card1 : player.card2);
					continue;
				}
				if (j == 1 && player.card2Position.childCount > 0 && player.playerID.Equals (NetworkManager.Instance.playerID)) {
					player.card2Position.GetChild (0).GetComponent<CardFlipAnimation> ().PlayAnimation (j == 0 ? player.card1 : player.card2);
					continue;
				}

				GameObject card = Instantiate (cardPrefab, cardsFromPosition, Quaternion.identity) as GameObject;
				card.transform.SetParent (j == 0 ? player.card1Position : player.card2Position);
				card.transform.localScale = Vector3.one;
				card.transform.SetParent (TexassGame.Instance.objectsGenerateHere);

				Vector3 targetPos = j == 0 ? player.card1Position.position : player.card2Position.position;
				//				StartCoroutine (MoveCardTo (card.transform, targetPos));

				StartCoroutine (DistTexassCard (j, card.transform, cardsFromPosition, targetPos, player));

				yield return new WaitForSeconds (.1f);
			}
		}

		FireCardDistributionFinished ();
		TexassGame.Instance.currentGameStatus = GAME_STATUS.RUNNING;
	}

	private IEnumerator DistTexassCard (int j, Transform card, Vector3 cardsFromPosition, Vector3 targetPos, TexassPlayer player)
	{
		SoundManager.Instance.PlayCardSuffleSound (Camera.main.transform.position);

		float a = 0;
		while (a < 1) {
			a += 4 * Time.deltaTime;
			card.transform.position = Vector3.Lerp (cardsFromPosition, targetPos, a);

			yield return 0;
		}

		if (j == 0)
			player.DestroyCard1 ();
		else
			player.DestroyCard2 ();

		card.transform.SetParent (j == 0 ? player.card1Position : player.card2Position);

		if (player.playerID.Equals (NetworkManager.Instance.playerID))
			card.GetComponent<CardFlipAnimation> ().PlayAnimation (j == 0 ? player.card1 : player.card2);
	}

	private IEnumerator DistributeWhoopAssGameCards ()
	{
		yield return new WaitForSeconds (1f);
		FireCollectBlindAmount ();

//		foreach (WhoopAssPlayer p in WhoopAssGame.Instance.allWhoopAssPlayers)
//			p.DestroyCards ();

		WhoopAssGame.Instance.SortPlayerBySeatIndex ();

		yield return new WaitForSeconds (1f);

		Vector3 cardsFromPosition = WhoopAssGame.Instance.playerPositions [0].position;
		WhoopAssPlayer dealerPlayer = WhoopAssGame.Instance.GetDealerPlayer ();
		if (dealerPlayer) {
			cardsFromPosition = dealerPlayer.transform.position;
		}

		int cardDistributionStartFrom = 0;
		WhoopAssPlayer smallBlindPlayer = WhoopAssGame.Instance.GetSmallBlindPlayer ();
		if (smallBlindPlayer)
			cardDistributionStartFrom = WhoopAssGame.Instance.allWhoopAssPlayers.IndexOf (smallBlindPlayer);

		for (int j = 0; j < 2; j++) {
			foreach (WhoopAssPlayer p in WhoopAssGame.Instance.allWhoopAssPlayers) {
				WhoopAssPlayer player = WhoopAssGame.Instance.allWhoopAssPlayers [cardDistributionStartFrom++];

				if (cardDistributionStartFrom >= WhoopAssGame.Instance.allWhoopAssPlayers.Count)
					cardDistributionStartFrom = 0;

				if (UIManager.Instance.isRegularTournament || UIManager.Instance.isSitNGoTournament) {
					if (player.playerInfo.Player_Status != (int)PLAYER_STATUS.ACTIVE &&
					    player.playerInfo.Player_Status != (int)PLAYER_STATUS.ABSENT &&
					    player.playerInfo.Player_Status != (int)PLAYER_STATUS.SIT_OUT &&
					    player.playerInfo.Player_Status != (int)PLAYER_ACTION.ALLIN)
						continue;
				} else {
					if (player.playerInfo.Player_Status != (int)PLAYER_STATUS.ACTIVE &&
					    player.playerInfo.Player_Status != (int)PLAYER_STATUS.ABSENT &&
					    player.playerInfo.Player_Status != (int)PLAYER_ACTION.ALLIN)
						continue;
				}

				if (j == 0 && player.card1Position.childCount > 0 && player.playerID.Equals (NetworkManager.Instance.playerID)) {
					player.card1Position.GetChild (0).GetComponent<CardFlipAnimation> ().PlayAnimation (j == 0 ? player.card1 : player.card2);
					continue;
				}
				if (j == 1 && player.card2Position.childCount > 0 && player.playerID.Equals (NetworkManager.Instance.playerID)) {
					p.card2Position.GetChild (0).GetComponent<CardFlipAnimation> ().PlayAnimation (j == 0 ? player.card1 : player.card2);
					continue;
				}

				GameObject card = Instantiate (whoopAssPlayerCardPrefab, cardsFromPosition, Quaternion.identity) as GameObject;
				card.transform.SetParent (j == 0 ? player.card1Position : player.card2Position);
				card.transform.localScale = Vector3.one;
				card.transform.SetParent (WhoopAssGame.Instance.objectsGenerateHere);

				Vector3 targetPos = j == 0 ? player.card1Position.position : player.card2Position.position;
				//				StartCoroutine (MoveCardTo (card.transform, targetPos));

				StartCoroutine (DistWhoopAssCard (j, card.transform, cardsFromPosition, targetPos, player));

				yield return new WaitForSeconds (.1f);
			}
		}

		FireCardDistributionFinished ();
		WhoopAssGame.Instance.currentGameStatus = GAME_STATUS.RUNNING;
	}

	private IEnumerator DistWhoopAssCard (int j, Transform card, Vector3 cardsFromPosition, Vector3 targetPos, WhoopAssPlayer player)
	{
		SoundManager.Instance.PlayCardSuffleSound (Camera.main.transform.position);

		float a = 0;
		while (a < 1) {
			a += 4 * Time.deltaTime;
			card.transform.position = Vector3.Lerp (cardsFromPosition, targetPos, a);

			yield return 0;
		}

		if (j == 0)
			player.DestroyCard1 ();
		else
			player.DestroyCard2 ();

		card.transform.SetParent (j == 0 ? player.card1Position : player.card2Position);

		if (player.playerID.Equals (NetworkManager.Instance.playerID))
			card.GetComponent<CardFlipAnimation> ().PlayAnimation (j == 0 ? player.card1 : player.card2);
	}

	private IEnumerator MoveCardTo (Transform card, Vector3 targetPos)
	{
		Vector3 fromPos = card.position;

		float i = 0;
		while (i < 1) {
			i += 3 * Time.deltaTime;
			card.position = Vector3.Lerp (fromPos, targetPos, i);

			yield return 0;
		}
	}

	/// <summary>
	/// Gets the max bet amount in current round.
	/// </summary>
	/// <returns>The max bet amount in current round.</returns>
	public double GetMinBetAmountInCurrentRound ()
	{
		if (UIManager.Instance.gameType == POKER_GAME_TYPE.TEXAS) {
			List<TexassGameHistory> history = HistoryManager.GetInstance ().GetTexassGameHistory (currentTexassGameRound);

			double minBetAmount = 0;
			for (int i = 0; i < history.Count; i++) {
				if (history [i].totalBetAmount >= minBetAmount)
					minBetAmount = history [i].totalBetAmount;
			}

			if (minBetAmount < TexassGame.Instance.bigBlindAmount && currentTexassGameRound == TEXASS_GAME_ROUND.PREFLOP)
				minBetAmount = TexassGame.Instance.bigBlindAmount;

			return minBetAmount;
		} else {
			List<WhoopAssGameHistory> history = HistoryManager.GetInstance ().GetWhoopAssGameHistory (currentWhoopAssGameRound);
			double minBetAmount = 0;
			for (int i = 0; i < history.Count; i++) {
				if (history [i].totalBetAmount >= minBetAmount)
					minBetAmount = history [i].totalBetAmount;
			}

			if (minBetAmount < WhoopAssGame.Instance.bigBlindAmount && currentWhoopAssGameRound == WHOOPASS_GAME_ROUND.START)
				minBetAmount = WhoopAssGame.Instance.bigBlindAmount;

			return minBetAmount;
		}
	}

	public double GetMaxBetAmountInWhoopAssRound (WHOOPASS_GAME_ROUND gr)
	{
		List<WhoopAssGameHistory> history = HistoryManager.GetInstance ().GetWhoopAssGameHistory (gr);

		double minBetAmount = 0;
		for (int i = 0; i < history.Count; i++) {
			if (history [i].betAmount >= minBetAmount)
				minBetAmount = history [i].betAmount;
		}

		return minBetAmount;
	}

	public double GetLastCallAmountByPlayerInWhoopAssRound (string playerID, WHOOPASS_GAME_ROUND gr)
	{
		List<WhoopAssGameHistory> history = HistoryManager.GetInstance ().GetWhoopAssGameHistory (gr);

		double minBetAmount = 0;
		for (int i = 0; i < history.Count; i++) {
			if (history [i].playerID.Equals (playerID))
				minBetAmount += history [i].betAmount;
		}

		return minBetAmount;
	}

	#region DELEGATE_CALLBACKS

	private void HandleOnRoundComplete (string sender, string roundInfo)
	{
		SoundManager.Instance.PlayRoundCompleteSound (Camera.main.transform.position);

		GameRound round = JsonUtility.FromJson<GameRound> (roundInfo);
		if (sender.Equals (Constants.TABLEGAME_SERVER_NAME)) {
			SetTableGameRound (round.Round);
		} else if (sender.Equals (Constants.TEXASS_SERVER_NAME)) {
			SetTexassGameRound (round.Round);
		} else if (sender.Equals (Constants.WHOOPASS_SERVER_NAME)) {
			SetWhoopAssGameRound (round.Round);
		}
	}

	private void HandleOnResetData ()
	{
		if (UIManager.Instance.gameType == POKER_GAME_TYPE.TABLE) {
			HideAllFlopCards ();
		} else if (UIManager.Instance.gameType == POKER_GAME_TYPE.TEXAS) {
			TexassGame.Instance.currentGameStatus = GAME_STATUS.STOPPED;
			HideAllTexassCards ();
		} else if (UIManager.Instance.gameType == POKER_GAME_TYPE.WHOOPASS) {
			WhoopAssGame.Instance.currentGameStatus = GAME_STATUS.STOPPED;
			HideAllWhoopAssCards ();
		}
	}

	private void HandleOnDefaultCardsDataReceived (string sender, string cardData)
	{
		if (sender.Equals (Constants.TABLEGAME_SERVER_NAME)) {
			DefaultCards defaultCards = JsonUtility.FromJson<DefaultCards> (cardData);
			GameManager.Instance.defaultCards = defaultCards;
			
			//GameManager.Instance.dealerFlop1Card1.GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (defaultCards.FirstFlop1);
			//GameManager.Instance.dealerFlop1Card2.GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (defaultCards.FirstFlop2);
			//GameManager.Instance.dealerFlop2Card1.GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (defaultCards.SecondFlop1);
			//GameManager.Instance.dealerFlop2Card2.GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (defaultCards.SecondFlop2);
			//GameManager.Instance.dealerFlop3Card1.GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (defaultCards.ThirdFlop1);
			//GameManager.Instance.dealerFlop3Card2.GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (defaultCards.ThirdFlop2);
		} else if (sender.Equals (Constants.WHOOPASS_SERVER_NAME)) {
			DefaultCards defaultCards = JsonUtility.FromJson<DefaultCards> (cardData);
			WhoopAssGame.Instance.whoopAssGameDefaultCards = defaultCards;

			//WhoopAssGame.Instance.defaultTableCardsList [0].GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (defaultCards.FirstFlop1);
			//WhoopAssGame.Instance.defaultTableCardsList [1].GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (defaultCards.FirstFlop2);
			//WhoopAssGame.Instance.defaultTableCardsList [2].GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (defaultCards.SecondFlop1);
			//WhoopAssGame.Instance.defaultTableCardsList [3].GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (defaultCards.SecondFlop2);
			//WhoopAssGame.Instance.defaultTableCardsList [4].GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (defaultCards.ThirdFlop1);
			//WhoopAssGame.Instance.defaultTableCardsList [5].GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (defaultCards.ThirdFlop2);
		}
	}

	private void HandleOnWinnerInfoReceived (string sender, string winnerInfo)
	{
		isBlindAmountCollected = false;

		if (sender.Equals (Constants.TABLEGAME_SERVER_NAME)
		    && winnerInfo != null) {
			OpenFirstFlopCards ();
			OpenSecondFlopCards ();
			OpenThirdFlopCards ();
		} else if (sender.Equals (Constants.TEXASS_SERVER_NAME)) {
			if (TexassGame.Instance.GetActivePlayers () > 1) {
				if (currentTexassGameRound == TEXASS_GAME_ROUND.PREFLOP) {
					OpenTexassFlopCards ();
					OpenTexassTurnCard ();
					OpenTexassRiverCard ();
				} else if (currentTexassGameRound == TEXASS_GAME_ROUND.FLOP) {
					OpenTexassTurnCard ();
					OpenTexassRiverCard ();
				} else if (currentTexassGameRound == TEXASS_GAME_ROUND.TURN) {
					OpenTexassRiverCard ();
				}

				TexassGame.Instance.currentGameStatus = GAME_STATUS.FINISHED;
			}
		} else if (sender.Equals (Constants.WHOOPASS_SERVER_NAME)) {
			if (WhoopAssGame.Instance.GetActivePlayers () > 1) {
				OpenWhoopAssFirstFlopCards ();
				OpenWhoopAssSecondFlopCards ();
				OpenWhoopAssThirdFlopCards ();

				WhoopAssGame.Instance.currentGameStatus = GAME_STATUS.FINISHED;
			}
		}
	}

	private void HandleOnDistributeCardResponseReceived (string sender)
	{
		TournamentWinnerPanel.isTournamentWinnersDeclared = false;

		if (sender.Equals (Constants.TEXASS_SERVER_NAME)) {
			currentTexassGameRound = TEXASS_GAME_ROUND.PREFLOP;
			TexassGame.Instance.currentGameStatus = GAME_STATUS.CARD_DISTRIBUTE;
			if (TexassGame.Instance.gameObject.activeSelf)
				StartCoroutine (DistributeTexassGameCards ());
			TexassGame.Instance.isGameCompleted = false;
		} else if (sender.Equals (Constants.WHOOPASS_SERVER_NAME)) {
			currentWhoopAssGameRound = WHOOPASS_GAME_ROUND.START;
			WhoopAssGame.Instance.currentGameStatus = GAME_STATUS.CARD_DISTRIBUTE;
			if (WhoopAssGame.Instance.gameObject.activeSelf)
				StartCoroutine (DistributeWhoopAssGameCards ());
			WhoopAssGame.Instance.isGameCompleted = false;
		}
	}

	private void HandleOnBlindPlayerResponseReceived (string sender, string blindPlayerInfo)
	{
		if ((sender.Equals (Constants.TEXASS_SERVER_NAME) && TexassGame.Instance.currentGameStatus == GAME_STATUS.CARD_DISTRIBUTE) ||
		    (sender.Equals (Constants.WHOOPASS_SERVER_NAME) && WhoopAssGame.Instance.currentGameStatus == GAME_STATUS.CARD_DISTRIBUTE))
			Invoke ("CBA", .5f);
	}

	private void HandleOnGameStartedByPlayer (string sender, string gameStarter)
	{
		if (UIManager.Instance.gameType == POKER_GAME_TYPE.TEXAS)
			TexassGame.Instance.isGameCompleted = false;
		else if (UIManager.Instance.gameType == POKER_GAME_TYPE.WHOOPASS)
			WhoopAssGame.Instance.isGameCompleted = false;
	}

	#endregion

	public void SetTableGameRound (int round)
	{
		switch (round) {
		case (int) TABLE_GAME_ROUND.FIRST_BET:
			currentTableGameRound = TABLE_GAME_ROUND.FIRST_BET;
			DistributeTableGameCards ();
			break;
		case (int) TABLE_GAME_ROUND.PLAY:
			currentTableGameRound = TABLE_GAME_ROUND.PLAY;
			OpenThirdFlopCards ();
			OpenWhoopAssCard ();
			break;
		case (int) TABLE_GAME_ROUND.SECOND_BET:
			currentTableGameRound = TABLE_GAME_ROUND.SECOND_BET;
			OpenFirstFlopCards ();
			break;
		case (int) TABLE_GAME_ROUND.START:
			currentTableGameRound = TABLE_GAME_ROUND.START;
			break;
		case (int) TABLE_GAME_ROUND.THIRD_BET:
			currentTableGameRound = TABLE_GAME_ROUND.THIRD_BET;
			OpenSecondFlopCards ();
			break;
		case (int) TABLE_GAME_ROUND.WHOOPASS:
			currentTableGameRound = TABLE_GAME_ROUND.WHOOPASS;
			break;
		}
	}

	public void SetTexassGameRound (int round)
	{
		switch (round) {
		case (int)TEXASS_GAME_ROUND.PREFLOP:
			currentTexassGameRound = TEXASS_GAME_ROUND.PREFLOP;
			break;
		case (int)TEXASS_GAME_ROUND.FLOP:
			currentTexassGameRound = TEXASS_GAME_ROUND.FLOP;
			OpenTexassFlopCards ();
			break;
		case (int)TEXASS_GAME_ROUND.TURN:
			currentTexassGameRound = TEXASS_GAME_ROUND.TURN;
			OpenTexassTurnCard ();
			break;
		case (int)TEXASS_GAME_ROUND.RIVER:
			currentTexassGameRound = TEXASS_GAME_ROUND.RIVER;
			OpenTexassRiverCard ();
			break;
		}
	}

	public void SetWhoopAssGameRound (int round)
	{
		switch (round) {
		case (int)WHOOPASS_GAME_ROUND.START:
			currentWhoopAssGameRound = WHOOPASS_GAME_ROUND.START;
			break;
		case (int)WHOOPASS_GAME_ROUND.FIRST_FLOP:
			currentWhoopAssGameRound = WHOOPASS_GAME_ROUND.FIRST_FLOP;
			OpenWhoopAssFirstFlopCards ();
			break;
		case (int)WHOOPASS_GAME_ROUND.SECOND_FLOP:
			currentWhoopAssGameRound = WHOOPASS_GAME_ROUND.SECOND_FLOP;
			OpenWhoopAssFirstFlopCards ();
			OpenWhoopAssSecondFlopCards ();
			break;
		case (int)WHOOPASS_GAME_ROUND.WHOOPASS_CARD:
			currentWhoopAssGameRound = WHOOPASS_GAME_ROUND.WHOOPASS_CARD;
			OpenWhoopAssFirstFlopCards ();
			OpenWhoopAssSecondFlopCards ();
			break;
		case (int)WHOOPASS_GAME_ROUND.THIRD_FLOP:
			currentWhoopAssGameRound = WHOOPASS_GAME_ROUND.THIRD_FLOP;
			OpenWhoopAssFirstFlopCards ();
			OpenWhoopAssSecondFlopCards ();
			OpenWhoopAssThirdFlopCards ();
			break;
		}
	}

	#region HANDLE_TABLE_GAME_CARDS

	private void OpenFirstFlopCards ()
	{
		GameManager gm = GameManager.Instance;
		if (gm.dealerFlop1Card1.gameObject.activeSelf)
			return;

		gm.dealerFlop1Card1.gameObject.SetActive (true);
		gm.dealerFlop1Card2.gameObject.SetActive (true);

		gm.dealerFlop1Card1.GetComponent<CardFlipAnimation> ().PlayAnimation (gm.defaultCards.FirstFlop1, .2f);
		gm.dealerFlop1Card2.GetComponent<CardFlipAnimation> ().PlayAnimation (gm.defaultCards.FirstFlop2, .2f);
	}

	private void OpenSecondFlopCards ()
	{
		GameManager gm = GameManager.Instance;
		if (gm.dealerFlop2Card1.gameObject.activeSelf)
			return;

		gm.dealerFlop2Card1.gameObject.SetActive (true);
		gm.dealerFlop2Card2.gameObject.SetActive (true);

		gm.dealerFlop2Card1.GetComponent<CardFlipAnimation> ().PlayAnimation (gm.defaultCards.SecondFlop1, .2f);
		gm.dealerFlop2Card2.GetComponent<CardFlipAnimation> ().PlayAnimation (gm.defaultCards.SecondFlop2, .2f);
	}

	private void OpenThirdFlopCards ()
	{
		GameManager gm = GameManager.Instance;
		if (gm.dealerFlop3Card1.gameObject.activeSelf)
			return;


		gm.dealerFlop3Card1.gameObject.SetActive (true);
		gm.dealerFlop3Card2.gameObject.SetActive (true);

		gm.dealerFlop3Card1.GetComponent<CardFlipAnimation> ().PlayAnimation (gm.defaultCards.ThirdFlop1, .2f);
		gm.dealerFlop3Card2.GetComponent<CardFlipAnimation> ().PlayAnimation (gm.defaultCards.ThirdFlop2, .2f);
	}

	private void HideAllFlopCards ()
	{
		GameManager.Instance.dealerFlop1Card1.gameObject.SetActive (false);
		GameManager.Instance.dealerFlop1Card2.gameObject.SetActive (false);
		GameManager.Instance.dealerFlop2Card1.gameObject.SetActive (false);
		GameManager.Instance.dealerFlop2Card2.gameObject.SetActive (false);
		GameManager.Instance.dealerFlop3Card1.gameObject.SetActive (false);
		GameManager.Instance.dealerFlop3Card2.gameObject.SetActive (false);

		GameManager.Instance.dealerFlop1Card1.GetComponent<CardFlipAnimation> ().ResetImage ();
		GameManager.Instance.dealerFlop1Card2.GetComponent<CardFlipAnimation> ().ResetImage ();
		GameManager.Instance.dealerFlop2Card1.GetComponent<CardFlipAnimation> ().ResetImage ();
		GameManager.Instance.dealerFlop2Card2.GetComponent<CardFlipAnimation> ().ResetImage ();
		GameManager.Instance.dealerFlop3Card1.GetComponent<CardFlipAnimation> ().ResetImage ();
		GameManager.Instance.dealerFlop3Card2.GetComponent<CardFlipAnimation> ().ResetImage ();
	}

	private void OpenWhoopAssCard ()
	{
		if (GameManager.Instance.ownTablePlayer)
			GameManager.Instance.ownTablePlayer.OpenWhoopAssCard ();
	}

	#endregion

	#region HANDLE_WHOOPASS_GAME_CARDS

	private void OpenWhoopAssFirstFlopCards ()
	{
		WhoopAssGame wg = WhoopAssGame.Instance;
		if (wg.defaultTableCardsList [0].gameObject.activeSelf)
			return;

		wg.defaultTableCardsList [0].gameObject.SetActive (true);
		wg.defaultTableCardsList [1].gameObject.SetActive (true);

		wg.defaultTableCardsList [0].GetComponent<CardFlipAnimation> ().PlayAnimation (wg.whoopAssGameDefaultCards.FirstFlop1, .2f);
		wg.defaultTableCardsList [1].GetComponent<CardFlipAnimation> ().PlayAnimation (wg.whoopAssGameDefaultCards.FirstFlop2, .2f);
	}

	private void OpenWhoopAssSecondFlopCards ()
	{
		WhoopAssGame wg = WhoopAssGame.Instance;
		if (wg.defaultTableCardsList [2].gameObject.activeSelf)
			return;

		wg.defaultTableCardsList [2].gameObject.SetActive (true);
		wg.defaultTableCardsList [3].gameObject.SetActive (true);

		wg.defaultTableCardsList [2].GetComponent<CardFlipAnimation> ().PlayAnimation (wg.whoopAssGameDefaultCards.SecondFlop1, .2f);
		wg.defaultTableCardsList [3].GetComponent<CardFlipAnimation> ().PlayAnimation (wg.whoopAssGameDefaultCards.SecondFlop2, .2f);
	}

	private void OpenWhoopAssThirdFlopCards ()
	{
		WhoopAssGame wg = WhoopAssGame.Instance;
		if (wg.defaultTableCardsList [4].gameObject.activeSelf)
			return;

		wg.defaultTableCardsList [4].gameObject.SetActive (true);
		wg.defaultTableCardsList [5].gameObject.SetActive (true);

		wg.defaultTableCardsList [4].GetComponent<CardFlipAnimation> ().PlayAnimation (wg.whoopAssGameDefaultCards.ThirdFlop1, .2f);
		wg.defaultTableCardsList [5].GetComponent<CardFlipAnimation> ().PlayAnimation (wg.whoopAssGameDefaultCards.ThirdFlop2, .2f);
	}

	private void OpenWhoopAssGameWhoopAssCard ()
	{
		if (WhoopAssGame.Instance.ownWhoopAssPlayer)
			WhoopAssGame.Instance.ownWhoopAssPlayer.OpenWhoopAssCard ();
	}

	#endregion

	public void GenerateTablePlayerCardsForWaitingPlayer ()
	{
		for (int i = 0; i < GameManager.Instance.allTableGamePlayers.Count; i++) {
			TableGamePlayer p = GameManager.Instance.allTableGamePlayers [i];
			if (p.playerInfo.Player_Status == (int)PLAYER_STATUS.WAITING ||
			    p.playerInfo.Player_Status == (int)PLAYER_ACTION.ACTION_WAITING_FOR_GAME)
				continue;

			for (int j = 0; j < 2; j++) {
				if (j == 0 && p.card1Position.childCount > 0)
					continue;
				if (j == 1 && p.card2Position.childCount > 0)
					continue;

				Vector3 targetPos = j == 0 ? p.card1Position.position : p.card2Position.position;
				GameObject card = Instantiate (cardPrefab, targetPos, Quaternion.identity) as GameObject;
				card.transform.SetParent (j == 0 ? p.card1Position : p.card2Position);
				card.transform.localScale = Vector3.one;
			}
		}


		//	Generate cards for dealer
		for (int i = 0; i < 2; i++) {
			Vector3 targetPos = i == 0 ? GameManager.Instance.dealerCard1.transform.position : GameManager.Instance.dealerCard2.transform.position;
			GameObject card = Instantiate (dealerCardPrefab, targetPos, Quaternion.identity) as GameObject;
			card.transform.SetParent (i == 0 ? GameManager.Instance.dealerCard1.transform : GameManager.Instance.dealerCard2.transform);
			card.transform.localScale = Vector3.one;

			if (i == 0)
				GameManager.Instance.dealerFirstCard = card.GetComponent<CardFlipAnimation> ();
			else
				GameManager.Instance.dealerSecondCard = card.GetComponent<CardFlipAnimation> ();
		}
	}

	public void GenerateTexassPlayerCardsForWaitingPlayer ()
	{
		Vector3 cardsFromPosition = TexassGame.Instance.playerPositions [0].position;
		TexassPlayer dealerPlayer = TexassGame.Instance.GetDealerPlayer ();
		if (dealerPlayer) {
			cardsFromPosition = dealerPlayer.transform.position;
		}

		foreach (TexassPlayer p in TexassGame.Instance.allTexassPlayers) {

			if (UIManager.Instance.isRegularTournament || UIManager.Instance.isSitNGoTournament) {
				if (p.playerInfo.Player_Status != (int)PLAYER_STATUS.ACTIVE &&
				    p.playerInfo.Player_Status != (int)PLAYER_STATUS.ABSENT &&
				    p.playerInfo.Player_Status != (int)PLAYER_STATUS.SIT_OUT &&
				    p.playerInfo.Player_Status != (int)PLAYER_ACTION.ALLIN &&
					p.playerInfo.Player_Status != (int)PLAYER_STATUS.FOLDED &&
					p.playerInfo.Player_Status != (int)PLAYER_ACTION.ACTION_FOLDED)
					continue;
			} else {
				if (p.playerInfo.Player_Status != (int)PLAYER_STATUS.ACTIVE &&
				    p.playerInfo.Player_Status != (int)PLAYER_STATUS.ABSENT &&
				    p.playerInfo.Player_Status != (int)PLAYER_ACTION.ALLIN &&
					p.playerInfo.Player_Status != (int)PLAYER_STATUS.FOLDED &&
					p.playerInfo.Player_Status != (int)PLAYER_ACTION.ACTION_FOLDED)
					continue;
			}


			for (int j = 0; j < 2; j++) {

				if (j == 0 && p.card1Position.childCount > 0 && p.playerID.Equals (NetworkManager.Instance.playerID)) {
					p.card1Position.GetChild (0).GetComponent<CardFlipAnimation> ().PlayAnimation (j == 0 ? p.card1 : p.card2);
					continue;
				}
				if (j == 1 && p.card2Position.childCount > 0 && p.playerID.Equals (NetworkManager.Instance.playerID)) {
					p.card2Position.GetChild (0).GetComponent<CardFlipAnimation> ().PlayAnimation (j == 0 ? p.card1 : p.card2);
					continue;
				}

//				if (TexassGame.Instance.currentGameStatus == GAME_STATUS.CARD_DISTRIBUTE) {
//					Vector3 targetPos = j == 0 ? p.card1Position.position : p.card2Position.position;
//					GameObject card = Instantiate (cardPrefab, cardsFromPosition, Quaternion.identity) as GameObject;
//					card.transform.SetParent (j == 0 ? p.card1Position : p.card2Position);
//					card.transform.localScale = Vector3.one;
//					if (p.playerID.Equals (NetworkManager.Instance.playerID))
//						card.GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (j == 0 ? p.card1 : p.card2);
//					StartCoroutine (MoveCardTo (card.transform, j == 0 ? p.card1Position.position : p.card2Position.position));
//				} else {
				if (j == 0)
					p.DestroyCard1 ();
				else
					p.DestroyCard2 ();
					
				Vector3 targetPos = j == 0 ? p.card1Position.position : p.card2Position.position;
				GameObject card = Instantiate (cardPrefab, targetPos, Quaternion.identity) as GameObject;
				card.transform.SetParent (j == 0 ? p.card1Position : p.card2Position);
				card.transform.localScale = Vector3.one;

				if (p.playerID.Equals (NetworkManager.Instance.playerID))
					card.GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (j == 0 ? p.card1 : p.card2);
//				}
			}
		}
	}

	public void GenerateWhoopAssPlayerCardsForWaitingPlayer (bool destroyCards = true)
	{
		Vector3 cardsFromPosition = WhoopAssGame.Instance.playerPositions [0].position;
		WhoopAssPlayer dealerPlayer = WhoopAssGame.Instance.GetDealerPlayer ();
		if (dealerPlayer) {
			cardsFromPosition = dealerPlayer.transform.position;
		}

		for (int i = 0; i < WhoopAssGame.Instance.allWhoopAssPlayers.Count; i++) {
			WhoopAssPlayer p = WhoopAssGame.Instance.allWhoopAssPlayers [i];

			if (UIManager.Instance.isRegularTournament || UIManager.Instance.isSitNGoTournament) {
				if (p.playerInfo.Player_Status != (int)PLAYER_STATUS.ACTIVE &&
				    p.playerInfo.Player_Status != (int)PLAYER_STATUS.ABSENT &&
				    p.playerInfo.Player_Status != (int)PLAYER_STATUS.SIT_OUT &&
					p.playerInfo.Player_Status != (int)PLAYER_ACTION.ALLIN &&
					p.playerInfo.Player_Status != (int)PLAYER_STATUS.FOLDED &&
					p.playerInfo.Player_Status != (int)PLAYER_ACTION.ACTION_FOLDED)
					continue;
			} else {
				if (p.playerInfo.Player_Status != (int)PLAYER_STATUS.ACTIVE &&
				    p.playerInfo.Player_Status != (int)PLAYER_STATUS.ABSENT &&
					p.playerInfo.Player_Status != (int)PLAYER_ACTION.ALLIN &&
					p.playerInfo.Player_Status != (int)PLAYER_STATUS.FOLDED &&
					p.playerInfo.Player_Status != (int)PLAYER_ACTION.ACTION_FOLDED)
					continue;
			}

			for (int j = 0; j < 2; j++) {

				if (j == 0 && p.card1Position.childCount > 0 && p.playerID.Equals (NetworkManager.Instance.playerID)) {
					p.card1Position.GetChild (0).GetComponent<CardFlipAnimation> ().PlayAnimation (j == 0 ? p.card1 : p.card2);
					continue;
				}
				if (j == 1 && p.card2Position.childCount > 0 && p.playerID.Equals (NetworkManager.Instance.playerID)) {
					p.card2Position.GetChild (0).GetComponent<CardFlipAnimation> ().PlayAnimation (j == 0 ? p.card1 : p.card2);
					continue;
				}

				if (j == 0)
					p.DestroyCard1 ();
				else
					p.DestroyCard2 ();

//				if (WhoopAssGame.Instance.currentGameStatus == GAME_STATUS.CARD_DISTRIBUTE) {
//					GameObject card = Instantiate (whoopAssPlayerCardPrefab, cardsFromPosition, Quaternion.identity) as GameObject;
//					card.transform.SetParent (j == 0 ? p.card1Position : p.card2Position);
//					card.transform.localScale = Vector3.one;
//					if (p.playerID.Equals (NetworkManager.Instance.playerID))
//						card.GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (j == 0 ? p.card1 : p.card2);
//					StartCoroutine (MoveCardTo (card.transform, j == 0 ? p.card1Position.position : p.card2Position.position));
//				} else {
				Vector3 targetPos = j == 0 ? p.card1Position.position : p.card2Position.position;
				GameObject card = Instantiate (whoopAssPlayerCardPrefab, targetPos, Quaternion.identity) as GameObject;
				card.transform.SetParent (j == 0 ? p.card1Position : p.card2Position);
				card.transform.localScale = Vector3.one;

				if (p.playerID.Equals (NetworkManager.Instance.playerID))
					card.GetComponent<CardFlipAnimation> ().DisplayCardWithoutAnimation (j == 0 ? p.card1 : p.card2);
//				}
			}
		}
	}

	private void CBA ()
	{
		FireCollectBlindAmount ();
	}

	public void GenerateDealerFlopCardsForWaitingPlayer ()
	{
		switch (currentTableGameRound) {
		case TABLE_GAME_ROUND.FIRST_BET:
			break;
		case TABLE_GAME_ROUND.PLAY:
			OpenFirstFlopCards ();
			OpenSecondFlopCards ();
			OpenThirdFlopCards ();
			OpenWhoopAssCard ();
			break;
		case TABLE_GAME_ROUND.SECOND_BET:
			OpenFirstFlopCards ();
			break;
		case TABLE_GAME_ROUND.START:
			break;
		case TABLE_GAME_ROUND.THIRD_BET:
			OpenFirstFlopCards ();
			OpenSecondFlopCards ();
			break;
		case TABLE_GAME_ROUND.WHOOPASS:
			OpenFirstFlopCards ();
			OpenSecondFlopCards ();
			break;
		}
	}

	public void GenerateTexassTableCardsForWaitingPlayer ()
	{
		switch (currentTexassGameRound) {
		case TEXASS_GAME_ROUND.PREFLOP:
			break;
		case TEXASS_GAME_ROUND.FLOP:
			OpenTexassFlopCards ();
			break;
		case TEXASS_GAME_ROUND.TURN:
			OpenTexassFlopCards ();
			OpenTexassTurnCard ();
			break;
		case TEXASS_GAME_ROUND.RIVER:
			OpenTexassFlopCards ();
			OpenTexassTurnCard ();
			OpenTexassRiverCard ();
			break;
		}
	}

	public void GenerateDefaultCardsForWhoopAssPlayer ()
	{
		switch (currentWhoopAssGameRound) {
		case WHOOPASS_GAME_ROUND.START:
			break;
		case WHOOPASS_GAME_ROUND.FIRST_FLOP:
			OpenWhoopAssFirstFlopCards ();
			break;
		case WHOOPASS_GAME_ROUND.SECOND_FLOP:
		case WHOOPASS_GAME_ROUND.WHOOPASS_CARD:
			OpenWhoopAssFirstFlopCards ();
			OpenWhoopAssSecondFlopCards ();
			break;
		case WHOOPASS_GAME_ROUND.THIRD_FLOP:
			OpenWhoopAssFirstFlopCards ();
			OpenWhoopAssSecondFlopCards ();
			OpenWhoopAssThirdFlopCards ();
			break;
		}
	}

	#region HANDLE_TEXASS_GAME_CARDS

	private void OpenTexassFlopCards ()
	{
		for (int i = 0; i < 3; i++) {
			if (TexassGame.Instance.defaultCardsPositionList [i].childCount > 0)
				continue;

			GameObject obj = Instantiate (texassGameDefaultCardPrefab, TexassGame.Instance.defaultCardsPositionList [i].position, Quaternion.identity) as GameObject;
			obj.transform.SetParent (TexassGame.Instance.defaultCardsPositionList [i]);
			obj.transform.localScale = Vector3.one;

			CardFlipAnimation anim = obj.GetComponent<CardFlipAnimation> ();
			if (i == 0)
				anim.PlayAnimation (TexassGame.Instance.texassDefaultCards.Flop1, .2f);
			else if (i == 1)
				anim.PlayAnimation (TexassGame.Instance.texassDefaultCards.Flop2, .2f);
			else
				anim.PlayAnimation (TexassGame.Instance.texassDefaultCards.Flop3, .2f);

			TexassGame.Instance.defaultCardsList.Add (anim);
		}
	}

	private void OpenTexassTurnCard ()
	{
		if (TexassGame.Instance.defaultCardsPositionList [3].childCount > 0)
			return;
		
		GameObject obj = Instantiate (texassGameDefaultCardPrefab, TexassGame.Instance.defaultCardsPositionList [3].position, Quaternion.identity) as GameObject;
		obj.transform.SetParent (TexassGame.Instance.defaultCardsPositionList [3]);
		obj.transform.localScale = Vector3.one;

		CardFlipAnimation anim = obj.GetComponent<CardFlipAnimation> ();
		anim.PlayAnimation (TexassGame.Instance.texassDefaultCards.Turn, .2f);

		TexassGame.Instance.defaultCardsList.Add (anim);
	}

	private void OpenTexassRiverCard ()
	{
		if (TexassGame.Instance.defaultCardsPositionList [4].childCount > 0)
			return;
		
		GameObject obj = Instantiate (texassGameDefaultCardPrefab, TexassGame.Instance.defaultCardsPositionList [4].position, Quaternion.identity) as GameObject;
		obj.transform.SetParent (TexassGame.Instance.defaultCardsPositionList [4]);
		obj.transform.localScale = Vector3.one;

		CardFlipAnimation anim = obj.GetComponent<CardFlipAnimation> ();
		anim.PlayAnimation (TexassGame.Instance.texassDefaultCards.River, .2f);

		TexassGame.Instance.defaultCardsList.Add (anim);
	}

	private void HideAllTexassCards ()
	{
		foreach (CardFlipAnimation card in TexassGame.Instance.defaultCardsList) {
			Destroy (card.gameObject);
		}

		TexassGame.Instance.defaultCardsList = new List<CardFlipAnimation> ();
	}

	private void HideAllWhoopAssCards ()
	{
		foreach (Transform t in WhoopAssGame.Instance.defaultTableCardsList) {
			t.gameObject.SetActive (false);
			t.GetComponent<CardFlipAnimation> ().ResetImage ();
		}
	}

	#endregion
}