Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.DataSourcesRaster
Imports ESRI.ArcGIS.Geodatabase

Public Class RasUtils
    Public Function CreateRasterDataset(ByVal Path As String, ByVal FileName As String) As IRasterDataset
        Try
            Dim ws As IRasterWorkspace2
            ws = OpenRasterworkspace(Path)

            'Define the spatial reference of the raster dataset.
            Dim sr As ISpatialReference
            sr = New UnknownCoordinateSystem

            'Define the origin for the raster dataset, which is the lower left corner of the raster.
            Dim origin As IPoint
            origin = New Point
            origin.PutCoords(15.0, 15.0)

            'Define the dimensions of the raster dataset.
            Dim Width As Integer 'The width of the raster dataset.
            Dim height As Integer 'The height of the raster dataset.
            Dim xCell As Double 'The cell size in x direction.
            Dim yCell As Double 'The cell size in y direction.
            Dim NumBand As Integer 'The number of bands the raster dataset contains.
            Width = 100
            height = 100
            xCell = 30
            yCell = 30
            NumBand = 1

            'Create a raster dataset in grid format.
            Dim rasterDS As IRasterDataset
            rasterDS = ws.CreateRasterDataset(FileName, "GRID", origin, Width, height, xCell, yCell, NumBand, rstPixelType.PT_UCHAR, sr, True)

            'Get the raster band.
            Dim bands As IRasterBandCollection
            Dim band As IRasterBand
            Dim rasterProps As IRasterProps
            bands = rasterDS
            band = bands.Item(0)
            rasterProps = band

            'Set NoData if necessary. For a multiband image, a NoData value needs to be set for each band.
            rasterProps.NoDataValue = 255

            'Create a raster from the dataset.
            Dim raster As IRaster
            raster = rasterDS.CreateFullRaster

            'Create a pixel block using the weight and height of the raster dataset.
            'If the raster dataset is large, a smaller pixel block should be used.
            'Refer to the topic "How to access pixel data using a raster cursor".
            Dim pixelBlock As IPixelBlock3
            Dim blockSize As IPnt
            blockSize = New Pnt
            blockSize.SetCoords(Width, height)
            pixelBlock = raster.CreatePixelBlock(blockSize)

            'Populate some pixel values to the pixel block.
            Dim pixels As System.Array
            Dim i, j As Integer
            pixels = CType(pixelBlock.PixelData(0), System.Array)
            For i = 0 To Width - 1
                For j = 0 To height - 1
                    If i = j Then
                        pixels.SetValue(Convert.ToByte(255), i, j)
                    Else
                        pixels.SetValue(Convert.ToByte((i * j) / 255), i, j)
                    End If
                Next j
            Next i
            pixelBlock.PixelData(0) = CType(pixels, System.Array)

            'Define the location that the upper left corner of the pixel block is to write.
            Dim upperLeft As IPnt
            upperLeft = New Pnt
            upperLeft.SetCoords(0, 0)

            'Write the pixel block.
            Dim rasterEdit As IRasterEdit
            rasterEdit = raster
            rasterEdit.Write(upperLeft, pixelBlock)

            'Release rasterEdit explicitly.
            System.Runtime.InteropServices.Marshal.ReleaseComObject(rasterEdit)

            Return rasterDS
        Catch ex As Exception
            System.Diagnostics.Trace.WriteLine(ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function OpenRasterworkspace(ByVal Path As String) As IRasterWorkspace2
        Try
            Dim workspaceFact As IWorkspaceFactory
            Dim rasterWS As IRasterWorkspace2
            workspaceFact = New RasterWorkspaceFactory
            rasterWS = workspaceFact.OpenFromFile(Path, 0)

            Return rasterWS
        Catch ex As Exception
            System.Diagnostics.Trace.WriteLine(ex.Message)
            Return Nothing
        End Try
    End Function
End Class
