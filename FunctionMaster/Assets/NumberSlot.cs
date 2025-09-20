using UnityEngine;

public class NumberSlot : BaseSlot
{
    protected override bool AcceptsType(Collider other, out Component component)
    {
        if (other.TryGetComponent<Number>(out Number num))
        {
            component = num;
            return true;
        }
        component = null;
        return false;
    }
}