using Application.Interfaces;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Asistencia.Catalogo.Commands
{
    public partial class CreateCatalogoAsistenciaCommand : IRequest<Response<int>>
    {
        public string Descripcion { get; set; }
    }

    public class CreateCatalogoAsistenciaCommandHandler : IRequestHandler<CreateCatalogoAsistenciaCommand, Response<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;

        public CreateCatalogoAsistenciaCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateCatalogoAsistenciaCommand request, CancellationToken cancellationToken)
        {
            var catalogoAsistencia = mapper.Map<CatalogoAsistencia>(request);
            context.CatalogoAsistencia.Add(catalogoAsistencia);
            await context.SaveChangesAsync(cancellationToken);

            return new Response<int>(catalogoAsistencia.Id);           

        }
    }
}
