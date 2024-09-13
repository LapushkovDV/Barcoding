using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Galaktika.ERP;
using Galaktika.ERP.Interop;
using Galaktika.ESB.Adapter.ERP;
using Galaktika.ESB.Adapter.ERP.AppSpecificERP;
using Galaktika.ESB.Adapter.ERP.Helpers;
using Galaktika.ESB.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Submit;
using Microsoft.Restier.Publishers.OData.Model;
using NLog;

namespace Galaktika.ESB.Adapter.Atlantis.Api
{
    /// <summary>
    /// Api of the Galaktika ERP
    /// </summary>
	public partial class AtlantisApi
    {

        static partial void RegisterChangeSetInitializer(IServiceCollection services)
        {
            services.AddService<IChangeSetInitializer>((sp, next) => new ErpChangeSetInitializer());
        }
		
		[Operation(Namespace = "Galaktika.ERP.OData.oSHK_InOut", HasSideEffects = true)]
        public string GetDescriptionShk(string actionName)
        {
			"GetDescriptionShk".ErpInfo();
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeOrder = GalnetApi.InitCaller("C_BARCODE::oSHK_InOut", "C_BARCODE::SHK_InOut");
                var result = GalnetApi.FieldDef.Create(AtlTypes.String);
                string fileName = (string)objMakeOrder.CallMethod("GenerateJSON_DescriptionFILE", result, actionName);
				string.Format("Респонс ERP:{0}", fileName).ErpInfo();
				return CompressionHelper.Compress(FileHelper.ReadAllBytes(fileName));
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }
		
		[Operation(Namespace = "Galaktika.ERP.OData.oSHK_InOut", HasSideEffects = true)]
        public string GetObjectByIdentifier(string actionName, string ident, string userName, string imei)
        {
			"GetObjectByIdentifier".ErpInfo();
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeOrder = GalnetApi.InitCaller("C_BARCODE::oSHK_InOut", "C_BARCODE::SHK_InOut");
                var result = GalnetApi.FieldDef.Create(AtlTypes.String);
                string fileName = (string)objMakeOrder.CallMethod("GenerateJSON_VALUESFILE", result, actionName, ident, userName, imei);
				fileName.ErpInfo();
				return CompressionHelper.Compress(FileHelper.ReadAllBytes(fileName));
            }			
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }
				
		[Operation(Namespace = "Galaktika.ERP.OData.oSHK_InOut", HasSideEffects = true)]
        public string ExecuteRequest(string userName, string imei, string request)
        {
			"ExecuteRequest".ErpInfo();
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeOrder = GalnetApi.InitCaller("C_BARCODE::oSHK_InOut", "C_BARCODE::SHK_InOut");
                var result = GalnetApi.FieldDef.Create(AtlTypes.String);
				string directory = string.Format(@"{0}\ShkTmp", Environment.CurrentDirectory);
				if (!System.IO.Directory.Exists(directory))
					System.IO.Directory.CreateDirectory(directory);
				string contentFileName = string.Format(@"{0}\{1}.txt", directory, Guid.NewGuid());
				FileHelper.WriteAllText(contentFileName, request, Encoding.UTF8);
				string.Format("Запрос в ERP:{0}", contentFileName).ErpInfo();
                string fileName = (string)objMakeOrder.CallMethod("MakeAction_FromJSONFILE", result, userName, imei, contentFileName);
				string.Format("Респонс ERP:{0}", fileName).ErpInfo();
				return CompressionHelper.Compress(FileHelper.ReadAllBytes(fileName));
            }
			catch(Exception ex)
			{
				ex.Message.ErpInfo();
				return string.Empty;
			}
            finally
            {				
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// функция возвращает следующий номер сопроводительного документа, доступна с версии C_ESB res 9.2.10.0
        /// </summary>
        /// <param name="pVidSopr"></param>
        /// <returns></returns>
        [Operation(HasSideEffects = false)]
        public string GetNextNumberSoprDoc(int pVidSopr)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeOrder = GalnetApi.InitCaller("C_ESB::ObjCallFromESB", "C_ESB::CallFromESB");
                var result = GalnetApi.FieldDef.Create(AtlTypes.String);
                return (string)objMakeOrder.CallMethod("GetNextNumberKatSopr", result, Convert.ToUInt16(pVidSopr));
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        ///      Получение по NT логину дескриптора. 
	    ///  Если DB пользователь не найден по NT логину, то возвращается текст 'Не определен'.
	    ///  Иначе будет возвращен дескриптор пользовтаеля
        /// </summary>
        /// <param name="sNTLogin"></param>
        /// <returns></returns>
	    [Operation(HasSideEffects = false)]
        public string GetDescrByNTLogin(string sNTLogin)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeOrder = GalnetApi.InitCaller("C_TUNE::iGetUsersEx2", "C_TUNE::GetUsers");
                var result = GalnetApi.FieldDef.Create(AtlTypes.String);
                var resultStr = (string)objMakeOrder.CallMethod("GetDescrByNTLogin", result, sNTLogin);
                return resultStr;
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        ///      Получение по NT логину группы дескриптора. 
	    ///   Если DB пользователь не найден по NT логину, то возвращается текст 'Не определен'.
	    ///   Иначе будет возвращена группа дескриптора пользовтаеля
        /// </summary>
        /// <param name="sNTLogin"></param>
        /// <returns></returns>
	    [Operation(HasSideEffects = false)]
        public string GetGrDescrByNTLogin(string sNTLogin)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeOrder = GalnetApi.InitCaller("C_TUNE::iGetUsersEx2", "C_TUNE::GetUsers");
                var result = GalnetApi.FieldDef.Create(AtlTypes.String);
                var resultStr = (string)objMakeOrder.CallMethod("GetGrDescrByNTLogin", result, sNTLogin);
                return resultStr;
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// процедура распределения суммы платежа ERP
        /// </summary>
        /// <param name="cSHoz"></param>
        /// <param name="cxRasp"></param>
        /// <returns></returns>
	    [Operation(HasSideEffects = false)]
        public bool DoDistrSoprHoz(long cSHoz, int cxRasp)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                var _cxRasp = Convert.ToUInt16(cxRasp);
                objMakeOrder = GalnetApi.InitCaller("F_DistPl::ASoprHozSumDistr", "F_DistPl::RaznSopHoz");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
                var resultStr = (bool)objMakeOrder.CallMethod("_DoDistrSoprHoz", result, cSHoz, _cxRasp);
                return resultStr;
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// Ненулевой системный код заданного элемента каталога или ближайшего вышестоящего
        /// </summary>
        /// <param name="cNRec"></param>
        /// <returns>int</returns>
        [Operation(HasSideEffects = false)]
        public short GetSysCodeIer(long cNRec)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeOrder = GalnetApi.InitCaller("Z_StaffCat::ObjCatFunc", "Z_StaffCat::CatalogsFunctions");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Integer);
                var resultStr = (short)objMakeOrder.CallMethod("GetSysCodeIer", result, cNRec);
                return resultStr;
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// Gets SPSOPR by KATSOPR value
        /// </summary>
        /// <param name="katsopr">KATSOPR entity</param>
        /// <returns>Variety of SPSOPR</returns>
		[Operation(Namespace = "Galaktika.ERP.OData.Models.KATSOPR", IsBound = true, HasSideEffects = false)]
        public virtual IQueryable<SPSOPR> GetSpSopr(KATSOPR katsopr)
        {
            var result = this.DbContext.SPSOPR.Where(s => s.CSOPR == katsopr.NREC);
            return result;
        }

