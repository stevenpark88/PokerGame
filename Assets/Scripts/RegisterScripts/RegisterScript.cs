using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Globalization;

public class RegisterScript : MonoBehaviour
{
	public GameObject registerPanel;
	public GameObject loginPanel;

	public Dropdown dateDropdown;
	public Dropdown monthDropdown;
	public Dropdown yearDropdown;

	public Sprite backgroundImage;

	void Awake ()
	{
		SetBirthdateDropdown ();
	}

	public void OnRegisterBtnClick ()
	{
		registerPanel.SetActive (false);
		loginPanel.SetActive (true);
	}

	private void SetBirthdateDropdown()
	{
		for (int i = 1; i <= 31; i++) {
			Dropdown.OptionData opt = new Dropdown.OptionData (i.ToString ());
			opt.image = backgroundImage;
			dateDropdown.options.Add (opt);
		}
		for (int i = 1; i <= 12; i++) {
			Dropdown.OptionData opt = new Dropdown.OptionData (DateTimeFormatInfo.CurrentInfo.GetMonthName(i));
			opt.image = backgroundImage;
			monthDropdown.options.Add(opt);
		}
		for (int i = 1900; i <= 1995; i++) {
			Dropdown.OptionData opt = new Dropdown.OptionData (i.ToString ());
			opt.image = backgroundImage;
			yearDropdown.options.Add (opt);
		}
		dateDropdown.RefreshShownValue ();
		monthDropdown.RefreshShownValue ();
		yearDropdown.RefreshShownValue ();
	}
}
