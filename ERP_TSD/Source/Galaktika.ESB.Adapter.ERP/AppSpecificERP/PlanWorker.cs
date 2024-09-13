using System;
using Galaktika.ERP;
using Galaktika.ERP.Interop;
using Galaktika.ESB.Adapter.ERP.Adapter;

namespace Galaktika.ESB.Adapter.ERP
{
    /// <summary>
    /// PlanWorker
    /// </summary>
	public class PlanWorker
	{
        /// <summary>
        /// Constructor PlanWorker
        /// </summary>
		public PlanWorker()
		{
			MethodCaller = GalnetApi.InitCaller("M_MNPLAN::oCreatePlan", "M_MNPLAN::iCreatePlan");
		}

        /// <summary>
        /// Constructor PlanWorker
        /// </summary>
        /// <param name="cTemplate">NREC</param>
		public PlanWorker(long cTemplate)
		{
			MethodCaller = GalnetApi.InitCaller("M_MNPLAN::oCreatePlan", "M_MNPLAN::iCreatePlan");
			InitTemplate(cTemplate);
			CalcFld = GalnetApi.InitCaller("M_MNPLAN::oCalcMnPlan", "M_MNPLAN::iCalcMnPlan");
		}

		private GalnetApi.PIfcMethodsCaller MethodCaller { set; get; }
		private GalnetApi.PIfcMethodsCaller CalcFld { set; get; }
		private long currentTemplate = 0;

        /// <summary>
        /// CreateDefaultPlan
        /// </summary>
        /// <returns>NREC of default plan</returns>
		public long CreateDefaultPlan()
		{
			if (!(bool)MethodCaller.CallMethod("SetDefaultMnplan", GalnetApi.fdBoolean))
				return 0;
			return (long)MethodCaller.CallMethod("InsertMnPlan", GalnetApi.fdComp);
		}

		public void SetCalcFieldPlan(long cPlan, ushort num, double val)
		{
			CalcFld.CallMethod("WriteValFldDocument", null, cPlan, num, val);
		}

		private string GetDSQLDate(DateTime val)
		{
			return String.Format("#Date({0},{1},{2})", val.Day.ToString(), val.Month.ToString(), val.Year.ToString());
		}
        /// <summary>
        /// UpdatePlan
        /// </summary>
        /// <param name="cPlan">NREC cPlan</param>
        /// <param name="plan">PlainPlan object</param>
        /// <returns>true/false в зависимости от успешности выполнения</returns>
		public bool UpdatePlan(long cPlan, PlainPlan plan)
		{
			string dsql = @"UPDATE MNPLAN SET " +
					"CANVAL1   = #comp(" + plan.Изготовитель_Nrec.ToString() + ")," +
					"CANVAL2   = #comp(" + plan.Склад_Nrec.ToString() + ")," +
					"CANVAL3   = #comp(" + plan.Договор_Nrec.ToString() + ")," +
					"CVPLAN    = #comp(" + plan.ГруппаДокументов_Nrec.ToString() + ")," +
					"STARTDATE = " + GetDSQLDate(plan.ДатаНачала) + "," +
					"ENDDATE   = " + GetDSQLDate(plan.ДатаОкончания) + "," +
					"UTVDATE   = " + GetDSQLDate(plan.ДатаУтверждения) + "," +
					"STATEMDATE = " + GetDSQLDate(plan.ДатаФормирования) +
					(string.IsNullOrWhiteSpace(plan.Дескриптор) ? "" : ", DESCR = '" + plan.Дескриптор + "'") +
					(string.IsNullOrWhiteSpace(plan.ГруппаДескрипторов) ? "" : ", DESGR = '" + plan.ГруппаДескрипторов + "'") +
					(string.IsNullOrWhiteSpace(plan.Номер) ? "" : ", NUMBER = '" + plan.Номер + "'") +
					(string.IsNullOrWhiteSpace(plan.Наименование) ? "" : ", NAME = '" + plan.Наименование + "'") +
					"WHERE NREC = #comp(" + cPlan.ToString() + ")";

			using (var dsqHelper = new DsqlHelper())
			{
				return dsqHelper.ExecuteDsqlNoQuery(dsql) == 1;
			}
		}
        /// <summary>
        /// PlanWorker desctructor
        /// </summary>
		~PlanWorker()
		{
			if (MethodCaller != null)
				MethodCaller.MtCallDoneIfc();
			if (CalcFld != null)
				CalcFld.MtCallDoneIfc();
		}

        /// <summary>
        /// CreateDefaultSpPlan
        /// </summary>
        /// <param name="spPlan">SpPlan object</param>
        /// <returns>NREC of SpPlan</returns>
		public long CreateDefaultSpPlan(SpPlan spPlan)
		{
			if (!(bool)MethodCaller.CallMethod("SetDefaultSpMnplan", GalnetApi.fdBoolean, spPlan.Заголовок_Nrec))
				return 0;
			return (long)MethodCaller.CallMethod("InsertsPMnPlan", GalnetApi.fdComp);
		}

