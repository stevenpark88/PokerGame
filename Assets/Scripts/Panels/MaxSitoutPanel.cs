using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MaxSitoutPanel : MonoBehaviour
{
	public Text txtTitle;
	public GameObject parentObject;

	// Use this for initialization
	void OnEnable ()
	{
		txtTitle.text = Constants.MESSAGE_MAX_SITOUT;
		if (UIManager.Instance.isRegularTournament ||
		    UIManager.Instance.isSitNGoTournament)
			txtTitle.text += "\n" + Constants.MESSAGE_ELIMINATION_REASON_MAX_SITOUT;

		GetComponent<Image> ().CrossFadeAlpha (.75f, .25f, true);

		Hashtable ht = new Hashtable ();
		ht.Add ("time", .5f);
		ht.Add ("easetype", iTween.EaseType.spring);
		ht.Add ("scale", Vector3.one);

		iTween.ScaleTo (parentObject, ht);

//		StartCoroutine ("TapOnOkayButtonAfterSometime");
	}

	void OnDisable ()
	{
		parentObject.transform.localScale = Vector3.zero;
		GetComponent<Image> ().CrossFadeAlpha (0f, 0f, true);

//		StopCoroutine ("TapOnOkayButtonAfterSometime");
	}

	public void OnOkayButtonTap ()
	{
		UIManager.Instance.backConfirmationPanel.OnYesButtonTap ();

//		StopCoroutine ("TapOnOkayButtonAfterSometime");
	}

	private IEnumerator TapOnOkayButtonAfterSometime()
	{
		yield return new WaitForSeconds (5f);

		OnOkayButtonTap ();
	}
}