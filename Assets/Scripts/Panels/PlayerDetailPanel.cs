using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerDetailPanel : MonoBehaviour
{
	public Image imgBackground;

	public Image imgProfilePic;
	public Text txtName;
	public Text txtPlayChips;
	public Text txtRealMoney;

	public Image imgPlayMoneySeparatorLine;
	public Image imgRealMoneySeparatorLine;

	private string playerName;
	private string totalPlayChips;
	private string totalRealMoney;

	private const float timeForTransition = .4f;

	// Use this for initialization
	void OnEnable ()
	{
		Hashtable htMove = new Hashtable ();
		htMove.Add ("time", timeForTransition);
		htMove.Add ("easetype", iTween.EaseType.easeInQuad);
		htMove.Add ("position", transform.parent.position);
		iTween.MoveTo (gameObject, htMove);

		Hashtable htScale = new Hashtable ();
		htScale.Add ("time", timeForTransition);
		htScale.Add ("easetype", iTween.EaseType.easeInQuad);
		htScale.Add ("scale", Vector3.one);
		iTween.ScaleTo (gameObject, htScale);

		StartCoroutine (DisplayPlayerDetails ());
	}

	void OnDisable ()
	{
		playerName = totalPlayChips = totalRealMoney = "";
	}

	public void SetPlayerDetails (Vector3 pos, Sprite spProfile, string name, string playChips, string realMoney, MoneyType moneyType = MoneyType.All)
	{
		transform.position = pos;
		transform.localScale = Vector3.zero;

		imgBackground.CrossFadeAlpha (0, 0, true);

		if (spProfile != null)
			imgProfilePic.sprite = spProfile;

		txtName.text = txtPlayChips.text = txtRealMoney.text = "";

		playerName = name;
		totalPlayChips = "Play Chips  : " + playChips;
		totalRealMoney = "Real Money  : " + realMoney;

		if (UIManager.Instance.isAffiliate) {
			if (moneyType == MoneyType.All) {
				imgPlayMoneySeparatorLine.gameObject.SetActive (true);
				txtPlayChips.gameObject.SetActive (true);
				imgRealMoneySeparatorLine.gameObject.SetActive (true);
				txtRealMoney.gameObject.SetActive (true);
			} else {
				if (moneyType == MoneyType.PlayMoney) {
					imgRealMoneySeparatorLine.gameObject.SetActive (false);
					txtRealMoney.gameObject.SetActive (false);

					imgPlayMoneySeparatorLine.gameObject.SetActive (true);
					txtPlayChips.gameObject.SetActive (true);
				} else if (moneyType == MoneyType.RealMoney) {
					imgPlayMoneySeparatorLine.gameObject.SetActive (false);
					txtPlayChips.gameObject.SetActive (false);

					imgRealMoneySeparatorLine.gameObject.SetActive (true);
					txtRealMoney.gameObject.SetActive (true);
				}
			}
		} else {
			imgPlayMoneySeparatorLine.gameObject.SetActive (true);
			txtPlayChips.gameObject.SetActive (true);
			imgRealMoneySeparatorLine.gameObject.SetActive (true);
			txtRealMoney.gameObject.SetActive (true);
		}

		gameObject.SetActive (true);
	}

	public void OnBackButtonTap ()
	{
		gameObject.SetActive (false);
	}

	private IEnumerator DisplayPlayerDetails ()
	{
		yield return new WaitForSeconds (timeForTransition);

		imgBackground.CrossFadeAlpha (1, timeForTransition / 3, true);

		yield return new WaitForSeconds (timeForTransition / 3);

		for (int i = 0; i < playerName.Length; i++) {
			txtName.text += playerName.Substring (i, 1);
			yield return 0;
		}

		if (txtPlayChips.gameObject.activeSelf)
			for (int i = 0; i < totalPlayChips.Length; i++) {
				txtPlayChips.text += totalPlayChips.Substring (i, 1);
				yield return 0;
			}

		if (txtRealMoney.gameObject.activeSelf)
			for (int i = 0; i < totalRealMoney.Length; i++) {
				txtRealMoney.text += totalRealMoney.Substring (i, 1);
				yield return 0;
			}
	}
}