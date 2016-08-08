using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DisableInactiveParticles : MonoBehaviour
{
    ParticleSystem.Particle[] m_unused = new ParticleSystem.Particle[1];

    void Awake()
    {
        GetComponent<ParticleSystemRenderer>().enabled = false;
    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
        GetComponent<ParticleSystemRenderer>().enabled = GetComponent<ParticleSystem>().GetParticles(m_unused) > 0;
	}
}
