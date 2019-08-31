using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HourlymessagePanel : MonoBehaviour {

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
	public void OpenMessagePanel(GetlastHourAchivement GetlastHourAchivementCall)
	{
		string DisplayMessage = "";
		DisplayMessage  = "win : "+GetlastHourAchivementCall.winning_amount +"\nLose : "+GetlastHourAchivementCall.lose_amount;
		txtMessage.text = DisplayMessage;
		this.gameObject.SetActive (true);
	}
	public void OnbackToGameBtnTap ()
	{

		if (UIManager.Instance.texassGamePanel.gameObject.activeInHierarchy) 
		{
			UIManager.Instance.texassGamePanel.ResetTimerForHourly ();
		}
		if (UIManager.Instance.whoopAssGamePanel.gameObject.activeInHierarchy) 
		{
			UIManager.Instance.whoopAssGamePanel.ResetTimerForHourly ();
		}
		if (UIManager.Instance.gamePlayPanel.gameObject.activeInHierarchy) 
		{
			UIManager.Instance.gamePlayPanel.ResetTimerForHourly ();
		}

		gameObject.SetActive (false);
		SoundManager.Instance.PlayButtonTapSound ();
	}
	public void OnEndSessionBtnTap ()
	{


		if (UIManager.Instance.texassGamePanel.gameObject.activeInHierarchy) 
		{
			UIManager.Instance.texassGamePanel.OnHomeButtonTap ();
		}
		if (UIManager.Instance.whoopAssGamePanel.gameObject.activeInHierarchy) 
		{
			UIManager.Instance.whoopAssGamePanel.OnHomeButtonTap ();
		}
		if (UIManager.Instance.gamePlayPanel.gameObject.activeInHierarchy) 
		{
			UIManager.Instance.gamePlayPanel.OnHomeButtonTap ();
		}

		SoundManager.Instance.PlayButtonTapSound ();
		gameObject.SetActive (false);
	}

	#endregion
}