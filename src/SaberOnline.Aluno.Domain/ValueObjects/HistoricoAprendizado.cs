using SaberOnline.Core.DomainValidations;

namespace SaberOnline.Aluno.Domain.ValueObjects
{
    public class HistoricoAprendizado
    {
        #region Atributos
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid MatriculaCursoId { get; private set; }
        public Guid CursoId { get; }
        public Guid AulaId { get; }
        public string NomeAula { get; }
        public DateTime DataInicio { get; }
        public DateTime? DataTermino { get; }

        protected HistoricoAprendizado() { }
        public HistoricoAprendizado(Guid matriculaCursoId, Guid cursoId, Guid aulaId, string nomeAula, DateTime? dataTermino = null)
        {
            MatriculaCursoId = matriculaCursoId;
            CursoId = cursoId;
            AulaId = aulaId;
            NomeAula = nomeAula;
            DataInicio = DateTime.Now.Date;
            DataTermino = dataTermino;

            ValidarIntegridadeHistoricoAprendizado();
        }
        #endregion

        #region Métodos 
        private void ValidarIntegridadeHistoricoAprendizado()
        {
            var validacao = new ResultadoValidacao<HistoricoAprendizado>();

            ValidacaoGuid.DeveSerValido(CursoId, "Identifição do curso não pode ser vazio", validacao);
            ValidacaoGuid.DeveSerValido(AulaId, "Identifição da aula não pode ser vazio", validacao);
            ValidacaoTexto.DevePossuirConteudo(NomeAula, "Nome da aula não pode ser vazio", validacao);
            ValidacaoTexto.DevePossuirTamanho(NomeAula, 5, 100, "Nome da aula deve ter entre 5 e 100 caracteres", validacao);
            ValidacaoData.DeveSerValido(DataInicio, "Data de início é inválida", validacao);
            ValidacaoData.DeveSerMenorQue(DataInicio, DateTime.Now, "Data de início não pode ser superior à data atual", validacao);

            if (DataTermino.HasValue)
            {
                ValidacaoData.DeveSerValido(DataTermino.Value, "Data de término é inválida", validacao);
                ValidacaoData.DeveSerMaiorQue(DataTermino.Value, DataInicio, "Data de término não pode ser menor que a data de início", validacao);
                ValidacaoData.DeveSerMenorQue(DataTermino.Value, DateTime.Now, "Data de término não pode ser superior à data atual", validacao);
            }

            validacao.DispararExcecaoDominioSeInvalido();
        }

        public override string ToString()
        {
            string conclusao = DataTermino.HasValue ? $"(Término em {DataTermino:dd/MM/yyyy})" : "(Em andamento)";
            return $"Aula {NomeAula} Iniciada em {DataInicio:dd/MM/yyyy} {conclusao}";
        }
        #endregion
    }
}
