using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HelpPanel : MonoBehaviour
{
	#region PUBLIC_VARIABLES

	public Text txtGameRulesTitle;
	public Text txtTipsTitle;

	public Image imgGameRules;
	public Image imgTips;

	public Sprite spSelected;
	public Sprite spDisabled;

	public Text txtGameRules;
	public Text txtTips;

	public Loader loader;

	#endregion

	#region PRIVATE_VARIABLES

	#endregion

	#region UNITY_CALLBACKS

	// Use this for initialization
	void OnEnable ()
	{
		OnGameRulesButtonTap ();

		APIManager.tipResposneReceived += HandleTipResponseReceived;
		APIManager.gameRulesResponseReceived += HandleGameRulesResponseReceived;
	}

	void OnDisable ()
	{
		APIManager.tipResposneReceived -= HandleTipResponseReceived;
		APIManager.gameRulesResponseReceived -= HandleGameRulesResponseReceived;
	}

	#endregion

	#region DELEGATE_CALLBACKS

	private void HandleTipResponseReceived (string tips)
	{
		API_Tips tp = new API_Tips (tips);

		txtTips.text = "";
		foreach (Tip t in tp.tipList) {
			txtTips.text += t.title;
			txtTips.text += "          " + t.description;
			txtTips.text += "\n";
		}

		if (txtTips.gameObject.activeSelf)
			loader.gameObject.SetActive (false);

//		RectTransform rt = txtTips.GetComponent<RectTransform> ();
//		rt.sizeDelta = new Vector2 (rt.rect.width, txtTips.preferredHeight);
	}

	private void HandleGameRulesResponseReceived (string rules)
	{
		API_GameRules gr = new API_GameRules (rules);

		txtGameRules.text = "";
		foreach (Rule r in gr.ruleList) {
			txtGameRules.text += r.title + "\n";
			txtGameRules.text += "          " + r.description;
			txtGameRules.text += "\n";
		}

		if (txtGameRules.gameObject.activeSelf)
			loader.gameObject.SetActive (false);

//		RectTransform rt = txtGameRules.GetComponent<RectTransform> ();
//		rt.sizeDelta = new Vector2 (rt.rect.width, txtGameRules.preferredHeight);
	}

	#endregion

	#region PUBLIC_METHODS

	public void OnGameRulesButtonTap ()
	{
		imgGameRules.sprite = spSelected;
		imgTips.sprite = spDisabled;

		imgGameRules.transform.localScale = new Vector3 (1, 1, 1);
		imgTips.transform.localScale = new Vector3 (1, 1, 1);

		Color bgColor = new Color ();
		ColorUtility.TryParseHtmlString (APIConstants.HEX_GOLDEN_HEADER, out bgColor);
		txtGameRulesTitle.color = bgColor;

		ColorUtility.TryParseHtmlString (APIConstants.HEX_RED_HEADER, out bgColor);
		txtTipsTitle.color = bgColor;

		txtGameRules.gameObject.SetActive (true);
		txtTips.gameObject.SetActive (false);

		if (txtGameRules.text.Length == 0)
			loader.gameObject.SetActive (true);

		APIManager.GetInstance ().GetGameRules ();
	}

	public void OnTipsButtonTap ()
	{
		imgGameRules.sprite = spDisabled;
		imgTips.sprite = spSelected;

		imgGameRules.transform.localScale = new Vector3 (-1, 1, 1);
		imgTips.transform.localScale = new Vector3 (-1, 1, 1);

		Color bgColor = new Color ();
		ColorUtility.TryParseHtmlString (APIConstants.HEX_RED_HEADER, out bgColor);
		txtGameRulesTitle.color = bgColor;

		ColorUtility.TryParseHtmlString (APIConstants.HEX_GOLDEN_HEADER, out bgColor);
		txtTipsTitle.color = bgColor;

		txtGameRules.gameObject.SetActive (false);
		txtTips.gameObject.SetActive (true);

		if (txtGameRules.text.Length == 0)
			loader.gameObject.SetActive (true);

		APIManager.GetInstance ().GetTips ();
	}

	#endregion

	#region PRIVATE_METHODS

	#endregion

	#region COROUTINES

	#endregion
}