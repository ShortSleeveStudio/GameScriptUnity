namespace GameScript
{
    public static class DbHelper
    {
        public static string SqlitePathToURI(string path) => "URI=file:" + path;
    }
}
