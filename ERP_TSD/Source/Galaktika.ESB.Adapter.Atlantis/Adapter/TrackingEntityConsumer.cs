using System;
using Galaktika.ESB.Core;
using Galaktika.ESB.Core.Contract.DbUpdates;
using MassTransit;
using System.Threading.Tasks;
using Galaktika.ERP.Interop;
using Galaktika.ESB.Adapter.Atlantis.Api;
using Galaktika.ESB.Adapter.Services;
using Galaktika.ESB.Core.Contract;
using Galaktika.ESB.Core.Exceptions;

namespace Galaktika.ESB.Adapter.ERP.Adapter
{
	/// <summary>
	/// Консьюмер для переотправки отслеженных записей
	/// </summary>
	public class EntityForTrackingConsumer : EsbConsumer, IConsumer<EntityForTracking>
	{
		/// <inheritdoc/>
		public Task Consume(ConsumeContext<EntityForTracking> context)
		{
			try
			{
				var publisher = ServiceAdapter.GetService<IPublisherService>();
				if (publisher != null)
				{
					AtlMeta atlMeta;
					using (var ctx = new DictionaryApi.DictionaryContext())
					{
						atlMeta = ctx.GetMeta;
					}

					var type = atlMeta.GetTypeByTableCode(context.Message.TableCode);
					return publisher.PublishMainEntities(type, context.Message.Changes, null, PubContext.DbTracking);
				}
				else
				{
					LogService.Warn("Publisher not found!");
                    return Task.CompletedTask;
				}
			}
			catch (Exception e)
			{
				//если исключение во время деактивации, то ничего не делаем а просто пишем в лог
				if (e.InnerException is EsbModelDeactivatedException)
                {
                    LogService.Warn(e.InnerException.Message);
                    return Task.CompletedTask;
                }
				else
					throw;
			}

		}
	}
}
