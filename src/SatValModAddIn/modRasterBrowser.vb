Imports ESRI.ArcGIS.Catalog
Imports ESRI.ArcGIS.CatalogUI

Module modRasterBrowser
    Public Function BrowseForTIFRaster(Optional title As String = "Select TIFF") As String
        Dim pGxDlg As IGxDialog
        Dim pGxTIFFilter As IGxObjectFilter
        Dim pEnumGxObj As IEnumGxObject
        Dim pGxCatalog As IGxCatalog
        Dim pGxObj As IGxObject

        pGxDlg = New GxDialog
        pGxTIFFilter = New RasterFormatTifFilter
        pGxDlg.ObjectFilter = pGxTIFFilter
        pGxDlg.Title = title
        pGxDlg.AllowMultiSelect = False
        'Display the dialog to allow the user to select datasets
        pEnumGxObj = Nothing
        pGxDlg.DoModalOpen(0, pEnumGxObj)
        'Place the GxObjects into an array to put into the SidEncoder
        pGxCatalog = pGxDlg.InternalCatalog
        pEnumGxObj = pGxCatalog.Selection.SelectedObjects
        pGxObj = pEnumGxObj.Next
        If Not pGxObj Is Nothing Then
            BrowseForTIFRaster = pGxObj.FullName
        Else
            BrowseForTIFRaster = ""
        End If
    End Function

    Public Function BrowseForAnyRaster(Optional title As String = "Select raster dataset") As String
        Dim pGxDlg As IGxDialog
        Dim pGxRasterFilter As IGxObjectFilter
        Dim pEnumGxObj As IEnumGxObject
        Dim pGxCatalog As IGxCatalog
        Dim pGxObj As IGxObject

        pGxDlg = New GxDialog
        pGxRasterFilter = New GxFilterRasterDatasets
        pGxDlg.ObjectFilter = pGxRasterFilter
        pGxDlg.Title = title
        pGxDlg.AllowMultiSelect = False
        'Display the dialog to allow the user to select datasets
        pEnumGxObj = Nothing
        pGxDlg.DoModalOpen(0, pEnumGxObj)
        'Place the GxObjects into an array to put into the SidEncoder
        pGxCatalog = pGxDlg.InternalCatalog
        pEnumGxObj = pGxCatalog.Selection.SelectedObjects
        pGxObj = pEnumGxObj.Next
        If Not pGxObj Is Nothing Then
            BrowseForAnyRaster = pGxObj.FullName
        Else
            BrowseForAnyRaster = ""
        End If
    End Function
End Module
