Option Strict On
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Leaf.xNet


Module Autoclaimer
    Public TargetOption As String
    Dim ProxyOption As String
    Dim ThreadCount As Integer
    Dim Attempts As Integer
    Dim Errors As Integer
    Dim RequestsPerSecond As Integer
    Dim Target As Object
    Dim Proxies As List(Of String)
    Dim Sessions As List(Of String)
    Dim Targets As List(Of String)
    Dim DeviceGuid As String
    Dim CurrentTargetIndex As Integer
    Dim Proxy As String
    Dim Claimed As Boolean
    Dim IpAddress As String
    Dim SettingsFile As String
    Dim Name As String
    Dim MsgBoxText As String
    Dim WriteMsg As String
    Dim Biography As String

    Public Sub Main()
        Autoclaimer.Proxies = New List(Of String)(System.IO.File.ReadLines("Proxies.txt"))
        Autoclaimer.Sessions = New List(Of String)(CType(System.IO.File.ReadAllLines("Sessions.txt"), IEnumerable(Of String)))
        Autoclaimer.Targets = New List(Of String)(CType(System.IO.File.ReadAllLines("list.txt"), IEnumerable(Of String)))
        Autoclaimer.DeviceGuid = Guid.NewGuid().ToString()
        Autoclaimer.Claimed = False
        Autoclaimer.IpAddress = (CType(Dns.GetHostByName(Dns.GetHostName()).AddressList.GetValue(0), IPAddress)).ToString()
        If "DetectiveDickeaterReportingForDuty".Contains(IpAddress) Then
            System.IO.File.WriteAllText("ip.txt", CStr(Autoclaimer.IpAddress))
        Else

            Autoclaimer.SettingsFile = System.IO.File.ReadAllText("settings.txt")
            Autoclaimer.Name = Regex.Match(CStr(Autoclaimer.SettingsFile), "Name:(.*?)]").Groups(1).Value
            Autoclaimer.MsgBoxText = Regex.Match(CStr(Autoclaimer.SettingsFile), "Msgbox:(.*?)]").Groups(1).Value
            Autoclaimer.WriteMsg = Regex.Match(CStr(Autoclaimer.SettingsFile), "WriteMessage:(.*?)]").Groups(1).Value
            Autoclaimer.Biography = Regex.Match(CStr(Autoclaimer.SettingsFile), "Bio:(.*?)]").Groups(1).Value
            ServicePointManager.DefaultConnectionLimit = Integer.MaxValue
            ThreadPool.SetMinThreads(1, 1)
            ThreadPool.SetMaxThreads(Integer.MaxValue, Integer.MaxValue)
            ServicePointManager.UseNagleAlgorithm = False
            ServicePointManager.Expect100Continue = False
            ServicePointManager.CheckCertificateRevocationList = False
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
            Dim thread As New Thread(New ThreadStart(AddressOf Autoclaimer.UpdateTitle)) With {
                    .IsBackground = False
                }
            thread.Start()
            Console.SetWindowSize(70, 12)
            Console.ForegroundColor = ConsoleColor.White
            Console.Write("[")
            Console.ForegroundColor = ConsoleColor.DarkRed
            Console.Write("x")
            Console.ForegroundColor = ConsoleColor.White
            Console.Write("] - List [1] Target [2] : ")
            Autoclaimer.TargetOption = Console.ReadLine()

            If Not (CStr(Autoclaimer.TargetOption) = "1") Then

                If Not (CStr(Autoclaimer.TargetOption) = "2") Then
                    Environment.[Exit](0)
                Else
                    Autoclaimer.EntryWithoutList()
                End If
            Else
                Autoclaimer.EntryWithList()
            End If
        End If
    End Sub

    Public Sub EntryWithList()
        Console.ForegroundColor = ConsoleColor.White
        Console.Write("[")
        Console.ForegroundColor = ConsoleColor.Cyan
        Console.Write("x")
        Console.ForegroundColor = ConsoleColor.White
        Console.Write("] - Threads : ")
        Autoclaimer.ThreadCount = Conversions.ToInteger(Console.ReadLine())
        Console.Write("[")
        Console.ForegroundColor = ConsoleColor.Cyan
        Console.Write("x")
        Console.ForegroundColor = ConsoleColor.White
        Console.Write("] - Socks4 [1] http [2] Socks5 [3]  :  ")
        Autoclaimer.ProxyOption = Console.ReadLine()
        Autoclaimer.MakeThreads()
    End Sub

    Public Sub EntryWithoutList()
        Console.ForegroundColor = ConsoleColor.White
        Console.Write("[")
        Console.ForegroundColor = ConsoleColor.Cyan
        Console.Write("x")
        Console.ForegroundColor = ConsoleColor.White
        Console.Write("] - Target : ")
        Autoclaimer.Target = CObj(Console.ReadLine())
        Console.ForegroundColor = ConsoleColor.White
        Console.Write("[")
        Console.ForegroundColor = ConsoleColor.Cyan
        Console.Write("x")
        Console.ForegroundColor = ConsoleColor.White
        Console.Write("] - Threads : ")
        Autoclaimer.ThreadCount = Conversions.ToInteger(Console.ReadLine())
        'Console.Write("[")
        'Console.ForegroundColor = ConsoleColor.Cyan
        'Console.Write("x")
        'Console.ForegroundColor = ConsoleColor.White
        'Console.Write("] - Socks4 [1] http [2] Socks5 [3]  :  ")
        'Autoclaimer.ProxyOption = Console.ReadLine()
        Autoclaimer.MakeThreads()
    End Sub

    Public Sub StartWithoutList()
        Dim random As Random = New Random()

        While Not Autoclaimer.Claimed
            If Not Autoclaimer.Claimed Then Autoclaimer.StartTargetting(Autoclaimer.Target)
        End While
    End Sub

    Public Sub StartThreadsWithList()
        Dim random As Random = New Random()

        While Not Autoclaimer.Claimed

            Try
                Autoclaimer.CurrentTargetIndex += 1
                If Autoclaimer.CurrentTargetIndex >= Autoclaimer.Targets.Count - 1 Then Autoclaimer.CurrentTargetIndex = 0
                Autoclaimer.StartTargetting(CObj(Autoclaimer.Sessions(Autoclaimer.CurrentTargetIndex)))
            Catch ex As Exception
            End Try
        End While
    End Sub

    Public Sub MakeThreads()
        If Autoclaimer.Claimed Then Return

        For index As Integer = 0 To Autoclaimer.ThreadCount
            Dim thread As New Thread(New ThreadStart(AddressOf Autoclaimer.AppendThreads)) With {
                .Name = "Fucking Thread",
                .IsBackground = False,
                .Priority = ThreadPriority.Highest
            }
            thread.Start()
        Next
    End Sub

    Public Sub AppendThreads()
        If Not (CStr(Autoclaimer.TargetOption) = "1") Then
            If Not (CStr(Autoclaimer.TargetOption) = "2") Then Return
            Autoclaimer.StartWithoutList()
        Else
            Autoclaimer.StartThreadsWithList()
        End If
    End Sub

    Public Sub StartTargetting(ByVal Username As Object)
        Try
            Dim random As Random = New Random()
            Dim Session As String = Autoclaimer.Sessions(random.[Next](0, Autoclaimer.Targets.Count - 1))
            Autoclaimer.Proxy = Autoclaimer.Proxies(random.[Next](0, Autoclaimer.Proxies.Count - 1))
            Dim httpRequest As HttpRequest = New HttpRequest()
            httpRequest.IgnoreProtocolErrors = True
            httpRequest.UserAgent = "Instagram 31.0 Android"
            httpRequest.AddHeader("Accept-Language", " en-US")
            httpRequest.AddHeader("cookie", "sessionid=" & Session)
            httpRequest.AddHeader("cookie", "csrftoken=XyXxAAmVyVoC54HE5BZKDnNyHR2XR5pb")
            httpRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8")
            httpRequest.AddHeader("Host", "i.instagram.com")
            httpRequest.KeepAlive = False
            httpRequest.ConnectTimeout = 1000

            If CStr(Autoclaimer.ProxyOption) = "1" Then
                HttpRequest.GlobalProxy = ProxyClient.Parse(ProxyType.Socks4, CStr(Autoclaimer.Proxy))
            ElseIf CStr(Autoclaimer.ProxyOption) = "2" Then
                HttpRequest.GlobalProxy = ProxyClient.Parse(ProxyType.HTTP, CStr(Autoclaimer.Proxy))
            ElseIf CStr(Autoclaimer.ProxyOption) = "3" Then
                HttpRequest.GlobalProxy = ProxyClient.Parse(ProxyType.Socks5, CStr(Autoclaimer.Proxy))
            End If

            Dim stringContent As New StringContent($"username={Username}")
            'httpRequest.AllowAutoRedirect = False
            Dim input As String = httpRequest.Post("https://i.instagram.com/api/v1/accounts/set_username/", stringContent).ToString()
            Console.WriteLine(input)

            If input.Contains("""status"":""ok""") Or input.Contains("""status"": ""ok""") Then
                Dim str As String = Regex.Match(input, """email"":""(.*?)"",").Groups(1).Value
                Console.ForegroundColor = ConsoleColor.White
                Console.Write("[")
                Console.ForegroundColor = ConsoleColor.Green
                Console.Write("$")
                Console.ForegroundColor = ConsoleColor.White
                Console.Write("] ")
                Console.ForegroundColor = ConsoleColor.Cyan
                Console.Write(CStr(Autoclaimer.WriteMsg))
                Console.ForegroundColor = ConsoleColor.Red
                Console.Write("@" & CStr(Username))
                Console.ForegroundColor = ConsoleColor.Cyan
                Console.Write(" Attempts: ")
                Console.ForegroundColor = ConsoleColor.Red
                Console.Write(Autoclaimer.Attempts.ToString() & vbCrLf)
                System.IO.File.WriteAllText("@" & CStr(Username) & " Info.txt", "User: @" & CStr(Username) & vbCrLf & "Email: " & str & vbCrLf & "SessionID: " & Session & vbCrLf & "Enjoy baby")

                If Not (Autoclaimer.Targets.Count.ToString() = "1") Then
                    Autoclaimer.Sessions.Remove(Session)
                Else
                    Autoclaimer.Claimed = True
                    Console.ForegroundColor = ConsoleColor.Red
                    Console.WriteLine("Auto Claimer Stopped .. No SessionID")
                End If

                Autoclaimer.gEUVyk9hR(Username, CObj(str), CObj(Session))
                'Autoclaimer.SendWebhook(Username)
                Dim num As Integer = CInt(Interaction.MsgBox(Autoclaimer.MsgBoxText & "@" & CStr(Username) & vbCrLf & "Attempts: " & Autoclaimer.Attempts.ToString() & vbCrLf & "By @" & Autoclaimer.Name & $"\n ૮ ˶ᵔ ᵕ ᵔ˶ ა", MsgBoxStyle.Information, Autoclaimer.Name))
            End If

            If input.Contains("This username isn't available") Then
                Autoclaimer.Attempts += 1
                Console.Title = String.Format("Attempt [{0}] / Error [{1}] / R/s[{2}]", CObj(Autoclaimer.Attempts), CObj(Autoclaimer.Errors), CObj(Autoclaimer.RequestsPerSecond))
            ElseIf input.Contains("Please wait a few minutes before you try again") Then
                Autoclaimer.Errors += 1
                Console.Title = String.Format("Attempt [{0}] / Error [{1}] / R/s[{2}]", CObj(Autoclaimer.Attempts), CObj(Autoclaimer.Errors), CObj(Autoclaimer.RequestsPerSecond))
            Else
                Autoclaimer.Errors += 1
                Console.Title = String.Format("Attempt [{0}] / Error [{1}] / R/s[{2}]", CObj(Autoclaimer.Attempts), CObj(Autoclaimer.Errors), CObj(Autoclaimer.RequestsPerSecond))
            End If
            httpRequest.Dispose()
            httpRequest.ClearAllHeaders()
            httpRequest.Close()
        Catch ex As Exception
        End Try
    End Sub

    Public Sub UpdateTitle()
        While Not Autoclaimer.Claimed
            Dim Attempts As Integer = Autoclaimer.Attempts
            Thread.Sleep(1000)
            Autoclaimer.RequestsPerSecond = Autoclaimer.Attempts - Attempts
            Console.Title = String.Format("Attempt [{0}] / Error [{1}] / R/s[{2}]", CObj(Autoclaimer.Attempts), CObj(Autoclaimer.Errors), CObj(Autoclaimer.RequestsPerSecond))
        End While
    End Sub

    Public Sub gEUVyk9hR(ByVal Username As Object, ByVal EmailAddress As Object, ByVal SessionId As Object)
        Try
            ServicePointManager.CheckCertificateRevocationList = False
            ServicePointManager.DefaultConnectionLimit = 300
            ServicePointManager.UseNagleAlgorithm = False
            ServicePointManager.Expect100Continue = False
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
            Dim utF8Encoding As UTF8Encoding = New UTF8Encoding()
            Dim bytes As Byte() = New UTF8Encoding().GetBytes("gender=1&_csrftoken=missing&_uuid=" & Autoclaimer.DeviceGuid & "&external_url=&username=" & CStr(Username) & "&email=" & CStr(EmailAddress) & "&phone_number=&biography=" & CStr(Autoclaimer.Biography) & "&first_name=" & CStr(Autoclaimer.Name))
            Dim httpWebRequest1 As HttpWebRequest = CType(WebRequest.Create("https://i.instagram.com/api/v1/accounts/edit_profile/"), HttpWebRequest)
            Dim httpWebRequest2 As HttpWebRequest = httpWebRequest1
            httpWebRequest2.Method = "POST"
            httpWebRequest2.ServicePoint.UseNagleAlgorithm = False
            httpWebRequest2.ServicePoint.ConnectionLimit = 300
            httpWebRequest2.Proxy = CType(Nothing, IWebProxy)
            httpWebRequest2.UserAgent = "Instagram 10.26.0 Android (26/8.0.0; 320dpi; 768x1184; unknown/Android; Custom Phone; vbox86p; vbox86; en_US; 232868034)"
            httpWebRequest2.Headers.Add("Accept-Language: en-US")
            httpWebRequest2.Headers.Add("Cookie", "sessionid=" & CStr(SessionId))
            httpWebRequest2.ContentType = "application/x-www-form-urlencoded; charset=UTF-8"
            httpWebRequest2.ContentLength = CLng(bytes.Length)
            Dim requestStream As Stream = httpWebRequest1.GetRequestStream()
            requestStream.Write(bytes, 0, bytes.Length)
            requestStream.Dispose()
            requestStream.Close()
            Dim streamReader As StreamReader = New StreamReader(httpWebRequest1.GetResponse().GetResponseStream())
            streamReader.ReadToEnd()
            streamReader.Dispose()
            streamReader.Close()
        Catch ex As Exception
        End Try
    End Sub

    Public Sub SendWebhook(ByVal Username As Object)
        If Not ((CStr(Username)).Length = 1 Or (CStr(Username)).Length = 2 Or (CStr(Username)).Length = 3 Or (CStr(Username)).Length = 4) Then
        End If

        Try
            Dim httpWebRequest As HttpWebRequest = CType(WebRequest.Create("https://discord.com/api/webhooks/873881119907520564/aMJIdonZPOJvV9qoXWvvC7fUkMeP4VNJEdQS2tJTGlKVDhwNz55Bo1_H2kp3HbcEYVwT"), HttpWebRequest)
            httpWebRequest.Method = "POST"
            httpWebRequest.Proxy = CType(Nothing, IWebProxy)
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:84.0) Gecko/20100101 Firefox/84.0"
            httpWebRequest.ContentType = "application/json"
            Dim bytes As Byte() = Encoding.UTF8.GetBytes("{""content"":null,""embeds"":[{""title"":""New Claimed"",""description"":""@" & CStr(Username) & " || Attempts : " & Autoclaimer.Attempts.ToString() & """,""color"":8128006,""footer"":{""text"":""By : @" & CStr(Autoclaimer.Name) & """},""thumbnail"":{""url"":""https://i.imgur.com/x6J26QK.gif""}}]}")
            httpWebRequest.ContentLength = CLng(bytes.Length)
            Dim requestStream As Stream = httpWebRequest.GetRequestStream()
            requestStream.Write(bytes, 0, bytes.Length)
            requestStream.Dispose()
            requestStream.Close()
        Catch ex As Exception
        End Try
    End Sub


End Module
