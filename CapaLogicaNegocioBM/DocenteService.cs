using System.Data;
using System.Globalization;
using System.Net.Mail;
using CapaEntiedadesBM;
using Microsoft.Data.SqlClient;

namespace CapaLogicaNegocioBM;

public sealed class DocenteService(string connectionString)
{
    private const string SelectColumns = "IdDocente, Codigo, Nombres, Apellidos, Email, Telefono, Especialidad, Estado";

    public List<Docente> ListarDocentes() => BuscarDocentes("");

    public void ProbarConexion()
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
    }

    public List<Docente> BuscarDocentes(string criterio)
    {
        criterio = criterio?.Trim() ?? "";
        if (criterio.Length > 254)
            throw new ArgumentException("La búsqueda no puede superar 254 caracteres.", nameof(criterio));

        using var connection = new SqlConnection(connectionString);
        using var command = connection.CreateCommand();
        command.CommandText = $"""
            SELECT {SelectColumns}
            FROM dbo.Docente
            WHERE @Criterio = N'' OR IdDocente = @Id OR Codigo = @Criterio
                OR Codigo LIKE @Patron OR Nombres LIKE @Patron OR Apellidos LIKE @Patron
                OR Email LIKE @Patron OR Telefono LIKE @Patron OR Especialidad LIKE @Patron
            ORDER BY Apellidos, Nombres, Codigo;
            """;
        command.Parameters.Add("@Criterio", SqlDbType.NVarChar, 254).Value = criterio;
        command.Parameters.Add("@Patron", SqlDbType.NVarChar, 256).Value = $"%{criterio}%";
        command.Parameters.Add("@Id", SqlDbType.Int).Value = int.TryParse(criterio, NumberStyles.None, CultureInfo.InvariantCulture, out var id)
            ? id
            : DBNull.Value;

        connection.Open();
        using var reader = command.ExecuteReader();
        var docentes = new List<Docente>();
        while (reader.Read())
        {
            docentes.Add(new Docente
            {
                IdDocente = reader.GetInt32(0),
                Codigo = reader.GetString(1),
                Nombres = reader.GetString(2),
                Apellidos = reader.GetString(3),
                Email = reader.IsDBNull(4) ? null : reader.GetString(4),
                Telefono = reader.IsDBNull(5) ? null : reader.GetString(5),
                Especialidad = reader.GetString(6),
                Estado = reader.GetBoolean(7)
            });
        }
        return docentes;
    }

    public Docente? BuscarDocentePorId(int id)
    {
        using var connection = new SqlConnection(connectionString);
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT {SelectColumns} FROM dbo.Docente WHERE IdDocente = @Id;";
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        connection.Open();
        using var reader = command.ExecuteReader();
        return reader.Read() ? LeerDocente(reader) : null;
    }

    public void RegistrarDocente(Docente docente)
    {
        ValidarDocente(docente);
        using var connection = new SqlConnection(connectionString);
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO dbo.Docente (Codigo, Nombres, Apellidos, Email, Telefono, Especialidad, Estado)
            VALUES (@Codigo, @Nombres, @Apellidos, @Email, @Telefono, @Especialidad, @Estado);
            """;
        AgregarCampos(command, docente);
        EjecutarCambio(command, connection);
    }

    public void ActualizarDocente(Docente docente)
    {
        ValidarDocente(docente);
        if (docente.IdDocente <= 0)
            throw new ArgumentException("Selecciona un docente para modificar.", nameof(docente));

        using var connection = new SqlConnection(connectionString);
        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE dbo.Docente
            SET Codigo = @Codigo, Nombres = @Nombres, Apellidos = @Apellidos,
                Email = @Email, Telefono = @Telefono, Especialidad = @Especialidad, Estado = @Estado
            WHERE IdDocente = @Id;
            """;
        AgregarCampos(command, docente);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = docente.IdDocente;
        EjecutarCambio(command, connection);
    }

    public void EliminarDocente(int id)
    {
        using var connection = new SqlConnection(connectionString);
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE dbo.Docente SET Estado = 0 WHERE IdDocente = @Id AND Estado = 1;";
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        connection.Open();
        if (command.ExecuteNonQuery() == 0)
            throw new InvalidOperationException("El docente no existe o ya está anulado.");
    }

    public static void ValidarDocente(Docente docente)
    {
        ArgumentNullException.ThrowIfNull(docente);
        docente.Codigo = docente.Codigo.Trim();
        docente.Nombres = docente.Nombres.Trim();
        docente.Apellidos = docente.Apellidos.Trim();
        docente.Especialidad = docente.Especialidad.Trim();
        docente.Email = LimpiarOpcional(docente.Email);
        docente.Telefono = LimpiarOpcional(docente.Telefono);

        Requerido(docente.Codigo, 20, "Código");
        Requerido(docente.Nombres, 80, "Nombres");
        Requerido(docente.Apellidos, 80, "Apellidos");
        Requerido(docente.Especialidad, 100, "Especialidad");
        Maximo(docente.Email, 254, "Email");
        Maximo(docente.Telefono, 30, "Teléfono");

        if (docente.Email is not null)
        {
            try
            {
                if (new MailAddress(docente.Email).Address != docente.Email)
                    throw new FormatException();
            }
            catch (FormatException)
            {
                throw new ArgumentException("El email no tiene un formato válido.", nameof(docente));
            }
        }
    }

    private static Docente LeerDocente(SqlDataReader reader) => new()
    {
        IdDocente = reader.GetInt32(0),
        Codigo = reader.GetString(1),
        Nombres = reader.GetString(2),
        Apellidos = reader.GetString(3),
        Email = reader.IsDBNull(4) ? null : reader.GetString(4),
        Telefono = reader.IsDBNull(5) ? null : reader.GetString(5),
        Especialidad = reader.GetString(6),
        Estado = reader.GetBoolean(7)
    };

    private static void AgregarCampos(SqlCommand command, Docente docente)
    {
        command.Parameters.Add("@Codigo", SqlDbType.NVarChar, 20).Value = docente.Codigo;
        command.Parameters.Add("@Nombres", SqlDbType.NVarChar, 80).Value = docente.Nombres;
        command.Parameters.Add("@Apellidos", SqlDbType.NVarChar, 80).Value = docente.Apellidos;
        command.Parameters.Add("@Email", SqlDbType.NVarChar, 254).Value = (object?)docente.Email ?? DBNull.Value;
        command.Parameters.Add("@Telefono", SqlDbType.NVarChar, 30).Value = (object?)docente.Telefono ?? DBNull.Value;
        command.Parameters.Add("@Especialidad", SqlDbType.NVarChar, 100).Value = docente.Especialidad;
        command.Parameters.Add("@Estado", SqlDbType.Bit).Value = docente.Estado;
    }

    private static void EjecutarCambio(SqlCommand command, SqlConnection connection)
    {
        try
        {
            connection.Open();
            if (command.ExecuteNonQuery() == 0)
                throw new InvalidOperationException("El docente no existe.");
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            throw new InvalidOperationException("Ya existe un docente con ese código.", ex);
        }
    }

    private static void Requerido(string valor, int maximo, string campo)
    {
        if (valor.Length == 0)
            throw new ArgumentException($"El campo {campo} es obligatorio.");
        Maximo(valor, maximo, campo);
    }

    private static void Maximo(string? valor, int maximo, string campo)
    {
        if (valor?.Length > maximo)
            throw new ArgumentException($"El campo {campo} admite hasta {maximo} caracteres.");
    }

    private static string? LimpiarOpcional(string? valor) => string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}
