using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChatMessageObject : MonoBehaviour
{
	[HideInInspector]
	public string msg;
	public Text txtChatMsg;

	public void SendChat()
	{
		PlayerChat chat = new PlayerChat ();
		chat.toPlayerID = "";
		chat.chatMessage = msg;

		NetworkManager.Instance.SendChatMessage (JsonUtility.ToJson (chat));

//		if (UIManager.Instance.gameType == POKER_GAME_TYPE.TABLE) {
//			GameManager.Instance.chatTemplatesPanel.gameObject.SetActive (false);
//		} else if (UIManager.Instance.gameType == POKER_GAME_TYPE.TEXAS) {
//			TexassGame.Instance.chatTemplatesPanel.gameObject.SetActive (false);
//		} else if (UIManager.Instance.gameType == POKER_GAME_TYPE.WHOOPASS) {
//			WhoopAssGame.Instance.chatTemplatesPanel.gameObject.SetActive (false);
//		}
	}
}