using Application.Features.Asistencia.Catalogo.Commands;
using Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Asistencia.Catalogo.Validators
{

    public class UpdateCatalogoAsistenciaCommandValidator : AbstractValidator<UpdateCatalogoAsistenciaCommand>
    {
        private readonly IApplicationDbContext context;

        public UpdateCatalogoAsistenciaCommandValidator(IApplicationDbContext context)
        {
            this.context = context;

            const string descriptionEmpty = "Description is empty.";

            RuleFor(T => T.Id).MustAsync(ExistId).WithMessage("Id not exist.");
            RuleFor(T => new RuleUnique{  Id = T.Id, Descripcion = T.Descripcion }).MustAsync(UniqueDescription)
                                                                                   .WithMessage("Description exist.");

            RuleFor(T => T.Descripcion).NotNull().WithMessage(descriptionEmpty)
                                       .NotEmpty().WithMessage(descriptionEmpty);
        }

        public async Task<bool> UniqueDescription(RuleUnique rule, CancellationToken cancellationToken)
        {
            var response = await context.CatalogoAsistencia.Where(T => T.Id != rule.Id).AllAsync(T => T.Descripcion != rule.Descripcion);
            return response;
        }

        public async Task<bool> ExistId(int id, CancellationToken cancellationToken)
        {
            return await context.CatalogoAsistencia.AnyAsync(T => T.Id == id);
        }

        public class RuleUnique
        {
            public int Id { get; set; }
            public string Descripcion { get; set; }
        }

    }

    
}
