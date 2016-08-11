using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DisableInactiveParticles : MonoBehaviour
{
    ParticleSystem.Particle[] m_unused = new ParticleSystem.Particle[1];
    public bool m_bypass;

    void Awake()
    {
        GetComponent<ParticleSystemRenderer>().enabled = false;
    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
        if (!m_bypass)
            GetComponent<ParticleSystemRenderer>().enabled = GetComponent<ParticleSystem>().GetParticles(m_unused) > 0;
        else
            GetComponent<ParticleSystemRenderer>().enabled = true;

    }
}
