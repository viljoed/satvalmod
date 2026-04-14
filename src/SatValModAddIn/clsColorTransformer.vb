Imports ESRI.ArcGIS.Display

Public Class clsColorTransformer
    Private Const PI As Double = 3.14159265358979
    Private pHSVColor As New HsvColor
    Private pRGBColor As New RgbColor

    Public Sub RGB2HSV2(r As Integer, g As Integer, b As Integer, _
                       ByRef h As Integer, ByRef s As Integer, ByRef v As Integer)
        Dim X As Double
        Dim Y As Double
        Dim dblR As Double
        Dim dblG As Double
        Dim dblB As Double
        Dim Gx As Double
        Dim Bx As Double
        Dim Gy As Double
        Dim By As Double
        dblR = r / 255
        dblG = g / 255
        dblB = b / 255
        If g = 0 Then
            Gx = 0
            Gy = 0
        Else
            Gx = 0.5 * dblG
            Gy = ((3 ^ 0.5) / (2)) * dblG
        End If
        If b = 0 Then
            Bx = 0
            By = 0
        Else
            Bx = 0.5 * dblB
            By = ((3 ^ 0.5) / (2)) * dblB
        End If
        X = dblR - Gx - Bx
        Y = Gy - By
        h = Rad2Deg(Atan2(X, Y))
        s = (X ^ 2 + Y ^ 2) ^ 0.5 * 100
        v = MaxOfRGB(dblR, dblG, dblB) * 100
    End Sub

    Public Sub RGB2HSV(r As Integer, g As Integer, b As Integer, _
                       ByRef h As Integer, ByRef s As Integer, ByRef v As Integer)
        pRGBColor.Red = r
        pRGBColor.Green = g
        pRGBColor.Blue = b
        pHSVColor.RGB = pRGBColor.RGB
        h = pHSVColor.Hue
        s = pHSVColor.Saturation
        v = pHSVColor.Value
    End Sub

    Public Sub HSV2RGB(h As Integer, s As Integer, v As Integer, _
                       ByRef r As Integer, ByRef g As Integer, ByRef b As Integer)
        pHSVColor.Hue = h
        pHSVColor.Saturation = s
        pHSVColor.Value = v
        pRGBColor.RGB = pHSVColor.RGB
        r = pRGBColor.Red
        g = pRGBColor.Green
        b = pRGBColor.Blue
    End Sub

    Public Sub hsv2rgb2(h As Integer, s As Integer, v As Integer, _
                       ByRef r As Integer, ByRef g As Integer, ByRef b As Integer)
        Dim dblR As Double
        Dim dblG As Double
        Dim dblB As Double
        Dim InSat As Double
        Dim InVal As Double
        Dim intervalNum As Integer   '60 deg interval, 0-60 = 0, 60-120 = 1, etc.
        Dim f As Double
        Dim negativeSlope As Double
        Dim positiveSlope As Double
        Dim minimumRGB As Double

        InSat = s / 100
        InVal = v / 100
        If InSat = 0 Then
            dblR = InVal
            dblG = InVal
            dblB = InVal
            r = dblR * 255
            g = dblG * 255
            b = dblB * 255
            Exit Sub
        End If
        intervalNum = Int(h / 60)
        f = (h / 60) - intervalNum
        minimumRGB = InVal * (1 - InSat)
        negativeSlope = InVal * (1 - (InSat * f))
        positiveSlope = InVal * (1 - (InSat * (1 - f)))
        Select Case intervalNum
            Case 0
                dblR = InVal
                dblG = positiveSlope
                dblB = minimumRGB
            Case 1
                dblR = negativeSlope
                dblG = InVal
                dblB = minimumRGB
            Case 2
                dblR = minimumRGB
                dblG = InVal
                dblB = positiveSlope
            Case 3
                dblR = minimumRGB
                dblG = negativeSlope
                dblB = InVal
            Case 4
                dblR = positiveSlope
                dblG = minimumRGB
                dblB = InVal
            Case 5, 6
                dblR = InVal
                dblG = minimumRGB
                dblB = negativeSlope
        End Select
        r = CInt(dblR * 255)
        g = CInt(dblG * 255)
        b = CInt(dblB * 255)
    End Sub

    Private Function Atan2(ByVal X As Double, ByVal Y As Double) As Double
        Dim Theta As Double

        If (Math.Abs(X) < 0.0000001) Then
            If (Math.Abs(Y) < 0.0000001) Then
                Theta = 0.0#
            ElseIf (Y > 0.0#) Then
                Theta = PI / 2
            Else
                Theta = -PI / 2
            End If
        Else
            Theta = Math.Atan(Y / X)

            If (X < 0) Then
                If (Y >= 0.0#) Then
                    Theta = PI + Theta
                Else
                    Theta = Theta - PI
                End If
            End If
        End If
        Atan2 = Theta
    End Function

    Private Function Rad2Deg(Radians As Double) As Double
        Rad2Deg = Radians * 180 / PI
        If Radians < 0 Then
            Rad2Deg = 360 + Rad2Deg
        End If
    End Function

    Private Function MaxOfRGB(r As Double, g As Double, b As Double) As Double
        If r >= g And r >= b Then MaxOfRGB = r
        If g >= r And g >= b Then MaxOfRGB = g
        If b >= r And b >= g Then MaxOfRGB = b
    End Function
End Class
