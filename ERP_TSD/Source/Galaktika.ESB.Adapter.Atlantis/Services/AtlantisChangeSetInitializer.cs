using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.OData;
using Galaktika.ESB.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.OData.Edm;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Query;
using Microsoft.Restier.Core.Submit;
using Galaktika.ESB.ServiceAdapterApi.ModelBuilder;

namespace Galaktika.ERP.Adapter.Atlantis
{
    /// <summary>
    /// AtlantisChangeSetInitializer
    /// </summary>
	public class AtlantisChangeSetInitializer : IChangeSetInitializer
	{
        /// <summary>
        /// Database context
        /// </summary>
		public static DbContext Context { get; set; }

		private static readonly MethodInfo PrepareEntryGeneric = typeof(AtlantisChangeSetInitializer)
				.GetMethod("PrepareEntry", BindingFlags.Static | BindingFlags.NonPublic);

		/// <inheritdoc />
		/// <summary>
		/// Asynchronously prepare the <see cref="T:Microsoft.Restier.Core.Submit.ChangeSet" />.
		/// </summary>
		/// <param name="context">The submit context class used for preparation.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The task object that represents this asynchronous operation.</returns>
		public virtual async Task InitializeAsync(
				SubmitContext context,
				CancellationToken cancellationToken)
		{
			Context = context.GetApiService<DbContext>();

			foreach (var entry in context.ChangeSet.Entries.OfType<DataModificationItem>())
			{
				var propertyInfo = Context.GetType().GetProperty(entry.ResourceSetName);
				if (propertyInfo == null) continue;

				var strongTypedDbSet = propertyInfo.GetValue(Context);
				if (strongTypedDbSet == null) continue;

				var entityType = strongTypedDbSet.GetType().GetGenericArguments()[0];

				// This means request resource is sub type of resource type
				if (entry.ActualResourceType != null && entityType != entry.ActualResourceType)
				{
					entityType = entry.ActualResourceType;
				}
				var prepareEntryMethod = PrepareEntryGeneric.MakeGenericMethod(entityType);
				var task = (Task)prepareEntryMethod.Invoke(
					obj: null,
					parameters: new[] { context, Context, entry, strongTypedDbSet, cancellationToken });
				await task;
			}
		}

        /// <summary>
        /// PrepareEntry
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity</typeparam>
        /// <param name="context">Submit context</param>
        /// <param name="dbContext">Database context</param>
        /// <param name="entry">Entry item</param>
        /// <param name="set">DB set instance</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
		protected static async Task PrepareEntry<TEntity>(
				SubmitContext context,
				DbContext dbContext,
				DataModificationItem entry,
				DbSet<TEntity> set,
				CancellationToken cancellationToken) where TEntity : class
		{
			Type entityType = typeof(TEntity);
			TEntity entity;

			if (entry.DataModificationItemAction == DataModificationItemAction.Insert)
			{
				// TODO: See if Create method is in DbSet<> in future EF7 releases, as the one EF6 has.
				entity = (TEntity)Activator.CreateInstance(typeof(TEntity));

				SetValues(entity, entityType, entry.LocalValues);
				set.Add(entity);
			}
			else if (entry.DataModificationItemAction == DataModificationItemAction.Remove)
			{
				entity = (TEntity)await FindEntity(context, entry, cancellationToken);
				set.Remove(entity);
			}
			else if (entry.DataModificationItemAction == DataModificationItemAction.Update && !entry.IsFullReplaceUpdateRequest)
			{
				entry.IsFullReplaceUpdateRequest = false;
				entity = (TEntity)await FindEntity(context, entry, cancellationToken);
				var dbEntry = dbContext.Attach(entity);
				SetValues(dbEntry, entry);
			}
			else
				throw new NotSupportedException();

			entry.Resource = entity;
		}

