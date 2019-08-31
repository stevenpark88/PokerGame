using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackConfirmationPanel : MonoBehaviour
{
	public Text txtTitle;
	public GameObject parentObject;

	// Use this for initialization
	void OnEnable ()
	{
		txtTitle.text = Constants.MESSAGE_CONFIRMATION;

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

	public void OnYesButtonTap ()
	{
		gameObject.SetActive (false);

		if (Application.platform == RuntimePlatform.WebGLPlayer) {
			if (UIManager.Instance.isAffiliate)
				BackToLobby ();
			else
				UIManager.Instance.BackToLobbyOnWebsite ();
		} else if (Application.platform == RuntimePlatform.Android ||
		           Application.platform == RuntimePlatform.IPhonePlayer ||
		           Application.platform == RuntimePlatform.WindowsPlayer ||
		           Application.platform == RuntimePlatform.WindowsEditor) {
			BackToLobby ();
		} else {
			LoginScript.loginDetails = null;
			SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
			return;

			UIManager.Instance.whoopAssGamePanel.gameObject.SetActive (false);
			UIManager.Instance.texassGamePanel.gameObject.SetActive (false);
			UIManager.Instance.gamePlayPanel.gameObject.SetActive (false);

			UIManager.Instance.lobbyPanel.gameObject.SetActive (true);
			UIManager.Instance.loginPanel.gameObject.SetActive (true);

			NetworkManager.Instance.Disconnect ();
		}

		gameObject.SetActive (false);
	}

	private void BackToLobby ()
	{
		NetworkManager.Instance.Disconnect ();
		UIManager.Instance.whoopAssGamePanel.gameObject.SetActive (false);
		UIManager.Instance.texassGamePanel.gameObject.SetActive (false);
		UIManager.Instance.gamePlayPanel.gameObject.SetActive (false);
		UIManager.Instance.lobbyPanel.gameObject.SetActive (true);
		UIManager.Instance.whoopAssGamePanel.gameObject.SetActive (false);
		UIManager.Instance.texassGamePanel.gameObject.SetActive (false);
		UIManager.Instance.gamePlayPanel.gameObject.SetActive (false);
		UIManager.Instance.lobbyPanel.gameObject.SetActive (true);
		UIManager.Instance.loginPanel.gameObject.SetActive (true);
	}

	public void OnNoButtonTap ()
	{
		gameObject.SetActive (false);
	}
}