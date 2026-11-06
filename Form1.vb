Imports System.IO
Imports System.Runtime.InteropServices
Imports Microsoft.Web.WebView2.Core

Public Class Form1
    Private Const DefaultUrl As String = "https://excalidraw.com/"
    Private Const ResizeBorder As Integer = 0 '8

    Private Const WM_NCLBUTTONDOWN As Integer = &HA1
    Private Const HTCAPTION As Integer = 2

    Private _resizeDirection As ResizeDirection = ResizeDirection.None
    Private _resizeStartPoint As Point
    Private _resizeStartBounds As Rectangle
    Private _resizeCaptureControl As Control
    Private _edgePanels As New List(Of Panel)

    Private Enum ResizeDirection
        None
        Left
        Right
        Top
        Bottom
        TopLeft
        TopRight
        BottomLeft
        BottomRight
    End Enum

    <DllImport("user32.dll")>
    Private Shared Function ReleaseCapture() As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function SendMessage(hWnd As IntPtr, msg As Integer, wParam As Integer, lParam As Integer) As IntPtr
    End Function

    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MinimumSize = New Size(200, 150)
        CreateResizePanels()
        Await InitializeWebViewAsync()
        Button1.BringToFront()
    End Sub

    Private Async Function InitializeWebViewAsync() As Task
        Await WebView21.EnsureCoreWebView2Async(Nothing)

        AddHandler WebView21.CoreWebView2.WebMessageReceived, AddressOf CoreWebView2_WebMessageReceived

        Dim dragScript = "
