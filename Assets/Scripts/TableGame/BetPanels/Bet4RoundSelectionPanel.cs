using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Bet4RoundSelectionPanel : MonoBehaviour
{
	public double chipsToBet = 10;

	public List<Button> betButtonsList;
	public Button btnOk;

	public void SetButtons(double amount)
	{
		chipsToBet = amount;

		for (int i = 0; i < betButtonsList.Count; i++) {
			double amt = amount * (i+1);
			amt = System.Math.Round (amt, 2);

			Text t = betButtonsList [i].transform.GetChild (0).GetComponent<Text> ();
			t.text = Utility.GetCurrencyPrefix() + amt;
			betButtonsList [i].onClick.AddListener(() => OnBetButtonTap(t));

			if (amt <= GameManager.Instance.ownTablePlayer.buyinChips) {
				betButtonsList [i].interactable = true;
				betButtonsList [i].transform.GetChild (0).GetComponent<Text> ().CrossFadeAlpha (1f, 0f, true);
			} else {
				betButtonsList [i].interactable = false;
				betButtonsList [i].transform.GetChild (0).GetComponent<Text> ().CrossFadeAlpha (.5f, 0f, true);
			}
		}

		if (GameManager.Instance.ownTablePlayer.buyinChips < amount) {
			btnOk.interactable = false;
            if (!UIManager.Instance.isSitNGoTournament &&
                !UIManager.Instance.isRegularTournament)
			GameManager.Instance.btnAddChips.interactable = true;
		} else {
			btnOk.interactable = true;
		}

		gameObject.SetActive (true);
	}

	public void OnBetButtonTap(Text chips)
	{
		string amt;
		if (UIManager.Instance.isRealMoney)
			amt = chips.text.Replace (Utility.GetCurrencyPrefix (), "");
		else
			amt = chips.text;
		
		chipsToBet = System.Math.Round(double.Parse(amt), 2);
	}

	public void OnOkButtonTap()
	{
		PlayerAction action = new PlayerAction ();
		action.Action = (int)PLAYER_ACTION.BET;
		action.Bet_Amount = chipsToBet;
		action.IsBetOnStraight = false;
		action.Player_Name = NetworkManager.Instance.playerID;

		NetworkManager.Instance.SendPlayerAction (action);
		GameManager.Instance.bet4Amount = chipsToBet;

		if (GameManager.Instance.ownTablePlayer)
			GameManager.Instance.ownTablePlayer.HideTurnTimer ();

		SoundManager.Instance.PlayWooshSound (Camera.main.transform.position);

		GameManager.Instance.btnFold.interactable = false;
//		GameManager.Instance.btnAddChips.interactable = false;
		gameObject.SetActive (false);
	}
}