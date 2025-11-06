Imports System.Data.OleDb
Imports System.Data

' This module handles all communication with the MS Access database.
Public Module DataAccess

    ' This is the connection string from your project sheet.
    ' It points to the AppDB.accdb file in the Data folder of your compiled output.
    Private Const CONN_STRING As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\Data\AppDB.accdb;Persist Security Info=False;"

    ''' <summary>
    ''' Executes a SELECT query (or any query that returns data) and returns a DataTable.
    ''' </summary>
    Public Function GetTable(sql As String, params As List(Of OleDbParameter)) As DataTable
        Dim dt As New DataTable()
        ' Using blocks ensure connections are closed even if errors occur
        Using cn As New OleDbConnection(CONN_STRING)
            Using cmd As New OleDbCommand(sql, cn)
                ' Add any parameters if they exist
                If params IsNot Nothing Then
                    For Each p In params
                        cmd.Parameters.Add(p)
                    Next
                End If

                ' Use a DataAdapter to fill the DataTable
                Using da As New OleDbDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using
        Return dt
    End Function

    ''' <summary>
    ''' Executes an INSERT, UPDATE, or DELETE query and returns the number of rows affected.
    ''' </summary>
    Public Function Exec(sql As String, params As List(Of OleDbParameter)) As Integer
        Dim rowsAffected As Integer = 0
        Using cn As New OleDbConnection(CONN_STRING)
            Using cmd As New OleDbCommand(sql, cn)
                ' Add any parameters
                If params IsNot Nothing Then
                    For Each p In params
                        cmd.Parameters.Add(p)
                    Next
                End If

                ' Open the connection, execute, and get rows affected
                cn.Open()
                rowsAffected = cmd.ExecuteNonQuery()
            End Using
        End Using
        Return rowsAffected
    End Function

    ''' <summary>
    ''' Helper function to create a new OleDbParameter.
    ''' MS Access (OLEDB) uses positional '?' parameters, not named parameters.
    ''' The name is irrelevant, but the Value and OleDbType are crucial.
    ''' </summary>
    Public Function CreateParam(value As Object, type As OleDbType) As OleDbParameter
        Dim p As New OleDbParameter
        p.OleDbType = type
        p.Value = value
        Return p
    End Function

End Module