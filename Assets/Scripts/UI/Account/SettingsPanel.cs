using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UIAccount
{
	public class SettingsPanel : MonoBehaviour
	{
		#region PUBLIC_VARIABLES
		public Button btnAccount;
		public Button btnTransactions;
		public Button btnStatistics;

		public GameObject mainObject;

		public AccountPanel accountPanel;
		#endregion

		#region PRIVATE_VARIABLES

		#endregion

		#region UNITY_CALLBACKS

		// Use this for initialization
		void OnEnable ()
		{
			mainObject.SetActive (true);	
		}

		void OnDisable()
		{
			accountPanel.gameObject.SetActive (false);
		}

		#endregion

		#region DELEGATE_CALLBACKS

		#endregion

		#region PUBLIC_METHODS

		public void OnAccountButtonTap ()
		{

			mainObject.SetActive (false);
			accountPanel.gameObject.SetActive (true);
		}

		public void OnTransactionsButtonTap ()
		{
			if (WebViewManager.Instance.webViewObject != null)
				WebViewManager.Instance.CloseWebview ();

			string url = Constants.URL_PLAYER_TRANSACTIONS;
			if (Application.platform == RuntimePlatform.WebGLPlayer && UIManager.Instance.isAffiliate)
				url = Constants.URL_PLAYER_TRANSACTIONS;
			url = url.Replace (Constants.FIELD_PLAYER_TOKEN_URL, APIConstants.PLAYER_TOKEN);

			WebViewManager.Instance.OpenWebviewWithURL (url);
		}

		public void OnStatisticsButtonTap ()
		{
			if (WebViewManager.Instance.webViewObject != null)
				WebViewManager.Instance.CloseWebview ();

			string url = Constants.URL_PLAYER_STATISTICS;
			if (Application.platform == RuntimePlatform.WebGLPlayer && UIManager.Instance.isAffiliate)
				url = Constants.URL_PLAYER_STATISTICS;
			url = url.Replace (Constants.FIELD_PLAYER_TOKEN_URL, APIConstants.PLAYER_TOKEN);

			WebViewManager.Instance.OpenWebviewWithURL (url);
		}

		#endregion

		#region PRIVATE_METHODS

		#endregion

		#region COROUTINES

		#endregion
	}
}