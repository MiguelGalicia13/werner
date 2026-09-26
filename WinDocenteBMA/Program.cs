using System.Text.Json;
using CapaLogicaNegocioBM;

namespace WinDocenteBMA;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        if (args.Contains("--self-check", StringComparer.Ordinal))
        {
            EjecutarAutocomprobacion();
            return;
        }

        try
        {
            Application.Run(new frmDocenteBM(new DocenteService(LeerCadenaConexion())));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"No se pudo iniciar la aplicación.\n\n{ex.Message}", "DocenteBM", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static string LeerCadenaConexion()
    {
        var archivo = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        using var json = JsonDocument.Parse(File.ReadAllText(archivo));
        var cadena = json.RootElement.GetProperty("ConnectionStrings").GetProperty("CadenaConexion").GetString();
        return string.IsNullOrWhiteSpace(cadena)
            ? throw new InvalidOperationException("Falta ConnectionStrings:CadenaConexion en appsettings.json.")
            : cadena;
    }

    private static void EjecutarAutocomprobacion()
    {
        try
        {
            DocenteService.ValidarDocente(new()
            {
                Codigo = "D-001", Nombres = "Ada", Apellidos = "Lovelace",
                Email = "ada@example.com", Especialidad = "Matemática"
            });

            try
            {
                DocenteService.ValidarDocente(new() { Nombres = "Sin código" });
                throw new InvalidOperationException("La validación aceptó un código vacío.");
            }
            catch (ArgumentException ex) when (ex.Message.Contains("Código", StringComparison.Ordinal))
            {
            }

            MessageBox.Show("Validación correcta.", "Autocomprobación", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Autocomprobación fallida", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.ExitCode = 1;
        }
    }
}
