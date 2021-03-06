﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    public LayerMask Layers;
    public float GrabDistance;
    public Vector3 GrabOffset;

    public float HoldDistance;
    public float HoldElevation;

    Joint joint;
    public bool grabbing;

    public bool HoldingHeavy { get; private set; }

    [ContextMenu("Grab")]
    public void TryGrab()
    {
        if (grabbing) return;

        RaycastHit hit;
        for (int i = 0; i < 5; i++)
        {
            var dir = Quaternion.AngleAxis(i * 10f, transform.right) * transform.forward;

            Ray r = new Ray(transform.TransformPoint(GrabOffset), dir);

            Debug.DrawRay(r.origin, r.direction * GrabDistance, Color.red, 1f);

            if (Physics.Raycast(r, out hit, GrabDistance, Layers))
            {
                var g = hit.collider.GetComponentInParent<Grabbable>();
                if (g != null)
                {
                    Grab(g, hit.point);
                    break;
                }
            }
        }


    }

    void Grab(Grabbable target, Vector3 point)
    {
        grabbing = true;

        HoldingHeavy = false;

        if (target.Heavy)
        {
            HoldingHeavy = true;

            var spring = gameObject.AddComponent<SpringJoint>();
            spring.autoConfigureConnectedAnchor = false;
            spring.connectedBody = target.GetComponent<Rigidbody>();
            spring.anchor = Vector3.forward * HoldDistance + Vector3.up * HoldElevation;
            spring.connectedAnchor = target.transform.InverseTransformPoint(point);
            spring.spring = 500;
            spring.damper = 100;

            joint = spring;
        }
        else
        {
            var fixedJoint = gameObject.AddComponent<FixedJoint>();
            fixedJoint.autoConfigureConnectedAnchor = false;
            fixedJoint.connectedBody = target.GetComponent<Rigidbody>();
            fixedJoint.anchor = Vector3.forward * HoldDistance + Vector3.up * HoldElevation;
            fixedJoint.connectedAnchor = target.transform.InverseTransformPoint(point);

            joint = fixedJoint;
        }
    }

    [ContextMenu("Release")]
    public void Release()
    {
        if (!grabbing) return;


        HoldingHeavy = false;

        grabbing = false;
        Destroy(joint);
        joint = null;
    }

    public Rigidbody CurrentlyGrabbedObject()
    {
        return joint != null ? joint.connectedBody : null;
    }
}
