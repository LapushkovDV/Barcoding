using Microsoft.AppCenter.Crashes;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using TSD.Model;
using TSD.Model.User;
using TSD.Services.Converters;
using TSD.Services.DataBase.Interfaces;
using TSD.Services.DataBase.Tables;
using TSD.Services.Extensions;
using TSD.Services.Interfaces;
using TSD.Services.Tasks;
using TSD.Services.Tasks.Models;
using Xamarin.Forms;
using Document = TSD.Services.DataBase.Tables.Document;
using Menu = TSD.Services.DataBase.Tables.Menu;

namespace TSD.Services.DataBase
{
    /// <summary>
    /// Управление базой данных
    /// </summary>
    public static class WorkDataBase
    {
        #region Приватные поля
        readonly static string _dbPath = Resources.NameDataBase;
        readonly static string _path;
        readonly static SQLiteOpenFlags _flags = SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex;
        #endregion

        #region Статический конструктор
        static WorkDataBase()
        {
            _path = DependencyService.Get<IPath>().GetDatabasePath(_dbPath);
            UpdateTables();
        }
        #endregion

        #region Приватные методы
        private static void UpdateTables(bool isDelete = false)
        {
            using (var db = new SQLiteConnection(_path, _flags))
            {
                db.BeginTransaction();

                if (isDelete)
                {
                    db.DropTable<User>();
                    db.DropTable<Document>();
                    db.DropTable<Menu>();
                    db.DropTable<Field>();
                    db.DropTable<Row>();
                    db.DropTable<Column>();
                    db.DropTable<Status>();
                    db.DropTable<Dictionary>();
                    db.DropTable<DictionaryField>();
                }

                db.CreateTable<User>();
                db.CreateTable<Document>();
                db.CreateTable<Menu>();
                db.CreateTable<Field>();
                db.CreateTable<Row>();
                db.CreateTable<Column>();
                db.CreateTable<Status>();
                db.CreateTable<Dictionary>();
                db.CreateTable<DictionaryField>();

                db.Commit();
            }
        }

        private static void AddElementsByMenus(SQLiteConnection db, Document document, AbstractMenu menu)
        {
            RemoveElementsDocument(db, document);
            AddElementsDocument(db, document, menu);
        }

        private static void AddElementsDictionaryByMenus(SQLiteConnection db, Dictionary document, AbstractMenu menu)
        {
            RemoveElementsDictioanry(db, document);
            AddElementsDictionaryDocument(db, document, menu);
        }

        private static void RemoveElementsDocument(SQLiteConnection db, Document document)
        {
            db.Table<Field>().Delete(x => x.DocumentId == document.Id);

            foreach (var row in db.Table<Row>().Where(x => x.DocumentId == document.Id).ToList())
            {
                db.Table<Column>().Delete(x => x.RowId == row.Id);
            }

            db.Table<Row>().Delete(x => x.DocumentId == document.Id);
        }

        private static void RemoveElementsDictioanry(SQLiteConnection db, Dictionary document)
        {
            db.Table<DictionaryField>().Delete(x => x.DictionaryId == document.Id);
        }

        private static void AddElementsDocument(SQLiteConnection db, Document document, AbstractMenu menu)
        {
            AddFields(db, document, menu);
            AddColumns(db, document, menu);
        }

        private static void AddElementsDictionaryDocument(SQLiteConnection db, Dictionary document, AbstractMenu menu)
        {
            AddDictionaryFields(db, document, menu);
        }

        private static void AddFields(SQLiteConnection db, Document document, AbstractMenu menu)
        {
            foreach (var fields in menu.Fields)
            {
                var field = Converter.Convert(document.Id, fields);

                db.Insert(field);

                fields.Id = field.Id;
            }
        }

        private static void AddDictionaryFields(SQLiteConnection db, Dictionary document, AbstractMenu menu)
        {
            foreach (var fields in menu.Fields)
            {
                var field = Converter.ConvertDictionary(document.Id, fields);

                db.Insert(field);

                fields.Id = field.Id;
            }
        }

