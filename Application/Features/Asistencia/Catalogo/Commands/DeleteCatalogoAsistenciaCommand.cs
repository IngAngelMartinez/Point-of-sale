using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Asistencia.Catalogo.Commands
{
    public class DeleteCatalogoAsistenciaCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
    }

    public class DeleteCatalogoAsistenciaCommandHandler : IRequestHandler<DeleteCatalogoAsistenciaCommand, Response<int>>
    {
        private readonly IApplicationDbContext context;

        public DeleteCatalogoAsistenciaCommandHandler(IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Response<int>> Handle(DeleteCatalogoAsistenciaCommand request, CancellationToken cancellationToken)
        {

            var catalogoAsistencia = await context.CatalogoAsistencia.FirstOrDefaultAsync(T => T.Id == request.Id);
            if (catalogoAsistencia == null) throw new ApiException("Not Found");

            context.CatalogoAsistencia.Remove(catalogoAsistencia);
            await context.SaveChangesAsync(cancellationToken);

            return new Response<int>(catalogoAsistencia.Id);

        }
    }
}
