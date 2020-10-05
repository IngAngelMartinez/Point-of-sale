using Application.DTOs.CatalogoAsistencia;
using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Asistencia.Catalogo.Queries
{
    public class GetByIdCatalogoAsistenciaQuery : IRequest<Response<CatalogoAsistenciaDTO>> 
    {
        public int Id { get; set; }
    }

    public class GetByIdCatalogoAsistenciaQueryHandler : IRequestHandler<GetByIdCatalogoAsistenciaQuery, Response<CatalogoAsistenciaDTO>>
    {

        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;

        public GetByIdCatalogoAsistenciaQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<Response<CatalogoAsistenciaDTO>> Handle(GetByIdCatalogoAsistenciaQuery request, CancellationToken cancellationToken)
        {
            var catalogoAsistencia = await context.CatalogoAsistencia.FirstOrDefaultAsync(T => T.Id == request.Id);
            var response = mapper.Map<CatalogoAsistenciaDTO>(catalogoAsistencia);

            return new Response<CatalogoAsistenciaDTO>(response);

        }
    }
}
