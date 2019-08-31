using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InsufficientChipsEliminationPanel : MonoBehaviour
{
	public Text txtTimer;
	public Text txtMessage;
	public GameObject parentObject;

	public Button btnAddChips;

	// Use this for initialization
	void OnEnable ()
	{
		StartCoroutine ("CountDownTimer");

		txtMessage.text = Constants.MESSAGE_INSUFFICIENT_CHIPS_ELIMINATION;

		GetComponent<Image> ().CrossFadeAlpha (.75f, .1f, true);

		Hashtable ht = new Hashtable ();
		ht.Add ("time", .1f);
		ht.Add ("easetype", iTween.EaseType.spring);
		ht.Add ("scale", Vector3.one);

		iTween.ScaleTo (parentObject, ht);

		if (UIManager.Instance.gameType == POKER_GAME_TYPE.TEXAS) {
			if (TexassGame.Instance.ownTexassPlayer != null) {
				if (UIManager.Instance.isRealMoney)
					btnAddChips.interactable = TexassGame.Instance.ownTexassPlayer.totalRealMoney >= TexassGame.Instance.initialBuyinAmountForTournament;
				else
					btnAddChips.interactable = TexassGame.Instance.ownTexassPlayer.totalChips >= TexassGame.Instance.initialBuyinAmountForTournament;
			}
		} else if (UIManager.Instance.gameType == POKER_GAME_TYPE.WHOOPASS) {
			if (WhoopAssGame.Instance.ownWhoopAssPlayer != null) {
				if (UIManager.Instance.isRealMoney)
					btnAddChips.interactable = WhoopAssGame.Instance.ownWhoopAssPlayer.totalRealMoney >= WhoopAssGame.Instance.initialBuyinAmountForTournament;
				else
					btnAddChips.interactable = WhoopAssGame.Instance.ownWhoopAssPlayer.totalChips >= WhoopAssGame.Instance.initialBuyinAmountForTournament;
			}
		}
	}

	void OnDisable ()
	{
		parentObject.transform.localScale = Vector3.zero;
		GetComponent<Image> ().CrossFadeAlpha (0f, 0f, true);
		StopCoroutine ("CountDownTimer");
	}

	public void OnOkayButtonTap ()
	{
		//		if (Application.platform == RuntimePlatform.WebGLPlayer) {
		//			UIManager.Instance.BackToLobby ();
		//		} else {
		//			if (UIManager.Instance.texassGamePanel.gameObject.activeSelf)
		//				UIManager.Instance.texassGamePanel.gameObject.SetActive (false);
		//			else if (UIManager.Instance.whoopAssGamePanel.gameObject.activeSelf)
		//				UIManager.Instance.whoopAssGamePanel.gameObject.SetActive (false);
		//					
		//			UIManager.Instance.roomsPanel.gameObject.SetActive (true);
		//			UIManager.Instance.bottomBarPanel.gameObject.SetActive (true);
		//		}

		gameObject.SetActive (false);
	}

	public void OnAddChipsButtonTap ()
	{
		btnAddChips.interactable = false;

		if (UIManager.Instance.gameType == POKER_GAME_TYPE.TEXAS) {
			NetworkManager.Instance.SendRequestToServer (Constants.REQUEST_FOR_REBUY + TexassGame.Instance.initialBuyinAmountForTournament);
		} else if (UIManager.Instance.gameType == POKER_GAME_TYPE.WHOOPASS) {
			NetworkManager.Instance.SendRequestToServer (Constants.REQUEST_FOR_REBUY + WhoopAssGame.Instance.initialBuyinAmountForTournament);
		}

		gameObject.SetActive (false);
	}

	private IEnumerator CountDownTimer ()
	{
		int countDown = 10;
		while (countDown >= 0) {
			txtTimer.text = "Time : " + countDown-- + " sec";

			yield return new WaitForSeconds (1f);
		}

		btnAddChips.interactable = false;
		yield return new WaitForSeconds (1f);

		UIManager.Instance.backConfirmationPanel.OnYesButtonTap ();
	}
}