using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TournamentWinnersDetailPanel : MonoBehaviour
{
	#region PUBLIC_VARIABLES

	public Image imgBackground;
	public Transform parentObject;

	public Loader loader;
	public Text txtMessage;

	[Space (10)]
	[Header ("Winner")]
	public TournamentWinnerObj winnerObjPrefab;
	[Space (10)]
	public Transform contentParent;
	//	public Text txtWinner1Name;
	//	public Text txtWinner1Rank;
	//	public Text txtWinner1PaidAmount;
	//	public Text txtWinner1Percentage;

	//	[Space(10)]
	//	[Header("Winner 2")]
	//	public Text txtWinner2Name;
	//	public Text txtWinner2Rank;
	//	public Text txtWinner2PaidAmount;
	//	public Text txtWinner2Percentage;
	//
	//	[Space(10)]
	//	[Header("Winner 3")]
	//	public Text txtWinner3Name;
	//	public Text txtWinner3Rank;
	//	public Text txtWinner3PaidAmount;
	//	public Text txtWinner3Percentage;

	[Space (10)]
	public string gameID;

	#endregion

	#region PRIVATE_VARIABLES

	private const float animationDuration = .1f;

	private List<TournamentWinnerObj > winnerObjList;

	#endregion

	#region UNITY_CALLBACKS

	// Use this for initialization
	void Start ()
	{
		
	}

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
		LobbyAPIManager.GetInstance ().GetTournamentAwardInfo (gameID);

		LobbyAPIManager.tournamentAwardInfoReceived += HandleTournamentAwardInfoReceived;
	}

	void OnDisable ()
	{
		imgBackground.CrossFadeAlpha (0f, 0f, true);
		parentObject.localScale = Vector3.zero;

		LobbyAPIManager.tournamentAwardInfoReceived -= HandleTournamentAwardInfoReceived;
	}

	#endregion

	#region DELEGATE_CALLBACKS

	private void HandleTournamentAwardInfoReceived (API_TournamentAwardInfo awardInfo)
	{
		winnerObjList = new List<TournamentWinnerObj> ();

		if (gameID.Equals (awardInfo.game_id)) {
			loader.gameObject.SetActive (false);

			if (awardInfo.awardsWinnerList.Count > 0) {
				foreach (API_TournaAwardPlayerInfo awardWinner in awardInfo.awardsWinnerList) {
					TournamentWinnerObj obj = Instantiate (winnerObjPrefab).GetComponent <TournamentWinnerObj> ();
					obj.transform.SetParent (contentParent, false);
					obj.gameObject.SetActive (true);

					obj.txtWinnerName.text = awardWinner.name;
					obj.txtWinnerPaidAmount.text = Utility.GetCommaSeperatedAmount (awardWinner.winning_amount);
					obj.txtWinnerPercentage.text = awardWinner.winning_percentage + "%";
					obj.txtWinnerRank.text = "" + awardWinner.rank;

					winnerObjList.Add (obj);
				}
			} else {
				txtMessage.text = APIConstants.MESSAGE_TOUR_WINNER_NOT_DECLARED;
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

	private void ResetData ()
	{
		if (winnerObjList != null)
			foreach (TournamentWinnerObj obj in winnerObjList)
				Destroy (obj.gameObject);

		winnerObjList = new List<TournamentWinnerObj> ();
	}

	#endregion

	#region COROUTINES

	#endregion
}