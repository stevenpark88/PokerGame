using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TexassDefaultCardsManager 
{
	GameObject gameObject;
	Card flop1;
	Card flop2;
	Card flop3;
	Card river;
	Card turn;
	AnimationsManager cardAnimationManager;

	public TexassDefaultCardsManager(AnimationsManager cardAnimationManager, GameObject gameObject){
		this.gameObject = gameObject;
		this.cardAnimationManager = cardAnimationManager;
	}

	public void setDefaultCards(string flop1,string flop2,string flop3,string turn,string river){
		this.flop1 = new Card (flop1);
		this.flop2 = new Card (flop2);
		this.flop3 = new Card (flop3);
		this.turn = new Card (turn);
		this.river = new Card (river);
	}
	public void openFlopCards(){
		gameObject.transform.Find(GameConstant.UI_PATH_CARD_FLOP1).GetComponent<Image>().gameObject.SetActive(true);
		gameObject.transform.Find(GameConstant.UI_PATH_CARD_FLOP2).GetComponent<Image>().gameObject.SetActive(true);
		gameObject.transform.Find(GameConstant.UI_PATH_CARD_FLOP3).GetComponent<Image>().gameObject.SetActive(true);

		gameObject.transform.Find(GameConstant.UI_PATH_CARD_FLOP1).GetComponent<Image>().sprite = flop1.getCardSprite();
		gameObject.transform.Find(GameConstant.UI_PATH_CARD_FLOP2).GetComponent<Image>().sprite = flop2.getCardSprite();
		gameObject.transform.Find(GameConstant.UI_PATH_CARD_FLOP3).GetComponent<Image>().sprite = flop3.getCardSprite();
	}

	public void openTurnCards(){
		gameObject.transform.Find(GameConstant.UI_PATH_CARD_TURN).GetComponent<Image>().gameObject.SetActive(true);

		gameObject.transform.Find(GameConstant.UI_PATH_CARD_TURN).GetComponent<Image>().sprite = turn.getCardSprite();
	}
	public void openRiverCards(){
		gameObject.transform.Find(GameConstant.UI_PATH_CARD_RIVER).GetComponent<Image>().gameObject.SetActive(true);

		gameObject.transform.Find(GameConstant.UI_PATH_CARD_RIVER).GetComponent<Image>().sprite = river.getCardSprite();
	}
	public GameObject getCardObjectFromCardName(string cardName){
		if (flop1.getCardName ().Equals (cardName)) {
			return gameObject.transform.Find(GameConstant.UI_PATH_CARD_FLOP1).gameObject;
		}
		else if (flop2.getCardName ().Equals (cardName)) {
			return gameObject.transform.Find(GameConstant.UI_PATH_CARD_FLOP2).gameObject;
		}
		else if (flop3.getCardName ().Equals (cardName)) {
			return gameObject.transform.Find(GameConstant.UI_PATH_CARD_FLOP3).gameObject;
		}
		else if (turn.getCardName ().Equals (cardName)) {
			return gameObject.transform.Find(GameConstant.UI_PATH_CARD_TURN).gameObject;
		}
		else if (river.getCardName ().Equals (cardName)) {
			return gameObject.transform.Find(GameConstant.UI_PATH_CARD_RIVER).gameObject;
		}
		return null;
	}
	public IEnumerator resetCards(){

		gameObject.transform.Find (GameConstant.UI_PATH_CARD_FLOP1 ).gameObject.transform.position =
			gameObject.transform.Find (GameConstant.UI_PATH_CARD_FLOP1+ GameConstant.UI_PATH_POSITION).gameObject.transform.position;
		gameObject.transform.Find (GameConstant.UI_PATH_CARD_FLOP2 ).gameObject.transform.position =
			gameObject.transform.Find (GameConstant.UI_PATH_CARD_FLOP2+ GameConstant.UI_PATH_POSITION).gameObject.transform.position;
		gameObject.transform.Find (GameConstant.UI_PATH_CARD_FLOP3 ).gameObject.transform.position =
			gameObject.transform.Find (GameConstant.UI_PATH_CARD_FLOP3+ GameConstant.UI_PATH_POSITION).gameObject.transform.position;

		gameObject.transform.Find (GameConstant.UI_PATH_CARD_TURN ).gameObject.transform.position =
			gameObject.transform.Find (GameConstant.UI_PATH_CARD_TURN+ GameConstant.UI_PATH_POSITION).gameObject.transform.position;

		gameObject.transform.Find (GameConstant.UI_PATH_CARD_RIVER ).gameObject.transform.position =
			gameObject.transform.Find (GameConstant.UI_PATH_CARD_RIVER+ GameConstant.UI_PATH_POSITION).gameObject.transform.position;
		
		yield return new WaitForSeconds (2f);
	}

	public void closeAllCards(){
		gameObject.transform.Find(GameConstant.UI_PATH_CARD_FLOP1).GetComponent<Image>().gameObject.SetActive(false);
		gameObject.transform.Find(GameConstant.UI_PATH_CARD_FLOP2).GetComponent<Image>().gameObject.SetActive(false);
		gameObject.transform.Find(GameConstant.UI_PATH_CARD_FLOP3).GetComponent<Image>().gameObject.SetActive(false);
		gameObject.transform.Find(GameConstant.UI_PATH_CARD_TURN).GetComponent<Image>().gameObject.SetActive(false);
		gameObject.transform.Find(GameConstant.UI_PATH_CARD_RIVER).GetComponent<Image>().gameObject.SetActive(false);

	}
}

