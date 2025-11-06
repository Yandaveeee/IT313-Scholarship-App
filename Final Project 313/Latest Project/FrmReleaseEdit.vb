Imports System.Data.OleDb
Imports System.Data

Public Class FrmReleaseEdit
    ' This variable stores the ID of the release we are editing.
    ' 0 means we are adding a new one.
    Private _releaseID As Integer = 0

    ' Constructor 1: For "Add New"
    Public Sub New()
        InitializeComponent()
    End Sub

    ' Constructor 2: For "Edit"
    Public Sub New(releaseID As Integer)
        MyBase.New()
        InitializeComponent()
        _releaseID = releaseID
    End Sub

    Private Sub FrmReleaseEdit_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Step 1: Always fill the grantees dropdown
        LoadGranteesComboBox()

        ' Step 2: Check if we are in Add or Edit mode
        If _releaseID > 0 Then
            ' "Edit" mode
            Me.Text = "Edit Release"
            LoadReleaseData()
        Else
            ' "Add New" mode
            Me.Text = "Add New Release"
            dtpReleaseDate.Value = DateTime.Now.Date ' Default to today
        End If
    End Sub

    Private Sub LoadGranteesComboBox()
        ' Get all *active* grantees from the database
        Dim sql As String = "SELECT GranteeID, StudentFullName FROM Grantees WHERE Status = 'Active' ORDER BY StudentFullName"
        Dim dt As DataTable = DataAccess.GetTable(sql, Nothing)

        ' Configure the ComboBox
        cboGrantee.DataSource = dt
        cboGrantee.DisplayMember = "StudentFullName"  ' The text the user sees
        cboGrantee.ValueMember = "GranteeID"      ' The ID we save to the database
        cboGrantee.DropDownStyle = ComboBoxStyle.DropDownList
        cboGrantee.SelectedIndex = -1 ' Clear selection by default
    End Sub

    Private Sub LoadReleaseData()
        ' Get the specific release record to edit
        Dim sql As String = "SELECT * FROM StipendReleases WHERE ReleaseID = ?"
        Dim params As New List(Of OleDbParameter)
        params.Add(DataAccess.CreateParam(_releaseID, OleDbType.Integer))

        Dim dt As DataTable = DataAccess.GetTable(sql, params)

        If dt.Rows.Count > 0 Then
            Dim row As DataRow = dt.Rows(0)

            ' Fill the controls with the data from the database
            cboGrantee.SelectedValue = CInt(row("GranteeID"))
            dtpReleaseDate.Value = CDate(row("ReleaseDate"))
            txtAmount.Text = row("Amount").ToString()
            txtORNo.Text = row("ORNo").ToString()
            txtNotes.Text = row("Notes").ToString()

            ' Optional: Disable editing the grantee when in edit mode
            cboGrantee.Enabled = False
        Else
            MessageBox.Show("Could not find the selected release.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.Close()
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        ' 1. --- Validation ---
        If cboGrantee.SelectedIndex = -1 Then
            MessageBox.Show("Please select a grantee.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cboGrantee.Focus()
            Return
        End If

        Dim amount As Decimal
        If Not Decimal.TryParse(txtAmount.Text, amount) OrElse amount <= 0 Then
            MessageBox.Show("Amount must be a valid, positive number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtAmount.Focus()
            Return
        End If

        ' --- Check if we are in "Add" or "Edit" mode ---
        If _releaseID = 0 Then
            ' --- ADD NEW (with Transaction) ---
            Dim selectedGranteeID As Integer = CInt(cboGrantee.SelectedValue)
            Try
                ' Call our new Transaction function
                SaveNewRelease(selectedGranteeID, dtpReleaseDate.Value.Date, amount, txtORNo.Text.Trim(), txtNotes.Text.Trim())

                Me.DialogResult = DialogResult.OK
                Me.Close()
            Catch ex As Exception
                MessageBox.Show("An error occurred. The transaction was rolled back." & vbCrLf & ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            ' --- EDIT EXISTING (Simple Exec) ---
            Try
                Dim sql As String = "UPDATE StipendReleases SET ReleaseDate = ?, Amount = ?, ORNo = ?, Notes = ?, UpdatedAt = ? " &
                                    "WHERE ReleaseID = ?"
                Dim params As New List(Of OleDbParameter)
                params.Add(DataAccess.CreateParam(dtpReleaseDate.Value.Date, OleDbType.Date)) ' Param 1
                params.Add(DataAccess.CreateParam(amount, OleDbType.Currency))             ' Param 2
                params.Add(DataAccess.CreateParam(txtORNo.Text.Trim(), OleDbType.VarWChar)) ' Param 3
                params.Add(DataAccess.CreateParam(txtNotes.Text.Trim(), OleDbType.LongVarWChar)) ' Param 4 (for MEMO/Long Text)
                params.Add(DataAccess.CreateParam(Now(), OleDbType.Date))                 ' Param 5
                params.Add(DataAccess.CreateParam(_releaseID, OleDbType.Integer))            ' Param 6

                DataAccess.Exec(sql, params)

                Me.DialogResult = DialogResult.OK
                Me.Close()
            Catch ex As Exception
                MessageBox.Show("An error occurred while updating." & vbCrLf & ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    ''' <summary>
    ''' This function saves a new release AND updates the grantee in a single transaction.
    ''' This fulfills the "Database Transaction" requirement.
    ''' </summary>
    Private Sub SaveNewRelease(granteeID As Integer, releaseDate As Date, amount As Decimal, orNo As String, notes As String)

        ' We can't use the simple Exec() function because we need to manage the connection and transaction here.
        ' Note: This connection string MUST be identical to the one in DataAccess.vb
        Dim connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\Data\AppDB.accdb;Persist Security Info=False;"

        Using cn As New OleDbConnection(connString)
            cn.Open()
            ' Start the transaction
            Using tx As OleDbTransaction = cn.BeginTransaction()
                Try
                    ' --- Step 1: Insert the new Stipend Release ---
                    Dim sql1 As String = "INSERT INTO StipendReleases (GranteeID, ReleaseDate, Amount, ORNo, Notes, CreatedAt, UpdatedAt) " &
                                         "VALUES (?, ?, ?, ?, ?, ?, ?)"
                    Using cmd1 As New OleDbCommand(sql1, cn, tx) ' Pass in the transaction object
                        cmd1.Parameters.Add(DataAccess.CreateParam(granteeID, OleDbType.Integer))
                        cmd1.Parameters.Add(DataAccess.CreateParam(releaseDate, OleDbType.Date))
                        cmd1.Parameters.Add(DataAccess.CreateParam(amount, OleDbType.Currency))
                        cmd1.Parameters.Add(DataAccess.CreateParam(orNo, OleDbType.VarWChar))
                        cmd1.Parameters.Add(DataAccess.CreateParam(notes, OleDbType.LongVarWChar)) ' Use LongVarWChar for MEMO/Long Text
                        cmd1.Parameters.Add(DataAccess.CreateParam(Now(), OleDbType.Date))
                        cmd1.Parameters.Add(DataAccess.CreateParam(Now(), OleDbType.Date))
                        cmd1.ExecuteNonQuery()
                    End Using

                    ' --- Step 2: Update the Grantee's record ---
                    Dim sql2 As String = "UPDATE Grantees SET UpdatedAt = ? WHERE GranteeID = ?"
                    Using cmd2 As New OleDbCommand(sql2, cn, tx) ' Pass in the transaction object
                        cmd2.Parameters.Add(DataAccess.CreateParam(Now(), OleDbType.Date))
                        cmd2.Parameters.Add(DataAccess.CreateParam(granteeID, OleDbType.Integer))
                        cmd2.ExecuteNonQuery()
                    End Using

                    ' If both commands succeed, commit the transaction
                    tx.Commit()

                Catch ex As Exception
                    ' If anything fails, roll back the entire transaction
                    tx.Rollback()
                    ' Send the error back to the btnSave_Click to show the user
                    Throw ex ' This re-throws the original error
                End Try
            End Using
        End Using
    End Sub

End Class