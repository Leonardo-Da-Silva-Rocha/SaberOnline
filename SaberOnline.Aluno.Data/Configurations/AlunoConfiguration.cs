using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaberOnline.Core.Constants;
using System.Diagnostics.CodeAnalysis;

namespace SaberOnline.Aluno.Data.Configurations
{
    [ExcludeFromCodeCoverage]
    public class AlunoConfiguration : IEntityTypeConfiguration<Domain.Entities.Aluno>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Aluno> builder)
        {
            
            builder.ToTable("Alunos");

            builder.HasKey(x => x.Id).HasName("AlunosPK");

            builder.Property(x => x.Id)
                   .HasColumnName("AlunoId")
                   .HasColumnType(DatabaseTypeConstant.UniqueIdentifier)
                   .IsRequired();

            builder.Property(x => x.CodigoUsuarioAutenticacao)
               .HasColumnName("CodigoUsuarioAutenticacao")
               .HasColumnType(DatabaseTypeConstant.UniqueIdentifier)
               .IsRequired();

            builder.Property(x => x.Nome)
                   .HasColumnName("Nome")
                   .HasColumnType(DatabaseTypeConstant.Varchar)
                   .HasMaxLength(50)
                   .UseCollation(DatabaseTypeConstant.Collate)
                   .IsRequired();

            builder.Property(x => x.Email)
                   .HasColumnName("Email")
                   .HasColumnType(DatabaseTypeConstant.Varchar)
                   .HasMaxLength(100)
                   .UseCollation(DatabaseTypeConstant.Collate)
                   .IsRequired();

            builder.Property(x => x.DataNascimento)
                   .HasColumnName("DataNascimento")
                   .HasColumnType(DatabaseTypeConstant.SmallDateTime)
                   .IsRequired();
        }
    }
}