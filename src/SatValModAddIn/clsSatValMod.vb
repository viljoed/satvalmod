Imports System.IO

Public Class clsSatValMod
    Public InRed As Integer               ' 0 <= InRed <= 255
    Public InGreen As Integer             ' 0 <= InGreen <= 255
    Public InBlue As Integer              ' 0 <= InBlue <= 255
    Public InHue As Integer               ' 0 <= InHue <= 360
    Public InSat As Integer               ' 0 <= InSat <= 100
    Public InValue As Integer             ' 0 <= InValue <= 100
    Public InShadeValue As Integer        ' 0 <= InShadeValue <= 255
    Public Vmin As Double                 ' 0 <= Vmin <= 1
    Public Vexp As Double                 ' 0 < Vexp < 10
    Public Smin As Double                 ' 0 <= Smin <= 1
    Public Sexp As Double                 ' 0 < Sexp < 10
    Public CutOff As Integer              ' 0 < CutOff <= 255
    Public ShadeRasterMax As Integer      ' 100 or 255
    Public OutRed As Integer              ' 0 <= OutRed <= 255
    Public OutGreen As Integer            ' 0 <= OutGreen <= 255
    Public OutBlue As Integer             ' 0 <= OutBlue <= 255
    Dim m_pColorTransformer As clsColorTransformer
    Private m_vm(256) As Double
    Private m_sm(256) As Double

    Public Sub New()

    End Sub

    Public Sub New(vMin As Double, vExp As Double, sMin As Double, sExp As Double, _
                   cutoff As Integer, shadeRasterMax As Integer)
        Me.Vmin = vMin
        Me.Vexp = vExp
        Me.Smin = sMin
        Me.Sexp = sExp
        Me.CutOff = cutoff
        Me.ShadeRasterMax = shadeRasterMax
    End Sub

    Public Sub Init()
        LoadSatModulationArray(m_sm, Smin, Sexp, CutOff)
        LoadValModulationArray(m_vm, Vmin, Vexp, CutOff)
        m_pColorTransformer = New clsColorTransformer
    End Sub

    Public Sub DumpValueModulationArray(OutFileName As String)
        DumpDoubleArrayToFile(m_vm, OutFileName)
    End Sub

    Public Sub DumpSaturationModulationArray(OutFileName As String)
        DumpDoubleArrayToFile(m_sm, OutFileName)
    End Sub

    Private Sub DumpDoubleArrayToFile(anArray() As Double, outFileName As String)
        Dim i As Long
        Dim itemcount As Long
        Dim sw As StreamWriter = New StreamWriter(outFileName)

        itemcount = Me.ShadeRasterMax
        For i = 0 To itemcount
            sw.WriteLine(i & "," & anArray(i))
        Next i
    End Sub

    Public Sub ModulateHSV()
        Dim SatMod As Double
        Dim ValMod As Double
        Dim r As Integer
        Dim g As Integer
        Dim b As Integer

        SatMod = Me.InSat * m_sm(InShadeValue)
        ValMod = Me.InValue * m_vm(InShadeValue)
        Me.InSat = CInt(SatMod)
        Me.InValue = CInt(ValMod)
        m_pColorTransformer.HSV2RGB(Me.InHue, Me.InSat, Me.InValue, _
                                     r, g, b)
        Me.OutRed = r
        Me.OutGreen = g
        Me.OutBlue = b
    End Sub

    Public Sub ModulateRGB()
        m_pColorTransformer.RGB2HSV(InRed, InGreen, InBlue, InHue, InSat, InValue)
        ModulateHSV()
    End Sub

    Private Sub LoadValModulationArray(ByRef ValMod() As Double, _
                                       Vmin As Double, _
                                       Vexp As Double, _
                                       CutOff As Integer)
        Dim i As Integer
        For i = 0 To Me.ShadeRasterMax
            If i <= CutOff Then
                ValMod(i) = Vmin + ((1 - Vmin) * ((i / CutOff) ^ Vexp))
            Else
                ValMod(i) = 1
            End If
        Next i
    End Sub

    Private Sub LoadSatModulationArray(ByRef SatMod() As Double, _
                                       Smin As Double, _
                                       Sexp As Double, _
                                       CutOff As Integer)
        Dim i As Integer
        For i = 0 To Me.ShadeRasterMax
            If i <= CutOff Then
                SatMod(i) = 1
            Else
                SatMod(i) = 1 - (1 - Smin) * ((i - CutOff) / (Me.ShadeRasterMax - CutOff)) ^ Sexp
            End If
        Next i
    End Sub

End Class
