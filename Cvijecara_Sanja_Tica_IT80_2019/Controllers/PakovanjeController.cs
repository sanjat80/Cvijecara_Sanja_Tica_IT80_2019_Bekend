﻿using AutoMapper;
using Cvijecara_Sanja_Tica_IT80_2019.Data.PakovanjeData;
using Cvijecara_Sanja_Tica_IT80_2019.Data.TipKorisnikaData;
using Cvijecara_Sanja_Tica_IT80_2019.Data.ValidationData;
using Cvijecara_Sanja_Tica_IT80_2019.Data.VrstaData;
using Cvijecara_Sanja_Tica_IT80_2019.Entities;
using Cvijecara_Sanja_Tica_IT80_2019.Models.KategorijaModel;
using Cvijecara_Sanja_Tica_IT80_2019.Models.PakovanjeModel;
using Cvijecara_Sanja_Tica_IT80_2019.Models.TipKorisnikaModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cvijecara_Sanja_Tica_IT80_2019.Controllers
{
    [ApiController]
    [Route("api/pakovanja")]
    [Produces("application/json","application/xml")]
    public class PakovanjeController:ControllerBase
    {
        private readonly IPakovanjeRepository pakovanjeRepository;
        private readonly LinkGenerator linkGenerator;
        private readonly IValidationRepository validationRepository;
        private readonly IMapper mapper;

        public PakovanjeController(IPakovanjeRepository pakovanjeRepository, LinkGenerator linkGenerator, IMapper mapper,IValidationRepository validationRepository)
        {
            this.pakovanjeRepository = pakovanjeRepository;
            this.linkGenerator = linkGenerator;
            this.mapper = mapper;
            this.validationRepository = validationRepository;
        }

        [HttpGet]
        [HttpHead]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult<List<PakovanjeDto>> GetAllPakovanje()
        {
            var pakovanja = pakovanjeRepository.GetAllPakovanje();
            if (pakovanja == null || pakovanja.Count == 0)
            {
                return NoContent();
            }
            return Ok(mapper.Map<List<PakovanjeDto>>(pakovanja));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<PakovanjeDto> GetPakovanjeById(int id)
        {
            var pakovanje = pakovanjeRepository.GetPakovanjeById(id);
            if (pakovanje == null)
            {
                return NotFound("Pakovanje sa proslijedjenim id-em nije pronadjen.");
            }
            return Ok(mapper.Map<PakovanjeDto>(pakovanje));
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<PakovanjeConfirmationDto> CreatePakovanje([FromBody] PakovanjeCreationDto pakovanje)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!validationRepository.ValidateValuta(pakovanje.Valuta))
                    { 
                        return BadRequest( "Valuta mora biti neka od 3 dozvoljene: RSD,EUR,BAM, duzine tacno 3 karaktera.");
                    }
                    Pakovanje pak = mapper.Map<Pakovanje>(pakovanje);
                    PakovanjeConfirmation confirmation = pakovanjeRepository.CreatePakovanje(pak);
                    pakovanjeRepository.SaveChanges();
                    //string? location = linkGenerator.GetPathByAction("GetTipKorisnikaById", "TipKorisnika", new { tipId = confirmation.TipId });
                    return Ok(mapper.Map<PakovanjeDto>(pak));
                } else
                {
                    return BadRequest(ModelState);
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Greska prilikom kreiranja pakovanja");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles ="admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeletePakovanje(int id)
        {
            try
            {
                var pakovanjeModel = pakovanjeRepository.GetPakovanjeById(id);
                if (pakovanjeModel == null)
                {
                    return NotFound("Pakovanje sa proslijedjenim id-em nije pronadjen.");
                }
                pakovanjeRepository.DeletePakovanje(id);
                pakovanjeRepository.SaveChanges();
                return NoContent();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Greska prilikom brisanja pakovanja");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<PakovanjeDto> UpdatePakovanje(PakovanjeUpdateDto pakovanje)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    if(!validationRepository.ValidateValuta(pakovanje.Valuta))
                    {
                        return BadRequest("Valuta mora biti neka od 3 dozvoljene: RSD,EUR,BAM i to duzine 3 karaktera.");
                    }
                    var staroPakovanje = pakovanjeRepository.GetPakovanjeById(pakovanje.PakovanjeId);
                    if (staroPakovanje == null)
                    {
                        return NotFound("Pakovanje sa proslijedjenim id-em nije pronadjen.");
                    }
                    Pakovanje pak = mapper.Map<Pakovanje>(pakovanje);
                    mapper.Map(pak, staroPakovanje);
                    pakovanjeRepository.SaveChanges();
                    return Ok(mapper.Map<PakovanjeDto>(staroPakovanje));
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Greska prilikom azuriranja pakovanja.");
            }
        }

        [HttpGet("pakovanjaId")]
        public ActionResult<List<int>> GetAllVrstaId()
        {
            var pakovanja = pakovanjeRepository.GetAllPakovanjeId();
            if (pakovanja != null)
            {
                return Ok(pakovanja);
            }
            return NotFound();
        }

    }
}
