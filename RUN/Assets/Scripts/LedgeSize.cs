using UnityEngine;
using System.Collections;

public class LedgeSize : MonoBehaviour
{
    BoxCollider m_Collider;
    Vector3 m_ParentScale;

	// Use this for initialization
	void Start()
    {
        m_Collider = GetComponent<BoxCollider>();
        m_ParentScale = GetComponentInParent<Transform>().lossyScale;

        //Debug.Log(m_ParentScale);
	}
	
	// Update is called once per frame
	void Update()
    {
        m_ParentScale = GetComponentInParent<Transform>().lossyScale;
	}
}
