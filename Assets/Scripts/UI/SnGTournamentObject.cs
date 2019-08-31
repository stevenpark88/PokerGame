using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SnGTournamentObject : MonoBehaviour
{
	public Text txtName;
	public Text txtBuyin;
	public Text txtPrizePool;
	public Text txtStatus;
	public Text txtEnrolled;

	public Image imgBackground;

	public Image imgTournamentStatus;

	public string gameID;

	public int index;

	// Use this for initialization
	void Start () {
	
	}

	public void OnDetailButtonTap()
	{
		LobbySngTournamentPanel.Instance.tournamentDetailPanel.DisplayGameDetail (LobbySngTournamentPanel.Instance.gameInfo.gameList [index]);

		SoundManager.Instance.PlayButtonTapSound ();
	}
}