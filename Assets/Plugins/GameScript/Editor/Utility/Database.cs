using System.IO;

public static class Database
{
    public static string SQLitePathToURI(string path)
    {
        return "URI=file:" + path;
    }

    public static void WriteLine(StreamWriter writer, int depth, string toWrite)
    {
        if (depth > 0) writer.WriteLine(new string(' ', depth * 4) + toWrite);
        else writer.WriteLine(toWrite);
    }
}
