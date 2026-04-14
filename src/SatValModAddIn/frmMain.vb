Imports System.IO

Public Class frmMain
    Private Const SAME_SIZE_RASTER_MSG As String = "All rasters must have the same pixel size and extent"
    Public Enum ColorRasterType
        crt8_BitWithCLR = 0
        crt8_BitTIF = 1
        crt24_BitTIf = 2
        crt3_RGB_Bands = 3
    End Enum
    Private m_CLRFileName As String
    Private m_ColorGridName As String
    Private m_8bitTifFileName As String
    Private m_24bitTifFileName As String
    Private m_R_RasterName As String
    Private m_G_RasterName As String
    Private m_B_RasterName As String
    Private m_ShadeGridName As String
    Private m_ColorRasterType As ColorRasterType
    Dim m_OutputBILFileName As String

    Private Sub frmMain_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        cboShadeGridMax.Items.Add("100")
        cboShadeGridMax.Items.Add("255")
        cboShadeGridMax.SelectedIndex = 1
        txtVmin.Text = 0.1
        txtVexp.Text = 1
        txtSmin.Text = 0.2
        txtSexp.Text = 1
        txtCutoff.Text = 180
        cmdOK.Enabled = InputsOK()
        lblStatus.Text = SAME_SIZE_RASTER_MSG
    End Sub

    Private Sub cboShadeGridMax_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cboShadeGridMax.SelectedIndexChanged
        txtCutoff.Text = CInt(cboShadeGridMax.SelectedItem * 0.707)
    End Sub

    Private Sub cboShadeGridMax_Click(sender As System.Object, e As System.EventArgs) Handles cboShadeGridMax.Click
        lblStatus.Text = "Maximum value of shade raster"
    End Sub

    Private Function InputsOK() As Boolean
        If AllInputsSet() Then
            If AllInputExtentsAreTheSame() Then
                InputsOK = True
            Else
                InputsOK = False
                MsgBox("Input rasters must have the same number of rows & columns", vbCritical, "ERROR")
                Exit Function
            End If
        Else
            InputsOK = False
        End If
    End Function

    Private Sub cmdSel24bitTIF_Click(sender As System.Object, e As System.EventArgs) Handles cmdSel24bitTIF.Click
        Dim pStringUtils As New clsStringUtils
        Dim initValue As String = m_24bitTifFileName
        lblStatus.Text = SAME_SIZE_RASTER_MSG
        Windows.Forms.Application.DoEvents()
        m_24bitTifFileName = BrowseForTIFRaster("Select 24-bit TIFF")
        If m_24bitTifFileName = String.Empty Then
            m_24bitTifFileName = initValue
        End If
        lbl32BitTIF.Text = " " & pStringUtils.ShortenFullPath(m_24bitTifFileName, 35)
        If InputsOK() Then
            cmdOK.Enabled = True
        Else
            cmdOK.Enabled = False
        End If
    End Sub


    Private Sub cmdOK_Click(sender As System.Object, e As System.EventArgs) Handles cmdOK.Click
        Dim pSatValMod As clsSatValMod
        Dim pStringUtils As clsStringUtils

        pSatValMod = New clsSatValMod
        pStringUtils = New clsStringUtils
        pSatValMod = modSatValMod.SetSatValModParams(CDbl(txtVmin.Text), _
                                                    CDbl(txtVexp.Text), _
                                                    CDbl(txtSmin.Text), _
                                                    CDbl(txtSexp.Text), _
                                                    CInt(txtCutoff.Text), _
                                                    CInt(cboShadeGridMax.SelectedItem))
        cmdCancel.Enabled = False
        WriteSVMParamsToFile()
        Select Case m_ColorRasterType
            Case ColorRasterType.crt8_BitWithCLR
                modSatValMod.ExecuteForColorGrid(pSatValMod, _
                                    m_ColorGridName, _
                                    m_CLRFileName, _
                                    m_ShadeGridName, _
                                    pStringUtils.StripExtension(m_OutputBILFileName))
            Case ColorRasterType.crt8_BitTIF
                modSatValMod.ExecuteFor8BitTIF(pSatValMod, _
                                    m_8bitTifFileName, _
                                    m_ShadeGridName, _
                                    pStringUtils.StripExtension(m_OutputBILFileName))
            Case ColorRasterType.crt24_BitTIf
                modSatValMod.ExecuteFor24BitTIF(pSatValMod, _
                                    m_24bitTifFileName, _
                                    m_ShadeGridName, _
                                    pStringUtils.StripExtension(m_OutputBILFileName))
            Case ColorRasterType.crt3_RGB_Bands
                modSatValMod.ExecuteForRGBBands(pSatValMod, _
                                        m_R_RasterName, _
                                        m_G_RasterName, _
                                        m_B_RasterName, _
                                        m_ShadeGridName, _
                                        pStringUtils.StripExtension(m_OutputBILFileName))
        End Select
        AddEndTimeToParamFile()
        cmdCancel.Enabled = True
    End Sub

    Private Function AllInputExtentsAreTheSame() As Boolean
        If Not AllInputsSet() Then
            AllInputExtentsAreTheSame = False
            Exit Function
        End If
        AllInputExtentsAreTheSame = True
        Select Case m_ColorRasterType
            Case ColorRasterType.crt8_BitWithCLR
                If Not RasterRowsAndColsAreTheSame(m_ColorGridName, _
                                                        m_ShadeGridName) Then
                    AllInputExtentsAreTheSame = False
                End If
            Case ColorRasterType.crt24_BitTIf
                If Not RasterRowsAndColsAreTheSame(m_24bitTifFileName, _
                                                        m_ShadeGridName) Then
                    AllInputExtentsAreTheSame = False
                End If
            Case ColorRasterType.crt8_BitTIF
                If Not RasterRowsAndColsAreTheSame(m_8bitTifFileName, _
                                                        m_ShadeGridName) Then
                    AllInputExtentsAreTheSame = False
                End If
            Case ColorRasterType.crt3_RGB_Bands
                If Not RasterRowsAndColsAreTheSame(m_R_RasterName, _
                                                       m_ShadeGridName) Then
                    AllInputExtentsAreTheSame = False
                End If
                If Not RasterRowsAndColsAreTheSame(m_G_RasterName, _
                                                        m_ShadeGridName) Then
                    AllInputExtentsAreTheSame = False
                End If
                If Not RasterRowsAndColsAreTheSame(m_B_RasterName, _
                                                        m_ShadeGridName) Then
                    AllInputExtentsAreTheSame = False
                End If
        End Select
    End Function

    Private Function AllInputsSet() As Boolean
        AllInputsSet = True
        Select Case m_ColorRasterType
            Case ColorRasterType.crt8_BitWithCLR
                If m_ColorGridName = "" Then AllInputsSet = False
                If m_CLRFileName = "" Then AllInputsSet = False
            Case ColorRasterType.crt24_BitTIf
                If m_24bitTifFileName = "" Then AllInputsSet = False
            Case ColorRasterType.crt8_BitTIF
                If m_8bitTifFileName = "" Then AllInputsSet = False
            Case ColorRasterType.crt3_RGB_Bands
                If m_R_RasterName = "" Then AllInputsSet = False
                If m_G_RasterName = "" Then AllInputsSet = False
                If m_B_RasterName = "" Then AllInputsSet = False
        End Select
        If m_ShadeGridName = "" Then AllInputsSet = False
        If m_OutputBILFileName = "" Then AllInputsSet = False
    End Function

    Private Sub cmdSel8bitTIF_Click(sender As System.Object, e As System.EventArgs) Handles cmdSel8bitTIF.Click
        Dim pStringUtils As New clsStringUtils
        Dim initValue As String = m_8bitTifFileName
        lblStatus.Text = SAME_SIZE_RASTER_MSG
        Windows.Forms.Application.DoEvents()
        m_8bitTifFileName = BrowseForTIFRaster("Select 8-bit TIFF")
        If m_8bitTifFileName = String.Empty Then
            m_8bitTifFileName = initValue
        End If
        lbl8BitTif.Text = pStringUtils.ShortenFullPath(m_8bitTifFileName, 35)
        If InputsOK() Then
            cmdOK.Enabled = True
        Else
            cmdOK.Enabled = False
        End If
    End Sub

    Private Sub cmdSelectColorGrid_Click(sender As System.Object, e As System.EventArgs) Handles cmdSelectColorGrid.Click
        Dim pStringUtils As New clsStringUtils
        Dim initValue As String = m_ColorGridName
        lblStatus.Text = SAME_SIZE_RASTER_MSG
        Windows.Forms.Application.DoEvents()
        m_ColorGridName = BrowseForAnyRaster("Select color grid associated with CLR file")
        If m_ColorGridName = String.Empty Then
            m_ColorGridName = initValue
        End If
        lblColorGridName.Text = pStringUtils.ShortenFullPath(m_ColorGridName, 35)
        If InputsOK() Then
            cmdOK.Enabled = True
        Else
            cmdOK.Enabled = False
        End If
    End Sub

    Private Sub cmdSelRraster_Click(sender As System.Object, e As System.EventArgs) Handles cmdSelRraster.Click
        Dim pStringUtils As New clsStringUtils
        Dim initValue As String = m_R_RasterName
        lblStatus.Text = SAME_SIZE_RASTER_MSG
        Windows.Forms.Application.DoEvents()
        m_R_RasterName = BrowseForAnyRaster("Select Red raster")
        If m_R_RasterName = String.Empty Then
            m_R_RasterName = initValue
        End If
        lblRRaster.Text = pStringUtils.ShortenFullPath(m_R_RasterName, 35)
        If InputsOK() Then
            cmdOK.Enabled = True
        Else
            cmdOK.Enabled = False
        End If
    End Sub

    Private Sub cmdSelGraster_Click(sender As System.Object, e As System.EventArgs) Handles cmdSelGraster.Click
        Dim pStringUtils As New clsStringUtils
        Dim initValue As String = m_G_RasterName
        lblStatus.Text = SAME_SIZE_RASTER_MSG
        Windows.Forms.Application.DoEvents()
        m_G_RasterName = BrowseForAnyRaster("Select Green raster")
        If m_G_RasterName = String.Empty Then
            m_G_RasterName = initValue
        End If
        lblGRaster.Text = pStringUtils.ShortenFullPath(m_G_RasterName, 35)
        If InputsOK() Then
            cmdOK.Enabled = True
        Else
            cmdOK.Enabled = False
        End If
    End Sub

    Private Sub cmdSelBraster_Click(sender As System.Object, e As System.EventArgs) Handles cmdSelBraster.Click
        Dim pStringUtils As New clsStringUtils
        Dim initValue As String = m_B_RasterName
        lblStatus.Text = SAME_SIZE_RASTER_MSG
        Windows.Forms.Application.DoEvents()
        m_B_RasterName = BrowseForAnyRaster("Select Blue raster")
        If m_B_RasterName = String.Empty Then
            m_B_RasterName = initValue
        End If
        lblBRaster.Text = pStringUtils.ShortenFullPath(m_B_RasterName, 35)
        If InputsOK() Then
            cmdOK.Enabled = True
        Else
            cmdOK.Enabled = False
        End If
    End Sub

    Private Sub InputRasterTabControl_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles InputRasterTabControl.SelectedIndexChanged
        m_ColorRasterType = InputRasterTabControl.SelectedIndex
    End Sub

    Private Sub cmdCancel_Click(sender As System.Object, e As System.EventArgs) Handles cmdCancel.Click
        Me.Hide()
    End Sub

    Private Sub cmdSelectCLRFile_Click(sender As System.Object, e As System.EventArgs) Handles cmdSelectCLRFile.Click
        Dim pFileUtils As clsFileUtils
        Dim pStringUtils As New clsStringUtils
        Dim initValue As String = m_CLRFileName
        pFileUtils = New clsFileUtils
        m_CLRFileName = pFileUtils.GetFileToOpen("", "CLR Files (*.clr)|*.clr", False)
        If m_CLRFileName = String.Empty Then
            m_CLRFileName = initValue
        End If
        lblCLRFileName.Text = pStringUtils.ShortenFullPath(m_CLRFileName, 35)
        If InputsOK() Then cmdOK.Enabled = True
    End Sub

    Private Sub cmdSelectOutputBIL_Click(sender As System.Object, e As System.EventArgs) Handles cmdSelectOutputBIL.Click
        Dim pFileUtils As clsFileUtils
        Dim pStringUtils As New clsStringUtils
        Dim initValue As String = m_OutputBILFileName
        pFileUtils = New clsFileUtils
        m_OutputBILFileName = pFileUtils.GetFileToSave("", "bil", _
                                    "BIL Files (*.bil)|*.bil", 0)
        If m_OutputBILFileName = String.Empty Then
            m_OutputBILFileName = initValue
        End If
        lblOutputBILName.Text = pStringUtils.ShortenFullPath(m_OutputBILFileName, 35)
        If InputsOK() Then cmdOK.Enabled = True
    End Sub


    Private Sub cmdSelectShadeGrid_Click(sender As System.Object, e As System.EventArgs) Handles cmdSelectShadeGrid.Click
        Dim pStringUtils As New clsStringUtils
        Dim shadeMax As Integer
        Dim initValue As String = m_ShadeGridName

        lblStatus.Text = SAME_SIZE_RASTER_MSG
        System.Windows.Forms.Application.DoEvents()
        m_ShadeGridName = BrowseForAnyRaster("Select shade raster")
        If m_ShadeGridName = String.Empty Then
            m_ShadeGridName = initValue
        End If
        lblShadeGridName.Text = pStringUtils.ShortenFullPath(m_ShadeGridName, 35)
        If Len(m_ShadeGridName) > 0 Then
            shadeMax = CInt(modSatValMod.GetShadeRasterMax(m_ShadeGridName))
            Select Case shadeMax
                Case Is < 0
                    MsgBox("Invalid shade grid or statistics have not been " & vbCrLf & _
                           "computed for it." & vbCrLf & _
                           "Manually select "" Shade Grid Max"".")
                Case 255
                    cboShadeGridMax.SelectedIndex = 1
                    lblStatus.Text = " Shade raster max. set to 255.  Cutoff set to sin(45) of max."
                Case 100
                    cboShadeGridMax.SelectedIndex = 0
                    lblStatus.Text = " Shade raster max. set to 100.  Cutoff set to sin(45) of max."
            End Select
        End If
        If InputsOK() Then
            cmdOK.Enabled = True
        Else
            cmdOK.Enabled = False
        End If
    End Sub

    Private Sub WriteSVMParamsToFile()
        Dim pStringUtils As clsStringUtils

        pStringUtils = New clsStringUtils

        Dim sw As New StreamWriter(pStringUtils.StripExtension(m_OutputBILFileName) & "_svmparams.txt")

        sw.WriteLine("SatValMod Parameters:")
        sw.WriteLine("   Vmin = " & txtVmin.Text)
        sw.WriteLine("   Vexp = " & txtVexp.Text)
        sw.WriteLine("   Smin = " & txtSmin.Text)
        sw.WriteLine("   Sexp = " & txtSexp.Text)
        sw.WriteLine("   Cutoff = " & txtCutoff.Text)
        sw.WriteLine("   ShadeGridMax = " & cboShadeGridMax.SelectedValue & vbCrLf)
        Select Case m_ColorRasterType
            Case ColorRasterType.crt8_BitWithCLR
                sw.WriteLine("Input Rasters (8- or 16- bit raster with CLR):")
                sw.WriteLine("   Colour Raster: " & m_ColorGridName)
                sw.WriteLine("   CLR File     : " & m_CLRFileName)
                sw.WriteLine("   Shade Raster : " & m_ShadeGridName)
                sw.WriteLine("   Output raster: " & m_OutputBILFileName)
            Case ColorRasterType.crt8_BitTIF
                sw.WriteLine("Input Rasters (8-bit TIFF):")
                sw.WriteLine("   Colour Raster: " & m_8bitTifFileName)
                sw.WriteLine("   Shade Raster : " & m_ShadeGridName)
                sw.WriteLine("   Output raster: " & m_OutputBILFileName)
            Case ColorRasterType.crt24_BitTIf
                sw.WriteLine("Input Rasters (24-bit TIFF):")
                sw.WriteLine("   Colour Raster: " & m_24bitTifFileName)
                sw.WriteLine("   Shade Raster : " & m_ShadeGridName)
                sw.WriteLine("   Output raster: " & m_OutputBILFileName)
            Case ColorRasterType.crt3_RGB_Bands
                sw.WriteLine("Input Rasters (3x8-bit rasters):")
                sw.WriteLine("   Red Raster   : " & m_24bitTifFileName)
                sw.WriteLine("   Green Raster : " & m_24bitTifFileName)
                sw.WriteLine("   Blue Raster  : " & m_24bitTifFileName)
                sw.WriteLine("   Shade Raster : " & m_ShadeGridName)
                sw.WriteLine("   Output raster: " & m_OutputBILFileName)
        End Select
        sw.WriteLine(vbCrLf & "Start: " & Now())
        sw.Close()
    End Sub

    Public Sub AddEndTimeToParamFile()
        Dim pStringUtils As clsStringUtils
        pStringUtils = New clsStringUtils

        Dim sw As New StreamWriter(pStringUtils.StripExtension(m_OutputBILFileName) & _
                                   "_svmparams.txt", True)
        sw.WriteLine("End:   " & Now())
        sw.Close()
    End Sub

    Private Sub btnTestValues_Click(sender As System.Object, e As System.EventArgs) Handles btnTestValues.Click
        m_ColorGridName = "D:\Projects\Archived\satvalmod_vba\data\dem255.bil"
        lblColorGridName.Text = m_ColorGridName
        m_ShadeGridName = "D:\Projects\Archived\satvalmod_vba\data\shd100.bil"
        lblShadeGridName.Text = m_ShadeGridName
        m_CLRFileName = "D:\Projects\Archived\satvalmod_vba\data\dem255.clr"
        lblCLRFileName.Text = m_CLRFileName
        m_OutputBILFileName = "D:\Projects\Archived\satvalmod_vba\data\testaddin.bil"
        lblOutputBILName.Text = m_OutputBILFileName
        txtCutoff.Text = 70
        cmdOK.Enabled = True
    End Sub

    Private Sub btnHelp_Click(sender As System.Object, e As System.EventArgs) Handles btnHelp.Click
        Dim helpForm As New frmHelp
        lblStatus.Text = "Loading help ..."
        System.Windows.Forms.Application.DoEvents()
        helpForm.Show()
        lblStatus.Text = "Help loaded"
        System.Windows.Forms.Application.DoEvents()
    End Sub

    Private Sub txtCutoff_Click(sender As System.Object, e As System.EventArgs) Handles txtCutoff.Click
        lblStatus.Text = "Maxima of shade raster histogram.  See help for details"
    End Sub

    Private Sub txtVmin_Click(sender As System.Object, e As System.EventArgs) Handles txtVmin.Click
        lblStatus.Text = "Darkness of shadows. (0.0=black, 1.0=no shadow). See help for details"
    End Sub

    Private Sub txtVexp_Click(sender As System.Object, e As System.EventArgs) Handles txtVexp.Click
        lblStatus.Text = "Amount of shadow (0.3=less, 3.0=more).  See help for details"
    End Sub

    Private Sub txtSmin_Click(sender As System.Object, e As System.EventArgs) Handles txtSmin.Click
        lblStatus.Text = "Gloss of illuminated surfaces (0.0=white, 1.0=no gloss).  See help for details"
    End Sub

    Private Sub txtSexp_Click(sender As System.Object, e As System.EventArgs) Handles txtSexp.Click
        lblStatus.Text = "Amount of gloss. (0.3=more , 3.0=less).  See help for details"
    End Sub
End Class