using UnityEngine;
using System.Collections;
using UnityEditor;

public class MenuUtility : MonoBehaviour
{
	[MenuItem ("Rv/Scale to Zero %&q")]
	static void ScaleToZero ()
	{
		if (Selection.activeGameObject != null) {
			Selection.activeGameObject.transform.localScale = Vector3.zero;
		}
	}

	[MenuItem ("Rv/Scale to One %&w")]
	static void ScaleToOne ()
	{
		if (Selection.activeGameObject != null) {
			Selection.activeGameObject.transform.localScale = Vector3.one;
		}
	}

	[MenuItem ("Rv/Move to Zero %&z")]
	static void MoveToZero ()
	{
		if (Selection.activeGameObject != null) {
			Selection.activeGameObject.transform.position = Vector3.zero;
		}
	}

	[MenuItem ("Rv/Make Testing Exe Environment")]
	static void MakeTestingExeEnvironment ()
	{
		PlayerSettings.SetAspectRatio (AspectRatio.Aspect16by10, true);
		PlayerSettings.SetAspectRatio (AspectRatio.Aspect16by9, true);
		PlayerSettings.SetAspectRatio (AspectRatio.Aspect4by3, true);
		PlayerSettings.SetAspectRatio (AspectRatio.Aspect5by4, true);
		PlayerSettings.SetAspectRatio (AspectRatio.AspectOthers, true);

		if (Selection.activeGameObject != null) {
			UIManager ui = Selection.activeGameObject.GetComponent<UIManager> ();
			if (ui != null)
				ui.isTestingExe = true;
		}
	}

	[MenuItem ("Rv/Make Release Exe Environment")]
	static void MakeReleaseExeEnvironment ()
	{
		PlayerSettings.SetAspectRatio (AspectRatio.Aspect16by10, true);
		PlayerSettings.SetAspectRatio (AspectRatio.Aspect16by9, true);
		PlayerSettings.SetAspectRatio (AspectRatio.Aspect4by3, false);
		PlayerSettings.SetAspectRatio (AspectRatio.Aspect5by4, false);
		PlayerSettings.SetAspectRatio (AspectRatio.AspectOthers, false);

		if (Selection.activeGameObject != null) {
			UIManager ui = Selection.activeGameObject.GetComponent<UIManager> ();
			if (ui != null)
				ui.isTestingExe = false;
		}
	}
}