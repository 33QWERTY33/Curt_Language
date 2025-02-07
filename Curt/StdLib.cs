namespace StdLib
{
    class Lib
    {
        public static object? show(object? msg)
        {
            Console.WriteLine(msg);
            return null;
        }

        public static string? ask(string msg)
        {
            Console.Write(msg);
            return Console.ReadLine();
        }
    }
}
