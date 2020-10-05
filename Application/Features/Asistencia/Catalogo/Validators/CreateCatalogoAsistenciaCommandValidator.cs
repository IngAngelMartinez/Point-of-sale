using Application.Features.Asistencia.Catalogo.Commands;
using Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Asistencia.Catalogo.Validators
{
    public class CreateCatalogoAsistenciaCommandValidator : AbstractValidator<CreateCatalogoAsistenciaCommand>
    {
        private readonly IApplicationDbContext context;

        public CreateCatalogoAsistenciaCommandValidator(IApplicationDbContext context)
        {
            this.context = context;

            const string descriptionEmpty = "Description is empty.";

            RuleFor(T => T.Descripcion).NotEmpty().WithMessage(descriptionEmpty)
                                       .NotNull().WithMessage(descriptionEmpty)
                                       .MustAsync(UniqueDescription).WithMessage("Description exist.");
        }

        public async Task<bool> UniqueDescription(string description, CancellationToken cancellationToken)
        {
            return await context.CatalogoAsistencia.AllAsync(T => T.Descripcion != description);
        }
        
    }
}
