using FluentValidation;
using SaberOnline.Core.Messages.FaturamentoEvents;


namespace SaberOnline.Aluno.Application.PagamentoRecusado;

internal class PagamentoRecusadoEventValidator : AbstractValidator<PagamentoRecusadoEvent>
{
    public PagamentoRecusadoEventValidator()
    {
        RuleFor(c => c.MatriculaCursoId).NotEqual(Guid.Empty).WithMessage("Matrícula do aluno é inválida");
        RuleFor(c => c.AlunoId).NotEqual(Guid.Empty).WithMessage("Id do aluno é inválido");
        RuleFor(c => c.CursoId).NotEqual(Guid.Empty).WithMessage("Id do curso é inválido");
    }
}
