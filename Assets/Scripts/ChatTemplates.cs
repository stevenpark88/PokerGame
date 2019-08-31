using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ChatTemplates : MonoBehaviour
{
	public GameObject chatTemplateObject;
	public RectTransform content;
	public List<string> chatTemplateMessages;
	

	// Use this for initialization
	void Awake ()
	{
		SetChatTemplates ();
	}

	void OnEnable()
	{
//		StartCoroutine ("HideAfterSometime");
	}

	void OnDisable()
	{
//		StopCoroutine ("HideAfterSometime");
	}

	private void SetChatTemplates()
	{
		foreach (string msg in chatTemplateMessages) {
			GameObject ct = Instantiate (chatTemplateObject) as GameObject;
			ChatMessageObject cmo = ct.GetComponent<ChatMessageObject> ();
			cmo.msg = cmo.txtChatMsg.text = msg;

			ct.transform.SetParent (content.transform);
			ct.transform.localScale = Vector3.one;
		}

		content.sizeDelta = new Vector2 (content.sizeDelta.x, GetContentHeight ());
	}

	private float GetContentHeight()
	{
		float height = chatTemplateObject.GetComponent<LayoutElement> ().preferredHeight;
		float contentHeight = (chatTemplateMessages.Count * height) + 5f + 5f;

		return contentHeight;
	}

	private IEnumerator HideAfterSometime()
	{
		yield return new WaitForSeconds (Constants.HIDE_CHAT_TEMPLATE_AFTER);

		gameObject.SetActive (false);
	}
}