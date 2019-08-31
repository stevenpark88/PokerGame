using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NotRegisteredWithTourna : MonoBehaviour
{
    public Text message;

    // Use this for initialization
    void OnEnable()
    {
        transform.SetAsLastSibling();
        message.text = Constants.MESSAGE_NOT_REGISTERED_WITH_TOURNAMENT;
    }

    public void OnBackButtonTap()
    {
        if (UIManager.Instance.gameType == POKER_GAME_TYPE.WHOOPASS)
        {
            WhoopAssGame.Instance.txtMessage.text = Constants.MESSAGE_NOT_REGISTERED_WITH_TOURNAMENT;
            WhoopAssGame.Instance.txtMessage.gameObject.SetActive(true);
        }
        else if (UIManager.Instance.gameType == POKER_GAME_TYPE.TEXAS)
        {
            TexassGame.Instance.txtMessage.text = Constants.MESSAGE_NOT_REGISTERED_WITH_TOURNAMENT;
            TexassGame.Instance.txtMessage.gameObject.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}