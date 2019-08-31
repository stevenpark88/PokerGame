using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
	private const string PlayerPrefsSound = "Sound";

	public static bool IsSoundEnabled {
		get { 
			return PlayerPrefs.GetInt (PlayerPrefsSound, 1) == 1 ? true : false;
		}
		set { 
			PlayerPrefs.SetInt (PlayerPrefsSound, value == true ? 1 : 0);
		}
	}
}