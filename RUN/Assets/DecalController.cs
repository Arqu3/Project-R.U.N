using UnityEngine;
using System.Collections;

public class DecalController : MonoBehaviour {

    public int m_animIndex;
    public float m_delay;
    Animator m_animator;

	void Start () {
        m_animator = GetComponent<Animator>();

      //  StartAnimation();
        Invoke("StartAnimation", m_delay);
    }

    void StartAnimation()
    {
        m_animator.SetInteger("animIndex", m_animIndex);
        m_animator.SetTime(m_delay);

        
    }

}
