Imports System.Data.OleDb


Public Class AuthLogin

    '---Init'

    '---Init Database

    'Variables
    Dim myReader As OleDbDataReader
    'Database Connection
    ReadOnly conn As New OleDbConnection("Provider=Microsoft.Ace.Oledb.12.0;Data Source=.\UserData.accdb")

    '---Database Functions

    'Read From Database
    Public Function SqlReadValue(command As String)
        Dim cmd As New OleDbCommand(command, conn)
        myReader = cmd.ExecuteReader
        While myReader.Read()
            Return myReader.GetValue(0)
        End While
        Return Nothing
    End Function

    'Load Usernames
    Public Sub LoadUsernames()
        conn.Open()
        CbxUsername.Items.Clear()
        Dim cmd As New OleDbCommand("SELECT Username FROM UserAuth", conn)
        myReader = cmd.ExecuteReader
        While myReader.Read
            CbxUsername.Items.Add(myReader("Username"))
        End While
    End Sub

    'Authenticates the User
    Private Function AuthUser(ByVal username As String, ByVal password As String)
        Dim storedPassword = SqlReadValue("SELECT PIN FROM UserAuth WHERE (Username='" & username & "')")
        If password = CStr(storedPassword) And CStr(storedPassword) <> "" Then
            Return True 'Returns true if combination of username and password is correct
        Else
            Return False 'Returns false if combination is inccorrect or fields are empty
        End If
    End Function

    '---Winforms Dragging

    'Winforms Init' 
    Private Sub UserLogin_OnLoad(ByVal qsender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        loadUsernames()
        lblCurrentVersion.Text = POSSystem.versionNumber
        lblShopName.Text = POSSystem.businessName
        pnlWindowContents.Dock = DockStyle.Fill
        pnlWindowContents.BringToFront()
    End Sub

    'Winforms Variable Init'
    Private Property MoveForm As Boolean
    Private Property MoveForm_MousePositiion As Point

    'Winforms Dragging Events
    Private Sub WindowDragging_MouseDown(sender As Object, e As MouseEventArgs) Handles tblWindow.MouseDown, pnlBackground.MouseDown, pnlWindowContents.MouseDown, pnlGroupBoxInner.MouseDown, pnlGroupUsernameTextbox.MouseDown, lblUsername.MouseDown, TableLayoutPanel2.MouseDown, TableLayoutPanel1.MouseDown, lblTitle.MouseDown, Panel5.MouseDown, Panel314.MouseDown, lblShopName.MouseDown, Label33.MouseDown, Label2.MouseDown, lblCurrentVersion.MouseDown
        If e.Button = MouseButtons.Left And Me.WindowState <> FormWindowState.Maximized Then
            MoveForm = True
            Me.Cursor = Cursors.Default
            MoveForm_MousePositiion = e.Location
        End If
    End Sub

    Private Sub WindowDragging_MouseUp(sender As Object, e As MouseEventArgs) Handles tblWindow.MouseUp, pnlBackground.MouseUp, pnlWindowContents.MouseUp, pnlGroupBoxInner.MouseUp, pnlGroupUsernameTextbox.MouseUp, lblUsername.MouseUp, TableLayoutPanel2.MouseUp, TableLayoutPanel1.MouseUp, lblTitle.MouseUp, Panel5.MouseUp, Panel314.MouseUp, lblShopName.MouseUp, Label33.MouseUp, Label2.MouseUp, lblCurrentVersion.MouseUp
        If e.Button = MouseButtons.Left Then
            MoveForm = False
            Me.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub WindowDragging_MouseMove(sender As Object, e As MouseEventArgs) Handles tblWindow.MouseMove, pnlBackground.MouseMove, pnlWindowContents.MouseMove, pnlGroupBoxInner.MouseMove, pnlGroupUsernameTextbox.MouseMove, lblUsername.MouseMove, TableLayoutPanel2.MouseMove, TableLayoutPanel1.MouseMove, lblTitle.MouseMove, Panel5.MouseMove, Panel314.MouseMove, lblShopName.MouseMove, Label33.MouseMove, Label2.MouseMove, lblCurrentVersion.MouseMove
        If MoveForm Then
            Me.Location += (e.Location - MoveForm_MousePositiion)
        End If
    End Sub

    '---Aplication Code

    'User Auth Button
    Private Sub AuthUser(sender As Object, e As EventArgs) Handles btnLogin.Click
        If CbxUsername.Text = "admin" And authUser(CbxUsername.Text, TbxPassword.Text) Then
            AdminPanel.Show()
        ElseIf authUser(CbxUsername.Text, TbxPassword.Text) Then
            POSSystem.currentUser = CbxUsername.Text
            POSSystem.Show()
        Else
            pnlNotification.Dock = DockStyle.Fill
            pnlNotification.BringToFront()
        End If
        If authUser(CbxUsername.Text, TbxPassword.Text) Then
            CbxUsername.Text = ""
            TbxPassword.Text = ""
            Me.Hide()
            conn.Close()
        End If
    End Sub

    'Dismiss Notification Button
    Private Sub DimissNotification(sender As Object, e As EventArgs) Handles btnContinueNotification.Click
        pnlNotification.Dock = DockStyle.None
        pnlNotification.Height = 0
    End Sub

    'Titlebar Button Events'

    'Exit Program
    Private Sub WindowExit(sender As Object, e As EventArgs) Handles btnExit.Click
        conn.Close()
        Application.Exit()
    End Sub

End Class
