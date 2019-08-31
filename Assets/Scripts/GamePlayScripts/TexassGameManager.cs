using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;

public class TexassGameManager : MonoBehaviour
{
	public Scrollbar logScrollbar;
	public Text txtLog;
	PlayersManager playersManager;
	TexassDefaultCardsManager defaultCardsManager;
	public GameObject gameManagerGameObject;
	private int currentRound = GameConstant.TEXASS_ROUND_PREFLOP;

	RoundManager preflopRound;
	RoundManager flopRound;
	RoundManager turnRound;
	RoundManager riverRound;
	GameObjectManager gameObjectManager;
	bool isRunningGame = true;
	public bool isBreakTime = false;
//	private HandleAnimations handleAnim;
	// Use this for initialization
	public GameObject movingCoinObject;
	public GameObject movingCardObject;
	public GameObject movingCardObjectTmp;
	private AnimationsManager animationManager;
	int smallBlindAmount = 0;
	private int GAME_STATUS;
	public Text txtWATitle;
	public AudioClip cardAudio;
	public AudioClip chipAudio;
	public AudioClip buttonAudio;
	public AudioClip winnerAudio;

	public TexassGameManager ()
	{
	}
	// Use this for initialization
	void Start ()
	{
		animationManager = new AnimationsManager (cardAudio,chipAudio);
		playersManager = new PlayersManager ();
		gameObjectManager = new GameObjectManager (gameManagerGameObject);
		defaultCardsManager = new TexassDefaultCardsManager (animationManager,gameObjectManager.getDefaultCardObject ());
//		handleAnim = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<HandleAnimations> ();
		initGamePlay ();
	}

	public void initGamePlay ()
	{
		isRunningGame = true;
		isBreakTime = false;
		GAME_STATUS = GameConstant.STOPPED;
		raiseValue = (int)0;
		totalBBPlayersTurn=0;
		SliderChips.value = raiseValue;
		gameObjectManager.getBetAmountText ().text = GameConstant.CURRENCY + raiseValue;
		resetDefaultCards (true);
		//		resetWinningPlayers (true);
		removeAllPlayers ();
		setTotalBetTableAmount (0);
		preflopRound = new RoundManager (GameConstant.TEXASS_ROUND_PREFLOP);
		flopRound = new RoundManager (GameConstant.TEXASS_ROUND_FLOP);
		turnRound = new RoundManager (GameConstant.TEXASS_ROUND_TURN);
		riverRound = new RoundManager (GameConstant.TEXASS_ROUND_RIVER);
		setEnableDisableOptions (false);
//		startPreFlopRound ();
	}

	IEnumerator moveObject ()
	{
		iTween.MoveTo (movingCardObject, iTween.Hash ("position", movingCardObjectTmp.transform.position, "easetype", iTween.EaseType.easeInOutBack, "time", 0.5f));
		yield return new WaitForSeconds (0.1f);
	}

	void scrollDown(){
		logScrollbar.value = 0;
	}

	public void resetWinningPlayers (bool hideFlag)
	{
		foreach (PlayerBean playerBean in playersManager.getAllPlayers()) {
			playerBean.resetPlayer ();
			if (hideFlag)
				playerBean.changeActiveStatusOfPlayerCards (false);
		}
	}

	// Update is called once per frame
	void Update ()
	{
//		if (canReBuy && appwarp.GAME_TYPE == GameConstant.GAME_TYPE_TOURNAMENT_REGULAR
//		    && playersManager.getLoggedPlayer () != null
//		    && playersManager.getLoggedPlayer ().getBalance () <= 0) {
//			gameObjectManager.getReBuyButton ().gameObject.SetActive (true);
//			gameObjectManager.getReBuyButton ().interactable = true;
//		} else {
//			gameObjectManager.getReBuyButton ().gameObject.SetActive (false);
//		}
//		if (playersManager != null && playersManager.totalPlayerOnTable () != 0) {
//			foreach (PlayerBean player in playersManager.getAllPlayers()) {
//				player.initTurnAnimation ();
//			}
//		} else {
//			return;
//		}
	}

	public void setDefaultTableCards (string flop1, string flop2, string flop3, string turn, string river)
	{
		defaultCardsManager.setDefaultCards (flop1, flop2, flop3, turn, river);
	}

	public void resetDefaultCards (bool hideFlag)
	{
		StartCoroutine (defaultCardsManager.resetCards ());
		if (hideFlag)
			defaultCardsManager.closeAllCards ();
	}

	public void removeAllPlayers ()
	{
		List<String> listPlayersName = new List<String> ();
		foreach (PlayerBean playerBean in playersManager.getAllPlayers ()) {
			listPlayersName.Add (playerBean.getPlayerName ());
		}
		foreach (string name in listPlayersName) {
			leavePlayerOnTable (name);
		}
	}

