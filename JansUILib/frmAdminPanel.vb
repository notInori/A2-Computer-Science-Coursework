Imports System.CodeDom.Compiler
Imports System.Data.OleDb

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
    Dim selectedUID As New Integer

    'Database Variables Init
    Dim myReader As OleDbDataReader
    ReadOnly conn As New OleDbConnection(AuthLogin.UserDataConnectionString)

    '---Winforms Init' 

    'Init tab system and load accent color
    Private Sub POSSystem_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        conn.Open()
        For Each cntrl As Control In TblTabsContainer.Controls.OfType(Of Panel)
            cntrl.Width = 0
        Next
        lblCurrentUser.Text = currentUser
        LoadUserConfig()
        ChangeTab(lblTabSel1, e)
        LoadUsernames()
    End Sub

    '---Database Functions

    'Read Value from Database
    Public Function SqlReadVAlue(command As String)
        Dim cmd As New OleDbCommand(command, conn)
        myReader = cmd.ExecuteReader
        While myReader.Read()
            Return myReader.GetValue(0)
        End While
        Return Nothing
    End Function

    'Load Usernames
    Private Sub LoadUsernames()
        Dim cmd As New OleDbCommand("SELECT Username FROM UserAuth", conn)
        myReader = cmd.ExecuteReader
        lbxUsernames.Items.Clear()
        While myReader.Read
            If myReader("Username") <> "admin" Then
                lbxUsernames.Items.Add(myReader("Username"))
            End If
        End While
    End Sub

    'Load User Configs
    Private Sub LoadUserConfig()
        UID = CInt(SqlReadVAlue("SELECT UID FROM UserAuth WHERE (Username='" & currentUser & "')"))
        accentColor = Color.FromArgb(SqlReadVAlue("SELECT Accent FROM UserConfig WHERE (UID=" & UID & ")"))
        UpdateAccent()
    End Sub

    'Save User Config
    Private Sub SaveConfig(command As String)
        Dim cmd As New OleDbCommand(command, conn)
        cmd.ExecuteNonQuery()
    End Sub

    '---UI Library Functions

    'Tab Changing System
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

    Public Sub UpdateAccent()
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

        'Tab Highlight Accent Updating
        For Each cntrl As Control In TblTabsContainer.Controls.OfType(Of Panel)
            cntrl.BackColor = accentColor
        Next

        'Tab Label Accent Updating
        lblTabSel4.ForeColor = accentColor

    End Sub

    '---Notifications

    'Full screen notifications
    Private Sub Notification(NotificationText As String)
        lblNotificationInfo.Text = NotificationText
        pnlNotification.Dock = DockStyle.Fill
        pnlNotification.BringToFront()
    End Sub

    'Dismiss Notification Button
    Private Sub DimissNotification(sender As Object, e As EventArgs) Handles btnContinueNotification.Click
        pnlNotification.Dock = DockStyle.None
        pnlNotification.Height = 0
    End Sub

    'UI Accent Colour Picker
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles pnlColorPicker.Click
        If Not ColorPicker.IsHandleCreated Then
            Dim PnlColorPicker As New ColorPicker()
            PnlColorPicker.Show()

        End If
        UpdateAccent()
        SaveConfig("UPDATE UserConfig SET Accent=" & accentColor.ToArgb() & " WHERE UID=" & UID)
    End Sub

    '---Application Code

    'Users Screen

    'Load selected User's Data
    Private Sub LoadSelectedUserInfo(sender As Object, e As EventArgs) Handles lbxUsernames.SelectedValueChanged
        If lbxUsernames.SelectedItem <> "" Then
            selectedUID = SqlReadVAlue("SELECT UID FROM UserAuth WHERE (Username='" & lbxUsernames.SelectedItem.ToString & "')")
            'Display User Credentials
            TbxUsername.Text = lbxUsernames.SelectedItem
            TbxPassword.Text = SqlReadVAlue("SELECT PIN FROM UserAuth WHERE UID=" & selectedUID)

            'Display User Personal Info
            LblFirstName.Text = SqlReadVAlue("SELECT [First Name] FROM UserData WHERE UID=" & selectedUID)
            LblLastName.Text = SqlReadVAlue("SELECT [Last Name] FROM UserData WHERE UID=" & selectedUID)
            LblJobStatus.Text = SqlReadVAlue("SELECT [Job Status] FROM UserData WHERE UID=" & selectedUID)
            'Display User Performance
            LblTotalHours.Text = SqlReadVAlue("SELECT [Total Hours] FROM UserStats WHERE UID=" & selectedUID) & " Hrs"
        End If
    End Sub

    'Save Changes to User's Username and Password
    Private Sub UpdateUserCredentials(sender As Object, e As EventArgs) Handles btnSave.Click
        If TbxUsername.Text <> "" And TbxPassword.Text <> "" And lbxUsernames.SelectedItem <> Nothing And InStr(TbxPassword.Text, " ") = 0 Then
            SaveConfig("UPDATE UserAuth SET Username='" & TbxUsername.Text & "' WHERE UID=" & selectedUID)
            SaveConfig("UPDATE UserAuth SET PIN='" & TbxPassword.Text & "' WHERE UID=" & selectedUID)
            lbxUsernames.SelectedItem = SqlReadVAlue("SELECT Username FROM UserAuth WHERE (UID=" & selectedUID & ")")
            Notification("New User Credentials for " & TbxUsername.Text & " have been saved successfully!")
        ElseIf TbxUsername.Text = "" Or TbxPassword.Text = "" Then
            Notification("Error: Fields can not be empty!")
        ElseIf InStr(TbxPassword.Text, " ") > 0 Then
            Notification("Error: Passwords can not have spaces in them!")
            TbxPassword.Text = TbxPassword.Text.Replace(" ", "")
        Else
            Notification("Error: User must be selected.")
        End If
        LoadUsernames()
    End Sub

    'Clears User Data Fields
    Private Sub ClearUserDataFields(sender As Object, e As EventArgs) Handles BtnClear.Click
        selectedUID = Nothing
        lbxUsernames.SelectedItem = Nothing
        TbxUsername.Clear()
        TbxPassword.Clear()
        LblFirstName.Text = "[FIRST NAME]"
        LblLastName.Text = "[LAST NAME]"
        LblJobStatus.Text = "[JOB STATUS]"
        LblTotalHours.Text = "0 Hrs"

    End Sub

    'Adds New User To Database
    Private Sub AddNewUser(sender As Object, e As EventArgs) Handles BtnAddUser.Click
        If SqlReadVAlue("SELECT UID FROM UserAuth WHERE (Username='" & TbxUsername.Text.ToString & "')") = Nothing And TbxUsername.Text <> "" And TbxPassword.Text <> "" Then
            SaveConfig("INSERT INTO UserAuth(Username,PIN) VALUES('" & TbxUsername.Text & "','" & TbxPassword.Text & "')")
            SaveConfig("INSERT INTO UserConfig(Accent) VALUES(-1)")
            SaveConfig("INSERT INTO UserData DEFAULT VALUES")
            SaveConfig("INSERT INTO UserStats DEFAULT VALUES")
            Notification("User " & TbxUsername.Text.ToString & " has been successfully added!")
            LoadUsernames()
        ElseIf SqlReadVAlue("SELECT UID FROM UserAuth WHERE (Username='" & TbxUsername.Text.ToString & "')") = Nothing Then
            Notification("Error: Fields can not be empty!")
        Else
            Notification("Error: " & TbxUsername.Text.ToString & " already exists.")
        End If
    End Sub

    'Deletes Selected User
    Private Sub DeleteUser(sender As Object, e As EventArgs) Handles BtnDelete.Click, BtnContinueAction.Click, BtnCancelAction.Click
        If sender Is BtnDelete And lbxUsernames.SelectedItem <> Nothing Then
            lblConfirmationText.Text = "Are you that you want to delete " & lbxUsernames.SelectedItem.ToString & "?"
            pnlConfirmation.Dock = DockStyle.Fill
            pnlConfirmation.BringToFront()
        ElseIf sender Is BtnDelete Then
            Notification("Error: User must be selected!")
        ElseIf sender Is BtnContinueAction Or sender Is BtnCancelAction Then
            pnlConfirmation.Dock = DockStyle.None
            pnlConfirmation.Height = 0
        End If
        If sender Is BtnContinueAction Then
            Dim tempUsername As String = SqlReadVAlue("SELECT Username FROM UserAuth WHERE UID=" & selectedUID)
            SaveConfig("DELETE FROM UserConfig WHERE UID=" & selectedUID)
            SaveConfig("DELETE FROM UserAuth WHERE UID=" & selectedUID)
            SaveConfig("DELETE FROM UserData WHERE UID=" & selectedUID)
            SaveConfig("DELETE FROM UserStats WHERE UID=" & selectedUID)
            selectedUID = Nothing
            ClearUserDataFields(sender, e)
            Notification("User " & tempUsername & " Successfully Deleted!")
        End If
        LoadUsernames()
    End Sub

    'Settings Tab 

    'User Logout Button
    Private Sub UserLogOut(sender As Object, e As EventArgs) Handles BtnLogOut.Click
        conn.Close()
        Me.Close()
        AuthLogin.Show()
        AuthLogin.LoadUsernames()
    End Sub

    'Save Admin Password Button
    Private Sub SaveAdminPassword(sender As Object, e As EventArgs) Handles btnSaveAdminPass.Click
        If tbxAdminPassword.Text <> "" Then
            SaveConfig("UPDATE UserAuth SET PIN='" & tbxAdminPassword.Text & "' WHERE UID=1")
            Notification("New admin credentials have been set successfully!")
        Else
            Notification("Error: Field can not be empty.")
        End If
        tbxAdminPassword.Clear()
    End Sub

    '---Watermark

    'Timer Tick Update
    Private Sub TmrMain_Tick(sender As Object, e As EventArgs) Handles tmrMain.Tick
        lblTitle.Text = "POS SYSTEM | " & versionNumber & " | " & currentUser & " | " & DateTime.Now.ToString("HH:mm:ss") & " | " & DateTime.Now.ToString("dd MMM. yyyy")
    End Sub

End Class
