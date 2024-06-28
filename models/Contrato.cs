namespace prova.models
{
    public class Contrato
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int ServicoId { get; set; }
        public decimal PrecoCobrado { get; set; }
        public DateTime DataContratacao { get; set; }
        public Servico Servico { get; set; }
    }
}
