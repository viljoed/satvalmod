Module modTest
    Private Sub TestSVM()
        Dim pSatValMod As clsSatValMod
        Dim pStringUtils As New clsStringUtils
        Dim dataPath As String

        dataPath = "D:\Projects\Archived\satvalmod_vba\SampleData"

        pStringUtils = New clsStringUtils
        pSatValMod = modSatValMod.SetSatValModParams(0.1, _
                                                    1, _
                                                    0.1, _
                                                    1, _
                                                    70, _
                                                    100)
        modSatValMod.ExecuteFor8BitTIF(pSatValMod, _
                                dataPath & "\dem255_8bit.tif", _
                                dataPath & "\shd100.bil", _
                                dataPath & "\testNoForm")
    End Sub
    Private Sub TestSVMForColorGrid()
        Dim pSatValMod As clsSatValMod
        Dim pStringUtils As New clsStringUtils

        pStringUtils = New clsStringUtils
        pSatValMod = modSatValMod.SetSatValModParams(0.1, _
                                                    1, _
                                                    0.1, _
                                                    1, _
                                                    180, _
                                                    255)
        'Load frmMain
        ' WriteSVMParamsToFile
        modSatValMod.ExecuteForColorGrid(pSatValMod, _
                                "D:\Scratch\Dekemp\Blake\geology\test_geo", _
                                "D:\Scratch\Dekemp\Blake\geology\Blake_Geology.clr", _
                                "D:\Scratch\Dekemp\Blake\Topo\test_dem", _
                                pStringUtils.StripExtension("D:\Scratch\Dekemp\Blake\geology\Blake_geoSVM.bil"))
    End Sub

    'Private Sub WriteSVMParamsToFile()
    '    Dim pStringUtils As clsStringUtils
    '    pStringUtils = New clsStringUtils
    'Open pStringUtils.StripExtension(m_OutputBILFileName) & "_svmparams.txt" _
    '        For Output As #1
    'Print #1, "SatValMod Parameters:"
    'Print #1, "   Vmin = " & txtVmin.Text
    'Print #1, "   Vexp = " & txtVexp.Text
    'Print #1, "   Smin = " & txtSmin.Text
    'Print #1, "   Sexp = " & txtSexp.Text
    'Print #1, "   Cutoff = " & txtCutoff.Text
    'Print #1, "   ShadeGridMax = " & cboShadeGridMax.value & vbCrLf
    '    Select Case m_ColorRasterType
    '        Case ColorRasterType.crt8_BitWithCLR
    '    Print #1, "Input Rasters (8- or 16- bit raster with CLR):"
    '    Print #1, "   Colour Raster: " & m_ColorGridName
    '    Print #1, "   CLR File     : " & m_CLRFileName
    '    Print #1, "   Shade Raster : " & m_ShadeGridName
    '    Print #1, "   Output raster: " & m_OutputBILFileName
    '        Case ColorRasterType.crt8_BitTIF
    '    Print #1, "Input Rasters (8-bit TIFF):"
    '    Print #1, "   Colour Raster: " & m_8bitTifFileName
    '    Print #1, "   Shade Raster : " & m_ShadeGridName
    '    Print #1, "   Output raster: " & m_OutputBILFileName
    '        Case ColorRasterType.crt24_BitTIf
    '    Print #1, "Input Rasters (24-bit TIFF):"
    '    Print #1, "   Colour Raster: " & m_24bitTifFileName
    '    Print #1, "   Shade Raster : " & m_ShadeGridName
    '    Print #1, "   Output raster: " & m_OutputBILFileName
    '        Case ColorRasterType.crt3_RGB_Bands
    '    Print #1, "Input Rasters (3x8-bit rasters):"
    '    Print #1, "   Red Raster   : " & m_24bitTifFileName
    '    Print #1, "   Green Raster : " & m_24bitTifFileName
    '    Print #1, "   Blue Raster  : " & m_24bitTifFileName
    '    Print #1, "   Shade Raster : " & m_ShadeGridName
    '    Print #1, "   Output raster: " & m_OutputBILFileName
    '    End Select
    'Print #1, vbCrLf & "Start: " & Now()
    'Close #1
    'End Sub

    'Private Sub TestRastersHaveSameExtentAndCellSize()
    '    '    Debug.Print RastersHaveSameExtentAndCellSize( _
    '    '                "D:\Projects\satvalmod_vba\SampleData\dem255_8bit.tif", _
    '    '                "D:\Scratch\chung\van-predict.tif")
    '    '    Debug.Print RastersHaveSameExtentAndCellSize( _
    '    '                "D:\Projects\satvalmod_vba\SampleData\dem255_8bit.tif", _
    '    '                "D:\Projects\satvalmod_vba\SampleData\blueband")
    '    Debug.Print(RasterExtentsAreTheSame( _
    '                "D:\Projects\satvalmod_vba\SampleData\dem255_8bit.tif", _
    '                "D:\Scratch\chung\van-predict.tif"))
    '    Debug.Print(RasterExtentsAreTheSame( _
    '                "D:\Projects\satvalmod_vba\SampleData\dem255_8bit.tif", _
    '                "D:\Projects\satvalmod_vba\SampleData\blueband"))
    'End Sub

    Private Sub TestValidateNumericValues()
        Dim myNum As String

        myNum = ""
        ValidateNumericValues(myNum, 0, 0)

        myNum = ""
        ValidateNumericValues(myNum, 0, 0)
        Debug.Print(myNum)
    End Sub

    'Private Sub TestGetRasterExtent()
    '    Dim pEnv As IEnvelope

    '    Application.SaveDocument()
    '    pEnv = GetRasterExtent("D:\Projects\satvalmod_vba\SampleData\dem255_8bit.tif")
    '    Debug.Print(pEnv.XMin & "," & pEnv.YMin & "," & pEnv.XMax & "," & pEnv.YMax)
    'End Sub

    'Private Sub TestEnvelopesAreTheSame()
    '    Dim pEnv1 As IEnvelope
    '    Dim pEnv2 As IEnvelope

    '    pEnv1 = GetRasterExtent("D:\Projects\satvalmod_vba\SampleData\dem255_8bit.tif")
    '    pEnv2 = GetRasterExtent("Q:\Scratch\buckle\ternary.tif")

    '    Debug.Print(EnvelopesAreTheSame(pEnv1, pEnv2))
    'End Sub

    Private Sub TestExecuteFor24bitTif()
        Dim pSatValMod As clsSatValMod
        Dim path As String

        pSatValMod = New clsSatValMod
        path = "D:\Projects\satvalmod_vba\SampleData\"
        With pSatValMod
            .CutOff = 71
            .Vmin = 0.3
            .Vexp = 3
            .Smin = 0.3
            .Sexp = 1
            .ShadeRasterMax = 100
            .Init()
        End With
        ExecuteFor24BitTIF(pSatValMod, path & "dem255_24bit.tif", _
                                      path & "shd100.bil", _
                                      path & "test24Bit")
    End Sub

    Private Sub TestSatValMod()
        Dim pSVM As clsSatValMod

        pSVM = New clsSatValMod
        With pSVM
            .CutOff = 71
            .Vmin = 0.3
            .Vexp = 1
            .Smin = 0
            .Sexp = 1
            .ShadeRasterMax = 100
            .Init()
            .DumpSaturationModulationArray("D:\temp\sm.txt")
            .DumpValueModulationArray("D:\temp\vm.txt")
        End With
    End Sub

    Private Sub TestColorTransformer()
        Dim pCT As clsColorTransformer
        Dim r As Integer
        Dim g As Integer
        Dim b As Integer
        Dim h As Integer
        Dim s As Integer
        Dim v As Integer

        pCT = New clsColorTransformer
        pCT.RGB2HSV(69, 113, 69, h, s, v)
        Debug.Print(h, s, v)
        pCT.HSV2RGB(h, s, v, r, g, b)
        Debug.Print(r, g, b)
        pCT.RGB2HSV(r, g, b, h, s, v)
        Debug.Print(h, s, v)
    End Sub

End Module