        /// <summary>
        /// Setting values to specified entry
        /// </summary>
        /// <typeparam name="T">Type of the entry</typeparam>
        /// <param name="entry">DataModificationItem</param>
        /// <returns>Entry with values</returns>
		protected static object SetValues<T>(DataModificationItem entry)
		{
			var ent = (T)Activator.CreateInstance(typeof(T));
			foreach (var val in entry.LocalValues)
			{
				var key = val.Key;
				var props = ent.GetType().GetProperties();
				var property = props.SingleOrDefault(p => p.Name.ToUpper() == key.ToUpper());
				if (property != null)
					property.SetValue(ent, val.Value);
			}
			return ent;
		}

        /// <summary>
        /// Find entity
        /// </summary>
        /// <param name="context">Submit context</param>
        /// <param name="entry">DataModificationItem</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns></returns>
		protected static async Task<object> FindEntity(
				SubmitContext context,
				DataModificationItem entry,
				CancellationToken cancellationToken)
		{
			// #warning Err ApiContext

			//IQueryable query = context.ApiContext.GetQueryableSource(entry.ResourceSetName);
			IQueryable query = context.GetApiService<ApiBase>().GetQueryableSource(entry.ResourceSetName);
			query = entry.ApplyTo(query);

			//QueryResult result = await context.ApiContext.QueryAsync(new QueryRequest(query), cancellationToken);
			QueryResult result = await context.GetApiService<ApiBase>().QueryAsync(new QueryRequest(query), cancellationToken);

			object entity = result.Results.SingleOrDefault();
			if (entity == null)
			{
				// TODO GitHubIssue#38 : Handle the case when entity is resolved
				// there are 2 cases where the entity is not found:
				// 1) it doesn't exist
				// 2) concurrency checks have failed
				// we should account for both - I can see 3 options:
				// a. always return "PreConditionFailed" result
				//  - this is the canonical behavior of WebAPI OData, see the following post:
				//    "Getting started with ASP.NET Web API 2.2 for OData v4.0" on http://blogs.msdn.com/b/webdev/.
				//  - this makes sense because if someone deleted the record, then you still have a concurrency error
				// b. possibly doing a 2nd query with just the keys to see if the record still exists
				// c. only query with the keys, and then set the DbEntityEntry's OriginalValues to the ETag values,
				//    letting the save fail if there are concurrency errors

				////throw new EntityNotFoundException
				throw new InvalidOperationException();
			}

			return entity;
		}

        /// <summary>
        /// Set values to an entry
        /// </summary>
        /// <param name="dbEntry">EntityEntry</param>
        /// <param name="entry">DataModificationItem</param>
		protected static void SetValues(EntityEntry dbEntry, DataModificationItem entry)
		{
			foreach (KeyValuePair<string, object> propertyPair in entry.LocalValues)
			{
				if (propertyPair.Value is EdmEntityObjectCollection)
				{
					throw new NotSupportedException();
					//var property = dbEntry.Collection(propertyPair.Key);
					//UpdateCollectionNavigation(edmEntityObjectCollection, property);
					//continue;
				}
				if (propertyPair.Value is EdmEntityObject)
				{
					throw new NotSupportedException();
					//var property = dbEntry.Reference(propertyPair.Key);
					//UpdateReferenceNavigation(edmEntityObject, property);
					//continue;
				}

				PropertyEntry propertyEntry = dbEntry.Property(propertyPair.Key);
				object value = propertyPair.Value;
				if (value == null)
				{
					// If the property value is null, we set null in the entry too.
					propertyEntry.CurrentValue = null;
					continue;
				}

				Type type = TypeHelper.GetUnderlyingTypeOrSelf(propertyEntry.Metadata.ClrType);
				value = ConvertToEfValue(type, value);
				if (value != null && !type.IsInstanceOfType(value))
				{
					var dic = value as IReadOnlyDictionary<string, object>;
					if (dic==null)
					{
						throw new NotSupportedException(string.Format(
								CultureInfo.InvariantCulture,
								//Resources.UnsupportedPropertyType,
								propertyPair.Key));
					}

					value = Activator.CreateInstance(type);
					SetValues(value, type, dic);
				}

				propertyEntry.CurrentValue = value;
			}
		}

