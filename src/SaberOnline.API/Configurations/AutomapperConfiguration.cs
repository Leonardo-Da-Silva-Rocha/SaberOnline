using AutoMapper;
using SaberOnline.Aluno.Application.DTO;
using SaberOnline.API.ViewModels;
using SaberOnline.Conteudo.Application.ViewModels;
using SaberOnline.Core.SharedDto;

namespace SaberOnline.API.Configurations;
public class AutomapperConfiguration : Profile
{
    public AutomapperConfiguration()
    {
        CreateMap<AulaViewModel, AulaDto>();
        CreateMap<CadastroCursoViewModel, CadastroCursoDto>();
        CreateMap<AtualizacaoCursoViewModel, AtualizacaoCursoDto>();

        CreateMap<AlunoDto, AlunoViewModel>();
        CreateMap<MatriculaCursoDto, MatriculaCursoViewModel>();
        CreateMap<CertificadoDto, CertificadoViewModel>();

        CreateMap<EvolucaoAlunoDto, EvolucaoAlunoViewModel>();
        CreateMap<EvolucaoMatriculaCursoDto, EvolucaoMatriculaCursoViewModel>();

        CreateMap<AulaCursoDto, AulaCursoViewModel>();
    }
}