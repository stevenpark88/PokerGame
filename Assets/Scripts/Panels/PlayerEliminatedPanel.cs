using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerEliminatedPanel : MonoBehaviour
{
	public Text txtTimer;
	public Text txtTitle;
	public GameObject parentObject;

	// Use this for initialization
	void OnEnable ()
	{
		StartCoroutine ("CountDownTimer");

		txtTitle.text = Constants.MESSAGE_YOU_HAVE_ELIMINATED;

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
		gameObject.SetActive (false);

		UIManager.Instance.backConfirmationPanel.OnYesButtonTap ();
	}

	private IEnumerator CountDownTimer()
	{
		int countDown = 10;
		while (countDown >= 0) {
			txtTimer.text = "Time : " + countDown-- + " sec";

			yield return new WaitForSeconds (1f);
		}

		yield return new WaitForSeconds (1f);

		OnOkayButtonTap ();
	}
}