using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RegularTournamentObject : MonoBehaviour
{
	public Text txtStartDate;
	public Text txtName;
	public Text txtBuyin;
	public Text txtPrizePool;
	public Text txtStatus;
	public Text txtEnrolled;

	public Image imgTournamentStatus;

	public Image imgBackground;

	public string gameID;

	public int index;

	// Use this for initialization
	void Start ()
	{
	
	}

	public void OnDetailButtonTap()
	{
		LobbyRegularTournamentPanel.Instance.tournamentDetailPanel.DisplayGameDetail (LobbyRegularTournamentPanel.Instance.gameInfo.gameList [index]);

		SoundManager.Instance.PlayButtonTapSound ();
	}
}