Imports Microsoft.VisualBasic
Imports System.IO

Public Class clsBILFileWriter
    Public BILFileNameNoExt As String
    Public nrows As Long
    Public ncols As Long
    Public nbits As Integer
    Public nbands As Integer
    Public xdim As Double
    Public ydim As Double
    Public ulxmap As Double
    Public ulymap As Double
    Public CurrentRow As Long

    Private m_fileNum As Integer
    'Private m_bilFile As FileStream
    Private m_bilFile As BinaryWriter

    Public Sub New()
        If nbits = 0 Then nbits = 8
        If nbands = 0 Then nbands = 1
    End Sub

    Public Sub OpenFile()
        ' m_bilFile = New FileStream(BILFileNameNoExt + ".bil", FileMode.Create)
        m_bilFile = New BinaryWriter(File.Open(BILFileNameNoExt + ".bil", FileMode.Create))
    End Sub

    Public Sub CloseFile()
        m_bilFile.Close()
    End Sub

    Public Sub WriteRowBytes(bytes As Byte(), ncols As Integer)
        Try
            m_bilFile.Write(bytes, 0, ncols)
        Catch ex As Exception
            Dim msg As String
            msg = ex.Message
        End Try

    End Sub

    Public Sub WriteRow(ByRef inRow As String)
        Dim recLength As Integer
        Dim bytes As Byte()

        recLength = inRow.Length
        bytes = StringToByteArray(inRow)
        Try
            m_bilFile.Write(bytes, 0, recLength)
        Catch ex As Exception
            Dim msg As String
            msg = ex.Message

        End Try
    End Sub

    Public Shared Function StringToByteArray(str As String) As Byte()
        Dim encoding As New System.Text.ASCIIEncoding
        Return encoding.GetBytes(str)
    End Function

    Public Sub WriteHDRFile()
        Dim HDRFile As String
        Dim hdrFileNum As Integer
        Dim sw As StreamWriter

        hdrFileNum = FreeFile()
        HDRFile = Me.BILFileNameNoExt & ".hdr"
        sw = New StreamWriter(HDRFile)
        sw.WriteLine("nrows " & Me.nrows)
        sw.WriteLine("ncols " & Me.ncols)
        sw.WriteLine("nbits " & Me.nbits)
        sw.WriteLine("nbands " & Me.nbands)
        sw.WriteLine("xdim " & Me.xdim)
        sw.WriteLine("ydim " & Me.ydim)
        sw.WriteLine("ulxmap " & Me.ulxmap)
        sw.WriteLine("ulymap " & Me.ulymap)
        sw.Close()
    End Sub
End Class
