using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedirectToTournamentPanel : MonoBehaviour
{
	#region PUBLIC_VARIABLES

	public Text txtTitle;
	public Text txtMessage;

	public Text txtTimer;

	public string gameId;

	#endregion

	#region PRIVATE_VARIABLES

	private int timer;

	#endregion

	#region UNITY_CALLBACKS

	void OnEnable ()
	{
		timer = 10;
		txtTimer.text = "" + timer;

		StartCoroutine ("StartTimer");
	}

	void OnDisable ()
	{
		StopCoroutine ("StartTimer");
	}

	#endregion

	#region DELEGATE_CALLBACKS

	#endregion

	#region PUBLIC_METHODS

	public void DisplayTournamentStarting (string gameId, string title, string message)
	{
		this.gameId = gameId;
		txtTitle.text = title;
		txtMessage.text = message;

		this.gameObject.SetActive (true);
	}

	public void OnPlayButtonTap ()
	{
		UIManager.Instance.backConfirmationPanel.OnYesButtonTap ();

		UIManager.Instance.DisplayLoader (Constants.MessageStartingTournament);
		LobbyAPIManager.GetInstance ().GetTournamentInfo (gameId, (www) => {
			UIManager.Instance.HideLoader ();
			this.gameObject.SetActive (false);

			if (!string.IsNullOrEmpty (www.error)) {
				Debug.LogError ("ERROR  : " + www.error);
				return;
			}

			try {
				GameData gameData = JsonUtility.FromJson <GameData> (www.text);
				
				WebGLLogin loginDetails = new WebGLLogin ();
				loginDetails.buyin = gameData.poker_type.Equals ("table") ? LoginScript.loggedInPlayer.balance_chips.ToString () : "1000";
				loginDetails.GameRoomID = gameData.id;
				
				loginDetails.GameType = gameData.poker_type.ToUpper () + "_" + (gameData.game_type.Equals (APIConstants.SNG_TOUR_GAME_TYPE) ? "SNG" : "REGULAR");
				loginDetails.game_id = gameData.id;
				loginDetails.game_type = gameData.money_type;
				loginDetails.isLimit = gameData.limit.Equals ("yes") ? "1" : "0";
				loginDetails.speed = gameData.game_speed;
				loginDetails.stake = gameData.small_blind + "/" + gameData.small_blind * 2;
				loginDetails.user_name = LoginScript.loggedInPlayer.name;
				loginDetails.max_player = gameData.maximum_players.ToString ();
				loginDetails.real_money = LoginScript.loggedInPlayer.balance_cash.ToString ();
				loginDetails.play_money = LoginScript.loggedInPlayer.balance_chips.ToString ();
				
				UIManager.Instance.loginPanel.StartMobileGame (JsonUtility.ToJson (loginDetails));
			} catch (System.Exception ex) {
				UIManager.Instance.DisplayMessagePanel ("Error", Constants.MESSAGE_SOMETHING_WENT_WRONG);
			}
		});

		SoundManager.Instance.PlayButtonTapSound ();
	}

	public void OnCancelButtonTap ()
	{
		this.gameObject.SetActive (false);
	}

	#endregion

	#region PRIVATE_METHODS

	#endregion

	#region COROUTINES

	private IEnumerator StartTimer ()
	{
		while (timer > 0) {
			yield return new WaitForSeconds (1f);

			txtTimer.text = "" + --timer;
		}

		OnPlayButtonTap ();
	}

	#endregion
}