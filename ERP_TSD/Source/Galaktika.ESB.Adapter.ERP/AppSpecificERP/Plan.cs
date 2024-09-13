using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Galaktika.ESB.Adapter.Atlantis.Api;

namespace Galaktika.ESB.Adapter.ERP.Adapter
{

    /// <summary>
    /// Перечисление алгоритм представления
    /// </summary>
    public enum СистемныйАлгоритмПредставления
    {
        /// <summary>
        /// Период
        /// </summary>
        Период = 1,
        /// <summary>
        /// План снабжения
        /// </summary>
        ПланСнабжения = 25
    }
    /// <summary>
    /// Перечисление тестовые шаблоны
    /// </summary>
    public enum TestsTemplates
    {
        /// <summary>
        /// План
        /// </summary>
        PlainPlan = 1,
        /// <summary>
        /// Период
        /// </summary>
        PeriodPlan = 9
    }
    /// <summary>
    /// Базовая сущность
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Атлантический контекст
        /// </summary>
        public static AtlantisApi.AtlantisContext context;
        /// <summary>
        /// Конструктор базового контекста
        /// </summary>
        public BaseEntity()
        {
            if (context == null)
                context = new AtlantisApi.AtlantisContext();
        }
    }
    /// <summary>
    /// Базовый план
    /// </summary>
    public abstract class BasePlan : BaseEntity
    {       
        /// <summary>
        /// Nrec
        /// </summary>
        [Key]
        public long Nrec { set; get; }
        /// <summary>
        /// Шаблон Nrec
        /// </summary>
        public long Шаблон_Nrec { set; get; }
        /// <summary>
        /// Шаблон
        /// </summary>
        public WAYACCOU Шаблон
        {
            get
            {
                return (from p in context.WAYACCOU where p.NREC == Шаблон_Nrec select p).First();
            }

        }
        /// <summary>
        /// Дескриптор
        /// </summary>
        public string Дескриптор { set; get; }
        /// <summary>
        /// Группа дескрипторов
        /// </summary>
        public string ГруппаДескрипторов { set; get; }
        /// <summary>
        /// Номер
        /// </summary>
        public string Номер { set; get; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Наименование { set; get; }
        /// <summary>
        /// Состояние
        /// </summary>
        public string Состояние { set; get; }
        /// <summary>
        /// Дата формирования
        /// </summary>
        public DateTime ДатаФормирования { set; get; }
        /// <summary>
        /// Статус Nrec
        /// </summary>
        public long Статус_Nrec { set; get; }
        /// <summary>
        /// Статус
        /// </summary>
        public KATNOTES Статус
        {
            get
            {
                return (from p in context.KATNOTES where p.NREC == Статус_Nrec select p).First();
            }
        }
        /// <summary>
        /// Дата утверждения
        /// </summary>
        public DateTime ДатаУтверждения { set; get; }
        /// <summary>
        /// Группа документов_Nrec
        /// </summary>
        public long ГруппаДокументов_Nrec { set; get; }
        /// <summary>
        /// Группа документов
        /// </summary>
        public VPLAN ГруппаДокументов
        {
            get
            {
                return (from p in context.VPLAN where p.NREC == ГруппаДокументов_Nrec select p).First();
            }
        }
        /// <summary>
        /// Дата начала
        /// </summary>
        public DateTime ДатаНачала { set; get; }
        /// <summary>
        /// Дата окончания
        /// </summary>
        public DateTime ДатаОкончания { set; get; }
        /// <summary>
        /// Приоритет
        /// </summary>
        public FARINFO Приоритет
        {
            get
            {
                return (from p in context.FARINFO where p.NREC == Приоритет_Nrec select p).First();
            }
        }
        /// <summary>
        /// Приоритет Nrec
        /// </summary>
        public long Приоритет_Nrec { set; get; }       
        /// <summary>
        /// Итого
        /// </summary>
        public double Итого { set; get; }
        private List<SpPlan> spPlansToInsert;
        /// <summary>
        /// Спецификация
        /// </summary>
        public List<SpPlan> Спецефикация
        {
            set
            {
                spPlansToInsert = value.ToList();
            }
            get
            {
                return (from p in context.SPMNPLAN
                       where p.CMNPLAN == this.Nrec
                       select new SpPlan
                       {
                           Nrec = p.NREC,
                           ДатаНачала = p.STARTDATE,
                           ДатаНачалаФактическая = p.STARTDATEFACT,
                           ДатаОкончания = p.ENDDATE,
                           ДатаОкончанияФактическая = p.ENDDATEFACT,
                           ЕдиницаИзмерения_Nrec = p.COTPED,
                           Заголовок_Nrec = p.CMNPLAN,
                           ЗаявкаВходящая_Nrec = p.CANVAL1,
                           Номер = p.NUMBER,
                           ОбъектПланирования_Nrec = p.CIZD,
                           ПризнакПродукта = (ПризнакПродукта)p.RAZR,
                           ПризнакПродуктаВыхода = (ПризнакПродуктаВыхода)p.CODTAR,
                           Приоритет_Nrec = p.PRIOR,
                           Статус_Nrec = p.CSTATUS,
                           Тип = (ТипОбъектаПланирования)p.TYPEIZD,
                           Шаблон_Nrec = this.Шаблон_Nrec
                       }).ToList();
            }
        }
        /// <summary>
        /// Список spPlan для вставки
        /// </summary>
        /// <returns></returns>
        public List<SpPlan> GetSpPlanToInsert()
        {
            return spPlansToInsert;
        }
    }

    /// <summary>
    /// План
    /// </summary>
    public class PlainPlan : BasePlan
    {
        /// <summary>
        /// Изготовитель Nrec
        /// </summary>
        public long Изготовитель_Nrec { set; get; }       
        /// <summary>
        /// Изготовитель
        /// </summary>
        public KATORG Изготовитель
        {
            get
            {
                return (from p in context.KATORG where p.NREC == Изготовитель_Nrec select p).First();
            }
        }

        /// <summary>
        /// Склад Nrec
        /// </summary>
        public long Склад_Nrec { set; get; }   
        /// <summary>
        /// Склад
        /// </summary>
        public KATPODR Склад
        {
            get
            {
                return (from p in context.KATPODR where p.NREC == Склад_Nrec select p).First();
            }
        }
        /// <summary>
        /// Договор Nrec
        /// </summary>
        public long Договор_Nrec { set; get; }
        /// <summary>
        /// Догоовр
        /// </summary>
        public DOGOVOR Договор
        {
            get
            {
                return (from p in context.DOGOVOR where p.NREC == Договор_Nrec select p).First();
            }
        }
        

    }
    /// <summary>
    /// Период
    /// </summary>
    public class PeriodPlan : BasePlan
    {
        /// <summary>
        /// Грузоотправитель Nrec
        /// </summary>
        public long Грузоотправитель_Nrec { set; get; }
        /// <summary>
        /// Грузоотправитель
        /// </summary>
        public KATORG Грузоотправитель
        {
            get
            {
                return (from p in context.KATORG where p.NREC == Грузоотправитель_Nrec select p).First();
            }
        }
        /// <summary>
        /// ОбъектСтроительства_Nrec
        /// </summary>
        public long ОбъектСтроительства_Nrec { set; get; }
        /// <summary>
        /// ОбъектСтроительства
        /// </summary>
        public KATSTROY ОбъектСтроительства
        {
            get
            {
                return (from p in context.KATSTROY where p.NREC == ОбъектСтроительства_Nrec select p).First();
            }
        }
    }
    /// <summary>
    /// SpPlan
    /// </summary>
    public partial class SpPlan : BaseEntity
    {
        private List<SpCell> spCellsToInsert = null;
        /// <summary>
        /// ЯчейкаПлана
        /// </summary>
        public List<SpCell> ЯчейкаПлана
        {
            set
            {
                spCellsToInsert = value.ToList();
            }
            get
            {               
                return (from p in context.SPMNPL
                       where p.CSPMNPLAN == this.Nrec
                       join val in context.VALSPMNP on p.NREC equals val.CSPMNPL into vg
                       from val in vg.DefaultIfEmpty()
                       select new SpCell
                       {
                           CтрокаПлана_Nrec = p.CSPMNPLAN,
                           Nrec = p.NREC,
                           Заголовок_Nrec = p.CMNANAL,
                           Период_Nrec = (p.WKOLAN == (int)СистемныйАлгоритмПредставления.Период ? p.CANVAL4 : 0),
                           Представление = (СистемныйАлгоритмПредставления)p.WKOLAN,
                           Шаблон_Nrec = this.Шаблон_Nrec,
                           План = (val != null ? val.KOL : 0),
                           ЦенаПлан = (val != null ? val.PRICE : 0),
                           Факт = (val != null ? val.VPRICE : 0),
                           ЦенаФакт = (val != null ? val.SUMMA : 0)
                       }).ToList();
            }
        }
        /// <summary>
        /// GetSpCellToInsert
        /// </summary>
        /// <returns>List of spcell</returns>
        public List<SpCell> GetSpCellToInsert()
        {
            return spCellsToInsert;
        }
        /// <summary>
        /// Заголовок
        /// </summary>
        public MNPLAN Заголовок
        {
            get
            {
                return (from p in context.MNPLAN where p.NREC == Заголовок_Nrec select p).First();
            }
        }
        /// <summary>
        /// Заголовок_Nrec
        /// </summary>
        public long Заголовок_Nrec { set; get; }
        /// <summary>
        /// Номер
        /// </summary>
        public string Номер { set; get; }
        /// <summary>
        /// Тип
        /// </summary>
        public ТипОбъектаПланирования Тип { set; get; }
        /// <summary>
        /// Код
        /// </summary>
        public string Код
        {
            get
            {
                if (Тип == ТипОбъектаПланирования.Матценность)
                {
                    if (ОбъектПланирования != null)
                        return ОбъектПланирования.BARKOD;
                }
                else if (Тип == ТипОбъектаПланирования.Услуга)
                {
                    return ""; 
                }
                    return "";
            }
        }
        /// <summary>
        /// Обозначение
        /// </summary>
        public string Обозначение
        {
            get
            {
                if (Тип == ТипОбъектаПланирования.Матценность)
                {
                    if (ОбъектПланирования != null)
                        return ОбъектПланирования.OBOZN;
                }
                else if (Тип == ТипОбъектаПланирования.Услуга)
                {
                    return "";
                }
                return "";
            }
                
            
        }
        /// <summary>
        /// ОбъектПланирования_Nrec
        /// </summary>
        public long ОбъектПланирования_Nrec { set; get; }
        /// <summary>
        /// ОбъектПланирования
        /// </summary>
        public KATMC ОбъектПланирования
        {
            get
            {
                return (from p in context.KATMC where p.NREC == ОбъектПланирования_Nrec select p).First();
            }
        }
        /// <summary>
        /// ЕдиницаИзмерения_Nrec
        /// </summary>
        public long ЕдиницаИзмерения_Nrec { set; get; }
        /// <summary>
        /// ЕдиницаИзмерения
        /// </summary>
        public KATOTPED ЕдиницаИзмерения
        {
            get
            {
                return (from p in context.KATOTPED where p.NREC == ОбъектПланирования_Nrec select p).First();
            }
        }

        /// <summary>
        /// Статус_Nrec
        /// </summary>
        public long Статус_Nrec { set; get; }
        /// <summary>
        /// Статус
        /// </summary>
        public KATNOTES Статус
        {
            get
            {
                return (from p in context.KATNOTES where p.NREC == Статус_Nrec select p).First();
            }
        }

        /// <summary>
        /// ЗаявкаВходящая
        /// </summary>
        public INDENT ЗаявкаВходящая
        {
            get
            {
                return (from p in context.INDENT where p.NREC == ЗаявкаВходящая_Nrec select p).First();
            }
        }

        /// <summary>
        /// ЗаявкаВходящая_Nrec
        /// </summary>
        public long ЗаявкаВходящая_Nrec { set; get; }


        /// <summary>
        /// Приоритет
        /// </summary>
        public FARINFO Приоритет
        {
            get
            {
                return (from p in context.FARINFO where p.NREC == Приоритет_Nrec select p).First();
            }
        }

        /// <summary>
        /// Приоритет_Nrec
        /// </summary>
        public long Приоритет_Nrec { set; get; }

        /// <summary>
        /// ДатаНачала
        /// </summary>
        public DateTime ДатаНачала { set; get; }
        /// <summary>
        /// ДатаОкончания
        /// </summary>
        public DateTime ДатаОкончания { set; get; }
        /// <summary>
        /// ДатаНачалаФактическая
        /// </summary>
        public DateTime ДатаНачалаФактическая { set; get; }
        /// <summary>
        /// ДатаОкончанияФактическая
        /// </summary>
        public DateTime ДатаОкончанияФактическая { set; get; }
        /// <summary>
        /// ПризнакПродукта
        /// </summary>
        public ПризнакПродукта ПризнакПродукта { set; get; }
        /// <summary>
        /// ПризнакПродуктаВыхода
        /// </summary>
        public ПризнакПродуктаВыхода ПризнакПродуктаВыхода { set; get; }
        /// <summary>
        /// Шаблон_Nrec
        /// </summary>
        public long Шаблон_Nrec { set; get; }
        /// <summary>
        /// Шаблон
        /// </summary>
        public WAYACCOU Шаблон
        {
            get
            {
                return (from p in context.WAYACCOU where p.NREC == Шаблон_Nrec select p).First();
            }
        }
        /// <summary>
        /// Nrec
        /// </summary>
        [Key]
        public long Nrec { set; get; }
    }

    /// <summary>
    /// Class SpCell
    /// </summary>
    public class SpCell : BaseEntity
    {
        /// <summary>
        /// Заголовок
        /// </summary>
        public MNPLAN Заголовок
        {
            get
            {
                return (from p in context.MNPLAN where p.NREC == Заголовок_Nrec select p).First();
            }
        }
        /// <summary>
        /// Заголовок_Nrec
        /// </summary>
        public long Заголовок_Nrec { set; get; }
        //private long строкаПлана_Nrec;
        /// <summary>
        /// CтрокаПлана
        /// </summary>
        public SpPlan CтрокаПлана { set; get; }
        /// <summary>
        /// CтрокаПлана_Nrec
        /// </summary>
        public long CтрокаПлана_Nrec { set; get; }
        /// <summary>
        /// Представление
        /// </summary>
        public СистемныйАлгоритмПредставления Представление { set; get; }
        /// <summary>
        /// Период_Nrec
        /// </summary>
        public long Период_Nrec { set; get; }
        /// <summary>
        /// Период
        /// </summary>
        public FPPERIOD Период
        {
            get
            {
                return (from p in context.FPPERIOD where p.NREC == Период_Nrec select p).First();
            }
        }
        /// <summary>
        /// План
        /// </summary>
        public double План { set; get; }
        /// <summary>
        /// ЦенаПлан
        /// </summary>
        public double ЦенаПлан { set; get; }
        /// <summary>
        /// Факт
        /// </summary>
        public double Факт { set; get; }
        /// <summary>
        /// ЦенаФакт
        /// </summary>
        public double ЦенаФакт { set; get; }
        /// <summary>
        /// Шаблон_Nrec
        /// </summary>
        public long Шаблон_Nrec { set; get; }
        /// <summary>
        /// Шаблон
        /// </summary>
        public WAYACCOU Шаблон
        {
            get
            {
                return (from p in context.WAYACCOU where p.NREC == Шаблон_Nrec select p).First();
            }
        }
        /// <summary>
        /// Nrec
        /// </summary>
        [Key]
        public long Nrec { set; get; }
    }

    /// <summary>
    /// ТипОбъектаПланирования
    /// </summary>
    public enum ТипОбъектаПланирования
    {
        /// <summary>
        /// Матценность
        /// </summary>
        Матценность = 1,
        /// <summary>
        /// Услуга
        /// </summary>
        Услуга = 2
    }
    /// <summary>
    /// ПризнакПродукта
    /// </summary>
    public enum ПризнакПродукта
    {
        /// <summary>
        /// Продукт
        /// </summary>
        П = 0,
        /// <summary>
        /// Выход
        /// </summary>
        В = 1
    }
    /// <summary>
    /// ПризнакПродуктаВыхода
    /// </summary>
    public enum ПризнакПродуктаВыхода
    {
        /// <summary>
        /// Продукт выхода
        /// </summary>
        ПВ = 0,
        /// <summary>
        /// Используемый отход
        /// </summary>
        ИО = 1,
        /// <summary>
        /// Неиспользуемый отход
        /// </summary>
        НО = 2
    }
}