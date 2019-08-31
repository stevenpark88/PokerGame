using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UIAccount
{
	public class ProfilePanel : MonoBehaviour
	{
		#region PUBLIC_VARIABLES
		public InputField inputFieldName;
		public InputField inputFieldFirstName;
		public InputField inputFieldLastName;
		public InputField inputFieldEmail;
		public InputField inputFieldMobile;
		public InputField inputFieldAddress;
		public InputField inputFieldCountry;
		public InputField inputFieldState;
		public InputField inputFieldZipcode;

		public Dropdown dropDownDate;
		public Dropdown dropDownMonth;
		public Dropdown dropDownYear;
		#endregion

		#region PRIVATE_VARIABLES
		private const string dropDownDefaultValue = "-Select-";
		#endregion

		#region UNITY_CALLBACKS

		// Use this for initialization
		void Start ()
		{
			InitializeYearDropdown ();
		}

		#endregion

		#region DELEGATE_CALLBACKS

		#endregion

		#region PUBLIC_METHODS
		public void OnYearDropdownValueChanged ()
		{
			InitializeMonthDropdown ();
			InitializeDateDropdown ();
		}

		public void OnMonthDropdownValueChanged ()
		{
			InitializeMonthDropdown ();
			InitializeDateDropdown ();
		}

		public void OnDateDropdownValueChanged ()
		{
			
		}
		#endregion

		#region PRIVATE_METHODS
		private void InitializeYearDropdown()
		{
			dropDownYear.ClearOptions ();

			List<string> yearOptions = new List<string> ();
			yearOptions.Add (dropDownDefaultValue);
			for (int i = 1950; i < System.DateTime.Now.Year; i++) {
				yearOptions.Add (i.ToString ());
			}
			dropDownYear.AddOptions (yearOptions);

			dropDownMonth.interactable = dropDownDate.interactable = false;
		}

		private void InitializeMonthDropdown()
		{
			if (dropDownYear.options [dropDownYear.value].text.Equals (dropDownDefaultValue)) {
				dropDownMonth.interactable = false;
				return;
			}

			dropDownMonth.ClearOptions ();

			dropDownMonth.interactable = true;
			List<string> monthOptions = new List<string> ();
			monthOptions.Add (dropDownDefaultValue);

			monthOptions.Add ("January");
			monthOptions.Add ("February");
			monthOptions.Add ("March");
			monthOptions.Add ("April");
			monthOptions.Add ("May");
			monthOptions.Add ("June");
			monthOptions.Add ("July");
			monthOptions.Add ("August");
			monthOptions.Add ("September");
			monthOptions.Add ("October");
			monthOptions.Add ("November");
			monthOptions.Add ("December");

			dropDownMonth.AddOptions (monthOptions);
		}

		private void InitializeDateDropdown()
		{
			if (dropDownYear.options [dropDownYear.value].text.Equals (dropDownDefaultValue) ||
			    dropDownMonth.options [dropDownMonth.value].text.Equals (dropDownDefaultValue)) {
				dropDownDate.interactable = false;
				return;
			}

			dropDownDate.ClearOptions ();

			dropDownDate.interactable = true;
			int selectedYear = int.Parse (dropDownYear.options [dropDownYear.value].text);
			int selectedMonth = dropDownMonth.value;

			List<string> dateOptions = new List<string> ();
			dateOptions.Add (dropDownDefaultValue);
			for (int i = 1; i <= System.DateTime.DaysInMonth (selectedYear, selectedMonth); i++) {
				dateOptions.Add (i.ToString ());
			}
			dropDownDate.AddOptions (dateOptions);
		}
		#endregion

		#region COROUTINES

		#endregion
	}
}