using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TournamentWinnerPanel : MonoBehaviour
{
	public GameObject winnerObjPrefab;

	public ScrollRect winnerScrollView;
	public RectTransform scrollViewContent;

	public int totalWinnerPlayers = 10;

	public static bool isTournamentWinnersDeclared;

	public static float gameWinnerDeclaredAt = 0;

	private List<GameObject> winnerObjectList;


	// Use this for initialization
	void Start ()
	{
//		for (int i = 0; i < totalWinnerPlayers; i++) {
//			GameObject obj = Instantiate (winnerObjPrefab) as GameObject;
//			obj.transform.SetParent (scrollViewContent.transform);
//			obj.transform.localScale = Vector3.one;
//		}
//
//		winnerScrollView.content.sizeDelta = new Vector2 (winnerScrollView.content.sizeDelta.x, GetContentHeight (totalWinnerPlayers));

//		SetTournamentWinnerDetails ("[{\"Total_Table_Amount\": 4000,\"Winner_Name\": \"636026261474671966\",\"Winning_Amount\": 2000,\"WinningPercentage\": 50.5000001}, {\"Total_Table_Amount\": 4000,\"Winner_Name\": \"636026259253255390\",\"Winning_Amount\": 1200,\"WinningPercentage\": 30.5000001}, {\"Total_Table_Amount\": 4000,\"Winner_Name\": \"636026259347994863\",\"Winning_Amount\": 800,\"WinningPercentage\": 20.5000001}]");
	}

	void OnEnable ()
	{
		StartCoroutine ("CountDownTimer");

		NetworkManager.onTournamentWinnerInfoReceived += HandleOnTournamentWinnerInfoReceived;
		UIManager.Instance.breakTimePanel.gameObject.SetActive (false);
	}

	void OnDisable ()
	{
		NetworkManager.onTournamentWinnerInfoReceived -= HandleOnTournamentWinnerInfoReceived;

		DestroyAllObjects ();
		StopCoroutine ("CountDownTimer");
	}

	#region DELEGATE_CALLBACKS

	private void HandleOnTournamentWinnerInfoReceived (string sender, string tournamentWinnerInfo)
	{
		gameObject.SetActive (false);
	}

	#endregion

	public void SetTournamentWinnerDetails (string winnerInfo)
	{
		isTournamentWinnersDeclared = true;

		JSONArray arr = new JSONArray (winnerInfo);
		if (arr.Count () > 0)
			DestroyAllObjects ();

		winnerObjectList = new List<GameObject> ();

		List<TournamentWinner> tournamentWinnerList = new List<TournamentWinner> ();
		for (int i = 0; i < arr.Count (); i++) {
			TournamentWinner winner = JsonUtility.FromJson<TournamentWinner> (arr.getString (i));
			tournamentWinnerList.Add (winner);

			GameObject obj = Instantiate (winnerObjPrefab) as GameObject;
			obj.transform.SetParent (scrollViewContent.transform);
			obj.transform.localScale = Vector3.one;

			obj.GetComponent<TournamentWinnerObject> ().SetWinnerObjectData (winner);

			winnerObjectList.Add (obj);
		}

		SetContentHeight (arr.Count ());

		if (Time.time - gameWinnerDeclaredAt <= 5)
			Invoke ("DisplayTournamentWinnerObject", Constants.RESET_GAME_DATA_AFTER);
		else
			Invoke ("DisplayTournamentWinnerObject", 2);
	}

	private void DisplayTournamentWinnerObject ()
	{
		UIManager.Instance.texassGamePanel.txtTableMessage.text = "";
		UIManager.Instance.whoopAssGamePanel.txtTableMessage.text = "";

		UIManager.Instance.noEnoughChipsPanel.gameObject.SetActive (false);
		UIManager.Instance.playerEliminatedPanel.gameObject.SetActive (false);
		gameObject.SetActive (true);
		isTournamentWinnersDeclared = false;
	}

	private void SetContentHeight (int totalWinnerPlayers)
	{
//		return (totalWinnerPlayers * 100) + ((totalWinnerPlayers + 1) * 10);
		scrollViewContent.sizeDelta = new Vector2 (scrollViewContent.sizeDelta.x, totalWinnerPlayers * 100f);
	}

	public void OnCloseButtonTap ()
	{
		gameObject.SetActive (false);

		UIManager.Instance.backConfirmationPanel.OnYesButtonTap ();
	}

	private void DestroyAllObjects ()
	{
		if (winnerObjectList != null)
			foreach (GameObject obj in winnerObjectList)
				Destroy (obj);

		winnerObjectList = new List<GameObject> ();
	}

	private IEnumerator CountDownTimer ()
	{
		int countDown = 10;
		while (countDown >= 0) {
			countDown--;

			yield return new WaitForSeconds (1f);
		}

		yield return new WaitForSeconds (1f);

		OnCloseButtonTap ();
	}
}