	public RoundManager getCurrentRoundInfo ()
	{
		if (preflopRound.getStatus () == GameConstant.ROUND_STATUS_ACTIVE) {
			return preflopRound;
		} else if (flopRound.getStatus () == GameConstant.ROUND_STATUS_ACTIVE) {
			return flopRound;
		} else if (turnRound.getStatus () == GameConstant.ROUND_STATUS_ACTIVE) {
			return turnRound;
		} else if (riverRound.getStatus () == GameConstant.ROUND_STATUS_ACTIVE) {
			return riverRound;
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
//			DEBUG.Log ("Name : "+ player.getPlayerName()+" >> "+player.isWaitingForGame()+" >> "+ player.isFoldedPlayer());
			if (!player.isWaitingForGame ()) {
//				DEBUG.Log ("Name : "+ player.getPlayerName()+" >< ");
				GameObject card1Obj = player.getCard1Object ();
				GameObject card2Obj = player.getCard2Object ();
				src.transform.position = cardDeskPosition.transform.position;
				animationManager.MoveCardsObject (src, card1Obj);
				yield return new WaitForSeconds (GameConstant.ANIM_CARD_TIME);
				card1Obj.SetActive (true);

				src.transform.position = cardDeskPosition.transform.position;
				animationManager.MoveCardsObject (src, card2Obj);
				yield return new WaitForSeconds (GameConstant.ANIM_CARD_TIME);
				card2Obj.SetActive (true);
			} else {
				DEBUG.Log ("Name : "+ player.getPlayerName());
			}
			i++;
		}
		dealerPlayer.getCardDeskObject ().SetActive (false);
		cardDeskPosition.SetActive (false);
		src.transform.position = cardDeskPosition.transform.position;

	}

	IEnumerator MoveCardObjectToPlayer (GameObject src, GameObject dest)
	{
		movingCardObject.SetActive (true);
		movingCardObject.GetComponent<Image> ().sprite = src.GetComponent<Image> ().sprite;
		movingCardObject.GetComponent<RectTransform> ().position = src.GetComponent<RectTransform> ().position;
//		handleAnim.GetInstanceCardAnimation ().MoveObject (movingCardObject, dest);
		animationManager.MoveObject(movingCardObject, dest);
		//	src.SetActive (false);
		yield return new WaitForSeconds (0.2f);
		movingCardObject.SetActive(false);
	}

	public void distributeCards ()
	{
//		DEBUG.Log ("Distribule Cards");
		startPreFlopRound();
		manageBlindPlayerTurn ();

		StartCoroutine (cardAnimation ());
	}

	public void manageBlindPlayerTurn ()
	{
		setSmallBlindDefaultBet ();
		setBigBlindDefaultBet ();
	}

	public void setSmallBlindDefaultBet ()
	{
		PlayerBean smallBlind = playersManager.getSmallBlindPlayer ();
//		StartCoroutine( managePlayerMoveAction (smallBlind.getPlayerName (), smallBlindAmount, smallBlindAmount, (smallBlind.getBalance () - smallBlindAmount), GameConstant.ACTION_BET));
		 managePlayerMoveAction (smallBlind.getPlayerName (), smallBlindAmount, smallBlindAmount, (smallBlind.getBalance () - smallBlindAmount), GameConstant.ACTION_BET);
	}

	public void setBigBlindDefaultBet ()
	{
		PlayerBean bigBlind = playersManager.getBigBlindPlayer ();
		int betAmount = smallBlindAmount * 2;
//		StartCoroutine( managePlayerMoveAction (bigBlind.getPlayerName (), betAmount, smallBlindAmount + betAmount, bigBlind.getBalance () - betAmount, GameConstant.ACTION_BET));
		managePlayerMoveAction (bigBlind.getPlayerName (), betAmount, smallBlindAmount + betAmount, bigBlind.getBalance () - betAmount, GameConstant.ACTION_BET);
	}


	/**
	 * Start pre flop round and set other round status as a pending
	 */
	public void startPreFlopRound ()
	{
		GAME_STATUS = GameConstant.RUNNING;
		currentRound = GameConstant.TEXASS_ROUND_PREFLOP;
		// gamePlay.setCurrentRoundIndex(ROUND_PREFLOP);
		preflopRound.setStatus (GameConstant.ROUND_STATUS_ACTIVE);
		flopRound.setStatus (GameConstant.ROUND_STATUS_PENDING);
		turnRound.setStatus (GameConstant.ROUND_STATUS_PENDING);
		riverRound.setStatus (GameConstant.ROUND_STATUS_PENDING);
	}

