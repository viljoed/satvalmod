Imports System.IO
Imports System.Windows.Forms

Public Class clsFileUtils
    Private Declare Function GetOpenFileName Lib "comdlg32.dll" Alias _
         "GetOpenFileNameA" (pOpenFileName As OPENFILENAME) As Long
    Private Structure OPENFILENAME
        Dim lStructSize As Long
        Dim hwndOwner As Long
        Dim hInstance As Long
        Dim lpstrFilter As String
        Dim lpstrCustomFilter As String
        Dim nMaxCustFilter As Long
        Dim nFilterIndex As Long
        Dim lpstrFile As String
        Dim nMaxFile As Long
        Dim lpstrFileTitle As String
        Dim nMaxFileTitle As Long
        Dim lpstrInitialDir As String
        Dim lpstrTitle As String
        Dim flags As Long
        Dim nFileOffset As Integer
        Dim nFileExtension As Integer
        Dim lpstrDefExt As String
        Dim lCustData As Long
        Dim lpfnHook As Long
        Dim lpTemplateName As String
    End Structure

    'Allow the user to select multiple files (Open File dialog box only).
    Const OFN_ALLOWMULTISELECT = &H200
    'Prompt if a non-existing file is chosen.
    Const OFN_CREATEPROMPT = &H2000
    'Use the function specified by lpfnHook to process the dialog box's messages.
    Const OFN_ENABLEHOOK = &H20
    'Use the dialog box template specifed by hInstance and lpTemplateName.
    Const OFN_ENABLETEMPLATE = &H40
    'Use the preloaded dialog box template specified by hInstance.
    Const OFN_ENABLETEMPLATEHANDLE = &H80
    'The function sets this flag if the user selects a file with an extension
    'different than the one specified by lpstrDefExt.
    Const OFN_EXTENSIONDIFFERENT = &H400
    'Only allow the selection of existing files.
    Const OFN_FILEMUSTEXIST = &H1000
    'Hide the Open As Read Only check box (Open File dialog box only).
    Const OFN_HIDEREADONLY = &H4
    'Don 't change Windows's current directory to match the one chosen in the
    'dialog box.
    Const OFN_NOCHANGEDIR = &H8
    'If a shortcut file (.lnk or .pif) is chosen, return the shortcut file
    'itself instead of the file or directory it points to.
    Const OFN_NODEREFERENCELINKS = &H100000
    'Hide and disable the Network button in the dialog box.
    Const OFN_NONETWORKBUTTON = &H20000
    'The function sets this flag if the selected file is not read-only
    '(Open File dialog box only).
    Const OFN_NOREADONLYRETURN = &H8000
    'Don 't check the filename for invalid characters.
    Const OFN_NOVALIDATE = &H100
    'Prompt the user if the chosen file already exists (Save File dialog box only).
    Const OFN_OVERWRITEPROMPT = &H2
    Const OFN_PATHMUSTEXIST = &H800
    Const OFN_READONLY = &H1
    Const OFN_SHAREAWARE = &H4000
    Const OFN_SHOWHELP = &H10

    Private Declare Function GetSaveFileName Lib "comdlg32.dll" Alias "GetSaveFileNameA" (pOpenFileName As OPENFILENAME) As Long
    Private Declare Function FindFirstFile Lib "kernel32" Alias "FindFirstFileA" (ByVal lpFileName As String, lpFindFileData As WIN32_FIND_DATA) As Long
    Private Declare Function FindNextFile Lib "kernel32.dll" Alias "FindNextFileA" (ByVal hFindFile As Long, lpFindFileData As WIN32_FIND_DATA) As Long
    Private Declare Function FindClose Lib "kernel32.dll" (ByVal hFindFile As Long) As Long
    Private Declare Function GetShortPathName Lib "kernel32" Alias "GetShortPathNameA" (ByVal lpszLongPath As String, ByVal lpszShortPath As String, ByVal cchBuffer As Long) As Long
    Private Declare Function SetEnvironmentVariable Lib "kernel32" Alias "SetEnvironmentVariableA" (ByVal lpName As String, ByVal lpValue As String) As Long
    Private Declare Function EnableWindow Lib "user32" (ByVal hwnd As Long, ByVal fEnable As Long) As Long
    Private Declare Function GetEnvironmentVariable Lib "kernel32" Alias "GetEnvironmentVariableA" (ByVal lpName As String, ByVal lpBuffer As String, ByVal nSize As Long) As Long
    Private Declare Function GetTempPath Lib "kernel32" Alias "GetTempPathA" (ByVal nBufferLength As Long, ByVal lpBuffer As String) As Long
    Private Declare Function LockWindowUpdate Lib "user32" (ByVal hwndLock As Long) As Long
    Private Declare Function GetWindowsDirectory Lib "kernel32" Alias "GetWindowsDirectoryA" (ByVal lpBuffer As String, ByVal nSize As Long) As Long
    Private Declare Function GetSystemDirectory Lib "kernel32" Alias "GetSystemDirectoryA" (ByVal lpBuffer As String, ByVal nSize As Long) As Long

    Private Structure FILETIME
        Dim dwLowDateTime As Long
        Dim dwHighDateTime As Long
    End Structure

    Private Structure WIN32_FIND_DATA
        Dim dwFileAttributes As Long
        Dim ftCreationTime As FILETIME
        Dim ftLastAccessTime As FILETIME
        Dim ftLastWriteTime As FILETIME
        Dim nFileSizeHigh As Long
        Dim nFileSizeLow As Long
        Dim dwReserved0 As Long
        Dim dwReserved1 As Long
        <VBFixedString(260)> Dim cFileName As String ' * 260
        <VBFixedString(14)> Dim cAlternate As String ' * 14
    End Structure
    Private nNumFiles As Long

    Public Function FileExists(BILFileNameNoExt As String, _
                               FileExtension As String) As Boolean
        '------ Requires a reference to Microsoft Scripting Runtime
        Dim FileNameToTest As String
        Dim pFSO As Object
        pFSO = CreateObject("Scripting.FileSystemObject")
        FileNameToTest = BILFileNameNoExt & "." & FileExtension
        '------ If FileExtension included ".", remove ".."
        FileNameToTest = Replace(FileNameToTest, "..", ".")
        'Set pFSO = New Scripting.FileSystemObject

        If pFSO.FileExists(FileNameToTest) Then
            FileExists = True
        Else
            FileExists = False
        End If
    End Function

    Public Sub FileCopy(SourceFile As String, _
                        TargetFile As String, _
                        Overwrite As Boolean)
        File.Copy(SourceFile, TargetFile, True)
    End Sub

    Private Function FileName(ByVal sFilePath As String, _
                              Optional bNoExtension As Boolean = True) As String
        '------ Returns the directory that the file in the path resides in:
        '       ie . returns "Temp.dbf" from "c:\temp\table.dbf"
        Dim i As Integer
        Dim s As String
        Dim iBeg As Integer
        Dim sName As String

        On Error GoTo GetFileName_ERR

        sFilePath = RemoveQuotes(sFilePath)
        For i = Len(sFilePath) To 1 Step -1
            s = Mid(sFilePath, i, 1)
            '------ Bail when when you get first backslash (s="\"):
            If s = "\" Then Exit For
        Next

        iBeg = i + 1

        If iBeg - 1 = Len(sFilePath) Then 'Is a root dir
            sName = Left(sFilePath, 1)
        Else
            sName = Mid(sFilePath, iBeg)
        End If

        If bNoExtension Then
            If Len(sName) > 3 Then
                '------ If there is an extension:
                If Mid(sName, Len(sName) - 3, 1) = "." Then
                    If Len(sName) > 4 Then
                        FileName = Mid(sName, 1, Len(sName) - 4)
                    Else
                        FileName = ""
                    End If
                Else
                    FileName = sName
                End If
            Else
                '------ Certainly no extension- filename is only 3 characters
                '       (ie. pat)
                FileName = sName
            End If
        Else
            FileName = sName
        End If
        Exit Function

