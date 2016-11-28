using UnityEngine;
using System.Collections;

public class BodyAnim : MonoBehaviour {

    Animator m_Anim_Legs, m_Anim_Arms;

    ControllerPlayer m_Player;

    int m_CurrentAnimation;

	// Use this for initialization
	void Start () {
        Animator[] anims = GetComponentsInChildren<Animator>();

        m_Anim_Legs = anims[0];
        m_Anim_Arms = Camera.main.GetComponentInChildren<Animator>();

        m_Player = GetComponentInParent<ControllerPlayer>();
	}
	
	// Update is called once per frame
	void Update () {
        PlayAnimation((int)m_Player.GetState());
    }

    void PlayAnimation(int index)
    {
        m_Anim_Legs.SetInteger("State" ,index);
        m_Anim_Arms.SetInteger("State", index);
        m_CurrentAnimation = index;
    }

}
