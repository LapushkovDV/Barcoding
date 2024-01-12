using System;
using TSD.Model;
using TSD.Services.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TSD.Services.FileServices
{
    /// <summary>
    /// Константы для работы с сервисом FileService
    /// </summary>
    public static class FileConstants
    {
        static FileConstants()
        {
            BASE_FILE_FOLDER = DependencyService.Get<IFileWork>().GetBaseDir();
        }
        public static readonly string BASE_FILE_FOLDER;

        #region Константы
        public const string IN_FOLDER_LOCAL = "IN";
        public const string OUT_FOLDER_LOCAL = "OUT";
        public const string ARCHIVE_FOLDER_LOCAL = "ARCHIVE";
        #endregion
    }
}
