using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TermixListing.API.Contracts;
using TermixListing.API.Data;

namespace TermixListing.API.Repository
{
    public class HotelRepository : GenericRepository<Hotel>, IHotelRepository
    {

        public HotelRepository(termixListViewContext context, IMapper mapper) : base(context, mapper)
        {

        }
    }
}
