using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageLatticeController : MonoBehaviour
{
    public float force = 10;
    public float radiusOfOneBulletImpact = 0.2f;
    public int numberOfNearLattices = 3;
    public float timeToLiveAfterShot = 6;

    public void AddForceOppositeOfPlayer(bool isRecursive)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Transform player = GameObject.Find("Player").transform;

        if (player == null)
        {
            Debug.Log("Player not found in CageLatticeController script in " + gameObject.name + " object");
            return;
        }

        // disabling joints
        FixedJoint[] joints = GetComponents<FixedJoint>();
        foreach(FixedJoint joint in joints)
        {
            //joint.connectedBody = null;
            joint.breakForce = 0;
        }
        // adding force
        Vector3 direction = gameObject.transform.position - player.position;
        if (rb != null)
        {
            rb.AddForce(direction * force, ForceMode.Impulse);
        }
        if (isRecursive)
        {
            Collider[] nearbyLattices = Physics.OverlapSphere(transform.position, radiusOfOneBulletImpact);
            foreach (var hitCollider in nearbyLattices)
            {
                if (hitCollider.GetComponent<CageLatticeController>() != null)
                {
                    hitCollider.GetComponent<CageLatticeController>().AddForceOppositeOfPlayer(false);
                    numberOfNearLattices--;
                    if (numberOfNearLattices <= 0) return;
                }
            }
        }
       
        // time to live
        gameObject.GetComponent<PieceDestroyer>().timeToLive = timeToLiveAfterShot;
    }
}
