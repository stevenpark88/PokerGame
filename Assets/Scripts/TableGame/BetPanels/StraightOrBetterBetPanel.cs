using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StraightOrBetterBetPanel : MonoBehaviour
{
	public Text txtTitle;

	public Button btnYes;
	public Button btnNo;

	public double chipsToBet = 10;

	public void SetTitle(double amount)
	{
		chipsToBet = System.Math.Round(amount, 2);
		txtTitle.text = Constants.MESSAGE_STRAIGHT_OR_BETTER + Utility.GetCurrencyPrefix() + amount;

		if (chipsToBet * 3 <= GameManager.Instance.ownTablePlayer.buyinChips) {
			btnYes.interactable = true;
			btnYes.transform.GetChild (0).GetComponent<Text> ().CrossFadeAlpha (1f, 0f, true);
		} else {
			btnYes.interactable = false;
			btnYes.transform.GetChild (0).GetComponent<Text> ().CrossFadeAlpha (.5f, 0f, true);
		}

		if (chipsToBet * 2 <= GameManager.Instance.ownTablePlayer.buyinChips) {
			btnNo.interactable = true;
			btnNo.transform.GetChild (0).GetComponent<Text> ().CrossFadeAlpha (1f, 0f, true);
		} else {
			btnNo.interactable = false;
			btnNo.transform.GetChild (0).GetComponent<Text> ().CrossFadeAlpha (.5f, 0f, true);
		}

		if (GameManager.Instance.ownTablePlayer.buyinChips < amount &&
                !UIManager.Instance.isRegularTournament &&
            !UIManager.Instance.isSitNGoTournament)
			GameManager.Instance.btnAddChips.interactable = true;

		gameObject.SetActive (true);
	}

	public void OnYesButtonTap()
	{
		PlayerAction action = new PlayerAction ();
		action.Action = (int)PLAYER_ACTION.BET;
		action.Bet_Amount = chipsToBet * 3;
		action.IsBetOnStraight = true;
		action.Player_Name = NetworkManager.Instance.playerID;

		GameManager.Instance.straightAmount = chipsToBet;

		SendAction (action);
	}

	public void OnNoButtonTap()
	{
		PlayerAction action = new PlayerAction ();
		action.Action = (int)PLAYER_ACTION.BET;
		action.Bet_Amount = chipsToBet * 2;
		action.IsBetOnStraight = false;
		action.Player_Name = NetworkManager.Instance.playerID;

		SendAction (action);
	}

	private void SendAction(PlayerAction action)
	{
		NetworkManager.Instance.SendPlayerAction (action);

		if (GameManager.Instance.ownTablePlayer)
			GameManager.Instance.ownTablePlayer.HideTurnTimer ();

		SoundManager.Instance.PlayWooshSound (Camera.main.transform.position);

		GameManager.Instance.btnFold.interactable = false;
//		GameManager.Instance.btnAddChips.interactable = false;
		gameObject.SetActive (false);
	}
}