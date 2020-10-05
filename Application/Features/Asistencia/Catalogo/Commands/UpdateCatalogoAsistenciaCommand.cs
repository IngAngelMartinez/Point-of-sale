using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Asistencia.Catalogo.Commands
{
    public class UpdateCatalogoAsistenciaCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }

    }

    public class UpdateCatalogoAsistenciaCommandHandler : IRequestHandler<UpdateCatalogoAsistenciaCommand, Response<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;

        public UpdateCatalogoAsistenciaCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<Response<int>> Handle(UpdateCatalogoAsistenciaCommand request, CancellationToken cancellationToken)
        {

            var catalogoAsistencia = mapper.Map<CatalogoAsistencia>(request);
            context.Entry(catalogoAsistencia).State = EntityState.Modified;
            await context.SaveChangesAsync(cancellationToken);

            return new Response<int>(catalogoAsistencia.Id);

        }
    }
}
