using System;
using System.IO;
using SQLite;
using Xamarin.Forms;
using BjjAcademy.Droid;
using BjjAcademy.Droid.Persistence;
using BjjAcademy.Persistence;

[assembly: Dependency(typeof(SQLiteDb))]

namespace BjjAcademy.Droid.Persistence
{
    class SQLiteDb : ISQLiteDb
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documentsPath, DbName.DatabaseName);

            return new SQLiteAsyncConnection(path);
        }
    }
}