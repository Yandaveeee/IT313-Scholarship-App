Imports System.Data.OleDb

Public Class FrmGranteesList

    Private Sub FrmGranteesList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        RefreshGrid()
    End Sub

    Private Sub RefreshGrid()
        ' This SQL JOINs the Grantees and ScholarshipPrograms tables
        ' to show the program name instead of just the ID.
        Dim sql As String = "SELECT G.GranteeID, G.StudentFullName, G.School, G.Course, G.Status, P.ProgramName " &
                           "FROM Grantees AS G " &
                           "INNER JOIN ScholarshipPrograms AS P ON G.ProgramID = P.ProgramID " &
                           "ORDER BY G.StudentFullName"

        dgvGrantees.DataSource = DataAccess.GetTable(sql, Nothing)

        ' --- Grid cleanup ---
        If dgvGrantees.Columns.Contains("GranteeID") Then
            dgvGrantees.Columns("GranteeID").Visible = False ' Hide the ID
        End If

        ' --- Set DataGridView properties for a cleaner look ---
        dgvGrantees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvGrantees.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvGrantees.ReadOnly = True
        dgvGrantees.AllowUserToAddRows = False
        dgvGrantees.MultiSelect = False
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        RefreshGrid()
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        ' Open the FrmGranteeEdit form
        Using frm As New FrmGranteeEdit()
            ' ShowDialog waits for the form to close
            If frm.ShowDialog() = DialogResult.OK Then
                ' If the user clicked "Save", refresh the grid
                RefreshGrid()
            End If
        End Using
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        ' 1. Check if a row is actually selected
        If dgvGrantees.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a grantee to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' 2. Get the ID from the selected grid row
        Dim selectedID As Integer = CInt(dgvGrantees.CurrentRow.Cells("GranteeID").Value)

        ' 3. Open the Edit form, passing the ID
        Using frm As New FrmGranteeEdit(selectedID) ' Use the constructor that takes an ID
            If frm.ShowDialog() = DialogResult.OK Then
                RefreshGrid()
            End If
        End Using
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        ' 1. Check for selection
        If dgvGrantees.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a grantee to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' 2. Confirm the deletion
        Dim studentName = dgvGrantees.CurrentRow.Cells("StudentFullName").Value.ToString()
        Dim result = MessageBox.Show($"Are you sure you want to delete '{studentName}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If result = DialogResult.No Then
            Return
        End If

        ' 3. Get ID and execute delete
        Try
            Dim selectedID As Integer = CInt(dgvGrantees.CurrentRow.Cells("GranteeID").Value)
            Dim sql As String = "DELETE FROM Grantees WHERE GranteeID = ?"
            Dim params As New List(Of OleDbParameter)
            params.Add(DataAccess.CreateParam(selectedID, OleDbType.Integer))

            DataAccess.Exec(sql, params)
            RefreshGrid() ' Refresh to show the deletion

        Catch ex As Exception
            ' This will fail if the Grantee has StipendReleases (due to the Foreign Key)
            MessageBox.Show("Could not delete grantee. They may have release records." & vbCrLf & "Error: " & ex.Message, "Delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub dgvGrantees_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvGrantees.CellContentClick

    End Sub
End Class