using System;
using TSD.Droid.Services;
using TSD.Services.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidFileWork))]
namespace TSD.Droid.Services
{
    public class AndroidFileWork : IFileWork
    {
        public string GetBaseDir()
        {
            var path = Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath;

            return path;
        }
    }
}