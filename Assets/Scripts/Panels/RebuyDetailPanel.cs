using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RebuyDetailPanel : MonoBehaviour
{
	public Text txtTitle;
	public GameObject parentObject;

	public double rebuyAmount = 0;

	// Use this for initialization
	void OnEnable ()
	{
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
	}

	public void DisplayRebuyMessage (double rebuyAmount)
	{
		this.rebuyAmount = rebuyAmount;

		txtTitle.text = "Want to rebuy " + rebuyAmount + " ?";
		gameObject.SetActive (true);
	}

	public void OnRebuyButtonTap ()
	{
		if (UIManager.Instance.gameType == POKER_GAME_TYPE.TEXAS) {
			if (UIManager.Instance.isRealMoney) {
				if (TexassGame.Instance.ownTexassPlayer.totalRealMoney >= TexassGame.Instance.initialBuyinAmountForTournament)
					NetworkManager.Instance.SendRequestToServer (Constants.REQUEST_FOR_REBUY + TexassGame.Instance.initialBuyinAmountForTournament);
			} else {
				if (TexassGame.Instance.ownTexassPlayer.totalChips >= TexassGame.Instance.initialBuyinAmountForTournament)
					NetworkManager.Instance.SendRequestToServer (Constants.REQUEST_FOR_REBUY + TexassGame.Instance.initialBuyinAmountForTournament);
			}
		} else if (UIManager.Instance.gameType == POKER_GAME_TYPE.WHOOPASS) {
			if (UIManager.Instance.isRealMoney) {
				if (WhoopAssGame.Instance.ownWhoopAssPlayer.totalRealMoney >= WhoopAssGame.Instance.initialBuyinAmountForTournament)
					NetworkManager.Instance.SendRequestToServer (Constants.REQUEST_FOR_REBUY + WhoopAssGame.Instance.initialBuyinAmountForTournament);
			} else {
				if (WhoopAssGame.Instance.ownWhoopAssPlayer.totalChips >= WhoopAssGame.Instance.initialBuyinAmountForTournament)
					NetworkManager.Instance.SendRequestToServer (Constants.REQUEST_FOR_REBUY + WhoopAssGame.Instance.initialBuyinAmountForTournament);
			}
		}

		gameObject.SetActive (false);
	}

	public void OnLobbyButtonTap ()
	{
		gameObject.SetActive (false);

		UIManager.Instance.backConfirmationPanel.OnYesButtonTap ();
	}
}