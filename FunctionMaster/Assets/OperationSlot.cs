using UnityEngine;

public class OperationSlot : BaseSlot
{
    protected override bool AcceptsType(Collider other, out Component component)
    {
        if (other.TryGetComponent<Operation>(out Operation op))
        {
            component = op;
            return true;
        }
        component = null;
        return false;
    }
}