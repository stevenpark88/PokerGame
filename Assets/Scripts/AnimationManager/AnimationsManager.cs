using UnityEngine;
using System.Collections;

public class AnimationsManager
{
	AudioClip cardAudio;
	AudioClip chipAudio;
	public  AnimationsManager(AudioClip cardAudio,AudioClip chipsAudio){
		this.cardAudio = cardAudio;
		this.chipAudio = chipsAudio;
	}

	public void MoveCardsObject(GameObject src,GameObject dest) {
		//		iTween.MoveTo (src, iTween.Hash ("position", dest.transform.position, "easetype", iTween.EaseType.easeInOutExpo, "time", 1f));
		Transform transform = dest.transform;

		iTween.MoveTo (src, iTween.Hash (
			"position", transform.position, 
			"easetype", iTween.EaseType.easeInOutExpo, 
			"time", GameConstant.ANIM_CARD_TIME)
		);
		iTween.Stab(dest,iTween.Hash("audioclip",cardAudio,"pitch",1));
	}

	public void MoveChipsObject(GameObject src,GameObject dest) {
		//		iTween.MoveTo (src, iTween.Hash ("position", dest.transform.position, "easetype", iTween.EaseType.easeInOutExpo, "time", 1f));
		Transform transform = dest.transform;

		iTween.MoveTo (src, iTween.Hash (
			"position", transform.position, 
			"easetype", iTween.EaseType.easeInOutExpo, 
			"time", GameConstant.ANIM_CHIP_TIME));
		iTween.Stab(dest,iTween.Hash("audioclip",chipAudio,"pitch",1));
	}

	public void MoveObject(GameObject src,GameObject dest) {
		iTween.MoveTo (src, iTween.Hash (
			"position", dest.transform.position, 
			"easetype", iTween.EaseType.easeInOutBack, 
			"time", 0.5f));
	}
	public void WinningCardAnimation(GameObject gameObject) {
		iTween.MoveBy(gameObject, iTween.Hash("y",20, "easeType", "easeInOutExpo", "loopType", "none", "delay", .1));
	}
}

