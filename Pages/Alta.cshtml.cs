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

        public AltaModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty] public string Nombre { get; set; } = "";
        [BindProperty] public string Precio { get; set; } = "";
        [BindProperty] public int IdCategoria { get; set; }

        public List<SelectListItem> Categorias { get; set; } = new();

        public string Error { get; set; } = "";

        [TempData] public string? MensajeOk { get; set; }

        public void OnGet()
        {
            CargarCategorias();
        }

        public IActionResult OnPost()
        {
            CargarCategorias();

            if (string.IsNullOrWhiteSpace(Nombre))
            {
                Error = "Ingresá el nombre del producto.";
                return Page();
            }

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