GetFileName_ERR:
        Debug.Assert(0)
        Debug.Print("GetFileName_ERR: " & Err.Description)
        '------ Return the last thing we got:
        If Len(sName) > 0 Then
            FileName = sName
        Else
            '------ Else pass back what we got in:
            FileName = sFilePath
        End If
    End Function

    Private Function RemoveQuotes(ByVal sTempExp) As String
        '------ If there are quotes or double quotes around the passed in expression,
        '       this function returns the expression without them.
        On Error GoTo RemoveQuotes_ERR
        sTempExp = Trim(sTempExp)
        RemoveQuotes = sTempExp
        If sTempExp <> Chr(34) And sTempExp <> Chr(39) Then
            If (Left(Trim(sTempExp), 1) = Chr(34) Or Left(Trim(sTempExp), 1) = Chr(39)) _
            Or (Right(Trim(sTempExp), 1) = Chr(34) Or Right(Trim(sTempExp), 1) = Chr(39)) Then

                If Left(Trim(sTempExp), 1) = Chr(34) Or Left(Trim(sTempExp), 1) = Chr(39) Then
                    sTempExp = Mid(sTempExp, 2)
                End If

                If Right(Trim(sTempExp), 1) = Chr(34) Or Right(Trim(sTempExp), 1) = Chr(39) Then
                    sTempExp = Mid(sTempExp, 1, Len(sTempExp) - 1)
                End If

                RemoveQuotes = sTempExp
            Else
                RemoveQuotes = sTempExp
            End If
        Else
            RemoveQuotes = ""
        End If
        Exit Function
