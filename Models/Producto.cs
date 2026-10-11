using System.Globalization;

namespace BluePhone.Models
{
    // Clase simple para llevar los datos de un producto desde el DataReader hasta la vista.
    public class Producto
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = "";
        public decimal Precio { get; set; }
        public int IdCategoria { get; set; }
        public string Categoria { get; set; } = "";

        // Precio listo para mostrar, ej: "U$S 1.199,00"
        public string PrecioTexto => "U$S " + Precio.ToString("N2", new CultureInfo("es-AR"));
    }
}
