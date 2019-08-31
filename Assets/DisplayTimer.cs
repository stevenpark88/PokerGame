using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent (typeof(Text))]
public class DisplayTimer : MonoBehaviour
{
	#region PUBLIC_VARIABLES

	#endregion

	#region PRIVATE_VARIABLES

	private Text txtTimer;

	#endregion

	#region UNITY_CALLBACKS

	void OnEnable ()
	{
		txtTimer = GetComponent<Text> ();
		if (Application.platform != RuntimePlatform.WebGLPlayer)
			txtTimer.text = "";
	}

	void OnDisable ()
	{
		
	}

	//#if UNITY_WEBGL
	void Update ()
	{
		txtTimer.text = System.DateTime.Now.ToLongTimeString ();
	}
	//#endif

	#endregion

	#region DELEGATE_CALLBACKS

	#endregion

	#region PUBLIC_METHODS

	#endregion

	#region PRIVATE_METHODS

	#endregion

	#region COROUTINES

	#endregion
}