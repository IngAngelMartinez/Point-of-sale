using Application.Features.Asistencia.Catalogo.Queries;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Asistencia.Catalogo.Validators
{
    public class GetAllCatalogoAsistenciaQueryValidator : AbstractValidator<GetAllCatalogoAsistenciaQuery>
    {
        public GetAllCatalogoAsistenciaQueryValidator()
        {
            const string PageNumberMessage = "PageNumber at least greater than or equal to 1.";
            const string PageSizeMessage = "PageSizeMessage at least greater than or equal to 1.";

            RuleFor(T => T.PageNumber).GreaterThanOrEqualTo(1).WithMessage(PageNumberMessage);
            RuleFor(T => T.PageSize).GreaterThanOrEqualTo(1).WithMessage(PageSizeMessage);
        }

    }
}
