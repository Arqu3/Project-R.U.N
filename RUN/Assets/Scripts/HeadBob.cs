using UnityEngine;
using System.Collections;

public class HeadBob : MonoBehaviour {

    public bool m_Active;
    public float m_HeadBobAmount;

    Vector3 originalPosition;
    SimpleSmoothMouseLook m_CameraLookScript;

    void Start()
    {
        originalPosition = transform.localPosition;
        m_CameraLookScript = GetComponent<SimpleSmoothMouseLook>();
    }


	void Update () {
        if (m_Active)
        {
            int direction = -1;

            if (transform.localPosition.y > originalPosition.y + m_HeadBobAmount)
            {
                direction = 1;
            }

            //transform.localPosition += 
        }
	}
}
