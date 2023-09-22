
public static class Debug
{
    public static void Log(object text){
        #if DEBUG
            Console.WriteLine("log: "+text);
        #endif
    }
}
