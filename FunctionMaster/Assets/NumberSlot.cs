using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class NumberSlot : MonoBehaviour
{
    [SerializeField] private Rigidbody slotAnchor;
    public Number SlottedNumber { get; private set; }
    private ConfigurableJoint slotJoint;
    private List<Number> competitors;
    // Start is called before the first frame update
    void Start()
    {
        competitors = new List<Number>();
    }

    // Update is called once per frame
    void Update()
    {
        SlotUpdate();
    }

    private void SlotUpdate()
    {
        //make the closest competitor the slotted number
        if (competitors.Count == 0)
        {
            return;
        }

        Number closestNumber = competitors[0];
        for (int i = 0; i < competitors.Count; i++)
        {
            if (Vector3.Distance(competitors[i].transform.position, transform.position) < Vector3.Distance(closestNumber.transform.position, transform.position))
            {
                closestNumber = competitors[i];
            }
        }

        if (closestNumber != SlottedNumber)
        {
            UnslotNumber();
            SlotNumber(closestNumber);
        }
    }

    private void SlotNumber(Number number)
    {
        //assign
        SlottedNumber = number;

        //fetch the number's rigidbody
        Rigidbody numberBody = null;
        if (number.TryGetComponent<Rigidbody>(out Rigidbody thisBody))
        {
            numberBody = thisBody;
        }
        else
        {
            Debug.LogError("NumberSlot: Could not find Rigidbody on Number to slot.");
            return;
        }

        //make physics joint from slotAnchor to slottedNumber
        slotJoint = slotAnchor.gameObject.AddComponent<ConfigurableJoint>();

        slotJoint.connectedBody = numberBody;
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

    private void UnslotNumber()
    {
        if(SlottedNumber == null)
        {
            return;
        }

        Destroy(slotJoint);
        SlottedNumber = null;
        slotJoint = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        Number thisNumber = null;
        if (other.TryGetComponent<Number>(out Number number))
        {
            thisNumber = number;
        }
        else
        {
            return;
        }
        competitors.Add(thisNumber);
    }

    private void OnTriggerExit(Collider other)
    {
        Number thisNumber = null;
        if (other.TryGetComponent<Number>(out Number number))
        {
            thisNumber = number;
        }
        else
        {
            return;
        }

        if(thisNumber == SlottedNumber)
        {
            UnslotNumber();
        }

        competitors.Remove(thisNumber);
    }
}
