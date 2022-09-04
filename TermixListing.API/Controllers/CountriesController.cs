#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TermixListing.API.Data;
using TermixListing.API.Models.Country;
using AutoMapper;
using TermixListing.API.Contracts;
using Microsoft.AspNetCore.Authorization;
using TermixListing.API.Middleware;
using TermixListing.API.Exceptions;

namespace TermixListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countriesRepository;
        private readonly ILogger _logger;

        public CountriesController(IMapper mapper, ICountriesRepository countriesRepository, ILogger<CountriesController> logger)

        {
            this._mapper = mapper;
            this._countriesRepository = countriesRepository;
            this._logger = logger;
        }

        // GET: api/Countries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()

        {
            //select * from Countries
            var countries = await _countriesRepository.GetAllAsync();
            //
            var records = _mapper.Map<List<GetCountryDto>>(countries);
            return Ok(records);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetCountryDetailsDto>> GetCountry(int id)
        {
            var country = await _countriesRepository.GetDetails(id);


            if (country == null)
            {
                _logger.LogWarning($"No Record Found in{nameof(GetCountry)} With Id: {id}.");
                throw new NotFoundException(nameof(GetCountry), id);
            }

            var countryDto = _mapper.Map<GetCountryDetailsDto>(country);

            return Ok(countryDto);
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)

        {
            //if (id != country.Id)
            if (id != updateCountryDto.Id)

            {
                _logger.LogWarning($"Invalid Record Id: {id} for {nameof(PutCountry)}.");
                throw new BadRequestException(id);
                //return BadRequest("Invalid Record Id");
            } 
              
            //_context.Entry(country).State = EntityState.Modified;

            var country = await _countriesRepository.GetAsync(id);
            if (country == null)
            {
                _logger.LogWarning($"No Record Found in{nameof(PutCountry)} With Id: {id}.");
                throw new NotFoundException(nameof(PutCountry), id);
            }
            _mapper.Map(updateCountryDto, country);


            try
            {
                await _countriesRepository.UpdateAsync(country);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CountryExists(id))
                {
                    _logger.LogWarning($"No Record Found in{nameof(PutCountry)} With Id: {id}.");
                    throw new NotFoundException(nameof(PutCountry), id);
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDto CreateCountryDto)
        {
            var country = _mapper.Map<Country>(CreateCountryDto);
            await _countriesRepository.AddAsync(country);
            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }


        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _countriesRepository.GetAsync(id);
            if (country == null)
            {
                _logger.LogWarning($"No Record Found in{nameof(DeleteCountry)} With Id: {id}.");
                throw new NotFoundException(nameof(DeleteCountry), id);
            }

            await _countriesRepository.DeleteAsync(id);

            return NoContent();
        }

        private async Task<bool> CountryExists(int id)
        {
            return await _countriesRepository.Exist(id);
        }
    }
}
