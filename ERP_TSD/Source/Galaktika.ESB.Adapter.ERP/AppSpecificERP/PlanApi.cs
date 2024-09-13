using Microsoft.Extensions.DependencyInjection;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Model;
using Microsoft.Restier.Publishers.OData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Galaktika.ERP.Interop;
using Galaktika.ESB.Adapter.Atlantis.Api;
using Galaktika.ESB.Adapter.ERP.Adapter;
using Galaktika.ESB.Core;
using Galaktika.ESB.Core.Context;
using Galaktika.ESB.ServiceAdapterApi;
using Galaktika.ESB.Utils.LogService;

namespace Galaktika.ESB.Adapter.ERP
{
    /// <summary>
    /// Plan api
    /// </summary>
	public partial class PlanApi : ApiBase, IEsbApi, IEsbContext, ILogger
    {
        /// <summary>
        /// Сервис логирования
        /// </summary>
        public ILogService LogService { get; private set; }

        static PlanApi()
		{
            //TODO LogService ErpTrace in static constructor
            "static PlanApi()".ErpTrace();
		}

        /// <summary>
        /// Провайдер сервисов
        /// </summary>
		public IServiceProvider srvProv;
		private AtlantisApi.AtlantisContext context;
		private static PlanWorker pw;

        /// <summary>
        /// Конструктор Plan api
        /// </summary>
        /// <param name="srvProv"></param>
		public PlanApi(IServiceProvider srvProv)
				: base(srvProv)
		{
			this.srvProv = srvProv;
            var serviceBase = srvProv.GetService<ServiceBase>();
            LogService = serviceBase.LogService ?? DefaultLogService.Instance;
            _Trace_PlanApi_IServiceProvider_srvProv();
            context = new AtlantisApi.AtlantisContext();
		}
        /// <summary>
        /// Конфигурирование Api
        /// </summary>
        /// <param name="apiType">Тип апи</param>
        /// <param name="services">Список сервисов</param>
        /// <returns></returns>
		public new static IServiceCollection ConfigureApi(Type apiType, IServiceCollection services)
		{
			services.AddService<IModelBuilder>((sp, next) => new PlanModelBuilder(sp));
			return ApiBase.ConfigureApi(apiType, services);
		}

        /// <summary>
        /// Создание плана
        /// </summary>
        /// <param name="plan">PlainPLan</param>
        /// <returns>Строку с планом</returns>
		[Operation(Namespace = "Galaktika.ERP.OData.Models.Examples", HasSideEffects = true)]
		public string CreatePlan(PlainPlan plan)
		{
			if (plan == null)
				return "Неверный формат плана";
			if (pw == null)
				pw = new PlanWorker(plan.Шаблон_Nrec);
			if (!pw.InitTemplate(plan.Шаблон_Nrec))
				return "Ошибка инициализации шаблона";
			var cPlan = pw.CreateDefaultPlan();
			if (cPlan == 0)
				return "План не создан";
			if (!pw.UpdatePlan(cPlan, plan))
				return "Ошибка обновления плана";
			pw.SetCalcFieldPlan(cPlan, 1, plan.Итого);
			var spList = plan.GetSpPlanToInsert();
			if (spList != null)
				foreach (SpPlan sp in spList)
				{
					sp.Заголовок_Nrec = cPlan;
					CreateSpPlan(sp);
				}
			return cPlan.ToString();
		}

