using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class OperationSlot : MonoBehaviour
{
    [SerializeField] private Rigidbody slotAnchor;
    public Operation SlottedOperation { get; private set; }
    private ConfigurableJoint slotJoint;
    private List<Operation> competitors;

    void Start()
    {
        competitors = new List<Operation>();
    }

    void Update()
    {
        SlotUpdate();
    }

    private void SlotUpdate()
    {
        //make the closest competitor the slotted operation
        if (competitors.Count == 0)
        {
            return;
        }

        Operation closestOperation = competitors[0];
        for (int i = 0; i < competitors.Count; i++)
        {
            if (Vector3.Distance(competitors[i].transform.position, transform.position) < Vector3.Distance(closestOperation.transform.position, transform.position))
            {
                closestOperation = competitors[i];
            }
        }

        if (closestOperation != SlottedOperation)
        {
            UnslotOperation();
            SlotOperation(closestOperation);
        }
    }

    private void SlotOperation(Operation operation)
    {
        //assign
        SlottedOperation = operation;

        //fetch the operation's rigidbody
        Rigidbody operationBody = null;
        if (operation.TryGetComponent<Rigidbody>(out Rigidbody thisBody))
        {
            operationBody = thisBody;
        }
        else
        {
            Debug.LogError("OperationSlot: Could not find Rigidbody on Operation to slot.");
            return;
        }

        //make physics joint from slotAnchor to slottedOperation
        slotJoint = slotAnchor.gameObject.AddComponent<ConfigurableJoint>();

        slotJoint.connectedBody = operationBody;
        slotJoint.autoConfigureConnectedAnchor = false;
        slotJoint.connectedAnchor = Vector3.zero;
        slotJoint.anchor = Vector3.zero;
        JointDrive jointDrive = new JointDrive();
        jointDrive.positionSpring = 1000f;
        jointDrive.positionDamper = 100f;
        jointDrive.maximumForce = Mathf.Infinity;
        slotJoint.xDrive = jointDrive;
        slotJoint.yDrive = jointDrive;
        slotJoint.zDrive = jointDrive;
        slotJoint.angularXDrive = jointDrive;
        slotJoint.angularYZDrive = jointDrive;
    }

    private void UnslotOperation()
    {
        if (SlottedOperation == null)
        {
            return;
        }

        Destroy(slotJoint);
        SlottedOperation = null;
        slotJoint = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        Operation thisOperation = null;
        if (other.TryGetComponent<Operation>(out Operation operation))
        {
            thisOperation = operation;
        }
        else
        {
            return;
        }
        competitors.Add(thisOperation);
    }

    private void OnTriggerExit(Collider other)
    {
        Operation thisOperation = null;
        if (other.TryGetComponent<Operation>(out Operation operation))
        {
            thisOperation = operation;
        }
        else
        {
            return;
        }

        if (thisOperation == SlottedOperation)
        {
            UnslotOperation();
        }

        competitors.Remove(thisOperation);
    }
}