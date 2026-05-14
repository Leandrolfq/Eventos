namespace Eventos.Models
{
    public class EventoCategoria
    {
    public int EventoId { get; set; }
    public Evento? Evento { get; set; }
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
    }
}