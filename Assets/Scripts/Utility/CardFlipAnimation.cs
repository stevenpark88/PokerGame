using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardFlipAnimation : MonoBehaviour
{
	public Image card;

	public Sprite cardBackImage;

	string currentCard;

	private bool isOpened = false;

	public void DisplayCardWithoutAnimation(string currentCard)
	{
		this.currentCard = currentCard;
		ChangeImage ();
	}

	public void PlayAnimation(string currentCard)
	{
		if (isOpened) {
			DisplayCardWithoutAnimation (currentCard);
		} else {
			this.currentCard = currentCard;
			if (gameObject.activeSelf)
				GetComponent<Animator> ().SetTrigger (Constants.CARD_ANIMATION_TRIGGER);
		}
	}

    public void PlayAnimation(string currentCard, float time)
	{
		if (isOpened) {
			DisplayCardWithoutAnimation (currentCard);
		} else {
			this.currentCard = currentCard;
			Invoke ("InvokeAnimation", time);
		}
	}

    private void InvokeAnimation()
    {
        GetComponent<Animator>().SetTrigger(Constants.CARD_ANIMATION_TRIGGER);
    }

    /// <summary>
    /// Resets the image
    /// </summary>
    public void ResetImage ()
	{
		isOpened = false;
		card.sprite = cardBackImage;
	}

	private void ChangeImage ()
	{
		isOpened = true;
		card.sprite = Resources.Load<Sprite> (Constants.RESOURCE_GAMECARDS + currentCard);		
	}
}