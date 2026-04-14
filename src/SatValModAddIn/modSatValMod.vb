Imports System.IO
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.DataSourcesRaster
Imports ESRI.ArcGIS.Geodatabase
Imports ESRI.ArcGIS.esriSystem
Imports ESRI.ArcGIS.Framework

Module modSatValMod
    Private m_mainForm As frmMain
    Private m_CLRFileName As String
    Private m_h(10000) As Integer               'HSV arrays with colors from CLRFile
    Private m_s(10000) As Integer
    Private m_v(10000) As Integer

    Public Sub ValidateNumericValues(ByRef value As String, _
                                          minValue As Double, _
                                          maxValue As Double)
        If Len(value) = 0 Then Exit Sub
        If Not IsNumeric(value) Then
            value = Left(value, Len(value) - 1)
            Exit Sub
        End If
    End Sub

    Public Function RastersHaveSameExtentAndCellSize(rasterName1 As String, _
                                            rasterName2 As String) As Boolean
        RastersHaveSameExtentAndCellSize = True
        If Not CellSizesAreTheSame(rasterName1, rasterName2, 0.5) Then
            RastersHaveSameExtentAndCellSize = False
            Exit Function
        End If
        'If Not RasterExtentsAreTheSame(rasterName1, rasterName2) Then
        '    RastersHaveSameExtentAndCellSize = False
        '    Exit Function
        'End If
    End Function

    Private Function CellSizesAreTheSame(rasterName1 As String, _
                                         rasterName2 As String, _
                                         percentAllowableCellSizeDifference As Double) As Boolean
        Dim pRasterDataset1 As IRasterDataset
        Dim pRasterDataset2 As IRasterDataset
        Dim pRaster1 As IRaster
        Dim pRaster2 As IRaster
        Dim pRasterAnalysisProps1 As IRasterAnalysisProps
        Dim pRasterAnalysisProps2 As IRasterAnalysisProps
        Dim pixelHeightRatio As Double
        Dim pixelWidthRatio As Double
        Dim maxRatio As Double
        Dim minRatio As Double

        maxRatio = 1 + percentAllowableCellSizeDifference / 100
        minRatio = 1 - percentAllowableCellSizeDifference / 100
        pRasterDataset1 = GetRasterDataset(rasterName1)
        pRasterDataset2 = GetRasterDataset(rasterName2)
        pRaster1 = pRasterDataset1.CreateDefaultRaster
        pRaster2 = pRasterDataset2.CreateDefaultRaster
        pRasterAnalysisProps1 = pRaster1
        pRasterAnalysisProps2 = pRaster2
        CellSizesAreTheSame = True

        pixelHeightRatio = pRasterAnalysisProps1.PixelHeight / _
                           pRasterAnalysisProps2.PixelHeight
        pixelWidthRatio = pRasterAnalysisProps1.PixelWidth / _
                          pRasterAnalysisProps2.PixelWidth
        If (pixelHeightRatio > maxRatio) Or (pixelHeightRatio < minRatio) Then
            CellSizesAreTheSame = False
            Exit Function
        End If
        If (pixelWidthRatio > maxRatio) Or (pixelWidthRatio < minRatio) Then
            CellSizesAreTheSame = False
            Exit Function
        End If
    End Function

    Public Function RasterRowsAndColsAreTheSame(rasterName1 As String,
                                                rasterName2 As String) As Boolean
        Dim pRasterDataset1 As IRasterDataset
        Dim pRasterDataset2 As IRasterDataset
        Dim pRasterProps1 As IRasterProps
        Dim pRasterProps2 As IRasterProps

        pRasterDataset1 = GetRasterDataset(rasterName1)
        pRasterDataset2 = GetRasterDataset(rasterName2)
        pRasterProps1 = pRasterDataset1.CreateDefaultRaster
        pRasterProps2 = pRasterDataset2.CreateDefaultRaster

        RasterRowsAndColsAreTheSame = True
        If pRasterProps1.Height <> pRasterProps2.Height Then
            RasterRowsAndColsAreTheSame = False
            Exit Function
        End If
        If pRasterProps1.Width <> pRasterProps2.Width Then
            RasterRowsAndColsAreTheSame = False
            Exit Function
        End If
    End Function

    Private Function GetRasterExtent(rasterName As String) As IEnvelope
        Dim pRasterDataset As IRasterDataset
        Dim pRaster As IRaster
        Dim pGeoDataset As IGeoDataset
        Dim pRasterProps As IRasterProps

        pRasterDataset = GetRasterDataset(rasterName)
        pGeoDataset = pRasterDataset
        pRaster = pRasterDataset.CreateDefaultRaster
        pRasterProps = pRaster
        GetRasterExtent = pGeoDataset.Extent
    End Function

    Public Sub ShowSatValModForm()
        m_mainForm = New frmMain()
        m_mainForm.ShowDialog()
        'm_mainForm.Show()
        'm_mainForm.ShowDialog()
    End Sub

    Public Sub ExecuteFor24BitTIF(pSatValMod As clsSatValMod, _
                                  tifFilename As String, _
                                  shadeRasterName As String, _
                                  outBILName As String)
        Dim pRasterDatasetColor As IRasterDataset
        Dim pGeoDatasetColor As IGeoDataset
        Dim pOriginColor As IPoint
        Dim pRasterColor As IRaster
        Dim pRasterColormap As IRasterColormap
        Dim pRasterPropsColor As IRasterProps
        Dim ncols As Long
        Dim nrows As Long
        Dim pRasterAnalysisPropsColor As IRasterAnalysisProps
        Dim xdim As Double
        Dim ydim As Double
        Dim ulxmap As Double
        Dim ulymap As Double
        Dim pRasterBandCol As IRasterBandCollection
        Dim pRasterBand As IRasterBand
        Dim pRasterDatasetShade As IRasterDataset
        Dim pRasterShade As IRaster
        Dim pRasterCursorColor As IRasterCursor
        Dim pRasterCursorShade As IRasterCursor
        Dim outRowR As Byte()
        Dim outRowG As Byte()
        Dim outRowB As Byte()
        Dim pBILWriterOut As clsBILFileWriter
        Dim pPixelBlockColor As IPixelBlock
        Dim vPixelsColor As Object
        Dim pPixelBlockShade As IPixelBlock
        Dim vPixelsShade As Object
        Dim RowCount As Long
        Dim ColCount As Long
        Dim RowIndex As Long
        Dim ColIndex As Long
        Dim inPixel_R As Object
        Dim inPixel_G As Object
        Dim inPixel_B As Object
        Dim inPixelShade As Object
        Dim pixelBlocks As Integer
        Dim pixleBlockComplete As Integer
        Dim percentComplete As Integer
        Dim stopWatch As New Stopwatch()
        Dim outBytes() As Byte
        Dim i As Integer

        If Not RasterDatasetExists(tifFilename) Then
            MsgBox(tifFilename & " does not exist.  Exiting")
            Return
        End If
        If Not RasterDatasetExists(shadeRasterName) Then
            MsgBox(shadeRasterName & " does not exist.  Exiting")
            Return
        End If
        pRasterDatasetColor = GetRasterDataset(tifFilename)
        pGeoDatasetColor = pRasterDatasetColor
        pRasterColor = pRasterDatasetColor.CreateDefaultRaster
        pRasterPropsColor = pRasterColor
        ncols = pRasterPropsColor.Width
        nrows = pRasterPropsColor.Height
        pRasterAnalysisPropsColor = pRasterPropsColor
        xdim = pRasterAnalysisPropsColor.PixelWidth
        ydim = pRasterAnalysisPropsColor.PixelHeight
        ulxmap = pGeoDatasetColor.Extent.XMin + xdim / 2
        ulymap = pGeoDatasetColor.Extent.YMax - ydim / 2
        pRasterBandCol = pRasterColor
        pRasterBand = pRasterBandCol.Item(0)
        pRasterColormap = pRasterBand.Colormap

        pRasterDatasetShade = GetRasterDataset(shadeRasterName)
        pRasterShade = pRasterDatasetShade.CreateDefaultRaster
        pOriginColor = GetRasterDatasetOrigin(pRasterDatasetColor)
        pOriginColor.Y = pOriginColor.Y - nrows * ydim
        pRasterCursorColor = pRasterColor.CreateCursor
        pRasterCursorShade = pRasterShade.CreateCursor

        ReDim outRowR(ncols)
        ReDim outRowG(ncols)
        ReDim outRowB(ncols)
        ReDim outBytes(ncols * 3)
        pBILWriterOut = New clsBILFileWriter()
        With pBILWriterOut
            .BILFileNameNoExt = outBILName
            .nbands = 3
            .nbits = 8
            .nrows = nrows
            .ncols = ncols
            .xdim = xdim
            .ydim = ydim
            .ulxmap = ulxmap
            .ulymap = ulymap
            .WriteHDRFile()
            .OpenFile()
        End With
        UpdateStatusLabel("Processing ... 0%")
        System.Windows.Forms.Application.DoEvents()
        pixelBlocks = nrows / 128
        stopWatch.Reset()
        stopWatch.Start()
        Do
            pPixelBlockColor = pRasterCursorColor.PixelBlock
            pPixelBlockShade = pRasterCursorShade.PixelBlock
            vPixelsShade = pPixelBlockShade.SafeArray(0)
            vPixelsColor = pPixelBlockColor.SafeArray(0)
            '------ Process pixels in pixelblock
            ColCount = UBound(vPixelsColor, 1)
            RowCount = UBound(vPixelsColor, 2)
            For RowIndex = 0 To RowCount
                For ColIndex = 0 To ColCount
                    Try
                        inPixel_R = CInt(pPixelBlockColor.GetVal(0, ColIndex, RowIndex))
                        inPixel_G = CInt(pPixelBlockColor.GetVal(1, ColIndex, RowIndex))
                        inPixel_B = CInt(pPixelBlockColor.GetVal(2, ColIndex, RowIndex))
                        inPixelShade = CInt(pPixelBlockShade.GetVal(0, ColIndex, RowIndex))
                    Catch ex As Exception
                        Dim fileName As String = pBILWriterOut.BILFileNameNoExt & "_RowColErrors.txt"
                        Dim sw As StreamWriter
                        If Not File.Exists(fileName) Then
                            sw = New StreamWriter(fileName, False)
                            sw.WriteLine("The following color or shade raster pixels could not be converted into integers")
                        End If
                        sw = New StreamWriter(fileName, True)
                        sw.WriteLine(RowIndex + "," + ColIndex)
                        sw.Close()
                        inPixel_R = 0
                        inPixel_G = 0
                        inPixel_B = 0
                        inPixelShade = pSatValMod.CutOff
                    End Try
                    If inPixelShade < 0 Then inPixelShade = 0
                    If inPixelShade > pSatValMod.ShadeRasterMax Then _
                        inPixelShade = pSatValMod.ShadeRasterMax
                    pSatValMod.InRed = inPixel_R
                    pSatValMod.InGreen = inPixel_G
                    pSatValMod.InBlue = inPixel_B
                    pSatValMod.InShadeValue = CInt(inPixelShade)
                    pSatValMod.ModulateRGB()
                    outBytes(i) = Convert.ToByte(pSatValMod.OutRed)
                    outBytes(i + ncols) = Convert.ToByte(pSatValMod.OutGreen)
                    outBytes(i + ncols * 2) = Convert.ToByte(pSatValMod.OutBlue)
                    i = i + 1
                Next ColIndex
                pBILWriterOut.WriteRowBytes(outBytes, ncols * 3)
                i = 0
            Next RowIndex
            pRasterCursorShade.Next()
            pixleBlockComplete = pixleBlockComplete + 1
            percentComplete = (pixleBlockComplete / pixelBlocks) * 100
            UpdateStatusLabel("Processing ... " & percentComplete & "%")
            System.Windows.Forms.Application.DoEvents()
        Loop Until Not pRasterCursorColor.Next
        pBILWriterOut.CloseFile()
        stopWatch.Stop()
        UpdateStatusLabel(String.Format("Complete in {0} ms / {1:0} sec / {2:0.00} min)",
                                        stopWatch.ElapsedMilliseconds,
                                        stopWatch.ElapsedMilliseconds / 1000,
                                        stopWatch.ElapsedMilliseconds / 60000))

    End Sub

    Public Sub ExecuteFor8BitTIF(pSatValMod As clsSatValMod, _
                                 tifFilename As String, _
                                 shadeRasterName As String, _
                                 outBILName As String)
        Dim pRasterDatasetColor As IRasterDataset
        Dim pGeoDatasetColor As IGeoDataset
        Dim pOriginColor As IPoint
        Dim pRasterColor As IRaster
        Dim pRasterColormap As IRasterColormap
        Dim pRasterPropsColor As IRasterProps
        Dim ncols As Long
        Dim nrows As Long
        Dim pRasterAnalysisPropsColor As IRasterAnalysisProps
        Dim xdim As Double
        Dim ydim As Double
        Dim ulxmap As Double
        Dim ulymap As Double
        Dim pRasterBandCol As IRasterBandCollection
        Dim pRasterBand As IRasterBand
        Dim pRasterDatasetShade As IRasterDataset
        Dim pRasterShade As IRaster
        Dim pRasterCursorColor As IRasterCursor
        Dim pRasterCursorShade As IRasterCursor
        Dim pBILWriterOut As clsBILFileWriter
        Dim pPixelBlockColor As IPixelBlock
        Dim vPixelsColor As Object
        Dim pPixelBlockShade As IPixelBlock
        Dim vPixelsShade As Object
        Dim RowCount As Long
        Dim ColCount As Long
        Dim RowIndex As Long
        Dim ColIndex As Long
        Dim inPixelColor As Object
        Dim colorMapIndex As Long
        Dim inPixelShade As Object
        Dim pixelBlocks As Integer
        Dim pixleBlockComplete As Integer
        Dim percentComplete As Integer
        Dim stopWatch As New Stopwatch()
        Dim outBytes() As Byte
        Dim i As Integer

        If Not RasterDatasetExists(tifFilename) Then
            MsgBox(tifFilename & " does not exist.  Exiting")
            Return
        End If
        If Not RasterDatasetExists(shadeRasterName) Then
            MsgBox(shadeRasterName & " does not exist.  Exiting")
            Return
        End If
        pRasterDatasetColor = GetRasterDataset(tifFilename)
        pGeoDatasetColor = pRasterDatasetColor
        pRasterColor = pRasterDatasetColor.CreateDefaultRaster
        pRasterPropsColor = pRasterColor
        ncols = pRasterPropsColor.Width
        nrows = pRasterPropsColor.Height
        pRasterAnalysisPropsColor = pRasterPropsColor
        xdim = pRasterAnalysisPropsColor.PixelWidth
        ydim = pRasterAnalysisPropsColor.PixelHeight
        ulxmap = pGeoDatasetColor.Extent.XMin + xdim / 2
        ulymap = pGeoDatasetColor.Extent.YMax - ydim / 2
        pRasterBandCol = pRasterColor
        pRasterBand = pRasterBandCol.Item(0)
        pRasterColormap = pRasterBand.Colormap
        Dim redValues As Integer()
        Dim greenValues As Integer()
        Dim blueValues As Integer()
        ReDim redValues(UBound(pRasterColormap.Colors))
        ReDim greenValues(UBound(pRasterColormap.Colors))
        ReDim blueValues(UBound(pRasterColormap.Colors))
        'Dim swclr = New StreamWriter("tifclrmap.txt")

        For i = 0 To UBound(redValues)
            redValues(i) = CInt(pRasterColormap.RedValues(i) * 255)
            greenValues(i) = CInt(pRasterColormap.GreenValues(i) * 255)
            blueValues(i) = CInt(pRasterColormap.BlueValues(i) * 255)
            '    swclr.WriteLine(CStr(redValues(i)) + "," + CStr(greenValues(i)) + "," + CStr(blueValues(i)))
        Next i
        i = 0
        'swclr.Close()
        pRasterDatasetShade = GetRasterDataset(shadeRasterName)
        pRasterShade = pRasterDatasetShade.CreateDefaultRaster
        pOriginColor = GetRasterDatasetOrigin(pRasterDatasetColor)
        pOriginColor.Y = pOriginColor.Y - nrows * ydim
        pRasterCursorColor = pRasterColor.CreateCursor
        pRasterCursorShade = pRasterShade.CreateCursor

        ReDim outBytes(ncols * 3)
        pBILWriterOut = New clsBILFileWriter
        With pBILWriterOut
            .BILFileNameNoExt = outBILName
            .nbands = 3
            .nbits = 8
            .nrows = nrows
            .ncols = ncols
            .xdim = xdim
            .ydim = ydim
            .ulxmap = ulxmap
            .ulymap = ulymap
            .WriteHDRFile()
            .OpenFile()
        End With
        UpdateStatusLabel("Processing ... 0%")
        System.Windows.Forms.Application.DoEvents()
        pixelBlocks = nrows / 128
        stopWatch.Reset()
        stopWatch.Start()
        Do
            pPixelBlockColor = pRasterCursorColor.PixelBlock
            pPixelBlockShade = pRasterCursorShade.PixelBlock
            vPixelsShade = pPixelBlockShade.SafeArray(0)
            vPixelsColor = pPixelBlockColor.SafeArray(0)
            ColCount = UBound(vPixelsColor, 1)
            RowCount = UBound(vPixelsColor, 2)
            For RowIndex = 0 To RowCount
                For ColIndex = 0 To ColCount
                    Try
                        inPixelColor = CInt(CStr(pPixelBlockColor.GetVal(0, ColIndex, RowIndex)))
                        inPixelShade = CInt(CStr(pPixelBlockShade.GetVal(0, ColIndex, RowIndex)))
                    Catch ex As Exception
                        Dim fileName As String = pBILWriterOut.BILFileNameNoExt & "_RowColErrors.txt"
                        Dim sw As StreamWriter
                        If Not File.Exists(fileName) Then
                            sw = New StreamWriter(fileName, False)
                            sw.WriteLine("The following color or shade raster pixels could not be converted into integers")
                        End If
                        sw = New StreamWriter(fileName, True)
                        sw.WriteLine(RowIndex + "," + ColIndex)
                        sw.Close()
                        inPixelColor = 1
                        inPixelShade = pSatValMod.CutOff
                    End Try

                    If inPixelShade < 0 Then inPixelShade = 0
                    If inPixelShade > pSatValMod.ShadeRasterMax Then _
                        inPixelShade = pSatValMod.ShadeRasterMax

                    Try
                        colorMapIndex = pRasterColormap.Bin(CDbl(inPixelColor))
                        'pSatValMod.InRed = pRasterColormap.RedValues(colorMapIndex) * 255
                        'pSatValMod.InGreen = pRasterColormap.GreenValues(colorMapIndex) * 255
                        'pSatValMod.InBlue = pRasterColormap.BlueValues(colorMapIndex) * 255
                        pSatValMod.InRed = redValues(colorMapIndex)
                        pSatValMod.InGreen = greenValues(colorMapIndex)
                        pSatValMod.InBlue = blueValues(colorMapIndex)
                        pSatValMod.InShadeValue = CInt(inPixelShade)
                    Catch ex As Exception
                        pSatValMod.InRed = 0
                        pSatValMod.InGreen = 0
                        pSatValMod.InBlue = 0
                    End Try
                    pSatValMod.ModulateRGB()
                    outBytes(i) = Convert.ToByte(pSatValMod.OutRed)
                    outBytes(i + ncols) = Convert.ToByte(pSatValMod.OutGreen)
                    outBytes(i + ncols * 2) = Convert.ToByte(pSatValMod.OutBlue)
                    i = i + 1
                Next ColIndex
                pBILWriterOut.WriteRowBytes(outBytes, ncols * 3)
                i = 0
            Next RowIndex
            pRasterCursorShade.Next()
            pixleBlockComplete = pixleBlockComplete + 1
            percentComplete = (pixleBlockComplete / pixelBlocks) * 100
            UpdateStatusLabel("Processing ... " & percentComplete & "%")
            System.Windows.Forms.Application.DoEvents()
        Loop Until Not pRasterCursorColor.Next
        pBILWriterOut.CloseFile()
        stopWatch.Stop()
        UpdateStatusLabel(String.Format("Complete in {0} ms / {1:0} sec / {2:0.00} min",
                                        stopWatch.ElapsedMilliseconds,
                                        stopWatch.ElapsedMilliseconds / 1000,
                                        stopWatch.ElapsedMilliseconds / 60000))
    End Sub

    Public Sub ExecuteForRGBBands(pSatValMod As clsSatValMod, _
                                  R_RasterName As String, _
                                  G_RasterName As String, _
                                  B_RasterName As String, _
                                  ShadeGridName As String, _
                                  outBILName As String)
        Dim pRasterDataset_R As IRasterDataset
        Dim pRasterDataset_G As IRasterDataset
        Dim pRasterDataset_B As IRasterDataset
        Dim pGeoDatasetRGB As IGeoDataset
        Dim pOriginRGB As IPoint
        Dim pRaster_R As IRaster
        Dim pRaster_G As IRaster
        Dim pRaster_B As IRaster
        Dim pRasterPropsRGB As IRasterProps
        Dim ncols As Long
        Dim nrows As Long
        Dim pRasterAnalysisPropsRGB As IRasterAnalysisProps
        Dim xdim As Double
        Dim ydim As Double
        Dim ulxmap As Double
        Dim ulymap As Double
        Dim pRasterDatasetShade As IRasterDataset
        Dim pRasterShade As IRaster
        Dim pRasterCursor_R As IRasterCursor
        Dim pRasterCursor_G As IRasterCursor
        Dim pRasterCursor_B As IRasterCursor
        Dim pRasterCursorShade As IRasterCursor
        Dim pBILWriterOut As clsBILFileWriter
        Dim pPixelBlock_R As IPixelBlock
        Dim pPixelBlock_G As IPixelBlock
        Dim pPixelBlock_B As IPixelBlock
        Dim vPixels_R As Object
        Dim vPixels_G As Object
        Dim vPixels_B As Object
        Dim pPixelBlockShade As IPixelBlock
        Dim vPixelsShade As Object
        Dim RowCount As Long
        Dim ColCount As Long
        Dim RowIndex As Long
        Dim ColIndex As Long
        Dim inPixel_R As Object
        Dim inPixel_G As Object
        Dim inPixel_B As Object
        Dim pColorTransformer As New clsColorTransformer
        Dim h As Integer
        Dim s As Integer
        Dim v As Integer
        Dim inPixelShade As Object
        Dim pixelBlocks As Integer
        Dim pixleBlockComplete As Integer
        Dim percentComplete As Integer
        Dim stopWatch As New Stopwatch()
        Dim outBytes() As Byte
        Dim i As Integer

        If Not RasterDatasetExists(R_RasterName) Then
            MsgBox(R_RasterName & " does not exist.  Exiting")
            Return
        End If
        If Not RasterDatasetExists(G_RasterName) Then
            MsgBox(G_RasterName & " does not exist.  Exiting")
            Return
        End If
        If Not RasterDatasetExists(B_RasterName) Then
            MsgBox(B_RasterName & " does not exist.  Exiting")
            Return
        End If
        If Not RasterDatasetExists(ShadeGridName) Then
            MsgBox(ShadeGridName & " does not exist.  Exiting")
            Return
        End If
        pRasterDataset_R = GetRasterDataset(R_RasterName)
        pRasterDataset_G = GetRasterDataset(G_RasterName)
        pRasterDataset_B = GetRasterDataset(B_RasterName)
        pGeoDatasetRGB = pRasterDataset_R
        pRaster_R = pRasterDataset_R.CreateDefaultRaster
        pRaster_G = pRasterDataset_G.CreateDefaultRaster
        pRaster_B = pRasterDataset_B.CreateDefaultRaster
        pRasterPropsRGB = pRaster_R
        ncols = pRasterPropsRGB.Width
        nrows = pRasterPropsRGB.Height
        pRasterAnalysisPropsRGB = pRasterPropsRGB
        xdim = pRasterAnalysisPropsRGB.PixelWidth
        ydim = pRasterAnalysisPropsRGB.PixelHeight
        ulxmap = pGeoDatasetRGB.Extent.XMin + xdim / 2
        ulymap = pGeoDatasetRGB.Extent.YMax - ydim / 2
        pRasterDatasetShade = GetRasterDataset(ShadeGridName)
        pRasterShade = pRasterDatasetShade.CreateDefaultRaster
        pOriginRGB = GetRasterDatasetOrigin(pRasterDataset_R)
        pOriginRGB.Y = pOriginRGB.Y - nrows * ydim
        pRasterCursor_R = pRaster_R.CreateCursor
        pRasterCursor_G = pRaster_G.CreateCursor
        pRasterCursor_B = pRaster_B.CreateCursor
        pRasterCursorShade = pRasterShade.CreateCursor

        ReDim outBytes(ncols * 3)
        pBILWriterOut = New clsBILFileWriter
        With pBILWriterOut
            .BILFileNameNoExt = outBILName
            .nbands = 3
            .nbits = 8
            .nrows = nrows
            .ncols = ncols
            .xdim = xdim
            .ydim = ydim
            .ulxmap = ulxmap
            .ulymap = ulymap
            .WriteHDRFile()
            .OpenFile()
        End With
        UpdateStatusLabel("Processing ... 0%")
        System.Windows.Forms.Application.DoEvents()
        pixelBlocks = nrows / 128
        stopWatch.Reset()
        stopWatch.Start()
        Do
            pPixelBlock_R = pRasterCursor_R.PixelBlock
            pPixelBlock_G = pRasterCursor_G.PixelBlock
            pPixelBlock_B = pRasterCursor_B.PixelBlock
            pPixelBlockShade = pRasterCursorShade.PixelBlock
            vPixels_R = pPixelBlock_R.SafeArray(0)
            vPixels_G = pPixelBlock_G.SafeArray(0)
            vPixels_B = pPixelBlock_B.SafeArray(0)
            vPixelsShade = pPixelBlockShade.SafeArray(0)
            ColCount = UBound(vPixels_R, 1)
            RowCount = UBound(vPixels_R, 2)
            For RowIndex = 0 To RowCount
                For ColIndex = 0 To ColCount
                    Try
                        inPixel_R = CInt(CStr(pPixelBlock_R.GetVal(0, ColIndex, RowIndex)))
                        inPixel_G = CInt(CStr(pPixelBlock_G.GetVal(0, ColIndex, RowIndex)))
                        inPixel_B = CInt(CStr(pPixelBlock_B.GetVal(0, ColIndex, RowIndex)))
                        inPixelShade = CInt(CStr(pPixelBlockShade.GetVal(0, ColIndex, RowIndex)))
                    Catch ex As Exception
                        Dim fileName As String = pBILWriterOut.BILFileNameNoExt & "_RowColErrors.txt"
                        Dim sw As StreamWriter
                        If Not File.Exists(fileName) Then
                            sw = New StreamWriter(fileName, False)
                            sw.WriteLine("The following color or shade raster pixels could not be converted into integers")
                        End If
                        sw = New StreamWriter(fileName, True)
                        sw.WriteLine(RowIndex + "," + ColIndex)
                        sw.Close()
                        inPixel_R = 0
                        inPixel_G = 0
                        inPixel_B = 0
                        inPixelShade = pSatValMod.CutOff
                    End Try
                    If inPixelShade < 0 Then inPixelShade = 0
                    If inPixelShade > pSatValMod.ShadeRasterMax Then _
                        inPixelShade = pSatValMod.ShadeRasterMax
                    pColorTransformer.RGB2HSV(CInt(inPixel_R), _
                                              CInt(inPixel_G), _
                                              CInt(inPixel_B), _
                                              h, s, v)
                    pSatValMod.InHue = h
                    pSatValMod.InSat = s
                    pSatValMod.InValue = v
                    pSatValMod.InShadeValue = CInt(inPixelShade)
                    pSatValMod.ModulateHSV()
                    outBytes(i) = Convert.ToByte(pSatValMod.OutRed)
                    outBytes(i + ncols) = Convert.ToByte(pSatValMod.OutGreen)
                    outBytes(i + ncols * 2) = Convert.ToByte(pSatValMod.OutBlue)
                    i = i + 1
                Next ColIndex
                pBILWriterOut.WriteRowBytes(outBytes, ncols * 3)
                i = 0
            Next RowIndex
            pRasterCursorShade.Next()
            pRasterCursor_R.Next()
            pRasterCursor_G.Next()
            pixleBlockComplete = pixleBlockComplete + 1
            percentComplete = (pixleBlockComplete / pixelBlocks) * 100
            UpdateStatusLabel("Processing ... " & percentComplete & "%")
            System.Windows.Forms.Application.DoEvents()
        Loop Until Not pRasterCursor_B.Next
        pBILWriterOut.CloseFile()
        stopWatch.Stop()
        UpdateStatusLabel(String.Format("Complete in {0} ms / {1:0} sec / {2:0.00} min)",
                                        stopWatch.ElapsedMilliseconds,
                                        stopWatch.ElapsedMilliseconds / 1000,
                                        stopWatch.ElapsedMilliseconds / 60000))
    End Sub

    Private Sub ExecuteForColorGridTest()
        Dim svm As New clsSatValMod(0, 1, 0.3, 1, 71, 100)
        Dim f As New frmMain()
        ExecuteForColorGrid(svm, "D:\Projects\Archived\satvalmod_vba\data\dem255.bil", _
        "D:\Projects\Archived\satvalmod_vba\data\dem255.clr", _
        "D:\Projects\Archived\satvalmod_vba\data\shd100.bil", _
        "D:\Projects\Archived\satvalmod_vba\data\testaddin.bil")

    End Sub

    Public Sub ExecuteForColorGrid(pSatValMod As clsSatValMod, _
                                   ColorGridName As String, _
                                   CLRFileName As String, _
                                   ShadeGridName As String, _
                                   outBILName As String)
        Dim pRasterDatasetColor As IRasterDataset
        Dim pGeoDatasetColor As IGeoDataset
        Dim pOriginColor As IPoint
        Dim pRasterColor As IRaster
        Dim pRasterPropsColor As IRasterProps
        Dim ncols As Long
        Dim nrows As Long
        Dim pRasterAnalysisPropsColor As IRasterAnalysisProps
        Dim xdim As Double
        Dim ydim As Double
        Dim ulxmap As Double
        Dim ulymap As Double
        Dim pRasterDatasetShade As IRasterDataset
        Dim pRasterShade As IRaster
        Dim pRasterCursorColor As IRasterCursor
        Dim pRasterCursorShade As IRasterCursor
        'Dim outRowR As Byte()
        'Dim outRowG As Byte()
        'Dim outRowB As Byte()
        Dim pBILWriterOut As clsBILFileWriter
        Dim pPixelBlockColor As IPixelBlock
        Dim vPixelsColor As Object
        Dim pPixelBlockShade As IPixelBlock
        Dim vPixelsShade As Object
        Dim RowCount As Long
        Dim ColCount As Long
        Dim RowIndex As Long
        Dim ColIndex As Long
        Dim inPixelColor As Object = 0
        Dim inPixelShade As Object = 0
        Dim pixelBlocks As Integer
        Dim pixleBlockComplete As Integer
        Dim percentComplete As Integer
        Dim stopWatch As New Stopwatch()
        Dim outBytes() As Byte
        Dim i As Integer

        If Not RasterDatasetExists(ColorGridName) Then
            MsgBox(ColorGridName & " does not exist.  Exiting")
            Return
        End If
        If Not RasterDatasetExists(ShadeGridName) Then
            MsgBox(ShadeGridName & " does not exist.  Exiting")
            Return
        End If
        pRasterDatasetColor = GetRasterDataset(ColorGridName)
        pGeoDatasetColor = pRasterDatasetColor
        pRasterColor = pRasterDatasetColor.CreateDefaultRaster
        pRasterPropsColor = pRasterColor
        ncols = pRasterPropsColor.Width
        nrows = pRasterPropsColor.Height
        pRasterAnalysisPropsColor = pRasterPropsColor
        xdim = pRasterAnalysisPropsColor.PixelWidth
        ydim = pRasterAnalysisPropsColor.PixelHeight
        ulxmap = pGeoDatasetColor.Extent.XMin + xdim / 2
        ulymap = pGeoDatasetColor.Extent.YMax - ydim / 2
        pRasterDatasetShade = GetRasterDataset(ShadeGridName)
        pRasterShade = pRasterDatasetShade.CreateDefaultRaster
        pOriginColor = GetRasterDatasetOrigin(pRasterDatasetColor)
        pOriginColor.Y = pOriginColor.Y - nrows * ydim
        LoadHSVArrays(CLRFileName)
        pRasterCursorColor = pRasterColor.CreateCursor
        pRasterCursorShade = pRasterShade.CreateCursor

        ReDim outBytes(ncols * 3)
        pBILWriterOut = New clsBILFileWriter
        With pBILWriterOut
            .BILFileNameNoExt = outBILName
            .nbands = 3
            .nbits = 8
            .nrows = nrows
            .ncols = ncols
            .xdim = xdim
            .ydim = ydim
            .ulxmap = ulxmap
            .ulymap = ulymap
            .WriteHDRFile()
            .OpenFile()
        End With
        UpdateStatusLabel("Processing ... 0%")
        System.Windows.Forms.Application.DoEvents()
        pixelBlocks = nrows / 128
        stopWatch.Reset()
        stopWatch.Start()
        Do
            pPixelBlockColor = pRasterCursorColor.PixelBlock
            pPixelBlockShade = pRasterCursorShade.PixelBlock
            vPixelsShade = pPixelBlockShade.SafeArray(0)
            vPixelsColor = pPixelBlockColor.SafeArray(0)
            ColCount = UBound(vPixelsColor, 1) + 1
            RowCount = UBound(vPixelsColor, 2) + 1
            For RowIndex = 0 To RowCount - 1
                For ColIndex = 0 To ColCount - 1
                    Try
                        inPixelColor = CInt(pPixelBlockColor.GetVal(0, ColIndex, RowIndex))
                        inPixelShade = CInt(pPixelBlockShade.GetVal(0, ColIndex, RowIndex))
                    Catch ex As Exception
                        Dim fileName As String = pBILWriterOut.BILFileNameNoExt & "_RowColErrors.txt"
                        Dim sw As StreamWriter
                        If Not File.Exists(fileName) Then
                            sw = New StreamWriter(fileName, False)
                            sw.WriteLine("The following color or shade raster pixels could not be converted into integers")
                        End If
                        sw = New StreamWriter(fileName, True)
                        sw.WriteLine(RowIndex + "," + ColIndex)
                        sw.Close()
                        inPixelColor = 1
                        inPixelShade = pSatValMod.CutOff
                    End Try
                    If inPixelShade < 0 Then inPixelShade = 0
                    If inPixelShade > pSatValMod.ShadeRasterMax Then _
                        inPixelShade = pSatValMod.ShadeRasterMax
                    pSatValMod.InHue = m_h(inPixelColor)
                    pSatValMod.InSat = m_s(inPixelColor)
                    pSatValMod.InValue = m_v(inPixelColor)
                    pSatValMod.InShadeValue = inPixelShade
                    pSatValMod.ModulateHSV()
                    outBytes(i) = Convert.ToByte(pSatValMod.OutRed)
                    outBytes(i + ncols) = Convert.ToByte(pSatValMod.OutGreen)
                    outBytes(i + ncols * 2) = Convert.ToByte(pSatValMod.OutBlue)
                    i = i + 1
                Next ColIndex
                pBILWriterOut.WriteRowBytes(outBytes, ncols * 3)
                i = 0
            Next RowIndex
            pRasterCursorShade.Next()
            pixleBlockComplete = pixleBlockComplete + 1
            percentComplete = (pixleBlockComplete / pixelBlocks) * 100
            UpdateStatusLabel("Processing ... " & percentComplete & "%")
            System.Windows.Forms.Application.DoEvents()
        Loop Until Not pRasterCursorColor.Next
        pBILWriterOut.CloseFile()
        stopWatch.Stop()
        UpdateStatusLabel(String.Format("Complete in {0} ms / {1:0} sec / {2:0.00} min)",
                                        stopWatch.ElapsedMilliseconds,
                                        stopWatch.ElapsedMilliseconds / 1000,
                                        stopWatch.ElapsedMilliseconds / 60000))
        Exit Sub
    End Sub

    Public Sub ExecuteForColorGrid2(pSatValMod As clsSatValMod,
                                    ColorGridName As String,
                                    CLRFileName As String,
                                    ShadeGridName As String,
                                    outBILName As String)
        Dim pRasterDatasetColor As IRasterDataset
        Dim pGeoDatasetColor As IGeoDataset
        Dim pOriginColor As IPoint
        Dim pRasterColor As IRaster
        Dim pRasterPropsColor As IRasterProps
        Dim ncols As Long
        Dim nrows As Long
        Dim pRasterAnalysisPropsColor As IRasterAnalysisProps
        Dim xdim As Double
        Dim ydim As Double
        Dim ulxmap As Double
        Dim ulymap As Double
        Dim pRasterDatasetShade As IRasterDataset
        Dim pRasterShade As IRaster
        Dim pRasterCursorColor As IRasterCursor
        Dim pRasterCursorShade As IRasterCursor
        Dim outRowR As String
        Dim outRowG As String
        Dim outRowB As String
        Dim pStepProgressor As IStepProgressor
        Dim pBILWriterOut As clsBILFileWriter
        Dim pPixelBlockColor As IPixelBlock
        Dim vPixelsColor As Object
        Dim pPixelBlockShade As IPixelBlock
        Dim vPixelsShade As Object
        Dim RowCount As Long
        Dim ColCount As Long
        Dim RowIndex As Long
        Dim ColIndex As Long
        Dim inPixelColor As Object
        Dim inPixelShade As Object
        Dim pixelBlocks As Integer
        Dim pixleBlockComplete As Integer
        Dim percentComplete As Integer

        On Error GoTo ErrorHandler
        If Not RasterDatasetExists(ColorGridName) Then
            MsgBox(ColorGridName & " does not exist.  Exiting")
            Return
        End If
        If Not RasterDatasetExists(ShadeGridName) Then
            MsgBox(ShadeGridName & " does not exist.  Exiting")
            Return
        End If
        pRasterDatasetColor = GetRasterDataset(ColorGridName)
        pGeoDatasetColor = pRasterDatasetColor
        pRasterColor = pRasterDatasetColor.CreateDefaultRaster
        pRasterPropsColor = pRasterColor
        ncols = pRasterPropsColor.Width
        nrows = pRasterPropsColor.Height
        pRasterAnalysisPropsColor = pRasterPropsColor
        xdim = pRasterAnalysisPropsColor.PixelWidth
        ydim = pRasterAnalysisPropsColor.PixelHeight
        ulxmap = pGeoDatasetColor.Extent.XMin + xdim / 2
        ulymap = pGeoDatasetColor.Extent.YMax - ydim / 2
        pRasterDatasetShade = GetRasterDataset(ShadeGridName)
        pRasterShade = pRasterDatasetShade.CreateDefaultRaster
        pOriginColor = GetRasterDatasetOrigin(pRasterDatasetColor)
        pOriginColor.Y = pOriginColor.Y - nrows * ydim
        LoadHSVArrays(CLRFileName)
        pRasterCursorColor = pRasterColor.CreateCursor
        pRasterCursorShade = pRasterShade.CreateCursor
        pBILWriterOut = New clsBILFileWriter
        With pBILWriterOut
            .BILFileNameNoExt = outBILName
            .OpenFile()
        End With
        outRowR = Space(ncols)
        outRowG = Space(ncols)
        outRowB = Space(ncols)
        With pBILWriterOut
            .nbands = 3
            .nbits = 8
            .nrows = nrows
            .ncols = ncols
            .xdim = xdim
            .ydim = ydim
            .ulxmap = ulxmap
            .ulymap = ulymap
            .WriteHDRFile()
        End With
        UpdateStatusLabel("Processing ... 0%")
        pixelBlocks = nrows / 128
        Do
            pPixelBlockColor = pRasterCursorColor.PixelBlock
            pPixelBlockShade = pRasterCursorShade.PixelBlock
            vPixelsShade = pPixelBlockShade.SafeArray(0)
            vPixelsColor = pPixelBlockColor.SafeArray(0)
            ColCount = UBound(vPixelsColor, 1)
            RowCount = UBound(vPixelsColor, 2)
            For RowIndex = 0 To RowCount
                For ColIndex = 0 To ColCount
                    inPixelColor = CInt(CStr(pPixelBlockColor.GetVal(0, ColIndex, RowIndex)))
                    inPixelShade = CInt(CStr(pPixelBlockShade.GetVal(0, ColIndex, RowIndex)))
                    If inPixelShade < 0 Then inPixelShade = 0
                    If inPixelShade > pSatValMod.ShadeRasterMax Then _
                        inPixelShade = pSatValMod.ShadeRasterMax
                    pSatValMod.InHue = m_h(inPixelColor)
                    pSatValMod.InSat = m_s(inPixelColor)
                    pSatValMod.InValue = m_v(inPixelColor)
                    pSatValMod.InShadeValue = CInt(inPixelShade)
                    pSatValMod.ModulateHSV()
                    Mid(outRowR, ColIndex + 1, 1) = Chr(pSatValMod.OutRed)
                    Mid(outRowG, ColIndex + 1, 1) = Chr(pSatValMod.OutGreen)
                    Mid(outRowB, ColIndex + 1, 1) = Chr(pSatValMod.OutBlue)
                Next ColIndex
                pBILWriterOut.WriteRow(outRowR)
                pBILWriterOut.WriteRow(outRowG)
                pBILWriterOut.WriteRow(outRowB)
            Next RowIndex
            pRasterCursorShade.Next()
            pixleBlockComplete = pixleBlockComplete + 1
            percentComplete = (pixleBlockComplete / pixelBlocks) * 100
            UpdateStatusLabel("Processing ... " & percentComplete & "%")
        Loop Until Not pRasterCursorColor.Next
        pBILWriterOut.CloseFile()
        UpdateStatusLabel("")
        Exit Sub
