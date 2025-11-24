using FluentValidation;
using SaberOnline.Core.Messages.AlunoEvents;

namespace SaberOnline.Aluno.Application.ProblemaRegistroHistoricoAprendizado;
public class RegistrarProblemaHistoricoAprendizadoEventValidator : AbstractValidator<RegistrarProblemaHistoricoAprendizadoEvent>
{
    public RegistrarProblemaHistoricoAprendizadoEventValidator()
    {
        RuleFor(c => c.AlunoId).NotEqual(Guid.Empty).WithMessage("Id do aluno é inválido");
        RuleFor(c => c.MatriculaCursoId).NotEqual(Guid.Empty).WithMessage("Id de matrícula do curso do aluno é inválida");
        RuleFor(c => c.AulaId).NotEqual(Guid.Empty).WithMessage("Id da aula é inválida");
    }
}
