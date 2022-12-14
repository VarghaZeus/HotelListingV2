using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TermixListing.API.Core.Contracts;
using TermixListing.API.Data;

namespace TermixListing.API.Core.Repository
{
    public class CountriesRepository : GenericRepository<Country>, ICountriesRepository
    {
        private readonly termixListViewContext _context;

        public CountriesRepository(termixListViewContext context, IMapper mapper) : base(context, mapper)
        {
            this._context = context;
        }

        public async Task<Country> GetDetails(int id)
        {
           return await _context.Countries.Include(q => q.Hotels)
                .FirstOrDefaultAsync(q => q.Id == id);
        }
    }
}