ErrorHandler:
        If Err.Number = 5 Then
            Resume Next
        ElseIf Err.Number = 13 Then
            Dim fileName As String = pBILWriterOut.BILFileNameNoExt & "_RowColErrors.txt"
            Dim sw As New StreamWriter(fileName, True)
            inPixelColor = 0
            sw.WriteLine(RowIndex + "," + ColIndex)
            sw.Close()
            Resume Next
        ElseIf Err.Number = 9 Then
            MsgBox("Your Shade raster seems to have values greater than 100." & vbCrLf &
                   "Change this and re-run." & vbCrLf &
                   "NOTE:  Adjust your cutoff accordingly.")
        Else
            Err.Raise(Err.Number, Err.Description)
        End If
    End Sub

    Private Sub LoadHSVArrays(fullCLRFileName As String)
        Dim pColorTransformer As clsColorTransformer
        Dim sr As StreamReader
        Dim fileNum As Integer
        Dim inRec As String
        Dim strRGB() As String
        Dim i As Integer
        Dim r As Integer
        Dim g As Integer
        Dim b As Integer

        pColorTransformer = New clsColorTransformer
        sr = New StreamReader(fullCLRFileName)
        fileNum = FreeFile()
        inRec = sr.ReadLine()
        Do Until sr.EndOfStream
            inRec = Trim(inRec)
            If Len(inRec) = 0 Then
                inRec = sr.ReadLine()
                Continue Do
            End If
            If Left(inRec, 1) = "#" Then
                inRec = sr.ReadLine()
                Continue Do
            End If
            strRGB = Split(inRec, " ")
            i = strRGB(0)
            r = strRGB(1)
            g = strRGB(2)
            b = strRGB(3)
            inRec = sr.ReadLine()
            pColorTransformer.RGB2HSV(r, g, b, m_h(i), m_s(i), m_v(i))
        Loop
        sr.Close()
    End Sub

    Public Function SetSatValModParams(Vmin As Double, _
                                  Vexp As Double, _
                                  Smin As Double, _
                                  Sexp As Double, _
                                  CutOff As Integer, _
                                  ShadeRasterMax As Integer) As clsSatValMod
        SetSatValModParams = New clsSatValMod
        With SetSatValModParams
            .Vmin = Vmin
            .Vexp = Vexp
            .Smin = Smin
            .Sexp = Sexp
            .CutOff = CutOff
            .ShadeRasterMax = ShadeRasterMax
            .Init()
        End With
    End Function

    Private Function RasterDatasetExists(FullRasterPath As String) As Boolean
        On Error GoTo ErrorHandler
        Dim pStringUtils As clsStringUtils
        Dim aPath As String
        Dim aRasterFile As String
        Dim pWSFactory As IWorkspaceFactory
        Dim pRasterWS As IRasterWorkspace2
        Dim pRasterDataset As IRasterDataset

        pStringUtils = New clsStringUtils
        aPath = pStringUtils.GetPath(FullRasterPath)
        aRasterFile = pStringUtils.GetFileNameOnly(FullRasterPath)
        pWSFactory = New RasterWorkspaceFactory
        pRasterWS = pWSFactory.OpenFromFile(aPath, 0)
        pRasterDataset = pRasterWS.OpenRasterDataset(aRasterFile)
        RasterDatasetExists = True
        pWSFactory = Nothing
        pRasterWS = Nothing
        pRasterDataset = Nothing
        Exit Function
