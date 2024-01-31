using Microsoft.AppCenter.Crashes;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using TSD.Model.User;
using TSD.Model;
using TSD.Services.DataBase.Tables;

namespace TSD.Services.DataBase.DataBaseServices
{
    public class DictionaryService : BaseDatabaseService
    {

        public void SaveDictionary()
        {
            using (var db = new SQLiteConnection(_pathDatabase, _flags))
            {
                //try
                //{
                //        db.BeginTransaction();

                //        var menubase = AddMenu(db, UserAccount.SelectedMenu);
                //        var document = db.Table<Dictionary>().FirstOrDefault(x => x.IdDoc == UserAccount.SelectedMenu.CurrentDocument.IdDoc);

                //        if (!(document is null))
                //        {
                //            document.Barcode = UserAccount.SelectedMenu.CurrentDocument.Barcode;
                //            document.IsAllowEditDoc = UserAccount.SelectedMenu.CurrentDocument.IsAllowEditDoc;
                //            document.StatusId = db.Get<Status>(x => x.Id == (int)status).Id;
                //            document.GuidStatus = status == StatusEnum.Complete || status == StatusEnum.InWork ? string.Empty
                //            : UserAccount.SelectedMenu.CurrentDocument.GuidStatus;
                //            document.LastAction = UserAccount.SelectedMenu.CurrentDocument.LastAction;

                //            db.Update(document);
                //        }
                //        else
                //        {
                //            document = new Document
                //            {
                //                IdDoc = UserAccount.SelectedMenu.CurrentDocument.IdDoc,
                //                Barcode = UserAccount.SelectedMenu.CurrentDocument.Barcode,
                //                IsAllowEditDoc = UserAccount.SelectedMenu.CurrentDocument.IsAllowEditDoc,
                //                StatusId = db.Get<Status>(x => x.Id == (int)status).Id,
                //                UserId = user.Id,
                //                GuidStatus = status == StatusEnum.Complete || status == StatusEnum.InWork ? string.Empty
                //                    : UserAccount.SelectedMenu.CurrentDocument.GuidStatus,
                //                MenuId = menubase.Id,
                //                LastAction = UserAccount.SelectedMenu.CurrentDocument.LastAction
                //            };

                //            db.Insert(document);
                //        }

                //        AddElementsByMenus(db, document, UserAccount.SelectedMenu);
                //        db.Commit();

                //        return document.Id;
                    
                //}
                //catch (SQLiteException ex)
                //{
                //    Crashes.TrackError(ex, new Dictionary<string, string>() { { Resources.TypeMethodDB, nameof(SaveCurrentDocument) } });
                //}

                //return -1;
            }
        }
    }
}
