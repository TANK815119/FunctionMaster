using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic base class for slots that attach objects via a ConfigurableJoint.
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public abstract class BaseSlot : MonoBehaviour
{
    [SerializeField] protected Rigidbody slotAnchor;

    protected ConfigurableJoint slotJoint;
    protected readonly List<Component> competitors = new List<Component>();

    public Component SlottedObject { get; protected set; }
    public bool HasItem => SlottedObject != null;

    protected virtual void Update()
    {
        SlotUpdate();
    }

    private void SlotUpdate()
    {
        if (competitors.Count == 0) return;

        // Find closest
        Component closest = competitors[0];
        float closestDist = Vector3.Distance(closest.transform.position, transform.position);

        for (int i = 1; i < competitors.Count; i++)
        {
            float dist = Vector3.Distance(competitors[i].transform.position, transform.position);
            if (dist < closestDist)
            {
                closest = competitors[i];
                closestDist = dist;
            }
        }

        if (closest != SlottedObject)
        {
            UnslotItem();
            SlotItem(closest);
        }
    }

    protected virtual void SlotItem(Component item)
    {
        SlottedObject = item;

        if (!item.TryGetComponent<Rigidbody>(out Rigidbody itemBody))
        {
            Debug.LogError($"{GetType().Name}: Could not find Rigidbody on {item.GetType().Name} to slot.");
            SlottedObject = null;
            return;
        }

        slotJoint = slotAnchor.gameObject.AddComponent<ConfigurableJoint>();
        slotJoint.connectedBody = itemBody;
        slotJoint.autoConfigureConnectedAnchor = false;
        slotJoint.connectedAnchor = Vector3.zero;
        slotJoint.anchor = Vector3.zero;

        JointDrive drive = new JointDrive
        {
            positionSpring = 1000f,
            positionDamper = 100f,
            maximumForce = Mathf.Infinity
        };
        slotJoint.xDrive = drive;
        slotJoint.yDrive = drive;
        slotJoint.zDrive = drive;
        slotJoint.angularXDrive = drive;
        slotJoint.angularYZDrive = drive;
    }

    protected virtual void UnslotItem()
    {
        if (SlottedObject == null) return;
        Destroy(slotJoint);
        SlottedObject = null;
        slotJoint = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (AcceptsType(other, out Component c))
        {
            competitors.Add(c);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (AcceptsType(other, out Component c))
        {
            if (c == SlottedObject) UnslotItem();
            competitors.Remove(c);
        }
    }

    // Derived classes define which component types are valid.
    protected abstract bool AcceptsType(Collider other, out Component component);
}