using UnityEngine;
using System.Collections;

public class AnimationHandler : MonoBehaviour
{
    //Public vars
    public bool m_AnimationStarted;
    public bool m_AnimationFinished;

    //Component vars
    Animator m_Animator;
    ControllerPlayer m_CPlayer;
    Hands m_Hands;

    //Other vars
    bool m_IsClimbing;

	void Start()
    {
        m_CPlayer = GetComponentInChildren<ControllerPlayer>();
        m_Hands = GetComponentInChildren<Hands>();

        m_AnimationStarted = false;
        m_AnimationFinished = true;
        m_IsClimbing = false;

        m_Animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update()
    {
    }

    void LateUpdate()
    {
        //Reset local position when animation is finished
        if (m_AnimationStarted && m_AnimationFinished)
        {
            m_AnimationStarted = false;
            transform.parent.position = transform.position;
            transform.localPosition = Vector3.zero;
        }
    }

    public void PlayAnimation(string name)
    {
        m_AnimationStarted = true;
        m_AnimationFinished = false;

        int temp = Animator.StringToHash(name);

        if (m_Animator.HasState(0, temp))
        {
            m_Animator.Play(name);

            if (name == "Climb")
            {
                m_IsClimbing = true;
            }
        }
        else
        {
            Debug.Log("Could not find animation with name: " + name);
            return;
        }
    }

    public void PlayAnimation(int id)
    {
        m_AnimationStarted = true;
        m_AnimationFinished = false;

        m_Animator.Play(id);
        m_CPlayer.ToggleControls(false);
    }

    public void animationFinished()
    {
        m_AnimationFinished = true;
        if (m_IsClimbing)
        {
            m_Hands.SetVelocity();
            m_IsClimbing = false;
        }
    }
}
