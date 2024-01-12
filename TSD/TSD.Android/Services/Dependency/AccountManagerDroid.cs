using Android.Content;
using TSD.Droid.Services.Dependency;
using TSD.Services.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(AccountManagerDroid))]
namespace TSD.Droid.Services.Dependency
{
    public class AccountManagerDroid : IAccountManagerService
    {
        readonly ISharedPreferences _shared = MainActivity.Instance.GetSharedPreferences("RunningAssistant.preferences", FileCreationMode.Private);
        public string GetValue(string key) => _shared.GetString(key, "");

        public bool IsValue(string key) => _shared.Contains(key);

        public void SetValue(string key, string value) => _shared.Edit().PutString(key, value).Commit();

        public void DeleteValue(string key) => _shared.Edit().Remove(key).Commit();
    }
}