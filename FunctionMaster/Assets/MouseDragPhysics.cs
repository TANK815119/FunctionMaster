using UnityEngine;
using UnityEngine.InputSystem; // Required for programmatic Input System access

public class MouseDragPhysics : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject draggedObject;
    private ConfigurableJoint dragJoint;
    private float dragDistance = 5f;
    private Vector3 initialOffset;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
        }
    }

    void Update()
    {
        // Access the mouse directly
        Mouse mouse = Mouse.current;
        if (mouse == null)
        {
            Debug.LogError("Mouse input device not found!");
            return;
        }

        // Start dragging on left mouse button down
        if (mouse.leftButton.wasPressedThisFrame)
        {
            StartDragging();
        }

        // Stop dragging on left mouse button release
        if (mouse.leftButton.wasReleasedThisFrame)
        {
            StopDragging();
        }

        // Update dragged object position
        if (draggedObject != null)
        {
            UpdateDragPosition();
        }
    }

    void StartDragging()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null && !rb.isKinematic)
            {
                draggedObject = hit.collider.gameObject;
                initialOffset = hit.point - draggedObject.transform.position;

                GameObject anchorObject = new GameObject("DragAnchor");
                anchorObject.transform.position = hit.point;

                dragJoint = draggedObject.AddComponent<ConfigurableJoint>();
                dragJoint.connectedBody = anchorObject.AddComponent<Rigidbody>();
                dragJoint.connectedBody.isKinematic = true;
                dragJoint.autoConfigureConnectedAnchor = false;
                dragJoint.anchor = initialOffset;
                dragJoint.connectedAnchor = Vector3.zero;

                dragJoint.xMotion = ConfigurableJointMotion.Free;
                dragJoint.yMotion = ConfigurableJointMotion.Free;
                dragJoint.zMotion = ConfigurableJointMotion.Free;
                dragJoint.angularXMotion = ConfigurableJointMotion.Locked;
                dragJoint.angularYMotion = ConfigurableJointMotion.Locked;
                dragJoint.angularZMotion = ConfigurableJointMotion.Locked;

                JointDrive jointDrive = new JointDrive
                {
                    positionSpring = 10000f,
                    positionDamper = 50f,
                    maximumForce = 1000f
                };
                dragJoint.xDrive = jointDrive;
                dragJoint.yDrive = jointDrive;
                dragJoint.zDrive = jointDrive;
            }
        }
    }

    void UpdateDragPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Vector3 targetPosition = ray.origin + ray.direction * dragDistance;

        if (dragJoint != null && dragJoint.connectedBody != null)
        {
            dragJoint.connectedBody.transform.position = targetPosition;
        }
    }

    void StopDragging()
    {
        if (draggedObject != null && dragJoint != null)
        {
            if (dragJoint.connectedBody != null)
            {
                Destroy(dragJoint.connectedBody.gameObject);
            }
            Destroy(dragJoint);
            draggedObject = null;
            dragJoint = null;
        }
    }
}