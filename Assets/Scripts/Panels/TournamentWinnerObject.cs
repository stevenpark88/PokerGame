using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TournamentWinnerObject : MonoBehaviour
{
	public Image imgWinnerProfile;
	public Text txtWinnerName;
	public Text txtWinningAmount;
	public Text txtWinningPercentage;

	public Sprite spInitialSprite;

	private string profilePicURL = "";

	public void SetWinnerObjectData(TournamentWinner winner)
	{
		txtWinnerName.text = winner.Winner_Name;
		txtWinningAmount.text = Utility.GetCurrencyPrefix () + winner.Winning_Amount.RoundTo2DigitFloatingPoint ();
		txtWinningPercentage.text = winner.WinningPercentage.RoundTo2DigitFloatingPoint () + "%";

		imgWinnerProfile.sprite = spInitialSprite;

		profilePicURL = winner.Profile_Pic;
	}

	void OnEnable()
	{
		StartCoroutine (GetProfilePic (profilePicURL));
	}

	private IEnumerator GetProfilePic (string url)
	{
		WWW www = new WWW (url);
		yield return www;

		if (www.error != null) {
			Debug.LogError ("Error while downloading profile pic  : " + www.error + "\nURL  : " + www.url);
		} else {
			if (www.texture != null) {
				imgWinnerProfile.sprite = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), Vector2.zero);
			}
		}
	}
}