using Microsoft.EntityFrameworkCore;

namespace SimpleInventario.Web.Models
{
    public class Paginar<T> : List<T>
    {
        public int PaginaIndice { get; private set; }
        public int TotalPaginas { get; private set; }
        public int ItemsPorPagina { get; private set; }

        public Paginar(List<T> items, int count, int pageIndex, int itemsPorPagina)
        {
            PaginaIndice = pageIndex;
            ItemsPorPagina = itemsPorPagina;
            TotalPaginas = (int)Math.Ceiling(count / (double)ItemsPorPagina);
            this.AddRange(items);
        }

        public bool PagAnterior => PaginaIndice > 1;
        public bool PagSiguiente => PaginaIndice < TotalPaginas;

        public static async Task<Paginar<T>> PaginarEntidad(IQueryable<T> modelo, int? pageIndex, int? itemsPorPagina, CancellationToken ct = default)
        {
            int actualItemsPorPagina = Math.Clamp(itemsPorPagina ?? 10, 1, 100);
            int actualPageIndex = Math.Max(pageIndex ?? 1, 1);
            var count = await modelo.CountAsync(ct);
            var totalPaginas = (int)Math.Ceiling(count / (double)actualItemsPorPagina);
            if (totalPaginas > 0) actualPageIndex = Math.Min(actualPageIndex, totalPaginas);
            else actualPageIndex = 1;

            var items = await modelo
                .Skip((actualPageIndex - 1) * actualItemsPorPagina)
                .Take(actualItemsPorPagina)
                .ToListAsync(ct);

            return new Paginar<T>(items, count, actualPageIndex, actualItemsPorPagina);
        }
    }
}
