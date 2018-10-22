Imports System.Windows.Forms


Public Class Form1
    Dim wc As Camera

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        wc = New Camera(PictureBox1)
        wc.Start()

        PictureBox2.SizeMode = PictureBoxSizeMode.AutoSize
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        PictureBox2.SizeMode = PictureBoxSizeMode.AutoSize
        Dim Mybmp As Image
        Dim Mybmp2 As Image
        'wc.CopyToClipBoard(Mybmp)
        'wc.StopWebCam()
        'wc.Start()
        'Mybmp2 = Clipboard.GetImage()
        Mybmp = PictureBox1.Image
        Mybmp2 = Mybmp.Clone
        Mybmp2.RotateFlip(RotateFlipType.Rotate180FlipNone)
        PictureBox2.Image = Mybmp2

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Try
            'PictureBox2.SizeMode = PictureBoxSizeMode.AutoSize
            Dim Mybmp As Bitmap
            Dim Mybmp2 As Bitmap
            wc.CopyToClipBoard(Mybmp)
            Mybmp2 = Clipboard.GetImage()
            Mybmp2.RotateFlip(RotateFlipType.RotateNoneFlipXY)
            'Mybmp2 = GrayBMP(Mybmp2)
            PictureBox2.Image = Mybmp2
            Clipboard.Clear()
        Catch ex As Exception
            Console.WriteLine(ex.ToString)
        End Try

        Label3.Text = PictureBox1.Width & " X " & PictureBox1.Height
        Label4.Text = PictureBox2.Width & " X " & PictureBox2.Height

    End Sub

    Private Function GrayBMP(ByVal bmp As Bitmap) As Bitmap
        For i As Integer = 0 To bmp.Width - 1
            For j As Integer = 0 To bmp.Height - 1
                Dim color As Color = bmp.GetPixel(i, j)
                Dim gray As Integer
                gray = color.R * 0.3 + color.G * 0.59 + color.B * 0.11
                Dim newColor As Color = Color.FromArgb(gray, gray, gray)
                bmp.SetPixel(i, j, newColor)
            Next
        Next
        Return bmp
    End Function

End Class




Public Class Camera

#Region "常數"
    '定義常數
    '開始攝影
    Const WM_CAP_START = CType(&H400, Integer)
    '停止攝影
    Const WM_CAP_STOP = WM_CAP_START + 68
    '抓圖
    Private Const WM_CAP_SAVEDIB As Integer = WM_CAP_START + 25
    '攝影參數
    Const WM_CAP_FILE_SET_CAPTURE_FILEA = WM_CAP_START + 20
    Const WM_CAP_SEQUENCE = WM_CAP_START + 62
    Const WS_CHILD = &H40000000
    Const WS_VISIBLE = &H10000000
    '取得與視訊的聯繫
    Const WM_CAP_DRIVER_CONNECT = WM_CAP_START + 10
    '視窗大小
    Const WM_CAP_SET_SCALE = WM_CAP_START + 53
    '中斷與視訊的聯繫
    Const WM_CAP_DRIVER_DISCONNECT = WM_CAP_START + 11
    Const WM_CAP_SET_PREVIEWRATE = WM_CAP_START + 52
    Const WM_CAP_SET_PREVIEW = WM_CAP_START + 50

    '
    Const WM_CAP_EDIT_COPY = &H41E
    Const WM_CAP_GRAB_FRAME = &H43C
#End Region

#Region "宣告"
    '設備
    Private hWndC As IntPtr

    '要顯示畫面的控制項
    Private myPic As PictureBox
    'api
    Private Declare Function capCreateCaptureWindowA Lib "avicap32.dll" (ByVal Name As String, ByVal Style As Integer, ByVal x As Integer, ByVal y As Integer, ByVal Width As Integer, ByVal Height As Integer, ByVal hWndParent As IntPtr, ByVal nID As Integer) As IntPtr
    Private Declare Function SendMessageA Lib "user32" (ByVal hWnd As IntPtr, ByVal msg As Integer, ByVal wParam As Integer, ByVal filePath As String) As Integer
#End Region

#Region "對外方法"

    '建購子-傳入要畫面顯示的控制項
    Public Sub New(ByVal pic As PictureBox)
        myPic = pic
    End Sub

    '開始顯示影像   
    Public Sub Start()
        hWndC = capCreateCaptureWindowA("WebCam", WS_CHILD Or WS_VISIBLE, 0, 0, myPic.Width, myPic.Height, myPic.Handle, 0)

        If hWndC.ToInt32() <> 0 Then
            '取得與視訊的聯繫
            SendMessageA(hWndC, WM_CAP_DRIVER_CONNECT, 0, 0)
            '讓視訊取得的視窗大小與符合要顯示的控制項大小
            SendMessageA(hWndC, WM_CAP_SET_SCALE, 1, 0)
            '---set the preview rate (ms)---
            SendMessageA(hWndC, WM_CAP_SET_PREVIEWRATE, 30, 0)
            '---start previewing the image---
            SendMessageA(hWndC, WM_CAP_SET_PREVIEW, 1, 0)
        End If
    End Sub

    '停止顯示影像
    Public Sub StopWebCam()
        SendMessageA(hWndC, WM_CAP_DRIVER_DISCONNECT, 0, 0)
    End Sub

    '抓圖
    Public Sub SaveImage(ByVal filePath As String)
        SendMessageA(hWndC, WM_CAP_SAVEDIB, 0, filePath)
    End Sub


    Public Sub CopyToClipBoard(ByVal MyBitmap As Bitmap)
        If (SendMessageA(hWndC, WM_CAP_GRAB_FRAME, 0, 0) > 0) Then
            If (SendMessageA(hWndC, WM_CAP_EDIT_COPY, 0, 0) > 0) Then
                Dim MyCO = Clipboard.GetDataObject()

                If (MyCO.GetDataPresent(DataFormats.Bitmap)) Then
                    MyBitmap = MyCO.GetData(DataFormats.Bitmap)
                End If

            End If
        End If

    End Sub
#End Region

End Class