        /// <summary>
        /// Обновление SpPlan
        /// </summary>
        /// <param name="cSpPlan">Nrec spPlan</param>
        /// <param name="spPlan">spPLan object</param>
        /// <returns>true/false в зависимости от успешности выполнения</returns>
		public bool UpdateSpPlan(long cSpPlan, SpPlan spPlan)
		{
			string dsql = @"UPDATE SPMNPLAN SET " +
				 "CANVAL1   = #comp(" + spPlan.ЗаявкаВходящая_Nrec.ToString() + ")," +
				 "TYPEIZD   = " + ((int)spPlan.Тип).ToString() + "," +
				 "CIZD      = #comp(" + spPlan.ОбъектПланирования_Nrec.ToString() + ")," +
				 "COTPED    = #comp(" + spPlan.ЕдиницаИзмерения_Nrec.ToString() + ")," +
				 "CONDREC   = 1," +
				 "RAZR      = " + ((int)spPlan.ПризнакПродукта).ToString() + "," +
				 "CODTAR    = " + ((int)spPlan.ПризнакПродуктаВыхода).ToString() + "," +
				 "STARTDATE = " + GetDSQLDate(spPlan.ДатаНачала) + "," +
				 "ENDDATE   = " + GetDSQLDate(spPlan.ДатаОкончания) + "," +
				 "STARTDATEFACT = " + GetDSQLDate(spPlan.ДатаНачалаФактическая) + "," +
				 "ENDDATEFACT   = " + GetDSQLDate(spPlan.ДатаОкончанияФактическая) +


				 (spPlan.Статус_Nrec == 0 ? "" : ",CKATNOTES = #comp(" + spPlan.Статус_Nrec.ToString() + ") ") +
				 (spPlan.Приоритет_Nrec == 0 ? "" : ",PRIOR = #comp(" + spPlan.Приоритет_Nrec.ToString() + ") ") +
				 "WHERE NREC = #comp(" + cSpPlan.ToString() + ")";

			using (var h = new DsqlHelper())
			{
				return h.ExecuteDsqlNoQuery(dsql) == 1;
			}
		}

        /// <summary>
        /// CreateValSpMnP
        /// </summary>
        /// <param name="cSpMnPl">NREC cSpMnPl</param>
        /// <param name="values">Массив значений</param>
        /// <returns>NREC созданного SpMnP</returns>
		public long CreateValSpMnP(long cSpMnPl, params double[] values)
		{
			if (!(bool)MethodCaller.CallMethod("SetDefaultValSpMnP", GalnetApi.fdBoolean, cSpMnPl))
				return 0;
			if ((values != null) && (values.Length > 0))
			{
				for (ushort i = 1; i < values.Length + 1; i++)
				{
					MethodCaller.CallMethod("SetStValSpMnPValue", null, i, values[i - 1]);
				}
			}
			return (long)MethodCaller.CallMethod("InsertValSpMnP", GalnetApi.fdComp);
		}
		public long CreateDefaultSpMnPl(long cSpMnPlan, СистемныйАлгоритмПредставления wKolAn, long cAnVal4)
		{
			if (!(bool)MethodCaller.CallMethod("SetDefaultSpMnpl", GalnetApi.fdBoolean, cSpMnPlan, (ushort)wKolAn, cAnVal4))
				return 0;
			long cSpMnPl = (long)MethodCaller.CallMethod("InsertsPMnPl", GalnetApi.fdComp);
			if (cSpMnPl == 0)
				return 0;
			return cSpMnPl;
		}

		public bool InitTemplate(long cTemplate)
		{
			if (cTemplate == 0)
				return currentTemplate != 0;
			if ((currentTemplate == cTemplate) && (currentTemplate != 0))
				return true;
			if (!(bool)MethodCaller.CallMethod("Init", GalnetApi.fdBoolean, cTemplate))
				return false;
			currentTemplate = cTemplate;
			return true;
		}

		public long GetViewAnalitic(int num, SpCell spCell)
		{
			switch (spCell.Представление)
			{
				case СистемныйАлгоритмПредставления.Период:
					return spCell.Период_Nrec;
				case СистемныйАлгоритмПредставления.ПланСнабжения:
					return spCell.Заголовок_Nrec;
				default: return spCell.Заголовок_Nrec;
			}
		}

        /// <summary>
        /// UpdatePeriodPlan
        /// </summary>
        /// <param name="cPlan">NREC cPlan</param>
        /// <param name="plan">PeriodPlan object</param>
        /// <returns>true/false в зависимости от успешности выполнения</returns>
		public bool UpdatePeriodPlan(long cPlan, PeriodPlan plan)
		{
			string dsql = @"UPDATE MNPLAN SET " +
					"CANVAL1   = #comp(" + plan.Грузоотправитель_Nrec.ToString() + ")," +
					"CANVAL2   = #comp(" + plan.ОбъектСтроительства_Nrec.ToString() + ")," +
					"CVPLAN    = #comp(" + plan.ГруппаДокументов_Nrec.ToString() + ")," +
					"STARTDATE = " + GetDSQLDate(plan.ДатаНачала) + "," +
					"ENDDATE   = " + GetDSQLDate(plan.ДатаОкончания) + "," +
					"UTVDATE   = " + GetDSQLDate(plan.ДатаУтверждения) + ",dsdsdsdssdsdf" +
					"STATEMDATE = " + GetDSQLDate(plan.ДатаФормирования) +
					(string.IsNullOrWhiteSpace(plan.Дескриптор) ? "" : ", DESCR = '" + plan.Дескриптор + "'") +
					(string.IsNullOrWhiteSpace(plan.ГруппаДескрипторов) ? "" : ", DESGR = '" + plan.ГруппаДескрипторов + "'") +
					(string.IsNullOrWhiteSpace(plan.Номер) ? "" : ", NUMBER = '" + plan.Номер + "'") +
					(string.IsNullOrWhiteSpace(plan.Наименование) ? "" : ", NAME = '" + plan.Наименование + "'") +
					"WHERE NREC = #comp(" + cPlan.ToString() + ")";

			using (var h = new DsqlHelper())
			{
				return h.ExecuteDsqlNoQuery(dsql) == 1;
			}
		}
	}
}