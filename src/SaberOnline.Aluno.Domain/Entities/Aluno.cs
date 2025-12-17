using SaberOnline.Aluno.Domain.ValueObjects;
using SaberOnline.Core.Agregrates;
using SaberOnline.Core.DomainValidations;
using SaberOnline.Core.Entities;
using SaberOnline.Core.Exceptions;

namespace SaberOnline.Aluno.Domain.Entities
{
    public class Aluno : Entidade, IRaizAgregacao
    {
        #region Atributos
        public Guid CodigoUsuarioAutenticacao { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public DateTime DataNascimento { get; private set; }

        private readonly List<MatriculaCurso> _matriculasCursos = [];
        public IReadOnlyCollection<MatriculaCurso> MatriculasCursos => _matriculasCursos.AsReadOnly();

        protected Aluno() { }

        public Aluno(string nome, string email, DateTime dataNascimento)
        {
            Nome = nome;
            Email = email;
            DataNascimento = dataNascimento;

            ValidarIntegridadeAluno();
        }
        #endregion

        #region Metodos
        public MatriculaCurso ObterMatriculaPorCursoId(Guid cursoId)
        {
            var matriculaCurso = _matriculasCursos.FirstOrDefault(m => m.CursoId == cursoId);
            if (matriculaCurso == null) { throw new DomainException("Matrícula pelo Curso não foi localizada"); }

            return matriculaCurso;
        }

        public MatriculaCurso ObterMatriculaCursoPeloId(Guid matriculaCursoId)
        {
            var matriculaCurso = _matriculasCursos.FirstOrDefault(m => m.Id == matriculaCursoId);
            if (matriculaCurso == null) { throw new DomainException("Matrícula não foi localizada"); }

            return matriculaCurso;
        }

        public HistoricoAprendizado ObterHistoricoAprendizado(Guid matriculaCursoId, Guid aulaId)
        {
            var matriculaCurso = ObterMatriculaCursoPeloId(matriculaCursoId);
            var historico = matriculaCurso.HistoricoAprendizado.FirstOrDefault(h => h.AulaId == aulaId);
            
            return historico;
        }

        public int ObterQuantidadeAulasMatriculaCurso(Guid cursoId)
        {
            return _matriculasCursos.Count(m => m.CursoId == cursoId);
        }

        public int ObterQuantidadeAulasPendenteMatriculaCurso(Guid cursoId)
        {
            return _matriculasCursos.Count(m => m.CursoId == cursoId && m.PodeFinalizarMatriculaCurso == false);
        }

        private bool AlunoJaMatriculado(Guid cursoId)
        {
            return _matriculasCursos.Any(m => m.CursoId == cursoId);
        }

        #region Manipuladores do Aluno
        public void IdentificarCodigoUsuarioNoSistema(Guid codigoUsuarioAutenticacao)
        {
            if (codigoUsuarioAutenticacao == Guid.Empty) { throw new DomainException("Código de autenticação não pode ser vazio"); }
            if (CodigoUsuarioAutenticacao != Guid.Empty) { throw new DomainException("Código de autenticação já foi definido"); }

            DefinirId(codigoUsuarioAutenticacao);
            CodigoUsuarioAutenticacao = codigoUsuarioAutenticacao;
        }

        public void AtualizarNome(string nome)
        {
            ValidarIntegridadeAluno(novoNome: nome ?? string.Empty);
            Nome = nome;
        }

        public void AtualizarEmail(string email)
        {
            ValidarIntegridadeAluno(novoEmail: email ?? string.Empty);
            Email = email;
        }

        public void AtualizarDataNascimento(DateTime dataNascimento)
        {
            ValidarIntegridadeAluno(novaDataNascimento: dataNascimento);
            DataNascimento = dataNascimento;
        }
        #endregion

        #region Manipuladores de MatriculaCurso
        public void MatricularEmCurso(Guid cursoId, string nomeCurso, decimal valor)
        {
            if (AlunoJaMatriculado(cursoId)) { throw new DomainException("Aluno já está matriculado neste curso"); }

            var novaMatricula = new MatriculaCurso(Id, cursoId, nomeCurso, valor);
            _matriculasCursos.Add(novaMatricula);
        }

        public void AtualizarPagamentoMatricula(Guid matriculaCursoId)
        {
            MatriculaCurso matriculaCurso = ObterMatriculaCursoPeloId(matriculaCursoId);
            matriculaCurso.AtualizarPagamentoMatricula();
        }

        public void AtualizarAbandonoMatricula(Guid matriculaCursoId)
        {
            MatriculaCurso matriculaCurso = ObterMatriculaCursoPeloId(matriculaCursoId);
            matriculaCurso.AtualizarAbandonoMatricula();
        }

        public void ConcluirCurso(Guid matriculaCursoId)
        {
            MatriculaCurso matriculaCurso = ObterMatriculaCursoPeloId(matriculaCursoId);
            matriculaCurso.ConcluirCurso();
        }
        #endregion

        #region Manipuladores de Certificado
        public void RequisitarCertificadoConclusao(Guid matriculaCursoId, string pathCertificado)
        {
            MatriculaCurso matriculaCurso = ObterMatriculaCursoPeloId(matriculaCursoId);
            matriculaCurso.RequisitarCertificadoConclusao(pathCertificado);
        }
        #endregion

        #region Manipuladores de HistoricoAprendizado
        public void RegistrarHistoricoAprendizado(Guid matriculaCursoId, Guid aulaId, string nomeAula, DateTime? dataTermino = null)
        {
            MatriculaCurso matriculaCurso = ObterMatriculaCursoPeloId(matriculaCursoId);
            matriculaCurso.RegistrarHistoricoAprendizado(aulaId, nomeAula, dataTermino);
        }
        #endregion

        private void ValidarIntegridadeAluno(string novoNome = null, string novoEmail = null, DateTime? novaDataNascimento = null)
        {
            var nome = novoNome ?? Nome;
            var email = novoEmail ?? Email;
            var dataNascimento = novaDataNascimento ?? DataNascimento;

            var validacao = new ResultadoValidacao<Aluno>();

            ValidacaoTexto.DevePossuirConteudo(nome, "Nome não pode ser nulo ou vazio", validacao);
            ValidacaoTexto.DevePossuirTamanho(nome, 3, 50, "Nome deve ter entre 3 e 50 caracteres", validacao);
            ValidacaoTexto.DevePossuirConteudo(email, "Email não pode ser nulo ou vazio", validacao);
            ValidacaoTexto.DevePossuirTamanho(email, 3, 100, "Email deve ter entre 3 e 100 caracteres", validacao);
            ValidacaoTexto.DeveAtenderRegex(email, @"^[\w\.\-]+@([\w\-]+\.)+[\w\-]{2,}$", "Email informado é inválido", validacao);
            ValidacaoData.DeveSerValido(dataNascimento, "Data de nascimento deve ser válida", validacao);
            ValidacaoData.DeveSerMenorQue(dataNascimento, DateTime.Now, "Data de nascimento não pode ser superior à data atual", validacao);

            validacao.DispararExcecaoDominioSeInvalido();
        }

        public override string ToString() => $"{Nome} (Email: {Email})";
        #endregion
    }
}
