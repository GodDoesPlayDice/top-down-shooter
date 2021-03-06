using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAttack : MonoBehaviour
{
    public float damagePerSecond = 1;
    public float areaRadius = 3;
    public bool givesInvulnerability = false;
    public GameObject auraObj;

    public GameObject electricGround;

    [HideInInspector]
    public bool isEnabled = false;

    private Vector3 pos;
    private Target ownerTarget;

    private Material electricGroundMat;

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent<Target>(out ownerTarget);
        electricGroundMat = electricGround.GetComponent<MeshRenderer>().material;
        electricGroundMat.SetFloat("LightingDiameter", areaRadius * 2);
    }

    // Update is called once per frame
    void Update()
    {
        pos = transform.position;
        if (isEnabled)
        {
            electricGroundMat.SetVector("PlayerPos", new Vector2(transform.position.x, transform.position.z));

            // damage
            Collider[] damagedObjects = Physics.OverlapSphere(pos, areaRadius);
            foreach (var hitCollider in damagedObjects)
            {
                Target target;
                hitCollider.gameObject.TryGetComponent<Target>(out target);
                if (target != null && target.CompareTag("Enemy"))
                {
                    target.TakeDamage(damagePerSecond * Time.deltaTime);
                }
            }
            // invulnerability
            if (givesInvulnerability && ownerTarget)
            {
                ownerTarget.invulnerability = true;
            }

            // aura
            if (auraObj != null) auraObj.SetActive(true);
        }
        else if(!isEnabled)
        {
            if (ownerTarget)
            {
                ownerTarget.invulnerability = false;
            }
            // aura
            if (auraObj != null) auraObj.SetActive(false);

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, areaRadius);
    }
}
