using SaberOnline.Core.DomainValidations;
using SaberOnline.Core.Entities;
using System.Text.Json.Serialization;

namespace SaberOnline.Aluno.Domain.Entities
{
    public class Certificado : Entidade
    {
        #region Atributos
        public Guid MatriculaCursoId { get; private set; }
        public DateTime DataSolicitacao { get; private set; }
        public string PathCertificado { get; private set; }

        [JsonIgnore]
        public MatriculaCurso MatriculaCurso { get; private set; }

        protected Certificado() { }

        public Certificado(Guid matriculaCursoId, string pathCertificado)
        {
            MatriculaCursoId = matriculaCursoId;
            DataSolicitacao = DateTime.Now;
            PathCertificado = pathCertificado;

            ValidarIntegridadeCertificado();
        }
        #endregion

        #region Metodos
        private void ValidarIntegridadeCertificado()
        {
            var validacao = new ResultadoValidacao<Certificado>();

            ValidacaoGuid.DeveSerValido(MatriculaCursoId, "Matrícula do curso deve ser informada", validacao);
            ValidacaoTexto.DevePossuirConteudo(PathCertificado, "Path do certificado não pode ser nulo ou vazio", validacao);
            ValidacaoTexto.DevePossuirTamanho(PathCertificado, 1, 1024, "Path do certificado deve ter no máximo 1024 caracteres", validacao);

            validacao.DispararExcecaoDominioSeInvalido();
        }

        public override string ToString() => $"Certificado da matrícula {MatriculaCursoId}, gerado em {DataSolicitacao:dd/MM/yyyy}";
        #endregion
    }
}
