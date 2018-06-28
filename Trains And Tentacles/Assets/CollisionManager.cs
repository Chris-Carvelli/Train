using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<RobotJoint>() != null)
        {
            bool onTrack = other.GetComponentInParent<TentactleController>().onTrack;

            if (onTrack)
            {
                Debug.Log("Hit");
            } else
            {
                Debug.Log("Tentacle Destroyed");
            }
        }
    }
}