        /// <summary>
        /// Создание PeriodPlan
        /// </summary>
        /// <param name="plan">План</param>
        /// <returns>Строку с PeriodPlan</returns>
		[Operation(Namespace = "Galaktika.ERP.OData.Models.Examples", HasSideEffects = true)]
		public string CreatePeriodPlan(PeriodPlan plan)
		{
			if (plan == null)
				return "Неверный формат плана";
			if (pw == null)
				pw = new PlanWorker(plan.Шаблон_Nrec);
			if (!pw.InitTemplate(plan.Шаблон_Nrec))
				return "Ошибка инициализации шаблона";
			var cPlan = pw.CreateDefaultPlan();
			if (cPlan == 0)
				return "План не создан";
			if (!pw.UpdatePeriodPlan(cPlan, plan))
				return "Ошибка обновления плана";
			pw.SetCalcFieldPlan(cPlan, 1, plan.Итого);
			var spList = plan.GetSpPlanToInsert();
			if (spList != null)
				foreach (SpPlan sp in spList)
				{
					sp.Заголовок_Nrec = cPlan;
					CreateSpPlan(sp);
				}
			return cPlan.ToString();
		}

        /// <summary>
        /// Создание SpPlan
        /// </summary>
        /// <param name="spPlan">SpPlan</param>
        /// <returns>Строку с SpPlan</returns>
		[Operation(Namespace = "Galaktika.ERP.OData.Models.Examples", HasSideEffects = true)]
		public string CreateSpPlan(SpPlan spPlan)
		{
			if (spPlan == null)
				return "Неверный формат";
			if (pw == null)
				pw = new PlanWorker(spPlan.Шаблон_Nrec);
			if (!pw.InitTemplate(spPlan.Шаблон_Nrec))
				return "Ошибка инициализации шаблона";
			var cSpPlan = pw.CreateDefaultSpPlan(spPlan);
			if (cSpPlan == 0)
				return "Строка не создана";
			if (!pw.UpdateSpPlan(cSpPlan, spPlan))
				return "Ошибка обновления строки плана";
			var spList = spPlan.GetSpCellToInsert();
			if (spList != null)
				foreach (SpCell sp in spList)
				{
					sp.CтрокаПлана_Nrec = cSpPlan;
					sp.Заголовок_Nrec = spPlan.Заголовок_Nrec;
					CreateSpCell(sp);
				}
			return cSpPlan.ToString();

		}

        /// <summary>
        /// Создание spCell
        /// </summary>
        /// <param name="spCell">spCell</param>
        /// <returns>Строку с spCell</returns>
		[Operation(Namespace = "Galaktika.ERP.OData.Models.Examples", HasSideEffects = true)]
		public string CreateSpCell(SpCell spCell)
		{
			if (spCell == null)
				return "Неверный формат";
			if (pw == null)
				pw = new PlanWorker(spCell.Шаблон_Nrec);
			if (!pw.InitTemplate(spCell.Шаблон_Nrec))
				return "Ошибка инициализации шаблона";
			long viewAnalitic1 = pw.GetViewAnalitic(1, spCell);

			var cSpMnPl = pw.CreateDefaultSpMnPl(spCell.CтрокаПлана_Nrec, spCell.Представление, viewAnalitic1);
			if (cSpMnPl == 0)
				return "Ошибка создания строки";
			var cValSpMnP = pw.CreateValSpMnP(cSpMnPl, spCell.План, spCell.ЦенаПлан, spCell.Факт, spCell.ЦенаФакт);
			if (cValSpMnP == 0)
				return "Ошиибка создания чиловых значений";
			return cSpMnPl.ToString();
		}

        /// <summary>
        /// InitTemplate
        /// </summary>
        /// <param name="cTemplate">cTemplate</param>
        /// <returns></returns>
		[Operation(Namespace = "Galaktika.ERP.OData.Models.Examples", HasSideEffects = false)]
		public string InitTemplate(long cTemplate)
		{
			var a = PlainPlanList.ToList<PlainPlan>();
			if (pw == null)
				pw = new PlanWorker(cTemplate);
			if (!pw.InitTemplate(cTemplate))
				return "Ошибка инициализации шаблона";
			return "ok";
		}

        /// <summary>
        /// PlainPlan
        /// </summary>
		public long PlainPlanProperty => (long)TestsTemplates.PlainPlan;