        /// <summary>
        /// Create instance and set values
        /// </summary>
        /// <param name="edmEntityObject">EdmEntityObject</param>
        /// <param name="innerType">Type of the objext</param>
        /// <returns>Created instance</returns>
		protected static object CreateInstanceAndSetValues(EdmEntityObject edmEntityObject, Type innerType)
		{
			var dict = new Dictionary<string, object>();

			var entType = Context.Model.FindEntityType(innerType);
			foreach (var pr in entType.GetProperties())
			{
				object v; 
				if (edmEntityObject.TryGetPropertyValue(pr.Name, out v))
					dict.Add(pr.Name, v);
			}
			var instance = Activator.CreateInstance(innerType);
			SetValues(instance, innerType, dict);
			return instance;
		}

		//protected static void UpdateReferenceNavigation(EdmEntityObject edmEntityObject, ReferenceEntry property)
		//{
		//	property.Load();
		//	var innerType = Type.GetType(edmEntityObject.ActualEdmType.ToString());
		//	if (innerType == null)
		//		throw new EsbException(string.Format("Can't find metadata of type {0}.", innerType));

		//	var currValue = property.CurrentValue;
		//	if (currValue == null)
		//	{
		//		var instance = CreateInstanceAndSetValues(edmEntityObject, innerType);
		//		property.CurrentValue = instance;
		//	}
		//	else
		//	{
		//		var properties = currValue.GetType().GetProperties();
		//		var keyProperties = innerType.GetProperties().Where(p => p.CustomAttributes.SingleOrDefault(a => a.AttributeType == typeof(KeyAttribute)) != null);
		//		foreach (var propertyInfo in keyProperties)
		//		{
		//			var includedKeysInUpdateData = edmEntityObject.GetChangedPropertyNames().Where(p => p == propertyInfo.Name).ToList();
		//			object propVal;
		//			if (includedKeysInUpdateData.Count > 0)
		//			{
		//				var keyProp = properties.SingleOrDefault(p => includedKeysInUpdateData.Find(s => s == p.Name) != null);
		//				if (keyProp != null && edmEntityObject.TryGetPropertyValue(keyProp.Name, out propVal))
		//				{
		//					if (keyProp.GetValue(currValue).Equals(propVal))
		//					{
		//						foreach (var info in properties)
		//						{
		//							var pName = edmEntityObject.GetChangedPropertyNames().SingleOrDefault(p => p == info.Name);
		//							if (pName != null && edmEntityObject.TryGetPropertyValue(info.Name, out propVal) && keyProp.Name != info.Name)
		//								info.SetValue(currValue, propVal);
		//						}
		//					}
		//					else
		//					{
		//						throw new EsbException("Wrong value of key property in update data");
		//					}
		//				}
		//			}
		//			else
		//			{
		//				foreach (var info in properties)
		//				{
		//					var pName = edmEntityObject.GetChangedPropertyNames().SingleOrDefault(p => p == info.Name);
		//					if (pName != null && edmEntityObject.TryGetPropertyValue(info.Name, out propVal))
		//						info.SetValue(currValue, propVal);
		//				}
		//			}
		//		}

		//	}
		//}

		//protected static void UpdateCollectionNavigation(EdmEntityObjectCollection values, CollectionEntry property)
		//{
		//	property.Load();
		//	var currValues = property.CurrentValue;
		//	foreach (var item in values)
		//	{
		//		var edmEntityObject = item as EdmEntityObject;
		//		if (edmEntityObject==null)
		//			throw new EsbException(string.Format("Not supported value. Must be EdmEntityObject."));

