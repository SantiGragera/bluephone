using BluePhone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace BluePhone.Pages
{
    public class ConsultaModel : PageModel
    {
        private readonly IConfiguration _config;

        public ConsultaModel(IConfiguration config)
        {
            _config = config;
        }

        // SupportsGet = true permite enlazar el valor desde la URL (?Buscar=...) en un GET
        [BindProperty(SupportsGet = true)] public string? Buscar { get; set; }

        public List<Producto> Productos { get; set; } = new();
        public string Error { get; set; } = "";

        public void OnGet()
        {
            try
            {
                using (var cn = new SqlConnection(_config.GetConnectionString("BluePhone")))
                {
                    cn.Open();

                    // JOIN entre productos y categorias
                    var cmd = new SqlCommand(
                        @"SELECT p.idProducto, p.nombre, p.precio, p.idCategoria, c.descripcion
                          FROM productos p
                          INNER JOIN categorias c ON p.idCategoria = c.idCategoria
                          WHERE (@buscar = '' OR p.nombre LIKE '%' + @buscar + '%')
                          ORDER BY p.idProducto", cn);

                    cmd.Parameters.AddWithValue("@buscar", Buscar?.Trim() ?? "");

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
            catch (SqlException ex)
            {
                Error = "No se pudo consultar la base de datos: " + ex.Message;
            }
        }
    }
}
