Imports System.Data.OleDb
Public Class DatabaseInterface

    '---Properties

    'Definte Properties
    Private _ConnectionString As String

    'Property Functions
    Private Sub SetpConnectionString(AutoPropertyValue As String)
        _ConnectionString = AutoPropertyValue
    End Sub

    '---Init

    'Init Function
    Public Sub New(newFilePath As String)
        SetpConnectionString("Provider=Microsoft.Ace.Oledb.12.0;Data Source=" & newFilePath) '.\UserData.accdb
    End Sub

    Public Function ReadValue(command As String)
        Dim DatabaseOutput As New List(Of String)()
        Using conn As New OleDbConnection(_ConnectionString)
            conn.Open()
            Dim cmd As New OleDbCommand(command, conn)
            Dim reader As OleDbDataReader = cmd.ExecuteReader()
            While reader.Read()
                DatabaseOutput.Add(reader.GetValue(0))
            End While
            reader.Close()
            conn.Close()
        End Using
        Dim ArrayOutput As Array = DatabaseOutput.ToArray
        If ArrayOutput.Length() > 0 Then
            Return ArrayOutput
        End If
        Return Nothing
    End Function

    Public Sub SaveValue(command As String)
        Using conn As New OleDbConnection(_ConnectionString)
            conn.Open()
            Dim cmd As New OleDbCommand(command, conn)
            cmd.ExecuteNonQuery()
            conn.Close()
        End Using
    End Sub
End Class