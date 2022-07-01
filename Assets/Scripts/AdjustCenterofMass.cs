using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustCenterofMass : MonoBehaviour
{
    [SerializeField] Vector3 centerOfMass = Vector3.zero;
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass += centerOfMass;
    }

    // Update is called once per frame
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        var currentCenterOfMass = this.GetComponent<Rigidbody>().worldCenterOfMass;

        Gizmos.DrawSphere(currentCenterOfMass + centerOfMass, 0.125f);
    }
}
