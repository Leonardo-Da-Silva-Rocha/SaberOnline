using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaberOnline.API.Enumerators;
using SaberOnline.API.ViewModels;
using SaberOnline.Core.SharedDto;
using System.Net;

namespace SaberOnline.API.Controllers.Aluno;

public partial class AlunoController
{
    
    [HttpGet("{id}")]
    public async Task<IActionResult> ObterAlunoPorId(Guid id)
    {
        var aluno = await _alunoQueryService.ObterAlunoPorIdAsync(id);
        if (aluno == null) { return GenerateResponse(null, ResponseTypeEnum.NotFound, HttpStatusCode.NotFound); }

        return GenerateResponse(_mapper.Map<AlunoViewModel>(aluno));
    }

    
    [HttpGet("{id}/evolucao")]
    public async Task<IActionResult> ObterEvolucaoMatriculasCursoDoAlunoPorIdAsync(Guid id)
    {
        var aluno = await _alunoQueryService.ObterEvolucaoMatriculasCursoDoAlunoPorIdAsync(id);
        if (aluno == null) { return GenerateResponse(null, ResponseTypeEnum.NotFound, HttpStatusCode.NotFound); }

        return GenerateResponse(_mapper.Map<EvolucaoAlunoViewModel>(aluno));
    }

   
    [HttpGet("{id}/todas-matriculas")]
    public async Task<IActionResult> ObterMatriculasPorAlunoId(Guid id)
    {
        var matriculas = await _alunoQueryService.ObterMatriculasPorAlunoIdAsync(id);
        if (matriculas == null || !matriculas.Any()) { return GenerateResponse(null, ResponseTypeEnum.NotFound, HttpStatusCode.NotFound); }

        return GenerateResponse(_mapper.Map<IEnumerable<MatriculaCursoViewModel>>(matriculas));
    }

   
    [HttpGet("matricula/{matriculaId}/certificado")]
    public async Task<IActionResult> ObterCertificadoPorMatriculaId(Guid matriculaId)
    {
        var certificado = await _alunoQueryService.ObterCertificadoPorMatriculaIdAsync(matriculaId);
        if (certificado == null) { return GenerateResponse(null, ResponseTypeEnum.NotFound, HttpStatusCode.NotFound); }

        return GenerateResponse(_mapper.Map<CertificadoViewModel>(certificado));
    }

    
    [HttpGet("aulas/{matriculaId}")]
    public async Task<IActionResult> ObterAulasPorMatriculaId(Guid matriculaId)
    {
        var matriculaCurso = await _alunoQueryService.ObterInformacaoMatriculaCursoAsync(matriculaId);
        if (matriculaCurso == null) { return GenerateResponse(null, ResponseTypeEnum.NotFound, HttpStatusCode.NotFound, ["Matrícula não encontrada"]); }

        CursoDto cursoDto = await _cursoAppService.ObterPorIdAsync(matriculaCurso.CursoId);
        if (cursoDto == null) { return GenerateResponse(null, ResponseTypeEnum.NotFound, HttpStatusCode.NotFound, ["Curso desta matrícula não encontrada"]); }

        var aulas = await _alunoQueryService.ObterAulasPorMatriculaIdAsync(matriculaId, cursoDto);
        if (aulas == null) { return GenerateResponse(null, ResponseTypeEnum.NotFound, HttpStatusCode.NotFound); }

        return GenerateResponse(_mapper.Map<IEnumerable<AulaCursoViewModel>>(aulas));
    }
}
