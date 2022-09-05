using TermixListing.API.Core.Models.Hotel;

namespace TermixListing.API.Core.Models.Country
{
    public class GetCountryDetailsDto : BaseCountryDto
    {
        public string Id { get; set; }
   
        public List<HotelDto> Hotels { get; set; }
    }


}
