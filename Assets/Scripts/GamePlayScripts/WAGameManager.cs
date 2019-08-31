using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;

public class WAGameManager : MonoBehaviour
{
	
	public Scrollbar logScrollbar;
	public Text txtLog;
	PlayersManager playersManager;
	WADefaultCardsManager defaultCardsManager;
	public GameObject gameManagerGameObject;
	private int currentRound = GameConstant.WA_ROUND_START;
	RoundManager startRound;
	RoundManager firstFlopRound;
	RoundManager secondFlopRound;
	RoundManager whoopAssRound;
	RoundManager thirdRound;
	GameObjectManager gameObjectManager;
	bool isRunningGame = true;
//	private HandleAnimations handleAnim;
	// Use this for initialization
	public GameObject movingCoinObject;
	public GameObject movingCardObject;
	public GameObject waPurchaseOption;
	public Text txtWATitle;
	private int smallBlindAmount = 0;
	private int GAME_STATUS;
	private int waCardAmt = 0;
	private AnimationsManager animationManager;
	public AudioClip cardAudio;
	public AudioClip chipAudio;
	public AudioClip buttonAudio;
	public AudioClip winnerAudio;

	public WAGameManager ()
	{
	}
	// Use this for initialization
	void Start ()
	{
		animationManager = new AnimationsManager (cardAudio,chipAudio);
		gameObjectManager = new GameObjectManager (gameManagerGameObject);
		defaultCardsManager = new WADefaultCardsManager (animationManager,gameObjectManager.getDefaultCardObject ());
		txtLog.transform.parent.GetComponent<ScrollRect> ().verticalNormalizedPosition = 0f;
		playersManager = new PlayersManager ();
		initGamePlay ();
	}

	public void initGamePlay ()
	{
		isRunningGame = true;
		GAME_STATUS = GameConstant.STOPPED;
		raiseValue = (int)0;
		totalBBPlayersTurn = 0;
		SliderChips.value = raiseValue;
		gameObjectManager.getBetAmountText ().text = GameConstant.CURRENCY + raiseValue;
		resetDefaultCards (true);
//		resetWinningPlayers (true);
		removeAllPlayers ();
		setTotalBetTableAmount (0);
		startRound = new RoundManager (GameConstant.WA_ROUND_START);
		firstFlopRound = new RoundManager (GameConstant.WA_ROUND_FIRST_FLOP);
		secondFlopRound = new RoundManager (GameConstant.WA_ROUND_SECOND_FLOP);
		whoopAssRound = new RoundManager (GameConstant.WA_ROUND_WHOOPASS);
		thirdRound = new RoundManager (GameConstant.WA_ROUND_THIRD_FLOP);
		setEnableDisableOptions (false);
	}

	// Update is called once per frame
	void Update ()
	{
		if (playersManager != null && playersManager.totalPlayerOnTable () != 0) {
			foreach (PlayerBean player in playersManager.getAllPlayers()) {
				player.initTurnAnimation ();
			}
		} else {
			return;
		}
	}

	public void setDefaultTableCards (string firstFlop1, string firstFlop2, string secondFlop1, string secondFlop2, string thirdFlop1, string thirdFlop2)
	{
		defaultCardsManager.setDefaultCards (firstFlop1, firstFlop2, secondFlop1, secondFlop2, thirdFlop1, thirdFlop2);
	}

	public RoundManager getCurrentRoundInfo ()
	{
		if (startRound.getStatus () == GameConstant.ROUND_STATUS_ACTIVE) {
			return startRound;
		} else if (firstFlopRound.getStatus () == GameConstant.ROUND_STATUS_ACTIVE) {
			return firstFlopRound;
		} else if (secondFlopRound.getStatus () == GameConstant.ROUND_STATUS_ACTIVE) {
			return secondFlopRound;
		} else if (whoopAssRound.getStatus () == GameConstant.ROUND_STATUS_ACTIVE) {
			return whoopAssRound;
		} else if (thirdRound.getStatus () == GameConstant.ROUND_STATUS_ACTIVE) {
			return thirdRound;
		}
		return null;
	}

	public int getCurrentRoundIndex ()
	{
		return currentRound;
	}

	IEnumerator cardAnimation ()
	{
		yield return new WaitForSeconds (1f);
		yield return StartCoroutine (DistributeHandCardsToAllPlayers ());
		yield return new WaitForSeconds (0.1f);
		foreach (PlayerBean playerBean in playersManager.getAllPlayers()) {
			playerBean.deltPlayerCards ();
		}
	}

	IEnumerator DistributeHandCardsToAllPlayers ()
	{
		PlayerBean dealerPlayer = playersManager.getDealerPlayer();
		GameObject src = dealerPlayer.getCardDeskObject ();
		GameObject cardDeskPosition = dealerPlayer.getCardDeskPositionObject ();
		cardDeskPosition.SetActive (true);
		src.SetActive (true);
		int i = 0;
		while (i < playersManager.getAllPlayers ().Count) {
			PlayerBean player= playersManager.getAllPlayers () [i];
			GameObject card1Obj = player.getCard1Object ();
			GameObject card2Obj = player.getCard2Object();
			src.transform.position = cardDeskPosition.transform.position;
			animationManager.MoveCardsObject (src, card1Obj);
			yield return new WaitForSeconds (GameConstant.ANIM_CARD_TIME);
			card1Obj.SetActive (true);

			src.transform.position = cardDeskPosition.transform.position;
			animationManager.MoveCardsObject(src,card2Obj );
			yield return new WaitForSeconds (GameConstant.ANIM_CARD_TIME);
			card2Obj.SetActive (true);
			i++;
		}
		dealerPlayer.getCardDeskObject ().SetActive (false);
		cardDeskPosition.SetActive (false);
		src.transform.position = cardDeskPosition.transform.position;

		startGameRequest();
	}

