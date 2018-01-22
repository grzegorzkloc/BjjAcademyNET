using PCLStorage;
using Plugin.Media.Abstractions;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BjjAcademy.GlobalMethods
{
    public static class DbHelper
    {
        public static string PhotoDirectory = "People";

        public async static Task<Belt> GetChosenBelt(SQLiteAsyncConnection _connection, int BeltId)
        {
            Belt belt = await _connection.GetAsync<Belt>(BeltId);
            return belt;
        }

        public async static Task<int> GetChosenBeltId(SQLiteAsyncConnection _connection, int BeltIndex, int Stripes)
        {
            BeltColour chosenBeltColor = (BeltColour)BeltIndex;
            byte chosenNoStripes = (byte)Stripes;

            var desiredBelt = from s in _connection.Table<Belt>()
                              where s.BeltColour == chosenBeltColor && s.Stripes == chosenNoStripes
                              select s;
            var desiredBeltList = await desiredBelt.ToListAsync();

            Belt belt = desiredBeltList[0];
            return belt.Id;
        }

        public async static Task<string> GetBeltAndStripeInfo(SQLiteAsyncConnection _connection, int BeltId)
        {
            Belt belt = await GetChosenBelt(_connection, BeltId);

            return "Pas: " + GetBeltColorString(belt) + " " + GetBeltStripeString(belt);
        }

        public static string GetBeltColorString(Belt belt)
        {
            if (belt.BeltColour == BeltColour.White) return "Biały";
            else if (belt.BeltColour == BeltColour.Blue) return "Niebieski";
            else if (belt.BeltColour == BeltColour.Purple) return "Purpurowy";
            else if (belt.BeltColour == BeltColour.Brown) return "Brązowy";
            else if (belt.BeltColour == BeltColour.Black) return "Czarny";
            else return "";
        }

        public static string GetBeltStripeString(Belt belt)
        {
            if (belt.Stripes == 0) return "0 belek";
            else if (belt.Stripes == 1) return "1 belka";
            else if (belt.Stripes == 2) return "2 belki";
            else if (belt.Stripes == 3) return "3 belki";
            else if (belt.Stripes == 4) return "4 belki";
            else return "";
        }

        public static async Task<string> PutFilesInCorrectDirectory(MediaFile photo)
        {
            var file = await FileSystem.Current.GetFileFromPathAsync(photo.Path);
            string TargetDirectoryPath1 = file.Path.Replace(file.Name, "");
            string TargetDirectoryPath = TargetDirectoryPath1.Replace("temp", "People");
            IFileSystem current = FileSystem.Current;
            var result = await current.LocalStorage.CheckExistsAsync(TargetDirectoryPath);
            if (result == ExistenceCheckResult.NotFound) await current.LocalStorage.CreateFolderAsync(TargetDirectoryPath, CreationCollisionOption.OpenIfExists);
            await file.MoveAsync(file.Path.Replace("temp", "People"), NameCollisionOption.GenerateUniqueName);
            return file.Path;
        }
    }
}
