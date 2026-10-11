using System.Data;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace BluePhone.Pages
{
    public class AltaModel : PageModel
    {
        private readonly IConfiguration _config;

        // Inyección de dependencias: ASP.NET Core nos entrega IConfiguration (appsettings.json)
        public AltaModel(IConfiguration config)
        {
            _config = config;
        }

        // Controles del formulario enlazados con [BindProperty]
        [BindProperty] public string Nombre { get; set; } = "";
        [BindProperty] public string Precio { get; set; } = "";
        [BindProperty] public int IdCategoria { get; set; }

        // Datos para llenar el <select> de categorías
        public List<SelectListItem> Categorias { get; set; } = new();

        public string Error { get; set; } = "";

        // Sobrevive a la redirección (patrón Post-Redirect-Get)
        [TempData] public string? MensajeOk { get; set; }

        public void OnGet()
        {
            CargarCategorias();
        }

        // Se ejecuta al presionar "Confirmar"
        public IActionResult OnPost()
        {
            CargarCategorias();

            if (string.IsNullOrWhiteSpace(Nombre))
            {
                Error = "Ingresá el nombre del producto.";
                return Page();
            }

            // El precio se lee como texto y se convierte con cultura invariante,
            // aceptando coma o punto como separador decimal.
            if (!decimal.TryParse(Precio?.Replace(',', '.'), NumberStyles.Number,
                    CultureInfo.InvariantCulture, out decimal precio) || precio <= 0)
            {
                Error = "Ingresá un precio válido mayor a 0.";
                return Page();
            }

            if (IdCategoria <= 0)
            {
                Error = "Elegí una categoría.";
                return Page();
            }

            try
            {
                using (var cn = new SqlConnection(_config.GetConnectionString("BluePhone")))
                {
                    cn.Open();
                    var cmd = new SqlCommand(
                        "INSERT INTO productos (nombre, precio, idCategoria) VALUES (@nombre, @precio, @idCategoria)", cn);

                    cmd.Parameters.AddWithValue("@nombre", Nombre.Trim());

                    var pPrecio = cmd.Parameters.Add("@precio", SqlDbType.Decimal);
                    pPrecio.Precision = 10;
                    pPrecio.Scale = 2;
                    pPrecio.Value = precio;

                    cmd.Parameters.AddWithValue("@idCategoria", IdCategoria);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                Error = "No se pudo guardar el producto: " + ex.Message;
                return Page();
            }

            MensajeOk = $"Producto \"{Nombre.Trim()}\" guardado correctamente.";
            return RedirectToPage();
        }

        private void CargarCategorias()
        {
            Categorias = new List<SelectListItem>();

            using (var cn = new SqlConnection(_config.GetConnectionString("BluePhone")))
            {
                cn.Open();
                var cmd = new SqlCommand("SELECT idCategoria, descripcion FROM categorias ORDER BY descripcion", cn);
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Categorias.Add(new SelectListItem(dr.GetString(1), dr.GetInt32(0).ToString()));
                    }
                }
            }
        }
    }
}
