using Application.DTOs.CatalogoAsistencia;
using Application.Extensions;
using Application.Interfaces;
using Application.Wrappers;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Asistencia.Catalogo.Queries
{
    public class GetAllCatalogoAsistenciaQuery : IRequest<PagedResponse<List<CatalogoAsistenciaDTO>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetAllCatalogoAsistenciaQueryHandler : IRequestHandler<GetAllCatalogoAsistenciaQuery, PagedResponse<List<CatalogoAsistenciaDTO>>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;

        public GetAllCatalogoAsistenciaQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<PagedResponse<List<CatalogoAsistenciaDTO>>> Handle(GetAllCatalogoAsistenciaQuery request, CancellationToken cancellationToken)
        {

            var catalogoAsistencia = await context.CatalogoAsistencia.ToListAsync(request.PageNumber, request.PageSize);
            
            var response = mapper.Map<List<CatalogoAsistenciaDTO>>(catalogoAsistencia);

            return new PagedResponse<List<CatalogoAsistenciaDTO>>(response, request.PageNumber, request.PageSize);

        }

    }
}
