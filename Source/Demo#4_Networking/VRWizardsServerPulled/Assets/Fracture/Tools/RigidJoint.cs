using System.Collections.Generic;
using UnityEngine;

public class RigidJoint : MonoBehaviour
{
    public bool isFixedToWorld;

    [SerializeField] private List<RigidJoint> attachedJoints = new List<RigidJoint>();

    private bool isDead;

    public bool IsFixedToWorld
    {
        get
        {
            return isFixedToWorld;
        }
    }
    private bool IsCurrentlyFixed { get { return IsFixedToWorld || CheckAllAttachedJoints(); } }




    private bool ShouldUseGravity
    {
        get
        {
            if(isFixedToWorld)
            {
                return false;
            }

            return !IsCurrentlyFixed;
        }
    }

    private static HashSet<RigidJoint> walkedJoints; 

    private bool CheckAllAttachedJoints()
    {
        if (walkedJoints == null)
        {
            walkedJoints = new HashSet<RigidJoint>();
        }
        else
        {
            walkedJoints.Clear();
        }

        walkedJoints.Add(this);

        return AnyAttachedFixed(this);
    }

    private static bool AnyAttachedFixed(RigidJoint joint)
    {
        if (joint.isFixedToWorld) return true;

        if(!walkedJoints.Contains(joint))
        {
            walkedJoints.Add(joint);
        }

        if (joint.attachedJoints == null) return false;

        for (int i = 0; i < joint.attachedJoints.Count; i++)
        {
            if (joint.attachedJoints[i] == null || joint.attachedJoints[i].isDead) continue;
            if (walkedJoints.Contains(joint.attachedJoints[i])) continue;

            if (AnyAttachedFixed(joint.attachedJoints[i]))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsConnectedToJoint(Rigidbody joint)
    {
        if (attachedJoints == null) return false;
        for (int i = 0; i < attachedJoints.Count; i++)
        {
            if (attachedJoints[i] != null && attachedJoints[i].GetComponent<Rigidbody>() == joint) return true;
        }

        return false;
    }

    public void AttachToObject(Rigidbody rBody)
    {
        if(rBody == null)
        {
            isFixedToWorld = true;
            return;
        }

        RigidJoint joint = rBody.GetComponent<RigidJoint>() ?? rBody.gameObject.AddComponent<RigidJoint>();

        if (IsConnectedToJoint(joint.GetComponent<Rigidbody>())) return;
        
        attachedJoints.Add(joint); 
    }

    private void OnDrawGizmosSelected()
    {
        if (attachedJoints == null) return;

        Vector3 myCentre = GetCentre(transform);

        foreach(var j in attachedJoints) 
        {
            if (j == null) continue;

            Gizmos.color = IsCurrentlyFixed ? Color.green : Color.white;
            Gizmos.color = isFixedToWorld || j.IsFixedToWorld ? Color.red : Gizmos.color;

            Vector3 jointCentre = GetCentre(j.transform);
            Gizmos.DrawSphere(jointCentre, 0.1f);
            Gizmos.DrawLine(myCentre, jointCentre);
        }
    }

    private Vector3 GetCentre(Transform obj)
    {
        if(obj.GetComponent<Collider>() != null)
        {
            return obj.GetComponent<Collider>().bounds.center;
        }

        if(obj.GetComponent<Renderer>() != null)
        {
            return obj.GetComponent<Renderer>().bounds.center;
        }

        return transform.position;
    }

    private void Awake()
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }

    private void OnConnectedJointDead()
    {
        if (ShouldUseGravity)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().WakeUp();
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        isDead = true;
        foreach (var joint in attachedJoints)
        {
            if (joint == null) continue;

            joint.OnConnectedJointDead();
        }
    }
}
