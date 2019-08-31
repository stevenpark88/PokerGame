//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.UI;
//using com.shephertz.app42.gaming.multiplayer.client;
//using com.shephertz.app42.gaming.multiplayer.client.events;
//using System;
//public class GamePlayScript_temp : MonoBehaviour {
//
//	public List<Sprite> cardArray;
//	public Image[] cardSprite;
//	public Image[] tableCards;
//	public Text[] userNamesTxtArray;
//	public Text txtAllText;
//	public List<GameObject> listUserObject;
//	public List<string> listUsers = new List<string>();
//	int intPlayerCounter = 0;
//	bool waiting = false;
//	float waitingTime = 20.0f;
//	GamePlayScript instance;
//	public Slider SliderChips;
//	private int CallAmt;
//	private int BetAmt=-1;
//	private int RaisAmt;
//	public Text PuttingText;
//	private int MaxValue=100;
//	public Text TotalTableText;
//	private int TotalTableAmt=0; 
//    PlayerManager plrManager;
//	private int CurrentAction=-1;
//	private string CurrentPlayerName;
//	int curPlayerId;
//	private int currentRound=-1;
//
//	private bool isActiveCall = true;
//	private bool isActiveBet;
//	private bool isActiveFold;
//	private bool isActiveRaise;
//	private bool isActiveCheck;
//	private bool isActiveAllIn;
//	private HandleAnimations handleAnim;
//	// Use this for initialization
//	public GameObject movingCoinObject;
//	public GameObject movingCardObject;
//
//	public GameObject WinningCardsDeck;
//
//
//	void Start () 
//	{
//
//		plrManager= GameObject.FindGameObjectWithTag("GamePlayPanel").GetComponent<PlayerManager> ();
//		handleAnim= GameObject.FindGameObjectWithTag("GamePlayPanel").GetComponent<HandleAnimations> ();
//
//		//Debug.Log("Selected Room Id : "+appwarp.currentRoomId);
//		WarpClient.GetInstance ().GetLiveRoomInfo (appwarp.currentRoomId);
//	
//		ShowWinnerPokerHand (WinningCardsDeck,listUserObject[1]);
//		StartCoroutine(AllAnim());
//		instance=this.GetComponent<GamePlayScript>();
//		currentRound = GameConstant.ROUND_FLOP;
//	}
//
//	IEnumerator AllAnim() {
//				handleAnim.AnimDisplayDefaultCard (GameConstant.ROUND_FLOP);
//		        yield return new WaitForSeconds (0.2f);
//		 
//		yield return 	StartCoroutine (CollectAllBetCoinsTOCenterOfTable());
//	    yield return 	StartCoroutine (DistributeHandCardsToAllPlayers());
//
//	//	yield return 	StartCoroutine (PassWinningAmountToPlayer("Nilesh"));
//	//	yield return 	StartCoroutine (PassCoinsFromPlayerToCenterOfTable("Nilesh"));
//    //  yield return 	StartCoroutine (AnimFlipCard(listUserObject [0].gameObject.transform.GetChild (0).transform.GetChild(0).gameObject));
//
//	}        
//
//
//	public void ShowWinnerPokerHand(GameObject PokerHandCardsDeck,GameObject PlayerObject){
//
//		PokerHandCardsDeck.transform.localPosition = PlayerObject.transform.localPosition;
//		PokerHandCardsDeck.SetActive (true);
//		GameObject firstCard = null;
//		GameObject secondCard = null;
//		firstCard = PokerHandCardsDeck.transform.GetChild (0).gameObject;
//		secondCard = PokerHandCardsDeck.transform.GetChild (3).gameObject;
//		MoveHandCardInAnyDirection (firstCard,secondCard);
//	}
//
//	public void MoveHandCardInAnyDirection(GameObject first,GameObject second){
//
//		//move by distance 20 in y direction
//		handleAnim.GetInstanceCardAnimation ().MoveObjectInDirection (first,20,"y");
//		handleAnim.GetInstanceCardAnimation ().MoveObjectInDirection (second,20,"y");
//
//	}
//
//
//
//	public void OnSliderMove(){
//		if (SliderChips!=null) {
//			MaxValue =int.Parse(plrManager.GetPlayerById(curPlayerId).GetComponent<Player>().GetPlayerBalance());
//					
//			if(SliderChips.maxValue!=MaxValue){
//				
//				SliderChips.maxValue=MaxValue;
//			}
//			
//			float raiseValue=SliderChips.value;
//			Debug.Log("this is slider value "+PuttingText);
//			PuttingText.text=raiseValue.ToString();
//		}
//	}
//
//
//	IEnumerator DistributeHandCardsToAllPlayers(){
//		
//		GameObject src = null;	
//		GameObject dest = null;
//		src = GameObject.Find ("CardDeck");
//		int i = 0;
//		while( i<listUserObject.Count){
//			yield return 	StartCoroutine (MoveCardObjectToPlayer(src,listUserObject [i].gameObject.transform.GetChild (0).transform.GetChild(0).gameObject));
//			yield return new WaitForSeconds (0.1f);
//			listUserObject [i].gameObject.transform.GetChild (0).transform.GetChild(0).gameObject.SetActive(true);
//			yield return StartCoroutine (MoveCardObjectToPlayer(src,listUserObject [i].gameObject.transform.GetChild (0).transform.GetChild(1).gameObject));
//			yield return new WaitForSeconds (0.1f);
//	        listUserObject [i].gameObject.transform.GetChild (0).transform.GetChild(1).gameObject.SetActive(true);
//			i++;
//		}
//
//		movingCardObject.SetActive (false);
//		yield return new WaitForSeconds (0.2f);
//	}
//
//
//	IEnumerator PassWinningAmountToPlayer(string playerName){
//		
//		GameObject src = null;	
//		GameObject dest = null;
//		GameObject Player = listUserObject [0];//   plrManager.GetPlayerByName (playerName);
//	
//		src = GameObject.Find ("TotalChipsSet1");
//		dest = Player.transform.GetChild (2).gameObject;
//	
//		movingCoinObject.GetComponent<Image> ().sprite = src.GetComponent<Image> ().sprite;
//		movingCoinObject.GetComponent<RectTransform> ().position = src.GetComponent<RectTransform> ().position;
//	
//		handleAnim.GetInstanceCardAnimation().MoveObject (movingCoinObject,dest);
//		yield return new WaitForSeconds (0.2f);
//	}
//
//	IEnumerator PassCoinsFromPlayerToCenterOfTable(string playerName){
//		
//		GameObject src = null;	
//		GameObject dest = null;
//		GameObject Player = listUserObject [0];//   plrManager.GetPlayerByName (playerName);
//
//		src = Player.transform.GetChild (2).gameObject;
//		dest = GameObject.Find ("TotalChipsSet1");
//
//		
//		movingCoinObject.GetComponent<Image> ().sprite = src.GetComponent<Image> ().sprite;
//		movingCoinObject.GetComponent<RectTransform> ().position = src.GetComponent<RectTransform> ().position;
//		
//		handleAnim.GetInstanceCardAnimation().MoveObject (movingCoinObject,dest);
//		yield return new WaitForSeconds (0.2f);
//	}
//
//
//
//	IEnumerator MoveCardObjectToPlayer(GameObject src,GameObject dest){
//	        
//		movingCardObject.SetActive (true);
//		   
//		movingCardObject.GetComponent<Image> ().sprite = src.GetComponent<Image> ().sprite;
//		movingCardObject.GetComponent<RectTransform> ().position = src.GetComponent<RectTransform> ().position;
//		handleAnim.GetInstanceCardAnimation().MoveObject (movingCardObject, dest);
//
//		//	src.SetActive (false);
//			yield return new WaitForSeconds (0.2f);
//		 
//	
//	}
//
//	IEnumerator CollectAllBetCoinsTOCenterOfTable(){
//
//		GameObject src = null;	
//		GameObject dest = null;
//
//		int i = 0;
//		while( i<listUserObject.Count){
//			src=	listUserObject [i].gameObject.transform.GetChild (2).gameObject;
//			dest = GameObject.Find ("TotalChipsSet1");
//			movingCoinObject.GetComponent<Image> ().sprite = src.GetComponent<Image> ().sprite;
//			movingCoinObject.GetComponent<RectTransform> ().position = src.GetComponent<RectTransform> ().position;
//			handleAnim.GetInstanceCardAnimation().MoveObject (movingCoinObject,dest);
//			src.SetActive (false);
//			yield return new WaitForSeconds (0.2f);
//			i++;
//		}
//
//	}
//
//
//
//	IEnumerator AnimFlipCard(GameObject gameObject){
//		
//		handleAnim.GetInstanceCardAnimation().FlipObject (gameObject);
//		yield return new WaitForSeconds (0.2f);
//	}
//
//
//	public void OnCallButtonPressed(){
//
//		CurrentAction = GameConstant.ACTION_CALL;
//		SetCurrentBetAmount (10);
//
//		
//		string BetAmount = "-1";
//		int Action = -1;
//		string PlayerName="";
//		BetAmount=GetCurrentBetAmount().ToString();
////			BetAmount="10";
////			Action=1;
//		PlayerName=appwarp.username;
//
//		JSONObject requestJson = new JSONObject ();
//		try {
//			requestJson.put (GameConstant.TAG_PLAYER_NAME, PlayerName);
//			requestJson.put (GameConstant.TAG_BET_AMOUNT, BetAmount);
//		  requestJson.putOpt (GameConstant.TAG_ACTION, CurrentAction);
//			
//		} catch (Exception e) {
//			
//		}	
//
//		WarpClient.GetInstance ().sendMove (GameConstant.REQUEST_FOR_ACTION + requestJson.ToString ());
//	}
//
//	public void OnBetButtonPressed(){
//		CurrentAction = GameConstant.ACTION_BET;
//		SetCurrentBetAmount (10);
//
//		string BetAmount = "-1";
//		int Action = -1;
//		string PlayerName="";
//		BetAmount=GetCurrentBetAmount().ToString();
//		//			BetAmount="10";
//		//			Action=1;
//		PlayerName=appwarp.username;
//		
//		JSONObject requestJson = new JSONObject ();
//		try {
//			requestJson.put (GameConstant.TAG_PLAYER_NAME, PlayerName);
//			requestJson.put (GameConstant.TAG_BET_AMOUNT, BetAmount);
//			requestJson.putOpt (GameConstant.TAG_ACTION, CurrentAction);
//			
//		} catch (Exception e) {
//			
//		}	
//		
//		WarpClient.GetInstance ().sendMove (GameConstant.REQUEST_FOR_ACTION + requestJson.ToString ());
//
//	}
//
//	public void OnFoldButtonPressed(){
//		CurrentAction = GameConstant.ACTION_FOLD;
//
//	}
//
//	public void OnCheckButtonPressed(){
//		CurrentAction = GameConstant.ACTION_CHECK;
//		SetCurrentBetAmount (0);
//	}
//
//	public void OnRaiseButtonPressed(){
//		CurrentAction = GameConstant.ACTION_RAISE;
//		SetCurrentBetAmount (15);
//						
//		string BetAmount = "-1";
//		int Action = -1;
//		string PlayerName="";
//		BetAmount=GetCurrentBetAmount().ToString();
//		//			BetAmount="10";
//		//			Action=1;
//		PlayerName=appwarp.username;
//		
//		JSONObject requestJson = new JSONObject ();
//		try {
//			requestJson.put (GameConstant.TAG_PLAYER_NAME, PlayerName);
//			requestJson.put (GameConstant.TAG_BET_AMOUNT, BetAmount);
//			requestJson.putOpt (GameConstant.TAG_ACTION, CurrentAction);
//			
//		} catch (Exception e) {
//			
//		}	
//		
//		WarpClient.GetInstance ().sendMove (GameConstant.REQUEST_FOR_ACTION + requestJson.ToString ());
//	}
//
//	public void OnAllInButtonPressed(){
//		CurrentAction = GameConstant.ACTION_ALL_IN;
//
//	}
//
////	IEnumerator DistributeCards()
////	{
////		yield return new WaitForSeconds (3f);
////		for (int i = 0; i < cardSprite.Length; i++) 
////		{
////			int rnd = Random.Range(0,cardArray.Count - 1);
////
////			cardSprite[i].GetComponent<Image>().sprite = cardArray[rnd];
////
////			cardArray.RemoveAt(rnd);
////		}
////
////		for(int j = 0; j < tableCards.Length; j++)
////		{
////			int rnd1 = Random.Range(0,cardArray.Count - 1);
////			tableCards[j].GetComponent<Image>().sprite = cardArray[rnd1];
////			cardArray.RemoveAt(rnd1);curRound
////		}
////	}
//
//	public void SetWinnerPlayerBalance(string plrName, string plrBal){
//		plrManager.UpdatePlayerBalance (plrName, plrBal);
//	}
//
//	public void SetPlayerBalance(string plrName, string plrBal){
//		plrManager.UpdatePlayerBalance (plrName, plrBal);
//	}
//
//
//	public void SetUpNextRound(int curRound ){
//
//		this.currentRound=curRound;
//
//	}
//
//
//	public void SetCurrentBetAmount(int betAmt){
//
//		this.BetAmt = betAmt;
//	}
//
//	public int GetCurrentBetAmount(){
//		return BetAmt;
//	}
//
//	public void SetTableTotalAmount(string tableAmt){
//		this.TotalTableText.text = tableAmt;
//	}
//
//	public  string GetTableTotalAmount(){
//
//		return this.TotalTableText.text;
//	}
//
//	public int GetCurrentAction(){
//	    return	this.CurrentAction;
//	}
//
//	public void SetCurrentPlayerName(string username){
//		this.CurrentPlayerName = username;
//	}
//
//	public string GetCurrentPlayerName(){
//		return  this.CurrentPlayerName;
//	}
//
//
//	public void FlopRoundCommCards()				// Display of Community Cards
//	{
//		for (int i=0; i<3; i++) {
//			tableCards[i].gameObject.SetActive(true);
//		}
//	}
//	
////	// For Testing
////	public void TestCommCards()				// Display of Community Cards
////	{
////		for(int i = 0;i<5;i++)
////		{
////			Card card = tableCards[i];
////		}
////	}
////	
//	
//	// Open cards for Turn Round Cards
//	public void TurnRoundCommCards()				// Display of Community Cards
//	{
//
//			tableCards[3].gameObject.SetActive(true);
//	
//	}
//	
//	
//	public void RiverRoundCommCards()				// Display of Community Cards
//	{
//
//			tableCards[4].gameObject.SetActive(true);
//
//	}
//
//
//	public void PresetForNextRound(int currentRound){
//
//
//		switch (currentRound) {
//				
////		case GameConstant.ROUND_PREFLOP:
////				//	curRound = ROUND_FLOP;
////				FlopRoundCommCards();
////				ResetForNextRound();
////				break;
//		case GameConstant.ROUND_FLOP:
//				
//			FlopRoundCommCards();
//			ResetForNextRound();
//						
//				break;
//		case GameConstant.ROUND_TURN:
//				
//			TurnRoundCommCards();
//			ResetForNextRound();
//
//				
//				break;
//		case GameConstant.ROUND_RIVER:
//
//			RiverRoundCommCards();
//			ResetForNextRound();
//				
//			//	ResetForNextRound();
//				break;
//				
//			}
//	}
//	public void ResetForNextRound() {
//		
//		if (currentRound == -1) {
//			isActiveBet = false;
//			isActiveCall = false;
//			isActiveCheck = false;
//			isActiveFold = false;
//			isActiveRaise = false;
//			isActiveAllIn = false;
//		} else {
//
//			for (int i = 0; i < plrManager.GetTotalNPlayers(); i++) {
//				plrManager.GetPlayerById(i).GetComponent<Player> ().SetPlayerBetAmount ("");
//			}
//			
//
//			isActiveBet = true;
//			isActiveCall = false;
//			isActiveCheck = true;
//		}
//		
//		
//	}
//
//	public void onRoomDetails(LiveRoomInfoEvent liveRoomInfoEvent){
//		//DEBUG.Log("1");
////		DEBUG.Log ("1 "+liveRoomInfoEvent.getJoinedUsers().Length);
////		txtSelectedRoom.text = liveRoomInfoEvent.getData ().getName ();
////		txtAllText.text = liveRoomInfoEvent.getData ().getName ();
//
//		foreach(string username in liveRoomInfoEvent.getJoinedUsers () ){
//			plrManager.listRoomPlayers(username);
//		}
//	}
//
//	public void onNewOtherUserJoined(string username,JSONObject jsonResponce){
//		//DEBUG.Log("2 "+username );
//		if (plrManager == null) {
//			plrManager = GameObject.FindGameObjectWithTag ("GamePlayPanel").GetComponent<PlayerManager> ();
//		}
//
//		plrManager.listRoomPlayers(username);
//	//	SetTableDefaultCards(jsonResponce);
//		SetPlayerHandCards (username, jsonResponce);
//
//	  //		listUserObject [intPlayerCounter++].transform.FindChild ("UserNameText").GetComponent<Text> ().text = username;
//
//	}
//
//
//	public void onNewUserJoined(string username,JSONObject jsonResponce){
//		//DEBUG.Log("2 "+username );
//		if (plrManager == null) {
//			plrManager = GameObject.FindGameObjectWithTag ("GamePlayPanel").GetComponent<PlayerManager> ();
//		}
//	//	if (CurrentPlayerName == "") {
//			SetCurrentPlayerName (username);
//	//	}
//		plrManager.listRoomPlayers(username);
//		SetTableDefaultCards(jsonResponce);
//		SetPlayerHandCards (username, jsonResponce);
//		plrManager.FaceUpPlayerCards(username);
//		
//		//		listUserObject [intPlayerCounter++].transform.FindChild ("UserNameText").GetComponent<Text> ().text = username;
//		
//	}
//
//	public void OnOtherUserActionDone(JSONObject jsonResponse){
//		string userName=jsonResponse.getString(GameConstant.TAG_PLAYER_NAME);
//		string betAmt = jsonResponse.getString (GameConstant.TAG_BET_AMOUNT);
//
//	//	string totalTableAmount= jsonResponse.getString (GameConstant.TAG_TABLE_AMOUNT);
//		/* To know last user action Performed 
//		string takenAction = jsonResponse.getString (GameConstant.TAG_ACTION);*/
//
//	//	SetTableTotalAmount (totalTableAmount);
//		plrManager.GetPlayerByName (userName).GetComponent<Player> ().SetPlayerBetAmount (betAmt);
//
//	}
//
//	public void OnRoundComplete(JSONObject jsonResponse){
//		int  nextRound=jsonResponse.getInt(GameConstant.TAG_ROUND);
//
//	//	string nextRound=jsonResponse.getString(GameConstant.TAG_ROUND);
//
//		string totalTableAmount= jsonResponse.getString (GameConstant.TAG_TABLE_AMOUNT);
//
//		SetTableTotalAmount (totalTableAmount);
//		SetUpNextRound (nextRound);
//		PresetForNextRound (currentRound);
//
//		
//	}
//
//	public void OnGameComplete(JSONObject jsonResponse){
//
//		
//		string winnerName=jsonResponse.getString(GameConstant.TAG_WINER);
//		string totalWinnerBalance= jsonResponse.getString (GameConstant.TAG_WINER_TOTAL_BALENCE);
//	    SetWinnerPlayerBalance(winnerName,totalWinnerBalance);
//
//	}
//
//	public void SetPlayerInfo(JSONObject jsonResponse){
//		
//		
//		string playerName=jsonResponse.getString(GameConstant.TAG_PLAYER_NAME);
//		string playerBalance= jsonResponse.getString (GameConstant.TAG_PLAYER_BALANCE);
//		SetPlayerHandCards (playerName, jsonResponse);
//
//		SetPlayerBalance (playerName, playerBalance);
//		
//
//	}
//
//	public void SetTableDefaultCards(JSONObject jsonResponce){
//
//		SetDefaultCards(GameConstant.TAG_CARD_FLOP_1,jsonResponce.getString(GameConstant.TAG_CARD_FLOP_1));
//		SetDefaultCards(GameConstant.TAG_CARD_FLOP_2,jsonResponce.getString(GameConstant.TAG_CARD_FLOP_2));
//		SetDefaultCards(GameConstant.TAG_CARD_FLOP_3,jsonResponce.getString(GameConstant.TAG_CARD_FLOP_3));
//		SetDefaultCards(GameConstant.TAG_CARD_TURN,jsonResponce.getString(GameConstant.TAG_CARD_TURN));
//		SetDefaultCards(GameConstant.TAG_CARD_RIVER,jsonResponce.getString(GameConstant.TAG_CARD_RIVER));
//
//	}
//
//	public void SetDefaultCards(string crdTag,string crdName){
//		Sprite sp = null;
//		
//		for (int i=0; i<cardArray.Count; i++) {
//			if(crdName.Equals(cardArray[i].name)){
//				sp=cardArray[i];
//				break;
//			}
//		}
//		
//		if (crdTag == GameConstant.TAG_CARD_FLOP_1) {
//			tableCards [0].GetComponent<Image> ().sprite = sp;
//		} else if (crdTag == GameConstant.TAG_CARD_FLOP_2) {
//			tableCards [1].GetComponent<Image> ().sprite = sp;
//		} else if (crdTag == GameConstant.TAG_CARD_FLOP_3) {
//			tableCards [2].GetComponent<Image> ().sprite = sp;
//		} else if (crdTag == GameConstant.TAG_CARD_TURN) {
//			tableCards [3].GetComponent<Image> ().sprite = sp;
//		} else if (crdTag == GameConstant.TAG_CARD_RIVER) {
//			tableCards [4].GetComponent<Image> ().sprite = sp;
//		}
//	}
//
//
//	public void SetPlayerHandCards(string username,JSONObject jsonResponce){
//		
//		SetPlayerCards(plrManager.GetPlayerByName (username).GetComponent<Player> (),jsonResponce);
//		SetPlayerCards(plrManager.GetPlayerByName (username).GetComponent<Player> (),jsonResponce);
//		
//	}
//	
//
//	public void SetPlayerCards(Player plr,JSONObject jsonResponce){
//		SetPlayerDefaultCards(GameConstant.TAG_CARD_PLAYER_1,jsonResponce.getString(GameConstant.TAG_CARD_PLAYER_1),plr);
//		SetPlayerDefaultCards(GameConstant.TAG_CARD_PLAYER_2,jsonResponce.getString(GameConstant.TAG_CARD_PLAYER_2),plr);
//
//	}
//
//	public void SetPlayerDefaultCards(string crdTag,string crdName,Player plr){
//		Sprite sp = null;
//		
//		for (int i=0; i<cardArray.Count; i++) {
//			if(crdName.Equals(cardArray[i].name)){
//				sp=cardArray[i];
//				break;
//			}
//		}
//		
//		if (crdTag==GameConstant.TAG_CARD_PLAYER_1) {
//			plr.SetCardFirst(sp);
//		} else if (crdTag==GameConstant.TAG_CARD_PLAYER_2) {
//			plr.SetCardSecond(sp);
//		}
//	}
//
//
//
////	public void listRoomPlayers(string userName){
////		//DEBUG.Log ("list users "+listUsers.Count);
////		for(int i = 0; i<listUsers.Count; i++)
////		{
////			//DEBUG.Log("user "+listUsers[i]);
////		}
////
////		if (!listUsers.Contains(userName)) {
////			txtAllText.text =txtAllText.text +"\n"+userName;
////			listUserObject[intPlayerCounter].transform.Find("UserInfo/UserNameText").GetComponent<Text>().text = userName;
////			//waiting = true;
////
////			//userNamesTxtArray[intPlayerCounter].text = userName;
////			listUserObject[intPlayerCounter++].SetActive(true);
////
//////			Debug.Log("count "+intPlayerCounter);
//////			Debug.Log(".... "+listUserObject [intPlayerCounter].transform.FindChild ("UserNameText"));
//////			listUserObject [intPlayerCounter].transform.FindChild ("UserNameText").GetComponent<Text> ().text = userName;
////			listUsers.Add(userName);
////		}
////	}
//
////	void Update()
////	{
////		if (waiting) 
////		{
////			Image img = listUserObject[intPlayerCounter].GetComponent<Image>();
////			img.fillAmount -= 1.0f/waitingTime * Time.deltaTime;
////			if(img.fillAmount == 0)
////			{
////				waiting = false;
////				intPlayerCounter++;
////			}
////
////		}
////	}
//}