	public void distributeCards ()
	{
		startStartRound ();
		manageBlindPlayerTurn ();

		StartCoroutine (cardAnimation ());
	}

	public void startGameRequest(){
//		startStartRound ();
//		manageBlindPlayerTurn ();
//		foreach (PlayerBean playerBean in playersManager.getAllPlayers()) {
//			playerBean.deltPlayerCards ();
//		}
		sendBlindPlayerActionToServer ();
	}
	public void manageBlindPlayerTurn ()
	{
		setSmallBlindDefaultBet ();
		setBigBlindDefaultBet ();
	}

	public void sendBlindPlayerActionToServer ()
	{
		WarpClient.GetInstance ().SendChat (GameConstant.RESPONSE_FOR_DESTRIBUTE_CARD);
	}

	/**
	 * Start pre flop round and set other round status as a pending
	 */
	public void startStartRound ()
	{
		GAME_STATUS = GameConstant.RUNNING;
		currentRound = GameConstant.WA_ROUND_START;
		startRound.setStatus (GameConstant.ROUND_STATUS_ACTIVE);
		firstFlopRound.setStatus (GameConstant.ROUND_STATUS_PENDING);
		secondFlopRound.setStatus (GameConstant.ROUND_STATUS_PENDING);
		whoopAssRound.setStatus (GameConstant.ROUND_STATUS_PENDING);
		thirdRound.setStatus (GameConstant.ROUND_STATUS_PENDING);
	}

	public void startFirstFlopRound ()
	{
		currentRound = GameConstant.WA_ROUND_FIRST_FLOP;
		startRound.setStatus (GameConstant.ROUND_STATUS_FINISH);
		firstFlopRound.setStatus (GameConstant.ROUND_STATUS_ACTIVE);
		secondFlopRound.setStatus (GameConstant.ROUND_STATUS_PENDING);
		whoopAssRound.setStatus (GameConstant.ROUND_STATUS_PENDING);
		thirdRound.setStatus (GameConstant.ROUND_STATUS_PENDING);
	}

	public void startSecondFlopRound ()
	{
		currentRound = GameConstant.WA_ROUND_SECOND_FLOP;
		startRound.setStatus (GameConstant.ROUND_STATUS_FINISH);
		firstFlopRound.setStatus (GameConstant.ROUND_STATUS_FINISH);
		secondFlopRound.setStatus (GameConstant.ROUND_STATUS_ACTIVE);
		whoopAssRound.setStatus (GameConstant.ROUND_STATUS_PENDING);
		thirdRound.setStatus (GameConstant.ROUND_STATUS_PENDING);
	}

	public void startWhoopAssRound ()
	{
		currentRound = GameConstant.WA_ROUND_WHOOPASS;
		startRound.setStatus (GameConstant.ROUND_STATUS_FINISH);
		firstFlopRound.setStatus (GameConstant.ROUND_STATUS_FINISH);
		secondFlopRound.setStatus (GameConstant.ROUND_STATUS_FINISH);
		whoopAssRound.setStatus (GameConstant.ROUND_STATUS_ACTIVE);
		thirdRound.setStatus (GameConstant.ROUND_STATUS_PENDING);
	}

	public void startThirdRound ()
	{
		currentRound = GameConstant.WA_ROUND_THIRD_FLOP;
		startRound.setStatus (GameConstant.ROUND_STATUS_FINISH);
		firstFlopRound.setStatus (GameConstant.ROUND_STATUS_FINISH);
		secondFlopRound.setStatus (GameConstant.ROUND_STATUS_FINISH);
		whoopAssRound.setStatus (GameConstant.ROUND_STATUS_FINISH);
		thirdRound.setStatus (GameConstant.ROUND_STATUS_ACTIVE);
	}

	public RoundManager getStartRound (){
		return startRound;
	}

	public RoundManager getFirstFlopRound (){
		return firstFlopRound;
	}

	public RoundManager getSecondFlopRound (){
		return secondFlopRound;
	}

	public RoundManager getWhoopAssRound (){
		return whoopAssRound;
	}

	public RoundManager getThirdRound (){
		return thirdRound;
	}

	public int getPlayerTotalBetAmount (String name){
		PlayerBean player = playersManager.getPlayerFromName (name);
		int totalBetAmount = 0;
		totalBetAmount += startRound.getTotalPlayerBetAmount (player);
		totalBetAmount += firstFlopRound.getTotalPlayerBetAmount (player);
		totalBetAmount += secondFlopRound.getTotalPlayerBetAmount (player);
		totalBetAmount += thirdRound.getTotalPlayerBetAmount (player);
		return totalBetAmount;
	}

	public int getTotalTableAmount (){
		int totalBetAmount = 0;
		totalBetAmount += startRound.getTotalRoundBetAmount ();
		totalBetAmount += firstFlopRound.getTotalRoundBetAmount ();
		totalBetAmount += secondFlopRound.getTotalRoundBetAmount ();
		totalBetAmount += thirdRound.getTotalRoundBetAmount ();
		return totalBetAmount;
	}

	public int getTotalRaiseCounter (){
		int totalRaiseCounter = 0;
		totalRaiseCounter += startRound.getTotalRaiseCounter ();
		totalRaiseCounter += firstFlopRound.getTotalRaiseCounter ();
		totalRaiseCounter += secondFlopRound.getTotalRaiseCounter ();
		totalRaiseCounter += thirdRound.getTotalRaiseCounter ();
		return totalRaiseCounter;
	}

