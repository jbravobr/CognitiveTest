namespace Poc.Luis.Xamarin
{
    public interface ISQLite
    {
        SQLite.SQLiteConnection GetConn();
    }
}