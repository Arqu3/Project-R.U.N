using UnityEngine;
using System.Collections;

public class BodyAnim : MonoBehaviour {

    Animator m_Anim;
    ControllerPlayer m_Player;



    int m_CurrentAnimation;

	// Use this for initialization
	void Start () {
        m_Anim = GetComponent<Animator>();
        m_Player = GetComponentInParent<ControllerPlayer>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (m_Player.GetState().Equals(MovementState.Idle))
        {
            PlayAnimation((int)MovementState.Idle);
        }
        else if (m_Player.GetState().Equals(MovementState.Moving))
        {
            PlayAnimation((int)MovementState.Moving);
        }
    }

    void PlayAnimation(int index)
    {
        m_Anim.SetInteger("State" ,index);

        m_CurrentAnimation = index;
    }

}
