using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class WADefaultCardsManager
{
	GameObject gameObject;
	Card firstFlop1;
	Card firstFlop2;
	Card secondFlop1;
	Card secondFlop2;
	Card thirdFlop1;
	Card thirdFlop2;
	AnimationsManager cardAnimationManager;
	PlayerBean dealerPlayer;
	GameObject cardDesk;
	GameObject cardDeskPosition;

	public WADefaultCardsManager (AnimationsManager cardAnimationManager, GameObject gameObject)
	{
		this.gameObject = gameObject;
		this.cardAnimationManager = cardAnimationManager;

	}

	public void setDealerPlayer (PlayerBean dealerPlayer)
	{
		this.dealerPlayer = dealerPlayer;
	}

	public void setDefaultCards (string firstFlop1, string firstFlop2, string secondFlop1, string secondFlop2, string thirdFlop1, string thirdFlop2)
	{
		this.firstFlop1 = new Card (firstFlop1);
		this.firstFlop2 = new Card (firstFlop2);
		this.secondFlop1 = new Card (secondFlop1);
		this.secondFlop2 = new Card (secondFlop2);
		this.thirdFlop1 = new Card (thirdFlop1);
		this.thirdFlop2 = new Card (thirdFlop2);
	}

	public IEnumerator openFirstFlopCards ()
	{

		if (!gameObject.transform.Find (GameConstant.UI_PATH_FIRST_FLOP).gameObject.activeSelf) {
			cardDesk = dealerPlayer.getCardDeskObject ();
			cardDeskPosition = dealerPlayer.getCardDeskPositionObject ();
			cardDeskPosition.SetActive (true);
			cardDesk.SetActive (true);
			gameObject.transform.Find (GameConstant.UI_PATH_FIRST_FLOP).gameObject.SetActive (true);

			Image card1 = gameObject.transform.Find (GameConstant.UI_PATH_FIRST_FLOP + GameConstant.UI_PATH_CARD_1).GetComponent<Image> ();
			Image card2 = gameObject.transform.Find (GameConstant.UI_PATH_FIRST_FLOP + GameConstant.UI_PATH_CARD_2).GetComponent<Image> ();
			card1.gameObject.SetActive (false);
			card2.gameObject.SetActive (false);

			cardDesk.transform.position = cardDeskPosition.transform.position;
			cardAnimationManager.MoveCardsObject (cardDesk, card1.gameObject);
			yield return new WaitForSeconds (GameConstant.ANIM_CARD_TIME);
			card1.gameObject.SetActive (true);

			cardDesk.transform.position = cardDeskPosition.transform.position;
			cardAnimationManager.MoveCardsObject (cardDesk, card2.gameObject);
			yield return new WaitForSeconds (GameConstant.ANIM_CARD_TIME);
			card2.gameObject.SetActive (true);

			card1.sprite = firstFlop1.getCardSprite ();
			card2.sprite = firstFlop2.getCardSprite ();

			cardDesk.SetActive (false);
			cardDeskPosition.SetActive (false);
		}

	}

	public IEnumerator openSecondFlopCards ()
	{
		if (!gameObject.transform.Find (GameConstant.UI_PATH_SECOND_FLOP).gameObject.activeSelf) {
			cardDesk = dealerPlayer.getCardDeskObject ();
			cardDeskPosition = dealerPlayer.getCardDeskPositionObject ();
			cardDeskPosition.SetActive (true);
			cardDesk.SetActive (true);
			gameObject.transform.Find (GameConstant.UI_PATH_SECOND_FLOP).gameObject.SetActive (true);

			Image card1 = gameObject.transform.Find (GameConstant.UI_PATH_SECOND_FLOP + GameConstant.UI_PATH_CARD_1).GetComponent<Image> ();
			Image card2 = gameObject.transform.Find (GameConstant.UI_PATH_SECOND_FLOP + GameConstant.UI_PATH_CARD_2).GetComponent<Image> ();
			card1.gameObject.SetActive (false);
			card2.gameObject.SetActive (false);

			cardDesk.transform.position = cardDeskPosition.transform.position;
			cardAnimationManager.MoveCardsObject (cardDesk, card1.gameObject);
			yield return new WaitForSeconds (GameConstant.ANIM_CARD_TIME);
			card1.gameObject.SetActive (true);

			cardDesk.transform.position = cardDeskPosition.transform.position;
			cardAnimationManager.MoveCardsObject (cardDesk, card2.gameObject);
			yield return new WaitForSeconds (GameConstant.ANIM_CARD_TIME);
			card2.gameObject.SetActive (true);

			card1.sprite = secondFlop1.getCardSprite ();
			card2.sprite = secondFlop2.getCardSprite ();

			cardDesk.SetActive (false);
			cardDeskPosition.SetActive (false);
		}
//		gameObject.transform.Find (GameConstant.UI_PATH_SECOND_FLOP + GameConstant.UI_PATH_CARD_1).GetComponent<Image> ().sprite = secondFlop1.getCardSprite ();
//		gameObject.transform.Find (GameConstant.UI_PATH_SECOND_FLOP + GameConstant.UI_PATH_CARD_2).GetComponent<Image> ().sprite = secondFlop2.getCardSprite ();
	}

	public IEnumerator openThirdCards ()
	{
		if (!gameObject.transform.Find (GameConstant.UI_PATH_THIRD_FLOP).gameObject.activeSelf) {
			cardDesk = dealerPlayer.getCardDeskObject ();
			cardDeskPosition = dealerPlayer.getCardDeskPositionObject ();
			cardDeskPosition.SetActive (true);
			cardDesk.SetActive (true);
			gameObject.transform.Find (GameConstant.UI_PATH_THIRD_FLOP).gameObject.SetActive (true);

			Image card1 = gameObject.transform.Find (GameConstant.UI_PATH_THIRD_FLOP + GameConstant.UI_PATH_CARD_1).GetComponent<Image> ();
			Image card2 = gameObject.transform.Find (GameConstant.UI_PATH_THIRD_FLOP + GameConstant.UI_PATH_CARD_2).GetComponent<Image> ();
			card1.gameObject.SetActive (false);
			card2.gameObject.SetActive (false);

			cardDesk.transform.position = cardDeskPosition.transform.position;
			cardAnimationManager.MoveCardsObject (cardDesk, card1.gameObject);
			yield return new WaitForSeconds (GameConstant.ANIM_CARD_TIME);
			card1.gameObject.SetActive (true);

			cardDesk.transform.position = cardDeskPosition.transform.position;
			cardAnimationManager.MoveCardsObject (cardDesk, card2.gameObject);
			yield return new WaitForSeconds (GameConstant.ANIM_CARD_TIME);
			card2.gameObject.SetActive (true);

			card1.sprite = thirdFlop1.getCardSprite ();
			card2.sprite = thirdFlop2.getCardSprite ();

			cardDesk.SetActive (false);
			cardDeskPosition.SetActive (false);
		}
//		gameObject.transform.Find (GameConstant.UI_PATH_THIRD_FLOP + GameConstant.UI_PATH_CARD_1).GetComponent<Image> ().sprite = thirdFlop1.getCardSprite ();
//		gameObject.transform.Find (GameConstant.UI_PATH_THIRD_FLOP + GameConstant.UI_PATH_CARD_2).GetComponent<Image> ().sprite = thirdFlop2.getCardSprite ();
	}

	public GameObject getCardObjectFromCardName (string cardName)
	{
		gameObject.transform.Find (GameConstant.UI_PATH_FIRST_FLOP).gameObject.SetActive (true);

		if (firstFlop1.getCardName ().Equals (cardName)) {
			return gameObject.transform.Find (GameConstant.UI_PATH_FIRST_FLOP + GameConstant.UI_PATH_CARD_1).gameObject;
		} else if (firstFlop2.getCardName ().Equals (cardName)) {
			return gameObject.transform.Find (GameConstant.UI_PATH_FIRST_FLOP + GameConstant.UI_PATH_CARD_2).gameObject;
		} else if (secondFlop1.getCardName ().Equals (cardName)) {
			return gameObject.transform.Find (GameConstant.UI_PATH_SECOND_FLOP + GameConstant.UI_PATH_CARD_1).gameObject;
		} else if (thirdFlop1.getCardName ().Equals (cardName)) {
			return gameObject.transform.Find (GameConstant.UI_PATH_THIRD_FLOP + GameConstant.UI_PATH_CARD_1).gameObject;
		} else if (secondFlop2.getCardName ().Equals (cardName)) {
			return gameObject.transform.Find (GameConstant.UI_PATH_SECOND_FLOP + GameConstant.UI_PATH_CARD_2).gameObject;
		} else if (thirdFlop2.getCardName ().Equals (cardName)) {
			return gameObject.transform.Find (GameConstant.UI_PATH_THIRD_FLOP + GameConstant.UI_PATH_CARD_2).gameObject;
		}
		return null;
	}

	public IEnumerator resetCards ()
	{

		gameObject.transform.Find (GameConstant.UI_PATH_FIRST_FLOP + GameConstant.UI_PATH_CARD_1).gameObject.transform.position =
			gameObject.transform.Find (GameConstant.UI_PATH_FIRST_FLOP + GameConstant.UI_PATH_CARD_1 + GameConstant.UI_PATH_POSITION).gameObject.transform.position;
		gameObject.transform.Find (GameConstant.UI_PATH_FIRST_FLOP + GameConstant.UI_PATH_CARD_2).gameObject.transform.position =
			gameObject.transform.Find (GameConstant.UI_PATH_FIRST_FLOP + GameConstant.UI_PATH_CARD_2 + GameConstant.UI_PATH_POSITION).gameObject.transform.position;

		gameObject.transform.Find (GameConstant.UI_PATH_SECOND_FLOP + GameConstant.UI_PATH_CARD_1).gameObject.transform.position =
			gameObject.transform.Find (GameConstant.UI_PATH_SECOND_FLOP + GameConstant.UI_PATH_CARD_1 + GameConstant.UI_PATH_POSITION).gameObject.transform.position;
		gameObject.transform.Find (GameConstant.UI_PATH_SECOND_FLOP + GameConstant.UI_PATH_CARD_2).gameObject.transform.position =
			gameObject.transform.Find (GameConstant.UI_PATH_SECOND_FLOP + GameConstant.UI_PATH_CARD_2 + GameConstant.UI_PATH_POSITION).gameObject.transform.position;

		gameObject.transform.Find (GameConstant.UI_PATH_THIRD_FLOP + GameConstant.UI_PATH_CARD_1).gameObject.transform.position =
			gameObject.transform.Find (GameConstant.UI_PATH_THIRD_FLOP + GameConstant.UI_PATH_CARD_1 + GameConstant.UI_PATH_POSITION).gameObject.transform.position;
		gameObject.transform.Find (GameConstant.UI_PATH_THIRD_FLOP + GameConstant.UI_PATH_CARD_2).gameObject.transform.position =
			gameObject.transform.Find (GameConstant.UI_PATH_THIRD_FLOP + GameConstant.UI_PATH_CARD_2 + GameConstant.UI_PATH_POSITION).gameObject.transform.position;
		yield return new WaitForSeconds (2f);
	}

	public void closeAllCards ()
	{
		gameObject.transform.Find (GameConstant.UI_PATH_FIRST_FLOP).gameObject.SetActive (false);
		gameObject.transform.Find (GameConstant.UI_PATH_SECOND_FLOP).gameObject.SetActive (false);
		gameObject.transform.Find (GameConstant.UI_PATH_THIRD_FLOP).gameObject.SetActive (false);

	}
}


