namespace OnlineStore.Data.Data.Common;

using Microsoft.EntityFrameworkCore;
public class Repository : IRepository
{
    private readonly DbContext data;
    public Repository(OnlineStoreDbContext _data)
    {
        data = _data;
    }
    private DbSet<T> DbSet<T>() where T : class
    {
        return this.data.Set<T>();
    }
    public async Task AddAsync<T>(T entity) where T : class
    {
        await DbSet<T>().AddAsync(entity);
    }

    public IQueryable<T> All<T>() where T : class
    {
        return DbSet<T>();
    }

    public IQueryable<T> AllReadOnly<T>() where T : class
    {
        return DbSet<T>()
             .AsNoTracking();
    }

    public async Task DeleteAsync<T>(object id) where T : class
    {
        T? entity = await GetByIdAsync<T>(id);

        if (entity != null)
        {
            DbSet<T>().Remove(entity);
        }
    }

    public async Task<T?> GetByIdAsync<T>(object id) where T : class
    {
        return await DbSet<T>().FindAsync(id);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await this.data.SaveChangesAsync();
    }

    public async Task UpdateAsync<T>(T entity) where T : class
    {
        this.data.Update(entity);
        await this.data.SaveChangesAsync();
    }

    public async Task DeleteRange<T>(IEnumerable<T> entities) where T : class
    {
        DbSet<T>().RemoveRange(entities);
    }
}
