using System;
using System.Linq;
using Galaktika.ESB.Storage.Adapter;
using Galaktika.ESB.Storage.Core;
using Galaktika.ESB.Utils.LogService;

namespace Galaktika.ESB.Adapter.ERP.Adapter
{
    /// <summary>
    /// Class to work with wlistincrementor
    /// </summary>
    public class WlistIncrementor : ILogger
    {
        private readonly object _lockObj = new object();

        //this class call in AtlantisBaseContext <- DictionaryContext <- AtlantisContext <- DictionaryApi.DictionaryContext <- AtlantisApi.AtlantisContext. (BaseClass <- ChildClass) 
        //The last is used in APPOINTMENTS (Galaktika.ERP.OData\Models\AppSpecificERP\AtlantisApi.cs) 
        //it is a DbSet<APPOINTMENTS> in GalaktikaApi.Generated.cs and can't have constructor with specific arguments not connected to real table attributes
        // TODO LogService insert real logService
        /// <summary>
        /// LogService instance
        /// </summary>
        public ILogService LogService => DefaultLogService.Instance;

        /// <summary>
        /// Reads wlist value from file
        /// </summary>
        /// <returns>Wlist value</returns>
        public int ReadLastCheckedWlist()
        {
            int result = 1;
            try
            {
                lock (_lockObj)
                {
                    using (var ctx = new AdapterDbContext(LogService))
                    {
                        var param = ctx.Parameters.FirstOrDefault(p => p.ParamName == "Wlist");
                        if (param != null)
                        {
                            result = ushort.Parse(param.Value) + 1;
                            if (result > ushort.MaxValue)
                                result = 1;
                            param.Value = result.ToString();
                        }
                        else
                            ctx.Parameters.Add(new Parameter("Wlist", result.ToString()));

                        ctx.SaveChanges();
                    }
                }
                
            }
            catch (Exception e)
            {
                LogService.Error(e);
            }
            return result;
        }
    }
}