RemoveQuotes_ERR:
        Debug.Assert(0)
        Debug.Print("RemoveQuotes_ERR: " & Err.Description)
    End Function

    Public Function GetFileToOpen(Optional sDefaultPath As String = "C:\", _
                              Optional sFilter As String = "All files (*.*)|*.*|All files (*.*)|*.*", _
                              Optional bMultiSelect As Boolean = False) As String
        Dim fd As OpenFileDialog = New OpenFileDialog()
        Dim fileName As String = String.Empty
        fd.Title = "Select file to open"
        fd.InitialDirectory = sDefaultPath
        fd.Filter = sFilter
        fd.FilterIndex = 2
        fd.RestoreDirectory = True
        fd.Multiselect = bMultiSelect
        If fd.ShowDialog() = DialogResult.OK Then
            fileName = fd.FileName
        End If
        Return fileName
    End Function

    Private Sub ReturnWOEndNull(ByRef sString As String)
        Dim i As Integer
        Dim s As String
        '------ Return a string up to its terminating null character
        On Error GoTo ReturnWOEndNull_ERR
        For i = 1 To Len(sString)
            s = Mid(sString, i, 1)
            If s = Chr(0) Then
                Exit For
            End If
        Next
        sString = Mid(sString, 1, i - 1)
        Exit Sub
ReturnWOEndNull_ERR:
        Debug.Assert(0)
        Debug.Print("ReturnWOEndNull_ERR: " & Err.Description)
    End Sub

    Public Function GetFileToSave(Optional sDefaultPath As String = "C:\", _
                        Optional sDefExtension As String = "txt", _
                        Optional sFilter As String = "All files (*.*)|*.*|All files (*.*)|*.*", _
                        Optional lOwnerHwnd As Long = 0) As String
        Dim sd As SaveFileDialog = New SaveFileDialog()
        sd.InitialDirectory = sDefaultPath
        sd.Filter = sFilter
        sd.DefaultExt = sDefExtension
        sd.ShowDialog()
        Return sd.FileName
    End Function
End Class
