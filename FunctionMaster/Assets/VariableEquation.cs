using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableEquation : MonoBehaviour
{
    [SerializeField] private Slot[] slots;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EquationToString();
        }
    }
    public string EquationToString()
    {
        string equation = "";

        //for (int i = 0; i < slots.Length; i++)
        //{
        //    equation += slots[i].SlottedObject != null ? slots[i].SlottedObject. : "_";
        //}

        foreach (Slot slot in slots)
        {
            if (slot.SlottedItem == null)
            {
                equation += "_";
                continue;
            }

            equation += slot.SlottedItem.Value;
        }

        Debug.Log(equation);
        return equation; //placeholder
    }
}
