
using Plataforma.Educacao.Core.Extensions;
using SaberOnline.Aluno.Application.DTO;
using SaberOnline.Aluno.Application.Interfaces;
using SaberOnline.Aluno.Domain.Interfaces;
using SaberOnline.Aluno.Domain.ValueObjects;
using SaberOnline.Core.SharedDto;

namespace SaberOnline.Aluno.Application.Queries;
public class AlunoQueryService(IAlunoRepository alunoRepository) : IAlunoQueryService
{
    private readonly IAlunoRepository _alunoRepository = alunoRepository;

    public async Task<AlunoDto> ObterAlunoPorIdAsync(Guid alunoId)
    {
        var aluno = await _alunoRepository.ObterPorIdAsync(alunoId);
        if (aluno == null) return null;

        return new AlunoDto
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Email = aluno.Email,
            DataNascimento = aluno.DataNascimento,
            MatriculasCursos = aluno.MatriculasCursos != null ? aluno.MatriculasCursos.Select(m => new MatriculaCursoDto
            {
                Id = m.Id,
                CursoId = m.CursoId,
                NomeCurso = m.NomeCurso,
                Valor = m.Valor,
                DataMatricula = m.DataMatricula,
                DataConclusao = m.DataConclusao,
                EstadoMatricula = m.EstadoMatricula.GetDescription(),
                Certificado = m.Certificado != null ? new CertificadoDto
                {
                    Id = m.Certificado.Id,
                    DataSolicitacao = m.Certificado.DataSolicitacao,
                    PathCertificado = m.Certificado.PathCertificado,
                } : null
            }).ToList() : []
        };
    }

    public async Task<EvolucaoAlunoDto> ObterEvolucaoMatriculasCursoDoAlunoPorIdAsync(Guid alunoId)
    {
        var aluno = await _alunoRepository.ObterPorIdAsync(alunoId);
        if (aluno == null) return null;

        return new EvolucaoAlunoDto
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Email = aluno.Email,
            DataNascimento = aluno.DataNascimento,
            MatriculasCursos = aluno.MatriculasCursos != null ? aluno.MatriculasCursos.Select(m => new EvolucaoMatriculaCursoDto
            {
                Id = m.Id,
                CursoId = m.CursoId,
                NomeCurso = m.NomeCurso,
                Valor = m.Valor,
                DataMatricula = m.DataMatricula,
                DataConclusao = m.DataConclusao,
                EstadoMatricula = m.EstadoMatricula.GetDescription(),
                //QuantidadeAulasNoCurso = cursos.FirstOrDefault(c => c.Id == m.CursoId)?.QuantidadeAulas ?? -1,
                QuantidadeAulasRealizadas = m.QuantidadeAulasFinalizadas,
                QuantidadeAulasEmAndamento = m.QuantidadeAulasEmAndamento,
                Certificado = m.Certificado != null ? new CertificadoDto
                {
                    Id = m.Certificado.Id,
                    DataSolicitacao = m.Certificado.DataSolicitacao,
                    PathCertificado = m.Certificado.PathCertificado,
                } : null
            }).ToList() : []
        };
    }

    public async Task<IEnumerable<MatriculaCursoDto>> ObterMatriculasPorAlunoIdAsync(Guid alunoId)
    {
        var aluno = await _alunoRepository.ObterPorIdAsync(alunoId);
        if (aluno == null) return [];

        return aluno.MatriculasCursos.Select(m => new MatriculaCursoDto
        {
            Id = m.Id,
            CursoId = m.CursoId,
            NomeCurso = m.NomeCurso,
            Valor = m.Valor,
            DataMatricula = m.DataMatricula,
            DataConclusao = m.DataConclusao,
            EstadoMatricula = m.EstadoMatricula.GetDescription(),
            Certificado = m.Certificado != null ? new CertificadoDto
            {
                Id = m.Certificado.Id,
                DataSolicitacao = m.Certificado.DataSolicitacao,
                PathCertificado = m.Certificado.PathCertificado,
            } : null
        });
    }

    public async Task<MatriculaCursoDto> ObterInformacaoMatriculaCursoAsync(Guid matriculaCursoId)
    {
        var matriculaCurso = await _alunoRepository.ObterMatriculaPorIdAsync(matriculaCursoId);
        if (matriculaCurso == null) return null;

        return new MatriculaCursoDto
        {
            Id = matriculaCurso.Id,
            AlunoId = matriculaCurso.AlunoId,
            CursoId = matriculaCurso.CursoId,
            NomeCurso = matriculaCurso.NomeCurso,
            Valor = matriculaCurso.Valor,
            PagamentoPodeSerRealizado = matriculaCurso.PagamentoPodeSerRealizado,
            DataMatricula = matriculaCurso.DataMatricula,
            DataConclusao = matriculaCurso.DataConclusao,
            EstadoMatricula = matriculaCurso.EstadoMatricula.GetDescription(),
            Certificado = matriculaCurso.Certificado != null ? new CertificadoDto
            {
                Id = matriculaCurso.Certificado.Id,
                DataSolicitacao = matriculaCurso.Certificado.DataSolicitacao,
                PathCertificado = matriculaCurso.Certificado.PathCertificado,
            } : null
        };
    }

    public async Task<CertificadoDto> ObterCertificadoPorMatriculaIdAsync(Guid matriculaCursoId)
    {
        var matricula = await _alunoRepository.ObterMatriculaPorIdAsync(matriculaCursoId);
        if (matricula == null || matricula.Certificado == null) return null;

        return new CertificadoDto
        {
            Id = matricula.Certificado.Id,
            DataSolicitacao = matricula.Certificado.DataSolicitacao,
            PathCertificado = matricula.Certificado.PathCertificado
        };
    }

    public async Task<IEnumerable<AulaCursoDto>> ObterAulasPorMatriculaIdAsync(Guid matriculaCursoId, CursoDto cursoDto)
    {
        var matricula = await _alunoRepository.ObterMatriculaPorIdAsync(matriculaCursoId);
        if (matricula == null) return null;

        var retorno = new List<AulaCursoDto>();
        foreach (var aula in cursoDto.Aulas)
        {
            HistoricoAprendizado historicoAprendizado = matricula.HistoricoAprendizado.FirstOrDefault(h => h.AulaId == aula.Id);

            retorno.Add(new AulaCursoDto
            {
                AulaId = aula.Id,
                CursoId = cursoDto.Id,
                NomeAula = historicoAprendizado?.NomeAula ?? aula.Descricao,
                OrdemAula = aula.OrdemAula,
                Ativo = aula.Ativo,
                DataInicio = historicoAprendizado?.DataInicio ?? null,
                DataTermino = historicoAprendizado?.DataInicio ?? null,
                Url = aula.Url
            });
        }

        foreach (var aula in matricula.HistoricoAprendizado)
        {
            if (!retorno.Any(a => a.AulaId == aula.AulaId))
            {
                retorno.Add(new AulaCursoDto
                {
                    AulaId = aula.AulaId,
                    CursoId = aula.CursoId,
                    NomeAula = aula.NomeAula,
                    OrdemAula = 0,
                    Ativo = false,
                    DataInicio = aula.DataInicio,
                    DataTermino = aula.DataTermino,
                    Url = null
                });
            }
        }

        return retorno.OrderBy(x => x.OrdemAula).ToList();
    }
}
