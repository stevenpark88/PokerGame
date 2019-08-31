using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BottomBarPanel : MonoBehaviour
{
	public List<Button> buttonsList;

	public Sprite selectedButtonSprite;
	public Sprite deselectedButtonSprite;

	// Use this for initialization
	void Start ()
	{
		
	}

	/// <summary>
	/// Raises the button tap event.
	/// </summary>
	public void OnButtonTap(Button sender)
	{
		foreach (Button btn in buttonsList) {
			if (btn == sender) {
				btn.image.sprite = selectedButtonSprite;
				btn.interactable = false;
			} else {
				btn.image.sprite = deselectedButtonSprite;
				btn.interactable = true;
			}
		}
	}

	public void OnAccountButtonTap()
	{
//		UIManager.Instance.accountInfoPanel.gameObject.SetActive (true);
//		UIManager.Instance.roomsPanel.gameObject.SetActive (false);
//		UIManager.Instance.settingsPanel.gameObject.SetActive (false);
//		UIManager.Instance.newsPanel.gameObject.SetActive (false);
	}

	public void OnLobbyButtonTap()
	{
//		UIManager.Instance.accountInfoPanel.gameObject.SetActive (false);
//		UIManager.Instance.roomsPanel.gameObject.SetActive (true);
//		UIManager.Instance.settingsPanel.gameObject.SetActive (false);
//		UIManager.Instance.newsPanel.gameObject.SetActive (false);
	}

	public void OnAddChipsButtonTap()
	{
		
	}

	public void OnLeaveTableButtonTap()
	{
		
	}

	public void OnSettingsButtonTap()
	{
//		UIManager.Instance.accountInfoPanel.gameObject.SetActive (false);
//		UIManager.Instance.roomsPanel.gameObject.SetActive (false);
//		UIManager.Instance.settingsPanel.gameObject.SetActive (true);
//		UIManager.Instance.newsPanel.gameObject.SetActive (false);
	}

	public void OnHelpButtonTap()
	{
		
	}

	public void OnNewsButtonTap()
	{
//		UIManager.Instance.accountInfoPanel.gameObject.SetActive (false);
//		UIManager.Instance.roomsPanel.gameObject.SetActive (false);
//		UIManager.Instance.settingsPanel.gameObject.SetActive (false);
//		UIManager.Instance.newsPanel.gameObject.SetActive (true);
	}

	public void OnClubButtonTap()
	{
		
	}

	public void OnHowToPlayButtonTap()
	{
		
	}

	public void OnOpenCloseButtonTap()
	{
		
	}
}