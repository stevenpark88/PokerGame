using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConnectionFailPanel : MonoBehaviour
{
	public Text txtMessage;
	public GameObject parentObject;

	void OnEnable ()
	{
		transform.SetAsLastSibling ();

		txtMessage.text = Constants.MESSAGE_CONNECTION_FAIL;

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

	public void OnOkButtonTap ()
	{
		this.gameObject.SetActive (false);
		UIManager.Instance.backConfirmationPanel.OnYesButtonTap ();
	}
}