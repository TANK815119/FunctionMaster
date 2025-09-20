using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic base class for slots that attach objects via a ConfigurableJoint.
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public abstract class BaseSlot<T> : MonoBehaviour where T : Component
{
    [SerializeField] private Rigidbody slotAnchor;

    public T SlottedItem { get; private set; }

    private ConfigurableJoint slotJoint;
    private readonly List<T> competitors = new List<T>();

    protected virtual void Update()
    {
        SlotUpdate();
    }

    private void SlotUpdate()
    {
        if (competitors.Count == 0) return;

        // Find closest competitor
        T closest = competitors[0];
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

        if (closest != SlottedItem)
        {
            UnslotItem();
            SlotItem(closest);
        }
    }

    private void SlotItem(T item)
    {
        SlottedItem = item;

        if (!item.TryGetComponent<Rigidbody>(out Rigidbody itemBody))
        {
            Debug.LogError($"{GetType().Name}: Could not find Rigidbody on {typeof(T).Name} to slot.");
            SlottedItem = null;
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

    private void UnslotItem()
    {
        if (SlottedItem == null) return;

        Destroy(slotJoint);
        SlottedItem = null;
        slotJoint = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<T>(out T item))
        {
            competitors.Add(item);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<T>(out T item))
        {
            if (item == SlottedItem) UnslotItem();
            competitors.Remove(item);
        }
    }
}
