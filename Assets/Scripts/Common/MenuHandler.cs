using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using com.shephertz.app42.gaming.multiplayer.client;

public class MenuHandler : MonoBehaviour 
{
	public GameObject roomListPanel;
	public GameObject createRoomPanel;
	public GameObject mainMenuPanel;
	public GameObject gamePlayPanel;
	public GameObject userAccountInfoPanel;
	public GameObject settingsPanel;
	public GameObject newsPanel;
	public Sprite btnNormalSprite;
	public Sprite selectedBtnSprite;
	public Button[] btnArray;

	public void OnAccountBtnClick()
	{

		userAccountInfoPanel.SetActive (true);

		GameObject.Find ("AccountButton").GetComponent<Button> ().image.sprite = selectedBtnSprite;

	}

	public void OnLobbyBtnClick()
	{

		//GameObject.Find ("LobbyButton").GetComponent<Button> ().image.sprite = selectedBtnSprite;
	}

	public void OnAddChipstBtnClick()
	{
		//GameObject.Find ("AddChipsButton").GetComponent<Button> ().image.sprite = selectedBtnSprite;
	}

	public void OnLeaveTableBtnClick()
	{
		
	}

	public void OnSettingBtnClick()
	{
		settingsPanel.SetActive (true);
		GameObject.Find ("SettingButton").GetComponent<Button> ().image.sprite = selectedBtnSprite;
	}

	public void OnHelptBtnClick()
	{
		
	}

	public void OnNewsBtnClick()
	{
		newsPanel.SetActive (true);
		GameObject.Find ("NewsButton").GetComponent<Button> ().image.sprite = selectedBtnSprite;
	}

	public void OnClubBtnClick()
	{
		
	}

	public void OnHowToPlayBtnClick()
	{
		
	}

	public void Exit()
	{
		//WarpClient.GetInstance ().Disconnect ();
		userAccountInfoPanel.SetActive (false);
		GameObject.Find ("AccountButton").GetComponent<Button> ().image.sprite = btnNormalSprite;
	}

	public void SettingsExit()
	{
		settingsPanel.SetActive (false);
		GameObject.Find ("SettingButton").GetComponent<Button> ().image.sprite = btnNormalSprite;
	}

	public void NewsExit()
	{
		newsPanel.SetActive (false);
		GameObject.Find ("NewsButton").GetComponent<Button> ().image.sprite = btnNormalSprite;
	}

	public void OnHomeBtnClick()
	{
		mainMenuPanel.SetActive (true);
		if (roomListPanel.activeSelf) 
		{
			roomListPanel.SetActive(false);
		}
		if (gamePlayPanel.activeSelf) 
		{
			gamePlayPanel.SetActive(false);
		}
	}
	
}
