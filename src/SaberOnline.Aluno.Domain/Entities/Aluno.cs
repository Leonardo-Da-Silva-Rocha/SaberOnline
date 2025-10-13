using SaberOnline.Core.Agregrates;
using SaberOnline.Core.Entities;
using SaberOnline.Core.Exceptions;

namespace SaberOnline.Aluno.Domain.Entities
{
    public class Aluno : Entidade, IRaizAgregacao
    {
        public Guid CodigoUsuarioAutenticacao { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public DateTime DataNascimento { get; private set; }

        protected Aluno() { }

        public Aluno(string nome, string email, DateTime dataNascimento)
        {
            Nome = nome;
            Email = email;
            DataNascimento = dataNascimento;
        }

        public void IdentificarCodigoUsuarioNoSistema(Guid codigoUsuarioAutenticacao)
        {
            if (codigoUsuarioAutenticacao == Guid.Empty) { throw new DomainException("Código de autenticação não pode ser vazio"); }
            if (CodigoUsuarioAutenticacao != Guid.Empty) { throw new DomainException("Código de autenticação já foi definido"); }

            DefinirId(codigoUsuarioAutenticacao);
            CodigoUsuarioAutenticacao = codigoUsuarioAutenticacao;
        }
    }
}