	public void addNewPlayerOnTable (int id, string playerName, int balance, string card1, string card2, string waCard,int gameStatus,int playerStatus){
		PlayerBean playerTmp = playersManager.getPlayerFromName (playerName);
		if (playerTmp == null) {
			PlayerBean player = new PlayerBean (
				                    gameObjectManager.getPlayerGameObject (playersManager.totalPlayerOnTable ()), 
				                    gameObjectManager.getPlayerChipsObject (playersManager.totalPlayerOnTable ()),
				                    id, 
				                    playerName, 
				                    balance, 
				                    card1, 
				                    card2, waCard);
			player.setBetAmount (0, 0);
			if (playerStatus == GameConstant.ACTION_FOLDED) {
				player.setFoldedPlayer (true);
			} else if (playerStatus == GameConstant.ACTION_WAITING_FOR_GAME) {
				player.setIsWatingForGamePlayer ();
				playersManager.closeActivePlayersCards ();			
			}
			playersManager.addPlayer (player);
		} else {
			playerTmp.setCardsAndBalance (balance, card1, card2, waCard);
		}

		if (playersManager.getAllPlayers ().Count < GameConstant.MIN_PLAYER_TO_START_GAME) {
			waitingForMinPlayer ();
		}
	}

	public void leavePlayerOnTable (string playerName){
		PlayerBean player = playersManager.getPlayerFromName (playerName);
		if (player != null) {
			playersManager.removePlayer (player);
		}
	}
	public int getBlindAmount(){
		return smallBlindAmount;
	}
	public void setBlindAmount(int blindAmount){
		this.smallBlindAmount = blindAmount;
	}
	public PlayersManager getPlayerManager(){
		return playersManager;
	}
	public void defineBlindPlayer (int blindAmount, string dealer, string sbPlayerName, string bbPlayerName){
		this.smallBlindAmount = blindAmount;
		playersManager.setDealerPlayer (dealer);
		playersManager.setBigBlindPlayer (bbPlayerName);
		playersManager.setSmallBlindPlayer (sbPlayerName);
		defaultCardsManager.setDealerPlayer (playersManager.getDealerPlayer ());
	}

	public void highLightTurnPlayer (string player){
		if (isRunningGame) {
			PlayerBean currentPlayer = playersManager.setCurrentTurnPlayer (player);
			if (appwarp.username.Equals (player)) {
				playerBalance = currentPlayer.getBalance ();
				RoundManager currentRoundManager = getCurrentRoundInfo ();
				if (currentPlayer.isWaitingForGame ()) {
					sendPlayerActionToServer (0, GameConstant.ACTION_WAITING_FOR_GAME);
				}else if (currentRound != GameConstant.WA_ROUND_START &&
				   currentRoundManager.getAllTurnRecords ().Count == 0 &&
				   !currentPlayer.getPlayerName ().Equals (playersManager.getSmallBlindPlayer ().getPlayerName ())) {
					sendPlayerActionToServer (0, GameConstant.ACTION_NO_TURN);
				} else if (currentRound == GameConstant.WA_ROUND_WHOOPASS) {
					if (currentPlayer.isFoldedPlayer ()) {
						sendPlayerActionToServer (0, GameConstant.ACTION_WA_NO);
						waPurchaseOption.SetActive (false);
					} else {
						// Check second flop round bet amount
						waCardAmt = secondFlopRound.getWACardAmount ();
						// Check first flop round bet amount if WA amount 0
						waCardAmt = waCardAmt == 0 ? firstFlopRound.getWACardAmount () : waCardAmt;
						// Second flop round bet amount is 0 then WA card is depend on Big blind amount
						waCardAmt = waCardAmt == 0 ? smallBlindAmount * 2 : waCardAmt;
						// If player have not enough balance then WA card is based on player balance
						PlayerBean loginPlayer = playersManager.getPlayerFromName (appwarp.username);
						if (loginPlayer.getBalance () < waCardAmt) {
							waCardAmt = loginPlayer.getBalance ();
						}
						if (currentPlayer.isAllInPlayer ()) {
							txtWATitle.text = "Buy WhoopAss Card for FREE.";
						} else {
							txtWATitle.text = "Buy WhoopAss Card for " + Utility.GetCurrencyPrefix() + waCardAmt;
						}
						waPurchaseOption.SetActive (true);

					}
				} else {
					if (currentPlayer.isFoldedPlayer ()) {
						sendPlayerActionToServer (0, GameConstant.ACTION_FOLDED);
					} else if (currentPlayer.isAllInPlayer ()) {
						sendPlayerActionToServer (0, GameConstant.ACTION_ALL_IN);
					} else {
						setEnableDisableOptions (true);
					}
				}
			} else {
				if (currentRound == GameConstant.WA_ROUND_WHOOPASS) {
					waPurchaseOption.SetActive (false);
				} else {
					playerBalance = 0;
					setEnableDisableOptions (false);
				}
			}
		}
	}

	public void gameFinished (){
		GAME_STATUS = GameConstant.FINISHED;
	}

	public IEnumerator manageWinnerPlayers(PlayerBean winnerPlayer){
		gameObjectManager.getTableChipSetPositionObject ().SetActive (true);
		gameObjectManager.getTableChipSetObject ().transform.position = gameObjectManager.getTableChipSetPositionObject ().transform.position;
		animationManager.MoveChipsObject ( gameObjectManager.getTableChipSetObject (),winnerPlayer.getChipObject ());
		yield return new WaitForSeconds (GameConstant.ANIM_WAITING_TIME);
		winnerPlayer.getChipObject ().transform.position = winnerPlayer.getChipPositionObject ().transform.position;
		gameObjectManager.getTableChipSetObject ().transform.position = gameObjectManager.getTableChipSetPositionObject ().transform.position;
		gameObjectManager.getTableChipSetPositionObject ().SetActive (false);
		gameObjectManager.getTableChipSetObject ().SetActive (false);
		StartCoroutine( blinkWinnerPlayer(winnerPlayer.getGameObject()));
	}

