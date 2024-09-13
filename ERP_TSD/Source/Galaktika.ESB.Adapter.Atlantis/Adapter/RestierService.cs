using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Galaktika.ERP;
using Galaktika.ESB.Core;
using Galaktika.ESB.Core.Exceptions;
using Galaktika.ESB.Core.RestierService;
using Galaktika.ESB.Utils;
using Galaktika.ESB.Utils.LogService;

namespace Galaktika.ESB.Adapter.Atlantis
{
	/// <summary>
	/// RestierService for ERP
	/// </summary>
	public partial class RestierService : RestierServiceCore
	{

		private static readonly Object _instanceLock = new Object();

		/// <summary>
		/// Creates session
		/// </summary>
		/// <returns></returns>
		public override ISession CreateSession()
		{
			try
			{
                LogTraceСreatingSession(Thread.CurrentThread.ManagedThreadId);
				return new AtlantisSession(LogService);
			}
			catch (Exception e)
			{
                LogService.Error(e, "Exception on creating session. Thread={0};", Thread.CurrentThread.ManagedThreadId);
				throw;
			}
		}

		/// <summary>
		/// Application version
		/// </summary>
		/// <returns></returns>
		public override string GetAppVersion()
		{
			return GalnetApi.GalaktikaVersion.ToString();
		}

	    private string _operationTimeoutParameterName = "OperationTimeout";

        /// <summary>
        /// Методы выполняет операцию через OData. В качестве дополнительного параметра 
        /// для длительных операций можно указать 'OperationTimeout', который увеличит таймаут проверки 
        /// адаптера на работоспособность. Этот параметр жизненно необходим для длительно выполняемых операций.
        /// </summary>
        /// <param name="operationName">Имя операции</param>
        /// <param name="parameters">Параметры в виде словаря. Ключом является имя параметра (регистрозависимо).</param>
        /// <returns>Результат выполнения операции</returns>
        public override Task<IQueryable> ExecuteOperation(string operationName, Dictionary<string, object> parameters)
	    {
	        if (parameters.ContainsKey(_operationTimeoutParameterName))
	        {
	            var atlAdapterSrv = this.ParentService.Adapter.GetWorkingService(typeof(AtlantisAdapterService), true);
	            if (atlAdapterSrv != null && atlAdapterSrv is AtlantisAdapterService atlSrv)
	            {
	                if (!TimeSpan.TryParse(parameters[_operationTimeoutParameterName].ToString(),out var res))
                        throw new EsbCallMethodException(
                            $"Не удалось преобразовать параметр таймаута длительности операции в тип TimeSpan. Значение вашего параметра = {parameters[_operationTimeoutParameterName]}");
                    _Info_Set_operation_timeout_0(res);
                    atlSrv.SetLongOperationTimeout(res);
	                var result = base.ExecuteOperation(operationName, parameters);
                    atlSrv.ResetLongOperationTimeout();
	                return result;
	            }
	            _Warn_Не_удалось_получить_атлантис_сервис_таймаут_на_длительную_операцию_не_будет_установлен_для_данной_функции();
	        }
	        return base.ExecuteOperation(operationName, parameters);

        }

	    //private readonly Dictionary<string, object> _restCache = new Dictionary<string, object>();

		/// <summary>
		/// Executes action in session
		/// </summary>
		/// <param name="action"></param>
		public override void ExecuteInSession(Action<ISession> action)
		{
            lock (_instanceLock)
            {
                using (var session = new AtlantisSession(LogService))
                {
                    session.RunMainAction(action);
                }
            }
		}

        public override void SaveChanges(ISession session)
        {
            lock (_instanceLock)
            {
                base.SaveChanges(session);
            }
        }

        public virtual void Save(ISession session, object obj)
        {
            lock (_instanceLock)
            {
                base.Save(session, obj);
            }
        }
    }
}