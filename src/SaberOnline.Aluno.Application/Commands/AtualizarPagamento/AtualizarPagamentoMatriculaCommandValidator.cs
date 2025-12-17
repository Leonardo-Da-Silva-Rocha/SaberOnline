using FluentValidation;
using SaberOnline.Core.Messages.AlunoCommands;


namespace SaberOnline.Aluno.Application.Commands.AtualizarPagamento
{
    public class AtualizarPagamentoMatriculaCommandValidator : AbstractValidator<AtualizarPagamentoMatriculaCommand>
    {
        public AtualizarPagamentoMatriculaCommandValidator()
        {
            RuleFor(c => c.AlunoId).NotEqual(Guid.Empty).WithMessage("Id do aluno é inválido");
            RuleFor(c => c.CursoId).NotEqual(Guid.Empty).WithMessage("Id do curso é inválido");
            RuleFor(c => c.CursoDisponivel).NotEqual(false).WithMessage("Curso precisa estar disponível para realizar o pagamento");
        }
    }
}