	public void startFlopRound ()
	{
		currentRound = GameConstant.TEXASS_ROUND_FLOP;
		// gamePlay.setCurrentRoundIndex(ROUND_FLOP);
		preflopRound.setStatus (GameConstant.ROUND_STATUS_FINISH);
		flopRound.setStatus (GameConstant.ROUND_STATUS_ACTIVE);
		turnRound.setStatus (GameConstant.ROUND_STATUS_PENDING);
		riverRound.setStatus (GameConstant.ROUND_STATUS_PENDING);
	}

	public void startTurnRound ()
	{
		currentRound = GameConstant.TEXASS_ROUND_TURN;
		// gamePlay.setCurrentRoundIndex(ROUND_TURN);
		preflopRound.setStatus (GameConstant.ROUND_STATUS_FINISH);
		flopRound.setStatus (GameConstant.ROUND_STATUS_FINISH);
		turnRound.setStatus (GameConstant.ROUND_STATUS_ACTIVE);
		riverRound.setStatus (GameConstant.ROUND_STATUS_PENDING);
	}

	public void startRiverRound ()
	{
		currentRound = GameConstant.TEXASS_ROUND_RIVER;
		// gamePlay.setCurrentRoundIndex(ROUND_RIVER);
		preflopRound.setStatus (GameConstant.ROUND_STATUS_FINISH);
		flopRound.setStatus (GameConstant.ROUND_STATUS_FINISH);
		turnRound.setStatus (GameConstant.ROUND_STATUS_FINISH);
		riverRound.setStatus (GameConstant.ROUND_STATUS_ACTIVE);
	}

	public RoundManager getPreflopRound ()
	{
		return preflopRound;
	}

	public RoundManager getFlopRound ()
	{
		return flopRound;
	}

	public RoundManager getTurnRound ()
	{
		return turnRound;
	}

	public RoundManager getRiverRound ()
	{
		return riverRound;
	}

	public int getPlayerTotalBetAmount (String name)
	{
		PlayerBean player = playersManager.getPlayerFromName (name);
		int totalBetAmount = 0;
		totalBetAmount += preflopRound.getTotalPlayerBetAmount (player);
		totalBetAmount += flopRound.getTotalPlayerBetAmount (player);
		totalBetAmount += turnRound.getTotalPlayerBetAmount (player);
		totalBetAmount += riverRound.getTotalPlayerBetAmount (player);
		
		return totalBetAmount;
	}

	public int getTotalTableAmount ()
	{
		int totalBetAmount = 0;
		totalBetAmount += preflopRound.getTotalRoundBetAmount ();
		totalBetAmount += flopRound.getTotalRoundBetAmount ();
		totalBetAmount += turnRound.getTotalRoundBetAmount ();
		totalBetAmount += riverRound.getTotalRoundBetAmount ();
		return totalBetAmount;
	}

	public int getTotalRaiseCounter ()
	{
		int totalRaiseCounter = 0;
		totalRaiseCounter += preflopRound.getTotalRaiseCounter ();
		totalRaiseCounter += flopRound.getTotalRaiseCounter ();
		totalRaiseCounter += turnRound.getTotalRaiseCounter ();
		totalRaiseCounter += riverRound.getTotalRaiseCounter ();
		return totalRaiseCounter;
	}

	public void addNewPlayerOnTable (int id, string playerName, int balance, string card1, string card2,int gameStatus,int playerStatus,int currentRound)
	{
		//	playerName = "Player "+playersManager.totalPlayerOnTable ();
		PlayerBean playerTmp = playersManager.getPlayerFromName (playerName);
		if (playerTmp == null) {
			GAME_STATUS = gameStatus;
			PlayerBean player = new PlayerBean (
				                    gameObjectManager.getPlayerGameObject (playersManager.totalPlayerOnTable ()), 
				                    gameObjectManager.getPlayerChipsObject (playersManager.totalPlayerOnTable ()),
				                    id, 
				                    playerName, 
				                    balance, 
				                    card1, 
				                    card2, null);
			player.setBetAmount (0, 0);
			if (playerStatus == GameConstant.ACTION_FOLDED) {
				player.setFoldedPlayer (true);
			} else if (playerStatus == GameConstant.ACTION_WAITING_FOR_GAME) {
				player.setIsWatingForGamePlayer ();
				playersManager.closeActivePlayersCards ();			
			}
			if (currentRound > GameConstant.TEXASS_ROUND_PREFLOP) {
				openRoundCards (currentRound);
			}
			gameObjectManager.getPlayerChipsObject (playersManager.totalPlayerOnTable ()).SetActive (false);
			playersManager.addPlayer (player);
//			handleAnim.GetInstanceCardAnimation ().MoveObject (movingCoinObject, player.getBetChipsObject().gameObject);
		} else {
//			playerTmp.setCardsAndBalance (balance, card1, card2, null);
		}

		if (playersManager.getAllPlayers ().Count < GameConstant.MIN_PLAYER_TO_START_GAME) {
			waitingForMinPlayer ();
		}
	}