ErrorHandler:
        RasterDatasetExists = False
    End Function

    Private Function GetRasterDatasetOrigin(pRasterDataset As IRasterDataset3) As IPoint
        Dim pRasterValue As IRasterValue
        Dim pRasterStorageDef As IRasterStorageDef

        pRasterValue = New RasterValue
        pRasterValue.RasterDataset = pRasterDataset
        pRasterStorageDef = pRasterValue.RasterStorageDef
        GetRasterDatasetOrigin = pRasterStorageDef.Origin
    End Function

    Private Function GetRasterDatasetOrigin(pRasterDataset As IRasterDataset) As IPoint
        Dim pGeoDataset As IGeoDataset
        Dim pPoint As IPoint

        pGeoDataset = pRasterDataset
        pPoint = New Point
        pPoint.X = pGeoDataset.Extent.XMin
        pPoint.Y = pGeoDataset.Extent.YMin
        GetRasterDatasetOrigin = pPoint
    End Function

    Private Function GetRasterDataset(FullRasterPath As String) As IRasterDataset
        On Error GoTo ErrorHandler
        Dim aPath As String
        Dim aRasterFile As String
        Dim pWSFactory As IWorkspaceFactory
        Dim pRasterWS As IRasterWorkspace2

        aPath = GetPath(FullRasterPath)
        aRasterFile = GetFileNameOnly(FullRasterPath)
        pWSFactory = New RasterWorkspaceFactory
        pRasterWS = pWSFactory.OpenFromFile(aPath, 0)
        GetRasterDataset = pRasterWS.OpenRasterDataset(aRasterFile)
        Exit Function
