using UnityEngine;
using System.Collections;

public class AnimationHandler : MonoBehaviour
{
    Animator m_Animator;
    ControllerPlayer m_CPlayer;

    public bool m_AnimationStarted;
    public bool m_AnimationFinished;

    bool m_IsClimbing;

	void Start()
    {
        m_CPlayer = GetComponentInChildren<ControllerPlayer>();

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
        if (m_AnimationStarted && m_AnimationFinished)
        {
            m_AnimationStarted = false;
            transform.parent.position = transform.position;
            transform.localPosition = Vector3.zero;
        }
    }

    public void ToggleClimb(bool state)
    {
        m_AnimationStarted = true;
        m_AnimationFinished = false;

        m_Animator.Play("Climb");
        m_CPlayer.ToggleControls(false);
    }

    public void animationFinished()
    {
        m_AnimationFinished = true;
        m_CPlayer.ToggleControls(true);
    }
}
