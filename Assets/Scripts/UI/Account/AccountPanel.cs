using UnityEngine;
using System.Collections;

namespace UIAccount
{
	public class AccountPanel : MonoBehaviour
	{
		#region PUBLIC_VARIABLES

		public GameObject globalTimeZoneOb;
		public GameObject selfLimitOb;
		public GameObject selfExclusionOb;

		[Header ("Button Arrows")]
		public Transform timeZoneArrow;
		public Transform selfLimitArrow;
		public Transform selfExclusionArrow;

		#endregion

		#region PRIVATE_VARIABLES

		#endregion

		#region UNITY_CALLBACKS

		// Use this for initialization
		void OnEnable ()
		{
			OnGlobalTimeZoneButtonTap ();
		}

		void OnDisable ()
		{
			globalTimeZoneOb.SetActive (false);
			selfLimitOb.SetActive (false);
			selfExclusionOb.SetActive (false);
		}

		#endregion

		#region DELEGATE_CALLBACKS

		#endregion

		#region PUBLIC_METHODS

		public void OnGlobalTimeZoneButtonTap ()
		{
			if (!globalTimeZoneOb.activeSelf) {
				globalTimeZoneOb.SetActive (true);
				selfLimitOb.SetActive (false);
				selfExclusionOb.SetActive (false);

				timeZoneArrow.rotation = new Quaternion (180, 0, 0, 0);
				selfLimitArrow.rotation = new Quaternion (0, 0, 0, 0);
				selfExclusionArrow.rotation = new Quaternion (0, 0, 0, 0);
			} else {
				globalTimeZoneOb.SetActive (false);
				selfLimitOb.SetActive (false);
				selfExclusionOb.SetActive (false);

				timeZoneArrow.rotation = new Quaternion (0, 0, 0, 0);
				selfLimitArrow.rotation = new Quaternion (0, 0, 0, 0);
				selfExclusionArrow.rotation = new Quaternion (0, 0, 0, 0);
			}
		}

		public void OnSelfLimitButtonTap ()
		{
			if (!selfLimitOb.activeSelf) {
				globalTimeZoneOb.SetActive (false);
				selfLimitOb.SetActive (true);
				selfExclusionOb.SetActive (false);

				timeZoneArrow.rotation = new Quaternion (0, 0, 0, 0);
				selfLimitArrow.rotation = new Quaternion (180, 0, 0, 0);
				selfExclusionArrow.rotation = new Quaternion (0, 0, 0, 0);
			} else {
				globalTimeZoneOb.SetActive (false);
				selfLimitOb.SetActive (false);
				selfExclusionOb.SetActive (false);

				timeZoneArrow.rotation = new Quaternion (0, 0, 0, 0);
				selfLimitArrow.rotation = new Quaternion (0, 0, 0, 0);
				selfExclusionArrow.rotation = new Quaternion (0, 0, 0, 0);
			}
		}

		public void OnSelfExlusionButtonTap ()
		{
			if (!selfExclusionOb.activeSelf) {
				globalTimeZoneOb.SetActive (false);
				selfLimitOb.SetActive (false);
				selfExclusionOb.SetActive (true);

				timeZoneArrow.rotation = new Quaternion (0, 0, 0, 0);
				selfLimitArrow.rotation = new Quaternion (0, 0, 0, 0);
				selfExclusionArrow.rotation = new Quaternion (180, 0, 0, 0);
			} else {
				globalTimeZoneOb.SetActive (false);
				selfLimitOb.SetActive (false);
				selfExclusionOb.SetActive (false);

				timeZoneArrow.rotation = new Quaternion (0, 0, 0, 0);
				selfLimitArrow.rotation = new Quaternion (0, 0, 0, 0);
				selfExclusionArrow.rotation = new Quaternion (0, 0, 0, 0);
			}
		}

		public void OnLogoutButtonTap ()
		{
			UIManager.Instance.logoutConfirmationPanel.gameObject.SetActive (true);
		}

		#endregion

		#region PRIVATE_METHODS

		#endregion

		#region COROUTINES

		#endregion
	}
}