using System;
using System.IO;
using TSD.Droid.Services.Database;
using TSD.Services.DataBase.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(PathDroid))]
namespace TSD.Droid.Services.Database
{
    public class PathDroid : IPath
    {
        public string GetDatabasePath(string filename) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filename);
    }
}