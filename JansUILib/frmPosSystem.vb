Imports System.Configuration
Imports System.Data.OleDb

Public Class POSSystem

    '---Init'

    'Client Info Variables
    Dim UID As Integer
    Public Shared ReadOnly businessName As String = ""
    Public Shared ReadOnly versionNumber As String = "[Dev Build]"
    Public Shared currentUser As String = "Dev"

    'Variables Init'
    Public Shared accentColor As Color = Color.FromArgb(255, 255, 255)
    Public ReadOnly UIfont = New System.Drawing.Font("Consolas", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))

    '---Database Variables Init

    'Form Wide Datareader
    Dim myReader As OleDbDataReader

    'User Data Database Connection
    ReadOnly conn As New OleDbConnection(AuthLogin.UserDataConnectionString)

    'Menu Database Connection
    ReadOnly menuconn As New OleDbConnection("Provider=Microsoft.Ace.Oledb.12.0;Data Source=.\Menu.accdb")

    ReadOnly MenuCatergories As New List(Of String)()

    '---Winforms Init' 

    'Read From Database
    Public Function SqlReadValue(command As String)
        Dim cmd As New OleDbCommand(command, conn)
        myReader = cmd.ExecuteReader
        While myReader.Read()
            Return myReader.GetValue(0)
        End While
        Return Nothing
    End Function

    'Read from Menu Database
    Public Function sqlReadMenuValue(command As String)
        Dim cmd As New OleDbCommand(command, menuconn)
        myReader = cmd.ExecuteReader()
        While myReader.Read()
            Return myReader.GetValue(0)
        End While
        Return Nothing
    End Function

    'Load User Configs
    Private Sub LoadUserConfig()
        UID = CInt(SqlReadValue("SELECT UID FROM UserAuth WHERE (Username='" & currentUser & "')"))
        accentColor = Color.FromArgb(SqlReadValue("SELECT Accent FROM UserConfig WHERE (UID=" & UID & ")"))
        UpdateAccent()
    End Sub

    'Save User Config
    Private Sub SaveConfig()
        Dim cmd As New OleDbCommand("UPDATE UserConfig SET Accent=" & accentColor.ToArgb() & " WHERE UID=" & UID, conn)
        cmd.ExecuteNonQuery()
    End Sub

    'Load Menu Items
    Private Sub LoadMenuItems()
        menuconn.Open()
        Dim cmd As New OleDbCommand("SELECT Category FROM Menu", menuconn)
        myReader = cmd.ExecuteReader
        While myReader.Read()
            Dim value As String = CStr(myReader.GetValue(0))
            If Not MenuCatergories.Contains(value) Then
                MenuCatergories.Add(value)
            End If
        End While

        'Adds the first item in the selector
        tblMenuTabsContainer.ColumnCount = 1
        tblMenuTabsContainer.ColumnStyles.RemoveAt(1)
        For i As Integer = 0 To MenuCatergories.Count - 1
            tblMenuTabsContainer.ColumnCount += 1
            tblMenuTabsContainer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize))
            Dim TabLabel As New Label With {.Name = MenuCatergories(i), .Size = New Size(100, 100), .Margin = New Padding(0), .Padding = New Padding(5), .Font = UIfont, .AutoSize = True, .Dock = DockStyle.Left, .ForeColor = Color.FromArgb(150, 150, 150), .Text = CStr(MenuCatergories(i))}
            tblMenuTabsContainer.Controls.Add(TabLabel, CInt(tblMenuTabsContainer.ColumnCount), 0)
            tblMenuTabsContainer.Controls.Add(New Panel With {.Size = New Size(0, 1), .Margin = New Padding(0), .Dock = DockStyle.Fill, .BackColor = Color.Transparent}, CInt(tblMenuTabsContainer.ColumnCount), 1)
            AddHandler TabLabel.Click, Sub(sender As Object, e As EventArgs)
                                           Dim currentColumn As Integer = tblMenuTabsContainer.GetColumn(sender)
                                           ChangeMenuTab(MenuCatergories(currentColumn - 2))
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


    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        Const WM_SYSCOMMAND As Integer = &H112
        Const SC_CLOSE As Integer = &HF060
        If m.Msg = WM_SYSCOMMAND AndAlso m.WParam.ToInt32() = SC_CLOSE Then
            Application.Exit() 'Patch bug where process not killed due to main form being hidden
        Else
            MyBase.WndProc(m)
        End If
    End Sub

    'Init tab system and load accent color
    Private Sub POSSystem_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        conn.Open()
        For Each cntrl As Control In TblTabsContainer.Controls.OfType(Of Panel)
            cntrl.Width = 0
        Next
        lblCurrentUser.Text = currentUser
        LoadUserConfig()
        ChangeTab(lblTabSel1, e)
        LoadMenuItems()
    End Sub

    '---Menu Tab Changing System
    Private Sub ChangeMenuTab(newTab As String)
        FlwMenuItemGrid.Controls.Clear()
        Dim categoryitems As New List(Of String)()
        Dim cmd As New OleDbCommand("Select UID From Menu Where Category='" & newTab & "'", menuconn)
        myReader = cmd.ExecuteReader
        While myReader.Read()
            categoryitems.Add(CStr(myReader.GetValue(0)))
        End While
        For i As Integer = 0 To categoryitems.Count - 1
            Dim ItemShadow As New Panel With {.BackColor = Color.Black,
            .ForeColor = Color.White, .Margin = New Padding(5), .Padding = New Padding(1), .Parent = FlwMenuItemGrid, .Size = New Size(125, 125)}
            Dim itemborder As New Panel With {.BackColor = Color.FromArgb(75, 75, 75),
            .ForeColor = Color.White, .Padding = New Padding(1), .Parent = ItemShadow, .Dock = DockStyle.Fill}
            Dim price As Decimal = sqlReadMenuValue("SELECT [Price] FROM Menu WHERE UID=" & categoryitems(i))
            Dim FormattedString As String = "£" & String.Format("{0:n}", price)
            Dim menuitem As New BorderlessButton(sqlReadMenuValue("SELECT [Display Name] FROM Menu WHERE UID=" & categoryitems(i)) & Environment.NewLine & FormattedString) With {.Parent = itemborder}

        Next
    End Sub
    '---Tab Changing System

    Private Sub ChangeTab(sender As Object, e As EventArgs) Handles lblTabSel1.Click, lblTabSel2.Click

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
            pnlSettingsPage.Dock = DockStyle.Fill
            pnlTabHighlight2.Visible = True
        End If

    End Sub

    '---Notification Prompts

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
        lblTabSel2.ForeColor = accentColor

    End Sub

    '---Application Code

    'Settings Tab 

    'UI Accent Colour Picker
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles pnlColorPicker.Click
        Button11.Focus()
        If Not ColorPicker.IsHandleCreated Then
            ColorPicker.Show()
        End If
        SaveConfig()
        UpdateAccent()
    End Sub

    'User Logout Button
    Private Sub UserLogOut(sender As Object, e As EventArgs) Handles BtnLogOut.Click
        Me.Close()
        AuthLogin.Show()
        AuthLogin.LoadUsernames()
    End Sub

    '---Watermark

    'Timer Tick Update
    Private Sub TmrMain_Tick(sender As Object, e As EventArgs) Handles tmrMain.Tick
        lblTitle.Text = "POS SYSTEM | " & versionNumber & " | " & currentUser & " | " & DateTime.Now.ToString("HH:mm:ss") & " | " & DateTime.Now.ToString("dd MMM. yyyy")
    End Sub

    Private Sub BtnSavePassword_Click(sender As Object, e As EventArgs) Handles BtnSavePassword.Click
        If tbxPassword.Text <> "" And InStr(tbxPassword.Text, " ") = 0 Then
            Dim cmd As New OleDbCommand("UPDATE UserAuth SET PIN='" & tbxPassword.Text & "' WHERE UID=" & UID, conn)
            cmd.ExecuteNonQuery()
            Notification("New password was set successfully!")
            tbxPassword.Clear()
        ElseIf InStr(tbxPassword.Text, " ") > 0 Then
            Notification("Error: Passwords can not have spaces in them!")
            tbxPassword.Text = tbxPassword.Text.Replace(" ", "")
        Else
            Notification("Error: Password field can not be empty!")
        End If
    End Sub

End Class

Public Class BorderlessButton
    Inherits Button

    Protected Overrides Sub OnPaint(ByVal pevent As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(pevent)
        Me.FlatAppearance.BorderSize = 0
        Me.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60)
        Me.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 30, 30)
    End Sub

    Public Sub New(newText As String)
        Me.Text = newText
        Me.BackColor = Color.FromArgb(40, 40, 40)
        Me.ForeColor = Color.White
        Me.Dock = DockStyle.Fill
        Me.FlatStyle = FlatStyle.Flat
        Me.Name = newText
        Me.Font = New System.Drawing.Font("Consolas", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    End Sub

End Class
