Public Class Form2

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.Hide()
    End Sub

    Private Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim plist() As Process = Process.GetProcesses()
        For Each prs As Process In plist
            ListBox1.Items.Add(prs.ProcessName)
        Next
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Form1.ProcessTextBox.Text = Me.ListBox1.SelectedItem.ToString()
        Me.Hide()
    End Sub
End Class
