<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
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
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.cmdOK = New System.Windows.Forms.Button()
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.cboShadeGridMax = New System.Windows.Forms.ComboBox()
        Me.lblSexp = New System.Windows.Forms.Label()
        Me.lblSmin = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.lblVexp = New System.Windows.Forms.Label()
        Me.lblVmin = New System.Windows.Forms.Label()
        Me.lblCutoff = New System.Windows.Forms.Label()
        Me.txtSexp = New System.Windows.Forms.TextBox()
        Me.txtSmin = New System.Windows.Forms.TextBox()
        Me.txtVexp = New System.Windows.Forms.TextBox()
        Me.txtVmin = New System.Windows.Forms.TextBox()
        Me.txtCutoff = New System.Windows.Forms.TextBox()
        Me.lblShadeGridText = New System.Windows.Forms.Label()
        Me.lblOutputBILText = New System.Windows.Forms.Label()
        Me.cmdSelectOutputBIL = New System.Windows.Forms.Button()
        Me.cmdSelectShadeGrid = New System.Windows.Forms.Button()
        Me.InputRasterTabControl = New System.Windows.Forms.TabControl()
        Me.tp8or16bitTab = New System.Windows.Forms.TabPage()
        Me.lblCLRFileName = New System.Windows.Forms.Label()
        Me.lblColorGridName = New System.Windows.Forms.Label()
        Me.cmdSelectColorGrid = New System.Windows.Forms.Button()
        Me.cmdSelectCLRFile = New System.Windows.Forms.Button()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.tp8bitTab = New System.Windows.Forms.TabPage()
        Me.lbl8BitTif = New System.Windows.Forms.Label()
        Me.cmdSel8bitTIF = New System.Windows.Forms.Button()
        Me.lbl8bitName = New System.Windows.Forms.Label()
        Me.tp24bitTif = New System.Windows.Forms.TabPage()
        Me.lbl32BitTIF = New System.Windows.Forms.Label()
        Me.cmdSel24bitTIF = New System.Windows.Forms.Button()
        Me.lbl32BitTIFText = New System.Windows.Forms.Label()
        Me.tp3by8bit = New System.Windows.Forms.TabPage()
        Me.lblBRaster = New System.Windows.Forms.Label()
        Me.cmdSelBraster = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.lblGRaster = New System.Windows.Forms.Label()
        Me.lblRRaster = New System.Windows.Forms.Label()
        Me.cmdSelRraster = New System.Windows.Forms.Button()
        Me.cmdSelGraster = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.lblShadeGridName = New System.Windows.Forms.Label()
        Me.lblOutputBILName = New System.Windows.Forms.Label()
        Me.btnTestValues = New System.Windows.Forms.Button()
        Me.btnHelp = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.InputRasterTabControl.SuspendLayout()
        Me.tp8or16bitTab.SuspendLayout()
        Me.tp8bitTab.SuspendLayout()
        Me.tp24bitTif.SuspendLayout()
        Me.tp3by8bit.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(12, 417)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(392, 21)
        Me.lblStatus.TabIndex = 0
        Me.lblStatus.Text = "Form loaded."
        '
        'cmdOK
        '
        Me.cmdOK.Location = New System.Drawing.Point(130, 388)
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.Size = New System.Drawing.Size(75, 23)
        Me.cmdOK.TabIndex = 1
        Me.cmdOK.Text = "OK"
        Me.cmdOK.UseVisualStyleBackColor = True
        '
        'cmdCancel
        '
        Me.cmdCancel.Location = New System.Drawing.Point(211, 388)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(75, 23)
        Me.cmdCancel.TabIndex = 2
        Me.cmdCancel.Text = "Close"
        Me.cmdCancel.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.cboShadeGridMax)
        Me.GroupBox1.Controls.Add(Me.lblSexp)
        Me.GroupBox1.Controls.Add(Me.lblSmin)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.lblVexp)
        Me.GroupBox1.Controls.Add(Me.lblVmin)
        Me.GroupBox1.Controls.Add(Me.lblCutoff)
        Me.GroupBox1.Controls.Add(Me.txtSexp)
        Me.GroupBox1.Controls.Add(Me.txtSmin)
        Me.GroupBox1.Controls.Add(Me.txtVexp)
        Me.GroupBox1.Controls.Add(Me.txtVmin)
        Me.GroupBox1.Controls.Add(Me.txtCutoff)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 258)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(392, 118)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "SVM Parameters"
        '
        'cboShadeGridMax
        '
        Me.cboShadeGridMax.FormattingEnabled = True
        Me.cboShadeGridMax.Location = New System.Drawing.Point(314, 22)
        Me.cboShadeGridMax.Name = "cboShadeGridMax"
        Me.cboShadeGridMax.Size = New System.Drawing.Size(50, 21)
        Me.cboShadeGridMax.TabIndex = 12
        '
        'lblSexp
        '
        Me.lblSexp.AutoSize = True
        Me.lblSexp.Location = New System.Drawing.Point(274, 79)
        Me.lblSexp.Name = "lblSexp"
        Me.lblSexp.Size = New System.Drawing.Size(34, 13)
        Me.lblSexp.TabIndex = 11
        Me.lblSexp.Text = "Sexp:"
        '
        'lblSmin
        '
        Me.lblSmin.AutoSize = True
        Me.lblSmin.Location = New System.Drawing.Point(275, 53)
        Me.lblSmin.Name = "lblSmin"
        Me.lblSmin.Size = New System.Drawing.Size(33, 13)
        Me.lblSmin.TabIndex = 10
        Me.lblSmin.Text = "Smin:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(216, 26)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(92, 13)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "Shade raster max:"
        '
        'lblVexp
        '
        Me.lblVexp.AutoSize = True
        Me.lblVexp.Location = New System.Drawing.Point(37, 79)
        Me.lblVexp.Name = "lblVexp"
        Me.lblVexp.Size = New System.Drawing.Size(34, 13)
        Me.lblVexp.TabIndex = 8
        Me.lblVexp.Text = "Vexp:"
        '
        'lblVmin
        '
        Me.lblVmin.AutoSize = True
        Me.lblVmin.Location = New System.Drawing.Point(38, 53)
        Me.lblVmin.Name = "lblVmin"
        Me.lblVmin.Size = New System.Drawing.Size(33, 13)
        Me.lblVmin.TabIndex = 7
        Me.lblVmin.Text = "Vmin:"
        '
        'lblCutoff
        '
        Me.lblCutoff.AutoSize = True
        Me.lblCutoff.Location = New System.Drawing.Point(33, 26)
        Me.lblCutoff.Name = "lblCutoff"
        Me.lblCutoff.Size = New System.Drawing.Size(38, 13)
        Me.lblCutoff.TabIndex = 6
        Me.lblCutoff.Text = "Cutoff:"
        '
        'txtSexp
        '
        Me.txtSexp.Location = New System.Drawing.Point(314, 75)
        Me.txtSexp.Name = "txtSexp"
        Me.txtSexp.Size = New System.Drawing.Size(50, 20)
        Me.txtSexp.TabIndex = 5
        '
        'txtSmin
        '
        Me.txtSmin.Location = New System.Drawing.Point(314, 49)
        Me.txtSmin.Name = "txtSmin"
        Me.txtSmin.Size = New System.Drawing.Size(50, 20)
        Me.txtSmin.TabIndex = 4
        '
        'txtVexp
        '
        Me.txtVexp.Location = New System.Drawing.Point(77, 75)
        Me.txtVexp.Name = "txtVexp"
        Me.txtVexp.Size = New System.Drawing.Size(50, 20)
        Me.txtVexp.TabIndex = 2
        '
        'txtVmin
        '
        Me.txtVmin.Location = New System.Drawing.Point(77, 49)
        Me.txtVmin.Name = "txtVmin"
        Me.txtVmin.Size = New System.Drawing.Size(50, 20)
        Me.txtVmin.TabIndex = 1
        '
        'txtCutoff
        '
        Me.txtCutoff.Location = New System.Drawing.Point(77, 22)
        Me.txtCutoff.Name = "txtCutoff"
        Me.txtCutoff.Size = New System.Drawing.Size(50, 20)
        Me.txtCutoff.TabIndex = 0
        '
        'lblShadeGridText
        '
        Me.lblShadeGridText.AutoSize = True
        Me.lblShadeGridText.Location = New System.Drawing.Point(27, 193)
        Me.lblShadeGridText.Name = "lblShadeGridText"
        Me.lblShadeGridText.Size = New System.Drawing.Size(70, 13)
        Me.lblShadeGridText.TabIndex = 4
        Me.lblShadeGridText.Text = "Shade raster:"
        '
        'lblOutputBILText
        '
        Me.lblOutputBILText.AutoSize = True
        Me.lblOutputBILText.Location = New System.Drawing.Point(36, 225)
        Me.lblOutputBILText.Name = "lblOutputBILText"
        Me.lblOutputBILText.Size = New System.Drawing.Size(61, 13)
        Me.lblOutputBILText.TabIndex = 5
        Me.lblOutputBILText.Text = "Output BIL:"
        '
        'cmdSelectOutputBIL
        '
        Me.cmdSelectOutputBIL.Location = New System.Drawing.Point(355, 221)
        Me.cmdSelectOutputBIL.Name = "cmdSelectOutputBIL"
        Me.cmdSelectOutputBIL.Size = New System.Drawing.Size(30, 20)
        Me.cmdSelectOutputBIL.TabIndex = 8
        Me.cmdSelectOutputBIL.Text = "..."
        Me.cmdSelectOutputBIL.UseVisualStyleBackColor = True
        '
        'cmdSelectShadeGrid
        '
        Me.cmdSelectShadeGrid.Location = New System.Drawing.Point(355, 189)
        Me.cmdSelectShadeGrid.Name = "cmdSelectShadeGrid"
        Me.cmdSelectShadeGrid.Size = New System.Drawing.Size(30, 20)
        Me.cmdSelectShadeGrid.TabIndex = 9
        Me.cmdSelectShadeGrid.Text = "..."
        Me.cmdSelectShadeGrid.UseVisualStyleBackColor = True
        '
        'InputRasterTabControl
        '
        Me.InputRasterTabControl.Controls.Add(Me.tp8or16bitTab)
        Me.InputRasterTabControl.Controls.Add(Me.tp8bitTab)
        Me.InputRasterTabControl.Controls.Add(Me.tp24bitTif)
        Me.InputRasterTabControl.Controls.Add(Me.tp3by8bit)
        Me.InputRasterTabControl.Location = New System.Drawing.Point(12, 12)
        Me.InputRasterTabControl.Name = "InputRasterTabControl"
        Me.InputRasterTabControl.SelectedIndex = 0
        Me.InputRasterTabControl.Size = New System.Drawing.Size(392, 158)
        Me.InputRasterTabControl.TabIndex = 10
        '
        'tp8or16bitTab
        '
        Me.tp8or16bitTab.Controls.Add(Me.lblCLRFileName)
        Me.tp8or16bitTab.Controls.Add(Me.lblColorGridName)
        Me.tp8or16bitTab.Controls.Add(Me.cmdSelectColorGrid)
        Me.tp8or16bitTab.Controls.Add(Me.cmdSelectCLRFile)
        Me.tp8or16bitTab.Controls.Add(Me.Label10)
        Me.tp8or16bitTab.Controls.Add(Me.Label11)
        Me.tp8or16bitTab.Location = New System.Drawing.Point(4, 22)
        Me.tp8or16bitTab.Name = "tp8or16bitTab"
        Me.tp8or16bitTab.Padding = New System.Windows.Forms.Padding(3)
        Me.tp8or16bitTab.Size = New System.Drawing.Size(384, 132)
        Me.tp8or16bitTab.TabIndex = 0
        Me.tp8or16bitTab.Text = "8- or 16-bit with CLR"
        Me.tp8or16bitTab.UseVisualStyleBackColor = True
        '
        'lblCLRFileName
        '
        Me.lblCLRFileName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCLRFileName.Location = New System.Drawing.Point(85, 72)
        Me.lblCLRFileName.Name = "lblCLRFileName"
        Me.lblCLRFileName.Size = New System.Drawing.Size(250, 20)
        Me.lblCLRFileName.TabIndex = 18
        '
        'lblColorGridName
        '
        Me.lblColorGridName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblColorGridName.Location = New System.Drawing.Point(85, 40)
        Me.lblColorGridName.Name = "lblColorGridName"
        Me.lblColorGridName.Size = New System.Drawing.Size(250, 20)
        Me.lblColorGridName.TabIndex = 17
        '
        'cmdSelectColorGrid
        '
        Me.cmdSelectColorGrid.Location = New System.Drawing.Point(341, 40)
        Me.cmdSelectColorGrid.Name = "cmdSelectColorGrid"
        Me.cmdSelectColorGrid.Size = New System.Drawing.Size(30, 20)
        Me.cmdSelectColorGrid.TabIndex = 16
        Me.cmdSelectColorGrid.Text = "..."
        Me.cmdSelectColorGrid.UseVisualStyleBackColor = True
        '
        'cmdSelectCLRFile
        '
        Me.cmdSelectCLRFile.Location = New System.Drawing.Point(341, 72)
        Me.cmdSelectCLRFile.Name = "cmdSelectCLRFile"
        Me.cmdSelectCLRFile.Size = New System.Drawing.Size(30, 20)
        Me.cmdSelectCLRFile.TabIndex = 15
        Me.cmdSelectCLRFile.Text = "..."
        Me.cmdSelectCLRFile.UseVisualStyleBackColor = True
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(32, 76)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(50, 13)
        Me.Label10.TabIndex = 14
        Me.Label10.Text = "CLR File:"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(19, 44)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(63, 13)
        Me.Label11.TabIndex = 13
        Me.Label11.Text = "Color raster:"
        '
        'tp8bitTab
        '
        Me.tp8bitTab.Controls.Add(Me.lbl8BitTif)
        Me.tp8bitTab.Controls.Add(Me.cmdSel8bitTIF)
        Me.tp8bitTab.Controls.Add(Me.lbl8bitName)
        Me.tp8bitTab.Location = New System.Drawing.Point(4, 22)
        Me.tp8bitTab.Name = "tp8bitTab"
        Me.tp8bitTab.Padding = New System.Windows.Forms.Padding(3)
        Me.tp8bitTab.Size = New System.Drawing.Size(384, 132)
        Me.tp8bitTab.TabIndex = 1
        Me.tp8bitTab.Text = "8-bit TIF (no CLR)"
        Me.tp8bitTab.UseVisualStyleBackColor = True
        '
        'lbl8BitTif
        '
        Me.lbl8BitTif.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lbl8BitTif.Location = New System.Drawing.Point(85, 56)
        Me.lbl8BitTif.Name = "lbl8BitTif"
        Me.lbl8BitTif.Size = New System.Drawing.Size(250, 20)
        Me.lbl8BitTif.TabIndex = 23
        '
        'cmdSel8bitTIF
        '
        Me.cmdSel8bitTIF.Location = New System.Drawing.Point(341, 56)
        Me.cmdSel8bitTIF.Name = "cmdSel8bitTIF"
        Me.cmdSel8bitTIF.Size = New System.Drawing.Size(30, 20)
        Me.cmdSel8bitTIF.TabIndex = 22
        Me.cmdSel8bitTIF.Text = "..."
        Me.cmdSel8bitTIF.UseVisualStyleBackColor = True
        '
        'lbl8bitName
        '
        Me.lbl8bitName.AutoSize = True
        Me.lbl8bitName.Location = New System.Drawing.Point(30, 60)
        Me.lbl8bitName.Name = "lbl8bitName"
        Me.lbl8bitName.Size = New System.Drawing.Size(49, 13)
        Me.lbl8bitName.TabIndex = 19
        Me.lbl8bitName.Text = "8-bit TIF:"
        '
        'tp24bitTif
        '
        Me.tp24bitTif.Controls.Add(Me.lbl32BitTIF)
        Me.tp24bitTif.Controls.Add(Me.cmdSel24bitTIF)
        Me.tp24bitTif.Controls.Add(Me.lbl32BitTIFText)
        Me.tp24bitTif.Location = New System.Drawing.Point(4, 22)
        Me.tp24bitTif.Name = "tp24bitTif"
        Me.tp24bitTif.Padding = New System.Windows.Forms.Padding(3)
        Me.tp24bitTif.Size = New System.Drawing.Size(384, 132)
        Me.tp24bitTif.TabIndex = 2
        Me.tp24bitTif.Text = "24-bit TIF"
        Me.tp24bitTif.UseVisualStyleBackColor = True
        '
        'lbl32BitTIF
        '
        Me.lbl32BitTIF.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lbl32BitTIF.Location = New System.Drawing.Point(85, 56)
        Me.lbl32BitTIF.Name = "lbl32BitTIF"
        Me.lbl32BitTIF.Size = New System.Drawing.Size(250, 20)
        Me.lbl32BitTIF.TabIndex = 14
        '
        'cmdSel24bitTIF
        '
        Me.cmdSel24bitTIF.Location = New System.Drawing.Point(341, 56)
        Me.cmdSel24bitTIF.Name = "cmdSel24bitTIF"
        Me.cmdSel24bitTIF.Size = New System.Drawing.Size(30, 20)
        Me.cmdSel24bitTIF.TabIndex = 13
        Me.cmdSel24bitTIF.Text = "..."
        Me.cmdSel24bitTIF.UseVisualStyleBackColor = True
        '
        'lbl32BitTIFText
        '
        Me.lbl32BitTIFText.AutoSize = True
        Me.lbl32BitTIFText.Location = New System.Drawing.Point(24, 60)
        Me.lbl32BitTIFText.Name = "lbl32BitTIFText"
        Me.lbl32BitTIFText.Size = New System.Drawing.Size(55, 13)
        Me.lbl32BitTIFText.TabIndex = 12
        Me.lbl32BitTIFText.Text = "24-bit TIF:"
        '
        'tp3by8bit
        '
        Me.tp3by8bit.Controls.Add(Me.lblBRaster)
        Me.tp3by8bit.Controls.Add(Me.cmdSelBraster)
        Me.tp3by8bit.Controls.Add(Me.Label7)
        Me.tp3by8bit.Controls.Add(Me.lblGRaster)
        Me.tp3by8bit.Controls.Add(Me.lblRRaster)
        Me.tp3by8bit.Controls.Add(Me.cmdSelRraster)
        Me.tp3by8bit.Controls.Add(Me.cmdSelGraster)
        Me.tp3by8bit.Controls.Add(Me.Label3)
        Me.tp3by8bit.Controls.Add(Me.Label5)
        Me.tp3by8bit.Location = New System.Drawing.Point(4, 22)
        Me.tp3by8bit.Name = "tp3by8bit"
        Me.tp3by8bit.Padding = New System.Windows.Forms.Padding(3)
        Me.tp3by8bit.Size = New System.Drawing.Size(384, 132)
        Me.tp3by8bit.TabIndex = 3
        Me.tp3by8bit.Text = "3 x 8-bit rasters"
        Me.tp3by8bit.UseVisualStyleBackColor = True
        '
        'lblBRaster
        '
        Me.lblBRaster.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblBRaster.Location = New System.Drawing.Point(83, 89)
        Me.lblBRaster.Name = "lblBRaster"
        Me.lblBRaster.Size = New System.Drawing.Size(250, 20)
        Me.lblBRaster.TabIndex = 21
        '
        'cmdSelBraster
        '
        Me.cmdSelBraster.Location = New System.Drawing.Point(339, 89)
        Me.cmdSelBraster.Name = "cmdSelBraster"
        Me.cmdSelBraster.Size = New System.Drawing.Size(30, 20)
        Me.cmdSelBraster.TabIndex = 20
        Me.cmdSelBraster.Text = "..."
        Me.cmdSelBraster.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(23, 93)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(60, 13)
        Me.Label7.TabIndex = 19
        Me.Label7.Text = "Blue raster:"
        '
        'lblGRaster
        '
        Me.lblGRaster.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblGRaster.Location = New System.Drawing.Point(83, 56)
        Me.lblGRaster.Name = "lblGRaster"
        Me.lblGRaster.Size = New System.Drawing.Size(250, 20)
        Me.lblGRaster.TabIndex = 18
        '
        'lblRRaster
        '
        Me.lblRRaster.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblRRaster.Location = New System.Drawing.Point(83, 24)
        Me.lblRRaster.Name = "lblRRaster"
        Me.lblRRaster.Size = New System.Drawing.Size(250, 20)
        Me.lblRRaster.TabIndex = 17
        '
        'cmdSelRraster
        '
        Me.cmdSelRraster.Location = New System.Drawing.Point(339, 24)
        Me.cmdSelRraster.Name = "cmdSelRraster"
        Me.cmdSelRraster.Size = New System.Drawing.Size(30, 20)
        Me.cmdSelRraster.TabIndex = 16
        Me.cmdSelRraster.Text = "..."
        Me.cmdSelRraster.UseVisualStyleBackColor = True
        '
        'cmdSelGraster
        '
        Me.cmdSelGraster.Location = New System.Drawing.Point(339, 56)
        Me.cmdSelGraster.Name = "cmdSelGraster"
        Me.cmdSelGraster.Size = New System.Drawing.Size(30, 20)
        Me.cmdSelGraster.TabIndex = 15
        Me.cmdSelGraster.Text = "..."
        Me.cmdSelGraster.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(10, 60)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(73, 13)
        Me.Label3.TabIndex = 14
        Me.Label3.Text = "Green Raster:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(24, 28)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(59, 13)
        Me.Label5.TabIndex = 13
        Me.Label5.Text = "Red raster:"
        '
        'lblShadeGridName
        '
        Me.lblShadeGridName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblShadeGridName.Location = New System.Drawing.Point(99, 189)
        Me.lblShadeGridName.Name = "lblShadeGridName"
        Me.lblShadeGridName.Size = New System.Drawing.Size(250, 20)
        Me.lblShadeGridName.TabIndex = 11
        '
        'lblOutputBILName
        '
        Me.lblOutputBILName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblOutputBILName.Location = New System.Drawing.Point(99, 221)
        Me.lblOutputBILName.Name = "lblOutputBILName"
        Me.lblOutputBILName.Size = New System.Drawing.Size(250, 20)
        Me.lblOutputBILName.TabIndex = 12
        '
        'btnTestValues
        '
        Me.btnTestValues.Location = New System.Drawing.Point(325, 415)
        Me.btnTestValues.Name = "btnTestValues"
        Me.btnTestValues.Size = New System.Drawing.Size(75, 23)
        Me.btnTestValues.TabIndex = 13
        Me.btnTestValues.Text = "Test"
        Me.btnTestValues.UseVisualStyleBackColor = True
        '
        'btnHelp
        '
        Me.btnHelp.Location = New System.Drawing.Point(324, 387)
        Me.btnHelp.Name = "btnHelp"
        Me.btnHelp.Size = New System.Drawing.Size(75, 23)
        Me.btnHelp.TabIndex = 14
        Me.btnHelp.Text = "Help"
        Me.btnHelp.UseVisualStyleBackColor = True
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(416, 444)
        Me.Controls.Add(Me.btnHelp)
        Me.Controls.Add(Me.btnTestValues)
        Me.Controls.Add(Me.lblOutputBILName)
        Me.Controls.Add(Me.lblShadeGridName)
        Me.Controls.Add(Me.InputRasterTabControl)
        Me.Controls.Add(Me.cmdSelectShadeGrid)
        Me.Controls.Add(Me.cmdSelectOutputBIL)
        Me.Controls.Add(Me.lblOutputBILText)
        Me.Controls.Add(Me.lblShadeGridText)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.cmdOK)
        Me.Controls.Add(Me.lblStatus)
        Me.Name = "frmMain"
        Me.Text = "SatValMod image integration"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.InputRasterTabControl.ResumeLayout(False)
        Me.tp8or16bitTab.ResumeLayout(False)
        Me.tp8or16bitTab.PerformLayout()
        Me.tp8bitTab.ResumeLayout(False)
        Me.tp8bitTab.PerformLayout()
        Me.tp24bitTif.ResumeLayout(False)
        Me.tp24bitTif.PerformLayout()
        Me.tp3by8bit.ResumeLayout(False)
        Me.tp3by8bit.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents cmdOK As System.Windows.Forms.Button
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents lblSexp As System.Windows.Forms.Label
    Friend WithEvents lblSmin As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lblVexp As System.Windows.Forms.Label
    Friend WithEvents lblVmin As System.Windows.Forms.Label
    Friend WithEvents lblCutoff As System.Windows.Forms.Label
    Friend WithEvents txtSexp As System.Windows.Forms.TextBox
    Friend WithEvents txtSmin As System.Windows.Forms.TextBox
    Friend WithEvents txtVexp As System.Windows.Forms.TextBox
    Friend WithEvents txtVmin As System.Windows.Forms.TextBox
    Friend WithEvents txtCutoff As System.Windows.Forms.TextBox
    Friend WithEvents lblShadeGridText As System.Windows.Forms.Label
    Friend WithEvents lblOutputBILText As System.Windows.Forms.Label
    Friend WithEvents cmdSelectOutputBIL As System.Windows.Forms.Button
    Friend WithEvents cmdSelectShadeGrid As System.Windows.Forms.Button
    Friend WithEvents InputRasterTabControl As System.Windows.Forms.TabControl
    Friend WithEvents tp8or16bitTab As System.Windows.Forms.TabPage
    Friend WithEvents tp8bitTab As System.Windows.Forms.TabPage
    Friend WithEvents tp24bitTif As System.Windows.Forms.TabPage
    Friend WithEvents tp3by8bit As System.Windows.Forms.TabPage
    Friend WithEvents cboShadeGridMax As System.Windows.Forms.ComboBox
    Friend WithEvents lblShadeGridName As System.Windows.Forms.Label
    Friend WithEvents lblOutputBILName As System.Windows.Forms.Label
    Friend WithEvents lblCLRFileName As System.Windows.Forms.Label
    Friend WithEvents lblColorGridName As System.Windows.Forms.Label
    Friend WithEvents cmdSelectColorGrid As System.Windows.Forms.Button
    Friend WithEvents cmdSelectCLRFile As System.Windows.Forms.Button
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents lbl8BitTif As System.Windows.Forms.Label
    Friend WithEvents cmdSel8bitTIF As System.Windows.Forms.Button
    Friend WithEvents lbl8bitName As System.Windows.Forms.Label
    Friend WithEvents lbl32BitTIF As System.Windows.Forms.Label
    Friend WithEvents cmdSel24bitTIF As System.Windows.Forms.Button
    Friend WithEvents lbl32BitTIFText As System.Windows.Forms.Label
    Friend WithEvents lblBRaster As System.Windows.Forms.Label
    Friend WithEvents cmdSelBraster As System.Windows.Forms.Button
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents lblGRaster As System.Windows.Forms.Label
    Friend WithEvents lblRRaster As System.Windows.Forms.Label
    Friend WithEvents cmdSelRraster As System.Windows.Forms.Button
    Friend WithEvents cmdSelGraster As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents btnTestValues As System.Windows.Forms.Button
    Friend WithEvents btnHelp As System.Windows.Forms.Button
End Class
