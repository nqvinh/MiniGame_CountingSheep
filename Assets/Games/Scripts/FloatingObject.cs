using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [SerializeField]
    float floatOffset = 4;

    [SerializeField]
    float moveSpeed = 1;

    float startY = 0;
    // Start is called before the first frame update
    void Start()
    {
        startY = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float y = startY + Mathf.Sin(Time.time) / floatOffset;

        Vector3 curPos = this.transform.position;
        curPos.y = y;
       
        this.transform.position = curPos + this.transform.forward * moveSpeed;
    }
}