        /// <summary>
        /// Удаление KATSOPR
        /// </summary>
        /// <param name="cnRecDoc">Нрек документа</param>
        /// <returns>Результат в виде инт значенияю. 0 - успешно</returns>
	    [Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = false)]
        public int Del_KatSopr(long cnRecDoc)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeOrder = GalnetApi.InitCaller("M_UP::OBJPROCSOPRDOC5", "M_UP::iProcSoprDoc");
                var result = GalnetApi.FieldDef.Create(AtlTypes.LongInt);
                return (int)objMakeOrder.CallMethod("Del_KatSopr", result, cnRecDoc);
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// Получение SPORDER по объекту KATSOPR
        /// </summary>
        /// <param name="katsopr">Объект KATSOPR</param>
        /// <returns>Множество SPORDER</returns>
        [Operation(Namespace = "Galaktika.ERP.OData.Models.KATSOPR", IsBound = true, HasSideEffects = false)]
        public virtual IQueryable<SPORDER> GetSpOrder(KATSOPR katsopr)
        {
            var SPSOPRNrecList = GetSpSopr(katsopr).Select(s => s.NREC);
            var result = this.DbContext.SPORDER.Where(s => SPSOPRNrecList.Contains(s.CSPSOPR));
            return result;
        }

        /// <summary>
        /// Получение значения поля ATTRVAL
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="nrec">Нрек таблицы</param>
        /// <param name="attrName">Имя аттрибута</param>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="info">Выводить ли информацию</param>
        /// <returns>AttrValue в виде строки</returns>
		[Operation(HasSideEffects = false)]
        public string GetAttrValue(string tableName, long nrec, string attrName, string fieldName, int info)
        {
            using (var dbContext = new AtlantisApi.AtlantisContext())
            {
                var meta = dbContext.GetMeta;
                var table = meta.GetTableByName(tableName);
                if (table == null)
                {
                    if (info == 0)
                        return string.Empty;
                    return string.Format("Table with name {0} not found", tableName);
                }


                var tableCode = table.XF_CODE;

                var attriD = dbContext.ATTRNAM.Where(a => a.NAME == attrName && a.WTABLE == tableCode)
                    .Select(s => s.NREC).FirstOrDefault();
                if (attriD == default(long))
                {
                    if (info == 0)
                        return string.Empty;
                    return string.Format("Nrec of  attrName = {0} not found", attrName);
                }

                var res = dbContext.ATTRVAL.FirstOrDefault(v =>
                    v.WTABLE == tableCode && v.CATTRNAM == attriD && v.CREC == nrec);
                if (res != null)
                {
                    switch (fieldName.ToLower())
                    {
                        case ("vstring"):
                            return res.VSTRING;
                        case ("vdate"):
                            return res.VDATE.ToString();
                        case ("vtime"):
                            return res.VTIME.ToString();
                        case ("vdouble"):
                            return res.VDOUBLE.ToString();
                        case ("vlongint"):
                            return res.VLONGINT.ToString();
                        case ("vcomp"):
                            return res.VCOMP.ToString();
                        default:
                            if (info == 0)
                                return string.Empty;
                            return "Invalid fieldName";
                    }
                }

                if (info == 0)
                    return string.Empty;
                return "AttrVal not found. Check ur parameters";
            }
        }

        /// <summary>
        /// Устанавливает значение поля в таблице ATTRVAL
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="nrec">Нрек таблицы</param>
        /// <param name="attrName">Имя аттрибута</param>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="attrValue">Значение поля</param>
        /// <returns>Строку подтверждающую установку значения</returns>
        [Operation(HasSideEffects = false)]
        public string SetAttrValue(string tableName, long nrec, string attrName, string fieldName, string attrValue)
        {
            int tableCode;

            using (var dbContext = new AtlantisApi.AtlantisContext())
            {
                var meta = dbContext.GetMeta;
                var table = meta.GetTableByName(tableName);
                if (table == null)
                    return string.Format("Table with name {0} not found", tableName);

                tableCode = table.XF_CODE;
            }

            //var attriD = dbContext.ATTRNAM
            //    .Where(a => a.NAME == attrName && a.WTABLE == tableCode)
            //    .Select(s => s.NREC).FirstOrDefault();
            //if (attriD == default(long))
            //    return string.Format("Nrec of  attrName = {0} not found",
            //        attrName);

            //var res = dbContext.ATTRVAL.FirstOrDefault(v =>
            //    v.WTABLE == tableCode && v.CATTRNAM == attriD && v.CREC == nrec);
            //if (res == null)
            //{
            //    res = new ATTRVAL
            //    {
            //        WTABLE = tableCode,
            //        CATTRNAM = attriD,
            //        CREC = nrec
            //    };
            //    dbContext.ATTRVAL.Add(res);
            //}
            bool fRes;
            switch (fieldName.ToLower())
            {
                case ("vstring"):
                    fRes = sSetAttr(tableCode, nrec, attrName, attrValue);
                    if (!fRes)
                    {
                        "Result of sSetAttr = false. Yours parameters = {0}, {1}, {2}, {3}".ErpWarn(tableCode, nrec,
                            attrName, attrValue);
                    }

                    return attrValue;
                case ("vdate"):
                    var date = Convert.ToDateTime(attrValue).Date;
                    fRes = dSetAttr(tableCode, nrec, attrName, date == DateTime.MinValue ? DateTimeOffset.MinValue : new DateTimeOffset(date));
                    if (!fRes)
                    {
                        "Result of dSetAttr = false. Yours parameters = {0}, {1}, {2}, {3}".ErpWarn(tableCode, nrec,
                            attrName, date);
                    }

                    return attrValue;
                case ("vtime"):
                    var time = Convert.ToDateTime(attrValue).TimeOfDay;
                    fRes = tSetAttr(tableCode, nrec, attrName, time);
                    if (!fRes)
                    {
                        "Result of tSetAttr = false. Yours parameters = {0}, {1}, {2}, {3}".ErpWarn(tableCode, nrec,
                            attrName, time);
                    }

                    return attrValue;
                case ("vdouble"):
                    var db = Convert.ToDouble(attrValue);
                    fRes = doSetAttr(tableCode, nrec, attrName, db);
                    if (!fRes)
                    {
                        "Result of doSetAttr = false. Yours parameters = {0}, {1}, {2}, {3}".ErpWarn(tableCode, nrec,
                            attrName, db);
                    }

                    return attrValue;
                case ("vlongint"):
                    var b = Convert.ToBoolean(attrValue);
                    fRes = bSetAttr(tableCode, nrec, attrName, b);
                    if (!fRes)
                    {
                        "Result of bSetAttr = false. Yours parameters = {0}, {1}, {2}, {3}".ErpWarn(tableCode, nrec,
                            attrName, b);
                    }

                    return attrValue;
                case ("vcomp"):
                    var comp = Convert.ToInt64(attrValue);
                    fRes = coSetAttr(tableCode, nrec, attrName, comp, attrValue);
                    if (!fRes)
                    {
                        "Result of doSetAttr = false. Yours parameters = {0}, {1}, {2}, {3}, {4}".ErpWarn(tableCode,
                            nrec, attrName, comp, attrValue);
                    }

                    return attrValue;
                default:
                    return string.Format("Unknown fieldName {0}", fieldName);
            }
        }




        #region Comment part

        //[Operation(HasSideEffects = false)]
        //public string Stop()
        //{
        //	DispatcherHelper.BeginInvokeShutdown(DispatcherPriority.Background);
        //	return "Shutdowwn started...";
        //}

        //L_Sklad::Akt_Spis
        //Procedure pMakeOrder(cKATSOPR : comp); // Создание расходного ордера
        //Function sTestMessage(sMess : string):string ; // Тестовый метод

        //[Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = true)]
        //public string sTestMessage(string sMess)
        //{
        //	var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
        //	try
        //	{
        //		objMakeOrder = GalnetApi.InitCaller("L_Sklad::oAkt_SpisObj", "L_Sklad::Akt_spis");
        //		var result = GalnetApi.FieldDef.Create(AtlTypes.String);
        //		var resultStr = (string)objMakeOrder.CallMethod("sTestMessage", result, sMess);
        //		return resultStr;
        //	}
        //	catch (Exception e)
        //	{
        //		throw;
        //	}
        //	finally
        //	{
        //		objMakeOrder.MtCallDoneIfc();
        //	}

        //}
        //string sCallLong(long cKATSOPR, string methodName)
        //{
        //	var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
        //	try
        //	{
        //		objMakeOrder = GalnetApi.InitCaller("L_Sklad::oAkt_SpisObj", "L_Sklad::Akt_Spis");
        //		var result = GalnetApi.FieldDef.Create(AtlTypes.String);
        //		var resultStr = (string)objMakeOrder.CallMethod(methodName, result, cKATSOPR);
        //		return resultStr;
        //	}
        //	catch (Exception e)
        //	{
        //		throw;
        //	}
        //	finally
        //	{
        //		objMakeOrder.MtCallDoneIfc();
        //	}
        //}

        //[Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = true)]
        //public string sMakeOrder(long cKATSOPR)
        //{
        //	return sCallLong(cKATSOPR, "sMakeOrder");
        //}


        //[Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = true)]
        //public string cTestNrec(long cKATSOPR)
        //{
        //	return sCallLong(cKATSOPR, "cTestNrec");
        //}
        //[Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = true)]
        //public string sTestKATSOPRName(long cKATSOPR)
        //{
        //	return sCallLong(cKATSOPR, "sTestKATSOPRName");
        //}
        //[Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = true)]
        //public string sTestwGetTune(long cKATSOPR)
        //{
        //	return sCallLong(cKATSOPR, "sTestwGetTune");
        //}
        //[Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = true)]
        //public string sTestwboGetTune(long cKATSOPR)
        //{
        //	return sCallLong(cKATSOPR, "sTestwboGetTune");
        //}
        //[Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = true)]
        //public string sTestUpdate_Current_KATSOPR(long cKATSOPR)
        //{
        //	return sCallLong(cKATSOPR, "sTestUpdate_Current_KATSOPR");
        //}
        //[Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = true)]
        //public string sTest_Loop_SPSOPR(long cKATSOPR)
        //{
        //	return sCallLong(cKATSOPR, "sTest_Loop_SPSOPR");
        //}
        //[Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = true)]
        //public string sTestMakeRashOrd(long cKATSOPR)
        //{
        //	return sCallLong(cKATSOPR, "sTestMakeRashOrd");
        //}
        //[Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = true)]
        //public string sTestUserName()
        //{
        //	var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
        //	try
        //	{
        //		objMakeOrder = GalnetApi.InitCaller("L_Sklad::oAkt_SpisObj", "L_Sklad::Akt_Spis");
        //		var result = GalnetApi.FieldDef.Create(AtlTypes.String);
        //		var resultStr = (string)objMakeOrder.CallMethod("sTestUserName", result);
        //		return resultStr;
        //	}
        //	catch (Exception e)
        //	{
        //		throw;
        //	}
        //	finally
        //	{
        //		objMakeOrder.MtCallDoneIfc();
        //	}

        //}

        #endregion

        /// <summary>
        /// Создание SF при помощи KATSOPR
        /// </summary>
        /// <param name="cKatSopr">Nrec сущности KATSOPR</param>
        /// <returns>true - если успех, false - неудача</returns>
        [Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = false)]
        public bool CreateSFbyKatSopr(long cKatSopr)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeOrder = GalnetApi.InitCaller("L_SF::ISchFactFunctions",
                    "L_SF::viSchFunc");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
                return (bool)objMakeOrder.CallMethod("CreateSFbyKatSopr", result, cKatSopr);
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// Удаление SOPR
        /// </summary>
        /// <param name="pNRec">NREC</param>
        /// <param name="isWarnings">Выбрасывать ли исключения</param>
        /// <param name="isVisual">Использовать ли визуальную часть</param>
        /// <returns>true - если успех, false - неудача</returns>
        [Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = false)]
        public bool DeleteSopr(long pNRec, bool isWarnings, bool isVisual)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeOrder = GalnetApi.InitCaller("L_SOPRDOC::ObjDelSopr", "L_SOPRDOC::DelSopr");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
                return (bool)objMakeOrder.CallMethod("DeleteSopr", result, pNRec, isWarnings, isVisual);
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// Посчитать среднюю цену в накладной.
        /// </summary>
        /// <param name="pKatSopr">NREC KATSOPR</param>
        /// <returns>true - если успех, false - неудача</returns>
		[Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = true)]
        public bool CalcSrPriceInNakl(long pKatSopr)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeOrder = GalnetApi.InitCaller("L_SKLAD::objMakeOrder", "L_SKLAD::MakeOrder");
                objMakeOrder.CallMethod("CalcSrPriceInNakl", null, pKatSopr);
                return true;
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }

        }

        /// <summary>
        /// Пересчет цен по связанным позициям ведомости переработки МТР.
        /// </summary>
        /// <param name="pKatSopr">NREC KATSOPR</param>
        /// <returns>true - если успех, false - неудача</returns>
        [Operation(Namespace = "Galaktika.ERP.OData.PeresortObj", HasSideEffects = true)]
        public bool CalcSumRelPos(long pKatSopr)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);

            try
            {
                objMakeOrder = GalnetApi.InitCaller("L_SKLAD::PeresortObj", "L_SKLAD::Peresort");
                objMakeOrder.CallMethod("CalcSumRelPos", null, pKatSopr);
                return true;
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        [Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = true)]
        public bool CountPriceCU(DateTimeOffset vdOper, int awVidUch,
    long vcSaldTune, long vcObj,
    long vcKau1, long vcKau2, long vcKau3,
    long vcKau4, long vcKau5, long vcKau6, long vcKau7, long vcKau8,
    long vcKau9, long vcMc, long vcPod, long vcMol, long vcPar,
    double dSrPrice, double dvPrice, double dvKol, long cVal)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                DateTime _vdOper = vdOper.DateTime;

                var _awVidUch = Convert.ToUInt16(awVidUch);
                objMakeOrder = GalnetApi.InitCaller("L_SaldoMTR::OUksStoreSrPrice", "L_SaldoMTR::IUksStoreSrPrice");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
                var resultStr = (bool)objMakeOrder.CallMethod("CountPriceCU", result, _vdOper, _awVidUch, vcSaldTune, vcObj, vcKau1, vcKau2, vcKau3, vcKau4, vcKau5, vcKau6, vcKau7, vcKau8, vcKau9, vcMc, vcPod, vcMol, vcPar, dSrPrice, dvPrice, dvKol, cVal);
                return resultStr;
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }

        }
        /*
objinterface oAkt_SpisObj;
  Function sTestMessage(sMess: string): string;
  Function sMakeOrder(cKATSOPR : comp): string;
  Function cTestNrec(cKATSOPR : comp): comp;
  Function sTestKATSOPRName(cKATSOPR : comp): string;
  Function sTestwGetTune(cKATSOPR : comp): string;
  Function sTestwboGetTune(cKATSOPR : comp): string;
  Function sTestUpdate_Current_KATSOPR(cKATSOPR : comp): string;
  Function sTest_Loop_SPSOPR(cKATSOPR : comp): string;
  Function sTestMakeRashOrd(cKATSOPR : comp): string;
  Function sTestUserName: string;
end;
		 */

        /// <summary>
        /// Формирование приходного складского ордера.
        /// wMode - битовые маски режимов работы функции(см.константы в MakeOptionsByDefines.inc).
        /// поддерживаются биты WMODE_BATCH, WMODE_REQUEST_DPRICE, WMODE_USE_MSGBATCH
        /// wParam - указывает, какие дефайны определены в вызывающем модуле(см.константы в MakeOptionsByDefines.inc).
        /// Для формирования параметров wMode, wParam при вызове метода рекомендуется использовать
        /// procedure MakeOptionsByDefines(MakeOptionsByDefines.vpp).
        /// В MakeOptionsByDefines.inc описаны возможные значения параметров.
        /// </summary>
        /// <param name="pKatSopr"></param>
        /// <param name="wSkPr"></param>
        /// <param name="wMode"></param>
        /// <param name="wParam"></param>
        /// <returns></returns>
        [Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = true)]
        public bool MakePrihOrder
        (
            long pKatSopr,  // Ссылка на сопроводительный документ
            int wSkPr,      // Вид учета:
                            //      0 - Складской учет
                            //      1 - Учет в модуле "Управление производственной логистикой"
                            //      2 - Учет в модуле "Техническое обслуживание и ремонт оборудования"
                            //      3 - Учет в модуле "Управуление капитальными вложениями и строительством"
            int wMode,      // Битовая маска вариантов взаимодействия с пользователем (см. примечание и константы в OrderFuncModes.inc):
                            //      1 - Пакетный режим работы (не должны выдаваться запросы пользователю на ввод данных)
                            //      2 - Выдача запроса на ввод даты при выполнеии условий: if(boGetTune('Oper.Buy.Nakl.dPrice')) And (KATSOPR.cVal <> 0) And (KATSOPR.vidSopr = 101 Or KATSOPR.vidSopr = 108)
                            //      4 - Протоколирование сообщений в объект MsgBatch
            int wParam      // Битовая маска для указания, какие дефайны определены в вызывающем модуле (см. примечание и константы в MakeOptionsByDefines.inc):
                            //      1 - SMETAKOLADD_DEFINED
                            //      2 - _USE_SERIAL__DEFINED
                            //      4 - G_L_PR_VPP_DEFINED
                            //      8 - _BOXNEEDAUTO_DEFINED
                            //     16 - _PRIHOD__DEFINED
                            //     32 - __PAKET_VIP___DEFINED
                            //     64 - _RETTARA__DEFINED
                            //    128 - _s2o__DEFINED
        ) // возвращает true при успехе
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                bool ShowMess = false; //#docl Выводить ли пользовательские сообщения
                bool firstRozn = false; // var boolean;
                bool prRozn = false; //var boolean;
                var _wSkPr = Convert.ToUInt16(wSkPr);
                var _wMode = Convert.ToUInt16(wMode);
                var _wParam = Convert.ToUInt16(wParam);

                objMakeOrder = GalnetApi.InitCaller("L_SKLAD::objMakeOrder", "L_SKLAD::MakeOrder");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
                return (bool)objMakeOrder.CallMethod("MakePrihOrder", result, pKatSopr, ShowMess, firstRozn, prRozn, _wSkPr, _wMode, _wParam);
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }


        /// <summary>
        /// Формирование расходного ордера 
        /// </summary>
        /// <param name="pKatSopr"></param>
        /// <param name="pDateOrd"></param>
        /// <param name="wSkPr"></param>
        /// <param name="wMode"></param>
        /// <param name="wParam"></param>
        /// <returns></returns>
        [Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = true)]
        public bool MakeRashOrd
        (
            long pKatSopr,  // Ссылка на сопроводительный документ
            DateTimeOffset pDateOrd, // Дата формирования ордера. Если передано пусто, то в непакетном режиме выдается запрос на ввод даты
            int wSkPr,      // Вид учета:
                            //      0 - Складской учет
                            //      1 - Учет в модуле "Управление производственной логистикой"
                            //      2 - Учет в модуле "Техническое обслуживание и ремонт оборудования"
                            //      3 - Учет в модуле "Управуление капитальными вложениями и строительством"
            int wMode,      // Битовая маска вариантов взаимодействия с пользователем (см. примечание и константы в OrderFuncModes.inc):
                            //      1 - Пакетный режим работы (не должны выдаваться запросы пользователю на ввод данных)
                            //      2 - Выдача запроса на ввод даты при выполнеии условий: if(boGetTune('Oper.Buy.Nakl.dPrice')) And (KATSOPR.cVal <> 0) And (KATSOPR.vidSopr = 101 Or KATSOPR.vidSopr = 108)
                            //      4 - Протоколирование сообщений в объект MsgBatch
            int wParam      // Битовая маска для указания, какие дефайны определены в вызывающем модуле (см. примечание и константы в MakeOptionsByDefines.inc):
                            //      1 - SMETAKOLADD_DEFINED
                            //      2 - _USE_SERIAL__DEFINED
                            //      4 - G_L_PR_VPP_DEFINED
                            //      8 - _BOXNEEDAUTO_DEFINED
                            //     16 - _PRIHOD__DEFINED
                            //     32 - __PAKET_VIP___DEFINED
                            //     64 - _RETTARA__DEFINED
                            //    128 - _s2o__DEFINED
        ) // возвращает true при успехе
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                var _pDateOrd = pDateOrd.DateTime;
                bool needMsg = false; //#docl Выводить ли пользовательские сообщения
                var _wSkPr = Convert.ToUInt16(wSkPr);
                var _wMode = Convert.ToUInt16(wMode);
                var _wParam = Convert.ToUInt16(wParam);
                int frmProt = 0; // var int;

                objMakeOrder = GalnetApi.InitCaller("L_SKLAD::objMakeOrder", "L_SKLAD::MakeOrder");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
                return (bool)objMakeOrder.CallMethod("MakeRashOrd", result, pKatSopr, needMsg, _pDateOrd, _wSkPr, _wMode, _wParam, frmProt);
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// проверка возможности удаления ордера
        /// </summary>
        [Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = false)]
        public bool CanDelOrdEx(
                long pKatSopr
        )
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                bool mesPrmt = false;
                bool checkTune = true;
                ushort wMode = 0;
                objMakeOrder = GalnetApi.InitCaller("L_SKLAD::objMakeOrder", "L_SKLAD::MakeOrder");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
                return (bool)objMakeOrder.CallMethod("CanDelOrdEx", result, pKatSopr, mesPrmt, checkTune, wMode);
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// RecalcSoprDoc
        /// </summary>
        /// <param name="pViSopr"></param>
        /// <param name="pKatSopr"></param>
        /// <param name="pSpSopr"></param>
        /// <param name="pValDoc"></param>
        /// <param name="pPrMC"></param>
        /// <returns></returns>
		[Operation(Namespace = "Galaktika.ERP.OData.objKatSoprFunc", HasSideEffects = true)]
        public bool RecalcSoprDoc(int pViSopr, long pKatSopr, long pSpSopr, long pValDoc, int pPrMC)
        {
            var objKatSoprFunc = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                var _pViSopr = Convert.ToUInt16(pViSopr);
                var _pPrMC = Convert.ToUInt16(pPrMC);
                objKatSoprFunc = GalnetApi.InitCaller("L_SoprDoc::objKatSoprFunc", "L_SoprDoc::iKatSoprFunc");
                objKatSoprFunc.CallMethod("RecalcSoprDoc", null, _pViSopr, pKatSopr, pSpSopr, pValDoc, _pPrMC);
                return true;
            }
            finally
            {
                objKatSoprFunc.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// Удаление ордеров
        /// </summary>
        /// <param name="pKatSopr">NREC KATSOPR</param>
        /// <param name="MesPrmt">Выдавать ли сообщение</param>
        /// <param name="make_prih">Создавать ли приходный ордер</param>
        /// <param name="make_rash">Создавать ли расходный ордер</param>
        /// <param name="wMode"></param>
        /// <param name="wParam"></param>
        /// <returns>true - если успех, false - неудача</returns>
		[Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = true)]
        public bool DeleteOrders(long pKatSopr, bool MesPrmt, bool make_prih, bool make_rash, int wMode, int wParam)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                var _wMode = Convert.ToUInt16(wMode);
                var _wParam = Convert.ToUInt16(wParam);
                objMakeOrder = GalnetApi.InitCaller("L_SKLAD::objMakeOrder", "L_SKLAD::MakeOrder");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
                return (bool)objMakeOrder.CallMethod("DeleteOrders", result, pKatSopr, MesPrmt, make_prih, make_rash, _wMode, _wParam);
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }


        /// <summary>
        /// Формирование ордера
        /// </summary>
        /// <param name="pKatSopr">Ссылка на KatSopr</param>
        /// <param name="make_rash">Формировать ли расходный ордер</param>
        /// <param name="make_prih">Формировать ли прихдный ордер</param>
        /// <returns></returns>
        [Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = true)]
        public bool MakeOrderAktPer(long pKatSopr, bool make_rash, bool make_prih)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                var paket_ = false;
                objMakeOrder = GalnetApi.InitCaller("L_SKLAD::objMakeOrdAktPer", "L_SKLAD::MakeOrdAktPer");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
                return (bool)objMakeOrder.CallMethod("MakeOrderAktPer", result, pKatSopr, paket_, 0, make_rash, make_prih);
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// Формирование ордера (вариант для ESB)
        /// </summary>
        /// <param name="pKatSopr">Ссылка на KatSopr</param>
        /// <param name="make_rash">Формировать ли расходный ордер</param>
        /// <param name="make_prih">Формировать ли прихдный ордер</param>
        /// <returns>
        /// Функция возвращает 0 в случае успеха лтбо код ошибки:
        ///   1 - Установлена настройка:  "Настройки Галактики \ Логистика \ Складской учет \ Разрешать ввод отрицательного количества в ордерах - нет"
        ///   2  - Невозможно создать складской ордер без указания серийных номеров!
        ///   3  - не указан набор аналитик целевого назначения
        ///   4  - Необходимо заполнить спецификацию для списания
        ///   5  - Не удалось загрузить интерфейс для работы с журналом хоз.операций
        ///   6  - Необходимо указать МОЛ для оприходования
        ///   7  - Укажите подразделение для списания
        ///   8  - Укажите подразделение для оприходования
        ///   9  - Установлена настройка:  "Настройки Галактики \ Логистика \ Складской учет \ Разрешать создание ордеров по архивных МЦ - нет
        ///   10 - Установлена настройка:  "Настройки Галактики \ Логистика \ Складской учет \ Контролировать дискретность и кратность количества при формировании ордеров - запрещать формирование"
        ///   11 - В настройке установлен запрет модификации ордеров в закрытом периоде
        ///   12 - Запрещено создание ордеров при невозможности полного распределения по ячейкам хранения
        ///   14 - Установлен контроль списания МЦ без наличия
        ///   15 - Установлен контроль списания МЦ по аналитикам целевого учета
        /// </returns>
        [Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = true)]
        public int MakeOrderAktPerW(long pKatSopr, bool make_rash, bool make_prih)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                var paket_ = true;
                objMakeOrder = GalnetApi.InitCaller("L_SKLAD::objMakeOrdAktPer", "L_SKLAD::MakeOrdAktPer");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Word);
                return (int)objMakeOrder.CallMethod("MakeOrderAktPerW", result, pKatSopr, paket_, 0, make_rash, make_prih);
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// Формирование ордеров
        /// </summary>
        /// <param name="pKatSopr">Ссылка на сопроводительный документ</param>
        /// <param name="pVidSopr"></param>
        /// <param name="pDOpr"></param>
        /// <param name="make_rash"></param>
        /// <param name="make_prih"></param>
        /// <param name="wSkPr"></param>
        /// <param name="wMode"></param>
        /// <param name="wParam"></param>
        /// <returns></returns>
        [Operation(Namespace = "Galaktika.ERP.OData.objCreateOrder", HasSideEffects = true)]
        public bool CreateOrderAllVidSopr
        (
            long pKatSopr,  // Ссылка на сопроводительный документ
            int pVidSopr,   // Ссылка на тип сопроводительного документа
            DateTimeOffset pDOpr, // Дата оприходования
            bool make_rash, // Формировать расходный ордер(при раздельном формировании ордеров)
            bool make_prih, // Формировать приходный ордер(при раздельном формировании ордеров)
            int wSkPr,      // Вид учета:
                            //      0 - Складской учет
                            //      1 - Учет в модуле "Управление производственной логистикой"
                            //      2 - Учет в модуле "Техническое обслуживание и ремонт оборудования"
                            //      3 - Учет в модуле "Управуление капитальными вложениями и строительством"
            int wMode,      // Битовая маска вариантов взаимодействия с пользователем (см. примечание и константы в OrderFuncModes.inc):
                            //      1 - Пакетный режим работы (не должны выдаваться запросы пользователю на ввод данных)
                            //      2 - Выдача запроса на ввод даты при выполнеии условий: if(boGetTune('Oper.Buy.Nakl.dPrice')) And (KATSOPR.cVal <> 0) And (KATSOPR.vidSopr = 101 Or KATSOPR.vidSopr = 108)
                            //      4 - Протоколирование сообщений в объект MsgBatch
            int wParam      // Битовая маска для указания, какие дефайны определены в вызывающем модуле (см. примечание и константы в MakeOptionsByDefines.inc):
                            //      1 - SMETAKOLADD_DEFINED
                            //      2 - _USE_SERIAL__DEFINED
                            //      4 - G_L_PR_VPP_DEFINED
                            //      8 - _BOXNEEDAUTO_DEFINED
                            //     16 - _PRIHOD__DEFINED
                            //     32 - __PAKET_VIP___DEFINED
                            //     64 - _RETTARA__DEFINED
                            //    128 - _s2o__DEFINED
        ) // возвращает true при успехе
        {
            var objCreateOrder = default(GalnetApi.PIfcMethodsCaller);

            try
            {
                var _pVidSopr = Convert.ToUInt16(pVidSopr);
                var _pDOpr = pDOpr.DateTime;
                bool _isMess = false;                       // Выводить ли пользовательские сообщения

                int _loptions = 1;                          // Опции, регулирующие работу интерфейса. 
                                                            // Задаются с помощью констант OPT_*, определенных в заголовочном файле интерфейса.
                                                            // Если ничего не устанавливаем - передать 0.
                                                            // OPT_HIDE_REQUESTS_TO_USER = 1 - Скрывать сообщения\диалоги для запроса действий пользователя (используется для пакетных режимов)

                var _wSkPr = Convert.ToUInt16(wSkPr);
                var _wMode = Convert.ToUInt16(wMode);
                var _wParam = Convert.ToUInt16(wParam);
                int _hHandle = 0;                           // Протокол ошибочных ситуаций

                objCreateOrder = GalnetApi.InitCaller("L_SKLAD::objCreateOrder", "L_SKLAD::CreateOrder");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
                return (bool)objCreateOrder.CallMethod("CreateOrderAllVidSopr", result, pKatSopr, _pVidSopr, _pDOpr, _isMess, make_rash, make_prih, _loptions, _wSkPr, _wMode, _wParam, _hHandle);
            }
            finally
            {
                objCreateOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// GenerateBarCodeEx
        /// </summary>
        /// <param name="wTable"></param>
        /// <param name="cSoprDoc"></param>
        /// <returns></returns>
	    [Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = false)]
        public string GenerateBarCodeEx(int wTable, long cSoprDoc)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                var _wTable = Convert.ToUInt16(wTable);
                objMakeOrder = GalnetApi.InitCaller("C_Common::o$RepBarCode", "C_Common::RepBarCode");
                var result = GalnetApi.FieldDef.Create(AtlTypes.String);
                return (string)objMakeOrder.CallMethod("GenerateBarCodeEx", result, _wTable, cSoprDoc);
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// Генерация баркода
        /// </summary>
        /// <param name="TiDkGal"></param>
        /// <param name="cSoprDoc"></param>
        /// <returns></returns>
	    [Operation(Namespace = "Galaktika.ERP.OData.objMakeOrder", HasSideEffects = false)]
        public string GenerateBarCode(int TiDkGal, long cSoprDoc)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                var _TiDkGal = Convert.ToUInt16(TiDkGal);
                objMakeOrder = GalnetApi.InitCaller("C_Common::o$RepBarCode", "C_Common::RepBarCode");
                var result = GalnetApi.FieldDef.Create(AtlTypes.String);
                return (string)objMakeOrder.CallMethod("GenerateBarCode", result, _TiDkGal, cSoprDoc);
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// CreateKNSell
        /// </summary>
        /// <param name="pKatSoprSrc"></param>
        /// <param name="dSoprKN"></param>
        /// <param name="dOprKN"></param>
        /// <param name="dSoprSN"></param>
        /// <param name="dOprSN"></param>
        /// <param name="wMakeOrder"></param>
        /// <param name="wMode"></param>
        /// <param name="wOptions"></param>
        /// <returns></returns>
        [Operation(HasSideEffects = false)]
        public long CreateKNSell(long pKatSoprSrc, DateTimeOffset dSoprKN, DateTimeOffset dOprKN, DateTimeOffset dSoprSN, DateTimeOffset dOprSN,
        int wMakeOrder, int wMode, int wOptions)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                var _dSoprKN = dSoprKN.DateTime;
                var _dOprKN = dOprKN.DateTime;
                var _dSoprSN = dSoprSN.DateTime;
                var _dOprSN = dOprSN.DateTime;
                var _wMakeOrder = Convert.ToUInt16(wMakeOrder);
                var _wMode = Convert.ToUInt16(wMode);
                var _wOptions = Convert.ToUInt16(wOptions);
                objMakeOrder = GalnetApi.InitCaller("L_SoprDoc::objCreateKNSell", "L_SoprDoc::CreateKNSell");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Comp);
                return (long)objMakeOrder.CallMethod("CreateKNSell", result, pKatSoprSrc, _dSoprKN, _dOprKN, _dSoprSN, _dOprSN, _wMakeOrder, _wMode, _wOptions);
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// GetFormPlWayAccou
        /// </summary>
        /// <param name="cWayAccou"></param>
        /// <returns></returns>
	    [Operation(HasSideEffects = false)]
        public long GetFormPlWayAccou(long cWayAccou)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeOrder = GalnetApi.InitCaller("M_MnPlan::objFTemplate", "M_MnPlan::oFTemplate");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Comp);
                return (long)objMakeOrder.CallMethod("GetFormPlWayAccou", result, cWayAccou);
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// GetNumberPlan
        /// </summary>
        /// <param name="cTempl"></param>
        /// <returns></returns>
        [Operation(HasSideEffects = false)]
        public string GetNumberPlan(long cTempl)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeOrder = GalnetApi.InitCaller("M_MnPlan::objFTemplate", "M_MnPlan::oFTemplate");
                var result = GalnetApi.FieldDef.Create(AtlTypes.String);
                return (string)objMakeOrder.CallMethod("GetNumberPlan", result, cTempl);
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// GetNamePlan
        /// </summary>
        /// <param name="cTempl"></param>
        /// <param name="cDoc"></param>
        /// <param name="wNumInterfSys"></param>
        /// <returns></returns>
        [Operation(HasSideEffects = false)]
        public string GetNamePlan(long cTempl, long cDoc, int wNumInterfSys)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeOrder = GalnetApi.InitCaller("M_MnPlan::objFTemplate", "M_MnPlan::oFTemplate");
                var result = GalnetApi.FieldDef.Create(AtlTypes.String);
                return (string)objMakeOrder.CallMethod("GetNamePlan", result, cTempl, cDoc, wNumInterfSys);
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

		/// <summary>
		/// InsertResSD	- Функция пересчета резервов по сопроводительным документам
		/// </summary>
		/// <param name="dFor">дата, на которую рассчитывать резерв</param>
		/// <param name="boAllDoc">TRUE = необходимо производить резервирование по всему документу</param>
		/// <param name="boReservIns">True = резервирование / False = снятие с резерва</param>
		/// <param name="_cKatSopr">ссылка на документ или на позиции спецификации</param>
		/// <param name="_cSpSopr"></param>
		/// <param name="_KolRes"></param>
		/// <param name="wModePar"></param>
		/// <returns>bool</returns>
		[Operation(HasSideEffects = false)]
	    public bool InsertResSD(DateTimeOffset dFor, bool boAllDoc, bool boReservIns, long _cKatSopr, long _cSpSopr, double _KolRes, int wModePar = 0)
	    {
		    var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
		    try
		    {
				//objMakeOrder = GalnetApi.InitCaller("L_Reserve::ObjSoprDocRes4", "L_Reserve::SoprDocRes");
				objMakeOrder = GalnetApi.InitCaller("L_Reserve::o$SoprDocRes", "L_Reserve::SoprDocRes");
				var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
			    var resultStr = (bool)objMakeOrder.CallMethod("InsertResSD", result, dFor.DateTime, boAllDoc, boReservIns, _cKatSopr, _cSpSopr, _KolRes, wModePar);
			    return resultStr;
		    }
		    finally
		    {
			    objMakeOrder.MtCallDoneIfc();
		    }
	    }

		/// <summary>
		/// Получение страны у STERR
		/// </summary>
		/// <param name="cNRecTerr">NREC записи STERR</param>
		/// <returns>int</returns>
		[Operation(HasSideEffects = false)]
	    public long GetCountry(long cNRecTerr)
	    {
		    var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
		    try
		    {
			    objMakeOrder = GalnetApi.InitCaller("Z_Staff::ObjAddressFunc", "Z_Staff::AddressFunc");
			    var result = GalnetApi.FieldDef.Create(AtlTypes.Comp);
			    var resultStr = (long)objMakeOrder.CallMethod("GetCountry", result, cNRecTerr);
			    return resultStr;
		    }
		    finally
		    {
			    objMakeOrder.MtCallDoneIfc();
		    }
	    }

	    /// <summary>
	    /// Получение полного адреса у ADDRESSN
	    /// </summary>
	    /// <param name="NRecAddressN">NREC записи ADDRESSN</param>
	    /// <returns>\string</returns>
	    [Operation(HasSideEffects = false)]
	    public string GetFullAddress(long NRecAddressN)
	    {
		    var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
		    try
		    {
			    objMakeOrder = GalnetApi.InitCaller("Z_Staff::ObjAddressFunc", "Z_Staff::AddressFunc");
			    var result = GalnetApi.FieldDef.Create(AtlTypes.String);
			    var resultStr = (string)objMakeOrder.CallMethod("GetFullAddress", result, NRecAddressN);
			    return resultStr;
		    }
		    finally
		    {
			    objMakeOrder.MtCallDoneIfc();
		    }
	    }

		/// <summary>
		/// Получение фамилии у Persons
		/// </summary>
		/// <param name="psnNrec">NREC записи в PERSONS</param>
		/// <returns>string</returns>
		[Operation(HasSideEffects = false)]
	    public string Get_LastName(long psnNrec)
	    {
		    var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
		    try
		    {
			    objMakeOrder = GalnetApi.InitCaller("Z_Staff::IPersonsFunctions16", "Z_Staff::PersonsFunctions");
			    var result = GalnetApi.FieldDef.Create(AtlTypes.String);
			    var resultStr = (string)objMakeOrder.CallMethod("Get_LastName", result, psnNrec);
			    return resultStr;
		    }
		    finally
		    {
			    objMakeOrder.MtCallDoneIfc();
		    }
	    }

	    /// <summary>
		/// Получение имени у Persons
		/// </summary>
		/// <param name="psnNrec">NREC записи в PERSONS</param>
		/// <returns>string</returns>
	    [Operation(HasSideEffects = false)]
	    public string Get_FirstName(long psnNrec)
	    {
		    var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
		    try
		    {
			    objMakeOrder = GalnetApi.InitCaller("Z_Staff::IPersonsFunctions16", "Z_Staff::PersonsFunctions");
			    var result = GalnetApi.FieldDef.Create(AtlTypes.String);
			    var resultStr = (string)objMakeOrder.CallMethod("Get_FirstName", result, psnNrec);
			    return resultStr;
		    }
		    finally
		    {
			    objMakeOrder.MtCallDoneIfc();
		    }
	    }

	    /// <summary>
	    /// Получение отчества у Persons
	    /// </summary>
	    /// <param name="psnNrec">NREC записи в PERSONS</param>
	    /// <returns>string</returns>
	    [Operation(HasSideEffects = false)]
	    public string GetPatronymic(long psnNrec)
	    {
		    var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
		    try
		    {
			    objMakeOrder = GalnetApi.InitCaller("Z_Staff::IPersonsFunctions10", "Z_Staff::PersonsFunctions");
			    var result = GalnetApi.FieldDef.Create(AtlTypes.String);
			    var resultStr = (string)objMakeOrder.CallMethod("GetPatronymic", result, psnNrec);
			    return resultStr;
		    }
		    finally
		    {
			    objMakeOrder.MtCallDoneIfc();
		    }
	    }

		/// <summary>
		/// Получение NAME каталога
		/// </summary>
		/// <param name="cPar">NREC каталога</param>
		/// <returns>string</returns>
		[Operation(HasSideEffects = false)]
	    public string GetCatalogsName(long cPar)
	    {
		    var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
		    try
		    {
			    objMakeOrder = GalnetApi.InitCaller("Z_StaffCat::ObjCatFunc", "Z_StaffCat::CatalogsFunctions");
			    var result = GalnetApi.FieldDef.Create(AtlTypes.String);
			    var resultStr = (string)objMakeOrder.CallMethod("GetCatalogsName", result, cPar);
			    return resultStr;
		    }
		    finally
		    {
			    objMakeOrder.MtCallDoneIfc();
		    }
	    }

		/// <summary>
		/// Функция получения рабочего периода, содержащего дату dOtchDate, по назначению cApp сотрудника; 
		/// по параметру bFindFirstWP развязка нужен только РП, в который попадает дата, или нужен первый РП на дату
		/// </summary>
		/// <param name="cPers">NREC PERSONS</param>
		/// <param name="cApp">NREC APPOINTMENTS</param>
		/// <param name="dOtchDate">NREC дата в промежутке WORKPERIOD</param>
		/// <param name="bFindFirstWP">по параметру bFindFirstWP развязка нужен только РП, в который попадает дата, или нужен первый РП на дату</param>
		/// <returns>string</returns>
		[Operation(HasSideEffects = false)]
	    public long GetWPByDateAndApp(long cPers, long cApp, DateTimeOffset dOtchDate, bool bFindFirstWP)
	    {
		    var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
		    try
		    {
			    //var dateTime = Convert.ToDateTime(dOtchDate);
				objMakeOrder = GalnetApi.InitCaller("Z_Staff::IWorkPeriods", "Z_Staff::WorkPeriods");
			    var result = GalnetApi.FieldDef.Create(AtlTypes.Comp);
			    var resultStr = (long)objMakeOrder.CallMethod("GetWPByDateAndApp", result, cPers, cApp, dOtchDate.DateTime, bFindFirstWP);
			    return resultStr;
		    }
		    finally
		    {
			    objMakeOrder.MtCallDoneIfc();
		    }
	    }

		/// <summary>
		/// Получение CODE каталога
		/// </summary>
		/// <param name="cPar">NREC каталога</param>
		/// <returns>string</returns>
		[Operation(HasSideEffects = false)]
	    public string GetCatalogsCode(long cPar)
	    {
		    var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
		    try
		    {
			    objMakeOrder = GalnetApi.InitCaller("Z_StaffCat::ObjCatFunc", "Z_StaffCat::CatalogsFunctions");
			    var result = GalnetApi.FieldDef.Create(AtlTypes.String);
			    var resultStr = (string)objMakeOrder.CallMethod("GetCatalogsCode", result, cPar);
			    return resultStr;
		    }
		    finally
		    {
			    objMakeOrder.MtCallDoneIfc();
		    }
	    }

		/// <summary>
		/// GetTypeDbDriver
		/// </summary>
		/// <returns></returns>
		[Operation(HasSideEffects = false)]
        public int GetTypeDbDriver()
        {
            var methodsCaller = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                methodsCaller = GalnetApi.InitCaller("L_Common::oDSQL", "L_Common::DSQL");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Word);
                return (int)methodsCaller.CallMethod("GetTypeDbDriver", result);
            }
            finally
            {
                methodsCaller.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// GetStringParameterExt
        /// </summary>
        /// <param name="aGroupName"></param>
        /// <param name="aParameterName"></param>
        /// <param name="aFlags"></param>
        /// <returns></returns>
        [Operation(HasSideEffects = false)]
        public string GetStringParameterExt(string aGroupName, string aParameterName, int aFlags)
        {
            var methodsCaller = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                methodsCaller = GalnetApi.InitCaller("M_MnPlan::IObjectParPr", "M_MnPlan::ObjectParPr");
                var result = GalnetApi.FieldDef.Create(AtlTypes.String);
                return (string)methodsCaller.CallMethod("GetStringParameterExt", result, aGroupName, aParameterName, aFlags);
            }
            finally
            {
                methodsCaller.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// coGetTune
        /// </summary>
        /// <param name="sKey"></param>
        /// <returns></returns>
        [Operation(HasSideEffects = false)]
        public long coGetTune(string sKey)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeOrder = GalnetApi.InitCaller("C_TUNE::iTuneFuncs", "C_TUNE::TuneFuncsMain");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Comp);
                var resultStr = (long)objMakeOrder.CallMethod("tf_coGetTune", result, sKey);
                return resultStr;
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// wGetTune
        /// </summary>
        /// <param name="sKey"></param>
        /// <returns></returns>
        [Operation(HasSideEffects = false)]
        public int wGetTune(string sKey)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeOrder = GalnetApi.InitCaller("C_TUNE::iTuneFuncs", "C_TUNE::TuneFuncsMain");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Word);
                var resultStr = (int)objMakeOrder.CallMethod("tf_wGetTune", result, sKey);
                return resultStr;
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// dGetTune
        /// </summary>
        /// <param name="sKey"></param>
        /// <returns></returns>
        [Operation(HasSideEffects = false)]
        public DateTimeOffset dGetTune(string sKey)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeOrder = GalnetApi.InitCaller("C_TUNE::iTuneFuncs", "C_TUNE::TuneFuncsMain");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Date);
                var resultStr = (DateTimeOffset)objMakeOrder.CallMethod("tf_dGetTune", result, sKey);
                return resultStr;
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// Возвращает NRec элемента каталога с заданным системным кодом
        /// </summary>
        /// <param name="cSysCode">Системный код</param>
        /// <returns>NRec элемента каталога</returns>
        [Operation(HasSideEffects = false)]
        public long GetCatalogsNRec(int cSysCode)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeOrder = GalnetApi.InitCaller("Z_StaffCat::ObjCatFunc", "Z_StaffCat::CatalogsFunctions");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Comp);
                var resultStr = (long)objMakeOrder.CallMethod("GetCatalogsNRec", result, cSysCode);
                return resultStr;
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// Устанавливает значение атрибута типа string
        /// </summary>
        /// <param name="_wTable">Таблица для которой будет задано значение атрибута</param>
        /// <param name="_cRec">nrec таблицы</param>
        /// <param name="_Name">имя атрибута</param>
        /// <param name="_Val">значение атрибута</param>
        /// <returns>bool</returns>
        [Operation(HasSideEffects = false)]
        public bool sSetAttr(int _wTable, long _cRec, string _Name, string _Val)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                var wTable = Convert.ToUInt16(_wTable);
                objMakeOrder = GalnetApi.InitCaller("C_ExtClass::objExtAttr", "C_ExtClass::iExtAttr");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
                var resultStr = (bool)objMakeOrder.CallMethod("sSetAttr", result, wTable, _cRec, _Name, _Val);
                return resultStr;
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        [Operation(HasSideEffects = false)]
        public bool doSetAttr(int _wTable, long _cRec, string _Name, double _Val)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                var wTable = Convert.ToUInt16(_wTable);
                objMakeOrder = GalnetApi.InitCaller("C_ExtClass::objExtAttr", "C_ExtClass::iExtAttr");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
                var resultStr = (bool)objMakeOrder.CallMethod("doSetAttr", result, wTable, _cRec, _Name, _Val);
                return resultStr;
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }


        /// <summary>
        /// Устанавливает значение атрибута типа date
        /// </summary>
        /// <param name="_wTable">Таблица для которой будет задано значение атрибута</param>
        /// <param name="_cRec">nrec таблицы</param>
        /// <param name="_Name">имя атрибута</param>
        /// <param name="_Val">значение атрибута</param>
        /// <returns>bool</returns>
        [Operation(HasSideEffects = false)]
        public bool dSetAttr(int _wTable, long _cRec, string _Name, DateTimeOffset _Val)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                var wTable = Convert.ToUInt16(_wTable);
                objMakeOrder = GalnetApi.InitCaller("C_ExtClass::objExtAttr", "C_ExtClass::iExtAttr");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
                var resultStr = (bool)objMakeOrder.CallMethod("dSetAttr", result, wTable, _cRec, _Name, _Val.DateTime);
                return resultStr;
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }


        [Operation(HasSideEffects = false)]
        public bool tSetAttr(int _wTable, long _cRec, string _Name, TimeSpan _Val)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                var wTable = Convert.ToUInt16(_wTable);
                objMakeOrder = GalnetApi.InitCaller("C_ExtClass::objExtAttr", "C_ExtClass::iExtAttr");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
                var resultStr = (bool)objMakeOrder.CallMethod("tSetAttr", result, wTable, _cRec, _Name, _Val);
                return resultStr;
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }


        /// <summary>
        /// Устанавливает значение атрибута 
        /// </summary>
        /// <param name="_wTable">Таблица для которой будет задано значение атрибута</param>
        /// <param name="_cRec">nrec таблицы</param>
        /// <param name="_Name">имя атрибута</param>
        /// <param name="_Val">значение атрибута</param>
        /// <returns>bool</returns>
        [Operation(HasSideEffects = false)]
        public bool mSetAttr(int _wTable, long _cRec, string _Name, string _Val)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                var wTable = Convert.ToUInt16(_wTable);
                objMakeOrder = GalnetApi.InitCaller("C_ExtClass::objExtAttr", "C_ExtClass::iExtAttr");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
                var resultStr = (bool)objMakeOrder.CallMethod("mSetAttr", result, wTable, _cRec, _Name, _Val);
                return resultStr;
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }


        [Operation(HasSideEffects = false)]
        public bool coSetAttr(int _wTable, long _cRec, string _Name, long _Val, string _ValStr)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                var wTable = Convert.ToUInt16(_wTable);
                objMakeOrder = GalnetApi.InitCaller("C_ExtClass::objExtAttr", "C_ExtClass::iExtAttr");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
                var resultStr = (bool)objMakeOrder.CallMethod("coSetAttr", result, wTable, _cRec, _Name, _Val, _ValStr);
                return resultStr;
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// Устанавливает значение атрибута типа longint
        /// </summary>
        /// <param name="_wTable">Таблица для которой будет задано значение атрибута</param>
        /// <param name="_cRec">nrec таблицы</param>
        /// <param name="_Name">имя атрибута</param>
        /// <param name="_Val">значение атрибута</param>
        /// <returns>bool</returns>
        [Operation(HasSideEffects = false)]
        public bool bSetAttr(int _wTable, long _cRec, string _Name, bool _Val)
        {
            var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                var wTable = Convert.ToUInt16(_wTable);
                objMakeOrder = GalnetApi.InitCaller("C_ExtClass::objExtAttrEx5", "C_ExtClass::iExtAttr");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
                var resultStr = (bool)objMakeOrder.CallMethod("bSetAttr", result, wTable, _cRec, _Name, _Val);
                return resultStr;
            }
            finally
            {
                objMakeOrder.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// Устанавливает значение филиала в текущей сессии
        /// </summary>
        /// <param name="cFilialNrec">nrec филиала</param> 
        /// <returns>bool</returns>
        [Operation(HasSideEffects = false)]
        public bool SetBranches(long cFilialNrec)
        {
            var objMakeCaller = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeCaller = GalnetApi.InitCaller("C_ESB::ObjCallFromESB", "C_ESB::CallFromESB");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Void);
                objMakeCaller.CallMethod("SetBranchesESB", result, cFilialNrec);
                return true;
            }
            finally
            {
                objMakeCaller.MtCallDoneIfc();
            }
        }
        /// <summary>
        /// Формирует ДО
        /// </summary>
        /// <param name="pVidSopr"></param>
        /// <param name="cKatSopr">Nrec сущности KATSOPR</param>
        /// <param name="pDSopr"></param>
        /// <returns>bool</returns>
        [Operation(HasSideEffects = false)]
        public bool CreateDONew(int pVidSopr, long cKatSopr, DateTimeOffset pDSopr)
        {
            var objMakeCaller = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeCaller = GalnetApi.InitCaller("C_ESB::ObjCallFromESB",
                "C_ESB::CallFromESB");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Void);
                objMakeCaller.CallMethod("do_batch_Button", result, Convert.ToUInt16(pVidSopr), cKatSopr, pDSopr.DateTime);
                return true;
            }
            finally
            {
                objMakeCaller.MtCallDoneIfc();
            }
        }

        /// <summary>
        /// Формирует СФ
        /// </summary>
        /// <param name="wDirect"></param>
        /// <param name="cKatSopr">Nrec сущности KATSOPR</param>
        /// <returns>bool</returns>
        public bool CreateSF(int wDirect, long cKatSopr)
        {
            var objMakeCaller = default(GalnetApi.PIfcMethodsCaller);
            try
            {
                objMakeCaller = GalnetApi.InitCaller("C_ESB::ObjCallFromESB",
                "C_ESB::CallFromESB");
                var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
                return (bool)objMakeCaller.CallMethod("KatSoprCreateSF", result, Convert.ToUInt16(wDirect), cKatSopr);
            }
            finally
            {
                objMakeCaller.MtCallDoneIfc();
            }
        }

        //[Operation(HasSideEffects = false)]
        //public bool mSetAttr(int _wTable, long _cRec, string _Name, string _Val)
        //{
        //    var objMakeOrder = default(GalnetApi.PIfcMethodsCaller);
        //    try
        //    {
        //        var wTable = Convert.ToUInt16(_wTable);
        //        objMakeOrder = GalnetApi.InitCaller("C_ExtClass::objExtAttr", "C_ExtClass::iExtAttr");
        //        var result = GalnetApi.FieldDef.Create(AtlTypes.Boolean);
        //        var resultStr = (bool)objMakeOrder.CallMethod("mSetAttr", result, wTable, _cRec, _Name, _Val);
        //        return resultStr;
        //    }
        //    finally
        //    {
        //        objMakeOrder.MtCallDoneIfc();
        //    }
        //}


        /// <summary>
        /// Тестовый метод эхо
        /// </summary>
        /// <param name="str">Строка для проверки</param>
        /// <returns>"Hello" + строку для проверки</returns>
        [Operation(HasSideEffects = false)]
        public string Echo(string str)
        {
            bSetAttr(2, 2, "d2dass", true);
            return "Hello " + str;
        }

        /*
		#doc
		Удаление ордеров.
		# end
		Function DeleteOrders (pKATSOPR: comp; MesPrmt, make_prih, make_rash: boolean; wMode, wParam: word): boolean;

		//------------------------------------------------------------------------------
		#doc
			Перерасчет средних цен в накладной
		# end
		Procedure CalcSrPriceInNakl(pKATSOPR: comp);

				//------------------------------------------------------------------------------
		# doc
				Проверка наличия МЦ
		# end
		Function CheckNalTov(pKATSOPR: comp; Dat: Date; var Counts: integer; wMode, wParam: word; frmProt: longint): word;
		*/
        //[Operation(HasSideEffects = false)]
        //public File FF()
        //{
        //	return FirstFile;
        //}
        //[Operation(HasSideEffects = false)]
        //public async Task<File> FFA1()
        //{
        //	return await new AtlantisContext().Tables.FirstOrDefaultAsync((f) => f.XF_CODE == 1);
        //}
        //[Operation(HasSideEffects = false)]
        //public Task<File> FFA2()
        //{
        //	return new AtlantisContext().Tables.FirstOrDefaultAsync((f) => f.XF_CODE == 1);
        //}

        //[Resource]
        //public File FirstFile
        //{
        //	get
        //	{
        //		return new AtlantisContext().Tables.FirstOrDefault((f) => f.XF_CODE == 1);
        //	}
        //}
        //[Resource]
        //public Task<File> FirstFileAsync
        //{
        //	get { return new AtlantisContext().Tables.FirstOrDefaultAsync((f) => f.XF_CODE == 1); }
        //}

        /// <summary>
        /// Context Galaktika ERP
        /// </summary>
        public partial class AtlantisContext
        {
            private static long _postMainLink = 0;

            /// <summary>
            /// PostMainLink
            /// </summary>
			public static long PostMainLink
            {
                get
                {
                    if (_postMainLink == 0)
                    {
                        using (var ctx = new AtlantisContext())
                        {
                            _postMainLink = (from a in ctx.CATALOGS where a.SYSCODE == -22 select a.NREC).FirstOrDefault();
                        }
                    }
                    return _postMainLink;
                }
            }

            /// <summary>
            /// Post
            /// </summary>
			public virtual IQueryable<Post> Post
            {
                get
                {
                    var lnq = from a in CATALOGS where a.MAINLINK == PostMainLink select new Post { Catalogs = a };
                    return lnq;
                }
            }

            private static long _postMainLinkTest = 0;
            /// <summary>
            /// PostMainLinkTest
            /// </summary>
			public static long PostMainLinkTest
            {
                get
                {
                    if (_postMainLinkTest == 0)
                    {
                        using (var ctx = new AtlantisContext())
                        {
                            _postMainLink = (from a in ctx.CATALOGS where a.SYSCODE == 32767 select a.NREC).FirstOrDefault();
                        }
                    }
                    return _postMainLinkTest;
                }
            }

            /// <summary>
            /// PostTest
            /// </summary>
			public virtual IQueryable<PostTest> PostTest
            {
                get
                {
                    var lnq = from a in CATALOGS where a.MAINLINK == PostMainLinkTest select new PostTest { Catalogs = a };
                    return lnq;
                }
            }

            /// <summary>
            /// Конфигурирование модели
            /// </summary>
            /// <param name="modelBuilder">modelBuilder</param>
			protected override void OnModelCreating(ModelBuilder modelBuilder)
            {

                base.OnModelCreating(modelBuilder);

                var KATMC = modelBuilder.Entity<KATMC>();
                KATMC.HasKey(a => new { a.NREC });
                KATMC
                                    .HasMany(t => t.KATOTPED)
                                    .WithOne(d => d.KATMC)

                                    .HasPrincipalKey(t => t.NREC)
                                    .HasForeignKey(t => t.CMCUSL)
                                    .IsRequired(false)
                ;

                KATMC
                    .HasMany(t => t.SPSOPR)
                    .WithOne(d => d.KATMC)

                    .HasPrincipalKey(t => t.NREC)
                    .HasForeignKey(t => t.CMCUSL)
                    .IsRequired(false)
;

                var KATOTPED = modelBuilder.Entity<KATOTPED>();
                KATOTPED.HasKey(a => new { a.NREC });
                KATOTPED
                    .HasOne(t => t.KATMC)
                    .WithMany(t => t.KATOTPED)
                    .HasForeignKey(t => t.CMCUSL)
                    .HasPrincipalKey(t => t.NREC)
                    .IsRequired(false)
                    ;

                var GRAFIK = modelBuilder.Entity<GRAFIK>();
                GRAFIK.HasKey(a => new { a.NREC });
                GRAFIK
                                    .HasMany(t => t.SPGRAF)
                                    .WithOne(d => d.GRAFIK)

                                    .HasPrincipalKey(t => t.NREC)
                                    .HasForeignKey(t => t.CGRAFIK)
                                    .IsRequired(false)
                ;

                var SPGRAF = modelBuilder.Entity<SPGRAF>();
                SPGRAF.HasKey(a => new { a.NREC });
                SPGRAF
                    .HasOne(t => t.GRAFIK)
                    .WithMany(t => t.SPGRAF)
                    .HasForeignKey(t => t.CGRAFIK)
                    .HasPrincipalKey(t => t.NREC)
                    .IsRequired(false)
                    ;

                var KATSOPR = modelBuilder.Entity<KATSOPR>();
                KATSOPR.HasKey(a => new { a.NREC });
                KATSOPR
                                    .HasMany(t => t.SPSOPR)
                                    .WithOne(d => d.KATSOPR)

                                    .HasPrincipalKey(t => t.NREC)
                                    .HasForeignKey(t => t.CSOPR)
                                    .IsRequired(false)
                ;

                var SPSOPR = modelBuilder.Entity<SPSOPR>();
                SPSOPR.HasKey(a => new { a.NREC });
                SPSOPR
                    .HasOne(t => t.KATSOPR)
                    .WithMany(t => t.SPSOPR)
                    .HasForeignKey(t => t.CSOPR)
                    .HasPrincipalKey(t => t.NREC)
                    .IsRequired(false)
                    ;

                SPSOPR
                    .HasOne(t => t.KATMC)
                    .WithMany(t => t.SPSOPR)
                    .HasForeignKey(t => t.CMCUSL)
                    .HasPrincipalKey(t => t.NREC)
                    .IsRequired(false)
                    ;

                var entPost = modelBuilder.Entity<Post>();
                entPost.HasKey(a => new { a.NREC });

                var entPostTest = modelBuilder.Entity<PostTest>();
                entPostTest.HasKey(a => new { a.NREC });

            }

            /// <summary>
            /// Adds the entity inside ERP
            /// </summary>
            /// <param name="entityType">Type of the entity</param>
            /// <returns>Added object</returns>
			public override object AtlantisAdd(Type entityType)
            {
                if (entityType == typeof(Post))
                {
                    var catalogs = new CATALOGS
                    {
                        MAINLINK = PostMainLink
                    };
                    var entity = new Post() { Catalogs = catalogs };
                    CATALOGS.Add(catalogs);
                    return entity;
                }
                return base.AtlantisAdd(entityType);
            }

            /// <summary>
            /// Finds the entity inside ERP db
            /// </summary>
            /// <param name="type">Type of the entity</param>
            /// <param name="key">Key of the entity</param>
            /// <returns>Finded object (entity)</returns>
			public override object AtlantisFind(Type type, string key)
            {
                if (!long.TryParse(key, out var id))
                {
                    //AtlantisApi.AtlantisContext is used in APPOINTMENTS (Galaktika.ERP.OData\Models\AppSpecificERP\AtlantisApi.cs) 
                    //it is a DbSet <APPOINTMENTS> in GalaktikaApi.Generated.cs and can't have constructor with specific arguments not connected to real table attributes
                    //"Неудалось преобразовать [objectId] = '{0}' к типу LONG. Не верный формат строки.".EsbError(key);
                    throw new EsbException("Неудалось преобразовать [objectId] = '{0}' к типу LONG. Не верный формат строки.", key);
                }

                if (type == typeof(Post))
                {
                    if (!(Find(typeof(CATALOGS), id) is CATALOGS catalogs))
                        return null;

                    var result = new Post { Catalogs = catalogs };
                    return result;
                }

                return base.AtlantisFind(type, key);
            }
        }
    }

    /// <summary>
    /// KATMC
    /// </summary>
	public partial class KATMC
    {
        /// <summary>
        /// KATOTPED
        /// </summary>
		public virtual ICollection<KATOTPED> KATOTPED { get; set; }
        /// <summary>
        /// SPSOPR
        /// </summary>
		public virtual ICollection<SPSOPR> SPSOPR { get; set; }
    }
    /// <summary>
    /// KATOTPED
    /// </summary>
	public partial class KATOTPED
    {
        /// <summary>
        /// KATMC
        /// </summary>
		public virtual KATMC KATMC { get; set; }
    }

    /// <summary>
    /// APPOINTMENTS
    /// </summary>
	public partial class APPOINTMENTS
    {
        /// <summary>
        /// cPodr
        /// </summary>
		public virtual long cPodr
        {
            get
            {
                using (var ctx = new AtlantisApi.AtlantisContext())
                {
                    var lnq = ctx.EXTCATLINKS.Where(k => k.STAFFCAT == this.DEPARTMENT).Select(k => k.EXTCAT).FirstOrDefault();
                    return lnq;
                }
            }
        }
    }
    /// <summary>
    /// GRAFIK
    /// </summary>
	public partial class GRAFIK
    {
        /// <summary>
        /// SPGRAF
        /// </summary>
		public virtual ICollection<SPGRAF> SPGRAF { get; set; }
    }
    /// <summary>
    /// KATSOPR
    /// </summary>
	public partial class KATSOPR
    {
        /// <summary>
        /// SPSOPR
        /// </summary>
		public virtual ICollection<SPSOPR> SPSOPR { get; set; }

    }
    /// <summary>
    /// SPGRAF
    /// </summary>
	public partial class SPGRAF
    {
        /// <summary>
        /// GRAFIK
        /// </summary>
		public virtual GRAFIK GRAFIK { get; set; }
    }

    /// <summary>
    /// SPSOPR
    /// </summary>
	public partial class SPSOPR
    {
        /// <summary>
        /// KATMC
        /// </summary>
		public virtual KATMC KATMC { get; set; }
        /// <summary>
        /// KATSOPR
        /// </summary>
		public virtual KATSOPR KATSOPR { get; set; }
    }
    /// <summary>
    /// ATTRVAL
    /// </summary>
	public partial class ATTRVAL
    {

    }
}
