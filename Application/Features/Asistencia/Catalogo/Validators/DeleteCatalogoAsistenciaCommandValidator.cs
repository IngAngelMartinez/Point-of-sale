using Application.Features.Asistencia.Catalogo.Commands;
using Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Asistencia.Catalogo.Validators
{
    public class DeleteCatalogoAsistenciaCommandValidator : AbstractValidator<DeleteCatalogoAsistenciaCommand>
    {
        private readonly IApplicationDbContext context;

        public DeleteCatalogoAsistenciaCommandValidator(IApplicationDbContext context)
        {
            this.context = context;

            RuleFor(T => T.Id).MustAsync(Exist).WithMessage("Not found.");
        }

        private async Task<bool> Exist(int id, CancellationToken cancellationToken)
        {
            return await context.CatalogoAsistencia.AnyAsync(T => T.Id == id);
        }
    }
}
