using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class AnteAndBlindBetPanel : MonoBehaviour
{
	public Button initialSelectedButton;
	public Image imgSelected;

	public Button btnOk;

	public List<Button> betButtonsList;
	public List<double> betAmountList = new List<double> () {
		5,
		10,
		20,
		50,
		100,
		500,
		1000,
		5000,
		10000,
		50000,
		100000,
		500000,
		1000000
	};
	public List<double> realMoneyBetAmountList = new List<double> () {
		.05,
		.10,
		.20,
		.50,
		1,
		5,
		10,
		50,
		100,
		500,
		1000,
		5000,
		10000
	};

	public double chipsToBet = 5;

	// Use this for initialization
	void OnEnable ()
	{
		chipsToBet = 5;
		imgSelected.transform.position = initialSelectedButton.transform.position;

		if (UIManager.Instance.isRealMoney) {
			chipsToBet = .05;
			SetRealMoneyButtons ();
		} else
			SetPlayMoneyButtons ();

		double newAmount = chipsToBet * 12.0;
		newAmount = System.Math.Round (newAmount, 2);

		if (newAmount <= GameManager.Instance.ownTablePlayer.buyinChips) {
			btnOk.interactable = true;
		} else {
			btnOk.interactable = false;
			if (!UIManager.Instance.isSitNGoTournament &&
			    !UIManager.Instance.isRegularTournament)
				GameManager.Instance.btnAddChips.interactable = true;
		}
	}

	void OnDisable ()
	{
		HideAllButtons ();
	}

	private void SetPlayMoneyButtons ()
	{
		for (int i = 0; i < betAmountList.Count; i++) {
			Button btn = betButtonsList [i];
			btn.gameObject.SetActive (true);
			Text t = btn.transform.GetChild (0).GetComponent<Text> ();
			t.text = Utility.GetCurrencyPrefix () + betAmountList [i];
			btn.onClick.AddListener (() => OnBetButtonTap (t));

			if (betAmountList [i] * 12 <= GameManager.Instance.ownTablePlayer.buyinChips) {
				btn.interactable = true;
				btn.transform.GetChild (0).GetComponent<Text> ().CrossFadeAlpha (1f, 0f, true);
			} else {
				btn.interactable = false;
				btn.transform.GetChild (0).GetComponent<Text> ().CrossFadeAlpha (.5f, 0f, true);
			}
		}
	}

	private void SetRealMoneyButtons ()
	{
		for (int i = 0; i < realMoneyBetAmountList.Count; i++) {
			Button btn = betButtonsList [i];
			btn.gameObject.SetActive (true);
			Text t = btn.transform.GetChild (0).GetComponent<Text> ();
			t.text = Utility.GetCurrencyPrefix () + realMoneyBetAmountList [i];
			btn.onClick.AddListener (() => OnBetButtonTap (t));

			double newAmount = realMoneyBetAmountList [i] * 12.0;
			newAmount = System.Math.Round (newAmount, 2);

			if (newAmount <= GameManager.Instance.ownTablePlayer.buyinChips) {
				btn.interactable = true;
				t.CrossFadeAlpha (1f, 0f, true);
			} else {
				btn.interactable = false;
				t.CrossFadeAlpha (.5f, 0f, true);
			}
		}
	}

	private void HideAllButtons ()
	{
		foreach (Button btn in betButtonsList)
			btn.gameObject.SetActive (false);
	}

	public void OnBetButtonTap (Text chips)
	{
		string amt;
		;
		if (UIManager.Instance.isRealMoney)
			amt = chips.text.Replace (Utility.GetCurrencyPrefix (), "");
		else
			amt = chips.text;

		imgSelected.transform.position = chips.transform.position;
		chipsToBet = double.Parse (double.Parse (amt).ToString ("##.##"));
	}

	public void OnOkButtonTap ()
	{
		GameManager.Instance.anteBetAmount = chipsToBet;

		PlayerAction action = new PlayerAction ();
		action.Action = (int)PLAYER_ACTION.BET;
		action.Bet_Amount = chipsToBet;
		action.IsBetOnStraight = false;
		action.Player_Name = NetworkManager.Instance.playerID;

		GameManager.Instance.anteAmount = GameManager.Instance.blindAmount = chipsToBet;
		UIManager.Instance.straightOrBetterBetPanel.SetTitle (chipsToBet);
		NetworkManager.Instance.SendRequestToServer (Constants.REQUEST_FOR_ANTE_AND_BLIND);

		SoundManager.Instance.PlayWooshSound (Camera.main.transform.position);

		gameObject.SetActive (false);
	}
}