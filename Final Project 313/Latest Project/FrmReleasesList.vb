Imports System.Data.OleDb
Imports System.Drawing.Printing

Public Class FrmReleasesList

    ' --- Variables for Printing ---
    ' These store the data from the grid to be printed
    Private _rowsToPrint As New List(Of DataRowView)
    Private _printRowIndex As Integer = 0

    ' --- Form Load & Search Button ---

    Private Sub FrmReleasesList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Set the date pickers to today by default
        dtpFrom.Value = DateTime.Now.Date
        dtpTo.Value = DateTime.Now.Date

        ' Load all data on startup
        RefreshGrid()
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        ' When user clicks search, reload the grid with filters
        RefreshGrid()
    End Sub

    ' --- The Main Data & Search Logic ---

    Private Sub RefreshGrid()
        ' 1. Start with the base SQL query
        Dim sql As String = "SELECT R.ReleaseID, R.GranteeID, G.StudentFullName, R.ReleaseDate, R.Amount, R.ORNo, R.Notes " &
                           "FROM StipendReleases AS R " &
                           "INNER JOIN Grantees AS G ON R.GranteeID = G.GranteeID"

        ' 2. Prepare dynamic lists for WHERE clauses and parameters
        Dim whereClauses As New List(Of String)
        Dim params As New List(Of OleDbParameter)

        ' 3. Check Search Filters
        ' --- Filter 1: By Name ---
        If Not String.IsNullOrWhiteSpace(txtSearchName.Text) Then
            whereClauses.Add("G.StudentFullName LIKE ?")
            params.Add(DataAccess.CreateParam("%" & txtSearchName.Text.Trim() & "%", OleDbType.VarWChar))
        End If

        ' --- Filter 2: By Date Range ---
        If chkUseDateRange.Checked Then
            whereClauses.Add("R.ReleaseDate BETWEEN ? AND ?")
            params.Add(DataAccess.CreateParam(dtpFrom.Value.Date, OleDbType.Date))
            params.Add(DataAccess.CreateParam(dtpTo.Value.Date, OleDbType.Date))
        End If

        ' 4. Build the final SQL query
        If whereClauses.Count > 0 Then
            sql &= " WHERE " & String.Join(" AND ", whereClauses)
        End If

        ' 5. Add Sorting
        sql &= " ORDER BY R.ReleaseDate DESC, G.StudentFullName"

        ' 6. Execute the query
        Dim dt As DataTable = DataAccess.GetTable(sql, params)
        dgvReleases.DataSource = dt

        ' --- 7. Grid cleanup & Formatting ---
        If dgvReleases.Columns.Contains("ReleaseID") Then
            dgvReleases.Columns("ReleaseID").Visible = False
        End If
        If dgvReleases.Columns.Contains("GranteeID") Then
            dgvReleases.Columns("GranteeID").Visible = False
        End If
        If dgvReleases.Columns.Contains("Amount") Then
            dgvReleases.Columns("Amount").DefaultCellStyle.Format = "c" ' "c" is for currency
        End If

        dgvReleases.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvReleases.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvReleases.ReadOnly = True
        dgvReleases.AllowUserToAddRows = False
        dgvReleases.MultiSelect = False
    End Sub

    ' --- CRUD Buttons ---

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        ' This code is now active and opens the new form
        Using frm As New FrmReleaseEdit()
            If frm.ShowDialog() = DialogResult.OK Then
                RefreshGrid()
            End If
        End Using
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        ' This code is now active
        If dgvReleases.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a release to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim selectedID As Integer = CInt(dgvReleases.CurrentRow.Cells("ReleaseID").Value)

        ' This opens the form and passes the ID of the selected release
        Using frm As New FrmReleaseEdit(selectedID)
            If frm.ShowDialog() = DialogResult.OK Then
                RefreshGrid()
            End If
        End Using
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If dgvReleases.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a release to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim releaseInfo = $"{dgvReleases.CurrentRow.Cells("StudentFullName").Value} - {CDate(dgvReleases.CurrentRow.Cells("ReleaseDate").Value).ToShortDateString()}"
        Dim result = MessageBox.Show($"Are you sure you want to delete this release?" & vbCrLf & "{releaseInfo}", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If result = DialogResult.No Then
            Return
        End If

        Try
            Dim selectedID As Integer = CInt(dgvReleases.CurrentRow.Cells("ReleaseID").Value)
            Dim sql As String = "DELETE FROM StipendReleases WHERE ReleaseID = ?"
            Dim params As New List(Of OleDbParameter)
            params.Add(DataAccess.CreateParam(selectedID, OleDbType.Integer))

            DataAccess.Exec(sql, params)
            RefreshGrid()

        Catch ex As Exception
            MessageBox.Show("Could not delete release." & vbCrLf & "Error: " & ex.Message, "Delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' --- Printing Logic ---

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        ' 1. Get the data from the grid
        _rowsToPrint.Clear()
        For Each row As DataGridViewRow In dgvReleases.Rows
            _rowsToPrint.Add(CType(row.DataBoundItem, DataRowView))
        Next

        If _rowsToPrint.Count = 0 Then
            MessageBox.Show("There is no data to print.", "Empty Report", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' 2. Reset the print index
        _printRowIndex = 0

        ' 3. Set the document for the preview dialog
        PrintPreviewDialog1.Document = PrintDocument1
        Try
            PrintPreviewDialog1.ShowDialog()
        Catch ex As Exception
            MessageBox.Show("An error occurred while opening the print preview." & vbCrLf & ex.Message)
        End Try
    End Sub

    Private Sub PrintDocument1_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PrintDocument1.PrintPage
        ' This code is adapted from your project sheet (Section E)

        ' --- Define Fonts ---
        Dim titleFont As New Font("Segoe UI", 14, FontStyle.Bold)
        Dim headerFont As New Font("Segoe UI", 9, FontStyle.Bold)
        Dim bodyFont As New Font("Consolas", 9, FontStyle.Regular) ' Monospaced font for alignment

        ' --- Set starting positions ---
        Dim leftMargin As Single = e.MarginBounds.Left
        Dim topMargin As Single = e.MarginBounds.Top
        Dim yPos As Single = topMargin

        ' --- 1. Draw Title ---
        e.Graphics.DrawString("Scholarship Stipend Releases", titleFont, Brushes.Black, leftMargin, yPos)
        yPos += titleFont.GetHeight(e.Graphics) * 2

        ' --- 2. Draw Header ---
        ' Use String.Format for column alignment
        ' {Index,-Width} - left-aligns in a field of Width characters
        Dim headerText As String = String.Format("{0,-12} {1,-30} {2,-25} {3,12}",
                                                 "Date", "Grantee", "OR No.", "Amount")
        e.Graphics.DrawString(headerText, headerFont, Brushes.Black, leftMargin, yPos)
        yPos += headerFont.GetHeight(e.Graphics)
        e.Graphics.DrawString(New String("-"c, 80), headerFont, Brushes.Black, leftMargin, yPos)
        yPos += headerFont.GetHeight(e.Graphics) * 1.5F

        ' --- 3. Draw Body (the data rows) ---
        Dim linesPerPage As Integer = 0
        While _printRowIndex < _rowsToPrint.Count
            Dim row As DataRowView = _rowsToPrint(_printRowIndex)

            ' Get data
            Dim releaseDate = CDate(row("ReleaseDate")).ToShortDateString()
            Dim studentName = row("StudentFullName").ToString()
            Dim orNumber = row("ORNo").ToString()
            Dim amount = CDec(row("Amount"))

            ' Format the line
            Dim line As String = String.Format("{0,-12} {1,-30} {2,-25} {3,12:C2}",
                                             releaseDate, studentName, orNumber, amount)

            ' Draw the line
            e.Graphics.DrawString(line, bodyFont, Brushes.Black, leftMargin, yPos)
            yPos += bodyFont.GetHeight(e.Graphics)

            ' Increment for next loop
            _printRowIndex += 1
            linesPerPage += 1

            ' Check if we are at the bottom of the page
            If yPos + bodyFont.GetHeight(e.Graphics) > e.MarginBounds.Bottom Then
                e.HasMorePages = True
                ' Clean up fonts
                titleFont.Dispose()
                headerFont.Dispose()
                bodyFont.Dispose()
                Return ' Stop drawing this page
            End If
        End While

        ' If we get here, we're done printing.
        e.HasMorePages = False
        _printRowIndex = 0 ' Reset for next print preview

        ' Clean up fonts
        titleFont.Dispose()
        headerFont.Dispose()
        bodyFont.Dispose()
    End Sub

End Class