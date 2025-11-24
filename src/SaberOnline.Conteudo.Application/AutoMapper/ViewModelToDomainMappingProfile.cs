using AutoMapper;
using SaberOnline.Conteudo.Application.ViewModels;
using SaberOnline.Conteudo.Domain.Entities;


namespace SaberOnline.Conteudo.Application.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<CursoViewModel, Curso>();
        }
    }
}