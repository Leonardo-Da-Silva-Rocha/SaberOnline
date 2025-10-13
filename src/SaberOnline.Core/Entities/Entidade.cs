using SaberOnline.Core.Exceptions;

namespace SaberOnline.Core.Entities
{
    public class Entidade
    {
        public Guid Id { get; internal set; }

        protected Entidade()
        {
            Id = Guid.NewGuid();
        }

        public void DefinirId(Guid id)
        {
            if (id == Guid.Empty) { throw new DomainException("Id não pode ser vazio"); }
            Id = id;
        }

        public override bool Equals(object obj)
        {
            var objeto = (obj as Entidade);

            if (ReferenceEquals(this, objeto)) return true;
            if (ReferenceEquals(null, objeto)) return false;

            return Id.Equals(objeto.Id);
        }

        public static bool operator ==(Entidade a, Entidade b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entidade a, Entidade b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetType().GetHashCode() * (new Random().Next(1, int.MaxValue))) + Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{GetType().Name} [Id={Id}]";
        }
    }
}
