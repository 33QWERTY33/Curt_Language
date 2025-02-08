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
            float val1 = TypeHandling.checkFloat(arg1) ? (float)arg1 : throw new RTE("In native function 'abs'", $"Invalid argument type: {arg1}, expected int type");
            return (float)Math.Abs((decimal)val1); 
        }
        public static object ascii(object arg1)
        {
            string val1 = TypeHandling.checkStr(arg1) ? (string)arg1 : throw new RTE("In native function 'ascii'", $"Invalid argument type: {arg1}, expected string type");
            if (val1.Length != 1) throw new RTE("In native function 'ascii'", $"Only 1 character is valid, you had {val1.Length} values"); 
            char c = val1[0];
            return (float)c;
        }
        public static object ask(object arg1)
        {
            Console.Write(arg1);
            return Console.ReadLine();
        }
        public static object pow(object arg1, object arg2) 
        {
            float val1 = TypeHandling.checkFloat(arg1) ? (float)arg1 : throw new RTE("In native function 'pow'", $"Invalid argument type: {arg1}, expected int type");
            float val2 = TypeHandling.checkFloat(arg1) ? (float)arg1 : throw new RTE("In native function 'pow'", $"Invalid argument type: {arg1}, expected int type");
            return (float)Math.Pow(val1, val2);
        }
        public static object rand(object arg1)
        {
            float val1 = TypeHandling.checkFloat(arg1) ? (float)arg1 : throw new RTE("In native function 'rand'", $"Invalid argument type: {arg1}, expected int type");
            if (val1.GetType().Name == "Float") throw new RTE("In native function 'bin'", $"Invalid argument type: {val1}");
            Random rng = new Random();
            return (float)rng.Next((int)val1);
        }
        public static object? show(object? arg1)
        {
            Console.WriteLine(arg1);
            return null;
        }

        public static object sqrt(object arg1) 
        {
            float val1 = TypeHandling.checkFloat(arg1) ? (float)arg1 : throw new RTE("In native function 'sqrt'", $"Invalid argument type: {arg1}, expected int type");
            return (float)Math.Sqrt(val1); 
        }

        public static object locals()
        {
            foreach (string name in Interpreter.globals.Keys) { Console.WriteLine(name); }
            return "You found my easter egg :). If python can do it, so can I!!";
        }
    }
}
