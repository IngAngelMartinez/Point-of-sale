using AutoMapper;
using Application.Features.Asistencia.Catalogo.Commands;
using Domain.Entities;
using Application.DTOs.CatalogoAsistencia;

namespace Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            //CreateMap<CatalogoAsistencia, CatalogoAsistenciaDTO>().ForMember("Description", T => T.MapFrom(A => A.Descripcion));
            CreateMap<CreateCatalogoAsistenciaCommand, CatalogoAsistencia>();
            CreateMap<UpdateCatalogoAsistenciaCommand, CatalogoAsistencia>();
            CreateMap<CatalogoAsistencia, CatalogoAsistenciaDTO>();


        }
    }
}
