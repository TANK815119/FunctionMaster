using System.Collections;
using System.Collections.Generic;
using System.Text;
using Assets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VariableEquation : MonoBehaviour
{
    [SerializeField] private Slot[] slots;

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            string equation = EquationToString();

            if (ValidateNoUnderscores(equation) == false)
            {
                //equation still needs more cooking
                return;
            }

            equation = SanitizeForParser(equation);

            MeshGenerator meshGenerator = gameObject.AddComponent<MeshGenerator>();

            meshGenerator.ShapeMesh(equation);

            string curScene = SceneManager.GetActiveScene().name;
            if (curScene.Equals("Level1"))
            {
                Level1 level1 = FindObjectOfType<Level1>();
                level1.StartLevel();
            }
            else if (curScene.Equals("Level2")) {
                Level2 level2 = FindObjectOfType<Level2>();
                level2.StartLevel();
            }
            
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

    public static string SanitizeForParser(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            Debug.LogWarning("Empty input provided; defaulting to 0.");
            return "0";
        }

        // Remove whitespace
        string input = raw.Replace(" ", "");

        // Replace negative numbers: find '-' that signify a negative value
        StringBuilder converted = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (c == '-')
            {
                bool isNegativeNumber =
                    // Negative if it's at the start OR preceded by an operator or '('
                    (i == 0 || "+-*/(".Contains(input[i - 1])) &&
                    // And followed by a digit or decimal
                    (i + 1 < input.Length && (char.IsDigit(input[i + 1]) || input[i + 1] == '.'));

                if (isNegativeNumber)
                {
                    converted.Append('#'); // Replace negative sign with #
                    continue;
                }
            }

            converted.Append(c);
        }

        input = converted.ToString();

        // Keep only allowed characters
        StringBuilder filtered = new StringBuilder();
        foreach (char c in input)
        {
            if (char.IsDigit(c) || c == 'x' || c == 'y' ||
                c == '+' || c == '-' || c == '*' || c == '/' ||
                c == '(' || c == ')' || c == '.' || c == '#')
            {
                filtered.Append(c);
            }
        }

        input = filtered.ToString();

        // Insert '*' where multiplication is implied between a number and a variable
        StringBuilder multiplied = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            char current = input[i];
            multiplied.Append(current);

            if (i < input.Length - 1)
            {
                char next = input[i + 1];

                // Number followed by x or y -> insert '*'
                if ((char.IsDigit(current) || current == ')') && (next == 'x' || next == 'y' || next == '('))
                {
                    multiplied.Append('*');
                }
                // Variable or ')' followed by number or '(' -> insert '*'
                else if ((current == 'x' || current == 'y' || current == ')') && (char.IsDigit(next) || next == '('))
                {
                    multiplied.Append('*');
                }
            }
        }

        input = multiplied.ToString();

        // Balance parentheses
        int balance = 0;
        foreach (char c in input)
        {
            if (c == '(') balance++;
            else if (c == ')') balance--;
        }

        if (balance > 0)
        {
            input += new string(')', balance);
        }
        else if (balance < 0)
        {
            int toRemove = -balance;
            for (int i = input.Length - 1; i >= 0 && toRemove > 0; i--)
            {
                if (input[i] == ')')
                {
                    input = input.Remove(i, 1);
                    toRemove--;
                }
            }
        }

        if (string.IsNullOrEmpty(input))
        {
            Debug.LogWarning("Input became empty after sanitization; defaulting to 0.");
            return "0";
        }

        return input;
    }


    public static bool ValidateNoUnderscores(string raw)
    {
        if (raw.Contains("_"))
        {
            Debug.LogWarning("Input contains underscores, which are not allowed.");
            return false;
        }
        return true;
    }
}
