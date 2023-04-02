Imports System.Data.OleDb
Public Class DatabaseInterface

    '---Properties

    'Define Properties
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
        Dim DatabaseOutput As New List(Of Object)()
        Using conn As New OleDbConnection(_ConnectionString)
            conn.Open()
            Using cmd As New OleDbCommand(command, conn)
                Using reader As OleDbDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        While reader.Read()
                            If Not reader.IsDBNull(0) Then
                                DatabaseOutput.Add(reader.GetValue(0))
                            Else
                                Return Nothing
                            End If
                        End While
                        Return DatabaseOutput.ToArray
                    Else
                        Return Nothing
                    End If
                    reader.Close()
                    conn.Close()
                End Using
            End Using
        End Using
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