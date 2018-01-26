using SQLite;

namespace EFApp.Presistance
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    }
}
