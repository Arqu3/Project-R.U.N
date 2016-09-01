using UnityEngine;
using System.Collections;

//This script should go on an empty gameobject, all child-meshes on this gameobject will be combined to one
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class CombineMesh : MonoBehaviour
{
    void Awake()
    {
        //Get all children
        foreach (Transform child in transform)
        {
            child.position += transform.position;
        }

        //Set position and rotation
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        //Get meshfilters and combineinstances
        var meshfilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshfilters.Length - 1];
        int index = 0;

        //Loop through all child-meshes
        for (int i = 0; i < meshfilters.Length; i++)
        {
            //Skip rest of loop if current meshfilter doesn't have a shared mesh
            if (meshfilters[i].sharedMesh == null) continue;
            //Add current sharedmesh to the total combine
            combine[index].mesh = meshfilters[i].sharedMesh;
            //Get worldspace transform and add it to combine mesh transform
            combine[index++].transform = meshfilters[i].transform.localToWorldMatrix;
            meshfilters[i].GetComponent<Renderer>().enabled = false;
        }

        //Create final combined mesh and set the material from child material
        GetComponent<MeshFilter>().mesh = new Mesh();
        GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        GetComponent<Renderer>().material = meshfilters[1].GetComponent<Renderer>().sharedMaterial;
    }
}
