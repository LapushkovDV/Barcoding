using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSD.Model;
using TSD.Services.DataBase.Tables;
using TSD.Services.Tasks.Models;
using Xamarin.Essentials;

namespace TSD.Services.Converters
{
    /// <summary>
    /// Конверторы
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Конвертирование полей с БД в поля для таблицы с задачами
        /// </summary>
        /// <param name="fields"></param>
        /// <returns>Поля для таблиц с задачами (Task)</returns>
        public static List<FieldsTask> ConvertList(List<Field> fields) => fields.Select(x => new FieldsTask
        {
            Name = x.Name,
            SysName = x.SysName,
            Value = x.Value,
            SizeColumn = ((DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density) - 45) / (fields.Count)
        }).ToList();

        /// <summary>
        /// Конвертирование полей с БД в поля документа на страницу
        /// </summary>
        /// <param name="fields"></param>
        /// <returns>Поля документа на страницу</returns>
        public static List<AbstractField> ConvertToAbstractFields(List<Field> fields) => fields.Select(x => new AbstractField
        {
            Name = x.Name,
            SysName = x.SysName,
            Value = x.Value,
            IsIdentifier = x.IsIdentifier,
            Row = x.Row,
            Size = x.Size,
            BrowseCard = x.BrowseCard,
            Nullable = x.Nullable,
        }).ToList();

        /// <summary>
        /// Конвертирование столбцов с БД в столбцы документа на страницу
        /// </summary>
        /// <param name="columns"></param>
        /// <returns>Столбцы документа на страницу</returns>
        public static List<AbstractColumn> ConvertToAbstractColumn(List<Column> columns) => columns.Select(x => new AbstractColumn
        {
            Name = x.Name,
            SysName = x.SysName,
            Value = x.Value,
            Action = x.Action,
            Browse = x.Browse,
            DataType = x.DataType,
            IsModify = x.IsModify,
            Npp = x.Npp,
            Size = x.Size,
            SizeColumn = x.SizeColumn,
            ValueOld = x.ValueOld,
            Modif = x.Modif,
            WIsActive = x.WIsActive,
            IsNew = x.IsNew,
            Id = x.Id,
            BrowseCard = x.BrowseCard,
            Nullable = x.Nullable,
        }).ToList();

        /// <summary>
        /// Асинхронный метод для конвертирования полей с БД в поля для таблицы с задачами
        /// </summary>
        /// <param name="fields"></param>
        /// <returns>Поля для таблицы с задачами</returns>
        public async static Task<List<FieldsTask>> ConvertListAsync(List<Field> fields) => await Task.Run(() => fields.Select(x => new FieldsTask
        {
            Name = x.Name,
            SysName = x.SysName,
            Value = x.Value
        }).ToList());

        /// <summary>
        /// Метод для конвертирования строки в строку БД
        /// </summary>
        /// <param name="docId">Идентификатор документа</param>
        /// <param name="row">Строка с массива</param>
        /// <returns>Строка в БД</returns>
        public static Row Convert(int docId, AbstractColumns row) => new Row 
        { 
            DocumentId = docId, 
            IsBlockRow = row.IsBlock, 
            IsOpenRow = row.IsOpen          
        };

        /// <summary>
        /// Метод для конвертирования колонок в колонку БД
        /// </summary>
        /// <param name="rowId">Идентификатор строки</param>
        /// <param name="column">Колонка с массива</param>
        /// <returns>Колонка в БД</returns>
        public static Column Convert(int rowId, AbstractColumn column) => new Column
        {
            RowId = rowId,
            Action = column.Action,
            Browse = column.Browse,
            DataType = column.DataType,
            Modif = column.Modif,
            ValueOld = column.ValueOld,
            Value = column.Value,
            Name = column.Name,
            SysName = column.SysName,
            WIsActive = column.WIsActive,
            IsModify = column.IsModify,
            Npp = column.Npp,
            IsNew = column.IsNew,
            Size = column.Size,
            SizeColumn = column.SizeColumn,
            BrowseCard = column.BrowseCard,
            Nullable = column.Nullable,
        };

        /// <summary>
        /// Метод для конвертирования полей в поле БД
        /// </summary>
        /// <param name="docId">Идентификатор документа</param>
        /// <param name="field">Поле с массива</param>
        /// <returns>Поле в БД</returns>
        public static Field Convert(int docId, AbstractField field) => new Field
        {
            DocumentId = docId,
            Name = field.Name,
            Row = field.Row,
            Size = field.Size,
            SysName = field.SysName,
            Value = field.Value,
            IsIdentifier = field.IsIdentifier,
            BrowseCard = field.BrowseCard,
            Nullable = field.Nullable,
        };

        /// <summary>
        /// Метод для конвертирования полей в поле БД (справочники)
        /// </summary>
        /// <param name="docId">Идентификатор документа</param>
        /// <param name="field">Поле с массива</param>
        /// <returns>Поле в БД</returns>
        public static DictionaryField ConvertDictionary(int docId, AbstractField field) => new DictionaryField
        {
            DictionaryId = docId,
            Name = field.Name,
            Row = field.Row,
            Size = field.Size,
            SysName = field.SysName,
            Value = field.Value,
            IsIdentifier = field.IsIdentifier,
            BrowseCard = field.BrowseCard,
            Nullable = field.Nullable,
        };

        /// <summary>
        /// Метод для конвертирования колонок БД в колонку массива
        /// </summary>
        /// <param name="rowId">Идентификатор строки</param>
        /// <param name="column">Колонка БД</param>
        /// <returns>Колонка в массив</returns>
        public static AbstractColumn Convert(Column column) => new AbstractColumn
        {
            Id = column.Id,
            Action = column.Action,
            Browse = column.Browse,
            DataType = column.DataType,
            Modif = column.Modif,
            ValueOld = column.ValueOld,
            Value = column.Value,
            Name = column.Name,
            SysName = column.SysName,
            WIsActive = column.WIsActive,
            IsModify = column.IsModify,
            Npp = column.Npp,
            IsNew = column.IsNew,
            Size = column.Size,
            SizeColumn = column.SizeColumn,
            BrowseCard = column.BrowseCard,
            Nullable = column.Nullable,
        };

        /// <summary>
        /// Метод конвертирования полей для модели задач из полей БД
        /// </summary>
        /// <param name="field">Поле из БД</param>
        /// <returns>Поле для модели задач</returns>
        public static FieldsTask Convert(Field field) => new FieldsTask
        {
            Name = field.Name,
            SysName = field.SysName,
            Value = field.Value
        };
    }
}
