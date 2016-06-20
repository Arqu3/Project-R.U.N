using UnityEngine;
using System.Collections;

public class AnimationHandler : MonoBehaviour
{
    Animator m_Animator;

    bool m_IsGrabbed = false;
    bool m_IsClimbing = false;
	
	void Start()
    {
        m_Animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update()
    {
        ClimbUpdate();
	}

    void IsGrabbed(bool state)
    {
        m_IsGrabbed = state;
    }

    void ClimbUpdate()
    {
        //Check if climbing or not
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Climb"))
        {
            m_IsClimbing = true;
        }
        else
        {
            m_IsClimbing = false;
        }

        //Play climb animation if flag is set and not climbing
        if (!m_IsClimbing && m_IsGrabbed)
        {
            m_Animator.SetBool("Climb", true);
        }
        else
        {
            m_Animator.SetBool("Climb", false);
        }
    }
}