	public void manageWACardPotAmt (string winnerName, int totalBalance, int winningAmt){
		PlayerBean winnerPlayer = playersManager.findWinnerPlayerFromName (winnerName);
		StartCoroutine(manageWinnerPlayers (winnerPlayer));
		winnerPlayer.setPlayerBalance (totalBalance);
		txtLog.text = txtLog.text.ToString () + "\n\nWA pot winner is " + winnerName + "\nWinning Amount : " + winningAmt + "\n";
	}

	public void manageGameFinishAction (string winnerPlayerName, int rank, int winnerPlayerTotalBalance, int winningAmount, List<string> listBestCard){
		if (GAME_STATUS == GameConstant.FINISHED) {
			iTween.Stab(gameObject,iTween.Hash("audioclip",winnerAudio,"pitch",1));
			bool isAllAreFolded = playersManager.isAllPlayersAreFolded ();

			StartCoroutine (defaultCardsManager.openFirstFlopCards ());
			StartCoroutine (defaultCardsManager.openSecondFlopCards ());
			StartCoroutine (defaultCardsManager.openThirdCards ());

			resetDefaultCards (false);
			resetWinningPlayers (false);

			isRunningGame = false;
			setEnableDisableOptions (false);

			txtLog.text = txtLog.text.ToString () + "\nWinner is " + winnerPlayerName + "\nRank : " + getRankName ((HAND_RANK)rank) + "\nWinning Amount : " + winningAmount + "\n";

			PlayerBean winnerPlayer = playersManager.findWinnerPlayerFromName (winnerPlayerName);
			StartCoroutine(manageWinnerPlayers (winnerPlayer));
			winnerPlayer.setPlayerBalance (winnerPlayerTotalBalance);
			//reset cards
			if (!isAllAreFolded) {
				winnerPlayer.getRankPannel ().SetActive (true);
				winnerPlayer.getRankText ().text = getRankName ((HAND_RANK)rank) + " (" + Utility.GetCurrencyPrefix() + "" + winningAmount + ")";
				winnerPlayer.getRankText ().color = Color.white;
				playersManager.openAllPlayerCards ();
				foreach (string cardName in listBestCard) {
					GameObject cardGameObject = winnerPlayer.getCardObjectFromCardName (cardName);
					if (cardGameObject != null) {
//						handleAnim.GetInstanceCardAnimation ().MoveObjectInDirection (cardGameObject, 20, "y");	
						animationManager.WinningCardAnimation (cardGameObject);
					} else {
						cardGameObject = defaultCardsManager.getCardObjectFromCardName (cardName);				
						if (cardGameObject != null) {
//							handleAnim.GetInstanceCardAnimation ().MoveObjectInDirection (cardGameObject, 20, "y");	
							animationManager.WinningCardAnimation (cardGameObject);
						}
					}
				}
			}
			Invoke ("scrollDown", 1f);
		}
	}

	public IEnumerator blinkWinnerPlayer(GameObject gameObject){
		int i = 0;
		while (i < 3) {
			gameObject.SetActive(false);	
			yield return new WaitForSeconds(.5f);
			gameObject.SetActive(true);
			yield return new WaitForSeconds(.5f); 
			i++;
		}
	}
	void scrollDown (){
		logScrollbar.value = 0;
	}

	public void resetWinningPlayers (bool hideFlag){
		foreach (PlayerBean playerBean in playersManager.getAllPlayers()) {
			playerBean.resetPlayer ();
			if (hideFlag)
				playerBean.changeActiveStatusOfPlayerCards (false);
		}
	}

	public void removeAllPlayers (){
		List<String> listPlayersName = new List<String> ();
		foreach (PlayerBean playerBean in playersManager.getAllPlayers ()) {
			listPlayersName.Add (playerBean.getPlayerName ());
		}
		foreach (string name in listPlayersName) {
			leavePlayerOnTable (name);
		}
	}

	public void resetDefaultCards (bool hideFlag){
		StartCoroutine (defaultCardsManager.resetCards ());
		if (hideFlag)
			defaultCardsManager.closeAllCards ();
	}

	int totalBBPlayersTurn = 0;

	public void managePlayerMoveAction (string name, int betamount, int totalTableAmount, int totalPlayerBalance, int action){
		PlayerBean playerBean = playersManager.getPlayerFromName (name); 
		RoundManager currentRoundManger = getCurrentRoundInfo ();
		if (currentRound == GameConstant.WA_ROUND_START && playerBean.isBigBlindPlayer ()) {
			totalBBPlayersTurn++;
		}
			
		if (currentRoundManger.getRound () == GameConstant.WA_ROUND_WHOOPASS) {
			StartCoroutine( managePlayerWACardAction (action, playerBean));
		}
		txtLog.text = txtLog.text.ToString () + "\n" + playerBean.getPlayerName () + " >> Action >> " + getActionName (action) + " >> " + betamount;
		Invoke ("scrollDown", 1f);
		if (betamount > 0) {
			animationManager.MoveChipsObject (playerBean.getChipObject (), gameObjectManager.getTableChipSetObject ());
//			yield return new WaitForSeconds (GameConstant.ANIM_WAITING_TIME);
			playerBean.getChipObject ().transform.position = playerBean.getChipPositionObject ().transform.position;
		}
		TurnManager turnManager = new TurnManager (playerBean, action, betamount);
		currentRoundManger.addTurnRecord (turnManager);
		int totalBetAmtInRound = currentRoundManger.getTotalPlayerBetAmount (playerBean);
		playerBean.setBetAmount (betamount, totalBetAmtInRound);
		playersManager.setLastPlayerAction (name, action, betamount, totalBetAmtInRound, totalPlayerBalance);
		if (playerBean.getBalance () <= 0 && !playerBean.isAllInPlayer ()) {
			playerBean.setPlayerBalance (0); // if player balance is not enough
			playerBean.setAllInStatus (true);
		}
		setTotalBetTableAmount (totalTableAmount);
	}

