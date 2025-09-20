using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [field:SerializeField] public SlotType ItemType { get; private set; }
    [field: SerializeField] public string Value { get; private set; }
}