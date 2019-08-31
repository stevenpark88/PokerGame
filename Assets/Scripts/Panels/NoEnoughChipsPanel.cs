using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NoEnoughChipsPanel : MonoBehaviour
{
	public Text txtTitle;
	public Text txtTimer;
	public GameObject parentObject;

	public GameObject allowAddChips;
	public GameObject notAllowAddChips;

	// Use this for initialization
	void OnEnable ()
	{
		StartCoroutine ("CountDownTimer");

		txtTitle.text = Constants.MESSAGE_NO_ENOUGH_CHIPS;

		GetComponent<Image> ().CrossFadeAlpha (.75f, .1f, true);

		Hashtable ht = new Hashtable ();
		ht.Add ("time", .1f);
		ht.Add ("easetype", iTween.EaseType.spring);
		ht.Add ("scale", Vector3.one);

		iTween.ScaleTo (parentObject, ht);
	}

	void OnDisable ()
	{
		parentObject.transform.localScale = Vector3.zero;
		GetComponent<Image> ().CrossFadeAlpha (0f, 0f, true);

		StopCoroutine ("CountDownTimer");
	}

	public void OnOkayButtonTap ()
	{
		NetworkManager.Instance.Disconnect ();
		
		gameObject.SetActive (false);
		UIManager.Instance.backConfirmationPanel.OnYesButtonTap ();
	}

	public void OnAddChipsButtonTap ()
	{
		if (UIManager.Instance.gameType == POKER_GAME_TYPE.TEXAS) {
			if (UIManager.Instance.isRealMoney)
				UIManager.Instance.texassGamePanel.rebuyPanel.DisplayRebuyPanel (UIManager.Instance.texassGamePanel.ownTexassPlayer.totalRealMoney - TexassGame.Instance.ownTexassPlayer.buyInAmount);
			else
				UIManager.Instance.texassGamePanel.rebuyPanel.DisplayRebuyPanel (UIManager.Instance.texassGamePanel.ownTexassPlayer.totalChips - TexassGame.Instance.ownTexassPlayer.buyInAmount);
		} else if (UIManager.Instance.gameType == POKER_GAME_TYPE.WHOOPASS) {
			if (UIManager.Instance.isRealMoney)
				UIManager.Instance.whoopAssGamePanel.rebuyPanel.DisplayRebuyPanel (UIManager.Instance.whoopAssGamePanel.ownWhoopAssPlayer.totalRealMoney - WhoopAssGame.Instance.ownWhoopAssPlayer.buyInAmount);
			else
				UIManager.Instance.whoopAssGamePanel.rebuyPanel.DisplayRebuyPanel (UIManager.Instance.whoopAssGamePanel.ownWhoopAssPlayer.totalChips - WhoopAssGame.Instance.ownWhoopAssPlayer.buyInAmount);
		} else if (UIManager.Instance.gameType == POKER_GAME_TYPE.TABLE) {
			if (UIManager.Instance.isRealMoney)
				GameManager.Instance.rebuyPanel.DisplayRebuyPanel (GameManager.Instance.ownTablePlayer.totalRealMoney - GameManager.Instance.ownTablePlayer.buyinChips, Constants.TABLE_GAME_REAL_MIN_MONEY);
			else
				GameManager.Instance.rebuyPanel.DisplayRebuyPanel (GameManager.Instance.ownTablePlayer.totalChips - GameManager.Instance.ownTablePlayer.buyinChips, Constants.TABLE_GAME_PLAY_MIN_CHIPS);
		}

		gameObject.SetActive (false);
	}

	private IEnumerator CountDownTimer ()
	{
		int countDown = 20;
		while (countDown >= 0) {
			txtTimer.text = "" + countDown--;

			yield return new WaitForSeconds (1f);
		}

		yield return new WaitForSeconds (1f);

		OnOkayButtonTap ();
	}
}