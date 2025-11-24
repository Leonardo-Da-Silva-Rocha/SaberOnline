using AutoMapper;
using SaberOnline.Conteudo.Application.ViewModels;
using SaberOnline.Conteudo.Domain.Entities;

namespace SaberOnline.Conteudo.Application.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Curso, CursoViewModel>();
        }
    }
}
