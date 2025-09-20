using Assets.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    internal static class FunctionParser
    {
        public static Function parse(string input)
        {
            input.Replace(" ", "");
            for (int i = 0; i < input.Length; i++) 
            {
                char c = input[i];
                if(c == '+')
                {
                    return new Add(parse(input.Substring(0, i)), parse(input.Substring(i + 1, input.Length - i - 1)));
                }else if (c == '-')
                {
                    if (i > 0)
                    {
                        return new Add(parse(input.Substring(0, i)), new Multiply(parse(input.Substring(i + 1)), new Constant(-1)));
                    }
                    else
                    {
                        return new Multiply(parse(input.Substring(1)), new Constant(-1));
                    }
                }
            }

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c == '*')
                {
                    return new Multiply(parse(input.Substring(0, i)), parse(input.Substring(i + 1, input.Length - i)));
                }
                else if (c == '/')
                {
                    return new Divide(parse(input.Substring(0, i)), parse(input.Substring(i + 1, input.Length - i)));
                }
            }

            if (input == "x")
            {
                return new XFunction();
            }
            else if (input == "y")
            {
                return new YFunction();
            }

            return new Constant(Double.Parse(input));
        }
    }
}
