using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class RagdollPart
{
    public Collider Collider;
    public Rigidbody Rb;

    public RagdollPart(Collider collider, Rigidbody rb)
    {
        Collider = collider;
        Rb = rb;
    }
}
public class RagdollController : MonoBehaviour
{
    public List<RagdollPart> RagdollParts = new List<RagdollPart>();
    public Animator Animator;

    [Button("Set Ragdoll Parts")]
    private void SetRagdollParts()
    {
        RagdollParts.Clear();
        
        var colliders = gameObject.GetComponentsInChildren<Collider>();

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                var rb = colliders[i].GetComponent<Rigidbody>();
                rb.useGravity = false;
                colliders[i].isTrigger = true;
                RagdollParts.Add(new RagdollPart(colliders[i],rb));
            }
        }
    }

    public void TurnOnRagdolls()
    {
        Animator.enabled = false;
        for (int i = 0; i < RagdollParts.Count; i++)
        {
            RagdollParts[i].Collider.isTrigger = false;
            RagdollParts[i].Rb.useGravity = true;
            RagdollParts[i].Rb.velocity = Vector3.zero;
        }
    }
    
    public void TurnOffRagdolls()
    {
        Animator.enabled = true;
        for (int i = 0; i < RagdollParts.Count; i++)
        {
            RagdollParts[i].Collider.isTrigger = true;
            RagdollParts[i].Rb.useGravity = false;
            RagdollParts[i].Rb.velocity = Vector3.zero;
        }
    }
}