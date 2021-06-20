using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigController : MonoBehaviour
{
    public GameObject player;

    public float movementRapidity = 0.1f;

    private Vector3 offset;
    private PlayerController playerController;

    public enum constraintType
    {
        HandPos,
        HeadPos
    }

    public constraintType constraintSelect;
    // Start is called before the first frame update
   
    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (constraintSelect == constraintType.HandPos)
        {
            offset = new Vector3(0.0f, 1.5f, 0.0f);
            transform.position = Vector3.Lerp(transform.position, playerController.aimAtPosition + offset, movementRapidity);
        }

        if (constraintSelect == constraintType.HeadPos)
        {
            offset = new Vector3(0.0f, 1f, 0.0f);
            transform.LookAt(playerController.aimAtPosition + offset);
            
        }

    }
}