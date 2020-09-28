using API.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Application.Filters;
using Application.Features.Asistencia.Catalogo.Commands;
using Application.Features.Asistencia.Catalogo.Queries;

namespace API.Controllers
{
    public class CatalogoAsistenciaController : BaseController
    {

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] FilterPageRequest filter)
        {
            return Ok(await Mediator.Send(new GetAllCatalogoAsistenciaQuery { PageNumber = filter.PageNumber, PageSize = filter.PageSize }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetByIdCatalogoAsistenciaQuery { Id = id }));
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateCatalogoAsistenciaCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UpdateCatalogoAsistenciaCommand command)
        {
            if (id != command.Id) return BadRequest();
            return Ok(await Mediator.Send(command));
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) 
        {
            return Ok(await Mediator.Send(new DeleteCatalogoAsistenciaCommand { Id = id }));
        }

    }
}
