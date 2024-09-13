using System;
using System.Linq;
using System.Linq.Expressions;
using Galaktika.ESB.Adapter.Atlantis.Api;
using Microsoft.EntityFrameworkCore;
using Galaktika.ESB.Adapter.Services;

namespace Galaktika.ESB.Core
{
    /// <summary>
    /// Extensions for Erp IRestierApiService implementation
    /// </summary>
    public static class RestierExtension
    {
        /// <summary>
        /// Gets AppEntity on read-only mode
        /// </summary>
        /// <param name="restier"></param>
        /// <param name="session"></param>
        /// <param name="predicate"></param>
        /// <param name="readOnly"></param>
        /// <typeparam name="TAppEntity"></typeparam>
        /// <returns></returns>
        public static TAppEntity GetAppEntity<TAppEntity>(this IRestierApiService restier, ISession session,
            Expression<Func<TAppEntity, bool>> predicate, bool readOnly) where TAppEntity : class
        {
            if (!readOnly)
                return restier.GetAppEntity(session, predicate);
            if (session.EsbContext == null)
            {
                restier.SetEsbContext(session, typeof(TAppEntity));
            }
            if (!(session.EsbContext is AtlantisBaseContext context))
                throw new InvalidCastException("Incorrect cast to AtlantisBaseContext");
            var resultSet = context.Set<TAppEntity>().AsNoTracking();
            return resultSet.FirstOrDefault(predicate);

        }

        /// <summary>
        /// Gets AppEntity by key on read-only mode
        /// </summary>
        /// <param name="restier"></param>
        /// <param name="session"></param>
        /// <param name="entityKey"></param>
        /// <param name="readOnly"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static TEntity GetAppEntityByKey<TEntity>(this IRestierApiService restier, ISession session, string entityKey, bool readOnly) where TEntity : class
        {
            if (!readOnly)
                return restier.GetAppEntityByKey<TEntity>(session, entityKey);
            if (session.EsbContext == null)
            {
                restier.SetEsbContext(session, typeof(TEntity));
            }
            if (!(session.EsbContext is AtlantisBaseContext context))
                throw new InvalidCastException("Incorrect cast to AtlantisBaseContext");
            var obj = context.AtlantisFind(typeof(TEntity), entityKey);
            if (obj == null)
            {
                return null;
            }
            var entity = (TEntity)obj;
            context.Entry(entity).State = EntityState.Detached;
            return entity;

        }
    }
}
