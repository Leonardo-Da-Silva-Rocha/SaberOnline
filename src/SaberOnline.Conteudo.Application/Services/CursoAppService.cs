using AutoMapper;
using SaberOnline.Conteudo.Application.ViewModels;
using SaberOnline.Conteudo.Domain.Entities;
using SaberOnline.Conteudo.Domain.Interfaces;
using SaberOnline.Conteudo.Domain.ValueObjects;
using SaberOnline.Core.Exceptions;
using SaberOnline.Core.SharedDto;

namespace SaberOnline.Conteudo.Application.Services
{
    public class CursoAppService : ICursoAppService
    {
        private readonly IMapper _mapper;

        private readonly IConteudoRepository _cursoRepository;

        public CursoAppService(IMapper mapper, IConteudoRepository cursoRepository)
        {
            _mapper = mapper;
            _cursoRepository = cursoRepository;
        }

        public async Task<Guid> CadastrarCursoAsync(CadastroCursoDto dto)

        {
            if (await _cursoRepository.ExisteCursoComMesmoNomeAsync(dto.Nome)) { throw new DomainException("Já existe um curso cadastrado com esse nome."); }

            var curso = new Curso(dto.Nome,
                dto.Valor,
                dto.ValidoAte,
                new ConteudoProgramatico(dto.Finalidade, dto.Ementa));

            await _cursoRepository.AdicionarAsync(curso);
            await _cursoRepository.UnitOfWork.Commit();
            return curso.Id;
        }

        public async Task AtualizarCursoAsync(Guid cursoId, AtualizacaoCursoDto dto)
        {
            var curso = await _cursoRepository.ObterPorIdAsync(cursoId) ?? throw new DomainException("Curso não encontrado");

            if (!curso.Nome.Equals(dto.Nome, StringComparison.OrdinalIgnoreCase) && await _cursoRepository.ExisteCursoComMesmoNomeAsync(dto.Nome))
            {
                throw new DomainException("Já existe um curso cadastrado com esse nome.");
            }

            curso.AlterarNome(dto.Nome);
            curso.AlterarValor(dto.Valor);
            curso.AlterarValidadeCurso(dto.ValidoAte);
            //curso.AlterarConteudoProgramatico(new ConteudoProgramatico(dto.Finalidade, dto.Ementa));
            curso.AtualizarConteudoProgramatico(dto.Finalidade, dto.Ementa);
            if (dto.Ativo) { curso.AtivarCurso(); }
            else { curso.DesativarCurso(); }

            await _cursoRepository.AtualizarAsync(curso);
            await _cursoRepository.UnitOfWork.Commit();
        }

        public async Task DesativarCursoAsync(Guid cursoId)
        {
            var curso = await _cursoRepository.ObterPorIdAsync(cursoId) ?? throw new DomainException("Curso não encontrado");

            curso.DesativarCurso();
            await _cursoRepository.AtualizarAsync(curso);
            await _cursoRepository.UnitOfWork.Commit();
        }

        public async Task<CursoDto> ObterPorIdAsync(Guid cursoId)
        {
            var curso = await _cursoRepository.ObterPorIdAsync(cursoId) ?? throw new DomainException("Curso não encontrado");
            return MapearParaCursoDto(curso);
        }

        public async Task<IEnumerable<CursoDto>> ObterTodosAsync()
        {
            var cursos = await _cursoRepository.ObterTodosAsync();
            return cursos.Select(MapearParaCursoDto);
        }

        public async Task<IEnumerable<CursoDto>> ObterAtivosAsync()
        {
            var cursos = await _cursoRepository.ObterAtivosAsync();
            return cursos.Select(MapearParaCursoDto);
        }

        #region Mapeamento Manual
        private CursoDto MapearParaCursoDto(Curso curso)
        {
            return new CursoDto
            {
                Id = curso.Id,
                Nome = curso.Nome,
                Valor = curso.Valor,
                Finalidade = curso.ConteudoProgramatico.Finalidade,
                Ementa = curso.ConteudoProgramatico.Ementa,
                CursoDisponivel = curso.CursoDisponivel(),
                CargaHoraria = curso.CargaHoraria(),
                QuantidadeAulas = curso.QuantidadeAulas(),
                Aulas = curso.Aulas.Select(a => new AulaDto
                {
                    Id = a.Id,
                    Descricao = a.Descricao,
                    Ativo = a.Ativo,
                    CargaHoraria = a.CargaHoraria,
                    OrdemAula = a.OrdemAula,
                    Url = a.Url
                }).OrderBy(x => x.OrdemAula)
                  .ToList()
            };
        }

        #endregion
    }
}
