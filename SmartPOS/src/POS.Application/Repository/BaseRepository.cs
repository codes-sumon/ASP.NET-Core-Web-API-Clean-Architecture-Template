using AutoMapper;
using Microsoft.EntityFrameworkCore;
using POS.Domain.Common;
using System.Linq.Expressions;
using POS.Application.Common;
using POS.Application.Common.Collection;
using POS.Application.Models;

namespace POS.Application.Repository;

public class BaseRepository<TEntity, TModel, T> : IBaseRepository<TEntity, TModel, T>
    where TEntity : class, IEntity<T>, new()
    where TModel : class, IEntityVM<T>, new()
    where T : IEquatable<T>
{
    protected readonly IMapper _mapper;
    private readonly DbContext _context;

    public DbSet<TEntity> DbSet { get; set; }

    public BaseRepository(IMapper mapper, DbContext context)
    {
        _mapper = mapper;
        _context = context;
        DbSet = _context.Set<TEntity>();
    }

    #region GetPageAsync

    public async Task<Paging<TModel>> GetPageAsync(int pageIndex, int pageSize)
    {
        var data = await DbSet.Where(x => !x.IsDelete).OrderByDescending(e => e.Id).PagingAsync(pageIndex, pageSize);
        return data.ToPagingModel<TEntity, TModel>(_mapper);
    }

    public async Task<Paging<TModel>> GetPageAsync(int pageIndex, int pageSize, Expression<Func<TEntity, bool>> predicate)
    {
        var data = await DbSet.Where(x => !x.IsDelete).Where(predicate).OrderByDescending(e => e.Id).PagingAsync(pageIndex, pageSize);
        return data.ToPagingModel<TEntity, TModel>(_mapper);
    }

    public async Task<Paging<TModel>> GetPageAsync(int pageIndex, int pageSize, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, params Expression<Func<TEntity, object>>[] includes)
    {
        var data = await orderBy(includes.Aggregate(DbSet.Where(x => !x.IsDelete).AsQueryable(),
            (current, include) => current.Include(include)))
            .PagingAsync(pageIndex, pageSize);
        return data.ToPagingModel<TEntity, TModel>(_mapper);
    }

    public async Task<Paging<TModel>> GetPageAsync(int pageIndex, int pageSize, Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, params Expression<Func<TEntity, object>>[] includes)
    {

        var data = await orderBy(includes.Aggregate(DbSet.Where(x => !x.IsDelete).AsQueryable(),
            (current, include) => current.Include(include), c => c.Where(predicate)))
            .PagingAsync(pageIndex, pageSize);
        return data.ToPagingModel<TEntity, TModel>(_mapper);
    }

    public async Task<Paging<TResult>> GetPageAsync<TResult>(int pageIndex, int pageSize, Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, Expression<Func<TEntity, TResult>> selector, params Expression<Func<TEntity, object>>[] includes)
    {
        return await orderBy(includes.Aggregate(DbSet.Where(x => !x.IsDelete).AsQueryable(),
            (current, include) => current.Include(include), c => c.Where(predicate)))
            .PagingAsync(selector, pageIndex, pageSize);

    }

    #endregion

    #region GetDropdown

    public async Task<Dropdown<TModel>> GetDropdownAsync(Expression<Func<TEntity, bool>> predicate, int size)
    {
        var data = await DbSet.Where(predicate).OrderByDescending(e => e.Id).DropdownAsync(size);
        return data.ToDropdownModel<TEntity, TModel>(_mapper);
    }

    public async Task<Dropdown<TResult>> GetDropdownAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, Expression<Func<TEntity, TResult>> selector, int size)
    {
        var query = DbSet.Where(predicate);
        if (orderBy is not null)
            query = orderBy(query);

        return await query.DropdownAsync(selector, size);
    }

    #endregion

    #region CRUDOperation

    public async Task<TModel> InsertAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return _mapper.Map<TModel>(entity);
    }

    public async Task<TModel> UpdateAsync(T id, TEntity entity)
    {
        var data = await DbSet.FindAsync(id);
        if (data != null)
        {
            entity.Copy(data);
            DbSet.Update(data);
            await _context.SaveChangesAsync();
        }
        return _mapper.Map<TModel>(data);
    }

    public async Task<TModel> DeleteAsync(T id)
    {
        var entity = await DbSet.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        entity.IsDelete = true;
        DbSet.Update(entity);
        await _context.SaveChangesAsync();
        return _mapper.Map<TModel>(entity);
    }


    public async Task<TModel> InsertRangeAsync(List<TEntity> entities)
    {
        await DbSet.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
        return _mapper.Map<TModel>(entities);
    }

    public async Task<TModel> UpdateRangeAsync(List<TEntity> entities)
    {
        DbSet.UpdateRange(entities);
        await _context.SaveChangesAsync();
        return _mapper.Map<TModel>(entities);
    }

    public async Task<TModel> DeleteRangeAsync(List<TEntity> entities)
    {
        DbSet.RemoveRange(entities);
        await _context.SaveChangesAsync();
        return _mapper.Map<TModel>(entities);
    }

    #endregion

    #region FirstOrDefault

    public async Task<TModel> GetAsNoTrackingAsync(T id)
    {
        var entity = await DbSet.FindAsync(id);
        _context.Entry(entity).State = EntityState.Detached;
        return _mapper.Map<TModel>(entity);
    }

    public async Task<TModel> FirstOrDefaultAsync(T id) => _mapper.Map<TModel>(await DbSet.FindAsync(id));

    public async Task<TModel> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return _mapper.Map<TModel>(await DbSet.Where(x => !x.IsDelete).FirstOrDefaultAsync(predicate));
    }

    public async Task<TModel> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
    {
        return _mapper.Map<TModel>(await orderBy(DbSet.Where(x => !x.IsDelete)).FirstOrDefaultAsync(predicate));
    }

    public async Task<TModel> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
    {
        return _mapper.Map<TModel>(await includes.Aggregate(DbSet.Where(x => !x.IsDelete).AsQueryable(),
            (current, include) => current.Include(include),
            c => c.AsNoTracking().FirstOrDefaultAsync(predicate))
            .ConfigureAwait(false));
    }

    public async Task<TModel> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, params Expression<Func<TEntity, object>>[] includes)
    {
        return _mapper.Map<TModel>(await includes.Aggregate(orderBy(DbSet.Where(x => !x.IsDelete)).AsQueryable(),
            (current, include) => current.Include(include),
            c => c.FirstOrDefaultAsync(predicate)).ConfigureAwait(false));
    }

    public async Task<TModel> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
    {
        return _mapper.Map<TModel>(await orderBy(DbSet.Where(x => !x.IsDelete)).LastOrDefaultAsync(predicate));
    }

    #endregion

    #region GetAllAsync      

    public async Task<List<TModel>> GetAllAsync()
    {
        return _mapper.Map<List<TModel>>(await DbSet.Where(x => !x.IsDelete).ToListAsync());
    }
    public async Task<List<TModel>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return _mapper.Map<List<TModel>>(await DbSet.Where(x => !x.IsDelete).Where(predicate).ToListAsync());
    }

    public async Task<List<TModel>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
    {
        return _mapper.Map<List<TModel>>(await orderBy(DbSet.Where(x => !x.IsDelete).Where(predicate)).ToListAsync());
    }

    public async Task<List<TModel>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
    {
        return _mapper.Map<List<TModel>>(await includes.Aggregate(DbSet.Where(x => !x.IsDelete).AsQueryable(),
            (current, include) => current.Include(include),
            c => c.Where(predicate)).ToListAsync()
            .ConfigureAwait(false));
    }

    public async Task<List<TModel>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, params Expression<Func<TEntity, object>>[] includes)
    {
        return _mapper.Map<List<TModel>>(await orderBy(includes.Aggregate(DbSet.Where(x => !x.IsDelete).AsQueryable(),
            (current, include) => current.Include(include),
            c => c.Where(predicate))).ToListAsync()
            .ConfigureAwait(false));
    }

    #endregion

    public void Attach(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Added;
    }

    public void Detach(TEntity entity)
    {
        if (_context.Entry(entity).State != EntityState.Detached)
            _context.Entry(entity).State = EntityState.Detached;
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate = null)
    {
        IQueryable<TEntity> query = DbSet;

        if (predicate != null)
            return await query.AnyAsync(predicate);
        else
            return await query.AnyAsync();
    }
}