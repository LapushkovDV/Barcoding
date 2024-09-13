using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Galaktika.ERP.Adapter.Atlantis;
using Galaktika.ESB.Adapter.Atlantis.Api;
using Galaktika.ESB.Adapter.ERP.AppSpecificERP;
using Microsoft.EntityFrameworkCore;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Submit;
using Galaktika.ESB.Core.Exceptions;

namespace Galaktika.ESB.Adapter.ERP
{
    /// <summary>
    /// ErpChangeSetInitializer
    /// </summary>
	public partial class ErpChangeSetInitializer : AtlantisChangeSetInitializer
	{

		private static MethodInfo prepareEntryGeneric = typeof(AtlantisChangeSetInitializer)
				.GetMethod("PrepareEntry", BindingFlags.Static | BindingFlags.NonPublic);

		private static MethodInfo prepareEnumerableEntryGeneric = typeof(ErpChangeSetInitializer)
			.GetMethod("PrepareEnumerableEntry", BindingFlags.Static | BindingFlags.NonPublic);

		/// <summary>
		/// Asynchronously prepare the <see cref="ChangeSet"/>.
		/// </summary>
		/// <param name="context">The submit context class used for preparation.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The task object that represents this asynchronous operation.</returns>
		public override async Task InitializeAsync(
				SubmitContext context,
				CancellationToken cancellationToken)
		{
			Context = context.GetApiService<DbContext>();

			foreach (var entry in context.ChangeSet.Entries.OfType<DataModificationItem>())
			{
				object strongTypedDbSet = Context.GetType().GetProperty(entry.ResourceSetName).GetValue(Context);
				Type entityType = strongTypedDbSet.GetType().GetGenericArguments()[0];

				// This means request resource is sub type of resource type
				if (entry.ActualResourceType != null && entityType != entry.ActualResourceType)
				{
					entityType = entry.ActualResourceType;
				}
				Task task;

				if (strongTypedDbSet.GetType().Name == "EntityQueryable`1")
				{
					MethodInfo prepareEntryMethod = prepareEnumerableEntryGeneric.MakeGenericMethod(entityType);
					task = (Task)prepareEntryMethod.Invoke(
						obj: null,
						parameters: new[] { context, Context, entry, strongTypedDbSet, cancellationToken });
				}
				else
				{
					MethodInfo prepareEntryMethod = prepareEntryGeneric.MakeGenericMethod(entityType);
					task = (Task)prepareEntryMethod.Invoke(
						obj: null,
						parameters: new[] { context, Context, entry, strongTypedDbSet, cancellationToken });
				}
				await task;
			}
		}

		private static async Task PrepareEnumerableEntry<TEntity>(
				SubmitContext context,
				DbContext dbContext,
				DataModificationItem entry,
				IEnumerable<TEntity> set,
				CancellationToken cancellationToken) where TEntity : class
		{
			Type entityType = typeof(TEntity);
			TEntity entity;

			var atlContext = dbContext as AtlantisApi.AtlantisContext;
			if (atlContext == null)
				throw new EsbException("AtlantisChangeSetInitializer.cs - dbContext as AtlantisApi.AtlantisContext == null");

			if (entry.DataModificationItemAction == DataModificationItemAction.Insert)
			{
				// TODO: See if Create method is in DbSet<> in future EF7 releases, as the one EF6 has.
				entity = (TEntity)Activator.CreateInstance(typeof(TEntity));

				if (entityType == typeof(Post))
				{
					var catalogs = (CATALOGS)SetValues<CATALOGS>(entry);
					catalogs.MAINLINK = AtlantisApi.AtlantisContext.PostMainLink;
					var props = entity.GetType().GetProperties();
					var catalogsProperty = props.SingleOrDefault(p => p.Name == "Catalogs");
					if (catalogsProperty != null)
						catalogsProperty.SetValue(entity, catalogs);
					atlContext.CATALOGS.Add(catalogs);
				}
			}
			else if (entry.DataModificationItemAction == DataModificationItemAction.Remove)
			{
				entity = (TEntity)await ErpChangeSetInitializer.FindEntity(context, entry, cancellationToken);
				if (entityType == typeof(Post))
				{
					object value;
					if (entry.ResourceKey.TryGetValue("NREC", out value))
					{
						var cat = atlContext.CATALOGS.FirstOrDefault(c => c.NREC.Equals(value));
						if (cat == null)
							throw new EsbException("CATALOGS with NREC == " + value + " not found");
						atlContext.CATALOGS.Remove(cat);
					}
				}
			}
			else if (entry.DataModificationItemAction == DataModificationItemAction.Update && !entry.IsFullReplaceUpdateRequest)
			{
				entry.IsFullReplaceUpdateRequest = false;
				entity = (TEntity)await ErpChangeSetInitializer.FindEntity(context, entry, cancellationToken);
				var dbEntry = dbContext.Attach(entity);
				ErpChangeSetInitializer.SetValues(dbEntry, entry);
			}
			else
				throw new NotSupportedException();

			entry.Resource = entity;
		}
	}
}
