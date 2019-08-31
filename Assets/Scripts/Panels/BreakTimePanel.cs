using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BreakTimePanel : MonoBehaviour
{
	public Image loaderImage;
	public Text message;

	private bool isBlinking = false;

	// Use this for initialization
	void OnEnable ()
	{
		transform.SetAsLastSibling ();

		NetworkManager.onPlayerInfoReceived += HandleOnPlayerInfoReceived;
		NetworkManager.onDistributeCardResponseReceived += HandleOnDistributeCardsResponseReceived;
		NetworkManager.onTournamentWinnerInfoReceived += HandleOnTournamentWinnerInfoReceived;
	}

	void OnDisable()
	{
		UIManager.Instance.isBreakTime = false;
		message.text = "";

		NetworkManager.onPlayerInfoReceived -= HandleOnPlayerInfoReceived;
		NetworkManager.onDistributeCardResponseReceived -= HandleOnDistributeCardsResponseReceived;
		NetworkManager.onTournamentWinnerInfoReceived -= HandleOnTournamentWinnerInfoReceived;
	}

    public void DisplayBreakTimer(int breakTime)
    {
		if (UIManager.Instance.tournamentWinnerPanel.gameObject.activeSelf)
			return;

        gameObject.SetActive(true);
		StopCoroutine ("DisplayDigitalTimer");
		StartCoroutine ("DisplayDigitalTimer", breakTime);
    }

    private IEnumerator DisplayDigitalTimer(int maxSeconds)
    {
		int initialTime = maxSeconds;
		bool noTablesAtBreaktime = UIManager.Instance.noTablesAtBreaktime;

		isBlinking = false;
		StopCoroutine ("BlinkTimer");
        while(maxSeconds > 0)
        {
            int seconds = --maxSeconds % 60;
            int minutes = maxSeconds / 60;
            string time = minutes.ToString("00") + " : " + seconds.ToString("00");

            message.text = time;

            yield return new WaitForSeconds(1f);

			if (!isBlinking && maxSeconds == 6) {
				StartCoroutine ("BlinkTimer");
			}
        }

        yield return new WaitForSeconds(1f);

		if (initialTime != 0)
			gameObject.SetActive (false);
		else {
			if (!noTablesAtBreaktime) {
				while (true) {
					string msg = "Game is running on other table(s)..\nPlease wait   ";
					message.text = msg;

					yield return new WaitForSeconds (1f);
					msg = "Game is running on other table(s)..\nPlease wait.  ";
					message.text = msg;

					yield return new WaitForSeconds (1f);
					msg = "Game is running on other table(s)..\nPlease wait.. ";
					message.text = msg;

					yield return new WaitForSeconds (1f);
					msg = "Game is running on other table(s)..\nPlease wait...";
					message.text = msg;

					yield return new WaitForSeconds (1f);
				}
			}
		}

		StopCoroutine ("BlinkTimer");
    }

	private void HandleOnPlayerInfoReceived(string sender, string playerInfo)
	{
		gameObject.SetActive (false);
	}

	private void HandleOnDistributeCardsResponseReceived(string sender)
	{
		gameObject.SetActive (false);
	}

	private void HandleOnTournamentWinnerInfoReceived(string sender, string winnerInfo)
	{
		gameObject.SetActive (false);
	}

	private IEnumerator BlinkTimer()
	{
		while (true) {
			message.CrossFadeAlpha (0, .25f, true);
			yield return new WaitForSeconds (.25f);

			message.CrossFadeAlpha (1, .25f, true);
			yield return new WaitForSeconds (.25f);
		}
	}
}