(function() {
    document.addEventListener('mousedown', function(e) {
        if (e.button !== 0 || !e.ctrlKey) return;
        e.preventDefault();
        e.stopPropagation();
        window.chrome.webview.postMessage('drag');
    }, true);
})();"

        Await WebView21.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(dragScript)
        WebView21.CoreWebView2.Navigate(DefaultUrl)
    End Function

    Private Sub CoreWebView2_WebMessageReceived(sender As Object, e As CoreWebView2WebMessageReceivedEventArgs)
        If e.TryGetWebMessageAsString() = "drag" Then
            BeginFormDrag()
        End If
    End Sub

    Private Sub BeginFormDrag()
        ReleaseCapture()
        SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0)
    End Sub

    Private Sub CreateResizePanels()
        Dim edges = New(Name As String, Dock As DockStyle, Direction As ResizeDirection, Cursor As Cursor)() {
            ("Top", DockStyle.None, ResizeDirection.Top, Cursors.SizeNS),
            ("Bottom", DockStyle.Bottom, ResizeDirection.Bottom, Cursors.SizeNS),
            ("Left", DockStyle.None, ResizeDirection.Left, Cursors.SizeWE),
            ("Right", DockStyle.Right, ResizeDirection.Right, Cursors.SizeWE),
            ("TopRight", DockStyle.None, ResizeDirection.TopRight, Cursors.SizeNESW),
            ("BottomLeft", DockStyle.None, ResizeDirection.BottomLeft, Cursors.SizeNESW),
            ("BottomRight", DockStyle.None, ResizeDirection.BottomRight, Cursors.SizeNWSE)
        }

        For Each edge In edges
            Dim panel As New Panel With {
                .BackColor = Color.Transparent,
                .Cursor = edge.Cursor,
                .Dock = edge.Dock,
                .Name = "Resize" & edge.Name,
                .TabStop = False
            }

            If edge.Dock = DockStyle.None AndAlso Not edge.Name = "Top" AndAlso Not edge.Name = "Left" Then
                panel.Size = New Size(ResizeBorder, ResizeBorder)
            ElseIf edge.Dock = DockStyle.Bottom Then
                panel.Height = ResizeBorder
            ElseIf edge.Dock = DockStyle.Right Then
                panel.Width = ResizeBorder
            End If

            Dim direction = edge.Direction
            AddHandler panel.MouseDown, Sub(s, ev)
                                            If ev.Button = MouseButtons.Left Then
                                                StartResize(DirectCast(s, Control), direction)
                                            End If
                                        End Sub
            AddHandler panel.MouseMove, AddressOf ResizePanel_MouseMove
            AddHandler panel.MouseUp, AddressOf ResizePanel_MouseUp

            _edgePanels.Add(panel)
            Controls.Add(panel)
            panel.BringToFront()
        Next

        PositionCornerPanels()
        AddHandler Resize, Sub(s, ev) PositionCornerPanels()
    End Sub

    Private Sub PositionCornerPanels()
        If _edgePanels.Count < 7 Then Return

        Dim w = ClientSize.Width
        Dim h = ClientSize.Height
        Dim buttonZoneRight = Math.Max(Button1.Right, ResizeBorder)
        Dim buttonZoneBottom = Math.Max(Button1.Bottom, ResizeBorder)

        '_edgePanels(0).SetBounds(buttonZoneRight, 0, Math.Max(0, w - buttonZoneRight), ResizeBorder)
        '_edgePanels(2).SetBounds(0, buttonZoneBottom, ResizeBorder, Math.Max(0, h - buttonZoneBottom))
        '_edgePanels(4).Location = New Point(w - ResizeBorder, 0)
        '_edgePanels(5).Location = New Point(0, h - ResizeBorder)
        '_edgePanels(6).Location = New Point(w - ResizeBorder, h - ResizeBorder)

        _edgePanels(0).SetBounds(buttonZoneRight, 0, Math.Max(0, w), ResizeBorder)
        _edgePanels(2).SetBounds(0, buttonZoneBottom, ResizeBorder, Math.Max(0, h))
        _edgePanels(4).Location = New Point(w, 0)
        _edgePanels(5).Location = New Point(0, h)
        _edgePanels(6).Location = New Point(w, h)
    End Sub

    Private Sub StartResize(captureControl As Control, direction As ResizeDirection)
        _resizeDirection = direction
        _resizeStartPoint = Cursor.Position
        _resizeStartBounds = Bounds
        _resizeCaptureControl = captureControl
        captureControl.Capture = True
    End Sub

    Private Sub ResizePanel_MouseMove(sender As Object, e As MouseEventArgs)
        If _resizeDirection = ResizeDirection.None Then Return
        ApplyResize()
    End Sub

    Private Sub ResizePanel_MouseUp(sender As Object, e As MouseEventArgs)
        EndResize()
    End Sub

    Private Sub ApplyResize()
        Dim current = Cursor.Position
        Dim dx = current.X - _resizeStartPoint.X
        Dim dy = current.Y - _resizeStartPoint.Y
        Dim bounds = _resizeStartBounds

        Select Case _resizeDirection
            Case ResizeDirection.Left
                bounds.X = _resizeStartBounds.X + dx
                bounds.Width = _resizeStartBounds.Width - dx
            Case ResizeDirection.Right
                bounds.Width = _resizeStartBounds.Width + dx
            Case ResizeDirection.Top
                bounds.Y = _resizeStartBounds.Y + dy
                bounds.Height = _resizeStartBounds.Height - dy
            Case ResizeDirection.Bottom
                bounds.Height = _resizeStartBounds.Height + dy
            Case ResizeDirection.TopLeft
                bounds.X = _resizeStartBounds.X + dx
                bounds.Width = _resizeStartBounds.Width - dx
                bounds.Y = _resizeStartBounds.Y + dy
                bounds.Height = _resizeStartBounds.Height - dy
            Case ResizeDirection.TopRight
                bounds.Width = _resizeStartBounds.Width + dx
                bounds.Y = _resizeStartBounds.Y + dy
                bounds.Height = _resizeStartBounds.Height - dy
            Case ResizeDirection.BottomLeft
                bounds.X = _resizeStartBounds.X + dx
                bounds.Width = _resizeStartBounds.Width - dx
                bounds.Height = _resizeStartBounds.Height + dy
            Case ResizeDirection.BottomRight
                bounds.Width = _resizeStartBounds.Width + dx
                bounds.Height = _resizeStartBounds.Height + dy
        End Select

        If bounds.Width < MinimumSize.Width Then
            If _resizeDirection = ResizeDirection.Left OrElse
               _resizeDirection = ResizeDirection.TopLeft OrElse
               _resizeDirection = ResizeDirection.BottomLeft Then
                bounds.X = _resizeStartBounds.Right - MinimumSize.Width
            End If
            bounds.Width = MinimumSize.Width
        End If

        If bounds.Height < MinimumSize.Height Then
            If _resizeDirection = ResizeDirection.Top OrElse
               _resizeDirection = ResizeDirection.TopLeft OrElse
               _resizeDirection = ResizeDirection.TopRight Then
                bounds.Y = _resizeStartBounds.Bottom - MinimumSize.Height
            End If
            bounds.Height = MinimumSize.Height
        End If

        Bounds = bounds
    End Sub

    Private Sub EndResize()
        If _resizeDirection = ResizeDirection.None Then Return

        _resizeDirection = ResizeDirection.None
        If _resizeCaptureControl IsNot Nothing Then
            _resizeCaptureControl.Capture = False
            _resizeCaptureControl = Nothing
        End If
    End Sub

    Private Sub Button1_MouseDown(sender As Object, e As MouseEventArgs) Handles Button1.MouseDown
        If e.Button = MouseButtons.Left AndAlso Control.ModifierKeys = Keys.Control Then
            BeginFormDrag()
        End If
    End Sub

    Private Sub CloseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloseToolStripMenuItem.Click
        Close()
    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        If WebView21.CoreWebView2 Is Nothing Then Return

        Dim current = If(WebView21.Source?.ToString(), DefaultUrl)
        Dim location = PromptOpenLocation(current)
        If location IsNot Nothing Then
            NavigateToLocation(location)
        End If

        ' thêm 18 kí tự đầu tên file hoặc địa chỉ web vào tiêu đề của form
        Dim title = If(location.Length > 18, location.Substring(location.Length - 18, 18) & "...", location)
        Text = "FullBrowser " & title

    End Sub

    Private Function PromptOpenLocation(current As String) As String
        Using dialog As New Form()
            dialog.Text = "Open"
            dialog.FormBorderStyle = FormBorderStyle.FixedDialog
            dialog.StartPosition = FormStartPosition.CenterParent
            dialog.MinimizeBox = False
            dialog.MaximizeBox = False
            dialog.ShowInTaskbar = False
            dialog.ClientSize = New Size(460, 72)

            Dim textBox As New TextBox With {
                .Location = New Point(12, 12),
                .Size = New Size(356, 23),
                .Text = current,
                .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            }

            Dim browseButton As New Button With {
                .Text = "...",
                .Location = New Point(374, 11),
                .Size = New Size(30, 25),
                .Anchor = AnchorStyles.Top Or AnchorStyles.Right
            }

            Dim okButton As New Button With {
                .Text = "Open",
                .DialogResult = DialogResult.OK,
                .Location = New Point(292, 41),
                .Size = New Size(75, 25),
                .Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            }

            Dim cancelButton As New Button With {
                .Text = "Cancel",
                .DialogResult = DialogResult.Cancel,
                .Location = New Point(373, 41),
                .Size = New Size(75, 25),
                .Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            }

            AddHandler browseButton.Click,
                Sub(s, ev)
                    Using fileDialog As New OpenFileDialog()
                        fileDialog.Filter = "Web files (*.html;*.htm;*.mhtml)|*.html;*.htm;*.mhtml|All files (*.*)|*.*"
                        fileDialog.Title = "Open file"
                        If fileDialog.ShowDialog(dialog) = DialogResult.OK Then
                            textBox.Text = fileDialog.FileName
                        End If
                    End Using
                End Sub

            dialog.Controls.AddRange(New Control() {textBox, browseButton, okButton, cancelButton})
            dialog.AcceptButton = okButton
            dialog.CancelButton = cancelButton

            If dialog.ShowDialog(Me) = DialogResult.OK Then
                Dim value = textBox.Text.Trim()
                If value.Length > 0 Then Return value
            End If
        End Using

        Return Nothing
    End Function

    Private Sub NavigateToLocation(location As String)
        Dim target = ResolveNavigationTarget(location)
        If target Is Nothing Then
            MessageBox.Show(Me, "URL hoặc file không hợp lệ.", "Open", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        WebView21.CoreWebView2.Navigate(target)
    End Sub

    Private Function ResolveNavigationTarget(location As String) As String
        location = location.Trim()
        If location.Length = 0 Then Return Nothing

        If Uri.IsWellFormedUriString(location, UriKind.Absolute) Then
            Return location
        End If

        If File.Exists(location) Then
            Return New Uri(Path.GetFullPath(location)).AbsoluteUri
        End If

        If Directory.Exists(location) Then
            Return New Uri(Path.GetFullPath(location)).AbsoluteUri
        End If

        If Not location.Contains("://") AndAlso Not location.StartsWith("\\") AndAlso Not location.Contains(":"c) Then
            Return "https://" & location
        End If

        Try
            Return New Uri(location, UriKind.RelativeOrAbsolute).AbsoluteUri
        Catch
            Return Nothing
        End Try
    End Function

    Private Sub TopmostToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TopmostToolStripMenuItem.Click
        If Me.TopMost = False Then
            Me.TopMost = True
        Else
            Me.TopMost = False
        End If
    End Sub

    Private Sub BorderOnOffToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BorderOnOffToolStripMenuItem.Click
        If Me.FormBorderStyle = FormBorderStyle.None Then
            Me.FormBorderStyle = FormBorderStyle.Sizable
        Else
            Me.FormBorderStyle = FormBorderStyle.None
        End If
    End Sub

    Private Sub OpacityToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpacityToolStripMenuItem.Click
        Me.Opacity = Me.Opacity + 0.1
        If Me.Opacity > 1 Then Me.Opacity = 1

    End Sub

    Private Sub OpacityToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles OpacityToolStripMenuItem1.Click
        Me.Opacity = Me.Opacity - 0.1
        If Me.Opacity < 0.1 Then Me.Opacity = 0.1
    End Sub
End Class