        //public class Test
        //{
        //	public int Id { get; set; }
        //	public double Massa { get; set; }
        //}

        /// <summary>
        /// Список PlainPlan
        /// </summary>
        [Resource]
		public IQueryable<PlainPlan> PlainPlanList
		{
			//get
			//{
			//    return from p in context.MNPLAN
			//           where p.CWAYACCOU == (long)TestsTemplates.PlainPlan
			//           join documentGroup in context.vPlan on p.CVPLAN equals documentGroup.NREC into g1 from documentGroup in g1.DefaultIfEmpty()   
			//           join dogovor in context.Dogovor on p.CANVAL3 equals dogovor.NREC into g2 from dogovor in g2.DefaultIfEmpty()
			//           join izgotovitel in context.KatOrg on p.CANVAL1 equals izgotovitel.NREC into g3 from izgotovitel in g3.DefaultIfEmpty()
			//           join prioritet in context.FarInfo on p.PRIOR equals prioritet.NREC into g4 from prioritet in g4.DefaultIfEmpty()
			//           join skald in context.KatPodr on p.CANVAL2 equals skald.NREC into g5 from skald in g5.DefaultIfEmpty()
			//           join status in context.KatNotes on p.CSTATUS equals status.NREC into g6 from status in g6.DefaultIfEmpty()
			//           join template in context.WayAccou on p.CWAYACCOU equals template.NREC into g7 from template in g7.DefaultIfEmpty()
			//           select new PlainPlan()
			//           {
			//               Nrec = p.NREC,
			//               ГруппаДескрипторов = p.DESGR,
			//               ГруппаДокументов_Nrec = p.CVPLAN,
			//          //     ГруппаДокументов = documentGroup,
			//               ДатаНачала = p.STARTDATE,
			//               ДатаОкончания = p.ENDDATE,
			//               ДатаУтверждения = p.UTVDATE,
			//               ДатаФормирования = p.STATEMDATE,
			//               Дескриптор = p.DESCR,
			//          //     Договор = dogovor,
			//               Договор_Nrec = p.CANVAL3,
			//          //     Изготовитель = izgotovitel,
			//               Изготовитель_Nrec = p.CANVAL1,
			//               Итого = 0,
			//               Наименование = p.NAME,
			//               Номер = p.NUMBER,
			//          //     Приоритет = prioritet,
			//               Приоритет_Nrec = p.PRIOR,
			//          //     Склад = skald,
			//               Склад_Nrec = p.CANVAL2,
			//             //  Статус = status,
			//               Статус_Nrec = p.CSTATUS,
			//              // Шаблон = template,
			//               Шаблон_Nrec = p.CWAYACCOU

			//           };
			//}
			get
			{
				return from p in context.MNPLAN
					   where p.CWAYACCOU == PlainPlanProperty
					   select new PlainPlan()
					   {
						   Nrec = p.NREC,
						   ГруппаДескрипторов = p.DESGR,
						   ГруппаДокументов_Nrec = p.CVPLAN,
						   ДатаНачала = p.STARTDATE,
						   ДатаОкончания = p.ENDDATE,
						   ДатаУтверждения = p.UTVDATE,
						   ДатаФормирования = p.STATEMDATE,
						   Дескриптор = p.DESCR,
						   Договор_Nrec = p.CANVAL3,
						   Изготовитель_Nrec = p.CANVAL1,
						   Итого = 0,
						   Наименование = p.NAME,
						   Номер = p.NUMBER,
						   Приоритет_Nrec = p.PRIOR,
						   Склад_Nrec = p.CANVAL2,
						   Статус_Nrec = p.CSTATUS,
						   Шаблон_Nrec = p.CWAYACCOU

					   };
			}
		}

