using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableEquation : MonoBehaviour
{
    [SerializeField] private BaseSlot[] slots;

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

        foreach (BaseSlot slot in slots)
        {
            if (!slot.HasItem)
            {
                equation += "_";
                continue;
            }

            // Try as Number
            if (slot.SlottedObject is Number number)
            {
                equation += number.Value;
            }
            // Try as Operation
            else if (slot.SlottedObject is Operation operation)
            {
                equation += operation.Value;
            }
        }

        Debug.Log(equation);
        return equation; //placeholder
    }
}
