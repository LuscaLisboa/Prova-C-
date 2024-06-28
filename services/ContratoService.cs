using Microsoft.EntityFrameworkCore;
using prova.data;
using prova.models;

namespace prova.services
{
    public class ContratoService
    {
        private readonly ProvaDbContext _dbContext;
        public ContratoService(ProvaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Gravar novo contrato
        public async Task AddContratoAsync(Contrato contrato)
        {
            _dbContext.Contratos.Add(contrato);
            await _dbContext.SaveChangesAsync();
        }

        // Obter contratos por ClienteId
        public async Task<List<Servico>> GetServicosByClienteIdAsync(int clienteId)
        {
            var contratos = await _dbContext.Contratos
                .Where(c => c.ClienteId == clienteId)
                .Include(c => c.Servico)
                .ToListAsync();

            return contratos.Select(c => c.Servico).ToList();
        }
    }
}

