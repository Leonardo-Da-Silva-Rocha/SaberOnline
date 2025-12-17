using SaberOnline.Core.Data;
using SaberOnline.Faturamento.Domain.Entities;

namespace SaberOnline.Faturamento.Domain.Interfaces;
public interface IFaturamentoRepository : IRepository<Pagamento>
{
    Task AdicionarAsync(Pagamento pagamento);
    Task AtualizarAsync(Pagamento pagamento);
    Task<Pagamento> ObterPorMatriculaIdAsync(Guid matriculaId);

}
