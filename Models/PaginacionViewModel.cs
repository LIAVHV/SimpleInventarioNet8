namespace SimpleInventario.Web.Models
{
    public class PaginacionViewModel
    {
        public int PaginaIndice { get; set; }
        public int TotalPaginas { get; set; }
        public int ItemsPorPagina { get; set; }
        public string Modelo { get; set; } = "";
        public string? Controlador { get; set; }
        public string Accion { get; set; } = "Index";
        public string? Query { get; set; }
        public int? ProductoId { get; set; }
        public string? Desde { get; set; }
        public string? Hasta { get; set; }
    }
}
