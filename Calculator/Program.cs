using System;
using System.Globalization;

class Program
{
    static void Main()
    {
        Console.WriteLine("Simple Calculator. Type 'exit' to quit.");
        while (true)
        {
            Console.Write("Enter arithmetic expression: ");
            string? input = Console.ReadLine();
            if (input == null || input.Trim().ToLower() == "exit") break;
            try
            {
                double result = new Parser(input).Parse();
                Console.WriteLine("Result: " + result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: Invalid expression. " + ex.Message);
            }

            // Hi it's Meir!!!!
        }
    }

    class Parser
    {
        private readonly string text;
        private int pos;

        public Parser(string text)
        {
            this.text = text;
            this.pos = 0;
        }

        public double Parse()
        {
            double val = ParseExpression();
            SkipWhitespace();
            if (pos != text.Length)
                throw new Exception("Unexpected characters at end of expression.");
            return val;
        }

        // Parse addition and subtraction
        private double ParseExpression()
        {
            double val = ParseTerm();
            while (true)
            {
                SkipWhitespace();
                if (Match('+')) val += ParseTerm();
                else if (Match('-')) val -= ParseTerm();
                else break;
            }
            return val;
        }

        // Parse multiplication and division
        private double ParseTerm()
        {
            double val = ParsePower();
            while (true)
            {
                SkipWhitespace();
                if (Match('*')) val *= ParsePower();
                else if (Match('/')) val /= ParsePower();
                else break;
            }
            return val;
        }

        // Parse exponentiation (right-associative)
        private double ParsePower()
        {
            double val = ParseFactor();
            SkipWhitespace();
            if (Match('^'))
            {
                // Right-associative: a^b^c = a^(b^c)
                val = Math.Pow(val, ParsePower());
            }
            return val;
        }

        // Parse unary minus, parentheses, and absolute value
        // Parse unary plus/minus, parentheses, and absolute value
        private double ParseFactor()
        {
            SkipWhitespace();

            int sign = 1;
            while (true)
            {
                if (Match('+')) continue;
                if (Match('-')) { sign = -sign; continue; }
                break;
            }

            if (Match('|'))
            {
                double val = ParseExpression();
                if (!Match('|')) throw new Exception("Mismatched absolute value bars");
                return Math.Abs(sign * val);
            }

            if (Match('('))
            {
                double val = ParseExpression();
                if (!Match(')')) throw new Exception("Mismatched parentheses");
                return sign * val;
            }

            if (char.IsLetter(Peek()))
            {
                string func = ParseIdentifier();
                if (!Match('(')) throw new Exception($"Expected '(' after function name '{func}'");
                double arg1 = ParseExpression();
                double result;

                if (func.ToLower() == "sqrt")
                {
                    result = Math.Sqrt(arg1);
                }
                else if (func.ToLower() == "sin")
                {
                    result = Math.Sin(arg1);
                }
                else if (func.ToLower() == "cos")
                {
                    result = Math.Cos(arg1);
                }
                else if (func.ToLower() == "tan")
                {
                    result = Math.Tan(arg1);
                }
                else if (func.ToLower() == "log")
                {
                    result = Math.Log(arg1); 
                }
                else if (func.ToLower() == "abs")
                {
                    result = Math.Abs(arg1);
                }
                else if (func.ToLower() == "pow")
                {
                    if (!Match(',')) throw new Exception("Expected ',' between pow arguments");
                    double arg2 = ParseExpression();
                    result = Math.Pow(arg1, arg2);
                }
                else
                {
                    throw new Exception($"Unknown function '{func}'");
                }

                if (!Match(')')) throw new Exception("Expected ')' after function arguments");

                return sign * result;
            }

            return sign * ParseNumber();
        }

        private double ParseNumber()
        {
            SkipWhitespace();
            int start = pos;
            bool hasDot = false;
            while (pos < text.Length && (char.IsDigit(text[pos]) || text[pos] == '.'))
            {
                if (text[pos] == '.')
                {
                    if (hasDot) throw new Exception("Invalid number format");
                    hasDot = true;
                }
                pos++;
            }
            if (start == pos) throw new Exception("Expected number");
            string numStr = text.Substring(start, pos - start);
            if (!double.TryParse(numStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                throw new Exception("Invalid number: " + numStr);
            return value;
        }

        private void SkipWhitespace()
        {
            while (pos < text.Length && char.IsWhiteSpace(text[pos])) pos++;
        }

        private bool Match(char c)
        {
            SkipWhitespace();
            if (pos < text.Length && text[pos] == c)
            {
                pos++;
                return true;
            }
            return false;
        }

        private char Peek()
        {
            SkipWhitespace();
            return pos < text.Length ? text[pos] : '\0';
        }

        private string ParseIdentifier()
        {
            SkipWhitespace();
            int start = pos;
            while (pos < text.Length && char.IsLetter(text[pos]))
            {
                pos++;
            }
            if (start == pos) throw new Exception("Expected identifier");
            return text.Substring(start, pos - start);
        }
    }
}