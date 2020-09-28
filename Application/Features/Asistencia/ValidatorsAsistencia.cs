using Application.Features.Asistencia.Catalogo.Commands;
using Application.Features.Asistencia.Catalogo.Queries;
using Application.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Asistencia
{
    public class ValidatorsAsistencia
    {

        private static readonly string PageNumberMessage = "PageNumber at least greater than or equal to 1.";
        private static readonly string PageSizeMessage = "PageSizeMessage at least greater than or equal to 1.";

        public class GetAllCatalogoAsistenciaQueryValidator : AbstractValidator<GetAllCatalogoAsistenciaQuery>
        {
            public GetAllCatalogoAsistenciaQueryValidator()
            {
                RuleFor(T => T.PageNumber).GreaterThanOrEqualTo(1).WithMessage(PageNumberMessage);
                RuleFor(T => T.PageSize).GreaterThanOrEqualTo(1).WithMessage(PageSizeMessage);
            }

        }

        //public class CreateCatalogoAsistenciaCommandValidator : AbstractValidator<CreateCatalogoAsistenciaCommand>
        //{

        //    public CreateCatalogoAsistenciaCommandValidator()
        //    {

        //        RuleFor(T => T.Descripcion).NotEmpty().WithMessage("No empty.")
        //                                   .MaximumLength(200).WithMessage("Not exceed 200 characters.")
        //                                   .MustAsync(UniqueDescription).WithMessage("That Descripcion exist.");
        //    }
            
        //}


        //public async Task<bool> UniqueDescription(string description, CancellationToken cancellationToken)
        //{
        //    return await context.CatalogoAsistencia.AllAsync(T => T.Descripcion != description);
        //}





    }
}
