using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TermixListing.API.Core.Contracts;
using TermixListing.API.Data;
using TermixListing.API.Core.Models;
using TermixListing.API.Core.Exceptions;

namespace TermixListing.API.Core.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly termixListViewContext _context;
        private readonly IMapper _mapper;

        public GenericRepository(termixListViewContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }
        public async Task<T> AddAsync(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TResult> AddAsync<TSource, TResult>(TSource source)
        {
            var entity = _mapper.Map<T>(source);
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<TResult>(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            if (entity == null)
            {
                throw new NotFoundException(typeof(T).Name, id);
            }
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Exist(int id)
        {
            var entity = await GetAsync(id);
            return entity != null;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<PageResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters)
        {
            var totalSize = await _context.Set<T>().CountAsync();
            var indexStart = (queryParameters.PageNumber - 1) * (queryParameters.PageSize);
            var items = await _context.Set<T>()
                .Skip(indexStart)
                .Take(queryParameters.PageSize)
                //exact colums
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToListAsync();
            var indexSize = totalSize - 1;
            var totalPages = ((indexSize - (indexSize % queryParameters.PageSize)) / queryParameters.PageSize + 1);

            return new PageResult<TResult>
            {
                Items = items,
                PageNumber = queryParameters.PageNumber,
                RecordNumber = queryParameters.PageSize,
                TotalCount = totalSize,
                TotalPage = totalPages
            };
        }

        public async Task<List<TResult>> GetAllAsync<TResult>()
        {
            return await _context.Set<T>()
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<T> GetAsync(int? id)
        {
            if (id is null)
            {
                return null;
            }

            return await _context.Set<T>().FindAsync(id);

        }

        public async Task<TResult> GetAsync<TResult>(int? id)
        {
            var result = await _context.Set<T>().FindAsync(id);
            if (result is null)
            {
                throw new NotFoundException(typeof(T).Name, id.HasValue ? id : "No Key Provided");
            }

            return _mapper.Map<TResult>(result);
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync<TSource>(int id, TSource source)
        {
            var entity = await GetAsync(id);

            if (entity is null)
            {
                throw new NotFoundException(typeof(T).Name, id);
            }
            _mapper.Map(source, entity);
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
