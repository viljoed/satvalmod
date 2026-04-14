Imports System.IO

Public Class clsStringUtils
    Public Function ShortenFullPath(inFullPath As String, _
                                Optional numChars As Integer = 19) As String
        Dim minChars As Integer
        Dim firstPart As String
        Dim lastPart As String

        minChars = 19
        If numChars = 0 Then numChars = minChars
        If numChars < minChars Then
            MsgBox("ShortenFullPath function does not support numChars < " & minChars)
            numChars = minChars
        End If
        If Len(inFullPath) <= numChars Then
            ShortenFullPath = inFullPath
            Exit Function
        End If
        firstPart = Left(inFullPath, 3) & "..."
        lastPart = Right(inFullPath, numChars - Len(firstPart))
        ShortenFullPath = firstPart & lastPart
    End Function

    Public Function ReplaceMultipleSpacesWithOne(inString As String) As String
        Dim i As Long
        Dim stringLen As Long
        Dim curChar As String
        Dim nextChar As String
        Dim outString As String = String.Empty
        stringLen = Len(inString)
        For i = 1 To stringLen
            curChar = Mid(inString, i, 1)
            nextChar = Mid(inString, i + 1, 1)
            If curChar <> " " Then
                outString = outString & curChar
            ElseIf curChar = " " And nextChar <> " " Then
                outString = outString & " "
            End If
        Next i
        ReplaceMultipleSpacesWithOne = outString
    End Function

    Public Function ExtractValue(KeywordValueString As String, keyword As String) As String
        Dim startPos As Integer
        Dim endPos As Integer
        Dim theValue As String
        KeywordValueString = LCase(KeywordValueString)
        keyword = LCase(keyword)
        startPos = InStr(1, KeywordValueString, keyword)        'Start of keyword
        If startPos = 0 Then
            ExtractValue = ""
            Exit Function
        End If
        startPos = InStr(startPos + 1, KeywordValueString, " ") 'Space after keyword
        startPos = startPos + 1                                 'First char of value
        endPos = InStr(startPos, KeywordValueString, " ")
        If endPos = 0 Then
            theValue = Mid(KeywordValueString, startPos)
        Else
            theValue = Mid(KeywordValueString, startPos, endPos - startPos)
        End If
        ExtractValue = theValue
    End Function

    Public Function FileContentsToString(fileName As String) As String
        Dim fileContents As String
        Dim sr As StreamReader

        sr = New StreamReader(fileName)
        fileContents = sr.ReadToEnd()
        sr.Close()
        Return fileContents
    End Function

    Public Function GetPath(FullFileName As String) As String
        Dim FileParts() As String

        FileParts = Split(FullFileName, "\")
        GetPath = Left(FullFileName, Len(FullFileName) - Len(FileParts(UBound(FileParts))))
    End Function

    Public Function GetFileNameOnly(FullFileName As String) As String
        Dim FileParts() As String

        FileParts = Split(FullFileName, "\")
        GetFileNameOnly = FileParts(UBound(FileParts))
    End Function

    Public Function StripExtension(FullFileName As String) As String
        Dim dotPos As Integer

        dotPos = Len(FullFileName) - 3
        If Mid(FullFileName, dotPos, 1) = "." Then
            StripExtension = Left(FullFileName, dotPos - 1)
        Else
            StripExtension = FullFileName
        End If
    End Function

End Class
