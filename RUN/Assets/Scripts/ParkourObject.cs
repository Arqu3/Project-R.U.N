using UnityEngine;
using System.Collections;

public class ParkourObject : MonoBehaviour
{

    public bool m_Wallrunnable;
    public bool m_Climbable;
    public bool m_VerticalClimbable;

    void Start()
    {

    }

    public bool GetClimbable()
    {
        return m_Climbable;
    }
}