using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBoxTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sheep"))
        {
            SheepController sheep = other.GetComponent<SheepController>();
            if (sheep.SheepStateProp == SheepController.SheepState.Running)
            {
                sheep.DoJumpOverFence();
            }
            
        }
    }
}
