using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameObjectManager 
{
	GameObject gameManagerGameObject;
	public GameObjectManager(GameObject gameObject){
		this.gameManagerGameObject = gameObject;
	}

	public GameObject getDefaultCardObject(){
		return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_DEFAULT_CARD).gameObject;
	}
	public Text getTotalTableBetAmountText(){
		return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_TOTAL_BET_AMOUNT).GetComponent<Text> ();
	}
	public GameObject getTableChipSetObject(){
		gameManagerGameObject.transform.Find (GameConstant.UI_PATH_TABLE_CHIP_SET_1).gameObject.transform.position = getTableChipSetPositionObject ().transform.position;
		return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_TABLE_CHIP_SET_1).gameObject;
	}
	public GameObject getTableChipSetPositionObject(){
		return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_TABLE_CHIP_SET_1+"Position").gameObject;
	}
	public Text getBetAmountText(){
		return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_BET_CHIP_DETAILS).GetComponent<Text> ();
	}
//	public Button getFoldButton(){
//		return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_BUTTON_FOLD).GetComponent<Button> ();
//	}
//	public Button getReBuyButton(){
//		return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_BUTTON_REBUY).GetComponent<Button> ();
//	}
//	public Button getCheckButton(){
//		return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_BUTTON_CHECK).GetComponent<Button> ();
//	}
//	public Button getRaiseButton(){
//		return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_BUTTON_RAISE).GetComponent<Button> ();
//	}
//	public Button getCallButton(){
//		return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_BUTTON_CALL).GetComponent<Button> ();
//	}
//	public Button getBetButton(){
//		return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_BUTTON_BET).GetComponent<Button> ();
//	}
//	public Button getAllInButton(){
//		return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_BUTTON_ALLIN).GetComponent<Button> ();
//	}

	public GameObject getRestartPannel(){
		return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_RESTART_PANNEL).gameObject;
	}
	public Text getRestartText(){
		return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_RESTART_TEXT).GetComponent<Text>();
	}

	public GameObject getPlayerChipsObject (int position)
	{
		
		switch (position) {
		case 0:
			return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_USER_1_CHIPS).gameObject;
		case 1:
			return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_USER_2_CHIPS).gameObject;
		case 2:
			return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_USER_3_CHIPS).gameObject;
		case 3:
			return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_USER_4_CHIPS).gameObject;
		case 4:
			return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_USER_5_CHIPS).gameObject;
		case 5:
			return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_USER_6_CHIPS).gameObject;
		case 6:
			return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_USER_7_CHIPS).gameObject;
		case 7:
			return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_USER_8_CHIPS).gameObject;
		case 8:
			return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_USER_9_CHIPS).gameObject;
		default:
			return null;
		}
	}
	
	public GameObject getPlayerGameObject (int position)
	{
		switch (position) {
		case 0:
			return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_PLAYER_1).gameObject;
		case 1:
			return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_PLAYER_2).gameObject;
		case 2:
			return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_PLAYER_3).gameObject;
		case 3:
			return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_PLAYER_4).gameObject;
		case 4:
			return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_PLAYER_5).gameObject;
		case 5:
			return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_PLAYER_6).gameObject;
		case 6:
			return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_PLAYER_7).gameObject;
		case 7:
			return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_PLAYER_8).gameObject;
		case 8:
			return gameManagerGameObject.transform.Find (GameConstant.UI_PATH_PLAYER_9).gameObject;
		default:
			return null;
		}
		
	}
}

