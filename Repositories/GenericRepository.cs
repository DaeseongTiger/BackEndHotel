using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ForMiraiProject.Data;
using ForMiraiProject.Repositories.Interfaces;

namespace ForMiraiProject.Repositories
{
    public class GenericRepository<T> : I_GenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly ILogger<GenericRepository<T>> _logger;

        public GenericRepository(AppDbContext context, ILogger<GenericRepository<T>> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Fetching all records for {EntityType}.", typeof(T).Name);
                return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching records for {EntityType}.", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // ตรวจสอบว่า id ไม่เป็น Guid.Empty
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid ID provided for GetByIdAsync.");
                throw new ArgumentException("The ID cannot be empty.", nameof(id));
            }

            try
            {
                _logger.LogInformation("Attempting to retrieve entity with ID {id}", id);
                
                // ค้นหาข้อมูลจาก DbContext
                var entity = await _dbSet.FirstOrDefaultAsync(
                    e => EF.Property<Guid>(e, "Id") == id,
                    cancellationToken);

                if (entity == null)
                {
                    _logger.LogWarning("Entity with ID {id} not found for {EntityType}.", id, typeof(T).Name);
                    throw new KeyNotFoundException($"Entity with ID {id} not found.");
                }

                _logger.LogInformation("Successfully retrieved entity with ID {id}", id);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching entity with ID {id} for {EntityType}.", id, typeof(T).Name);
                throw;
            }
        }

        public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                _logger.LogWarning("Attempted to add a null entity.");
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }

            try
            {
                await _dbSet.AddAsync(entity, cancellationToken);
                await SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Entity of type {EntityType} added successfully.", typeof(T).Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding entity for {EntityType}.", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                _logger.LogWarning("Attempted to update a null entity.");
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }

            try
            {
                _dbSet.Update(entity);
                await SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Entity of type {EntityType} updated successfully.", typeof(T).Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating entity for {EntityType}.", typeof(T).Name);
                throw;
            }
        }

        public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await GetByIdAsync(id, cancellationToken);
                if (entity == null)
                {
                    _logger.LogWarning("Entity with ID {Id} not found for deletion in {EntityType}.", id, typeof(T).Name);
                    throw new KeyNotFoundException($"Entity with ID {id} was not found.");
                }

                _dbSet.Remove(entity);
                await SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Entity of type {EntityType} with ID {Id} deleted successfully.", typeof(T).Name, id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting entity with ID {Id} for {EntityType}.", id, typeof(T).Name);
                throw;
            }
        }

        public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Invalid ID provided for ExistsAsync.");
                    throw new ArgumentException("The ID cannot be empty.", nameof(id));
                }

                return await _dbSet.AnyAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence of entity with ID {Id} for {EntityType}.", id, typeof(T).Name);
                throw;
            }
        }

        private  async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error occurred while saving changes for {EntityType}.", typeof(T).Name);
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while saving changes for {EntityType}.", typeof(T).Name);
                throw;
            }
        }
    }
}
