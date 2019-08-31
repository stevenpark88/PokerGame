using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TourRegConfirmationPanel : MonoBehaviour
{
	#region PUBLIC_VARIABLES

	public Text txtAvailBal;
	public Text txtFees;

	public Button btnConfirm;

	public Loader loader;

	public GameData gameData;

	#endregion

	#region PRIVATE_VARIABLES

	#endregion

	#region UNITY_CALLBACKS

	// Use this for initialization
	void OnEnable ()
	{
		bool isRealMoney = gameData.money_type.Equals ("real money");

		string balance = Utility.GetCommaSeperatedAmount (LoginScript.loggedInPlayer.balance_chips);
		if (isRealMoney)
			balance = Utility.GetRealMoneyAmount (LoginScript.loggedInPlayer.balance_cash);

		string totalFees = Utility.GetCommaSeperatedAmount (gameData.fee + gameData.entry_fee);
		if (isRealMoney)
			totalFees = Utility.GetRealMoneyAmount (gameData.fee + gameData.entry_fee);

		string buyin = Utility.GetCommaSeperatedAmount (gameData.buy_in);
		if (isRealMoney)
			buyin = Utility.GetRealMoneyAmount (gameData.buy_in);

		string entryFee = Utility.GetCommaSeperatedAmount (gameData.fee - gameData.buy_in);
		if (isRealMoney)
			entryFee = Utility.GetRealMoneyAmount (gameData.fee - gameData.buy_in);

		txtAvailBal.text = "<color=" + APIConstants.HEX_COLOR_RED_HEADER + ">" + balance + "</color>";
		txtFees.text = "<color=" + APIConstants.HEX_COLOR_RED_HEADER + ">" + totalFees + " (" + buyin + " Buy-in + " + entryFee + " Entry Fee)</color>";

		LobbyAPIManager.registerInTournamentDone += HandleRegisterInTournamentDone;
	}

	void OnDisable ()
	{
		LobbyAPIManager.registerInTournamentDone -= HandleRegisterInTournamentDone;
	}

	#endregion

	#region DELEGATE_CALLBACKS

	private void HandleRegisterInTournamentDone (bool success)
	{
		loader.gameObject.SetActive (false);

		if (success) {
			gameData.joined = true;
			gameObject.SetActive (false);
		}

		if (UIManager.Instance.lobbyPanel.sngTournamentPanel.gameObject.activeSelf) {
			UIManager.Instance.lobbyPanel.sngTournamentPanel.tournamentDetailPanel.UpdateTournamentDetailText (gameData.id);
		} else if (UIManager.Instance.lobbyPanel.regularTournamentPanel.gameObject.activeSelf) {
			UIManager.Instance.lobbyPanel.regularTournamentPanel.tournamentDetailPanel.UpdateTournamentDetailText (gameData.id);
		}
	}

	#endregion

	#region PUBLIC_METHODS

	public void OnConfirmButtonTap ()
	{
		bool isRealMoney = gameData.money_type.Equals ("real money");
		if (isRealMoney && LoginScript.loggedInPlayer.balance_cash < gameData.fee + gameData.entry_fee) {
			UIManager.Instance.DisplayMessagePanel ("Insufficient Real Money", "Not enough Real Money in your account to play this tournament.", null);
			return;
		} else if (!isRealMoney && LoginScript.loggedInPlayer.balance_chips < gameData.fee + gameData.entry_fee) {
			UIManager.Instance.DisplayMessagePanel ("Insufficient Chips", "You do not have enough chips to play in this tournament.", null);
			return;
		}

		loader.gameObject.SetActive (true);
		LobbyAPIManager.GetInstance ().RegisterInTournament (gameData.id, gameData.game_type);

		SoundManager.Instance.PlayButtonTapSound ();
	}

	public void OnCancelButtonTap ()
	{
		gameObject.SetActive (false);

		SoundManager.Instance.PlayButtonTapSound ();
	}

	#endregion

	#region PRIVATE_METHODS

	#endregion

	#region COROUTINES

	#endregion
}