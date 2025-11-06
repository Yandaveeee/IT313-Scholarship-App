Imports System.Data.OleDb

Public Class FrmProgramsList

    Private Sub FrmProgramsList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        RefreshGrid()
    End Sub

    Private Sub RefreshGrid()
        ' Define the SQL query
        Dim sql As String = "SELECT ProgramID, ProgramName, Sponsor, AmountPerRelease FROM ScholarshipPrograms ORDER BY ProgramName"

        ' Call the DataAccess module to get the data
        dgvPrograms.DataSource = DataAccess.GetTable(sql, Nothing)

        ' --- Optional but recommended cleanup ---
        If dgvPrograms.Columns.Contains("ProgramID") Then
            dgvPrograms.Columns("ProgramID").Visible = False ' Hide the ID column
        End If
        If dgvPrograms.Columns.Contains("AmountPerRelease") Then
            ' Format the currency column
            dgvPrograms.Columns("AmountPerRelease").DefaultCellStyle.Format = "c" ' "c" is for currency
        End If

        ' --- Set DataGridView properties for a cleaner look ---
        dgvPrograms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvPrograms.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvPrograms.ReadOnly = True
        dgvPrograms.AllowUserToAddRows = False
        dgvPrograms.MultiSelect = False
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        RefreshGrid()
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        ' Use a Using block to properly dispose of the form
        Using frm As New FrmProgramEdit()
            ' ShowDialog() pauses this form and waits for the edit form to close
            If frm.ShowDialog() = DialogResult.OK Then
                ' If the user clicked "Save" (we'll set this up later), refresh the grid
                RefreshGrid()
            End If
        End Using
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        ' 1. Check if a row is actually selected
        If dgvPrograms.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a program to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' 2. Get the ID from the selected grid row
        Dim selectedID As Integer = CInt(dgvPrograms.CurrentRow.Cells("ProgramID").Value)

        ' 3. Open the Edit form, passing the ID
        Using frm As New FrmProgramEdit(selectedID) ' We created this constructor
            If frm.ShowDialog() = DialogResult.OK Then
                RefreshGrid()
            End If
        End Using
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        ' 1. Check for selection
        If dgvPrograms.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a program to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' 2. Confirm the deletion
        Dim programName = dgvPrograms.CurrentRow.Cells("ProgramName").Value.ToString()
        Dim result = MessageBox.Show($"Are you sure you want to delete '{programName}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If result = DialogResult.No Then
            Return
        End If

        ' 3. Get ID and execute delete
        Try
            Dim selectedID As Integer = CInt(dgvPrograms.CurrentRow.Cells("ProgramID").Value)
            Dim sql As String = "DELETE FROM ScholarshipPrograms WHERE ProgramID = ?"
            Dim params As New List(Of OleDbParameter)

            ' Use the helper function from our DataAccess module
            params.Add(DataAccess.CreateParam(selectedID, OleDbType.Integer))

            DataAccess.Exec(sql, params)
            RefreshGrid() ' Refresh to show the deletion

        Catch ex As Exception
            ' This will fail if a Grantee is linked to this Program (due to the Foreign Key)
            MessageBox.Show("Could not delete program. It may have grantees associated with it." & vbCrLf & "Error: " & ex.Message, "Delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub dgvPrograms_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPrograms.CellContentClick

    End Sub
End Class