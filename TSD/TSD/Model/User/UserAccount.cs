using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Linq;
using TSD.Services;
using TSD.Services.DataBase.Tables;
using TSD.Services.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TSD.Model.User
{
    public static class UserAccount
    {
        public static int Id { get; set; } = -1;
        public static int UserId { get; set; } = -1;
        public static string Login { get; set; }

        public static bool IsBlock { get; set; } = false;

        public static string UserName { get; set; }

        public static bool IsUsbWork { get; set; }

        public static ObservableCollection<AbstractMenu> Menus { get; set; } = new ObservableCollection<AbstractMenu>();
        public static AbstractMenu SelectedMenu { get; set; }

        public static void FromJsonResponse(JObject json)
        {
            if (json != null)
            {

                Id = json.ContainsKey("userId") ? int.Parse(json["userId"].ToString()) : -1;

                if (json.ContainsKey("menu"))
                {
                    var menus = JArray.Parse(json["menu"].ToString());

                    foreach (JObject menu in menus)
                    {
                        Menus.Add(new AbstractMenu()
                        {
                            Key = menu.ContainsKey("id") ? menu["id"].ToString() : string.Empty,
                            Title = menu.ContainsKey("name") ? menu["name"].ToString() : string.Empty,
                            Action = menu.ContainsKey("action") ? menu["action"].ToString() : string.Empty
                        });
                    }
                }
            }
        }

        public static void FromJsonFileMenu(JObject json)
        {
            if (json != null)
            {
                if (json.ContainsKey("OBJECTDESCRIPTIONARRAY"))
                {
                    var menus = JArray.Parse(json["OBJECTDESCRIPTIONARRAY"].ToString());

                    foreach (JObject menu in menus.Cast<JObject>())
                    {
                        Menus.Add(new AbstractMenu()
                        {
                            Key = menu.ContainsKey("id") ? menu["id"].ToString() : string.Empty,
                            Title = menu.ContainsKey("MENUPOINTNAME") ? menu["MENUPOINTNAME"].ToString() : string.Empty,
                            Action = menu.ContainsKey("OBJECTACTION") ? menu["OBJECTACTION"].ToString() : string.Empty,
                            AllowAddRows = menu.ContainsKey("ALLOWADDROWS") && bool.Parse(menu["ALLOWADDROWS"].ToString()),
                            IsDoc = menu.ContainsKey("ISDOC") && bool.Parse(menu["ISDOC"].ToString()),
                            IsBatchLoad = menu.ContainsKey("BATCHLOAD") && bool.Parse(menu["BATCHLOAD"].ToString()),
                            IsBlockProcessedRows = menu.ContainsKey("BLOCKPROCESSEDROWS") && bool.Parse(menu["BLOCKPROCESSEDROWS"].ToString())
                        });

                        if (menu.ContainsKey("ACTIONS"))
                        {
                            Menus[Menus.Count - 1].Actions = JsonConvert.DeserializeObject<ObservableCollection<AbstractAction>>(menu["ACTIONS"].ToString());
                        }

                        if (menu.ContainsKey("FIELDS"))
                        {
                            Menus[Menus.Count - 1].Fields = JsonConvert.DeserializeObject<ObservableCollection<AbstractField>>(menu["FIELDS"].ToString());
                        }

                        if (menu.ContainsKey("COLUMNS"))
                        {
                            Menus[Menus.Count - 1].Columns = JsonConvert.DeserializeObject<ObservableCollection<AbstractColumn>>(menu["COLUMNS"].ToString());

                            var sum = Menus[Menus.Count - 1].Columns.Where(element => element.Browse == 1).Sum(element => element.Size);

                            Menus[Menus.Count - 1].Columns = Menus[Menus.Count - 1].Columns.Select(x =>
                            {
                                if (x.Browse == 1)
                                {
                                    x.SizeColumn = ((DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density) - 45)
                                    * (x.Size / (double)sum);
                                }

                                return x;
                            }).ToObservableCollection();
                        }
                    }
                }


                IsUsbWork = true;
            }
        }

        public static bool FromJsonUser(JObject json, string login)
        {
            if (json != null)
            {
                if (json.ContainsKey("TSD_USERS_ARRAY"))
                {
                    var users = JArray.Parse(json["TSD_USERS_ARRAY"].ToString());

                    foreach (JObject user in users.Cast<JObject>())
                    {
                        if (!user.ContainsKey("USERSYSNAME") || !(user["USERSYSNAME"].ToString().ToUpper() == login.ToUpper())) continue;

                        UserId = 0;
                        UserName = user["USERNAME"].ToString() ?? string.Empty;
                        Login = user["USERSYSNAME"].ToString() ?? string.Empty;

                        return true;
                    }
                }
            }

            return false;
        }

        public static void Clear()
        {
            ClearMenus();
            Login = string.Empty;
            Id = -1;
            UserId = -1;
        }

        public static void ClearMenus()
        {
            Menus.Clear();
            SelectedMenu = null;
        }
        public static void ClearDocMenus()
        {
            foreach (var menu in Menus)
            {
                menu.CurrentDocument = new CurrentDocument();
                menu.Fields = menu.Fields.Select(element => { element.Value = string.Empty; return element; }).ToObservableCollection();
                menu.ColumnsValue.Clear();
            }
            MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagDefaultPage);
        }

        public static void ClearDocMenusLocal()
        {
            foreach (var menu in Menus)
            {
                menu.CurrentDocument = new CurrentDocument();
                menu.Fields = menu.Fields.Select(element => { element.Value = string.Empty; return element; }).ToObservableCollection();
                menu.ColumnsValue.Clear();
            }
        }

        public static void ClearSelectMenuDoc()
        {
            SelectedMenu.CurrentDocument = new CurrentDocument();
            SelectedMenu.Fields = SelectedMenu.Fields.Select(element => { element.Value = string.Empty; return element; }).ToObservableCollection();
            SelectedMenu.ColumnsValue.Clear();
        }

    }
}
