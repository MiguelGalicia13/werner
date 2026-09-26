namespace CapaEntiedadesBM;

public sealed class Docente
{
    public int IdDocente { get; set; }
    public string Codigo { get; set; } = "";
    public string Nombres { get; set; } = "";
    public string Apellidos { get; set; } = "";
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string Especialidad { get; set; } = "";
    public bool Estado { get; set; } = true;
}