	public IEnumerator managePlayerWACardAction(int action, PlayerBean playerBean){
		PlayerBean dealerPlayer = playersManager.getDealerPlayer();
		GameObject src = dealerPlayer.getCardDeskObject ();
		GameObject cardDeskPosition = dealerPlayer.getCardDeskPositionObject ();
		cardDeskPosition.SetActive (true);
		src.SetActive (true);
		src.transform.position = cardDeskPosition.transform.position;
		if (action == GameConstant.ACTION_WA_UP) {
			animationManager.MoveCardsObject (src, playerBean.getWACardUPObject());
			yield return new WaitForSeconds (GameConstant.ANIM_WAITING_TIME);
			playerBean.upWACardBuy ();
		} else if (action == GameConstant.ACTION_WA_DOWN) {
			animationManager.MoveCardsObject (src, playerBean.getWACardDownObject());
			yield return new WaitForSeconds (GameConstant.ANIM_WAITING_TIME);
			playerBean.downWACardBuy ();
		}
	}
	public void setSmallBlindDefaultBet (){
		PlayerBean smallBlind = playersManager.getSmallBlindPlayer ();
		managePlayerMoveAction (smallBlind.getPlayerName (), smallBlindAmount, smallBlindAmount, (smallBlind.getBalance () - smallBlindAmount), GameConstant.ACTION_BET);
	}

	public void setBigBlindDefaultBet (){
		PlayerBean bigBlind = playersManager.getBigBlindPlayer ();
		int betAmount = smallBlindAmount * 2;
		managePlayerMoveAction (bigBlind.getPlayerName (), betAmount, smallBlindAmount + betAmount, bigBlind.getBalance () - betAmount, GameConstant.ACTION_BET);
	}



	public void moveToNextRound (int round, int totalBetAmount){
		txtLog.text = txtLog.text.ToString () + "\n << " + getRoundName (round) + " >>round Started !!";
		Invoke ("scrollDown", 1f);
		//		StartCoroutine (CollectAllBetCoinsTOCenterOfTable ());
		switch (round) {
		case GameConstant.WA_ROUND_START:
			startStartRound ();
			break;
		case GameConstant.WA_ROUND_FIRST_FLOP:
			startFirstFlopRound ();
			StartCoroutine (defaultCardsManager.openFirstFlopCards ());
			break;
		case GameConstant.WA_ROUND_SECOND_FLOP:
			startSecondFlopRound ();
			StartCoroutine (defaultCardsManager.openSecondFlopCards ());
			break;
		case GameConstant.WA_ROUND_WHOOPASS:
			StartCoroutine (defaultCardsManager.openFirstFlopCards ());
			StartCoroutine (defaultCardsManager.openSecondFlopCards ());
			startWhoopAssRound ();
			break;
		case GameConstant.WA_ROUND_THIRD_FLOP:
			playersManager.openWAUpcardsIfBuyed ();
			startThirdRound ();
			StartCoroutine (defaultCardsManager.openThirdCards ());
			break;
		default:
			break;
		}
		//currentRoundTurns.Clear ();
		playersManager.resetPlayersBetAmount ();
		setTotalBetTableAmount (totalBetAmount);
	}

	public void setTotalBetTableAmount (int amount){
		gameObjectManager.getTotalTableBetAmountText ().text = Utility.GetCurrencyPrefix() + amount;
	}

