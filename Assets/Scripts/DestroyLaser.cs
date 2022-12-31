using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyLaser : MonoBehaviour
{
    public GameObject Laser;
    public Transform muzzle;

    void Start()
    {   
        Destroy(Laser, 0.1f);
    }

    private void FixedUpdate()
    {
        //transform.position += GameObject.FindGameObjectWithTag("Muzzle").transform.position;
        //Debug.Log("move to the muzzle");
    }
}