		//		var innerType = Type.GetType(edmEntityObject.ActualEdmType.ToString());
		//		if (innerType == null)
		//			throw new EsbException(string.Format("Can't find metadata of type {0}.", innerType));
		//		var keyProperties = innerType.GetProperties().Where(p => p.CustomAttributes.SingleOrDefault(a => a.AttributeType == typeof(KeyAttribute)) != null);
		//		foreach (var propertyInfo in keyProperties)
		//		{
		//			var includedKeysInUpdateData = edmEntityObject.GetChangedPropertyNames().Where(p => p == propertyInfo.Name).ToList();

		//			if (includedKeysInUpdateData.Count == 0)
		//			{
		//				var collectionToSet = GetSpecificGenericList(innerType);

		//				foreach (var val in currValues)
		//				{
		//					collectionToSet.Add(val);
		//				}
		//				var instance = CreateInstanceAndSetValues(edmEntityObject, innerType);
		//				collectionToSet.Add(instance);
		//				property.CurrentValue = collectionToSet;
		//			}
		//			else
		//			{
		//				foreach (var objectCurrentValue in currValues)
		//				{
		//					object propVal;
		//					var properties = objectCurrentValue.GetType().GetProperties();
		//					var keyProp = properties.SingleOrDefault(p => includedKeysInUpdateData.Find(s => s == p.Name) != null);
		//					if (keyProp != null && item.TryGetPropertyValue(keyProp.Name, out propVal))
		//					{
		//						if (keyProp.GetValue(objectCurrentValue).Equals(propVal))
		//						{
		//							foreach (var info in properties)
		//							{
		//								var pName = edmEntityObject.GetChangedPropertyNames().SingleOrDefault(p => p == info.Name);
		//								if (pName != null && item.TryGetPropertyValue(info.Name, out propVal) && keyProp.Name != info.Name)
		//									info.SetValue(objectCurrentValue, propVal);
		//							}
		//						}
		//					}
		//				}
		//			}
		//		}
		//	}
		//}

        /// <summary>
        /// Setting values to the entity
        /// </summary>
        /// <param name="instance">Instance of an object</param>
        /// <param name="instanceType">Type of the instance</param>
        /// <param name="values">Values to set</param>
		protected static void SetValues(object instance, Type instanceType, IReadOnlyDictionary<string, object> values)
		{
			foreach (KeyValuePair<string, object> propertyPair in values)
			{
				object value = propertyPair.Value;
				PropertyInfo propertyInfo = instanceType.GetProperty(propertyPair.Key);
				if (value == null)
				{
					// If the property value is null, we set null in the object too.
					propertyInfo.SetValue(instance, null);
					continue;
				}

				//if generic arguments.count > 1 throw?
				Type type = TypeHelper.GetUnderlyingTypeOrSelf(propertyInfo.PropertyType);
				Type innerType = type;

				value = ConvertToEfValue(type, value);
				if (value != null && !type.IsInstanceOfType(value))
				{
					if (type.IsConstructedGenericType)
					{
						innerType = type.GenericTypeArguments[0];
					}

					var entType = Context.Model.FindEntityType(innerType.FullName);

					var dict = new Dictionary<string, object>();
					var dic = value as IReadOnlyDictionary<string, object>;
					if (dic == null)
					{
						var edm = value as EdmEntityObjectCollection;
						if (edm!=null)
						{
							var collectionToSet = GetSpecificGenericList(innerType);
							var lst = GetCollectionOfNavigations(edm, entType, innerType);
							foreach (var item in lst)
							{
								collectionToSet.Add(item);
							}
							value = collectionToSet;
						}
						else
						{
							var edmEntityObject = value as EdmEntityObject;
							if (edmEntityObject==null)
								throw new EsbException("Not supported value. Must be EdmEntityObject.");
							foreach (var property in entType.GetProperties())
							{
								object v;
								if (edmEntityObject.TryGetPropertyValue(property.Name, out v) && edmEntityObject.GetChangedPropertyNames().SingleOrDefault(p => p == property.Name) != null)
									dict.Add(property.Name, v);
							}
							value = Activator.CreateInstance(type);
							SetValues(value, type, dict);
						}
					}

					if (dic == null && value == null)
					{
						throw new NotSupportedException(string.Format(
								CultureInfo.InvariantCulture,
								//Resources.UnsupportedPropertyType,
								propertyPair.Key));
					}

				}
				propertyInfo.SetValue(instance, value);
			}
		}

