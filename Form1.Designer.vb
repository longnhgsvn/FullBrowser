<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        WebView21 = New Microsoft.Web.WebView2.WinForms.WebView2()
        Button1 = New Button()
        ContextMenuStrip1 = New ContextMenuStrip(components)
        OpenToolStripMenuItem = New ToolStripMenuItem()
        CloseToolStripMenuItem = New ToolStripMenuItem()
        TopmostToolStripMenuItem = New ToolStripMenuItem()
        BorderOnOffToolStripMenuItem = New ToolStripMenuItem()
        OpacityToolStripMenuItem = New ToolStripMenuItem()
        OpacityToolStripMenuItem1 = New ToolStripMenuItem()
        CType(WebView21, ComponentModel.ISupportInitialize).BeginInit()
        WebView21.SuspendLayout()
        ContextMenuStrip1.SuspendLayout()
        SuspendLayout()
        ' 
        ' WebView21
        ' 
        WebView21.AllowExternalDrop = True
        WebView21.Controls.Add(Button1)
        WebView21.CreationProperties = Nothing
        WebView21.DefaultBackgroundColor = Color.White
        WebView21.Dock = DockStyle.Fill
        WebView21.Location = New Point(0, 0)
        WebView21.Margin = New Padding(0)
        WebView21.Name = "WebView21"
        WebView21.Size = New Size(706, 450)
        WebView21.TabIndex = 0
        WebView21.ZoomFactor = 1R
        ' 
        ' Button1
        ' 
        Button1.ContextMenuStrip = ContextMenuStrip1
        Button1.Location = New Point(0, 0)
        Button1.Margin = New Padding(0)
        Button1.Name = "Button1"
        Button1.Size = New Size(18, 14)
        Button1.TabIndex = 0
        Button1.Text = "..."
        Button1.UseVisualStyleBackColor = True
        ' 
        ' ContextMenuStrip1
        ' 
        ContextMenuStrip1.Items.AddRange(New ToolStripItem() {OpenToolStripMenuItem, CloseToolStripMenuItem, TopmostToolStripMenuItem, BorderOnOffToolStripMenuItem, OpacityToolStripMenuItem, OpacityToolStripMenuItem1})
        ContextMenuStrip1.Name = "ContextMenuStrip1"
        ContextMenuStrip1.Size = New Size(151, 136)
        ' 
        ' OpenToolStripMenuItem
        ' 
        OpenToolStripMenuItem.Name = "OpenToolStripMenuItem"
        OpenToolStripMenuItem.Size = New Size(150, 22)
        OpenToolStripMenuItem.Text = "Open"
        ' 
        ' CloseToolStripMenuItem
        ' 
        CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
        CloseToolStripMenuItem.Size = New Size(150, 22)
        CloseToolStripMenuItem.Text = "Close"
        ' 
        ' TopmostToolStripMenuItem
        ' 
        TopmostToolStripMenuItem.Name = "TopmostToolStripMenuItem"
        TopmostToolStripMenuItem.Size = New Size(150, 22)
        TopmostToolStripMenuItem.Text = "Topmost"
        ' 
        ' BorderOnOffToolStripMenuItem
        ' 
        BorderOnOffToolStripMenuItem.Name = "BorderOnOffToolStripMenuItem"
        BorderOnOffToolStripMenuItem.Size = New Size(150, 22)
        BorderOnOffToolStripMenuItem.Text = "Border On/Off"
        ' 
        ' OpacityToolStripMenuItem
        ' 
        OpacityToolStripMenuItem.Name = "OpacityToolStripMenuItem"
        OpacityToolStripMenuItem.Size = New Size(150, 22)
        OpacityToolStripMenuItem.Text = "Opacity +"
        ' 
        ' OpacityToolStripMenuItem1
        ' 
        OpacityToolStripMenuItem1.Name = "OpacityToolStripMenuItem1"
        OpacityToolStripMenuItem1.Size = New Size(150, 22)
        OpacityToolStripMenuItem1.Text = "Opacity -"
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(706, 450)
        Controls.Add(WebView21)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "Form1"
        Text = "Full Browser"
        CType(WebView21, ComponentModel.ISupportInitialize).EndInit()
        WebView21.ResumeLayout(False)
        ContextMenuStrip1.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents WebView21 As Microsoft.Web.WebView2.WinForms.WebView2
    Friend WithEvents Button1 As Button
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents OpenToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CloseToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TopmostToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BorderOnOffToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpacityToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpacityToolStripMenuItem1 As ToolStripMenuItem

End Class
