using System.Drawing;
using CapaEntiedadesBM;
using CapaLogicaNegocioBM;

namespace WinDocenteBMA;

public sealed class frmDocenteBM : Form
{
    private readonly DocenteService _servicio;
    private readonly TextBox _busqueda = new() { MaxLength = 254, PlaceholderText = "ID, código, nombre, email...", Dock = DockStyle.Fill };
    private readonly TextBox _codigo = new() { MaxLength = 20, PlaceholderText = "Ej. DOC-001", Dock = DockStyle.Fill };
    private readonly TextBox _nombres = new() { MaxLength = 80, Dock = DockStyle.Fill };
    private readonly TextBox _apellidos = new() { MaxLength = 80, Dock = DockStyle.Fill };
    private readonly TextBox _email = new() { MaxLength = 254, Dock = DockStyle.Fill };
    private readonly TextBox _telefono = new() { MaxLength = 30, Dock = DockStyle.Fill };
    private readonly TextBox _especialidad = new() { MaxLength = 100, Dock = DockStyle.Fill };
    private readonly CheckBox _estado = new() { Text = "Activo", Checked = true, AutoSize = true, Anchor = AnchorStyles.Left };
    private readonly DataGridView _grilla = new();
    private int? _idSeleccionado;

    public frmDocenteBM(DocenteService servicio)
    {
        _servicio = servicio;
        Text = "DocenteBM | Gestión docente";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(900, 620);
        Size = new Size(1120, 760);
        Font = new Font("Segoe UI", 10);
        BackColor = Color.FromArgb(245, 247, 250);
        BuildLayout();
        Load += (_, _) => Ejecutar(CargarDocentes);
    }

