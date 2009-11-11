Module Module1

    Sub Main(ByVal sArgs() As String)
        Dim oWeb As New System.Net.WebClient
        Dim strResult As String
        Dim boolAuthorized As Boolean = True

        If sArgs.Count = 0 Then
            PrintUsage()
            Exit Sub
        End If

        Dim strProwlKey As String = ""
        Dim strApp As String = ""
        Dim strEvent As String = ""
        Dim strDesc As String = ""
        Dim strPriority As String = ""

        Dim myOpts As New Collection
        myOpts = GetOpts(sArgs)

        If myOpts.Contains("-k") Then
            strProwlKey = myOpts.Item("-k")
            If strProwlKey = "0" Then
                PrintError("No prowl key specified!")
                Exit Sub
            End If
        Else
            PrintError("No prowl key specified!")
            Exit Sub
        End If

        If myOpts.Contains("-a") Then
            strApp = myOpts.Item("-a")
        Else
            PrintError("No application name specified!")
            Exit Sub
        End If

        If myOpts.Contains("-e") Then
            strEvent = myOpts.Item("-e")
        Else
            PrintError("No event name specified!")
            Exit Sub
        End If

        If myOpts.Contains("-d") Then
            strDesc = myOpts.Item("-d")
        Else
            PrintError("No description specified!")
            Exit Sub
        End If

        If myOpts.Contains("-p") Then
            strPriority = myOpts.Item("-p")
        Else
            strPriority = "0"
        End If

        Dim strProwlURL As String = "https://prowl.weks.net/publicapi/add?apikey=" + strProwlKey + "&application=" + strApp + "&event=" + strEvent + "&description=" + strDesc + "&priority=" + strPriority

        Try
            strResult = oWeb.DownloadString(strProwlURL)
        Catch ex As Exception
            If ex.Message.ToString.Contains("(401) Unauthorized") Then
                boolAuthorized = False
            Else
                Console.WriteLine("Unknown error: " + ex.Message)
            End If
        End Try

        If boolAuthorized = True Then
            Dim doc As New Xml.XmlDocument
            doc.LoadXml(strResult)

            Dim reader As New Xml.XmlNodeReader(doc)
            While reader.Read
                Select Case reader.NodeType
                    Case Xml.XmlNodeType.Element
                        If reader.Name = "success" Then
                            If reader.GetAttribute("code").ToString = "200" Then
                                If Not myOpts.Contains("-q") Then
                                    Console.WriteLine("Successfully posted notification.")
                                End If
                            End If
                        End If
                End Select
            End While
        Else
            Console.WriteLine("Invalid Prowl API key.")
        End If
    End Sub

    Sub PrintUsage()
        Console.WriteLine("WinProwl Version 1.0")
        Console.WriteLine("Written by Mike Cherry <mcherry@inditech.org>")
        Console.WriteLine("")
        Console.WriteLine("Usage: WinProwl.exe -a " + Chr(34) + "Name" + Chr(34) + " -e " + Chr(34) + "Event" + Chr(34) + " -d " + Chr(34) + "Description" + Chr(34) + " -p Priority")
        Console.WriteLine("")
        Console.WriteLine("-p is optional and defaults to 0")
    End Sub

    Sub PrintError(ByVal strErrorMsg As String)
        PrintUsage()
        Console.WriteLine("")
        Console.WriteLine(strErrorMsg)
    End Sub

    Public Function GetOpts(ByVal strOptions() As String) As Collection
        Dim retCollection As New Collection
        Dim strOpt, strValue As String
        Dim i As Integer

        For i = 0 To strOptions.Length - 1
            strOpt = strOptions(i)

            If i < (strOptions.Length - 1) Then
                If strOptions(i + 1).StartsWith("-") Then
                    strValue = "0"
                Else
                    i += 1
                    strValue = strOptions(i)
                End If
            Else
                strValue = "0"
            End If

            retCollection.Add(strValue, strOpt)
        Next

        Return retCollection
    End Function

End Module
