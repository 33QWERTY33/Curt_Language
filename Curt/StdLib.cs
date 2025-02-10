using Interpreting;

namespace StdLib
{
    class TypeHandling
    {
        public static bool checkStr(object val)
        {
            return val is string;
        }
        public static bool checkBool(object val)
        {
            return val is bool;
        }
        public static bool checkFloat(object val)
        {
            return val is float;
        }
    }
    class Lib
    {
        public static object abs(object arg1)
        {
            float val1 = TypeHandling.checkFloat(arg1) ? (float)arg1 : throw new RTE("In native function 'abs'", $"Invalid argument: \"{arg1}\", expected int type");
            return (float)Math.Abs((decimal)val1);
        }
        public static object ascii(object arg1)
        {
            string val1 = TypeHandling.checkStr(arg1) ? (string)arg1 : throw new RTE("In native function 'ascii'", $"Invalid argument: \"{arg1}\", expected string type");
            if (val1.Length != 1) throw new RTE("In native function 'ascii'", $"Only 1 character is valid, you had {val1.Length} values");
            char c = val1[0];
            return (float)c;
        }
        public static object ask(object arg1)
        {
            Console.Write(arg1);
            return Console.ReadLine();
        }
        public static object contains(object arg1, object arg2)
        {
            string val1 = TypeHandling.checkStr(arg1) ? (string)arg1 : throw new RTE("In native function 'contains'", $"Invalid argument: \"{arg1}\", expected string type for the string to search through");
            string val2 = TypeHandling.checkStr(arg2) ? (string)arg2 : throw new RTE("In native function 'contains'", $"Invalid argument: \"{arg2}\", expected string type for the string to search for");
            return val1.Contains(val2);
        }
        public static object endsWith(object arg1, object arg2)
        {
            string val1 = TypeHandling.checkStr(arg1) ? (string)arg1 : throw new RTE("In native function 'endsWith'", $"Invalid argument: \"{arg1}\", expected string type for the string to search through");
            string val2 = TypeHandling.checkStr(arg2) ? (string)arg2 : throw new RTE("In native function 'endsWith'", $"Invalid argument: \"{arg2}\", expected string type for the string to search for");
            return val1.EndsWith(val2);
        }
        public static object len(object arg1)
        {
            string val1 = TypeHandling.checkStr(arg1) ? (string)arg1 : throw new RTE("In native function 'length'", $"Invalid argument: \"{arg1}\", expected string type");
            return (float)val1.Length;
        }
        public static object numToStr(object arg1)
        {
            float val1 = TypeHandling.checkFloat(arg1) ? (float)arg1 : throw new RTE("In native function 'numToStr'", $"Invalid argument: \"{arg1}\", expected int type");
            return Convert.ToString(val1);

        }
        public static object pow(object arg1, object arg2)
        {
            float val1 = TypeHandling.checkFloat(arg1) ? (float)arg1 : throw new RTE("In native function 'pow'", $"Invalid argument: \"{arg1}\", expected int type");
            float val2 = TypeHandling.checkFloat(arg2) ? (float)arg2 : throw new RTE("In native function 'pow'", $"Invalid argument: \"{arg2}\", expected int type");
            return (float)Math.Pow(val1, val2);
        }
        public static object rand(object arg1)
        {
            float val1 = TypeHandling.checkFloat(arg1) ? (float)arg1 : throw new RTE("In native function 'rand'", $"Invalid argument: \"{arg1}\", expected int type");
            Random rng = new Random();
            return (float)rng.Next((int)val1);
        }

        public static object replace(object arg1, object arg2, object arg3)
        {
            string val1 = TypeHandling.checkStr(arg1) ? (string)arg1 : throw new RTE("In native function 'replace'", $"Invalid argument: \"{arg1}\", expected string type for the string subject to character replacing");
            string val2 = TypeHandling.checkStr(arg2) ? (string)arg2 : throw new RTE("In native function 'replace'", $"Invalid argument: \"{arg2}\", expected string type for the string to search for");
            string val3 = TypeHandling.checkStr(arg3) ? (string)arg3 : throw new RTE("In native function 'replace'", $"Invalid argument: \"{arg3}\", expected string type for the string to replace found strings with");

            return val1.Replace(val2, val3);
        }
        public static object? show(object? arg1)
        {
            Console.WriteLine(arg1);
            return null;
        }

        public static object strToNum(object arg1)
        {
            string val1 = TypeHandling.checkStr(arg1) ? (string)arg1 : throw new RTE("In native function 'strToNum'", $"Invalid argument: \"{arg1}\", expected string type");
            return float.Parse(val1);
        }
        public static object subStr(object arg1, object arg2, object arg3)
        {
            string val1 = TypeHandling.checkStr(arg1) ? (string)arg1 : throw new RTE("In native function 'subStr'", $"Invalid argument: \"{arg1}\", expected string type");
            float val2 = TypeHandling.checkFloat(arg2) ? (float)arg2 : throw new RTE("In native function 'subStr'", $"Invalid argument: \"{arg2}\", expected int type for start index of slice");
            float val3 = TypeHandling.checkFloat(arg3) ? (float)arg3 : throw new RTE("In native function 'subStr'", $"Invalid argument: \"{arg3}\", expected int type for end index of slice");

            return val1.Substring((int)val2, (int)val3);
        }

        public static object sqrt(object arg1)
        {
            float val1 = TypeHandling.checkFloat(arg1) ? (float)arg1 : throw new RTE("In native function 'sqrt'", $"Invalid argument: \"{arg1}\", expected int type");
            return (float)Math.Sqrt(val1);
        }

        public static object startsWith(object arg1, object arg2)
        {
            string val1 = TypeHandling.checkStr(arg1) ? (string)arg1 : throw new RTE("In native function 'startsWith'", $"Invalid argument: \"{arg1}\", expected string type for the string to search through");
            string val2 = TypeHandling.checkStr(arg2) ? (string)arg2 : throw new RTE("In native function 'startsWith'", $"Invalid argument: \"{arg2}\", expected string type for the string to search for");
            return val1.StartsWith(val2);
        }

        public static object toLower(object arg1)
        {
            string val1 = TypeHandling.checkStr(arg1) ? (string)arg1 : throw new RTE("In native function 'subStr'", $"Invalid argument: \"{arg1}\", expected string type");
            return val1.ToLower();
        }

        public static object trim(object arg1)
        {
            string val1 = TypeHandling.checkStr(arg1) ? (string)arg1 : throw new RTE("In native function 'subStr'", $"Invalid argument: \"{arg1}\", expected string type");
            return val1.Trim();
        }

        public static object toUpper(object arg1)
        {
            string val1 = TypeHandling.checkStr(arg1) ? (string)arg1 : throw new RTE("In native function 'subStr'", $"Invalid argument: \"{arg1}\", expected string type");
            return val1.ToUpper();
        }

        public static object locals()
        {
            foreach (KeyValuePair<string, object> pair in Interpreter.globals) { Console.WriteLine(pair.Key + " >> " + pair.Value); }
            return "You found my easter egg :). If python can do it, so can I!!";
        }
    }
}
