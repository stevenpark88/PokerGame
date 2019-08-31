using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WhoopAssCardRoundPanel : MonoBehaviour
{
	public Transform mainObjBg;
	public Text txtTitle;

	public Button btnUp;
	public Button btnDown;
	public Button btnNo;

    public Outline panelBorder;

	private double amountToBet;

    void OnEnable()
	{
//        if (UIManager.Instance.gameType == POKER_GAME_TYPE.WHOOPASS)
//            StartCoroutine("BlinkOutline");

		if (UIManager.Instance.gameType == POKER_GAME_TYPE.TABLE)
			mainObjBg.localPosition = new Vector3 (mainObjBg.localPosition.x, 110, mainObjBg.localPosition.z);
		else if (UIManager.Instance.gameType == POKER_GAME_TYPE.WHOOPASS)
			mainObjBg.localPosition = new Vector3 (mainObjBg.localPosition.x, 0, mainObjBg.localPosition.z);
	}

    void OnDisable()
    {
        panelBorder.enabled = false;
//        StopCoroutine("BlinkOutline");
    }

	public void SetTitle(double amount)
	{
		amountToBet = System.Math.Round (amount, 2);
		amountToBet = amountToBet < 0 ? 0 : amountToBet;
		txtTitle.text = Constants.MESSAGE_BUY_WHOOPASS_CARD + Utility.GetCurrencyPrefix() + amountToBet + "?";

        if (UIManager.Instance.gameType == POKER_GAME_TYPE.TABLE)
        {
            if (amountToBet <= GameManager.Instance.ownTablePlayer.buyinChips)
            {
                btnUp.interactable = true;
                btnDown.interactable = true;
                btnUp.transform.GetChild(0).GetComponent<Text>().CrossFadeAlpha(1f, 0f, true);
                btnDown.transform.GetChild(0).GetComponent<Text>().CrossFadeAlpha(1f, 0f, true);
            }
            else
            {
                btnUp.interactable = false;
                btnDown.interactable = false;
                btnUp.transform.GetChild(0).GetComponent<Text>().CrossFadeAlpha(.5f, 0f, true);
                btnDown.transform.GetChild(0).GetComponent<Text>().CrossFadeAlpha(.5f, 0f, true);
            }

            if (GameManager.Instance.ownTablePlayer.buyinChips < amountToBet &&
                !UIManager.Instance.isRegularTournament &&
                !UIManager.Instance.isSitNGoTournament)
                GameManager.Instance.btnAddChips.interactable = true;
        }
		else if (UIManager.Instance.gameType == POKER_GAME_TYPE.WHOOPASS) {
            if (amountToBet <= WhoopAssGame.Instance.ownWhoopAssPlayer.buyInAmount ||
				amountToBet == 0)
            {
                btnUp.interactable = true;
                btnDown.interactable = true;
                btnUp.transform.GetChild(0).GetComponent<Text>().CrossFadeAlpha(1f, 0f, true);
                btnDown.transform.GetChild(0).GetComponent<Text>().CrossFadeAlpha(1f, 0f, true);
            }
            else
            {
                btnUp.interactable = false;
                btnDown.interactable = false;
                btnUp.transform.GetChild(0).GetComponent<Text>().CrossFadeAlpha(.5f, 0f, true);
                btnDown.transform.GetChild(0).GetComponent<Text>().CrossFadeAlpha(.5f, 0f, true);
            }
        }

		gameObject.SetActive (true);
	}

	public void OnUpButtonTap()
	{
		PlayerAction action = new PlayerAction ();
		action.Action = (int)PLAYER_ACTION.ACTION_WA_UP;
		action.Bet_Amount = amountToBet;
        if (UIManager.Instance.gameType == POKER_GAME_TYPE.TABLE)
            action.IsBetOnStraight = false;
		action.Player_Name = NetworkManager.Instance.playerID;

		SendAction (action);
	}

	public void OnDownButtonTap()
	{
		PlayerAction action = new PlayerAction ();
		action.Action = (int)PLAYER_ACTION.ACTION_WA_DOWN;
		action.Bet_Amount = amountToBet;
        if (UIManager.Instance.gameType == POKER_GAME_TYPE.TABLE)
            action.IsBetOnStraight = false;
        action.Player_Name = NetworkManager.Instance.playerID;

		SendAction (action);
	}

	public void OnNoButtonTap()
	{
		PlayerAction action = new PlayerAction ();
		action.Action = (int)PLAYER_ACTION.ACTION_WA_NO;
		action.Bet_Amount = 0;
		action.IsBetOnStraight = false;
		action.Player_Name = NetworkManager.Instance.playerID;

		SendAction (action);
	}

	private void SendAction(PlayerAction action)
	{
		NetworkManager.Instance.SendPlayerAction (action);

        if (UIManager.Instance.gameType == POKER_GAME_TYPE.TABLE)
        {
            if (GameManager.Instance.ownTablePlayer)
                GameManager.Instance.ownTablePlayer.HideTurnTimer();
			GameManager.Instance.waCardAmount = action.Bet_Amount;
        }
        else
        {
            if (WhoopAssGame.Instance.ownWhoopAssPlayer)
                WhoopAssGame.Instance.ownWhoopAssPlayer.HideTurnTimer();
        }

		SoundManager.Instance.PlayWooshSound (Camera.main.transform.position);

        if (UIManager.Instance.gameType == POKER_GAME_TYPE.TABLE)
        {
            GameManager.Instance.btnFold.interactable = false;
//            GameManager.Instance.btnAddChips.interactable = false;
        }
        else
        {
            WhoopAssGame.Instance.btnFold.interactable = false;
        }
		gameObject.SetActive (false);
	}

    private IEnumerator BlinkOutline()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);
            panelBorder.enabled = true;

            yield return new WaitForSeconds(.5f);
            panelBorder.enabled = false;
        }
    }
}