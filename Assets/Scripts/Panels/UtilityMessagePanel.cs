using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UtilityMessagePanel : MonoBehaviour
{
	#region PUBLIC_VARIABLES

	public Text txtTitle;
	public Text txtMessage;
	public GameObject parentObject;

	public Button btnOk;

	#endregion

	#region PRIVATE_VARIABLES

	#endregion

	#region UNITY_CALLBACKS

	// Use this for initialization
	void OnEnable ()
	{
		transform.SetAsLastSibling ();

		Hashtable htScale = new Hashtable ();
		htScale.Add ("amount", new Vector3 (.1f, .1f, .1f));
		htScale.Add ("time", .1f);
		htScale.Add ("easetype", iTween.EaseType.easeOutCirc);

		iTween.ShakeScale (parentObject, htScale);
	}

	#endregion

	#region PUBLIC_METHODS

	public void OnOkButtonTap ()
	{
		gameObject.SetActive (false);

		SoundManager.Instance.PlayButtonTapSound ();
	}

	#endregion
}