        private static void AddColumns(SQLiteConnection db, Document document, AbstractMenu menu)
        {
            foreach (AbstractColumns row in menu.ColumnsValue)
            {
                var rowValue = Converter.Convert(document.Id, row);

                db.Insert(rowValue);

                row.Id = rowValue.Id;

                foreach (var column in row.ColumnsElement)
                {
                    var columnValue = Converter.Convert(rowValue.Id, column);

                    db.Insert(columnValue);

                    column.Id = columnValue.Id;
                }
            }
        }

        private static User GetCurrentUser()
        {
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    return db.Table<User>().FirstOrDefault(x => x.Login == UserAccount.Login);
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(GetCurrentUser) } });

                    return null;
                }
            }
        }
        #endregion

        #region Публичные методы
        public static List<Document> GetAllDoc(Func<Document, bool> predicate)
        {
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    return db.Table<Document>().Where(predicate).ToList();
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(GetAllDoc) } });

                    return null;
                }
            }
        }

        public static int SaveUser(string login, int externalUserId = 0)
        {
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    var user = db.Query<User>($"SELECT * FROM {nameof(User)} WHERE Login = '{login}'").FirstOrDefault();

                    if (user is null)
                    {
                        db.BeginTransaction();

                        user = new User { Login = login, ExternalUserId = externalUserId };

                        db.Insert(user);
                        db.Commit();

                        return db.Query<User>($"SELECT * FROM {nameof(User)} WHERE Login = '{user.Login}'").FirstOrDefault().Id;
                    }
                    db.BeginTransaction();

                    user.ExternalUserId = externalUserId;

                    db.Update(user);
                    db.Commit();

                    return user.Id;
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(SaveUser) } });

                    return -1;
                }
            }
        }

        public static List<AbstractField> GetFieldsDoc(int id)
        {
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    return Converter.ConvertToAbstractFields(db.Query<Field>($"SELECT * FROM {nameof(Field)} WHERE DocumentId = {id}").ToList());
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(GetFieldsDoc) } });

                    return null;
                }
            }
        }

        public static int GetDocIdByField(Func<Field, bool> predicate)
        {
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    var document = db.Table<Field>().FirstOrDefault(predicate);
                    
                    return document != null ? document.DocumentId : 0;
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(GetDocIdByField) } });

                    return 0;
                }
            }
        }


        public static int GetDocIdByBarcode(string barcode)
        {
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    var document = db.Table<Document>().FirstOrDefault(x => x.Barcode == barcode);

                    return document != null ? document.Id : 0;
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(GetDocIdByBarcode) } });

                    return 0;
                }
            }
        }

        public static int GetDictionaryIdByBarcode(string barcode)
        {
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    var document = db.Table<Dictionary>().FirstOrDefault(x => x.Barcode == barcode);

                    return document != null ? document.Id : 0;
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(GetDictionaryIdByBarcode) } });

                    return 0;
                }
            }
        }

        public static void LoadCurrentDictionary(int id)
        {
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    var user = GetCurrentUser();

                    if (!(user is null))
                    {
                        var dictionary = db.Query<Dictionary>($"SELECT * FROM {nameof(Dictionary)} WHERE Id = {id}").FirstOrDefault();

                        if (!(dictionary is null))
                        {
                            UserAccount.SelectedMenu.CurrentDocument.Id = dictionary.Id;
                            UserAccount.SelectedMenu.CurrentDocument.IdDoc = dictionary.IdDoc;
                            UserAccount.SelectedMenu.CurrentDocument.Barcode = dictionary.Barcode;
                            UserAccount.SelectedMenu.CurrentDocument.IsAllowEditDoc = false;

                            var fields = db.Query<DictionaryField>($"SELECT * FROM {nameof(DictionaryField)} WHERE DictionaryId = {dictionary.Id}");
                            List<Row> rows = new List<Row>();

                            UserAccount.SelectedMenu.Fields = UserAccount.SelectedMenu.Fields.Select(x =>
                            {
                                x.Id = fields.FirstOrDefault(element => element.SysName == x.SysName).Id;
                                x.Value = fields.FirstOrDefault(element => element.SysName == x.SysName).Value;

                                return x;
                            }).ToObservableCollection();
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(LoadCurrentDictionary) } });
                }
            }
        }

        public static List<AbstractColumns> GetColumnsDoc(int id)
        {
            using (var db = new SQLiteConnection(_path, _flags))
            {
                var listColumns = new List<AbstractColumns>();
                try
                {

                    var rows = db.Query<Row>($"SELECT * FROM {nameof(Row)} WHERE DocumentId = {id}");

                    foreach (var row in rows)
                    {
                        var columnsValue = new AbstractColumns();
                        var columns = db.Query<Column>($"SELECT * FROM {nameof(Column)} WHERE RowId = {row.Id}").ToList();

                        listColumns.Add(new AbstractColumns { ColumnsElement = Converter.ConvertToAbstractColumn(columns).ToObservableCollection() });
                    }
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(GetColumnsDoc) } });
                }

                return listColumns;
            }
        }

        public static AbstractColumns GetColumnsByValue(int id, string searchText)
        {
            var result = new AbstractColumns();
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    var row = db.Query<Row>($"SELECT * FROM {nameof(Row)} WHERE DocumentId = {id} AND Id IN(SELECT RowId FROM {nameof(Column)} WHERE Value LIKE '{searchText}')").FirstOrDefault();

                    if (row != null)
                    {
                        result.IsBlock = row.IsBlockRow;
                        result.IsOpen = row.IsOpenRow;
                        result.Id = row.Id;
                        result.ColumnsElement = Converter.ConvertToAbstractColumn(db.Query<Column>($"SELECT * FROM {nameof(Column)} WHERE RowId = {row.Id}")).ToObservableCollection();

                        return result;
                    }
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(GetColumnsByValue) } });
                }

                return null;
            }

        }
        public static int SaveCurrentDictionery()
        {
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                        db.BeginTransaction();

                        var menubase = AddMenu(db, UserAccount.SelectedMenu);
                        var document = db.Table<Dictionary>().FirstOrDefault(x => x.IdDoc == UserAccount.SelectedMenu.CurrentDocument.IdDoc && x.MenuId == menubase.Id);

                        if (!(document is null))
                        {
                            document.Barcode = UserAccount.SelectedMenu.CurrentDocument.Barcode;

                            db.Update(document);
                        }
                        else
                        {
                            document = new Dictionary
                            {
                                IdDoc = UserAccount.SelectedMenu.CurrentDocument.IdDoc,
                                Barcode = UserAccount.SelectedMenu.CurrentDocument.Barcode,
                                MenuId = menubase.Id,
                            };

                            db.Insert(document);
                        }

                        AddElementsDictionaryByMenus(db, document, UserAccount.SelectedMenu);
                        db.Commit();

                        return document.Id;
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(SaveCurrentDictionery) } });
                }

                return -1;
            }
        }
        public static int SaveCurrentDocument(StatusEnum status)
        {
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    var user = GetCurrentUser();

                    if (!(user is null))
                    {
                        db.BeginTransaction();

                        var menubase = AddMenu(db, UserAccount.SelectedMenu);
                        var document = db.Table<Document>().FirstOrDefault(x => x.IdDoc == UserAccount.SelectedMenu.CurrentDocument.IdDoc && x.UserId == user.Id && x.MenuId == menubase.Id);

                        if (!(document is null))
                        {
                            document.Barcode = UserAccount.SelectedMenu.CurrentDocument.Barcode;
                            document.IsAllowEditDoc = UserAccount.SelectedMenu.CurrentDocument.IsAllowEditDoc;
                            document.StatusId = db.Get<Status>(x => x.Id == (int)status).Id;
                            document.GuidStatus = status == StatusEnum.Complete || status == StatusEnum.InWork ? string.Empty
                            : UserAccount.SelectedMenu.CurrentDocument.GuidStatus;
                            document.LastAction = UserAccount.SelectedMenu.CurrentDocument.LastAction;

                            db.Update(document);
                        }
                        else
                        {
                            document = new Document
                            {
                                IdDoc = UserAccount.SelectedMenu.CurrentDocument.IdDoc,
                                Barcode = UserAccount.SelectedMenu.CurrentDocument.Barcode,
                                IsAllowEditDoc = UserAccount.SelectedMenu.CurrentDocument.IsAllowEditDoc,
                                StatusId = db.Get<Status>(x => x.Id == (int)status).Id,
                                UserId = user.Id,
                                GuidStatus = status == StatusEnum.Complete || status == StatusEnum.InWork ? string.Empty
                                    : UserAccount.SelectedMenu.CurrentDocument.GuidStatus,
                                MenuId = menubase.Id,
                                LastAction = UserAccount.SelectedMenu.CurrentDocument.LastAction
                            };

                            db.Insert(document);
                        }

                        AddElementsByMenus(db, document, UserAccount.SelectedMenu);
                        db.Commit();

                        return document.Id;
                    }
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(SaveCurrentDocument) } });
                }

                return -1;
            }
        }
        public static void SaveStatusDocumentById(StatusEnum status, int id, string guid = null)
        {
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    var document = db.Table<Document>().FirstOrDefault(x => x.Id == id);

                    if (!(document is null))
                    {
                        document.StatusId = db.Get<Status>(x => x.Id == (int)status).Id;
                        document.GuidStatus = status == StatusEnum.Complete || status == StatusEnum.InWork ? string.Empty
                            : (guid ?? document.GuidStatus);

                        db.Update(document);
                    }

                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(SaveStatusDocumentById) } });
                }
            }
        }
        public static void LoadCurrentDocument(int id, int startPagination = 0, int take = 0, string searchText = null)
        {
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    var user = GetCurrentUser();

                    if (!(user is null))
                    {
                        var document = db.Query<Document>($"SELECT * FROM {nameof(Document)} WHERE Id = {id}").FirstOrDefault();

                        if (!(document is null))
                        {
                            UserAccount.SelectedMenu.CurrentDocument.Id = document.Id;
                            UserAccount.SelectedMenu.CurrentDocument.IdDoc = document.IdDoc;
                            UserAccount.SelectedMenu.CurrentDocument.Barcode = document.Barcode;
                            UserAccount.SelectedMenu.CurrentDocument.IsAllowEditDoc = document.IsAllowEditDoc;
                            UserAccount.SelectedMenu.CurrentDocument.StatusId = document.StatusId;
                            UserAccount.SelectedMenu.CurrentDocument.LastAction = document.LastAction;

                            var fields = db.Query<Field>($"SELECT * FROM {nameof(Field)} WHERE DocumentID = {document.Id}");
                            List<Row> rows = new List<Row>();

                            if (startPagination != 0)
                            {
                                if (searchText == null)
                                {
                                    rows = db.Query<Row>($"SELECT * FROM Row WHERE DocumentId = {document.Id} LIMIT {startPagination}, {take}");
                                }
                                else
                                {
                                    rows = db.Query<Row>($"SELECT * FROM Row WHERE DocumentId = {document.Id} AND Id IN(SELECT RowId FROM Column WHERE Value LIKE '{searchText}%') LIMIT {startPagination}, {take}");
                                }
                            }
                            else
                            {
                                if (searchText == null)
                                {
                                    rows = db.Query<Row>($"SELECT * FROM Row WHERE DocumentId = {document.Id} LIMIT {startPagination}, {take}");
                                }
                                else
                                {
                                    rows = db.Query<Row>($"SELECT * FROM Row WHERE DocumentId = {document.Id} AND Id IN(SELECT RowId FROM Column WHERE Value LIKE '{searchText}%') LIMIT {startPagination}, {take}");
                                }
                            }

                            UserAccount.SelectedMenu.Fields = UserAccount.SelectedMenu.Fields.Select(x =>
                            {
                                x.Id = fields.FirstOrDefault(element => element.SysName == x.SysName).Id;
                                x.Value = fields.FirstOrDefault(element => element.SysName == x.SysName).Value;

                                return x;
                            }).ToObservableCollection();

                            UserAccount.SelectedMenu.ColumnsValue.Clear();

                            AbstractColumns prevRow = null;
                            
                            foreach (var row in rows)
                            {
                                var columns = db.Query<Column>($"SELECT * FROM {nameof(Column)} WHERE RowId = {row.Id}");

                                var rowValue = new AbstractColumns
                                {
                                    Id = row.Id,
                                    IsBlock = row.IsBlockRow,
                                    IsOpen = row.IsOpenRow,
                                    ColumnsElement = columns.Select(element => Converter.Convert(element)).ToObservableCollection()
                                };

                                if (prevRow != null)
                                {
                                    rowValue.PrevRowId = prevRow.Id;
                                    prevRow.NextRowId = rowValue.Id;
                                }

                                UserAccount.SelectedMenu.ColumnsValue.Add(rowValue);

                                prevRow = UserAccount.SelectedMenu.ColumnsValue[UserAccount.SelectedMenu.ColumnsValue.Count - 1];
                            }
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(LoadCurrentDocument) } });
                }
            }
        }

        public static void UpdateDocumentByMenu(AbstractMenu menu, int startPagination = 0, int take = 0)
        {
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    var user = GetCurrentUser();

                    if (!(user is null))
                    {
                        var document = db.Table<Document>().
                            FirstOrDefault(x => x.Id == menu.CurrentDocument.Id);

                        if (!(document is null))
                        {
                            menu.CurrentDocument.Id = document.Id;
                            menu.CurrentDocument.IdDoc = document.IdDoc;
                            menu.CurrentDocument.Barcode = document.Barcode;
                            menu.CurrentDocument.IsAllowEditDoc = document.IsAllowEditDoc;
                            menu.CurrentDocument.StatusId = document.StatusId;
                            menu.CurrentDocument.LastAction = document.LastAction;

                            var fields = db.Table<Field>().Where(x => x.DocumentId == document.Id);
                            TableQuery<Row> rows = null;

                            if (startPagination != 0)
                            {
                                rows = db.Table<Row>().Where(x => x.DocumentId == document.Id).Skip(startPagination).Take(take);
                            }
                            else
                            {
                                rows = db.Table<Row>().Where(x => x.DocumentId == document.Id).Take(take);
                            }

                            menu.Fields = menu.Fields.Select(x =>
                            {
                                x.Value = fields.FirstOrDefault(element => element.SysName == x.SysName).Value;

                                return x;
                            }).ToObservableCollection();


                            menu.ColumnsValue.Clear();

                            AbstractColumns prevElement = null;

                            foreach(var row in rows)
                            {
                                var columns = db.Table<Column>().Where(column => column.RowId == row.Id);
                                var rowValue = new AbstractColumns
                                {
                                    Id = row.Id,
                                    IsBlock = row.IsBlockRow,
                                    IsOpen = row.IsOpenRow,
                                    ColumnsElement = columns.Select(element => Converter.Convert(element)).ToObservableCollection()
                                };

                                if (prevElement != null)
                                {
                                    rowValue.PrevRowId = prevElement.Id;
                                    prevElement.NextRowId = rowValue.Id;
                                }

                                menu.ColumnsValue.Add(rowValue);

                                prevElement = menu.ColumnsValue[menu.ColumnsValue.Count - 1];
                            }
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(UpdateDocumentByMenu) } });
                }
            }
        }

        public static void AddStatuses()
        {
            using (var db = new SQLiteConnection(_path, _flags))
            {
                if (db.Table<Status>().Count() == 0)
                {
                    db.BeginTransaction();

                    db.Insert(new Status() { Name = "В работе" });
                    db.Insert(new Status() { Name = "На синхронизации с сервером" });
                    db.Insert(new Status() { Name = "Передано в ERP" });
                    db.Insert(new Status() { Name = "Завершено" });
                    db.Insert(new Status() { Name = "Словарь" });

                    db.Commit();
                }
            }
        }

        public static int GetMenuId(AbstractMenu menu)
        {
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    if (menu == null) return -1;

                    var menuValue = db.Query<Menu>($"SELECT * FROM {nameof(Menu)} WHERE Action = '{menu.Action}'").FirstOrDefault();

                    if (menuValue == null) return -1;

                    return menuValue.Id;
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(GetMenuId) } });
                }

                return -1;
            }
        }

        public static DocumentTask GetColumnByIdent(string ident)
        {
            using (var db = new SQLiteConnection(_path))
            {
                try
                {
                    var documentId = db.Query<Field>($"SELECT * FROM {nameof(Field)} WHERE Value = '{ident}'").FirstOrDefault()?.DocumentId;

                    if (documentId != null && documentId != 0)
                    {
                        var document = db.Query<Document>($"SELECT * FROM {nameof(Document)} WHERE Id = {documentId}").FirstOrDefault();
                        var fields = db.Query<Field>($"SELECT * FROM {nameof(Field)} WHERE DocumentId = {documentId}");

                        return new DocumentTask
                        {
                            Id = document.Id,
                            IdDoc = document.IdDoc,
                            IsAllowEditDoc = document.IsAllowEditDoc,
                            StatusId = document.StatusId,
                            FieldsTasks = fields.Select(x => Converter.Convert(x)).ToObservableCollection()
                        };
                    }
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(GetColumnByIdent) } });
                }

                return null;
            }
        }

        //TaskViewModel - задачи по документам
        public static List<FieldsTask> GetFieldsTaskDoc(Document document)
        {
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    if (document != null)
                    {
                        return Converter.ConvertList(db.Table<Field>().Where(x => x.DocumentId == document.Id).ToList());
                    }
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(GetColumnByIdent) } });
                }

                return new List<FieldsTask>();
            }
        }

        public static List<DocumentTask> GetDocTasks(AbstractMenu menu = null, int layerSelect = 0)
        {
            using (var db = new SQLiteConnection(_path, _flags))
            {
                var documentTasks = new List<DocumentTask>();

                try
                {
                    var user = GetCurrentUser();

                    if (user != null)
                    {
                        var menuId = -1;

                        if (menu != null)
                        {
                            menuId = GetMenuId(menu);

                            if (menuId == -1) return documentTasks;
                        }

                        var documents = menuId == -1 ? (layerSelect == 1 ? GetAllDoc(x => x.UserId == user.Id && x.StatusId == (int)StatusEnum.InWork)
                            : (layerSelect == 2 ? GetAllDoc(x => x.UserId == user.Id && (x.StatusId == (int)StatusEnum.InWork || x.StatusId == (int)StatusEnum.Dictionary)) : GetAllDoc(x => x.UserId == user.Id && x.StatusId != (int)StatusEnum.Dictionary)))
                            : (layerSelect == 1 ? GetAllDoc(x => x.UserId == user.Id && x.MenuId == menuId && x.StatusId == (int)StatusEnum.InWork)
                            : (layerSelect == 2 ? GetAllDoc(x => x.UserId == user.Id && x.MenuId == menuId && (x.StatusId == (int)StatusEnum.InWork || x.StatusId == (int)StatusEnum.Dictionary)) : GetAllDoc(x => x.UserId == user.Id && x.MenuId == menuId && x.StatusId != (int)StatusEnum.Dictionary)));

                        foreach (var document in documents)
                        {
                            var fieldTask = GetFieldsTaskDoc(document).ToList();

                            documentTasks.Add(new DocumentTask
                            {
                                Id = document.Id,
                                FieldsTasks = fieldTask.ToObservableCollection(),
                                GuidStatus = document.GuidStatus,
                                StatusId = document.StatusId,
                                LastAction = document.LastAction,
                                MenuAction = db.Table<Menu>().FirstOrDefault(x => x.Id == document.MenuId).Action,
                                IdDoc = document.IdDoc,
                                Barcode = document.Barcode,
                                IsAllowEditDoc = document.IsAllowEditDoc
                            });
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(GetDocTasks) } });
                }

                return documentTasks;
            }
        }

        private static Menu AddMenu(SQLiteConnection db, AbstractMenu menu)
        {
            var menuBase = db.Table<Menu>().FirstOrDefault(x => x.Action == menu.Action);

            if (menuBase == null)
            {
                menuBase = new Menu { Action = menu.Action };
                db.Insert(menuBase);
            }

            return menuBase;
        }

        public static void OpenAsyncPhone() => DependencyService.Get<IWorkService>().StartForegroundServiceCompact();

        public static void CloseAsyncPhone() => DependencyService.Get<IWorkService>().StopForegroundServiceCompact();

        public static void IsAllUnblockPosition()
        {
            using (var db = new SQLiteConnection(_path))
            {
                try
                {
                    var user = GetCurrentUser();

                    if (!(user is null))
                    {
                        var positions = new List<Row>();
                        var documents = db.Table<Document>().
                           Where(x => x.UserId == user.Id);
                        foreach (var document in documents)
                        {
                            positions.AddRange(db.Table<Row>().Where(x => x.DocumentId == document.Id && x.IsBlockRow == true).Select(x => { x.IsBlockRow = false; return x; }).ToList());
                        }

                        if (positions.Count != 0)
                        {
                            db.UpdateAll(positions);
                            db.Commit();
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(IsAllUnblockPosition) } });
                }
            }
        }

        public static void DeleteDocument(int id)
        {
            CloseAsyncPhone();
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    db.Table<Field>().Delete(x => x.DocumentId == id);

                    foreach (var row in db.Table<Row>().Where(x => x.DocumentId == id).ToList())
                    {
                        db.Table<Column>().Delete(x => x.RowId == row.Id);
                    }

                    db.Table<Row>().Delete(x => x.DocumentId == id);
                    db.Table<Document>().Delete(x => x.Id == id);

                    db.Commit();
                }
                catch(SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(DeleteDocument) } });
                }
            }
            OpenAsyncPhone();
        }

        public static void ClearDataBase()
        {
            CloseAsyncPhone();
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    if (db.Table<Document>().Count() > 0)
                    {
                        if (db.Table<Document>().Count() > 0)
                            db.DeleteAll<Document>();

                        if (db.Table<Row>().Count() > 0)
                            db.DeleteAll<Row>();

                        if (db.Table<Column>().Count() > 0)
                            db.DeleteAll<Column>();

                        if (db.Table<Field>().Count() > 0)
                            db.DeleteAll<Field>();
                        db.Commit();
                    }
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(ClearDataBase) } });
                }
            }
            OpenAsyncPhone();
        }

        public static bool? ClearDataBaseByUser()
        {
            CloseAsyncPhone();
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    if (db.Table<Document>().Count(x => x.UserId == UserAccount.UserId) > 0)
                    {
                        var documents = db.Table<Document>().Where(x => x.UserId == UserAccount.UserId).ToList();

                        for (var i = 0; i < documents.Count; i++)
                        {
                            var documentId = documents[i].Id;
                            var fields = db.Table<Field>().Where(x => x.DocumentId == documentId).Select(x => x.Id).ToList();
                            var rows = db.Table<Row>().Where(x => x.DocumentId == documentId).Select(x => x.Id).ToList();

                            _ = db.Execute($"DELETE FROM {nameof(Field)} WHERE id IN({string.Join(",", fields)})");
                            _ = db.Execute($"DELETE FROM {nameof(Row)} WHERE id IN({string.Join(",", rows)})");
                            _ = db.Execute($"DELETE FROM {nameof(Column)} WHERE RowId IN({string.Join(",", rows)})");
                            _ = db.Execute($"DELETE FROM {nameof(Document)} WHERE id = {documentId}");
                        }

                        db.Commit();
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(ClearDataBaseByUser) } });
                    
                    return null;
                }
            }
            OpenAsyncPhone();

            return true;
        }

        public static bool SaveColumn(int id, string value)
        {
            CloseAsyncPhone();
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    var item = db.Table<Column>().FirstOrDefault(x => x.Id == id);

                    if (item != null)
                    {
                        item.Value = value;

                        db.Update(item);
                        db.Commit();

                        OpenAsyncPhone();

                        return true;
                    }
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(SaveColumn) } });
                }
            }
            OpenAsyncPhone();

            return false;
        }

        public static bool UpdateRow(AbstractColumns rowValue)
        {
            CloseAsyncPhone();

            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    var item = db.Table<Row>().FirstOrDefault(x => x.Id == rowValue.Id);

                    if (item != null)
                    {
                        item.IsBlockRow = rowValue.IsBlock;
                        item.IsOpenRow = rowValue.IsOpen;

                        db.Update(item);

                        foreach (var column in rowValue.ColumnsElement)
                        {
                            var columnValue = db.Table<Column>().FirstOrDefault(x => x.RowId == item.Id && x.Id == column.Id);

                            if (columnValue != null && columnValue.Value != column.Value)
                            {
                                columnValue.IsModify = column.IsModify;
                                columnValue.Value = column.Value;
                                db.Update(columnValue);
                                db.Commit();

                            }
                        }
                        db.Commit();
                        db.Close();
                        OpenAsyncPhone();

                        return true;
                    }

                    db.Close();
                }
                catch(SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(UpdateRow) } });
                }

                OpenAsyncPhone();

                return false;
            }
        }

        public static bool InsertRow(int documentId, AbstractColumns rowValue)
        {
            CloseAsyncPhone();

            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    if (rowValue != null)
                    {
                        var rowV = Converter.Convert(documentId, rowValue);
                        
                        db.Insert(rowV);

                        rowValue.Id = rowV.Id;

                        foreach (var column in rowValue.ColumnsElement)
                        {
                            var columnValue = Converter.Convert(rowV.Id, column);

                            db.Insert(columnValue);

                            column.Id = columnValue.Id;
                        }

                        db.Commit();
                        db.Close();
                        OpenAsyncPhone();

                        return true;
                    }

                    db.Close();
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(InsertRow) } });
                }

                OpenAsyncPhone();

                return false;
            }
        }

        public static bool DeleteRow(AbstractColumns rowValue)
        {
            CloseAsyncPhone();

            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    var item = db.Table<Row>().FirstOrDefault(x => x.Id == rowValue.Id);

                    if (item != null)
                    {
                        db.Delete(item);
                        db.Commit();
                        db.Close();
                        OpenAsyncPhone();

                        return true;
                    }

                    db.Close();
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(DeleteRow) } });
                }

                OpenAsyncPhone();

                return false;
            }
        }

        public static AbstractColumns GetRowById (int id)
        {
            var result = new AbstractColumns();
            using (var db = new SQLiteConnection(_path, _flags))
            {
                try
                {
                    var row = db.Query<Row>($"SELECT * FROM {nameof(Row)} WHERE Id = {id}").FirstOrDefault();

                    if (row != null)
                    {
                        result.IsBlock = row.IsBlockRow;
                        result.IsOpen = row.IsOpenRow;
                        result.Id = row.Id;
                        result.ColumnsElement = Converter.ConvertToAbstractColumn(db.Query<Column>($"SELECT * FROM {nameof(Column)} WHERE RowId = {row.Id}")).ToObservableCollection();

                        return result;
                    }
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(GetRowById) } });
                }

                return null;
            }
        }

        public static int CountRowDocById(int id, string searchText = null)
        {
            using (var db = new SQLiteConnection(_path))
            {
                try
                {
                    if (searchText == null)
                    {
                        return db.Table<Row>().Count(x => x.DocumentId == id);
                    }
                    else
                    {
                        return db.Query<Row>($"SELECT * FROM Row WHERE DocumentId = {id} AND Id IN(SELECT RowId FROM Column WHERE Value LIKE '{searchText}%')").Count;
                    }
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(CountRowDocById) } });
                }

                return 0;
            }
        }

        public static bool IsDataBaseEmpty()
        {
            using (var db = new SQLiteConnection(_path))
            {
                try
                {
                    return (db.Table<Document>().Count() == 0 &&
                        db.Table<Row>().Count() == 0 &&
                        db.Table<Column>().Count() == 0 &&
                        db.Table<Field>().Count() == 0);
                }
                catch (SQLiteException ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(IsDataBaseEmpty) } });
                }

                return false;
            }
        }
        #endregion
    }
}