	public void setEnableDisableOptions (bool flag){
//		callFlag = false;
//		raiseFlag = false;
//		checkFlag = false;
//		betFlag = false;
//		if (!flag) {
//			gameObjectManager.getFoldButton ().interactable = flag;
//			gameObjectManager.getCheckButton ().interactable = flag;
//			gameObjectManager.getRaiseButton ().interactable = flag;
//			gameObjectManager.getCallButton ().interactable = flag;
//			gameObjectManager.getBetButton ().interactable = flag;
//			gameObjectManager.getAllInButton ().interactable = flag;
//		} else {
//			gameObjectManager.getFoldButton ().interactable = true;
//			RoundManager currentRoundManager = getCurrentRoundInfo ();
//			TurnManager lastActivePlayerTurn = currentRoundManager.getLastActivePlayerTurn ();
//			gameObjectManager.getBetAmountText ().text = GameConstant.CURRENCY + "0";
//
//			if (lastActivePlayerTurn != null) {
//				lastPlayerBetAmt = currentRoundManager.getTotalPlayerBetAmount (lastActivePlayerTurn.getPlayer ());
//				gameObjectManager.getBetAmountText ().text = GameConstant.CURRENCY + lastPlayerBetAmt;
//			} else {
//				lastPlayerBetAmt = 0;
//				if (appwarp.isLimitGame) {
//					if (currentRound == GameConstant.WA_ROUND_START || currentRound == GameConstant.WA_ROUND_FIRST_FLOP) {
//						lastPlayerBetAmt = smallBlindAmount;
//					} else if (currentRound == GameConstant.WA_ROUND_SECOND_FLOP || currentRound == GameConstant.WA_ROUND_THIRD_FLOP) {
//						lastPlayerBetAmt = smallBlindAmount * 2;
//					}
//				}
//			}
//			int totalRaiseOnGame = currentRoundManager.getTotalRaiseCounter ();
//			if (currentRound == GameConstant.WA_ROUND_START) {
//				gameObjectManager.getCallButton ().interactable = true;
//				gameObjectManager.getRaiseButton ().interactable = true;
//				SliderChips.enabled = true;
//			} else if (currentRoundManager.getAllTurnRecords ().Count == 0 || lastActivePlayerTurn == null) {
//				gameObjectManager.getCheckButton ().interactable = true;
//				gameObjectManager.getBetButton ().interactable = true;
//				if (appwarp.isLimitGame) {
//					SliderChips.enabled = false;
//				} else {
//					SliderChips.enabled = true;
//				}
//			} else {
//				gameObjectManager.getCallButton ().interactable = true;
//				gameObjectManager.getRaiseButton ().interactable = true;
//				SliderChips.enabled = true;
//			}
//			int loggedPlrBalance = playersManager.getPlayerFromName (appwarp.username).getBalance ();
//			int lastBetAmount = currentRoundManager.getTotalPlayerBetAmount (lastActivePlayerTurn.getPlayer ());
//			int myTotalBetAmount = currentRoundManager.getTotalPlayerBetAmount (playersManager.getPlayerFromName (appwarp.username));
//			int pendingBetAmount = lastBetAmount - myTotalBetAmount;
//
//			if (currentRound == GameConstant.WA_ROUND_START && totalBBPlayersTurn == 1 && pendingBetAmount == 0) {
//				gameObjectManager.getCallButton ().interactable = false;
//				gameObjectManager.getCheckButton ().interactable = true;
//				gameObjectManager.getRaiseButton ().interactable = true;
//				SliderChips.enabled = true;
//			}
//			if (lastActivePlayerTurn != null && pendingBetAmount > loggedPlrBalance) {
//				gameObjectManager.getCallButton ().interactable = false;
//				gameObjectManager.getRaiseButton ().interactable = false;
//				gameObjectManager.getAllInButton ().interactable = true;
//				gameObjectManager.getAllInButton ().gameObject.SetActive (true);
//				lastPlayerBetAmt = loggedPlrBalance;
//			} else {
//				gameObjectManager.getAllInButton ().gameObject.SetActive (false);
//			}
//
//			if (totalRaiseOnGame >= GameConstant.MAXIMUM_RAISE_ON_GAME) {
//				gameObjectManager.getRaiseButton ().interactable = false;
//			}
//			SliderChips.value = (float)lastPlayerBetAmt;
//			raiseValue = lastPlayerBetAmt;
//			gameObjectManager.getBetAmountText ().text = GameConstant.CURRENCY + raiseValue;
//		}
	}

	public void onFoldButtonSelected (){
//		gameObjectManager.getFoldButton ().interactable = false;
//		sendPlayerActionToServer (0, GameConstant.ACTION_FOLD);
	}

	public void onCheckButtonSelected (){
//		gameObjectManager.getCheckButton ().interactable = false;
//		sendPlayerActionToServer (0, GameConstant.ACTION_CHECK);
	}

	public void onRaiseButtonSelected (){
		RoundManager currentRoundManager = getCurrentRoundInfo ();
		TurnManager lastActiverPlayerTurn = currentRoundManager.getLastActivePlayerTurn ();
		int lastPlayerBetAmt = currentRoundManager.getTotalPlayerBetAmount (lastActiverPlayerTurn.getPlayer ());
		int totalPlrPrvBetAmt = currentRoundManager.getTotalPlayerBetAmount (playersManager.getPlayerFromName (appwarp.username));
		if ((raiseValue == 0 || raiseValue == lastPlayerBetAmt) && appwarp.isLimitGame) {
			int maxRaiseValue = 0;
			int totalPendingBetAmt = lastPlayerBetAmt - totalPlrPrvBetAmt;
			if (currentRound == GameConstant.WA_ROUND_START || currentRound == GameConstant.WA_ROUND_FIRST_FLOP) {
				maxRaiseValue = totalPendingBetAmt + smallBlindAmount;
			} else if (currentRound == GameConstant.WA_ROUND_SECOND_FLOP || currentRound == GameConstant.WA_ROUND_THIRD_FLOP) {
				maxRaiseValue = totalPendingBetAmt + smallBlindAmount * 2;
			}
			SliderChips.value = maxRaiseValue;
			raiseValue = maxRaiseValue;
			gameObjectManager.getBetAmountText ().text = GameConstant.CURRENCY + raiseValue;
		} else {
			raiseValue = raiseValue - totalPlrPrvBetAmt;
		}

		if (raiseValue != 0 && (raiseValue + totalPlrPrvBetAmt) > lastPlayerBetAmt) {
//			gameObjectManager.getRaiseButton ().interactable = false;
//			sendPlayerActionToServer (raiseValue, GameConstant.ACTION_RAISE);
		}
	}

	public void onCallButtonSelected ()	{
		raiseValue = (int)0;
		SliderChips.value = (float)raiseValue;
		gameObjectManager.getBetAmountText ().text = GameConstant.CURRENCY + raiseValue;
		RoundManager currentRoundManager = getCurrentRoundInfo ();
		TurnManager lastActiverPlayerTurn = currentRoundManager.getLastActivePlayerTurn ();
		int lastBetAmount = currentRoundManager.getTotalPlayerBetAmount (lastActiverPlayerTurn.getPlayer ());
		int myTotalBetAmount = currentRoundManager.getTotalPlayerBetAmount (playersManager.getPlayerFromName (appwarp.username));
		int pendingBetAmount = lastBetAmount - myTotalBetAmount;
		if (pendingBetAmount > 0) {
//			gameObjectManager.getCallButton ().interactable = false;
//			sendPlayerActionToServer (pendingBetAmount, GameConstant.ACTION_CALL);
		} else {
			DEBUG.Log ("Bug : Bet Amt : " + pendingBetAmount);
			DEBUG.Log ("Call >> Last Bet : " + lastBetAmount + " >> My Total Bet : " + myTotalBetAmount + " >> Pending Bet " + pendingBetAmount);
		}
	}

