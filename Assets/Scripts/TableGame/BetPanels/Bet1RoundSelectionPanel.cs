using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class Bet1RoundSelectionPanel : MonoBehaviour
{
	public double chipsToBet = 10;

	public Button btnCheck;
	
	public Image imgSelected;
	public List<Button> betButtonsList;

	public void SetButtons (double amount)
	{
		chipsToBet = 0;

		for (int i = 0; i < betButtonsList.Count; i++) {
			double amt = amount * (i + 1);
			amt = System.Math.Round (amt, 2);

			Text t = betButtonsList [i].transform.GetChild (0).GetComponent<Text> ();
			t.text = Utility.GetCurrencyPrefix () + amt;
			betButtonsList [i].onClick.AddListener (() => OnBetButtonTap (t));

			if (amt <= GameManager.Instance.ownTablePlayer.buyinChips) {
				betButtonsList [i].interactable = true;
				t.CrossFadeAlpha (1f, 0f, true);
			} else {
				betButtonsList [i].interactable = false;
				t.CrossFadeAlpha (.5f, 0f, true);
			}
		}

		imgSelected.transform.position = btnCheck.transform.position;
		gameObject.SetActive (true);
	}

	public void OnCheckButtonTap (Button btn)
	{
		imgSelected.transform.position = btn.transform.position;
		chipsToBet = 0;
	}

	public void OnBetButtonTap (Text chips)
	{
		string amt;
		if (UIManager.Instance.isRealMoney)
			amt = chips.text.Replace (Utility.GetCurrencyPrefix (), "");
		else
			amt = chips.text;

		imgSelected.transform.position = chips.transform.position;
		chipsToBet = System.Math.Round (double.Parse (amt), 2);
	}

	public void OnOkButtonTap ()
	{
		PlayerAction action = new PlayerAction ();
		action.Action = chipsToBet == 0 ? (int)PLAYER_ACTION.CHECK : (int)PLAYER_ACTION.BET;
		action.Bet_Amount = chipsToBet;
		action.IsBetOnStraight = false;
		action.Player_Name = NetworkManager.Instance.playerID;

		NetworkManager.Instance.SendPlayerAction (action);
		GameManager.Instance.bet1Amount = chipsToBet;

		if (GameManager.Instance.ownTablePlayer)
			GameManager.Instance.ownTablePlayer.HideTurnTimer ();

		SoundManager.Instance.PlayWooshSound (Camera.main.transform.position);
		
		GameManager.Instance.btnFold.interactable = false;
//		GameManager.Instance.btnAddChips.interactable = false;
		gameObject.SetActive (false);
	}
}