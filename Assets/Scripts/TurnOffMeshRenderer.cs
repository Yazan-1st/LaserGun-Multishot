using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffMeshRenderer : MonoBehaviour
{   
    public bool RenderMeshes = false;

    void Start()
    {
        if (!RenderMeshes)
        {
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (GameObject go in allObjects)
            {
                MeshRenderer renderer = go.GetComponent<MeshRenderer>();

                if (renderer != null && go.tag != "Player")
                {
                    renderer.enabled = false;
                }
            }
        }
    }
}
