using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UIAccount
{
	public class BuyChipsPanel : MonoBehaviour
	{
		#region PUBLIC_VARIABLES

		public Button btnBuyRealMoney;
		public Button btnBuyPlayMoney;

		#endregion

		#region PRIVATE_VARIABLES

		#endregion

		#region UNITY_CALLBACKS

		// Use this for initialization
		void OnEnable ()
		{
			UpdateBuyButtons ();
		}

		#endregion

		#region DELEGATE_CALLBACKS

		#endregion

		#region PUBLIC_METHODS

		public void OnRealMoneyButtonTap ()
		{
			if (WebViewManager.Instance.webViewObject != null)
				WebViewManager.Instance.CloseWebview ();

			string url = Constants.URL_BUY_CASH;
			if (Application.platform == RuntimePlatform.WebGLPlayer && UIManager.Instance.isAffiliate)
				url = Constants.URL_BUY_CASH;
			url = url.Replace (Constants.FIELD_PLAYER_TOKEN_URL, APIConstants.PLAYER_TOKEN);

			WebViewManager.Instance.OpenWebviewWithURL (url);
		}

		public void OnPlayMoneyButtonTap ()
		{
			if (WebViewManager.Instance.webViewObject != null)
				WebViewManager.Instance.CloseWebview ();

			string url = Constants.URL_BUY_CHIPS;
			if (Application.platform == RuntimePlatform.WebGLPlayer && UIManager.Instance.isAffiliate)
				url = Constants.URL_BUY_CHIPS;
			url = url.Replace (Constants.FIELD_PLAYER_TOKEN_URL, APIConstants.PLAYER_TOKEN);

			WebViewManager.Instance.OpenWebviewWithURL (url);
		}

		public void UpdateBuyButtons ()
		{
			btnBuyPlayMoney.gameObject.SetActive (UIManager.Instance.lobbyPanel.availableMoneyType.Contains ("Play Money"));
			btnBuyRealMoney.gameObject.SetActive (UIManager.Instance.lobbyPanel.availableMoneyType.Contains ("Real Money"));
		}

		#endregion

		#region PRIVATE_METHODS

		#endregion

		#region COROUTINES

		#endregion
	}
}