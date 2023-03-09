﻿Imports System.Data.OleDb

Public Class POSSystem

    '---Init'

    'Client Info Variables
    Dim UID As Integer
    Public Shared ReadOnly businessName As String = ""
    Public Shared ReadOnly versionNumber As String = "[Dev Build]"
    Public Shared currentUser As String = "Dev"

    'Variables Init'
    Public Shared accentColor As Color = Color.FromArgb(255, 255, 255)
    ReadOnly cDialog As New ColorDialog()

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

        'Temporary Code
        Label4.ForeColor = Color.FromArgb(150, 150, 150)
        Panel5.Visible = False

    End Sub

    '---Tab Changing System

    Private Sub ChangeTab(sender As Object, e As EventArgs) Handles lblTabSel1.Click, lblTabSel2.Click

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
            pnlSettingsPage.Dock = DockStyle.Fill
            pnlTabHighlight2.Visible = True
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
        lblTabSel2.ForeColor = accentColor

    End Sub

    '---Application Code

    'Misc Tab

    'Example Toggle Switches
    Private Sub toggle1_Click(sender As Object, e As EventArgs) Handles Panel93.Click, Panel94.Click, Panel95.Click, Panel96.Click, Label3.Click

        If toggle1 Then
            Panel96.BackColor = Color.FromArgb(30, 30, 30)
            toggle1 = False
            Label3.ForeColor = Color.FromArgb(150, 150, 150)
        Else
            Panel96.BackColor = accentColor
            toggle1 = True
            Label3.ForeColor = Color.FromArgb(255, 255, 255)
            Me.Refresh()
        End If
    End Sub

    Private Sub toggle2_Click(sender As Object, e As EventArgs) Handles Panel134.Click, Panel133.Click, Panel90.Click, Label12.Click
        If toggle2 Then
            Panel134.BackColor = Color.FromArgb(30, 30, 30)
            toggle2 = False
            Label12.ForeColor = Color.FromArgb(150, 150, 150)
        Else
            Panel134.BackColor = accentColor
            toggle2 = True
            Label12.ForeColor = Color.FromArgb(255, 255, 255)
            Me.Refresh()
        End If

    End Sub

    Private Sub toggle3_Click(sender As Object, e As EventArgs) Handles Panel139.Click, Panel136.Click, Panel137.Click, Panel138.Click, Panel139.Click, Label13.Click
        If toggle3 Then
            Panel139.BackColor = Color.FromArgb(30, 30, 30)
            toggle3 = False
            Label13.ForeColor = Color.FromArgb(150, 150, 150)
        Else
            Panel139.BackColor = accentColor
            toggle3 = True
            Label13.ForeColor = Color.FromArgb(255, 255, 255)
            Me.Refresh()
        End If
    End Sub

    'Hides dropdown when another control in focus
    Private Sub DropDown_Hide(sender As Object, e As EventArgs) Handles Label24.Click
        If Panel169.Visible = False Then
            Panel169.Visible = True
        Else
            Panel169.Visible = False
        End If
    End Sub

    'Settings Tab 

    'UI Accent Colour Picker
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles pnlColorPicker.Click

        If (cDialog.ShowDialog() = DialogResult.OK) Then
            accentColor = cDialog.Color ' update with user selected color.
        End If
        saveConfig()
        UpdateAccent()
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
