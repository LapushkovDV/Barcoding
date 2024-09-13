using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using Galaktika.ERP.Interop;
using Galaktika.ESB.Core.Context;
using Galaktika.ESB.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Galaktika.ESB.Adapter.Atlantis.Api
{
    /// <summary>
    /// DbContext для работы с Atlantis
    /// </summary>
	public class AtlantisBaseContext : DictionaryContext, IEsbContext
	{
	    //TODO: Разобраться с наследованием контекстов. Например, XafContext наследник от EsbContext и ORM доступна через переменную ObjectSpace. Здесь почему-то ORM доступна через наследование.
	    //TODO: Очень странное наследование DictionaryContext -> AtlantisBaseContext -> DictionaryContext

	    private static readonly ConcurrentDictionary<Type, PropertyInfo> _keyProperties = new ConcurrentDictionary<Type, PropertyInfo>();
        private static readonly ConcurrentDictionary<Type, string> _tableNamesCache = new ConcurrentDictionary<Type, string>();
        private static readonly ConcurrentDictionary<Type, Func<object>> _constructorsCashe = new ConcurrentDictionary<Type, Func<object>>();
        private static readonly ConcurrentDictionary<Type, MethodInfo> _containsMethods = new ConcurrentDictionary<Type, MethodInfo>();
        private static readonly ConcurrentDictionary<Type, MethodInfo> _castMethods = new ConcurrentDictionary<Type, MethodInfo>();
        private static readonly ConcurrentDictionary<Type, MethodInfo> _toListMethods = new ConcurrentDictionary<Type, MethodInfo>();

        /// <summary>
        /// Конструктор
        /// </summary>
	    public AtlantisBaseContext()
        {
            _hashId = GetHashCode();
        }

        private readonly int _hashId;

        /// <summary>
        /// Возвращение хэша контекста
        /// </summary>
        /// <returns>Хэш</returns>
	    public int GetHashId()
        {
            return _hashId;
        }

        /// <summary>
        /// Переопределение ToString
        /// </summary>
        /// <returns>Строковое представление контекста</returns>
	    public override string ToString()
        {
            return base.ToString() + "(" + GetHashId() + ")";
        }

        /// <summary>
        /// Добавление сущности в контекст
        /// </summary>
        /// <param name="entityType">Тип сущности</param>
        /// <returns>Созданная сущность</returns>
        public object AddEntity(Type entityType)
		{
			return AtlantisAdd(entityType);
		}

        /// <summary>
        /// Сохранение изменений внутри контекста
        /// </summary>
		void IEsbContext.SaveChanges()
		{
			SaveChanges();
		}

        /// <summary>
        /// Добавление сущности в контекст, если ее там еще нет
        /// </summary>
        /// <param name="appEntity">Сущность</param>
	    public void SaveEntity(object appEntity)
	    {
            if (appEntity == null) return;

            DsqlHelper.LastContextActivity = DateTime.Now;
            EntityEntry entry = Entry(appEntity);

            if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
            {
                Update(appEntity);
            }
	    }

	    /// <summary>
        /// Удаление сущности
        /// </summary>
        /// <param name="appEntity">Сущность</param>
		public void RemoveEntity(object appEntity)
		{
			Remove(appEntity);
		}

        /// <summary>
        /// Поиск сущности по ключу
        /// </summary>
        /// <param name="type">Тип сущности</param>
        /// <param name="key">Ключ сущности</param>
        /// <returns>Сущность</returns>
		public object FindEntity(Type type, string key)
		{
			return AtlantisFind(type, key);
		}

        /// <summary>
        /// Получение ключа сущности
        /// </summary>
        /// <param name="typeAppEntity">Тип сущности</param>
        /// <param name="entity">Сущность</param>
        /// <returns>Ключ сущности</returns>
		public string GetKey(Type typeAppEntity, object entity)
        {
            if (entity == null)
                return string.Empty;

            var type = typeAppEntity == typeof(object) ? entity.GetType() : typeAppEntity;
            var kp =_keyProperties.GetOrAdd(type, GetKeyProperty);
		    return kp.GetValue(entity).ToString();
		}

        /// <summary>
        /// Сохранение изменений в контексте
        /// </summary>
        /// <returns>Количество изменений, которые улетели в БД</returns>
		public override int SaveChanges()
		{
            return base.SaveChanges();
		}

		private static void Add<T>(DbSet<T> dbSet, T obj) where T : class
		{
			dbSet.Add(obj);
		}

		private static MethodInfo miDbsetAdd = typeof(AtlantisBaseContext).GetMethod("Add", BindingFlags.Static | BindingFlags.NonPublic);

	    /// <summary>
        /// Создание сущности и добавление ее в контекст
        /// </summary>
        /// <param name="entityType">Тип сущности</param>
        /// <returns>Созданная сущность</returns>
        public virtual object AtlantisAdd(Type entityType)
        {
            var entity = _constructorsCashe.GetOrAdd(entityType, CreateEntityInternal)();
            Add(entity);
            return entity;
        }
        
        private Func<object> CreateEntityInternal(Type type)
        {
            return Expression.Lambda<Func<object>>(
                   Expression.New(
                       type.GetConstructor(Type.EmptyTypes)
                   )
               )
               .Compile();
        }


        /// <summary>
        /// Поиск сущности по ключу
        /// </summary>
        /// <param name="type">Тип сущности</param>
        /// <param name="key">Ключ</param>
        /// <returns>Найденная сущность</returns>
        public virtual object AtlantisFind(Type type, string key)
		{
			if (!long.TryParse(key, out var id))
			{
                //AtlantisContext <- DictionaryApi.DictionaryContext <- AtlantisApi.AtlantisContext
                //last is used in APPOINTMENTS (Galaktika.ERP.OData\Models\AppSpecificERP\AtlantisApi.cs) 
                //it is a DbSet<APPOINTMENTS> in GalaktikaApi.Generated.cs and can't have constructor with specific arguments not connected to real table attributes
                //"Неудалось преобразовать [objectId] = '{0}' к типу LONG. Не верный формат строки. Тип объекта = '{1}'".EsbError(key, type.FullName);

                throw new EsbException("Неудалось преобразовать [objectId] = '{0}' к типу LONG. Не верный формат строки. Тип объекта = '{1}'", key, type.FullName);
			}

			return Find(type, id);
		}

        /// <summary>
        /// Получение списка сущностей в соответствии с предикатом
        /// </summary>
        /// <typeparam name="TAppEntity">Тип сущности</typeparam>
        /// <param name="condition">Предикат</param>
        /// <returns>Набор сущностей, удовлетворяющих предикату</returns>
        public IQueryable<TAppEntity> GetAppEntities<TAppEntity>(string condition = "") where TAppEntity : class
        {
            if (string.IsNullOrEmpty(condition))
                return Set<TAppEntity>();

            var tableName = GetTableName(typeof(TAppEntity));
            var query = $"Select * From {tableName} {condition} ";
            RawSqlString rawSql = new RawSqlString(query);
            return Set<TAppEntity>().FromSql(rawSql).AsQueryable<TAppEntity>();
        }

        /// <summary>
        /// Получение набора сущностей в соответствии со списком ключей
        /// </summary>
        /// <typeparam name="TAppEntity">Тип сущности</typeparam>
        /// <param name="ids">Перечисление Id-ников сущностей</param>
        /// <returns>Набор сущностей с соответствующими Id</returns>
        public IQueryable<TAppEntity> GetAppEntities<TAppEntity>(IEnumerable<string> ids) where TAppEntity : class
        {
            var listIds = ids.ToList();
            switch (listIds.Count)
            {
                case 0:
                    return new List<TAppEntity>().AsQueryable();
                case 1:
                {
                    return GetOneAppEntity<TAppEntity>(listIds[0]);
                }
                default:
                {
                    var pkPropInfo = _keyProperties.GetOrAdd(typeof(TAppEntity), GetKeyProperty);

                    var targetTypeList = ConvertList(ids, pkPropInfo.PropertyType);

                    var containsMethod = _containsMethods.GetOrAdd(typeof(TAppEntity), (t) =>
                    typeof(Enumerable).GetMethods().FirstOrDefault(m => m.Name == "Contains" && m.GetParameters().Length == 2).MakeGenericMethod(pkPropInfo.PropertyType));

                    var parameter = Expression.Parameter(typeof(TAppEntity), "e");
                    var body = Expression.Call(null, containsMethod,
                        Expression.Constant(targetTypeList),
                        Expression.Convert(Expression.MakeMemberAccess(parameter, pkPropInfo), pkPropInfo.PropertyType));
                    var predicateExpression = Expression.Lambda<Func<TAppEntity, bool>>(body, parameter);

                    return Set<TAppEntity>().Where(predicateExpression).AsQueryable<TAppEntity>();
                }
            }
        }

        private static object ConvertList(IEnumerable<string> items, Type targetItemType)
        {
            var enumerableType = typeof(System.Linq.Enumerable);
            var castMethod = _castMethods.GetOrAdd(targetItemType, enumerableType.GetMethod(nameof(System.Linq.Enumerable.Cast)).MakeGenericMethod(targetItemType));
            var toListMethod = _toListMethods.GetOrAdd(targetItemType, enumerableType.GetMethod(nameof(System.Linq.Enumerable.ToList)).MakeGenericMethod(targetItemType));

            IEnumerable<object> itemsToCast;

            itemsToCast = items.Select(item => Convert.ChangeType(item, targetItemType));

            var castedItems = castMethod.Invoke(null, new[] { itemsToCast });
            return toListMethod.Invoke(null, new[] { castedItems });
        }

        /// <summary>
        /// Получение списка сущностей в соответствии с предикатом
        /// </summary>
        /// <typeparam name="TAppEntity">Тип сущности</typeparam>
        /// <param name="predicate">Предикат</param>
        /// <returns>Набор сущностей, удовлетворяющих предикату</returns>
        public IQueryable<TAppEntity> GetAppEntities<TAppEntity>(Expression<Func<TAppEntity, bool>> predicate) where TAppEntity : class
        {
            return Set<TAppEntity>().Where(predicate);
        }

        private IQueryable<TAppEntity> GetOneAppEntity<TAppEntity>(string id) where TAppEntity : class
        {
            var ret = new List<TAppEntity>(1);
            var obj = AtlantisFind(typeof(TAppEntity), id);
            if (obj != null)
            {
                ret.Add((TAppEntity)obj);
            }
            return ret.AsQueryable();
        }

        private string CreateQueryForGetAppEntities<TAppEntity>(List<string> ids) where TAppEntity : class
        {
            var tableName = GetTableName(typeof(TAppEntity));
            // Разбиваем на транки IN в sql запросе, где будет по GetAppEntitiesInOperatorItemsCount(по-умолчанию 100, задается в конфиге) id-шников (на всякий случай, если есть ограничение)
            var sql = ids
                .Select((Value, Index) => new { Value, Index })
                .GroupBy(p => p.Index / AtlantisTable.GetAppEntitiesInOperatorItemsCount)
                .Select(g => $"Select {tableName}.* From {tableName} WHERE {tableName}.NREC IN ({g.Select(x => $"#comp({x.Value})").Aggregate((f, s) => $"{f},{s}")})")
                .Aggregate((f, s) => $"{f} UNION {s}");
            return sql;
        }

        private IQueryable<TAppEntity> GetManyAppEntities<TAppEntity>(string query) where TAppEntity : class
        {
            using (var connection = Database.GetDbConnection())
            {
                return connection.Query<TAppEntity>(query).AsQueryable();
            }
        }

        /// <summary>
        /// Получение списка Id для сущностей, удовлетворяющих предикату
        /// </summary>
        /// <typeparam name="TAppEntity">Тип сущности</typeparam>
        /// <param name="condition">Предикат</param>
        /// <returns>Список Id</returns>
        public List<string> GetAppEntitiesIds<TAppEntity>(string condition = "") where TAppEntity : class
        {
            using (var command = Database.GetDbConnection().CreateCommand())
            {
                var tableName = GetTableName(typeof(TAppEntity));

                command.CommandText = string.Format("Select NREC From {0} {1}", tableName, condition);

                Database.OpenConnection();
                using (var reader = command.ExecuteReader()) // ADO.NET sql executing.
                {
                    var result = new List<string>();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            // Get key of the reader.
                            result.Add(reader.GetValue(0).ToString());
                        }
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// Обновление состояния сущности
        /// </summary>
        /// <typeparam name="TAppEntity">Тип сущности</typeparam>
        /// <param name="entity">Сущность</param>
        /// <returns>Обновленная сущность</returns>
        public TAppEntity Reload<TAppEntity>(TAppEntity entity) where TAppEntity : class
        {
            Entry(entity).Reload();
            return entity;
        }

        /// <summary>
        /// Получение сущности, удовлетворяющей предикату
        /// </summary>
        /// <typeparam name="TAppEntity">Тип сущности</typeparam>
        /// <param name="predicate">Предикат</param>
        /// <returns>Сущность</returns>
        public TAppEntity GetAppEntity<TAppEntity>(Expression<Func<TAppEntity, bool>> predicate) where TAppEntity : class
        {
            var resultSet = Set<TAppEntity>();

            //сначала ищем в БД
            var result = resultSet.FirstOrDefault(predicate);

            //если нашли, включаем в локальный кеш
            if (result != null && Entry(result) == null)
                Attach(result);

            //если нет, ищем в локальном кеше
            if (result == null)
                result = resultSet.Local.FirstOrDefault(predicate.Compile());

            return result;
        }

        private string GetTableName(Type appEntityType)
		{
            return _tableNamesCache.GetOrAdd(appEntityType, (t) => {
                var entityType = Model.FindEntityType(appEntityType);
                var result = entityType.Relational().TableName;
                return result;
            });
		}

        private static PropertyInfo GetKeyProperty(Type type)
        {
            var keyFields = type.GetProperties().Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any()).ToList();
            switch (keyFields.Count())
            {
                case 0:
                    {
                        throw new EsbException($"Can't find Key property for entity[{type}]");
                    }
                case 1:
                    {
                        return keyFields[0];
                    }
                default:
                    {
                        throw new EsbException($"Multiple Key property is not supported. Entity[{type}]");
                    }
            }
        }
    }
}
