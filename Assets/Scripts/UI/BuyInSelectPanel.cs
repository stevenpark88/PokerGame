using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BuyInSelectPanel : MonoBehaviour
{
	#region PUBLIC_VARIABLES

	public Image imgBackground;
	public Transform parentObject;

	public Loader loader;
	public Text txtMessage;

	public GameData gameData;

	public Text txtBuyinAmount;
	public Button btnSelectBuyin;
	public Slider sliderBuyin;

	#endregion

	#region PRIVATE_METHODS

	private const float animationDuration = .5f;

	#endregion

	#region UNITY_CALLBACKS

	// Use this for initialization
	void OnEnable ()
	{
		LobbyAPIManager.buyInAmountSetDone += HandleBuyInAmountSetDone;

//		if (gameData.poker_type.Equals (APIConstants.KEY_TABLE)) {
//			loader.gameObject.SetActive (true);
//			LobbyAPIManager.GetInstance ().SetBuyin (gameData.id, (long)LoginScript.loggedInPlayer.balance_chips);
//		}
//		else
//		{
		txtMessage.text = "";
//
//			imgBackground.CrossFadeAlpha (0f, 0f, true);
//
//			imgBackground.CrossFadeAlpha (1f, animationDuration / 1.5f, true);
//			Hashtable htScale = new Hashtable ();
//			htScale.Add ("scale", Vector3.one);
//			htScale.Add ("time", animationDuration);
//			htScale.Add ("easetype", iTween.EaseType.easeOutCirc);
//			iTween.ScaleTo (parentObject.gameObject, htScale);

		parentObject.localScale = Vector3.one;

		btnSelectBuyin.interactable = true;
//		}
	}

	void OnDisable ()
	{
		LobbyAPIManager.buyInAmountSetDone -= HandleBuyInAmountSetDone;

//		imgBackground.CrossFadeAlpha (0f, 0f, true);
		parentObject.localScale = Vector3.zero;
	}

	#endregion

	#region DELEGATE_CALLBACKS

	private void HandleBuyInAmountSetDone (bool success)
	{
		loader.gameObject.SetActive (false);

		if (success) {
			WebGLLogin loginDetails = new WebGLLogin ();
			//			loginDetails.buyin = gameData.poker_type.Equals ("table") ? LoginScript.loggedInPlayer.balance_chips.ToString () : "1000";
			loginDetails.buyin = gameData.buy_in.ToString ();
			loginDetails.GameRoomID = gameData.id;
			DebugLog.Log ("CD-------> GameType : " + gameData.poker_type);
			loginDetails.GameType = gameData.poker_type.ToUpper ();
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
			DebugLog.Log ("CD-------> loginDetails : " + JsonUtility.ToJson (loginDetails));
		} else {
			btnSelectBuyin.interactable = true;

			txtMessage.text = "Something went wrong. Try again.";
		}
	}

	#endregion

	#region PUBLIC_METHODS

	public void OnBuyinAmountValueChanged (Slider scrollBuyin)
	{
		if (!UIManager.Instance.isRealMoney)
			sliderBuyin.value = scrollBuyin.value.RoundToTen ();

		txtBuyinAmount.text = Utility.GetCommaSeperatedAmount (scrollBuyin.value);
	}

	public void SetBuyinButtonTap ()
	{
		btnSelectBuyin.interactable = false;
		txtMessage.text = "";

		double buyInAmount = (double)sliderBuyin.value;

		loader.gameObject.SetActive (true);
		LobbyAPIManager.GetInstance ().SetBuyin (gameData.id, buyInAmount);
	}

	public void OnCloseButtonTap ()
	{
		UIManager.Instance.lobbyPanel.cashGamePanel.cashGameDetailPanel.btnPlay.interactable = true;
		gameObject.SetActive (false);
	}

	#endregion

	#region PRIVATE_METHODS

	#endregion

	#region COROUTINES

	#endregion
}