using FluentValidation;
using SaberOnline.Core.Messages.Comunications.AlunoCommands;

namespace Plataforma.Educacao.Aluno.Application.Commands.RegistrarHistoricoAprendizado;
public class RegistrarHistoricoAprendizadoCommandValidator : AbstractValidator<RegistrarHistoricoAprendizadoCommand>
{
    public RegistrarHistoricoAprendizadoCommandValidator()
    {
        RuleFor(c => c.AlunoId).NotEqual(Guid.Empty).WithMessage("Id do aluno inválido.");
        RuleFor(c => c.MatriculaCursoId).NotEqual(Guid.Empty).WithMessage("Id da matrícula inválido.");
        RuleFor(c => c.AulaId).NotEqual(Guid.Empty).WithMessage("Id da aula inválido.");
        RuleFor(c => c.CursoDto).NotNull().WithMessage("Curso precisa ser informado.");
    }
}