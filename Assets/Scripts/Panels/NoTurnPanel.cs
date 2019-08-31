using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NoTurnPanel : MonoBehaviour
{
	public Toggle toggleCheck;
	public Toggle toggleCall;
	public Toggle toggleCallAny;
	public Toggle toggleFold;

	public CanvasGroup cgCheck;
	public CanvasGroup cgCall;
	public CanvasGroup cgCallAny;
	public CanvasGroup cgFold;

	public Text txtCall;

	public double callAmount;

	void OnEnable()
	{
		NetworkManager.onRoundComplete += HandleOnRoundComplete;
	}

	void OnDisable()
	{
		NetworkManager.onRoundComplete -= HandleOnRoundComplete;

		ResetAllCheckboxes ();
	}

	#region DELEGATE_CALLBACKS
	private void HandleOnRoundComplete(string sender, string roundInfo)
	{
		ResetAllCheckboxes ();
	}
	#endregion

	public OFF_TURN_ACTION GetSelectedAction()
	{
		if (toggleCall.isOn)
			return OFF_TURN_ACTION.CALL;
		else if (toggleCallAny.isOn)
			return OFF_TURN_ACTION.CALL_ANY;
		else if (toggleCheck.isOn)
			return OFF_TURN_ACTION.CHECK;
		else if (toggleFold.isOn)
			return OFF_TURN_ACTION.FOLD;

		return OFF_TURN_ACTION.NONE;
	}

	public void DisplayCheckboxes()
	{
//		toggleCall.isOn = false;
//		toggleCallAny.isOn = false;
//		toggleCheck.isOn = false;
//		toggleFold.isOn = false;

		double minBetAmount = 0;
		if (UIManager.Instance.gameType == POKER_GAME_TYPE.TEXAS) {
			minBetAmount = RoundController.GetInstance ().GetMinBetAmountInCurrentRound () - TexassGame.Instance.ownTexassPlayer.betAmount;
			minBetAmount = minBetAmount > TexassGame.Instance.ownTexassPlayer.buyInAmount ? TexassGame.Instance.ownTexassPlayer.buyInAmount : minBetAmount;
		} else if (UIManager.Instance.gameType == POKER_GAME_TYPE.WHOOPASS) {
			minBetAmount = RoundController.GetInstance ().GetMinBetAmountInCurrentRound () - WhoopAssGame.Instance.ownWhoopAssPlayer.betAmount;
			minBetAmount = minBetAmount > WhoopAssGame.Instance.ownWhoopAssPlayer.buyInAmount ? WhoopAssGame.Instance.ownWhoopAssPlayer.buyInAmount : minBetAmount;
		}

		if (minBetAmount <= 0) {
			cgCheck.interactable = true;
		} else {
			cgCheck.interactable = false;

			txtCall.text = "   Call " + Utility.GetAmount (minBetAmount);
			cgCall.interactable = true;
			cgCallAny.interactable = true;
		}

		cgFold.interactable = true;
	}

	private void ResetAllCheckboxes()
	{
		toggleCall.isOn = false;
		toggleCallAny.isOn = false;
		toggleCheck.isOn = false;
		toggleFold.isOn = false;
	}
}

public enum OFF_TURN_ACTION
{
	CHECK,
	CALL,
	CALL_ANY,
	FOLD,
	NONE
}