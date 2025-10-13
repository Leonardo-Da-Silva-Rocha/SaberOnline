namespace SaberOnline.Core.Exceptions
{
    public class DomainException : Exception
    {
        public IReadOnlyCollection<string> Errors { get; }

        public DomainException(string mensagem) : base(mensagem)
        {
            Errors = [mensagem];
        }

        public DomainException(IEnumerable<string> mensagens) : base(string.Join("; ", mensagens))
        {
            Errors = mensagens.ToList().AsReadOnly();
        }
    }
}
