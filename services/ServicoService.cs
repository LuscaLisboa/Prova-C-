using Microsoft.EntityFrameworkCore;
using prova.data;
using prova.models;

namespace prova.services
{
    public class ServicoService
    {
        private readonly ProvaDbContext _dbContext;
        public ServicoService(ProvaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Obter um serviço pelo ID
        public async Task<Servico> GetServicoByIdAsync(int id)
        {
            return await _dbContext.Servicos.FindAsync(id);
        }

        // Gravar novo serviço
        public async Task AddServicoAsync(Servico servico)
        {
            _dbContext.Servicos.Add(servico);
            await _dbContext.SaveChangesAsync();
        }

        // Atualizar um serviço
        public async Task UpdateServicoAsync(Servico servico)
        {
            _dbContext.Entry(servico).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}