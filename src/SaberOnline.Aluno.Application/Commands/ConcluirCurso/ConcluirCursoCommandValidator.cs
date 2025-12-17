using FluentValidation;
using SaberOnline.Core.Messages.Comunications.AlunoCommands;

namespace SaberOnline.Aluno.Application.Commands.ConcluirCurso;
public class ConcluirCursoCommandValidator : AbstractValidator<ConcluirCursoCommand>
{
    public ConcluirCursoCommandValidator()
    {
        RuleFor(c => c.AlunoId).NotEqual(Guid.Empty).WithMessage("Id do aluno inválido.");
        RuleFor(c => c.MatriculaCursoId).NotEqual(Guid.Empty).WithMessage("Id da matrícula inválido.");
        //RuleFor(c => c.CursoDto.CursoDisponivel).NotEqual(false).WithMessage("Curso precisa estar disponível");
        RuleFor(c => c.CursoDto).NotNull().WithMessage("Curso precisa ser informado");
    }
}