	public void onBetButtonSelected (){
		if (raiseValue != 0) {
//			gameObjectManager.getBetButton ().interactable = false;
//			sendPlayerActionToServer (raiseValue, GameConstant.ACTION_BET);
		}
	}

	public void onAllInButtonSelected (){
		raiseValue = playersManager.getPlayerFromName (appwarp.username).getBalance ();
//		gameObjectManager.getAllInButton ().interactable = false;
//		sendPlayerActionToServer (raiseValue, GameConstant.ACTION_ALL_IN);
	}

	private void sendPlayerActionToServer (int betAmount, int action){
		iTween.Stab(gameObject,iTween.Hash("audioclip",buttonAudio,"pitch",1));
		raiseValue = 0;
		lastPlayerBetAmt = 0;
		SliderChips.value = 0f;
		SliderChips.enabled = false;
		gameObjectManager.getBetAmountText ().text = GameConstant.CURRENCY + raiseValue;
		PlayerBean loginPlayer = playersManager.getPlayerFromName (appwarp.username);

		JSON_Object requestJson = new JSON_Object ();
		try {
			requestJson.put (GameConstant.TAG_PLAYER_NAME, appwarp.username);
			requestJson.put (GameConstant.TAG_BET_AMOUNT, betAmount);
			requestJson.putOpt (GameConstant.TAG_ACTION, action);
			WarpClient.GetInstance ().sendMove (GameConstant.REQUEST_FOR_ACTION + requestJson.ToString ());	
		} catch (Exception e) {
			Debug.Log ("GameManager : " + e.ToString ());
		}	
		setEnableDisableOptions (false);
	}

	public void OnSliderMove ()
	{
		if (SliderChips != null) {
			if (playerBalance != 0 && SliderChips.maxValue != playerBalance) {
				SliderChips.maxValue = playerBalance;
			}

			if (SliderChips.value <= lastPlayerBetAmt) {
				SliderChips.value = lastPlayerBetAmt;
			}
			if (appwarp.isLimitGame) {
				RoundManager currentRoundManager = getCurrentRoundInfo ();
				int totalPlrPrvBetAmt = currentRoundManager.getTotalPlayerBetAmount (playersManager.getPlayerFromName (appwarp.username));
				int maxRaiseValue = 0;
				int totalPendingBetAmt = lastPlayerBetAmt - totalPlrPrvBetAmt;
				if (currentRound == GameConstant.WA_ROUND_START || currentRound == GameConstant.WA_ROUND_FIRST_FLOP) {
					maxRaiseValue = totalPendingBetAmt + smallBlindAmount;
				} else if (currentRound == GameConstant.WA_ROUND_SECOND_FLOP || currentRound == GameConstant.WA_ROUND_THIRD_FLOP) {
					maxRaiseValue = totalPendingBetAmt + smallBlindAmount * 2;
				}
				maxRaiseValue = maxRaiseValue + totalPlrPrvBetAmt;
				if (SliderChips.value >= maxRaiseValue) {
					SliderChips.value = maxRaiseValue;
				}
			}
			manageOptionsFromSlider ();
			raiseValue = (int)SliderChips.value;
			gameObjectManager.getBetAmountText ().text = GameConstant.CURRENCY + raiseValue;
		}
	}

	bool callFlag, raiseFlag, checkFlag, betFlag = false;

	public void manageOptionsFromSlider (){
//		if (SliderChips.value == SliderChips.maxValue) {
//			gameObjectManager.getAllInButton ().interactable = true;
//			gameObjectManager.getAllInButton ().gameObject.SetActive (true);
//			if (gameObjectManager.getCallButton ().IsInteractable ()) {
//				gameObjectManager.getCallButton ().interactable = false;
//				callFlag = true;
//			}
//			if (gameObjectManager.getRaiseButton ().IsInteractable ()) {
//				gameObjectManager.getRaiseButton ().interactable = false;
//				raiseFlag = true;
//			}
//			if (gameObjectManager.getCheckButton ().IsInteractable ()) {
//				gameObjectManager.getCheckButton ().interactable = false;
//				checkFlag = true;
//			}
//			if (gameObjectManager.getBetButton ().IsInteractable ()) {
//				gameObjectManager.getBetButton ().interactable = false;
//				betFlag = true;
//			}
//		} else {
//			gameObjectManager.getAllInButton ().interactable = false;
//			gameObjectManager.getAllInButton ().gameObject.SetActive (false);
//			if (callFlag)
//				gameObjectManager.getCallButton ().interactable = true;
//			if (raiseFlag)
//				gameObjectManager.getRaiseButton ().interactable = true;
//			if (checkFlag)
//				gameObjectManager.getCheckButton ().interactable = true;
//			if (betFlag)
//				gameObjectManager.getBetButton ().interactable = true;
//		}
	}

	IEnumerator CollectAllBetCoinsTOCenterOfTable (){
		GameObject src = null;	
		GameObject dest = null;
		
		int i = 0;
		while (i < playersManager.getAllPlayers ().Count) {
			PlayerBean player = playersManager.getAllPlayers () [i];
			if (!player.isFoldedPlayer ()) {
				src = player.getBetChipsObject ().gameObject;
				dest = gameManagerGameObject.transform.Find ("TableChips/ChipsSet1").gameObject;
				movingCoinObject.GetComponent<Image> ().sprite = src.GetComponent<Image> ().sprite;
				movingCoinObject.GetComponent<RectTransform> ().position = src.GetComponent<RectTransform> ().position;
				animationManager.MoveObject (movingCoinObject, dest);
				src.SetActive (false);
				yield return new WaitForSeconds (1f);
			}
			yield return new WaitForSeconds (0.0f);
			i++;
		}		                        
	}


