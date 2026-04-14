Imports System.Windows.Forms

Public Class frmHelp

    Private Sub btnClose_Click(sender As System.Object, e As System.EventArgs) Handles btnClose.Click
        Me.Dispose()
    End Sub

    Private Sub frmHelp_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        lblHelpText.Text = GetHelpTextForHotSpot("")
    End Sub

    Private Sub picBoxSVMParams_Click(sender As System.Object, e As System.EventArgs) Handles picBoxSVMParams.Click
        lblHelpText.Text = MousePosition.X & "," & MousePosition.Y
    End Sub

    Private Sub picBoxSVMParams_MouseUp(sender As System.Object, e As MouseEventArgs) Handles picBoxSVMParams.MouseUp
        lblHelpText.Text = e.X & "," & e.Y
        If MouseIsOverHotSpot(e.X, e.Y) Then
            picBoxMain.Image = GetHelpImage(e.X, e.Y)
            lblHelpText.Text = GetHelpTextForHotSpot(GetHotSpotName(e.X, e.Y))
        End If
    End Sub

    Private Function GetHelpTextForHotSpot(hotSpotName As String) As String
        Dim helpText As String
        Dim nl As String = Environment.NewLine
        Select Case hotSpotName
            Case "Cutoff"
                helpText = "Cutoff: The shade raster value where value modulation stops "
                helpText += "and saturation modulation begins.  The best value is the "
                helpText += "maxima of the shade raster's histogram" + nl
                helpText += "For a digital elevation model, this is usually" + nl
                helpText += "    sin(alt) * shadeMax" + nl
                helpText += "where alt = Altitude of illumination source)" + nl
                helpText += " shadeMax = Max pixel value of shade raster" + nl + nl
                helpText += "The x-axis in the above graph represents the pixel values "
                helpText += "of the shade raster"
            Case "Vmin"
                helpText = "Vmin: Determines the darkness of the shadows" + nl + nl
                helpText += "The x-axis in the above graph represents the pixel values "
                helpText += "of the shade raster"
            Case "Vexp"
                helpText = "Vexp: Determines the amount of shadow" + nl + nl
                helpText += "The x-axis in the above graph represents the pixel values "
                helpText += "of the shade raster"
            Case "ShadeMax"
                helpText = "The maximum pixel value of the shade raster." + nl
                helpText += "Usually 255"
            Case "Smin"
                helpText = "Smin: The color saturation in areas of high illumination" + nl
                helpText += "(The brightness of the gloss)" + nl + nl
                helpText += "The x-axis in the above graph represents the pixel values "
                helpText += "of the shade raster"
            Case "Sexp"
                helpText = "Sexp: The amount of area with low saturation" + nl
                helpText += "(The amount of gloss)" + nl + nl
                helpText += "The x-axis in the above graph represents the pixel values "
                helpText += "of the shade raster"
            Case Else
                helpText = "The Sat-Val Modulation image integration method involves "
                helpText += "the following steps:" + nl
                helpText += "- transforming color image, or RGB color images, to HSV" + nl
                helpText += "  coordinates" + nl
                helpText += "- modulating the S and V components with multipliers" + nl
                helpText += "  determined by the value in the shade raster" + nl
                helpText += "- transform modulated HSV back to RGB coordinates" + nl + nl
                helpText += "Click on the parameters in the image to the left "
                helpText += "to find more detail on a given parameter"
        End Select
        Return helpText
    End Function

    Private Sub picBoxSVMParams_MouseMove(sender As System.Object, e As MouseEventArgs) Handles picBoxSVMParams.MouseMove
        If MouseIsOverHotSpot(e.X, e.Y) Then
            Me.Cursor = Cursors.Hand
        Else
            Me.Cursor = Cursors.Default
        End If
    End Sub

    Private Function GetHotSpotName(x As Integer, y As Integer) As String
        Dim hotSpotName As String = ""

        If x >= 35 And x <= 128 Then
            If y >= 23 And y <= 43 Then
                hotSpotName = "Cutoff"
            End If
            If y >= 51 And y <= 72 Then
                hotSpotName = "Vmin"
            End If
            If y >= 78 And y <= 100 Then
                hotSpotName = "Vexp"
            End If
        End If
        If x >= 266 And x <= 375 Then
            If y >= 23 And y <= 43 Then
                hotSpotName = "ShadeMax"
            End If
            If y >= 51 And y <= 72 Then
                hotSpotName = "Smin"
            End If
            If y >= 78 And y <= 100 Then
                hotSpotName = "Sexp"
            End If
        End If
        Return hotSpotName
    End Function

    Private Function MouseIsOverHotSpot(x As Integer, y As Integer) As Boolean
        Return GetHotSpotName(x, y) <> ""
    End Function

    Private Function GetHelpImage(x As Integer, y As Integer) As System.Drawing.Bitmap
        Dim img As System.Drawing.Bitmap = My.Resources.SVMOverview
        Dim hotSpotName As String = GetHotSpotName(x, y)

        Select Case hotSpotName
            Case "Cutoff"
                img = My.Resources.Cutoff
            Case "Vmin"
                img = My.Resources.Vmin
            Case "Vexp"
                img = My.Resources.Vexp
            Case "ShadeMax"
                img = My.Resources.SVMOverview
            Case "Smin"
                img = My.Resources.Smin
            Case "Sexp"
                img = My.Resources.Sexp
            Case Else
                img = My.Resources.SVMOverview
        End Select
        Return img
    End Function

    Private Sub btnReset_Click(sender As System.Object, e As System.EventArgs) Handles btnReset.Click
        picBoxMain.Image = My.Resources.SVMOverview
        lblHelpText.Text = GetHelpTextForHotSpot("")
    End Sub
End Class

