using UnityEngine;
using System.Collections;

public class BodyAnim : MonoBehaviour {

    Animator m_Anim;
    ControllerPlayer m_Player;

    string m_CurrentAnimation;

	// Use this for initialization
	void Start () {
        m_Anim = GetComponent<Animator>();
        m_Player = GetComponentInParent<ControllerPlayer>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (m_Player.GetState().Equals(MovementState.Idle))
        {

        }
	}

    void PlayAnimation(string name)
    {




        m_CurrentAnimation = name;
    }

}
