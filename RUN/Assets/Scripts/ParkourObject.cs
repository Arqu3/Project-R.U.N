using UnityEngine;
using System.Collections;

public class ParkourObject : MonoBehaviour
{

    public bool m_Climbable;

    void Start()
    {

    }

    public bool GetClimbable()
    {
        return m_Climbable;
    }
}