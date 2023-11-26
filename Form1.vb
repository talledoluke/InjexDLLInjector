Imports System.Threading
Public Class Form1
    Private TargetProcessHandle As Integer
    Private pfnStartAddr As Integer
    Private pszLibFileRemote As String
    Private TargetBufferSize As Integer
    Public Const PROCESS_VM_READ = &H10
    Public Const TH32CS_SNAPPROCESS = &H2
    Public Const MEM_COMMIT = 4096
    Public Const PAGE_READWRITE = 4
    Public Const PROCESS_CREATE_THREAD = (&H2)
    Public Const PROCESS_VM_OPERATION = (&H8)
    Public Const PROCESS_VM_WRITE = (&H20)
    Public Declare Function ReadProcessMemory Lib "kernel32" ( _
    ByVal hProcess As Integer, _
    ByVal lpBaseAddress As Integer, _
    ByVal lpBuffer As String, _
    ByVal nSize As Integer, _
    ByRef lpNumberOfBytesWritten As Integer) As Integer
    Public Declare Function LoadLibrary Lib "kernel32" Alias "LoadLibraryA" ( _
    ByVal lpLibFileName As String) As Integer
    Public Declare Function VirtualAllocEx Lib "kernel32" ( _
    ByVal hProcess As Integer, _
    ByVal lpAddress As Integer, _
    ByVal dwSize As Integer, _
    ByVal flAllocationType As Integer, _
    ByVal flProtect As Integer) As Integer
    Public Declare Function WriteProcessMemory Lib "kernel32" ( _
    ByVal hProcess As Integer, _
    ByVal lpBaseAddress As Integer, _
    ByVal lpBuffer As String, _
    ByVal nSize As Integer, _
    ByRef lpNumberOfBytesWritten As Integer) As Integer
    Public Declare Function GetProcAddress Lib "kernel32" ( _
    ByVal hModule As Integer, ByVal lpProcName As String) As Integer
    Private Declare Function GetModuleHandle Lib "Kernel32" Alias "GetModuleHandleA" ( _
    ByVal lpModuleName As String) As Integer
    Public Declare Function CreateRemoteThread Lib "kernel32" ( _
    ByVal hProcess As Integer, _
    ByVal lpThreadAttributes As Integer, _
    ByVal dwStackSize As Integer, _
    ByVal lpStartAddress As Integer, _
    ByVal lpParameter As Integer, _
    ByVal dwCreationFlags As Integer, _
    ByRef lpThreadId As IntPtr) As Integer
    Public Declare Function OpenProcess Lib "kernel32" ( _
    ByVal dwDesiredAccess As Integer, _
    ByVal bInheritHandle As Integer, _
    ByVal dwProcessId As Integer) As Integer
    Private Declare Function FindWindow Lib "user32" Alias "FindWindowA" ( _
    ByVal lpClassName As String, _
    ByVal lpWindowName As String) As Integer
    Private Declare Function CloseHandle Lib "kernel32" Alias "CloseHandleA" ( _
    ByVal hObject As Integer) As Integer
    Dim ExeName As String = IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath)
    Dim SCurrentDll As String
    Dim objMutex As Mutex

    Private Sub Inject()
        On Error GoTo 1
        InjectTimer.Stop()
        'Inject Dll
        Dim TargetProcess As Process() = Process.GetProcessesByName(ProcessTextBox.Text)
        Dim lpThreadId As IntPtr
        TargetProcessHandle = OpenProcess(&H1F0FFF, 0, TargetProcess(0).Id)
        Dim FileDll = SCurrentDll
        pszLibFileRemote = (FileDll)
        pfnStartAddr = GetProcAddress(GetModuleHandle("Kernel32"), "LoadLibraryA")
        TargetBufferSize = 1 + Len(pszLibFileRemote)
        Dim Rtn As Integer
        Dim LoadLibParamAdr As Integer
        LoadLibParamAdr = VirtualAllocEx(TargetProcessHandle, 0, TargetBufferSize, MEM_COMMIT, PAGE_READWRITE)
        Rtn = WriteProcessMemory(TargetProcessHandle, LoadLibParamAdr, pszLibFileRemote, TargetBufferSize, 0)
        CreateRemoteThread(TargetProcessHandle, 0, 0, pfnStartAddr, LoadLibParamAdr, 0, lpThreadId)
1:      Me.Show()
    End Sub
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        objMutex = New Mutex(False, "ST70R")
        If objMutex.WaitOne(0, False) = False Then
            objMutex.Close()
            objMutex = Nothing
            MessageBox.Show("Error !!")
            End
        End If



    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InjectTimer.Tick
        If IO.File.Exists(OpenFileDialog1.FileName) Then
            Dim TargetProcess As Process() = Process.GetProcessesByName(ProcessTextBox.Text)
            If TargetProcess.Length = 0 Then
                InjectTimer.Stop()
                DelayTimer.Start()
            End If
        Else
        End If
    End Sub

    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DelayTimer.Tick
        If DelayNumeric.Value = 0 Then
            InjectButton.Text = "Injected!"
            InjectButton.Enabled = False
            For i = 0 To (DllListBox.Items.Count + -1)
                SCurrentDll = DllListBox.Items(i)
                Call Inject()
                If CloseCheckBox.Checked = True Then

                    End
                Else
                End If
            Next i
        Else
            DelayNumeric.Value = DelayNumeric.Value - 1
        End If
    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If IO.File.Exists(OpenFileDialog1.FileName) Then
            Dim TargetProcess As Process() = Process.GetProcessesByName(ProcessTextBox.Text)
            If TargetProcess.Length = 0 Then
                InjectTimer.Stop()
                DelayTimer.Start()
            End If
        Else
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        OpenFileDialog1.Filter = "DLL (*.dll) |*.dll"
        OpenFileDialog1.ShowDialog()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        For i As Integer = (DllListBox.SelectedItems.Count - 1) To 0 Step -1
            DllListBox.Items.Remove(DllListBox.SelectedItems(i))
        Next
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        DllListBox.Items.Clear()
    End Sub

    Private Sub OpenFileDialog1_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        Dim FileName As String
        FileName = OpenFileDialog1.FileName.Substring(OpenFileDialog1.FileName.LastIndexOf("\"))
        Dim DllFileName As String = FileName.Replace("\", "")
        DllListBox.Items.Add(DllFileName)
    End Sub

    Private Sub RadioButton2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton2.CheckedChanged
        InjectButton.Enabled = True
        InjectTimer.Enabled = False

    End Sub

    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged
        InjectButton.Enabled = False
        InjectTimer.Enabled = True
    End Sub

    Private Sub InjectButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InjectButton.Click
        If IO.File.Exists(OpenFileDialog1.FileName) Then
            Dim TargetProcess As Process() = Process.GetProcessesByName(ProcessTextBox.Text)
            If TargetProcess.Length = 0 Then
                InjectButton.Text = ("Waiting for " + ProcessTextBox.Text + ".exe")
            Else
                InjectTimer.Stop()
                DelayTimer.Start()
            End If
        Else
        End If

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Form2.Show()
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        Form3.Show()
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click

    End Sub
End Class