	public void onWADown ()	{
		sendPlayerActionToServer (waCardAmt, GameConstant.ACTION_WA_DOWN);
		waPurchaseOption.SetActive (false);
	}

	public void onWAUp (){
		sendPlayerActionToServer (waCardAmt, GameConstant.ACTION_WA_UP);
		waPurchaseOption.SetActive (false);
	}

	public void onWANo (){	
		sendPlayerActionToServer (0, GameConstant.ACTION_WA_NO);
		waPurchaseOption.SetActive (false);
	}

	public void restartGame (){
		if (GAME_STATUS == GameConstant.FINISHED)
			StartCoroutine (startGameCounter ());
	}

	public void waitingForMinPlayer (){
		StartCoroutine (watingForPlayer ());
	}

	public IEnumerator startGameCounter (){
		int cntr = 0;
		resetDefaultCards (true);
		resetWinningPlayers (true);
		WarpClient.GetInstance ().SendChat (GameConstant.REQUEST_FOR_RESTART_GAME);
		gameObjectManager.getRestartPannel ().SetActive (true);
		while (!isRunningGame) {
			string dot = getDotFromCntr (cntr++);
			gameObjectManager.getRestartText ().text = "Waiting for all players restart response" + dot;
			if (cntr > 3)
				cntr = -1;
			yield return new WaitForSeconds (1);
		}
		gameObjectManager.getRestartPannel ().SetActive (false);
	}

	public IEnumerator watingForPlayer (){
		gameObjectManager.getRestartPannel ().SetActive (true);
		int cntr = 0;
		while (playersManager.getAllPlayers ().Count < GameConstant.MIN_PLAYER_TO_START_GAME) {
			string dot = getDotFromCntr (cntr++);
			gameObjectManager.getRestartText ().text = "Waiting for players" + dot;
			if (cntr > 3)
				cntr = -1;
			yield return new WaitForSeconds (1);
		}
		gameObjectManager.getRestartPannel ().SetActive (false);
	}

	public IEnumerator hideWinnerRankPannel (PlayerBean winnerPlayer)
	{
		yield return new WaitForSeconds (2f);
		winnerPlayer.getRankPannel ().SetActive (false);
	}

	public string getDotFromCntr (int cntr)
	{
		string dot = "";
		switch (cntr) {
		case 0:
			dot = ".  ";
			break;
		case 1:
			dot = ".. ";
			break;
		case 2:
			dot = "...";
			break;
		default:
			dot = "...";

			break;
		}
		return dot;
	}

	public string getRankName (HAND_RANK rank)
	{
	
		switch (rank) {
		case HAND_RANK.ROYAL_FLUSH:
			return "Royal Flush";
		case HAND_RANK.STRAIGHT_FLUSH:
			return "Straight Flush";
		case HAND_RANK.FOUR_OF_A_KIND:
			return "Four Of A Kind";
		case HAND_RANK.FULL_HOUSE:
			return "Full House";
		case HAND_RANK.FLUSH:
			return "Flush";
		case HAND_RANK.STRAIGHT:
			return "Straight";
		case HAND_RANK.THREE_OF_A_KIND:
			return "Three Of A Kind";
		case HAND_RANK.TWO_PAIR:
			return "Two Pair";
		case HAND_RANK.PAIR:
			return "Pair";
		case HAND_RANK.HIGH_CARD:
			return "High Card";
		default:
			return "";
		}
	}

	public Slider SliderChips;
	int playerBalance = 0;
	int raiseValue = 0;
	int lastPlayerBetAmt = 0;

	public enum HAND_RANK
	{
		ROYAL_FLUSH,
		STRAIGHT_FLUSH,
		FOUR_OF_A_KIND,
		FULL_HOUSE,
		FLUSH,
		STRAIGHT,
		THREE_OF_A_KIND,
		TWO_PAIR,
		PAIR,
		HIGH_CARD
	}
	public string getActionName (int memberValue){
		switch (memberValue) {
		case GameConstant.ACTION_CALL: 
			return "Call";

		case GameConstant.ACTION_FOLD: 
			return "Fold";

		case GameConstant.ACTION_BET:
			return "Bet";

		case GameConstant.ACTION_CHECK: 
			return "Check";

		case GameConstant.ACTION_RAISE: 
			return "Raise";

		case GameConstant.ACTION_WA_DOWN: 
			return "Down WA Card";

		case GameConstant.ACTION_WA_UP: 
			return "UP WA Card";

		case GameConstant.ACTION_WA_NO: 
			return "Pass WA Card";

		case GameConstant.ACTION_FOLDED: 
			return "Folded";

		case GameConstant.ACTION_PENDING: 
			return "action pending";

		case GameConstant.ACTION_TIMEOUT: 
			return "Time Out";

		case GameConstant.ACTION_ALL_IN: 
			return "All In";

		case GameConstant.ACTION_DEALER: 
			return "dealer";

		}
		return "";
	}

	public string getRoundName (int roundValue){
		switch (roundValue) {
		case GameConstant.WA_ROUND_START: 
			return "WA_Start";

		case GameConstant.WA_ROUND_FIRST_FLOP: 
			return "WA_First_Flop";

		case GameConstant.WA_ROUND_SECOND_FLOP:
			return "WA_SECOND_FLOP";

		case GameConstant.WA_ROUND_THIRD_FLOP: 
			return "WA_THIRD_FLOP";

		case GameConstant.WA_ROUND_WHOOPASS: 
			return "WA Pass";

		}
		return "";
	}
}

