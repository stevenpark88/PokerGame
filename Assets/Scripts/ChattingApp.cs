using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChattingApp : MonoBehaviour
{
	public InputField ifChat;
	public Text txtChat;

	// Use this for initialization
	void Start ()
	{
	
	}

	void OnEnable()
	{
		NetworkManager.onChatMessageReceived += HandleOnChatMessageReceived;
	}

	void OnDisable()
	{
		NetworkManager.onChatMessageReceived -= HandleOnChatMessageReceived;
	}

	public void OnSendButtonTap()
	{
		if (!string.IsNullOrEmpty (ifChat.text) && ifChat.text.Trim ().Length != 0) {
			PlayerChat chat = new PlayerChat ();
			chat.toPlayerID = "";

			//  Cut the message if more than specified length
			string msg = ifChat.text.Trim ();
			if (msg.Length > Constants.MAX_CHAT_MESSAGE_LENGTH) {
				msg = msg.Substring (0, Constants.MAX_CHAT_MESSAGE_LENGTH);
				msg += "...";
			}
			chat.chatMessage = msg;

			NetworkManager.Instance.SendChatMessage (JsonUtility.ToJson (chat));
		}

		ifChat.text = "";
	}

	private void HandleOnChatMessageReceived (string playerID, string chatMessage)
	{
		PlayerChat chat = JsonUtility.FromJson<PlayerChat> (chatMessage);

		txtChat.text += "\n<b>" + playerID + "</b> : " + chat.chatMessage;
		Canvas.ForceUpdateCanvases ();
//		scrollChat.verticalScrollbar.value = 0;
	}
}