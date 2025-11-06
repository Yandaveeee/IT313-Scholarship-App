Imports System.Data.OleDb

Public Class FrmGranteeEdit
    ' This variable stores the ID of the grantee we are editing.
    ' 0 means we are adding a new one.
    Private _granteeID As Integer = 0

    ' Constructor 1: For "Add New"
    ' This is the default constructor
    Public Sub New()
        InitializeComponent()
    End Sub

    ' Constructor 2: For "Edit"
    ' This constructor is called by the list form, passing in the selected ID
    Public Sub New(granteeID As Integer)
        MyBase.New()
        InitializeComponent()
        _granteeID = granteeID
    End Sub

    Private Sub FrmGranteeEdit_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Step 1: Always fill the programs dropdown
        ' This must be done before loading grantee data, or the SelectedValue will not work.
        LoadProgramsComboBox()

        ' Step 2: Check if we are in Add or Edit mode
        If _granteeID > 0 Then
            ' "Edit" mode
            Me.Text = "Edit Grantee"
            LoadGranteeData()
        Else
            ' "Add New" mode
            Me.Text = "Add New Grantee"
            cboStatus.SelectedItem = "Active" ' Default status to Active
        End If
    End Sub

    Private Sub LoadProgramsComboBox()
        ' Get all programs from the database to fill the dropdown
        Dim sql As String = "SELECT ProgramID, ProgramName FROM ScholarshipPrograms ORDER BY ProgramName"
        Dim dt As DataTable = DataAccess.GetTable(sql, Nothing)

        ' Configure the ComboBox
        cboProgram.DataSource = dt
        cboProgram.DisplayMember = "ProgramName"  ' The text the user sees
        cboProgram.ValueMember = "ProgramID"      ' The ID we save to the database

        ' Ensure the user cannot type a custom value
        cboProgram.DropDownStyle = ComboBoxStyle.DropDownList

        ' Clear selection by default
        cboProgram.SelectedIndex = -1
    End Sub

    Private Sub LoadGranteeData()
        ' Get the specific grantee's record
        Dim sql As String = "SELECT * FROM Grantees WHERE GranteeID = ?"
        Dim params As New List(Of OleDbParameter)
        params.Add(DataAccess.CreateParam(_granteeID, OleDbType.Integer))

        Dim dt As DataTable = DataAccess.GetTable(sql, params)

        If dt.Rows.Count > 0 Then
            ' Fill the form controls with the data from the database
            Dim row As DataRow = dt.Rows(0)
            txtFullName.Text = row("StudentFullName").ToString()
            txtSchool.Text = row("School").ToString()
            txtCourse.Text = row("Course").ToString()
            cboStatus.SelectedItem = row("Status").ToString()

            ' This is key: Set the ComboBox to the program ID
            cboProgram.SelectedValue = CInt(row("ProgramID"))
        Else
            MessageBox.Show("Could not find the selected grantee.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.Close()
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        ' Setting DialogResult tells the list form that no changes were made
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        ' 1. --- Validation ---
        If String.IsNullOrWhiteSpace(txtFullName.Text) Then
            MessageBox.Show("Full Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFullName.Focus()
            Return
        End If

        If cboProgram.SelectedIndex = -1 Then
            MessageBox.Show("Please select a scholarship program.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cboProgram.Focus()
            Return
        End If

        If cboStatus.SelectedIndex = -1 Then
            MessageBox.Show("Please select a status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cboStatus.Focus()
            Return
        End If

        ' Get the selected ProgramID from the ComboBox
        Dim selectedProgramID As Integer = CInt(cboProgram.SelectedValue)

        ' 2. --- Prepare SQL and Parameters ---
        Dim sql As String = ""
        Dim params As New List(Of OleDbParameter)

        If _granteeID = 0 Then
            ' INSERT (Add New)
            sql = "INSERT INTO Grantees (ProgramID, StudentFullName, School, Course, Status, CreatedAt, UpdatedAt) " &
                  "VALUES (?, ?, ?, ?, ?, ?, ?)"

            params.Add(DataAccess.CreateParam(selectedProgramID, OleDbType.Integer))  ' Param 1: ProgramID
            params.Add(DataAccess.CreateParam(txtFullName.Text.Trim(), OleDbType.VarWChar)) ' Param 2: StudentFullName
            params.Add(DataAccess.CreateParam(txtSchool.Text.Trim(), OleDbType.VarWChar))   ' Param 3: School
            params.Add(DataAccess.CreateParam(txtCourse.Text.Trim(), OleDbType.VarWChar))   ' Param 4: Course
            params.Add(DataAccess.CreateParam(cboStatus.SelectedItem.ToString(), OleDbType.VarWChar)) ' Param 5: Status
            params.Add(DataAccess.CreateParam(Now(), OleDbType.Date)) ' Param 6: CreatedAt
            params.Add(DataAccess.CreateParam(Now(), OleDbType.Date)) ' Param 7: UpdatedAt

        Else
            ' UPDATE (Edit Existing)
            sql = "UPDATE Grantees SET ProgramID = ?, StudentFullName = ?, School = ?, Course = ?, Status = ?, UpdatedAt = ? " &
                  "WHERE GranteeID = ?"

            params.Add(DataAccess.CreateParam(selectedProgramID, OleDbType.Integer))  ' Param 1: ProgramID
            params.Add(DataAccess.CreateParam(txtFullName.Text.Trim(), OleDbType.VarWChar)) ' Param 2: StudentFullName
            params.Add(DataAccess.CreateParam(txtSchool.Text.Trim(), OleDbType.VarWChar))   ' Param 3: School
            params.Add(DataAccess.CreateParam(txtCourse.Text.Trim(), OleDbType.VarWChar))   ' Param 4: Course
            params.Add(DataAccess.CreateParam(cboStatus.SelectedItem.ToString(), OleDbType.VarWChar)) ' Param 5: Status
            params.Add(DataAccess.CreateParam(Now(), OleDbType.Date)) ' Param 6: UpdatedAt
            params.Add(DataAccess.CreateParam(_granteeID, OleDbType.Integer)) ' Param 7: GranteeID (for WHERE)
        End If

        ' 3. --- Execute and Close ---
        Try
            DataAccess.Exec(sql, params)

            ' Setting DialogResult tells the list form to refresh its grid
            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch ex As Exception
            MessageBox.Show("An error occurred while saving." & vbCrLf & ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class