        /// <summary>
        /// Список PeriodPlan
        /// </summary>
		[Resource]
		public IQueryable<PeriodPlan> PeriodPlanList
		{
			get
			{
				return from p in context.MNPLAN
					   where p.CWAYACCOU == PlainPlanProperty
					   select new PeriodPlan()
					   {
						   Nrec = p.NREC,
						   ГруппаДескрипторов = p.DESGR,
						   ГруппаДокументов_Nrec = p.CVPLAN,
						   ДатаНачала = p.STARTDATE,
						   ДатаОкончания = p.ENDDATE,
						   ДатаУтверждения = p.UTVDATE,
						   ДатаФормирования = p.STATEMDATE,
						   Дескриптор = p.DESCR,
						   Итого = 0,
						   Наименование = p.NAME,
						   Номер = p.NUMBER,
						   Приоритет_Nrec = p.PRIOR,
						   Грузоотправитель_Nrec = p.CANVAL1,
						   ОбъектСтроительства_Nrec = p.CANVAL2,
						   Статус_Nrec = p.CSTATUS,
						   Шаблон_Nrec = p.CWAYACCOU
					   };
			}
		}


        /// <summary>
        /// Adds an entity object to api
        /// </summary>
        /// <param name="entityType">Type of the entity</param>
        /// <returns>Entity</returns>
        public object AddEntity(Type entityType)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Save changes inside api context
        /// </summary>
		public void SaveChanges()
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Marks entity
        /// </summary>
        /// <param name="appEntity"></param>
	    public void SaveEntity(object appEntity)
	    {
	        throw new NotImplementedException();
	    }

	    /// <summary>
        /// Removes the entity
        /// </summary>
        /// <param name="appEntity">Entity to remove</param>
		public void RemoveEntity(object appEntity)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Finds the entity
        /// </summary>
        /// <param name="type">Type of the entity</param>
        /// <param name="key">Key value</param>
        /// <returns>Entity as an object</returns>
		public object FindEntity(Type type, string key)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Gets key of the specified entity
        /// </summary>
        /// <param name="typeAppEntity">Type of the entity</param>
        /// <param name="entity">Entity where to get key</param>
        /// <returns>Key value as string</returns>
		public string GetKey(Type typeAppEntity, object entity)
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Get entities by specified condition
        /// </summary>
        /// <typeparam name="TAppEntity">Type of entities</typeparam>
        /// <param name="condition">Condition for get (select) as SQL condition</param>
        /// <returns>Variety of entities</returns>
		public IQueryable<TAppEntity> GetAppEntities<TAppEntity>(string condition = "") where TAppEntity : class
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// Gets entities by specified condition
        /// </summary>
        /// <typeparam name="TAppEntity">Type of entities</typeparam>
        /// <param name="ids">List of oids to select</param>
        /// <returns>Variety of entities</returns>
        public IQueryable<TAppEntity> GetAppEntities<TAppEntity>(IEnumerable<string> ids) where TAppEntity : class
		{
			throw new NotImplementedException();
		}

        public IQueryable<TAppEntity> GetAppEntities<TAppEntity>(Expression<Func<TAppEntity, bool>> predicate) where TAppEntity : class
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets ids of entities
        /// </summary>
        /// <typeparam name="TAppEntity">Type of the entity</typeparam>
        /// <param name="condition">Condition to select entities</param>
        /// <returns>List of ids</returns>
		public List<string> GetAppEntitiesIds<TAppEntity>(string condition = "") where TAppEntity : class
		{
			throw new NotImplementedException();
		}

	    /// <inheritdoc />
	    public TAppEntity Reload<TAppEntity>(TAppEntity entity) where TAppEntity : class
	    {
	        throw new NotImplementedException();
	    }

        /// <inheritdoc />
        public TAppEntity GetAppEntity<TAppEntity>(Expression<Func<TAppEntity, bool>> predicate) where TAppEntity : class
	    {
	        throw new NotImplementedException();
	    }

	    /// <inheritdoc />
	    public int GetHashId()
	    {
	        throw new NotImplementedException();
	    }
	}
}