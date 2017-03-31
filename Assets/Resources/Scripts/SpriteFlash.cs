using UnityEngine;
using System.Collections;
using HWTools;

/// <summary>
/// Controls Triggers for Wizard Spell Animations
/// </summary>
public class SpriteFlash : MonoBehaviour
{

	//Timer fade;

    /// <summary>
    /// Animator associated with the attached GameObject. Needs to match or override the "Spell" Controller.
    /// </summary>
    Animator spellcast;

	// Use this for initialization
	void Awake()
	{
		//fade = new Timer {
		//	Loop = false,
		//	End = 1,
		//	OnTick = (dt) => SetColor() };
        spellcast = this.GetComponent<Animator>();
	}

	void SetColor()
	{
		//GetComponent<SpriteRenderer>().color = 
			//Color.white.Mult(new Color(1, 1, 1, 1 - fade.Completion));
	}

	public void Blink()
	{
        //fade.Run();
        spellcast.SetTrigger("castSpell");
	}
}
