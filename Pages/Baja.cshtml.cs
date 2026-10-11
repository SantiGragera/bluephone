using BluePhone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace BluePhone.Pages
{
    public class BajaModel : PageModel
    {
        private readonly IConfiguration _config;

        public BajaModel(IConfiguration config)
        {
            _config = config;
        }

        public List<Producto> Productos { get; set; } = new();
        public string Error { get; set; } = "";

        [TempData] public string? MensajeOk { get; set; }
        [TempData] public string? MensajeError { get; set; }

        public void OnGet()
        {
            try
            {
                CargarProductos();
            }
            catch (SqlException ex)
            {
                Error = "No se pudo consultar la base de datos: " + ex.Message;
            }
        }

        // Handler personalizado: se ejecuta al confirmar la eliminación de una fila
        public IActionResult OnPostBorrar(int id)
        {
            try
            {
                using (var cn = new SqlConnection(_config.GetConnectionString("BluePhone")))
                {
                    cn.Open();

                    // Nombre del producto, solo para armar el mensaje
                    var cmdNombre = new SqlCommand("SELECT nombre FROM productos WHERE idProducto = @id", cn);
                    cmdNombre.Parameters.AddWithValue("@id", id);
                    string? nombre = cmdNombre.ExecuteScalar() as string;

                    // Solo se borra de productos: la tabla categorias no se toca
                    var cmd = new SqlCommand("DELETE FROM productos WHERE idProducto = @id", cn);
                    cmd.Parameters.AddWithValue("@id", id);
                    int filas = cmd.ExecuteNonQuery();

                    if (filas > 0)
                        MensajeOk = $"Producto \"{nombre}\" eliminado correctamente.";
                    else
                        MensajeError = "El producto ya no existe.";
                }
            }
            catch (SqlException ex)
            {
                MensajeError = "No se pudo eliminar el producto: " + ex.Message;
            }

            return RedirectToPage();
        }

        private void CargarProductos()
        {
            using (var cn = new SqlConnection(_config.GetConnectionString("BluePhone")))
            {
                cn.Open();
                var cmd = new SqlCommand(
                    @"SELECT p.idProducto, p.nombre, p.precio, p.idCategoria, c.descripcion
                      FROM productos p
                      INNER JOIN categorias c ON p.idCategoria = c.idCategoria
                      ORDER BY p.idProducto", cn);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Productos.Add(new Producto
                        {
                            IdProducto = dr.GetInt32(0),
                            Nombre = dr.GetString(1),
                            Precio = dr.GetDecimal(2),
                            IdCategoria = dr.GetInt32(3),
                            Categoria = dr.GetString(4)
                        });
                    }
                }
            }
        }
    }
}