ErrorHandler:
        MsgBox("Error in GetRasterDataset()", vbCritical)
    End Function

    Public Function GetShadeRasterMax(FullRasterPath As String) As Double
        Dim pRasterDataset As IRasterDataset
        Dim pRasterBandCollection As IRasterBandCollection
        Dim pRasterBand As IRasterBand
        Dim pRasterStats As IRasterStatistics
        Dim rasterMax As Double

        pRasterDataset = GetRasterDataset(FullRasterPath)
        pRasterBandCollection = pRasterDataset
        pRasterBand = pRasterBandCollection.Item(0)
        pRasterStats = pRasterBand.Statistics
        If pRasterStats Is Nothing Then
            GetShadeRasterMax = 255
            Exit Function
        End If
        rasterMax = pRasterStats.Maximum
        Select Case rasterMax
            Case Is > 255
                GetShadeRasterMax = -1
            Case Is <= 100
                GetShadeRasterMax = 100
            Case Is <= 255
                GetShadeRasterMax = 255
            Case Else
                GetShadeRasterMax = -1
        End Select
    End Function

    Private Function GetPath(FullFileName As String) As String
        Dim FileParts() As String

        FileParts = Split(FullFileName, "\")
        GetPath = Left(FullFileName, Len(FullFileName) - Len(FileParts(UBound(FileParts))))
    End Function

    Private Function GetFileNameOnly(FullFileName As String) As String
        Dim FileParts() As String

        FileParts = Split(FullFileName, "\")
        GetFileNameOnly = FileParts(UBound(FileParts))
    End Function

    Private Function SetupProgressDialog(maxRange As Long) As IProgressor
        Dim pProgressDialogFact As IProgressDialogFactory
        Dim pStepProgressor As IStepProgressor
        pProgressDialogFact = New ProgressDialogFactory
        pStepProgressor = pProgressDialogFact.Create(Nothing, 0)
        With pStepProgressor
            .MaxRange = maxRange
            .StepValue = 1
            .Message = "SatValMod processing ..."
        End With
        SetupProgressDialog = pStepProgressor
    End Function

    Private Sub UpdateStatusLabel(msg As String)
        If Not m_mainForm Is Nothing Then
            m_mainForm.lblStatus.Text = "  " & msg
        End If
    End Sub
End Module
