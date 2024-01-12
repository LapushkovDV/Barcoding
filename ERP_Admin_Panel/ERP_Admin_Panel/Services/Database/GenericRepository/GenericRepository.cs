using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ERP_Admin_Panel.Services.Database.GenericRepository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationContext _dbContext;
        private readonly DbSet<TEntity> _entities;

        public GenericRepository(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
            _entities = _dbContext.Set<TEntity>();
        }

        public bool Create(TEntity entity)
        {
            var added = _entities.Add(entity).State == EntityState.Added;

            _dbContext.SaveChanges();

            return added;
        }

        public bool DeleteById(int id)
        {
            var entity = _entities.Find(id);
            var deleted = _entities.Remove(entity).State == EntityState.Deleted;

            _dbContext.SaveChanges();

            return deleted;
        }

        public bool Delete(TEntity entity)
        {
            var deleted = _entities.Remove(entity).State == EntityState.Deleted;

            _dbContext.SaveChanges();

            return deleted;
        }

        public async Task DeleteAll()
        {
            _entities.RemoveRange(_dbContext.Set<TEntity>().AsNoTracking());
            await _dbContext.SaveChangesAsync();
        }

        public TEntity? GetById(int id) => _entities.Find(id);


        public bool Update(TEntity entity)
        {
            var update = _entities.Update(entity).State == EntityState.Modified;

            _dbContext.SaveChanges();

            return update;
        }

        public List<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _entities;

            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            return query.ToList();
        }

        public TEntity? Get(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _entities;

            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            return query.FirstOrDefault(filter);
        }
    }
}
