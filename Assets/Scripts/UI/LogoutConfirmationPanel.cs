using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LogoutConfirmationPanel : MonoBehaviour
{
	#region PUBLIC_VARIABLES
	public Text txtMessage;
	public GameObject parentObject;
	#endregion

	#region PRIVATE_VARIABLES

	#endregion

	#region UNITY_CALLBACKS
	// Use this for initialization
	void OnEnable ()
	{
		txtMessage.text = Constants.MESSAGE_CONFIRMATION;

		Hashtable htScale = new Hashtable ();
		htScale.Add ("amount", new Vector3 (.1f, .1f, .1f));
		htScale.Add ("time", .1f);
		htScale.Add ("easetype", iTween.EaseType.easeOutCirc);
		iTween.ShakeScale (parentObject, htScale);
	}
	#endregion

	#region PUBLIC_METHODS
	public void OnYesButtonTap()
	{
		LoginScript.loginDetails = null;
		gameObject.SetActive (false);

		SoundManager.Instance.PlayButtonTapSound ();

		UIManager.Instance.DisplayLoader ("Logging out..");
		APIManager.GetInstance ().Logout ((success) => {
			UIManager.Instance.HideLoader ();

			if (success) {
				UIManager.Instance.lobbyPanel.gameObject.SetActive (false);
				UIManager.Instance.dashboardPanel.gameObject.SetActive (false);
				UIManager.Instance.loginPanel.gameObject.SetActive (true);
			}
		});
	}

	public void OnNoButtonTap()
	{
		gameObject.SetActive (false);

		SoundManager.Instance.PlayButtonTapSound ();
	}
	#endregion
}
