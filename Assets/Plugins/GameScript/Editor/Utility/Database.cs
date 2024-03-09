public static class Database
{
    public static string SQLitePathToURI(string path)
    {
        return "URI=file:" + path;
    }
}
