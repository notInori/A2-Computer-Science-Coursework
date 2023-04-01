Public Class AdminPanel

    '---Init'

    'Client Info Variables
    ReadOnly currentUser As String = "Admin"
    Dim UID As Integer

    'Variables Init'
    Public accentColor As Color = Color.FromArgb(255, 255, 255)
    Dim selectedUID As New Integer
    ReadOnly MenuCategories As New List(Of String)()

    'Database Variables Init
    ReadOnly UserData As New DatabaseInterface(".\UserData.accdb")
    ReadOnly MenuData As New DatabaseInterface(".\Menu.accdb")

    '---Winforms Init' 

    'Init tab system and load accent color
    Private Sub POSSystem_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        For Each cntrl As Control In TblTabsContainer.Controls.OfType(Of Panel)
            cntrl.Width = 0
        Next
        lblCurrentUser.Text = currentUser
        LoadUserConfig()
        ChangeTab(lblTabSel1, e)
        LoadUsernames()
        LoadMenuItems()
    End Sub

    '---Database Functions

    'Load Usernames
    Public Sub LoadUsernames()
        Dim UsernameArray As Array = UserData.ReadValue("SELECT Username From UserAuth")
        lbxUsernames.Items.Clear()
        For i = 1 To UsernameArray.Length() - 1
            lbxUsernames.Items.Add(UsernameArray(i))
        Next
    End Sub

    'Load User Configs
    Private Sub LoadUserConfig()
        UID = CInt(UserData.ReadValue("SELECT UID FROM UserAuth WHERE (Username='" & currentUser & "')")(0))
        accentColor = Color.FromArgb(UserData.ReadValue("SELECT Accent FROM UserConfig WHERE (UID=" & UID & ")")(0))
        UpdateAccent()
    End Sub

    '---Menu Tab Changing System
    Private Sub ChangeMenuTab(newTab As String)
        FlwMenuItemGrid.Controls.Clear()
        Dim categoryitems = MenuData.ReadValue("Select UID From Menu Where Category='" & newTab & "'")
        For i As Integer = 0 To categoryitems.length - 1
            Dim ItemShadow As New Panel With {.BackColor = Color.Black,
            .ForeColor = Color.White, .Margin = New Padding(5), .Padding = New Padding(1), .Parent = FlwMenuItemGrid, .Size = New Size(125, 125)}
            Dim itemborder As New Panel With {.BackColor = Color.FromArgb(75, 75, 75),
            .ForeColor = Color.White, .Padding = New Padding(1), .Parent = ItemShadow, .Dock = DockStyle.Fill}
            Dim price As Decimal = MenuData.ReadValue("SELECT [Price] FROM Menu WHERE UID=" & categoryitems(i))(0)
            Dim FormattedString As String = "£" & String.Format("{0:n}", price)
            Dim menuitem As New BorderlessButton(categoryitems(i), MenuData.ReadValue("SELECT [Display Name] FROM Menu WHERE UID=" & categoryitems(i))(0) & Environment.NewLine & FormattedString) With {.Parent = itemborder}
            AddHandler menuitem.Click, Sub(sender As Object, e As EventArgs)
                                           LblMenuItemUID.Text = menuitem.UID
                                           TbxDisplayName.Text = MenuData.ReadValue("SELECT [Display Name] FROM Menu WHERE UID=" & menuitem.UID)(0)
                                           TbxItemPrice.Text = FormattedString
                                           TbxCategory.Text = MenuData.ReadValue("SELECT [Category] FROM Menu WHERE UID=" & menuitem.UID)(0)
                                       End Sub
        Next

    End Sub

    'Load Menu Items
    Private Sub LoadMenuItems()
        Dim Menu = MenuData.ReadValue("SELECT Category FROM Menu")
        For i = 0 To Menu.length - 1
            If Not MenuCategories.Contains(Menu(i)) Then
                MenuCategories.Add(Menu(i))
            End If
        Next
        'Adds the first item in the selector
        tblMenuTabsContainer.ColumnCount = 1
        tblMenuTabsContainer.ColumnStyles.RemoveAt(1)
        For i As Integer = 0 To MenuCategories.Count - 1
            tblMenuTabsContainer.ColumnCount += 1
            tblMenuTabsContainer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize))
            Dim TabLabel As New Label With {.Name = MenuCategories(i), .Size = New Size(100, 100), .Margin = New Padding(0), .Padding = New Padding(5), .Font = ProgramData.UIfont, .AutoSize = True, .Dock = DockStyle.Left, .ForeColor = Color.FromArgb(150, 150, 150), .Text = CStr(MenuCategories(i))}
            tblMenuTabsContainer.Controls.Add(TabLabel, CInt(tblMenuTabsContainer.ColumnCount), 0)
            tblMenuTabsContainer.Controls.Add(New Panel With {.Size = New Size(0, 1), .Margin = New Padding(0), .Dock = DockStyle.Fill, .BackColor = Color.Transparent}, CInt(tblMenuTabsContainer.ColumnCount), 1)
            AddHandler TabLabel.Click, Sub(sender As Object, e As EventArgs)
                                           Dim currentColumn As Integer = tblMenuTabsContainer.GetColumn(sender)
                                           ChangeMenuTab(MenuCategories(currentColumn - 2))
                                           For Each cntrl As Control In tblMenuTabsContainer.Controls.OfType(Of Panel)
                                               If tblMenuTabsContainer.GetColumn(cntrl) = currentColumn Then
                                                   cntrl.BackColor = Color.White
                                               Else
                                                   cntrl.BackColor = Color.Transparent
                                               End If
                                           Next
                                           For Each cntrl As Control In tblMenuTabsContainer.Controls.OfType(Of Label)
                                               If cntrl Is sender Then
                                                   cntrl.ForeColor = Color.White
                                               Else
                                                   cntrl.ForeColor = Color.FromArgb(150, 150, 150)
                                               End If
                                           Next
                                       End Sub
        Next
        tblMenuTabsContainer.ColumnCount += 2
        tblMenuTabsContainer.Controls.Add(New Panel With {.Size = New Size(0, 1), .Margin = New Padding(0), .Dock = DockStyle.Fill, .BackColor = Color.Transparent}, CInt(tblMenuTabsContainer.ColumnCount), 1)
    End Sub

    '---UI Library Functions

    'Patch bug where process not killed due to main form being hidden
    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        Const WM_SYSCOMMAND As Integer = &H112
        Const SC_CLOSE As Integer = &HF060
        If m.Msg = WM_SYSCOMMAND AndAlso m.WParam.ToInt32() = SC_CLOSE Then
            Application.Exit()
        Else
            MyBase.WndProc(m)
        End If
    End Sub

    'Change Current Program Tab
    Private Sub ChangeTab(sender As Object, e As EventArgs) Handles lblTabSel1.Click, lblTabSel2.Click, lblTabSel3.Click, lblTabSel4.Click, lblTabSel5.Click

        'Hides selected tab indicator
        For Each cntrl As Control In TblTabsContainer.Controls.OfType(Of Panel)
            cntrl.Visible = False
        Next

        'Undocks all tab panels and hides them
        If sender.forecolor = Color.FromArgb(150, 150, 150) Then
            For Each menuscreen As Control In Panel1.Controls.OfType(Of Panel)
                menuscreen.Dock = DockStyle.None
                menuscreen.Height = 0
            Next
        End If

        'Darkens all tab indicator text
        For Each lbl As Control In TblTabsContainer.Controls.OfType(Of Label)
            lbl.ForeColor = Color.FromArgb(150, 150, 150)
        Next

        'Hightlights selected tab with accent color
        sender.ForeColor = accentColor

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
            PnlTicketsPage.Dock = DockStyle.Fill
            pnlTabHighlight4.Visible = True
        ElseIf sender Is lblTabSel5 Then
            pnlSettingsPage.Dock = DockStyle.Fill
            pnlTabHighlight5.Visible = True
        End If
    End Sub

    'Change Colourisable Accents in UI
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
        lblTabSel5.ForeColor = accentColor

        UserData.SaveValue("UPDATE UserConfig SET Accent=" & accentColor.ToArgb() & " WHERE UID=" & UID)
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
    End Sub

    '---Application Code

    'Users Screen

    'Load selected User's Data
    Private Sub LoadSelectedUserInfo(sender As Object, e As EventArgs) Handles lbxUsernames.SelectedValueChanged
        If lbxUsernames.SelectedItem <> "" Then
            selectedUID = UserData.ReadValue("SELECT UID FROM UserAuth WHERE (Username='" & lbxUsernames.SelectedItem.ToString & "')")(0)
            'Display User Credentials
            TbxUsername.Text = lbxUsernames.SelectedItem
            TbxPassword.Text = UserData.ReadValue("SELECT PIN FROM UserAuth WHERE UID=" & selectedUID)(0)

            'Display User Personal Info
            LblStaffUID.Text = selectedUID
            TbxFirstName.Text = UserData.ReadValue("SELECT [First Name] FROM UserData WHERE UID=" & selectedUID)(0)
            TbxLastName.Text = UserData.ReadValue("SELECT [Last Name] FROM UserData WHERE UID=" & selectedUID)(0)
            'Display Job Status
            TbxJobStatus.Text = UserData.ReadValue("SELECT [Job Status] FROM UserData WHERE UID=" & selectedUID)(0)
            TbxDateJoined.Text = UserData.ReadValue("SELECT [Date Employed] FROM UserData WHERE UID=" & selectedUID)(0)
            TbxDateDismissed.Text = UserData.ReadValue("SELECT [Date Dismissed] FROM UserData WHERE UID=" & selectedUID)(0)
            'Display User Performance
            LblTotalHours.Text = UserData.ReadValue("SELECT [Total Hours] FROM UserStats WHERE UID=" & selectedUID)(0) & " Hrs"
        End If
    End Sub

    'Save Changes to User's Username and Password
    Private Sub UpdateUserCredentials(sender As Object, e As EventArgs) Handles btnSave.Click
        If TbxUsername.Text <> "" And TbxPassword.Text <> "" And lbxUsernames.SelectedItem <> Nothing And InStr(TbxPassword.Text, " ") = 0 Then
            UserData.SaveValue("UPDATE UserAuth SET Username='" & TbxUsername.Text & "' WHERE UID=" & selectedUID)
            UserData.SaveValue("UPDATE UserAuth SET PIN='" & TbxPassword.Text & "' WHERE UID=" & selectedUID)
            lbxUsernames.SelectedItem = UserData.ReadValue("SELECT Username FROM UserAuth WHERE (UID=" & selectedUID & ")")(0)
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
        LblStaffUID.Text = "0"
        TbxUsername.Clear()
        TbxPassword.Clear()
        TbxFirstName.Clear()
        TbxLastName.Clear()
        TbxJobStatus.Clear()
        TbxDateJoined.Clear()
        TbxDateDismissed.Clear()
        LblTotalHours.Text = "0 Hrs"

    End Sub

    'Adds New User To Database
    Private Sub AddNewUser(sender As Object, e As EventArgs) Handles BtnAddUser.Click
        If UserData.ReadValue("SELECT UID FROM UserAuth WHERE (Username='" & TbxUsername.Text.ToString & "')") Is Nothing And TbxUsername.Text <> "" And TbxPassword.Text <> "" Then
            UserData.SaveValue("INSERT INTO UserAuth(Username,PIN) VALUES('" & TbxUsername.Text & "','" & TbxPassword.Text & "')")
            UserData.SaveValue("INSERT INTO UserConfig(Accent) VALUES(-1)")
            UserData.SaveValue("INSERT INTO UserData DEFAULT VALUES")
            UserData.SaveValue("INSERT INTO UserStats DEFAULT VALUES")
            Notification("User " & TbxUsername.Text.ToString & " has been successfully added!")
            LoadUsernames()
        ElseIf UserData.ReadValue("SELECT UID FROM UserAuth WHERE (Username='" & TbxUsername.Text.ToString & "')")(0) = Nothing Then
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
            Dim tempUsername As String = UserData.ReadValue("SELECT Username FROM UserAuth WHERE UID=" & selectedUID)(0)
            UserData.SaveValue("DELETE FROM UserConfig WHERE UID=" & selectedUID)
            UserData.SaveValue("DELETE FROM UserAuth WHERE UID=" & selectedUID)
            UserData.SaveValue("DELETE FROM UserData WHERE UID=" & selectedUID)
            UserData.SaveValue("DELETE FROM UserStats WHERE UID=" & selectedUID)
            selectedUID = Nothing
            ClearUserDataFields(sender, e)
            Notification("User " & tempUsername & " Successfully Deleted!")
        End If
        LoadUsernames()
    End Sub

    'Menu Tab

    'Clear Current Menu Item
    Private Sub BtnItemClear_Click(sender As Object, e As EventArgs) Handles BtnItemClear.Click
        LblMenuItemUID.Text = "0"
        TbxDisplayName.Clear()
        TbxItemPrice.Clear()
        TbxCategory.Clear()
    End Sub

    'Settings Tab 

    'User Logout Button
    Private Sub UserLogOut(sender As Object, e As EventArgs) Handles BtnLogOut.Click
        Me.Close()
        AuthLogin.Show()
        AuthLogin.LoadUsernames()
    End Sub

    'Save Admin Password Button
    Private Sub SaveAdminPassword(sender As Object, e As EventArgs) Handles btnSaveAdminPass.Click
        If tbxAdminPassword.Text <> "" Then
            UserData.SaveValue("UPDATE UserAuth SET PIN='" & tbxAdminPassword.Text & "' WHERE UID=1")
            Notification("New admin credentials have been set successfully!")
        Else
            Notification("Error: Field can not be empty.")
        End If
        tbxAdminPassword.Clear()
    End Sub

    '---Watermark

    'Timer Tick Update
    Private Sub TmrMain_Tick(sender As Object, e As EventArgs) Handles tmrMain.Tick
        lblTitle.Text = "POS SYSTEM | " & ProgramData.ProgramVersion & " | " & currentUser & " | " & DateTime.Now.ToString("HH:mm:ss") & " | " & DateTime.Now.ToString("dd MMM. yyyy")
    End Sub

End Class
