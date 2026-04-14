<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmHelp
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmHelp))
        Me.picBoxMain = New System.Windows.Forms.PictureBox()
        Me.picBoxSVMParams = New System.Windows.Forms.PictureBox()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.lblHelpText = New System.Windows.Forms.Label()
        Me.btnReset = New System.Windows.Forms.Button()
        CType(Me.picBoxMain, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picBoxSVMParams, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'picBoxMain
        '
        Me.picBoxMain.Image = CType(resources.GetObject("picBoxMain.Image"), System.Drawing.Image)
        Me.picBoxMain.Location = New System.Drawing.Point(74, 12)
        Me.picBoxMain.Name = "picBoxMain"
        Me.picBoxMain.Size = New System.Drawing.Size(590, 333)
        Me.picBoxMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.picBoxMain.TabIndex = 0
        Me.picBoxMain.TabStop = False
        '
        'picBoxSVMParams
        '
        Me.picBoxSVMParams.Image = CType(resources.GetObject("picBoxSVMParams.Image"), System.Drawing.Image)
        Me.picBoxSVMParams.Location = New System.Drawing.Point(12, 350)
        Me.picBoxSVMParams.Name = "picBoxSVMParams"
        Me.picBoxSVMParams.Size = New System.Drawing.Size(401, 129)
        Me.picBoxSVMParams.TabIndex = 1
        Me.picBoxSVMParams.TabStop = False
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(374, 494)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(75, 23)
        Me.btnClose.TabIndex = 3
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'lblHelpText
        '
        Me.lblHelpText.Location = New System.Drawing.Point(419, 358)
        Me.lblHelpText.Name = "lblHelpText"
        Me.lblHelpText.Size = New System.Drawing.Size(311, 124)
        Me.lblHelpText.TabIndex = 4
        Me.lblHelpText.Text = "This is a place to put a lot of text - in theory"
        '
        'btnReset
        '
        Me.btnReset.Location = New System.Drawing.Point(293, 494)
        Me.btnReset.Name = "btnReset"
        Me.btnReset.Size = New System.Drawing.Size(75, 23)
        Me.btnReset.TabIndex = 5
        Me.btnReset.Text = "Reset"
        Me.btnReset.UseVisualStyleBackColor = True
        '
        'frmHelp
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(742, 527)
        Me.Controls.Add(Me.btnReset)
        Me.Controls.Add(Me.lblHelpText)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.picBoxSVMParams)
        Me.Controls.Add(Me.picBoxMain)
        Me.Name = "frmHelp"
        Me.Text = "SVM Help"
        CType(Me.picBoxMain, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picBoxSVMParams, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents picBoxMain As System.Windows.Forms.PictureBox
    Friend WithEvents picBoxSVMParams As System.Windows.Forms.PictureBox
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents lblHelpText As System.Windows.Forms.Label
    Friend WithEvents btnReset As System.Windows.Forms.Button
End Class
