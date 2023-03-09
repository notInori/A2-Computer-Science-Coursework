﻿Imports System.Data.OleDb

Public Class AdminPanel

    '---Init'

    'Client Info Variables
    Public Shared ReadOnly businessName As String = ""
    Public Shared ReadOnly versionNumber As String = "[Dev Build]"
    ReadOnly currentUser As String = "Admin"
    Dim UID As Integer

    'Variables Init'
    Public Shared accentColor As Color = Color.FromArgb(255, 255, 255)
    ReadOnly cDialog As New ColorDialog()

    'Toggles Variables Init
    Public toggle1 As Boolean = False
    Public toggle2 As Boolean = False
    Public toggle3 As Boolean = False

    'Databases
    Friend Class UserDataDataSet
    End Class

    Friend Class UserDataDataSetTableAdapters
    End Class

    '---Winforms Init' 

    'Load UID
    Private Sub setUID(ByVal username As String)
        Dim temp As String = ""
        Dim conn As New OleDbConnection(AuthLogin.UserDataConnectionString)
        conn.Open()
        Dim cmdInput As String = "SELECT UID FROM UserAuth WHERE (Username='" & username & "')"
        Dim cmd As New OleDbCommand(cmdInput, conn)
        Dim myReader As OleDbDataReader = cmd.ExecuteReader
        While myReader.Read()
            temp = myReader("UID")
        End While
        UID = CInt(temp)
        conn.Close()
    End Sub

    'Load Usernames
    Private Sub loadUsernames()
        Dim conn As New OleDbConnection(AuthLogin.UserDataConnectionString)
        conn.Open()
        Dim cmd As New OleDbCommand("SELECT Username FROM UserAuth", conn)
        Dim myReader As OleDbDataReader = cmd.ExecuteReader
        lbxUsernames.Items.Clear()
        While myReader.Read
            lbxUsernames.Items.Add(myReader("Username"))
        End While
        conn.Close()
    End Sub

    'Load User Configs
    Private Sub loadUserConfig()
        Dim tempColor As Int32
        Dim conn As New OleDbConnection(AuthLogin.UserDataConnectionString)
        conn.Open()
        Dim cmdInput As String = "SELECT Accent FROM UserConfig WHERE (UID=" & UID & ")"
        Dim cmd As New OleDbCommand(cmdInput, conn)
        Dim myReader As OleDbDataReader = cmd.ExecuteReader
        While myReader.Read()
            tempColor = myReader("Accent")
        End While
        accentColor = Color.FromArgb(tempColor)
        UpdateAccent()
        conn.Close()
    End Sub

    'Save User Config
    Private Sub saveConfig()
        Dim conn As New OleDbConnection(AuthLogin.UserDataConnectionString)
        conn.Open()
        Dim cmdInput As String = "UPDATE UserConfig SET Accent=" & accentColor.ToArgb() & " WHERE UID=" & UID
        Dim cmd As New OleDbCommand(cmdInput, conn)
        cmd.ExecuteNonQuery()
        conn.Close()
    End Sub

    'Init tab system and load accent color
    Private Sub POSSystem_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        For Each cntrl As Control In TblTabsContainer.Controls.OfType(Of Panel)
            cntrl.Width = 0
        Next
        lblCurrentUser.Text = currentUser
        setUID(currentUser)
        loadUserConfig()
        ChangeTab(lblTabSel1, e)
        loadUsernames()
    End Sub


    '---Tab Changing System

    Private Sub ChangeTab(sender As Object, e As EventArgs) Handles lblTabSel1.Click, lblTabSel2.Click, lblTabSel3.Click, lblTabSel4.Click

        'Hides selected tab indicator
        For Each cntrl As Control In TblTabsContainer.Controls.OfType(Of Panel)
            cntrl.Visible = False
        Next

        'Darkens all tab indicator text
        For Each lbl As Control In TblTabsContainer.Controls.OfType(Of Label)
            lbl.ForeColor = Color.FromArgb(150, 150, 150)
        Next

        'Hightlights selected tab with accent color
        sender.ForeColor = accentColor

        'Undocks all tab panels and hides them
        For Each menuscreen As Control In Panel1.Controls.OfType(Of Panel)
            menuscreen.Dock = DockStyle.None
            menuscreen.Height = 0
        Next

        'Docks the selected tab panel and accents selected tab indicator
        If sender Is lblTabSel1 Then
            pnlMainPage.Dock = DockStyle.Fill
            pnlTabHighlight1.Visible = True
        ElseIf sender Is lblTabSel2 Then
            pnlMenuPage.Dock = DockStyle.Fill
            pnlTabHighlight2.Visible = True
        ElseIf sender Is lblTabSel3 Then
            pnlPerformancePage.Dock = DockStyle.Fill
            pnlTabHighlight3.Visible = True
        ElseIf sender Is lblTabSel4 Then
            pnlSettingsPage.Dock = DockStyle.Fill
            pnlTabHighlight4.Visible = True
        End If
    End Sub

    '---Change Colourisable Accents in UI

    Private Sub UpdateAccent()
        'Groupbox Topbar Color Updating
        Panel8.BackColor = accentColor
        For Each menuscreen As Control In Panel1.Controls.OfType(Of Panel)
            For Each findGroupbox As Control In menuscreen.Controls.OfType(Of TableLayoutPanel)
                If findGroupbox.Tag = "groupbox" Then 'Finds groupboxes in menu panels
                    For Each findGroupboxHeader As Control In findGroupbox.Controls.OfType(Of Panel)
                        For Each findBarTable As Control In findGroupboxHeader.Controls.OfType(Of TableLayoutPanel)
                            For Each findBarOuter As Control In findBarTable.Controls
                                For Each findBarInner As Control In findBarOuter.Controls
                                    If findBarInner.Tag = "colorise" Then
                                        findBarInner.BackColor = accentColor 'Sets top border to new accent color
                                    End If
                                Next
                            Next
                        Next
                    Next
                End If
            Next
        Next

        'Update Color Picker UI Preview
        pnlColorPicker.BackColor = accentColor

        'Toggle Control Accent Updating
        If toggle1 Then
            Panel96.BackColor = accentColor
        End If

        If toggle2 Then
            Panel134.BackColor = accentColor
        End If

        If toggle3 Then
            Panel139.BackColor = accentColor
        End If

        'Tab Highlight Accent Updating
        For Each cntrl As Control In TblTabsContainer.Controls.OfType(Of Panel)
            cntrl.BackColor = accentColor
        Next

        'Tab Label Accent Updating
        lblTabSel4.ForeColor = accentColor

    End Sub

    '---Application Code

    'Settings Tab 

    'UI Accent Colour Picker
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles pnlColorPicker.Click
        If (cDialog.ShowDialog() = DialogResult.OK) Then
            accentColor = cDialog.Color ' update with user selected color.
        End If
        UpdateAccent()
        saveConfig()
    End Sub

    'User Logout Button
    Private Sub UserLogOut(sender As Object, e As EventArgs) Handles BtnLogOut.Click
        Me.Close()
        AuthLogin.Show()
    End Sub

    '---Watermark

    'Timer Tick Update
    Private Sub TmrMain_Tick(sender As Object, e As EventArgs) Handles tmrMain.Tick
        lblTitle.Text = "POS SYSTEM | " & versionNumber & " | " & currentUser & " | " & DateTime.Now.ToString("HH:mm:ss") & " | " & DateTime.Now.ToString("dd MMM. yyyy")
    End Sub

End Class
