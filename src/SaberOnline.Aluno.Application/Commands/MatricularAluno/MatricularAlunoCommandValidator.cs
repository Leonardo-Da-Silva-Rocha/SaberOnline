
using FluentValidation;
using SaberOnline.Core.Messages.AlunoCommands;

namespace SaberOnline.Aluno.Application.Commands.MatricularAluno;
public class MatricularAlunoCommandValidator : AbstractValidator<MatricularAlunoCommand>
{
    public MatricularAlunoCommandValidator()
    {
        RuleFor(c => c.AlunoId).NotEqual(Guid.Empty).WithMessage("Id do aluno inválido.");
        RuleFor(c => c.CursoId).NotEqual(Guid.Empty).WithMessage("Id do curso inválido.");
        RuleFor(c => c.CursoDisponivel).NotEqual(false).WithMessage("Curso precisa estar disponível para matricular o Aluno");
        RuleFor(c => c.NomeCurso)
            .NotEmpty().WithMessage("Nome do curso não pode ser vazio")
            .NotNull().WithMessage("Nome do curso não pode ser nulo");
        RuleFor(c => c.ValorCurso).NotEqual(0.00m).WithMessage("Curso precisa ter o seu valor informado");
    }
}
