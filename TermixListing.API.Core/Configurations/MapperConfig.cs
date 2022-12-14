using AutoMapper;
using TermixListing.API.Data;
using TermixListing.API.Core.Models.Country;
using TermixListing.API.Core.Models.Hotel;
using TermixListing.API.Core.Models.Users;

namespace TermixListing.API.Core.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Country, CreateCountryDto>().ReverseMap();
            CreateMap<Country, GetCountryDto>().ReverseMap();
            CreateMap<Country, GetCountryDetailsDto>().ReverseMap();
            CreateMap<Country, UpdateCountryDto>().ReverseMap();

            CreateMap<Hotel, HotelDto>().ReverseMap();
            CreateMap<Hotel, CreateHotelDTO>().ReverseMap();

            CreateMap<ApiUserDto, ApiUser>().ReverseMap();
            CreateMap<LoginDTO, ApiUser>().ReverseMap();

        }
    }
}

