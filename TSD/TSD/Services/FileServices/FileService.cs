using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace TSD.Services.FileServices
{
    /// <summary>
    /// Сервис раоты с файлами и папками ТСД
    /// </summary>
    public static class FileService
    {
        private readonly static string _pathInLocal = string.Empty;
        private readonly static string _pathOutLocal = string.Empty;
        private readonly static string _pathArchiveLocal = string.Empty;

        static FileService()
        {
            _pathInLocal = Path.Combine(FileConstants.BASE_FILE_FOLDER, FileConstants.IN_FOLDER_LOCAL);
            _pathOutLocal = Path.Combine(FileConstants.BASE_FILE_FOLDER, FileConstants.OUT_FOLDER_LOCAL);
            _pathArchiveLocal = Path.Combine(FileConstants.BASE_FILE_FOLDER, FileConstants.ARCHIVE_FOLDER_LOCAL);

            if (!Directory.Exists(_pathInLocal))
                Directory.CreateDirectory(_pathInLocal);

            if (!Directory.Exists(_pathOutLocal))
                Directory.CreateDirectory(_pathOutLocal);
        }

        /// <summary>
        /// Метод инициализации базовых папок
        /// </summary>
        public static void InitFolder()
        {
            if (!Directory.Exists(_pathInLocal))
                Directory.CreateDirectory(_pathInLocal);

            if (!Directory.Exists(_pathOutLocal))
                Directory.CreateDirectory(_pathOutLocal);

            if (!Directory.Exists(_pathArchiveLocal))
                Directory.CreateDirectory(_pathArchiveLocal);
        }

        /// <summary>
        /// Проверка на существование файлов в директории IN
        /// </summary>
        /// <returns>Выводит true - если в директории есть файлы, иначе false</returns>
        public static bool IsExistsFilesInFolderLocal()
        {
            var result = Directory.GetFiles(_pathInLocal).Length;

            return result > 0;
        }

        /// <summary>
        /// Проверка на существование файлов в директории OUT
        /// </summary>
        /// <returns>Выводит true - если в директории есть файлы, иначе false</returns>
        public static bool IsExistsFilesOutFolderLocal()
        {
            var result = Directory.GetFiles(_pathOutLocal).Length;

            return result > 0;
        }


        /// <summary>
        /// Метод вывода путей или названий файлов
        /// </summary>
        /// <param name="isOnlyName"> вывод только названий</param>
        /// <returns>Возвращает массив путей или названий файлов из папки IN</returns>
        public static List<string> GetInLocalFiles(bool isOnlyName = false)
        {
            var result = !isOnlyName ? Directory.GetFiles(_pathInLocal).ToList() : Directory.GetFiles(_pathInLocal).Select(f => Path.GetFileName(f)).ToList();

            return result;
        }

        /// <summary>
        /// Метод вывода путей или названий файлов
        /// </summary>
        /// <param name="isOnlyName"> вывод только названий</param>
        /// <returns>Возвращает массив путей или названий файлов из папки OUT</returns>
        public static List<string> GetOutLocalFiles(bool isOnlyName = false)
        {
            var result = !isOnlyName ? Directory.GetFiles(_pathOutLocal).ToList() : Directory.GetFiles(_pathOutLocal).Select(f => Path.GetFileName(f)).ToList();

            return result;
        }

        /// <summary>
        /// Удаление локального файла
        /// </summary>
        /// <param name="path">путь до файла</param>
        /// <returns>Если файл был успешно удален, то выводит true, иначе false</returns>
        public static bool DeleteLocalFile(string path)
        {
            if (!File.Exists(path))
                return false;

            File.Delete(path);

            return true;
        }

        /// <summary>
        /// Вывод пути до меню файла
        /// </summary>
        /// <returns>путь к меню</returns>
        public static string GetMenuFileName()
        {
            var menus = Directory.GetFiles(_pathInLocal, "*MENU_COMMON*.json", SearchOption.TopDirectoryOnly).ToList();

            if (menus.Count == 0)
            {
                return null;
            }

            if (menus.Count == 1)
            {
                return menus.FirstOrDefault();
            }

            var date = new List<DateTime>();

            foreach (var menu in menus) 
            {
                date.Add(File.GetLastWriteTimeUtc(menu)); 
            }

            var lastDate = date.Max();
            var indexLastDate = date.IndexOf(lastDate);
            var result = menus[indexLastDate];

            menus.RemoveAt(indexLastDate);

            foreach (var menu in menus)
            {
                File.Delete(menu);
            }

            return result;
        }

        /// <summary>
        /// Вывод путей до справочников
        /// </summary>
        /// <returns>Путь к справочнику по системному имени меню</returns>
        public static List<string> GetDictionariesFileName(string sysName)
        {
            return Directory.GetFiles(_pathInLocal, $"*GAL_COMMON*{sysName.ToUpper()}.json", SearchOption.TopDirectoryOnly).ToList();
        }

        /// <summary>
        /// Вывод пути пользователей на ТСД
        /// </summary>
        /// <returns>Путь к файлу</returns>
        public static string GetUsersFileName()
        {
            var users = Directory.GetFiles(_pathInLocal, $"*USERS_COMMON*.json", SearchOption.TopDirectoryOnly).ToList();

            if (users.Count == 0)
            {
                return null;
            }

            if (users.Count == 1)
            {
                return users.FirstOrDefault();
            }

            var date = new List<DateTime>();

            foreach (var user in users)
            {
                date.Add(File.GetLastWriteTimeUtc(user));
            }

            var lastDate = date.Max();
            var indexLastDate = date.IndexOf(lastDate);
            var result = users[indexLastDate];

            users.RemoveAt(indexLastDate);

            foreach (var user in users)
            {
                File.Delete(user);
            }

            return result;
        }

        /// <summary>
        /// Получение документов
        /// </summary>
        /// <param name="sysName">системное имя меню</param>
        /// <param name="login">логин пользователя</param>
        /// <param name="idDevice">id ТСД</param>
        /// <returns>Список путей документов</returns>
        public static List<string> GetDocuments(string sysName, string login, string idDevice)
        {
            return Directory.GetFiles(_pathInLocal, $"*GAL_{idDevice.ToLower()}*{login.ToUpper()}*{sysName.ToUpper()}.json", SearchOption.TopDirectoryOnly).ToList();
        }

        /// <summary>
        /// Получение всех документов
        /// </summary>
        /// <param name="sysName">системное имя меню</param>
        /// <param name="login">логин пользователя</param>
        /// <param name="idDevice">id ТСД</param>
        /// <returns>Список путей документов</returns>
        public static List<string> GetDocuments(string sysName, string idDevice)
        {
            return Directory.GetFiles(_pathInLocal, $"*GAL_{idDevice.ToLower()}*{sysName.ToUpper()}.json", SearchOption.TopDirectoryOnly).ToList();
        }


        /// <summary>
        /// Считывание данных json
        /// </summary>
        /// <param name="fileName">путь до json</param>
        /// <returns>Объект json</returns>
        public static JObject GetJsonFromFile(string fileName)
        {
            var data = File.ReadAllText(fileName);
            
            return string.IsNullOrEmpty(data) ? null : JObject.Parse(data);
        }

        /// <summary>
        /// Считывание данных json
        /// </summary>
        /// <param name="fileName">путь до json</param>
        /// <returns>Сохранен файл или нет</returns>
        public static bool SetJsonFileFile(JObject data, string fileName)
        {
            var path = Path.Combine(_pathOutLocal, fileName);
            var archivePath = Path.Combine(_pathArchiveLocal, fileName);

            File.WriteAllText(path, data.ToString(Formatting.Indented));
            File.WriteAllText(archivePath, data.ToString(Formatting.Indented));

            if (File.Exists(path))
            {
                return true;
            }

            return false;
        }
    }
}
