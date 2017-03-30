using SQLite;

namespace Poc.Luis.Xamarin
{
    public class BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
    }
}