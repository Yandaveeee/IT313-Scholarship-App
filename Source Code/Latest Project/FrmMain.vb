Public Class FrmMain
    Private Sub FrmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' This is your aggregate query (SUM)
            Dim sql As String = "SELECT SUM(Amount) FROM StipendReleases"

            ' Use our DataAccess module to get the data
            Dim dt As DataTable = DataAccess.GetTable(sql, Nothing)

            ' The result will be in the first row, first column
            If dt.Rows.Count > 0 AndAlso Not IsDBNull(dt.Rows(0)(0)) Then
                Dim total As Decimal = CDec(dt.Rows(0)(0))
                lblTotalReleased.Text = $"Total Stipend Released: {total:C2}"
            Else
                lblTotalReleased.Text = "Total Stipend Released: ₱0.00"
            End If

        Catch ex As Exception
            ' If it fails (e.g., table is empty), just show an error
            lblTotalReleased.Text = "Error loading total."
        End Try
    End Sub

    Private Sub ScholarshipProgramsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ScholarshipProgramsToolStripMenuItem.Click
        ' This code opens your list form as a child of the main MDI window
        Dim frm As New FrmProgramsList()
        frm.MdiParent = Me ' "Me" refers to FrmMain
        frm.Show()
    End Sub

    Private Sub StipendReleasesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StipendReleasesToolStripMenuItem.Click
        Dim frm As New FrmReleasesList()
        frm.MdiParent = Me
        frm.Show()
    End Sub

    Private Sub GranteesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GranteesToolStripMenuItem.Click
        Dim frm As New FrmGranteesList()
        frm.MdiParent = Me ' "Me" refers to FrmMain
        frm.Show()
    End Sub

End Class
