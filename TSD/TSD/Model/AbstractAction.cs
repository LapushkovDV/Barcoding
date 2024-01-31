using Newtonsoft.Json;
using System.Windows.Input;
using TSD.Model.User;
using TSD.Services;
using Xamarin.Forms;

namespace TSD.Model
{
    /// <summary>
    /// Модел действий (операций)
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class AbstractAction : SharedModel
    {
        #region Свойства
        /// <summary>
        /// Номер строки для сортировки
        /// </summary>
        [JsonProperty("ROW")]
        public int Row { get; set; }

        /// <summary>
        /// Название операции
        /// </summary>
        [JsonProperty("NAME")]
        public string Name { get; set; }

        /// <summary>
        /// Системное имя операции
        /// </summary>
        [JsonProperty("SYSNAME")]
        public string SysName { get; set; }

        /// <summary>
        /// Размер элемента операций
        /// </summary>
        [JsonProperty("SIZE")]
        public int Size { get; set; }

        /// <summary>
        /// Условие, если операция является синхронной
        /// </summary>
        [JsonProperty("ISSYNCHACTION")]
        public bool IsSynchAction { get; set; }

        /// <summary>
        /// Условие, если после выполнения операции, документ нужно закрыть
        /// </summary>
        [JsonProperty("CLOSEDOC")]
        public bool CloseDoc { get; set; }

        /// <summary>
        /// Системное имя операции в таблице
        /// </summary>
        [JsonProperty("SYSNAMETBL")]
        public string SysNameTbl { get; set; }
        #endregion

        #region Свойства для чтения
        /// <summary>
        /// Условие, если операцию нужно сделать неактивной
        /// </summary>
        public bool IsEnabled => ExecuterAction.IsEnabledAction(SysName == "CREATEORDERS"); //костыль
        #endregion

        #region Команды
        /// <summary>
        /// Универсальная команда операции
        /// </summary>
        public ICommand Command => new Command(CommandMethod);
        #endregion

        #region Приатные методы
        /// <summary>
        /// Приватный метод команды
        /// </summary>
        private async void CommandMethod()
        {
            if (UserAccount.IsBlock) return;

            UserAccount.IsBlock = true;

            if (SysName != string.Empty)
            {
                if (IsSynchAction)
                    MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagLoading);

                if (await ExecuterAction.Execute(SysName, IsSynchAction, CloseDoc))
                {
                    if (IsSynchAction)
                        MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagLoading);

                    MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagCancelView);
                    MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagDefaultPage);
                }
                else
                {
                    if (IsSynchAction)
                        MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagLoading);
                }

                MessagingCenter.Send(new MessageClass<object>(), Resources.MsgCenterTagUpdateSelect);
            }

            UserAccount.IsBlock = false;
        }
        #endregion
    }
}
