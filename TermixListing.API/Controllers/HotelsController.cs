using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TermixListing.API.Data;
using AutoMapper;
using TermixListing.API.Core.Contracts;
using TermixListing.API.Core.Models.Hotel;
using Microsoft.AspNetCore.Authorization;
using TermixListing.API.Core.Models.Country;
using TermixListing.API.Core.Models;
using TermixListing.API.Core.Repository;
using TermixListing.API.Core.Exceptions;

namespace TermixListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class HotelsController : ControllerBase
    {
        private readonly IHotelRepository _hotelRepository;

        private readonly IMapper _mapper;

        public HotelsController(IHotelRepository hotelRepository, IMapper mapper)
        {
            this._hotelRepository = hotelRepository;
            this._mapper = mapper;
        }

        // GET: api/Hotels
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels()
        {

            var hotels = await _hotelRepository.GetAllAsync<HotelDto>();
            return Ok(hotels);
        }
        // GET: api/Hotels?StartIndex=0&PageNumber=2&PageSize=4
        [HttpGet]
        public async Task<ActionResult<PageResult<HotelDto>>> GetPagedCountry([FromQuery] QueryParameters queryParameters)
        {
            var PagedHotelsResult = await _hotelRepository.GetAllAsync<HotelDto>(queryParameters);
            return Ok(PagedHotelsResult);
        }
        // GET: api/Hotels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDto>> GetHotel(int id)
        {
            var hotels = await _hotelRepository.GetAsync<HotelDto>(id);
            return Ok(hotels);
        }

        // PUT: api/Hotels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutHotel(int id, HotelDto hoteldto)
        {

            if (id != hoteldto.Id)
            {
                throw new BadRequestException(id);
            }

            try
            {
                await _hotelRepository.UpdateAsync(id, hoteldto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await HotelExists(id))
                {
                    throw new NotFoundException("Hotel", id);
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Hotels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<HotelDto>> PostHotel(CreateHotelDTO createHotelDTO)
        {
            var hotel = await _hotelRepository.AddAsync<CreateHotelDTO, HotelDto>(createHotelDTO);
            return CreatedAtAction(nameof(GetHotel), new { id = hotel.Id }, hotel);
        }

        // DELETE: api/Hotels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var hotel = await _hotelRepository.GetAsync(id);
            if (hotel == null)
            {
                throw new NotFoundException("Hotel", id);
            }
            await _hotelRepository.DeleteAsync(id);

            return NoContent();
        }

        private async Task<bool> HotelExists(int id)
        {
            return await _hotelRepository.Exist(id);
        }
    }
}
