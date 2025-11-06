Imports System.Data.OleDb

Public Class FrmProgramEdit
    ' This variable stores the ID of the program we are editing.
    ' 0 means we are adding a new one.
    Private _programID As Integer = 0

    ' Constructor 1: For "Add New"
    ' This is the default constructor called when you run "New FrmProgramEdit()"
    Public Sub New()
        InitializeComponent()
    End Sub

    ' Constructor 2: For "Edit"
    ' This is the constructor FrmProgramsList calls, passing in the selected ID
    Public Sub New(programID As Integer)
        ' This calls the default constructor first to set up the form
        MyBase.New()
        InitializeComponent()

        ' Store the ID
        _programID = programID
    End Sub

    Private Sub FrmProgramEdit_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If _programID > 0 Then
            ' We are in "Edit" mode
            Me.Text = "Edit Program"
            LoadProgramData()
        Else
            ' We are in "Add New" mode
            Me.Text = "Add New Program"
        End If
    End Sub

    Private Sub LoadProgramData()
        Dim sql As String = "SELECT * FROM ScholarshipPrograms WHERE ProgramID = ?"
        Dim params As New List(Of OleDbParameter)
        params.Add(DataAccess.CreateParam(_programID, OleDbType.Integer))

        Dim dt As DataTable = DataAccess.GetTable(sql, params)

        If dt.Rows.Count > 0 Then
            ' If we found the record, fill the textboxes
            Dim row As DataRow = dt.Rows(0)
            txtName.Text = row("ProgramName").ToString()
            txtSponsor.Text = row("Sponsor").ToString()
            txtAmount.Text = row("AmountPerRelease").ToString()
        Else
            ' This should not happen, but it's good practice to check
            MessageBox.Show("Could not find the selected program.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.Close()
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        ' The DialogResult property was set to Cancel in the designer,
        ' but we can also set it here in code. This tells the list form
        ' that the user clicked "Cancel".
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        ' 1. --- Validation ---
        If String.IsNullOrWhiteSpace(txtName.Text) Then
            MessageBox.Show("Program Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtName.Focus()
            Return
        End If

        Dim amount As Decimal
        If Not Decimal.TryParse(txtAmount.Text, amount) Then
            MessageBox.Show("Amount must be a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtAmount.Focus()
            Return
        End If

        ' 2. --- Prepare SQL and Parameters ---
        Dim sql As String = ""
        Dim params As New List(Of OleDbParameter)

        ' MS Access uses '?' for parameters. The ORDER MATTERS.
        If _programID = 0 Then
            ' INSERT (Add New)
            sql = "INSERT INTO ScholarshipPrograms (ProgramName, Sponsor, AmountPerRelease, CreatedAt, UpdatedAt) " &
                  "VALUES (?, ?, ?, ?, ?)"

            params.Add(DataAccess.CreateParam(txtName.Text.Trim(), OleDbType.VarWChar)) ' Param 1: ProgramName
            params.Add(DataAccess.CreateParam(txtSponsor.Text.Trim(), OleDbType.VarWChar)) ' Param 2: Sponsor
            params.Add(DataAccess.CreateParam(amount, OleDbType.Currency))             ' Param 3: Amount
            params.Add(DataAccess.CreateParam(Now(), OleDbType.Date))                 ' Param 4: CreatedAt
            params.Add(DataAccess.CreateParam(Now(), OleDbType.Date))                 ' Param 5: UpdatedAt

        Else
            ' UPDATE (Edit Existing)
            sql = "UPDATE ScholarshipPrograms SET ProgramName = ?, Sponsor = ?, AmountPerRelease = ?, UpdatedAt = ? " &
                  "WHERE ProgramID = ?"

            params.Add(DataAccess.CreateParam(txtName.Text.Trim(), OleDbType.VarWChar)) ' Param 1: ProgramName
            params.Add(DataAccess.CreateParam(txtSponsor.Text.Trim(), OleDbType.VarWChar)) ' Param 2: Sponsor
            params.Add(DataAccess.CreateParam(amount, OleDbType.Currency))             ' Param 3: Amount
            params.Add(DataAccess.CreateParam(Now(), OleDbType.Date))                 ' Param 4: UpdatedAt
            params.Add(DataAccess.CreateParam(_programID, OleDbType.Integer))            ' Param 5: ProgramID (for WHERE)
        End If

        ' 3. --- Execute and Close ---
        Try
            DataAccess.Exec(sql, params)

            ' Set DialogResult to OK so the list form knows to refresh
            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch ex As Exception
            MessageBox.Show("An error occurred while saving." & vbCrLf & ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class