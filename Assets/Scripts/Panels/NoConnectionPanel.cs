using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NoConnectionPanel : MonoBehaviour
{
	public Text txtMessage;

	// Use this for initialization
	void OnEnable ()
	{
		txtMessage.text = Constants.MESSAGE_NO_INTERNET_CONNECTION;
	}

	public void OnCloseButtonTap()
	{
		gameObject.SetActive (false);

		SoundManager.Instance.PlayButtonTapSound ();
	}
}