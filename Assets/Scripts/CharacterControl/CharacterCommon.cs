using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using CS4455.Utility;

public class CharacterCommon : MonoBehaviour{
    public class GroundHit
    {
        public float DistanceToGround {get;set;}
        public GameObject ClosestGround {get;set;}
        public bool IsJumpable {get;set;}
    }
    public GroundHit gh;

    void Awake()
    {
        gh = new GroundHit();
        gh.DistanceToGround = 100f;
        gh.IsJumpable = false;
    }


    public void CheckGroundNear(
        Vector3 charPos,       
        float jumpableGroundNormalMaxAngle, 
        float rayDepth, //how far down from charPos will we look for ground?
        float rayOriginOffset //charPos near bottom of collider, so need a fudge factor up away from there
    ) 
    {
        bool _isJumpable = false;
        gh.DistanceToGround = 1000f;


        float totalRayLen = rayOriginOffset + rayDepth;

        Ray ray = new Ray(charPos + Vector3.up * rayOriginOffset, Vector3.down);

        int layerMask = 1 << LayerMask.NameToLayer("Default");


        RaycastHit[] hits = Physics.RaycastAll(ray, totalRayLen, layerMask);
        Array.Sort(hits, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));

        RaycastHit groundHit = new RaycastHit();

        foreach(RaycastHit hit in hits)
        {

            if (hit.collider.gameObject.CompareTag("ground") || hit.collider.gameObject.CompareTag("Plane"))
            {           

                groundHit = hit;

                _isJumpable = Vector3.Angle(Vector3.up, hit.normal) < jumpableGroundNormalMaxAngle;

                gh.ClosestGround = hit.collider.gameObject;
                float distance = hit.distance - rayOriginOffset - 1;
                if (distance < .001) distance = 0;
                gh.DistanceToGround = distance;
                gh.IsJumpable = _isJumpable;

                break; //only need to find the ground once

            }

        }

        Helper.DrawRay(ray, totalRayLen, hits.Length > 0, groundHit, Color.magenta, Color.green);

    }

}
