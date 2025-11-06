<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmProgramEdit
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer.
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.lblName = New System.Windows.Forms.Label()
        Me.lblSponsor = New System.Windows.Forms.Label()
        Me.lblAmount = New System.Windows.Forms.Label()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.txtSponsor = New System.Windows.Forms.TextBox()
        Me.txtAmount = New System.Windows.Forms.TextBox()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblName
        '
        Me.lblName.AutoSize = True
        Me.lblName.Location = New System.Drawing.Point(30, 30)
        Me.lblName.Name = "lblName"
        Me.lblName.Size = New System.Drawing.Size(119, 20)
        Me.lblName.TabIndex = 0
        Me.lblName.Text = "Program Name:"
        '
        'lblSponsor
        '
        Me.lblSponsor.AutoSize = True
        Me.lblSponsor.Location = New System.Drawing.Point(30, 80)
        Me.lblSponsor.Name = "lblSponsor"
        Me.lblSponsor.Size = New System.Drawing.Size(73, 20)
        Me.lblSponsor.TabIndex = 1
        Me.lblSponsor.Text = "Sponsor:"
        '
        'lblAmount
        '
        Me.lblAmount.AutoSize = True
        Me.lblAmount.Location = New System.Drawing.Point(30, 130)
        Me.lblAmount.Name = "lblAmount"
        Me.lblAmount.Size = New System.Drawing.Size(159, 20)
        Me.lblAmount.TabIndex = 2
        Me.lblAmount.Text = "Amount per Release:"
        '
        'txtName
        '
        Me.txtName.Location = New System.Drawing.Point(200, 27)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(350, 26)
        Me.txtName.TabIndex = 3
        '
        'txtSponsor
        '
        Me.txtSponsor.Location = New System.Drawing.Point(200, 77)
        Me.txtSponsor.Name = "txtSponsor"
        Me.txtSponsor.Size = New System.Drawing.Size(350, 26)
        Me.txtSponsor.TabIndex = 4
        '
        'txtAmount
        '
        Me.txtAmount.Location = New System.Drawing.Point(200, 127)
        Me.txtAmount.Name = "txtAmount"
        Me.txtAmount.Size = New System.Drawing.Size(150, 26)
        Me.txtAmount.TabIndex = 5
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(200, 190)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(100, 35)
        Me.btnSave.TabIndex = 6
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(310, 190)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(100, 35)
        Me.btnCancel.TabIndex = 7
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'FrmProgramEdit
        '
        Me.AcceptButton = Me.btnSave
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(600, 260)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.txtAmount)
        Me.Controls.Add(Me.txtSponsor)
        Me.Controls.Add(Me.txtName)
        Me.Controls.Add(Me.lblAmount)
        Me.Controls.Add(Me.lblSponsor)
        Me.Controls.Add(Me.lblName)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FrmProgramEdit"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Add/Edit Program"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblName As Label
    Friend WithEvents lblSponsor As Label
    Friend WithEvents lblAmount As Label
    Friend WithEvents txtName As TextBox
    Friend WithEvents txtSponsor As TextBox
    Friend WithEvents txtAmount As TextBox
    Friend WithEvents btnSave As Button
    Friend WithEvents btnCancel As Button
End Class
