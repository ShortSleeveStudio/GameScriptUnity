namespace GameScript
{
    public static class Database
    {
        public static string SqlitePathToURI(string path)
        {
            return "URI=file:" + path;
        }
    }
}