    private void BuildLayout()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(18),
            ColumnCount = 1,
            RowCount = 6,
            BackColor = BackColor
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 142));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var heading = new Label
        {
            Text = "Gestión de docentes",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.FromArgb(26, 43, 68),
            TextAlign = ContentAlignment.MiddleLeft
        };
        root.Controls.Add(heading, 0, 0);
        root.Controls.Add(BuildSearchBar(), 0, 1);
        root.Controls.Add(new Label
        {
            Text = "Datos del docente",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.FromArgb(48, 65, 89),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 2);
        root.Controls.Add(BuildFields(), 0, 3);
        root.Controls.Add(BuildActions(), 0, 4);
        root.Controls.Add(BuildGrid(), 0, 5);
        Controls.Add(root);
    }

    private Control BuildSearchBar()
    {
        var bar = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1,
            Padding = new Padding(0, 4, 0, 4)
        };
        bar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95));
        bar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        bar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        bar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        bar.Controls.Add(new Label { Text = "Búsqueda", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        bar.Controls.Add(_busqueda, 1, 0);
        bar.Controls.Add(CreateButton("Buscar", "BusRapida.png", (_, _) => Ejecutar(Buscar)), 2, 0);
        bar.Controls.Add(CreateButton("Probar conexión", "aplicarservidorr.PNG", (_, _) => Ejecutar(ProbarConexion)), 3, 0);
        _busqueda.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Ejecutar(Buscar);
            }
        };
        return bar;
    }

    private Control BuildFields()
    {
        var fields = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12, 8, 12, 8),
            BackColor = Color.White,
            ColumnCount = 4,
            RowCount = 4
        };
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        for (var i = 0; i < 4; i++) fields.RowStyles.Add(new RowStyle(SizeType.Percent, 25));

        AddField(fields, "Código", _codigo, 0, 0);
        AddField(fields, "Nombres", _nombres, 2, 0);
        AddField(fields, "Apellidos", _apellidos, 0, 1);
        AddField(fields, "Email", _email, 2, 1);
        AddField(fields, "Teléfono", _telefono, 0, 2);
        AddField(fields, "Especialidad", _especialidad, 2, 2);
        fields.Controls.Add(new Label { Text = "Estado", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
        fields.Controls.Add(_estado, 1, 3);
        return fields;
    }

    private Control BuildActions()
    {
        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 6, 0, 2)
        };
        actions.Controls.Add(CreateButton("Guardar", "procesosr.png", (_, _) => Ejecutar(Guardar), primary: true));
        actions.Controls.Add(CreateButton("Modificar", "procesosr.png", (_, _) => Ejecutar(Modificar)));
        actions.Controls.Add(CreateButton("Anular", "anulaciondocr.PNG", (_, _) => Ejecutar(Anular)));
        actions.Controls.Add(CreateButton("Limpiar", null, (_, _) => Limpiar()));
        return actions;
    }

    private Control BuildGrid()
    {
        _grilla.Dock = DockStyle.Fill;
        _grilla.BackgroundColor = Color.White;
        _grilla.BorderStyle = BorderStyle.None;
        _grilla.AutoGenerateColumns = false;
        _grilla.AllowUserToAddRows = false;
        _grilla.AllowUserToDeleteRows = false;
        _grilla.ReadOnly = true;
        _grilla.MultiSelect = false;
        _grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grilla.RowHeadersVisible = false;
        _grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grilla.EnableHeadersVisualStyles = false;
        _grilla.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(232, 238, 246);
        _grilla.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(38, 54, 76);
        _grilla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        _grilla.RowTemplate.Height = 34;
        AddColumn("IdDocente", "ID", 55);
        AddColumn("Codigo", "Código", 90);
        AddColumn("Nombres", "Nombres", 110);
        AddColumn("Apellidos", "Apellidos", 110);
        AddColumn("Email", "Email", 130);
        AddColumn("Telefono", "Teléfono", 90);
        AddColumn("Especialidad", "Especialidad", 110);
        _grilla.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "Estado", HeaderText = "Activo", FillWeight = 55 });
        _grilla.SelectionChanged += (_, _) => SeleccionarFila();
        return _grilla;
    }

    private static void AddField(TableLayoutPanel panel, string label, Control input, int column, int row)
    {
        panel.Controls.Add(new Label { Text = label, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, column, row);
        input.Margin = new Padding(3, 4, 14, 4);
        panel.Controls.Add(input, column + 1, row);
    }

    private void AddColumn(string property, string label, float weight) =>
        _grilla.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = property, HeaderText = label, FillWeight = weight });

    private Button CreateButton(string text, string? icon, EventHandler click, bool primary = false)
    {
        var button = new Button
        {
            Text = text,
            AutoSize = true,
            Height = 38,
            MinimumSize = new Size(100, 38),
            Padding = new Padding(8, 2, 8, 2),
            Margin = new Padding(0, 0, 8, 0),
            FlatStyle = FlatStyle.Flat,
            BackColor = primary ? Color.FromArgb(31, 94, 160) : Color.White,
            ForeColor = primary ? Color.White : Color.FromArgb(38, 54, 76),
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            TextImageRelation = TextImageRelation.ImageBeforeText,
            ImageAlign = ContentAlignment.MiddleLeft
        };
        button.FlatAppearance.BorderColor = Color.FromArgb(205, 215, 226);
        if (icon is not null)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Resources", icon);
            if (File.Exists(path)) button.Image = Image.FromFile(path);
        }
        button.Click += click;
        return button;
    }

    private void CargarDocentes() => MostrarDocentes(_servicio.ListarDocentes());

    private void Buscar() => MostrarDocentes(_servicio.BuscarDocentes(_busqueda.Text));

    private void ProbarConexion()
    {
        _servicio.ProbarConexion();
        MessageBox.Show("Conexión disponible.", "DocenteBM", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void Guardar()
    {
        _servicio.RegistrarDocente(LeerFormulario());
        CambioCompletado("Docente guardado.");
    }

    private void Modificar()
    {
        var docente = LeerFormulario();
        docente.IdDocente = _idSeleccionado ?? 0;
        _servicio.ActualizarDocente(docente);
        CambioCompletado("Docente actualizado.");
    }

    private void Anular()
    {
        if (_idSeleccionado is null)
            throw new InvalidOperationException("Selecciona un docente de la grilla.");
        if (MessageBox.Show("¿Anular el docente seleccionado?", "Confirmar anulación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        _servicio.EliminarDocente(_idSeleccionado.Value);
        CambioCompletado("Docente anulado.");
    }

    private Docente LeerFormulario() => new()
    {
        Codigo = _codigo.Text,
        Nombres = _nombres.Text,
        Apellidos = _apellidos.Text,
        Email = _email.Text,
        Telefono = _telefono.Text,
        Especialidad = _especialidad.Text,
        Estado = _estado.Checked
    };

    private void SeleccionarFila()
    {
        if (_grilla.SelectedRows.Count == 0 || _grilla.CurrentRow?.DataBoundItem is not Docente docente) return;
        _idSeleccionado = docente.IdDocente;
        _codigo.Text = docente.Codigo;
        _nombres.Text = docente.Nombres;
        _apellidos.Text = docente.Apellidos;
        _email.Text = docente.Email ?? "";
        _telefono.Text = docente.Telefono ?? "";
        _especialidad.Text = docente.Especialidad;
        _estado.Checked = docente.Estado;
    }

    private void Limpiar()
    {
        _idSeleccionado = null;
        _codigo.Clear();
        _nombres.Clear();
        _apellidos.Clear();
        _email.Clear();
        _telefono.Clear();
        _especialidad.Clear();
        _estado.Checked = true;
        _grilla.ClearSelection();
        _codigo.Focus();
    }

    private void MostrarDocentes(List<Docente> docentes)
    {
        _grilla.DataSource = docentes;
        _grilla.ClearSelection();
        _idSeleccionado = null;
        _codigo.Clear();
        _nombres.Clear();
        _apellidos.Clear();
        _email.Clear();
        _telefono.Clear();
        _especialidad.Clear();
        _estado.Checked = true;
    }

    private void CambioCompletado(string mensaje)
    {
        Limpiar();
        try
        {
            CargarDocentes();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{mensaje}\nNo se pudo actualizar la grilla: {ex.Message}", "DocenteBM", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        MessageBox.Show(mensaje, "DocenteBM", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void Ejecutar(Action accion)
    {
        try
        {
            accion();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "No se pudo completar la operación", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
