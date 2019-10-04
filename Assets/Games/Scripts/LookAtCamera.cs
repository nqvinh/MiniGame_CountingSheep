using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Camera mainCamera;
    public bool BillboardX = true;
    public bool BillboardY = true;
    public bool BillboardZ = true;
    public float OffsetToCamera;
    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
          mainCamera.transform.rotation * Vector3.up);
    }
}