	public void leavePlayerOnTable (string playerName)
	{
		PlayerBean player = playersManager.getPlayerFromName (playerName);
		if (player != null) {
			playersManager.removePlayer (player);
		}
	}

	public int getBlindAmount(){
		return smallBlindAmount;
	}
	public void setBlindAmount(int blindAmount){
//		DEBUG.Log ("Blind Amt : "+ blindAmount);
		this.smallBlindAmount = blindAmount;
	}
	public void defineBlindPlayer (int blindAmount, string dealer, string sbPlayerName, string bbPlayerName)
	{
		this.smallBlindAmount = blindAmount;
		playersManager.setDealerPlayer (dealer);
		playersManager.setBigBlindPlayer (bbPlayerName);
		playersManager.setSmallBlindPlayer (sbPlayerName);
	}

	public void highLightTurnPlayer (string player)
	{
		if (isRunningGame) {
			PlayerBean currentPlayer = playersManager.setCurrentTurnPlayer (player);

			if (appwarp.username.Equals (player)) {
				playerBalance = currentPlayer.getBalance ();
				RoundManager currentRoundManager = getCurrentRoundInfo ();
				if (currentPlayer.isWaitingForGame ()) {
					sendPlayerActionToServer (0, GameConstant.ACTION_WAITING_FOR_GAME);
				}else if(currentRound!=GameConstant.TEXASS_ROUND_PREFLOP && 
					currentRoundManager.getAllTurnRecords().Count == 0 &&
					!currentPlayer.getPlayerName().Equals( playersManager.getSmallBlindPlayer().getPlayerName())){
					sendPlayerActionToServer (0, GameConstant.ACTION_NO_TURN);
				}else if (currentPlayer.isFoldedPlayer ()) {
					sendPlayerActionToServer (0, GameConstant.ACTION_FOLDED);
				} else if (currentPlayer.isAllInPlayer ()) {
					sendPlayerActionToServer (0, GameConstant.ACTION_ALL_IN);
				} else {
					setEnableDisableOptions (true);
				}

			} else {
				playerBalance = 0;
				setEnableDisableOptions (false);
				//gameManagerGameObject.transform.Find(GameConstant.UI_PATH_BUTTONS).gameObject.SetActive(false);
			}
		}
		//raiseValue = 0;
		//gameManagerGameObject.transform.Find (GameConstant.UI_PATH_BET_CHIP_DETAILS).GetComponent<Text> ().text = GameConstant.CURRENCY + raiseValue;
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
	public void gameFinished ()
	{
		GAME_STATUS = GameConstant.FINISHED;
	}

	public void manageGameFinishAction (string winnerPlayerName, int rank, int winnerPlayerWinningAmount, int winnerPlayerTotalBalance, List<string> listBestCard)
	{
		if (GAME_STATUS == GameConstant.FINISHED) {
			iTween.Stab(gameObject,iTween.Hash("audioclip",winnerAudio,"pitch",1));
			bool isAllAreFolded = playersManager.isAllPlayersAreFolded ();
			defaultCardsManager.openFlopCards ();
			defaultCardsManager.openTurnCards ();
			defaultCardsManager.openRiverCards ();

			resetDefaultCards (false);
			resetWinningPlayers (false);

			isRunningGame = false;
			setEnableDisableOptions (false);

			txtLog.text = txtLog.text.ToString () + "\nWinner is " + winnerPlayerName + "\nRank : " + getRankName((HAND_RANK)rank) +"\nWinning Amount : "+winnerPlayerWinningAmount+ "\n";

			PlayerBean winnerPlayer = playersManager.findWinnerPlayerFromName (winnerPlayerName);
			StartCoroutine(manageWinnerPlayers (winnerPlayer));
			winnerPlayer.setPlayerBalance (winnerPlayerTotalBalance);

			//DEBUG.Log ("Game Finish : Winner is " + winnerPlayerName + " >> Rank : " + (HAND_RANK)rank);

			if (!isAllAreFolded) {
				winnerPlayer.getRankPannel ().SetActive (true);
				winnerPlayer.getRankText ().text = getRankName ((HAND_RANK)rank) + " (" + Utility.GetCurrencyPrefix() + "" + winnerPlayerWinningAmount + ")";
				winnerPlayer.getRankText ().color = Color.white;
				playersManager.openAllPlayerCards ();
				//reset cards
				foreach (string cardName in listBestCard) {
					GameObject cardGameObject = winnerPlayer.getCardObjectFromCardName (cardName);
					if (cardGameObject != null) {
						animationManager.WinningCardAnimation (cardGameObject);	
					} else {
						cardGameObject = defaultCardsManager.getCardObjectFromCardName (cardName);				
						if (cardGameObject != null) {

							animationManager.WinningCardAnimation (cardGameObject);	
						}
					}
				}
			}
			//			StartCoroutine(hideWinnerRankPannel(winnerPlayer));
			Invoke ("scrollDown",1f);
		}
	}
	int totalBBPlayersTurn = 0;
	public void managePlayerMoveAction (string name, int betamount, int totalTableAmount, int totalPlayerBalance, int action)
	{
		PlayerBean playerBean = playersManager.getPlayerFromName (name); 

		RoundManager currentRoundManger = getCurrentRoundInfo ();
		if (currentRound == GameConstant.TEXASS_ROUND_PREFLOP && playerBean.isBigBlindPlayer ()) {
			totalBBPlayersTurn++;
		}
	
		if (!playerBean.isWaitingForGame ()) {
			txtLog.text = txtLog.text.ToString () + "\n" + playerBean.getPlayerName () + " >> Action >> " + getActionName (action) + " >> " + betamount;
			Invoke ("scrollDown",1f);
		}
		if (betamount > 0) {
			animationManager.MoveChipsObject (playerBean.getChipObject (), gameObjectManager.getTableChipSetObject ());
//			yield return new WaitForSeconds (GameConstant.ANIM_WAITING_TIME);
			playerBean.getChipObject ().transform.position = playerBean.getChipPositionObject ().transform.position;
		}
//		Debug.Log ("Player Moved : " + playerBean.getPlayerName () + " >> Action >> " + action + " >> " + betamount);
		TurnManager turnManager = new TurnManager (playerBean, action, betamount);
		currentRoundManger.addTurnRecord (turnManager);
		int totalBetAmtInRound = currentRoundManger.getTotalPlayerBetAmount (playerBean);
		playerBean.setBetAmount (betamount, totalBetAmtInRound);
		playersManager.setLastPlayerAction (name, action, betamount, totalBetAmtInRound, totalPlayerBalance);
//		playersManager.setPlayerBetAmount (playerBean, betamount, totalBetAmtInRound, totalPlayerBalance);
		setTotalBetTableAmount (totalTableAmount);
		//currentRoundTurns.Add (turnManager);
	
	}
	public void openRoundCards(int round){
		switch (round) {
		case GameConstant.TEXASS_ROUND_FLOP:
			startFlopRound ();
			defaultCardsManager.openFlopCards ();
			break;
		case GameConstant.TEXASS_ROUND_TURN:
			startTurnRound ();
			defaultCardsManager.openFlopCards ();
			defaultCardsManager.openTurnCards ();
			break;
		case GameConstant.TEXASS_ROUND_RIVER:
			startRiverRound ();
			defaultCardsManager.openFlopCards ();
			defaultCardsManager.openTurnCards ();
			defaultCardsManager.openRiverCards ();
			break;
		default:
			break;
		}

	}
	public void moveToNextRound (int round, int totalBetAmount)
	{
		Invoke ("scrollDown",1f);
//		StartCoroutine (CollectAllBetCoinsTOCenterOfTable ());
		switch (round) {
		case GameConstant.TEXASS_ROUND_FLOP:
			startFlopRound ();
			PassCoinsFromPlayerToCenterOfTable (appwarp.username);
			defaultCardsManager.openFlopCards ();

			break;
		case GameConstant.TEXASS_ROUND_TURN:
			startTurnRound ();
			defaultCardsManager.openTurnCards ();
			break;
		case GameConstant.TEXASS_ROUND_RIVER:
			startRiverRound ();
			defaultCardsManager.openRiverCards ();
			break;
		default:
			break;
		}
		//currentRoundTurns.Clear ();
		playersManager.resetPlayersBetAmount ();
		setTotalBetTableAmount (totalBetAmount);
	}

	public void setTotalBetTableAmount (int amount)
	{
		gameObjectManager.getTotalTableBetAmountText ().text = Utility.GetCurrencyPrefix() + amount;
	}
	// Will remove this code


	public void setEnableDisableOptions (bool flag)
	{
//		callFlag= false;raiseFlag= false;checkFlag = false;betFlag = false;
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
////				lastPlayerBetAmt = lastActivePlayerTurn.getBetAmount ();
//				lastPlayerBetAmt = currentRoundManager.getTotalPlayerBetAmount (lastActivePlayerTurn.getPlayer ());
//				gameObjectManager.getBetAmountText ().text = GameConstant.CURRENCY + lastPlayerBetAmt;
//			} else {
//				lastPlayerBetAmt = 0;
//				if (appwarp.isLimitGame) {
//					if (currentRound == GameConstant.TEXASS_ROUND_PREFLOP || currentRound == GameConstant.TEXASS_ROUND_FLOP) {
//						lastPlayerBetAmt = smallBlindAmount;
//					} else if (currentRound == GameConstant.TEXASS_ROUND_RIVER || currentRound == GameConstant.TEXASS_ROUND_TURN) {
//						lastPlayerBetAmt = smallBlindAmount * 2;
//					}
//
//				}
//			}
//		
//			int totalRaiseOnGame = currentRoundManager.getTotalRaiseCounter ();
//			if (currentRound == GameConstant.TEXASS_ROUND_PREFLOP) {
//				gameObjectManager.getCallButton ().interactable = true;
//				gameObjectManager.getRaiseButton ().interactable = true;
//				SliderChips.enabled = true;
//			} else if (currentRoundManager.getAllTurnRecords ().Count == 0 ||
//				lastActivePlayerTurn == null){
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
//
//			int lastBetAmount = currentRoundManager.getTotalPlayerBetAmount (lastActivePlayerTurn.getPlayer ());
//			int myTotalBetAmount = currentRoundManager.getTotalPlayerBetAmount (playersManager.getPlayerFromName (appwarp.username));
//			int pendingBetAmount = lastBetAmount - myTotalBetAmount;
//
//			if (currentRound == GameConstant.TEXASS_ROUND_PREFLOP && totalBBPlayersTurn == 1 && pendingBetAmount == 0) {
//				gameObjectManager.getCallButton ().interactable = false;
//				gameObjectManager.getCheckButton().interactable = true;
//				gameObjectManager.getRaiseButton ().interactable = true;
//				SliderChips.enabled = true;
//			}
//			if (lastActivePlayerTurn != null && pendingBetAmount > loggedPlrBalance) {
//				//				DEBUG.Log ("Last Plr : "+ lastActivePlayerTurn.getBetAmount() +" >> "+ playersManager.getPlayerFromName(appwarp.username).getBalance());
//				gameObjectManager.getCallButton ().interactable = false;
//				gameObjectManager.getRaiseButton ().interactable = false;
//				gameObjectManager.getAllInButton ().interactable = true;
//				gameObjectManager.getAllInButton ().gameObject.SetActive (true);
//				lastPlayerBetAmt = loggedPlrBalance;
//			} else {
//				//				DEBUG.Log (" >> "+ playersManager.getPlayerFromName(appwarp.username).getBalance());
//				gameObjectManager.getAllInButton ().gameObject.SetActive (false);
//			}
//			if (totalRaiseOnGame >= GameConstant.MAXIMUM_RAISE_ON_GAME) {
//				gameObjectManager.getRaiseButton ().interactable = false;
//			}
//			SliderChips.value = (float)lastPlayerBetAmt;
//			raiseValue = lastPlayerBetAmt;
//			gameObjectManager.getBetAmountText().text = GameConstant.CURRENCY + raiseValue;
//
//		}
	}

	public void onReBuyButtonSeleted(){
//		gameObjectManager.getReBuyButton ().interactable = false;
//		WarpClient.GetInstance ().SendChat(GameConstant.REQUEST_FOR_RE_BUY);	
	}
	public void onFoldButtonSelected ()
	{
//		gameObjectManager.getFoldButton ().interactable = false;
//		sendPlayerActionToServer (0, GameConstant.ACTION_FOLD);
	}

	public void onCheckButtonSelected ()
	{
//		gameObjectManager.getCheckButton ().interactable = false;
//		sendPlayerActionToServer (0, GameConstant.ACTION_CHECK);
	}

	public void onRaiseButtonSelected ()
	{
		RoundManager currentRoundManager = getCurrentRoundInfo ();
		TurnManager lastActiverPlayerTurn = currentRoundManager.getLastActivePlayerTurn ();
		int lastPlayerBetAmt = currentRoundManager.getTotalPlayerBetAmount (lastActiverPlayerTurn.getPlayer ());
		int totalPlrPrvBetAmt = currentRoundManager.getTotalPlayerBetAmount (playersManager.getPlayerFromName (appwarp.username));
//		DEBUG.Log (">><< "+raiseValue);
		if ((raiseValue == 0 || raiseValue == lastPlayerBetAmt) && appwarp.isLimitGame) {
			int maxRaiseValue = 0;
			int totalPendingBetAmt = lastPlayerBetAmt - totalPlrPrvBetAmt;
			if (currentRound == GameConstant.TEXASS_ROUND_PREFLOP || currentRound == GameConstant.TEXASS_ROUND_FLOP) {
				maxRaiseValue = totalPendingBetAmt + smallBlindAmount;
			} else if (currentRound == GameConstant.TEXASS_ROUND_RIVER || currentRound == GameConstant.TEXASS_ROUND_TURN) {
				maxRaiseValue = totalPendingBetAmt + smallBlindAmount * 2;
			}
//			DEBUG.Log ("LastPlrBet : " + lastPlayerBetAmt + " >> TtlPlrBet : " + totalPlrPrvBetAmt + 
//								" >> PendingBet : " + totalPendingBetAmt + " >> MaxRais : " + maxRaiseValue +
//								" >> TotalPlayerBet : " +( maxRaiseValue + totalPendingBetAmt));
			SliderChips.value = maxRaiseValue;
			raiseValue = maxRaiseValue;
			gameObjectManager.getBetAmountText ().text = GameConstant.CURRENCY + raiseValue;
		}else {
			raiseValue = raiseValue - totalPlrPrvBetAmt;
//			DEBUG.Log ("LastPlrBet : " + lastPlayerBetAmt + " >> TtlPlrBet : " + totalPlrPrvBetAmt + " >> TotalPlayerBet : " + (raiseValue + totalPlrPrvBetAmt) + " >> Raise : " + raiseValue);
		}
		if (raiseValue != 0 && (raiseValue + totalPlrPrvBetAmt) > lastPlayerBetAmt) {
//			gameObjectManager.getRaiseButton ().interactable = false;
////			DEBUG.Log (">><< "+raiseValue);
//			sendPlayerActionToServer (raiseValue, GameConstant.ACTION_RAISE);
		}
	}

	public void onCallButtonSelected ()
	{
		raiseValue = (int)0;
		SliderChips.value = (float)raiseValue;
		gameObjectManager.getBetAmountText ().text = GameConstant.CURRENCY + raiseValue;
		RoundManager currentRoundManager = getCurrentRoundInfo ();
		TurnManager lastActiverPlayerTurn = currentRoundManager.getLastActivePlayerTurn ();
		int lastBetAmount = currentRoundManager.getTotalPlayerBetAmount (lastActiverPlayerTurn.getPlayer ());
		int myTotalBetAmount = currentRoundManager.getTotalPlayerBetAmount (playersManager.getPlayerFromName (appwarp.username));
		int pendingBetAmount = lastBetAmount - myTotalBetAmount;

		//		DEBUG.Log ("Call >> Last Bet : "+lastBetAmount+" >> My Total Bet : "+myTotalBetAmount+" >> Pending Bet "+pendingBetAmount);
		if (pendingBetAmount > 0) {
//			gameObjectManager.getCallButton ().interactable = false;
//			sendPlayerActionToServer (pendingBetAmount, GameConstant.ACTION_CALL);
		} else {
			DEBUG.Log ("Bug : Bet Amt : " + pendingBetAmount);
		}
	}

	public void onBetButtonSelected ()
	{
		if (raiseValue != 0) {
//			gameObjectManager.getBetButton ().interactable = false;
//			sendPlayerActionToServer (raiseValue, GameConstant.ACTION_BET);

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
	public void onAllInButtonSelected ()
	{
//		raiseValue = playersManager.getPlayerFromName (appwarp.username).getBalance ();
//		gameObjectManager.getAllInButton ().interactable = false;
//		sendPlayerActionToServer (raiseValue, GameConstant.ACTION_ALL_IN);
	}

	private void sendPlayerActionToServer (int betAmount, int action)
	{
		iTween.Stab(gameObject,iTween.Hash("audioclip",buttonAudio,"pitch",1));
		raiseValue = 0;
		lastPlayerBetAmt = 0;
		SliderChips.value = 0f;
		SliderChips.enabled = false;
		gameObjectManager.getBetAmountText ().text = GameConstant.CURRENCY + raiseValue;

		JSON_Object requestJson = new JSON_Object ();
		try {
			requestJson.put (GameConstant.TAG_PLAYER_NAME, appwarp.username);
			requestJson.put (GameConstant.TAG_BET_AMOUNT, betAmount);
			requestJson.putOpt (GameConstant.TAG_ACTION, action);
			//setPlayerBetAmount(appwarp.username,betAmount);
			WarpClient.GetInstance ().sendMove (GameConstant.REQUEST_FOR_ACTION + requestJson.ToString ());	
		} catch (Exception e) {
			Debug.Log ("GameManager : " + e.ToString ());
		}	

	}

	public void OnSliderMove ()
	{
		if (SliderChips != null) {
			//MaxValue =int.Parse(plrManager.GetPlayerById(curPlayerId).GetComponent<Player>().GetPlayerBalance());
			//int MaxValue=100;
			if (playerBalance != 0 && SliderChips.maxValue != playerBalance) {
				SliderChips.maxValue = (float)playerBalance;
			}
			if (SliderChips.value <= lastPlayerBetAmt) {
				SliderChips.value = lastPlayerBetAmt;
			}

			if (appwarp.isLimitGame) {
				RoundManager currentRoundManager = getCurrentRoundInfo ();
				int totalPlrPrvBetAmt = currentRoundManager.getTotalPlayerBetAmount (playersManager.getPlayerFromName (appwarp.username));
				int maxRaiseValue = 0;
				int totalPendingBetAmt = lastPlayerBetAmt - totalPlrPrvBetAmt;
				if (currentRound == GameConstant.TEXASS_ROUND_PREFLOP || currentRound == GameConstant.TEXASS_ROUND_FLOP) {
					maxRaiseValue = totalPendingBetAmt + smallBlindAmount;
				} else if (currentRound == GameConstant.TEXASS_ROUND_RIVER || currentRound == GameConstant.TEXASS_ROUND_TURN) {
					maxRaiseValue = totalPendingBetAmt + smallBlindAmount * 2;
				}
//				DEBUG.Log ("MaxRaise : "+ maxRaiseValue);
				maxRaiseValue = maxRaiseValue + totalPlrPrvBetAmt;
//				DEBUG.Log ("LastPlrAmt : "+lastPlayerBetAmt +" >> PlrPrvAmt >> "+totalPlrPrvBetAmt+" >> MaxRaise :>> "+ maxRaiseValue);
				if (SliderChips.value >= maxRaiseValue) {
					SliderChips.value = maxRaiseValue;
				}
			}
			manageOptionsFromSlider ();
			raiseValue = (int)SliderChips.value;
			gameObjectManager.getBetAmountText ().text = GameConstant.CURRENCY + raiseValue;
			//PuttingText.text=raiseValue.ToString();
		}
	}
	bool callFlag,raiseFlag,checkFlag,betFlag = false;
	public void manageOptionsFromSlider(){
		if (SliderChips.value == SliderChips.maxValue) {

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
		} else {
			//			DEBUG.Log ("<< "+gameObjectManager.getCheckButton().IsInteractable());
//			gameObjectManager.getAllInButton ().interactable = false;
//			gameObjectManager.getAllInButton ().gameObject.SetActive (false);
//			if(callFlag)
//				gameObjectManager.getCallButton ().interactable = true;
//			if(raiseFlag)
//				gameObjectManager.getRaiseButton().interactable = true;
//			if(checkFlag)
//				gameObjectManager.getCheckButton ().interactable = true;
//			if(betFlag)
//				gameObjectManager.getBetButton().interactable = true;

		}
	}
	IEnumerator CollectAllBetCoinsTOCenterOfTable ()
	{
		
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

	IEnumerator PassCoinsFromPlayerToCenterOfTable (string playerName)
	{
					
		GameObject src = null;	
		GameObject dest = null;
						
		src = playersManager.getPlayerFromName (playerName).getBetChipsObject ().gameObject;
		dest = gameManagerGameObject.transform.Find ("TableChips/ChipsSet1").gameObject;
					                        
					                        
		movingCoinObject.GetComponent<Image> ().sprite = src.GetComponent<Image> ().sprite;
		movingCoinObject.GetComponent<RectTransform> ().position = src.GetComponent<RectTransform> ().position;
					                        
		animationManager.MoveObject (movingCoinObject, dest);
		yield return new WaitForSeconds (0.2f);
	}

	public void waitingForMinPlayer ()
	{
		StartCoroutine (watingForPlayer ());
	}

	public void restartGame ()
	{
		if (GAME_STATUS == GameConstant.FINISHED)
			StartCoroutine (startGameCounter ());
	}

	public IEnumerator startGameCounter ()
	{
		int cntr = 0;
		resetDefaultCards (true);
		resetWinningPlayers (true);
//		WarpClient.GetInstance ().SendChat (GameConstant.REQUEST_FOR_RESTART_GAME);
		gameObjectManager.getRestartPannel ().SetActive (true);
		string message = "Break Time";
		if (!isBreakTime) {
			gameObjectManager.getRestartPannel ().SetActive (false);
			message = "Waiting for all players restart response";
		}
		while (!isRunningGame) {
			string dot = getDotFromCntr (cntr++);
			gameObjectManager.getRestartText ().text = message + dot;
			if (cntr > 3)
				cntr = -1;
			yield return new WaitForSeconds (1);
		}

		Debug.Log ("Countdown Complete!");
		gameObjectManager.getRestartPannel ().SetActive (false);

	}

	public IEnumerator watingForPlayer ()
	{

		gameObjectManager.getRestartPannel ().SetActive (true);
		int cntr = 0;
		while (playersManager.getAllPlayers ().Count < GameConstant.MIN_PLAYER_TO_START_GAME) {
			string dot = getDotFromCntr (cntr++);
			gameObjectManager.getRestartText ().text = "Waiting for players" + dot;
			if (cntr > 3)
				cntr = -1;
			yield return new WaitForSeconds (1);
		}
		Debug.Log ("Countdown Complete!");
		gameObjectManager.getRestartPannel ().SetActive (false);
	}

	public PlayersManager getPlayerManager(){
		return playersManager;
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

	public string getActionName (int memberValue)
	{

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

	public Slider SliderChips;
	int playerBalance = 0;
	int raiseValue = 0;
	int lastPlayerBetAmt = 0;
	public bool canReBuy =false;
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

}

