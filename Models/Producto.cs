using System.Globalization;

namespace BluePhone.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = "";
        public decimal Precio { get; set; }
        public int IdCategoria { get; set; }
        public string Categoria { get; set; } = "";

        public string PrecioTexto => "U$S " + Precio.ToString("N2", new CultureInfo("es-AR"));
    }
}
