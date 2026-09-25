namespace MarroquineriaMarcani.Models
{
    public class InsumoSeleccionadoDto
    {
        public int InsumoId { get; set; }
        public decimal CantidadRequerida { get; set; }
    }

    public class ProductoConInsumosViewModel
    {
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioVenta { get; set; }
        public int StockAFabricar { get; set; }
        public List<InsumoSeleccionadoDto> Insumos { get; set; } = new();
    }
}