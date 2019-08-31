using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CashGameObject : MonoBehaviour
{
	public Text txtName;
	public Text txtBuyin;
	public Text txtStake;
	public Text txtPlayers;

	public Image imgBackground;

	public string gameID;

	public int index;

	// Use this for initialization
	void Start ()
	{
		
	}

	public void OnDetailButtonTap()
	{
		LobbyCashGamePanel.Instance.cashGameDetailPanel.DisplayGameDetail (LobbyCashGamePanel.Instance.gameInfo.gameList [index]);

		SoundManager.Instance.PlayButtonTapSound ();
	}
}