        /// <summary>
        /// Gets specific generic list
        /// </summary>
        /// <param name="innerType">Generic type</param>
        /// <returns>Specific generic list</returns>
		protected static IList GetSpecificGenericList(Type innerType)
		{
			Type generic = typeof(List<>);
			Type specific = generic.MakeGenericType(innerType);
			ConstructorInfo ci = specific.GetConstructor(Type.EmptyTypes);

			if (ci == null)
				throw new EsbException(string.Format("The constructor info of generic collection is null. Collection of {0}", innerType));


			var collection = ci.Invoke(new object[] { });
			var collectionToSet = collection as IList;
			if (collectionToSet==null)
				throw new EsbException(string.Format("Constructor is not a IList. Needed collection of {0}", innerType));

			return collectionToSet;
		}

        /// <summary>
        /// Gets collection of navigations
        /// </summary>
        /// <param name="edmColl">Entity data model collection</param>
        /// <param name="et">Entity type</param>
        /// <param name="instanceType">Type of the instace</param>
        /// <returns>List of navigations</returns>
		protected static List<object> GetCollectionOfNavigations(EdmEntityObjectCollection edmColl, IEntityType et, Type instanceType)
		{
			var result = new List<object>();

			foreach (var t in edmColl)
			{
				var dict = new Dictionary<string, object>();
				var instance = Activator.CreateInstance(instanceType);
				foreach (var property in et.GetProperties())
				{
					object v;
					if (t.TryGetPropertyValue(property.Name, out v))
					{
						dict.Add(property.Name, v);
					}
				}
				SetValues(instance, instanceType, dict);
				result.Add(instance);
			}
			return result;
		}

        /// <summary>
        /// Convert value for entity framework
        /// </summary>
        /// <param name="type">Type of the value</param>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
		protected static object ConvertToEfValue(Type type, object value)
		{
			// string[EdmType = Enum] => System.Enum
			if (TypeHelper.IsEnum(type))
			{
				return Enum.Parse(TypeHelper.GetUnderlyingTypeOrSelf(type), (string)value);
			}

			// Edm.Date => System.DateTime[SqlType = Date]
			
			if (value is Date)
			{
				var dateValue = (Date)value;
				return (DateTime)dateValue;
			}

			// System.DateTimeOffset => System.DateTime[SqlType = DateTime or DateTime2]
			if (value is DateTimeOffset  && TypeHelper.IsDateTime(type))
			{
				var dateTimeOffsetValue = (DateTimeOffset) value;
				return dateTimeOffsetValue.DateTime;
			}

			// Edm.TimeOfDay => System.TimeSpan[SqlType = Time]
			if (value is TimeOfDay && TypeHelper.IsTimeSpan(type))
			{
				var timeOfDayValue = (TimeOfDay) value;
				return (TimeSpan)timeOfDayValue;
			}

			// In case key is long type, when put an entity, key value will be from key parsing which is type of int
			if (value is int && type == typeof(long))
			{
				return Convert.ToInt64(value, CultureInfo.InvariantCulture);
			}

			return value;
		}
	}

    /// <summary>
    /// EnumerableExtensions
    /// </summary>
	public static class EnumerableExtensions
	{
		static string QueryShouldGetSingleRecord
		{
			get { return "A query for a single entity resulted in more than one record."; }
		}

        /// <summary>
        /// SingleOrDefault extension
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns></returns>
		public static object SingleOrDefault(this IEnumerable enumerable)
		{
			IEnumerator enumerator = enumerable.GetEnumerator();
			object result = enumerator.MoveNext() ? enumerator.Current : null;

			if (enumerator.MoveNext())
			{
				throw new InvalidOperationException(QueryShouldGetSingleRecord);
			}

			return result;
		}

	}
}

