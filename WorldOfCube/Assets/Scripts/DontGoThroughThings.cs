﻿using UnityEngine;
using System.Collections;

public class DontGoThroughThings : MonoBehaviour
{
    public bool sendTriggerMessage = true;

    public LayerMask layerMask = -1; //make sure aren't in this layer
    public float skinWidth = 0.1f; //probably doesn't need to be changed

    private float minimumExtent;
    private float partialExtent;
    private float sqrMinimumExtent;
    private Vector3 previousPosition;
    private Rigidbody myRigidbody;
    private Collider myCollider;

    // Use this for initialization
    void Start()
    {
        //initialize values
        myRigidbody = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        previousPosition = myRigidbody.position;
        minimumExtent = Mathf.Min(myCollider.bounds.extents.x, myCollider.bounds.extents.y, myCollider.bounds.extents.z);
        partialExtent = minimumExtent * (1.0f - skinWidth);
        sqrMinimumExtent = minimumExtent * minimumExtent;

    }

    // Update is called once per frame
    void Update()
    {
        //have we moved more than our minimum extent?
        Vector3 movementThisStep = myRigidbody.position - previousPosition;
        float movementSqrMagnitude = movementThisStep.sqrMagnitude;

        if (movementSqrMagnitude > sqrMinimumExtent)
        {
            float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
            RaycastHit hitInfo;

            //check for obstructions we might have missed
            if (Physics.Raycast(previousPosition, movementThisStep, out hitInfo, movementMagnitude, layerMask.value))
            {
                if (!hitInfo.collider)
                    return;

                if (hitInfo.collider.isTrigger)
                    hitInfo.collider.SendMessage("OnTriggerEnter", myCollider);

                if (!hitInfo.collider.isTrigger)
                    myRigidbody.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent;
            }
        }

        previousPosition = myRigidbody.position;
    }
}
