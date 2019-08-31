using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour
{
	#region PUBLIC_VARIABLES
	public Transform loader;
	#endregion

	#region PRIVATE_VARIABLES
	#endregion

	#region UNITY_CALLBACKS
	// Use this for initialization
	void OnEnable ()
	{
		transform.SetAsLastSibling ();
	}

	void OnDisable()
	{
		
	}

	void Update()
	{
		loader.Rotate (Vector3.forward * -10f);
	}
	#endregion

	#region PUBLIC_METHODS
	#endregion

	#region PRIVATE_METHODS
	#endregion

	#region COROUTINES
	#endregion
}