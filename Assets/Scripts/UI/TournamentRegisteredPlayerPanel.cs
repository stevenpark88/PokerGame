using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TournamentRegisteredPlayerPanel : MonoBehaviour
{
	#region PUBLIC_VARIABLES

	public Image imgBackground;
	public Transform parentObject;

	public Loader loader;
	public Text txtMessage;

	public GameObject regPlayerPrefab;

	public RectTransform scrollViewContent;

	[Space (10)]
	public string gameID;
	public bool isRealMoney;

	#endregion

	#region PRIVATE_VARIABLES

	private const float animationDuration = .1f;

	private List<GameObject> regPlayerList;

	#endregion

	#region UNITY_CALLBACKS

	void OnEnable ()
	{
		txtMessage.text = "";
		ResetData ();

		imgBackground.CrossFadeAlpha (0f, 0f, true);

		imgBackground.CrossFadeAlpha (1f, animationDuration, true);
		Hashtable htScale = new Hashtable ();
		htScale.Add ("scale", Vector3.one);
		htScale.Add ("time", animationDuration);
		htScale.Add ("easetype", iTween.EaseType.easeOutCirc);
		iTween.ScaleTo (parentObject.gameObject, htScale);

		loader.gameObject.SetActive (true);
		LobbyAPIManager.GetInstance ().GetTournamentRegisteredPlayers (gameID);

		LobbyAPIManager.registeredPlayerInfoReceived += HandleRegisteredPlayersInfoReceived;
	}

	void OnDisable ()
	{
		imgBackground.CrossFadeAlpha (0f, 0f, true);
		parentObject.localScale = Vector3.zero;

		LobbyAPIManager.registeredPlayerInfoReceived -= HandleRegisteredPlayersInfoReceived;
	}

	#endregion

	#region DELEGATE_METHODS

	private void HandleRegisteredPlayersInfoReceived (API_TournamentRegPlayers regPlayers)
	{
		if (gameID.Equals (regPlayers.id)) {
			loader.gameObject.SetActive (false);

			if (regPlayers.playerList.Count == 0) {
				txtMessage.text = APIConstants.MESSAGE_TOUR_NO_REGD_PLAYERS;
			} else {
				regPlayerList = new List<GameObject> ();
				int i = 0;
				foreach (SingleGameInfoPlayer sgip in regPlayers.playerList) {
					GameObject obj = Instantiate (regPlayerPrefab) as GameObject;
					obj.transform.SetParent (scrollViewContent);
					obj.transform.localScale = Vector3.one;

					RegdPlayerObject rpo = obj.GetComponent<RegdPlayerObject> ();
					rpo.txtName.text = sgip.name;

					string buyIn = Utility.GetCommaSeperatedAmount (sgip.buy_in);
					if (isRealMoney)
						buyIn = Utility.GetRealMoneyAmount (sgip.buy_in);
					rpo.txtBuyin.text = buyIn;

					string balance = Utility.GetCommaSeperatedAmount (sgip.balance_chips);
					if (isRealMoney)
						balance = Utility.GetRealMoneyAmount (sgip.balance_cash);
					rpo.txtTotalChips.text = balance;

					rpo.DownloadCountryFlag (sgip.flagUrl);

					Color bgColor = new Color ();
					bgColor.a = ++i % 2 == 0 ? 0f : .25f;
					rpo.imgBackground.color = bgColor;

					regPlayerList.Add (obj);
				}

				SetContentHeight (regPlayers.playerList.Count);
			}
		}
	}

	#endregion

	#region PUBLIC_METHODS

	public void OnCloseButtonTap ()
	{
		gameObject.SetActive (false);

		SoundManager.Instance.PlayButtonTapSound ();
	}

	#endregion

	#region PRIVATE_METHODS

	private void SetContentHeight (int totalObjects)
	{
		scrollViewContent.sizeDelta = new Vector2 (scrollViewContent.sizeDelta.x, totalObjects * 50f);
	}

	private void ResetData ()
	{
		if (regPlayerList != null)
			foreach (GameObject go in regPlayerList)
				Destroy (go);

		regPlayerList = new List<GameObject> ();
	}

	#endregion

	#region COROUTINES

	#endregion
}