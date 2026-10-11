using System.Data;
using System.Globalization;
using BluePhone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace BluePhone.Pages
{
    public class ModificacionModel : PageModel
    {
        private readonly IConfiguration _config;

        public ModificacionModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty] public int IdProducto { get; set; }
        [BindProperty] public string Nombre { get; set; } = "";
        [BindProperty] public string Precio { get; set; } = "";
        [BindProperty] public int IdCategoria { get; set; }

        public List<SelectListItem> Productos { get; set; } = new();
        public List<SelectListItem> Categorias { get; set; } = new();

        public bool HayProducto { get; set; }
        public string Error { get; set; } = "";

        [TempData] public string? MensajeOk { get; set; }

        public void OnGet(int? id)
        {
            if (id.HasValue)
            {
                Producto? p = ObtenerProducto(id.Value);
                if (p != null)
                {
                    IdProducto = p.IdProducto;
                    Nombre = p.Nombre;
                    Precio = p.Precio.ToString("0.00", CultureInfo.InvariantCulture);
                    IdCategoria = p.IdCategoria;
                    HayProducto = true;
                }
                else
                {
                    Error = "El producto seleccionado no existe.";
                }
            }

            CargarListas(HayProducto ? IdProducto : 0);
        }

        public IActionResult OnPostGuardar()
        {
            CargarListas(IdProducto);
            HayProducto = true;

            Producto? actual = ObtenerProducto(IdProducto);
            if (actual == null)
            {
                HayProducto = false;
                Error = "El producto ya no existe.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Nombre))
            {
                Error = "El nombre no puede estar vacío.";
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

            string nombre = Nombre.Trim();
            bool cambioNombre = nombre != actual.Nombre;
            bool cambioPrecio = precio != actual.Precio;
            bool cambioCategoria = IdCategoria != actual.IdCategoria;

            if (!cambioNombre && !cambioPrecio && !cambioCategoria)
            {
                Error = "No modificaste ningún campo. Cambiá al menos uno para guardar.";
                return Page();
            }

            try
            {
                using (var cn = new SqlConnection(_config.GetConnectionString("BluePhone")))
                {
                    cn.Open();
                    var cmd = new SqlCommand(
                        @"UPDATE productos
                          SET nombre = @nombre, precio = @precio, idCategoria = @idCategoria
                          WHERE idProducto = @id", cn);

                    cmd.Parameters.AddWithValue("@nombre", nombre);

                    var pPrecio = cmd.Parameters.Add("@precio", SqlDbType.Decimal);
                    pPrecio.Precision = 10;
                    pPrecio.Scale = 2;
                    pPrecio.Value = precio;

                    cmd.Parameters.AddWithValue("@idCategoria", IdCategoria);
                    cmd.Parameters.AddWithValue("@id", IdProducto);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                Error = "No se pudo modificar el producto: " + ex.Message;
                return Page();
            }

            MensajeOk = $"Producto \"{nombre}\" modificado correctamente.";
            return RedirectToPage(new { id = IdProducto });
        }

        private Producto? ObtenerProducto(int id)
        {
            using (var cn = new SqlConnection(_config.GetConnectionString("BluePhone")))
            {
                cn.Open();
                var cmd = new SqlCommand(
                    "SELECT idProducto, nombre, precio, idCategoria FROM productos WHERE idProducto = @id", cn);
                cmd.Parameters.AddWithValue("@id", id);

                using (var dr = cmd.ExecuteReader())
                {
                    if (!dr.Read()) return null;

                    return new Producto
                    {
                        IdProducto = dr.GetInt32(0),
                        Nombre = dr.GetString(1),
                        Precio = dr.GetDecimal(2),
                        IdCategoria = dr.GetInt32(3)
                    };
                }
            }
        }

        private void CargarListas(int productoSeleccionado)
        {
            Productos = new List<SelectListItem>();
            Categorias = new List<SelectListItem>();

            using (var cn = new SqlConnection(_config.GetConnectionString("BluePhone")))
            {
                cn.Open();

                var cmdProd = new SqlCommand("SELECT idProducto, nombre FROM productos ORDER BY nombre", cn);
                using (var dr = cmdProd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int idProd = dr.GetInt32(0);
                        Productos.Add(new SelectListItem
                        {
                            Text = dr.GetString(1),
                            Value = idProd.ToString(),
                            Selected = idProd == productoSeleccionado
                        });
                    }
                }

                var cmdCat = new SqlCommand("SELECT idCategoria, descripcion FROM categorias ORDER BY descripcion", cn);
                using (var dr = cmdCat.ExecuteReader())
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
