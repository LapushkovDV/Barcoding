using System;
using Galaktika.ERP.Interop;
using Galaktika.ESB.Utils.LogService;

namespace Galaktika.ESB.Adapter.Atlantis
{
	/// <summary>
	/// ErpSession
	/// </summary>
	public class AtlantisSession : Core.Session, ILogger
	{
        /// <summary>
        /// Service for logging
        /// </summary>
        public ILogService LogService { get; private set; }
		/// <summary>
		/// ctor
		/// </summary>
		public AtlantisSession(ILogService logService) : base()
		{
            LogService = logService ?? DefaultLogService.Instance;
		}

		private static readonly Object lockObj = new Object();

		/// <summary>
		/// Invokes action
		/// </summary>
		/// <param name="action"></param>
		public override void Invoke(Action action)
		{
			DispatcherHelper.Invoke(action);
		}

		/// <summary>
		/// Invokes action
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="action"></param>
		/// <returns></returns>
		public override TResult Invoke<TResult>(Func<TResult> action)
		{
			TResult result = default(TResult);
			DispatcherHelper.Invoke(new Action(() => result = action()));
			return result;
		}

		/// <summary>
		/// Runs action
		/// </summary>
		/// <param name="action"></param>
		public override void RunMainAction(Action<Core.Session> action)
		{
			DispatcherHelper.Invoke(new System.Action(() =>
			{
				try
				{
                    lock (lockObj)
                    {
                        action(this);
                    }
				}
				catch (Exception e)
				{
					LogService.Error(e);
					throw;
				}
			}));
		}

        /// <summary>
        /// Starts transaction
        /// </summary>
        public override void StartTransaction()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Commits transaction
		/// </summary>
		public override void CommitTransaction()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Rollbacks transaction
		/// </summary>
		public override void RollbackTransaction()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Dispose
		/// </summary>
		public override void Dispose()
		{
			EsbContext?.Dispose();
			//			context?.Dispose();
			EsbContext = null;
			//			context = null;

			base.Dispose();
		}
	}
}
