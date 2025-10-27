using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepEmitter : MonoBehaviour {

    public float footstepWeight = 1;

    public void ExecuteFootstep() {

        EventManager.TriggerEvent<FootstepEvent, Vector3, float>(transform.position, footstepWeight);
    }
}
