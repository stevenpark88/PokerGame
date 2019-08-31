using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
	public AudioClip cardSuffle;
	public AudioClip chips;
	public AudioClip gameComplete;
	public AudioClip woosh;
	public AudioClip tick;
	public AudioClip playerActionCheck;
	public AudioClip playerActionAllin;
	public AudioClip playerActionFold;
	public AudioClip chipSound;
	public AudioClip betcallSound;
	public AudioClip roundComplete;
	public AudioClip buttonTap;

	public static SoundManager Instance;

	// Use this for initialization
	void Awake ()
	{
		Instance = this;
	}

	public void PlayGameCompleteSound (Vector3 pos)
	{
		if (DataManager.IsSoundEnabled)
			AudioSource.PlayClipAtPoint (gameComplete, pos);
	}

	public void PlayCardSuffleSound (Vector3 pos)
	{
		if (DataManager.IsSoundEnabled)
			AudioSource.PlayClipAtPoint (cardSuffle, pos);
	}

	public void PlayWooshSound (Vector3 pos)
	{
		//AudioSource.PlayClipAtPoint (woosh, pos);
	}

	public void PlayTickSound (Vector3 pos)
	{
		if (DataManager.IsSoundEnabled)
			AudioSource.PlayClipAtPoint (tick, pos);
	}

	public void PlayCheckActionSound (Vector3 pos)
	{
		if (DataManager.IsSoundEnabled)
			AudioSource.PlayClipAtPoint (playerActionCheck, pos);
	}

	public void PlayAllinActionSound (Vector3 pos)
	{
		if (DataManager.IsSoundEnabled)
			AudioSource.PlayClipAtPoint (playerActionAllin, pos);
	}

	public void PlayFoldActionSound (Vector3 pos)
	{
		if (DataManager.IsSoundEnabled)
			AudioSource.PlayClipAtPoint (playerActionFold, pos);
	}

	public void PlayChipsSound (Vector3 pos)
	{
		if (DataManager.IsSoundEnabled)
			AudioSource.PlayClipAtPoint (chipSound, pos);
	}

	public void PlayBetCallSound (Vector3 pos)
	{
		if (DataManager.IsSoundEnabled)
			AudioSource.PlayClipAtPoint (betcallSound, pos);
	}

	public void PlayRoundCompleteSound (Vector3 pos)
	{
		if (DataManager.IsSoundEnabled)
			AudioSource.PlayClipAtPoint (roundComplete, pos);
	}

	public void PlayButtonTapSound ()
	{
		if (DataManager.IsSoundEnabled)
			AudioSource.PlayClipAtPoint (buttonTap, Camera.main.transform.position);
	}
}