using System.Linq.Expressions;

namespace ERP_Admin_Panel.Services.Database.GenericRepository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        List<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>,
            IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includes);


        TEntity? GetById(int id);

        TEntity? Get(Expression<Func<TEntity, bool>> filter = null,
            params Expression<Func<TEntity, object>>[] includes);

        bool Create(TEntity entity);

        bool Update(TEntity entity);

        bool Delete(TEntity entity);
        bool DeleteById(int id);

        Task DeleteAll();
    }
}
