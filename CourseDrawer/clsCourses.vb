''' <summary>
''' Class for collection of courses
''' </summary>
''' <remarks>Singleton class for wrapping courses</remarks>
Public Class clsCourses
    Private _courses As List(Of clsCourse)
    Private _selectedWP As clsWaypoint
    Private _seledtedCrs As Integer
    Public ReadOnly Property Count As Integer
        Get
            Return _courses.Count
        End Get
    End Property
    Private Shared _instance As clsCourses
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
        _courses = New List(Of clsCourse)
        AddHandler clsWaypoint.SelectionChanged, AddressOf Me.selectedChangeHandler
    End Sub
    ''' <summary>
    ''' Get instance of singleton class
    ''' </summary>
    ''' <param name="forceNew">force create new instance</param>
    ''' <returns>Instance of courses collection</returns>
    ''' <remarks></remarks>
    Public Shared Function getInstance(Optional ByVal forceNew As Boolean = False) As clsCourses
        If _instance Is Nothing Or forceNew = True Then
            _instance = New clsCourses
        End If
        Return _instance
    End Function
    ''' <summary>
    ''' Get list of course name and hidden attribute
    ''' </summary>
    ''' <value></value>
    ''' <returns>Dictionary of course names and hidden attribute</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property CourseList As Dictionary(Of String, Boolean)
        Get
            Dim dir As New Dictionary(Of String, Boolean)
            For Each crs As clsCourse In _courses
                dir.Add(crs.id.ToString & " : " & crs.Name, Not crs.Hidden)
            Next
            Return dir
        End Get
    End Property
    ''' <summary>
    ''' Set course hidden or visible
    ''' </summary>
    ''' <param name="id">id</param>
    ''' <param name="hide">true = hidden, false = visible</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ItemHide(ByVal id As Integer, ByVal hide As Boolean) As Boolean
        If id < 0 Or id >= _courses.Count Then Return False
        _courses(id).Hidden = hide
        Return True
    End Function
    ''' <summary>
    ''' Read courses from XML file
    ''' </summary>
    ''' <param name="file">filename incl. full path</param>
    ''' <remarks></remarks>
    Public Sub ReadXML(ByVal file As String)
        Dim xmlDoc As New Xml.XmlDocument()
        Dim xmlNode As Xml.XmlNode
        Dim xmlNodeReader As Xml.XmlNodeReader
        Dim course As New clsCourse
        Dim waypoint As New clsWaypoint
        Dim stringA() As String

        If file = String.Empty Then Exit Sub

        xmlDoc.Load(file)
        If xmlDoc Is Nothing Then Exit Sub
        xmlNode = xmlDoc.DocumentElement.SelectSingleNode("courses")
        xmlNodeReader = New Xml.XmlNodeReader(xmlNode)
        Do While (xmlNodeReader.Read())
            Select Case xmlNodeReader.NodeType
                Case Xml.XmlNodeType.Element
                    If xmlNodeReader.LocalName = "course" Then
                        course = New clsCourse
                        _courses.Add(course)
                        While xmlNodeReader.MoveToNextAttribute
                            Select Case xmlNodeReader.LocalName
                                Case "name"
                                    course.Name = xmlNodeReader.Value
                                Case "id"
                                    Integer.TryParse(xmlNodeReader.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, course.id)
                            End Select
                        End While
                    ElseIf xmlNodeReader.LocalName.StartsWith("waypoint") Then
                        waypoint = New clsWaypoint
                        If Not course Is Nothing Then
                            course.addWaypoint(waypoint)
                        End If
                        While xmlNodeReader.MoveToNextAttribute
                            Select Case xmlNodeReader.LocalName
                                Case "pos"
                                    stringA = xmlNodeReader.Value.Split(" "c)
                                    Double.TryParse(stringA(0), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, waypoint.Pos_X)
                                    Double.TryParse(stringA(1), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, waypoint.Pos_Y)
                                Case "angle"
                                    Double.TryParse(xmlNodeReader.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, waypoint.Angle)
                                Case "rev"
                                    If xmlNodeReader.Value = "1" Then
                                        waypoint.Reverse = True
                                    Else
                                        waypoint.Reverse = False
                                    End If
                                Case "wait"
                                    If xmlNodeReader.Value = "1" Then
                                        waypoint.Wait = True
                                    Else
                                        waypoint.Wait = False
                                    End If
                                Case "crossing"
                                    If xmlNodeReader.Value = "1" Then
                                        waypoint.Cross = True
                                    Else
                                        waypoint.Cross = False
                                    End If
                                Case "speed"
                                    Double.TryParse(xmlNodeReader.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, waypoint.Speed)
                            End Select
                        End While
                    End If
            End Select
        Loop
        _courses.Sort(AddressOf SortCourses)
        Me.RecalcCoursesID()

    End Sub
    ''' <summary>
    ''' Draw courses
    ''' </summary>
    ''' <param name="graphics"></param>
    ''' <param name="zoomLvl"></param>
    ''' <remarks></remarks>
    Public Sub draw(ByRef graphics As Graphics, ByVal zoomLvl As Integer)
        Dim course As clsCourse
        For Each course In _courses
            If course.Hidden = False Then
                course.draw(graphics, zoomLvl)
            End If
        Next
    End Sub
    ''' <summary>
    ''' Select course at point
    ''' </summary>
    ''' <param name="point"></param>
    ''' <remarks></remarks>
    Public Sub selectWP(ByVal point As PointF)
        Dim selected As Boolean
        Me._seledtedCrs = -1
        For Each crs As clsCourse In _courses
            If crs.Hidden = False Then
                selected = crs.selectWP(point)
                If selected = True Then
                    Me._seledtedCrs = _courses.IndexOf(crs)
                    Exit For
                End If
            End If
        Next
    End Sub
    ''' <summary>
    ''' Select course by ID
    ''' </summary>
    ''' <param name="id"></param>
    ''' <remarks></remarks>
    Public Sub selectWP(ByVal id As Integer)
        If id > 0 And id <= _courses.Count Then
            _courses(id - 1).selectWP(1)
        End If
    End Sub
    ''' <summary>
    ''' Sort courses by ID
    ''' </summary>
    ''' <param name="X"></param>
    ''' <param name="Y"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function SortCourses(ByVal X As clsCourse, ByVal Y As clsCourse) As Integer
        Return X.id.CompareTo(Y.id)
    End Function
    ''' <summary>
    ''' Set selected waypoint
    ''' </summary>
    ''' <param name="wp"></param>
    ''' <remarks></remarks>
    Private Sub selectedChangeHandler(ByRef wp As clsWaypoint)
        Me._selectedWP = wp
    End Sub
    ''' <summary>
    ''' Delete selected waypoint
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub deleteSelectedWP()
        If Me._seledtedCrs >= 0 Then
            Me._courses(Me._seledtedCrs).deleteSelectedWP()
        End If
    End Sub
    ''' <summary>
    ''' Delete selected course
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub deleteSelectedCrs()
        If Me._seledtedCrs >= 0 Then
            _courses.RemoveAt(_seledtedCrs)
            Me.RecalcCoursesID()
            clsWaypoint.forceUnselect()
            clsCourse.forceUnselect()
        End If
    End Sub
    ''' <summary>
    ''' Move course UP in list
    ''' </summary>
    ''' <param name="id"></param>
    ''' <remarks></remarks>
    Public Sub moveCourseUp(ByVal id As Integer)
        If id > 1 Then
            Dim selCrs As clsCourse
            selCrs = _courses(id - 1)
            _courses.Remove(selCrs)
            _courses.Insert(id - 2, selCrs)
            Me.RecalcCoursesID()
        End If
    End Sub
    ''' <summary>
    ''' Move course DOWN in list
    ''' </summary>
    ''' <param name="id"></param>
    ''' <remarks></remarks>
    Public Sub moveCourseDown(ByVal id As Integer)
        If id < _courses.Count Then
            Dim selCrs As clsCourse
            selCrs = _courses(id - 1)
            _courses.Remove(selCrs)
            _courses.Insert(id, selCrs)
            Me.RecalcCoursesID()
        End If
    End Sub
    ''' <summary>
    ''' Recalculate IDs of courses in list based on their position in list
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub RecalcCoursesID()
        For idx = 1 To _courses.Count
            _courses(idx - 1).id = idx
        Next
    End Sub
    ''' <summary>
    ''' Insert waypoint before selected WP in selected course
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub insertBeforeWP()
        If Me._seledtedCrs >= 0 Then
            Me._courses(Me._seledtedCrs).insertBeforeWP()
        End If
    End Sub
    ''' <summary>
    ''' Append waypoint to selected course
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub appendWP()
        If Me._seledtedCrs >= 0 Then
            Me._courses(Me._seledtedCrs).appendWP()
        End If
    End Sub
    ''' <summary>
    ''' Destructor
    ''' </summary>
    ''' <remarks></remarks>
    Protected Overrides Sub Finalize()
        RemoveHandler clsWaypoint.SelectionChanged, AddressOf Me.selectedChangeHandler
        MyBase.Finalize()
    End Sub
    ''' <summary>
    ''' Save courses to XML file
    ''' </summary>
    ''' <param name="fName">filename incl. full path</param>
    ''' <remarks></remarks>
    Public Sub SaveXML(ByVal fName As String)
        Dim doc As XDocument
        Dim courses As XElement
        Dim root As New XElement("XML")
        doc = New XDocument(New XDeclaration("1.0", "utf-8", "no"))

        courses = New XElement("courses")
        For Each crs In _courses
            courses.Add(crs.getXML)
        Next

        root.Add(courses)
        doc.Document.Add(root)

        If System.IO.File.Exists(fName) = True Then
            Dim backupName As String = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fName), System.IO.Path.GetFileNameWithoutExtension(fName) & "_backup" & System.IO.Path.GetExtension(fName))
            If System.IO.File.Exists(backupName) Then System.IO.File.Delete(backupName)
            System.IO.File.Move(fName, backupName)
        End If
        doc.Save(fName)

    End Sub
    ''' <summary>
    ''' Create new course
    ''' </summary>
    ''' <param name="point">Starting point</param>
    ''' <remarks></remarks>
    Public Sub addCourse(ByVal point As PointF)
        Dim crs As New clsCourse("course " & _courses.Count + 1, _courses.Count + 1)
        crs.initWPforNewCourse(point)
        _courses.Add(crs)
    End Sub
    ''' <summary>
    ''' Calculate angles(directions) for selected waypoint
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function calculateAngleSelWP() As Double
        If Me._seledtedCrs < 0 Then Return 0
        Return _courses(_seledtedCrs).calculateAngleSelWP
    End Function
    ''' <summary>
    ''' Calculate angles(directions) for all waypoints in selected course
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub calculateAngleAllWP()
        If Me._seledtedCrs < 0 Then Exit Sub
        _courses(_seledtedCrs).calculateAngleAllWP()
    End Sub
    ''' <summary>
    ''' Fill waypoints between selected and previous WP in course
    ''' </summary>
    ''' <param name="range"></param>
    ''' <remarks></remarks>
    Public Sub fillBeforeSelected(ByVal range As Integer)
        If Me._seledtedCrs < 0 Then Exit Sub
        _courses(_seledtedCrs).fillBeforeSelected(range)
    End Sub
End Class
