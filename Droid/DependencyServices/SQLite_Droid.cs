using System;
using System.IO;
using Poc.Luis.Xamarin.Droid;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_Droid))]
namespace Poc.Luis.Xamarin.Droid
{
    public class SQLite_Droid : ISQLite
    {
        SQLiteConnection ISQLite.GetConn()
        {
            var sqliteFilename = "LuisXamarinPoc.db3";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libraryPath = Path.Combine(documentsPath, "..", "Library");
            var path = Path.Combine(libraryPath, sqliteFilename);
            var conn = new SQLiteConnection(path);
            return conn;
        }
    }
}