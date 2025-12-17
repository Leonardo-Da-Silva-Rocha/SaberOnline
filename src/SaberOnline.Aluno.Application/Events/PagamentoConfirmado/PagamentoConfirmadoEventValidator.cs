using FluentValidation;
using SaberOnline.Core.Messages.FaturamentoEvents;


namespace SaberOnline.Aluno.Application.AtualizarPagamento;
public class PagamentoConfirmadoEventValidator : AbstractValidator<PagamentoConfirmadoEvent>
{
    public PagamentoConfirmadoEventValidator()
    {
        RuleFor(c => c.MatriculaCursoId).NotEqual(Guid.Empty).WithMessage("Matrícula do aluno é inválida");
        RuleFor(c => c.AlunoId).NotEqual(Guid.Empty).WithMessage("Id do aluno é inválido");
        RuleFor(c => c.CursoId).NotEqual(Guid.Empty).WithMessage("Id do curso é inválido");
        RuleFor(c => c.CursoDisponivel).NotEqual(false).WithMessage("Curso deve estar disponível");
    }
}
