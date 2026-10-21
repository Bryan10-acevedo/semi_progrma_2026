using System.Configuration;
using System.Data.SqlClient;

namespace SistemaAcademico
{
    public class Conexion
    {
        public static string Cadena => ConfigurationManager.ConnectionStrings["cndbAcademica"].ConnectionString;

        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(Cadena);
        }
    }
}