Public Class clsCourse
    Public Shared Event SelectionChanged(ByRef course As clsCourse)
    Public Shared Property CircleDiameter As Integer
    Private Shared _isAnySelected As Boolean
    Private _waypoints As List(Of clsWaypoint)
    Private _selectedWP As Integer
    Public Property Name As String
    Public Property id As Integer
    Public ReadOnly Property WPCount As Integer
        Get
            Return _waypoints.Count
        End Get
    End Property
    Public Property isSelected As Boolean
    Public Property Hidden As Boolean


    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        _waypoints = New List(Of clsWaypoint)
        AddHandler clsWaypoint.SelectionChanged, AddressOf Me.selectionChangedHandler
        AddHandler clsCourse.SelectionChanged, AddressOf Me.selectedCourseChanged
    End Sub
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="Name">Course name</param>
    ''' <param name="id">Course ID</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal Name As String, ByVal id As Integer)
        Me.New()
        Me.Name = Name
        Me.id = id
    End Sub
    ''' <summary>
    ''' Append waypoint to course
    ''' </summary>
    ''' <param name="waypoint"></param>
    ''' <remarks></remarks>
    Public Sub addWaypoint(ByVal waypoint As clsWaypoint)
        _waypoints.Add(waypoint)
    End Sub
    ''' <summary>
    ''' Insert waypoint before
    ''' </summary>
    ''' <param name="waypoint"></param>
    ''' <param name="before"></param>
    ''' <remarks></remarks>
    Public Sub insertWaypoint(ByVal waypoint As clsWaypoint, ByVal before As Integer)
        If before < 1 Then Exit Sub
        If _waypoints.Count > before Then
            _waypoints.Insert(before, waypoint)
        ElseIf _waypoints.Count = before Then
            _waypoints.Add(waypoint)
        End If
    End Sub
    ''' <summary>
    ''' Fill waypoints between selected and previous waypoint
    ''' </summary>
    ''' <param name="range"></param>
    ''' <remarks></remarks>
    Public Sub fillBeforeSelected(ByVal range As Integer)
        If _selectedWP < 1 Then Exit Sub

        Dim dXtotal As Double
        Dim dYtotal As Double
        Dim lenTotal As Double
        Dim steps As Integer
        Dim wp As clsWaypoint

        dXtotal = _waypoints(_selectedWP).Pos_X - _waypoints(_selectedWP - 1).Pos_X
        dYtotal = _waypoints(_selectedWP).Pos_Y - _waypoints(_selectedWP - 1).Pos_Y
        lenTotal = Math.Sqrt((dXtotal * dXtotal) + (dYtotal * dYtotal))
        steps = lenTotal \ range

        For i As Integer = 1 To steps
            wp = _waypoints(_selectedWP - 1).Clone(dXtotal / steps * i, dYtotal / steps * i)
            _waypoints.Insert(_selectedWP - 1 + i, wp)
            wp = Nothing
        Next
    End Sub
    ''' <summary>
    ''' Draw waypoints
    ''' </summary>
    ''' <param name="graphic"></param>
    ''' <param name="zoomLvl"></param>
    ''' <remarks></remarks>
    Public Sub draw(ByRef graphic As Graphics, ByVal zoomLvl As Integer)
        Dim waypoint As clsWaypoint
        Dim dr_points() As PointF
        Dim idx As Integer
        Dim pen As Pen
        Dim dr_point As PointF

        ReDim dr_points(_waypoints.Count - 1)
        idx = 1
        For Each waypoint In _waypoints
            dr_points(idx - 1) = waypoint.PositionScreenDraw(zoomLvl)
            idx += 1
        Next
        'path
        If Me.isSelected = True Then
            pen = New Pen(Brushes.Blue, 2)
        Else
            pen = New Pen(Brushes.DarkBlue)
        End If
        graphic.DrawLines(pen, dr_points)

        'waypoints
        idx = 1
        For Each waypoint In _waypoints
            If waypoint.isSelected = True Then
                pen = New Pen(Brushes.WhiteSmoke, 2)
            ElseIf idx = 1 Then
                pen = New Pen(Brushes.LightGreen, 2)
            ElseIf idx = _waypoints.Count Then
                pen = New Pen(Brushes.Red, 2)
            ElseIf waypoint.Wait = True Then
                pen = New Pen(Brushes.Blue, 2)
            ElseIf waypoint.Cross = True Then
                pen = New Pen(Brushes.Yellow, 2)
            ElseIf waypoint.Reverse = True Then
                pen = New Pen(Brushes.Pink, 1)
            Else
                pen = New Pen(Brushes.DarkBlue)
            End If
            dr_point = waypoint.PositionScreenDraw(zoomLvl)
            graphic.DrawEllipse(pen, dr_point.X - 3, dr_point.Y - 3, 6, 6)
            idx += 1
        Next
        'guiding circle around selected node
        If Me.isSelected = True And _CircleDiameter > 0 And _selectedWP >= 0 Then
            Dim diameter As Single
            diameter = CircleDiameter * zoomLvl / 100
            dr_point = _waypoints(_selectedWP).PositionScreenDraw(zoomLvl)
            pen = New Pen(Brushes.LightGreen, 0.1)
            graphic.DrawEllipse(pen, dr_point.X - diameter / 2, dr_point.Y - diameter / 2, diameter, diameter)
            pen = New Pen(Brushes.OrangeRed, 0.1)
            graphic.DrawEllipse(pen, dr_point.X - diameter, dr_point.Y - diameter, diameter * 2, diameter * 2)
        End If
    End Sub
    ''' <summary>
    ''' Select waypoint at point
    ''' </summary>
    ''' <param name="point"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function selectWP(ByVal point As PointF) As Boolean
        Dim selected As Boolean
        If _isAnySelected = True Then
            RaiseEvent SelectionChanged(Nothing)
        End If
        For Each wp As clsWaypoint In _waypoints
            selected = wp.selectWP(point, 3)
            If selected = True Then
                If Me._isSelected = False Then
                    RaiseEvent SelectionChanged(Me)
                End If
                Exit For
            End If
        Next
        Return selected
    End Function
    ''' <summary>
    ''' Select waypoint by ID
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function selectWP(ByVal id As Integer) As Boolean
        If _isAnySelected = True Then
            RaiseEvent SelectionChanged(Nothing)
        End If
        If id > 0 And id <= _waypoints.Count Then
            _waypoints(id - 1).forceSelect()
            If Me._isSelected = False Then
                RaiseEvent SelectionChanged(Me)
            End If
        End If
        Return True
    End Function
    ''' <summary>
    ''' Set selected waypoint if selection changed
    ''' </summary>
    ''' <param name="wp"></param>
    ''' <remarks></remarks>
    Private Sub selectionChangedHandler(ByRef wp As clsWaypoint)
        If wp Is Nothing Then
            Me._isSelected = False
            Me._selectedWP = -1
        Else
            If Me._waypoints.Contains(wp) Then
                Me._selectedWP = _waypoints.IndexOf(wp)
            Else
                Me._selectedWP = -1
            End If
        End If
    End Sub
    ''' <summary>
    ''' Set isSelected attribute if selection changed
    ''' </summary>
    ''' <param name="crs"></param>
    ''' <remarks></remarks>
    Private Sub selectedCourseChanged(ByRef crs As clsCourse)
        If crs Is Nothing Then
            Me._isSelected = False
            If clsCourse._isAnySelected = True Then clsCourse._isAnySelected = False
            Exit Sub
        Else
            If clsCourse._isAnySelected = False Then clsCourse._isAnySelected = True
            If crs.Equals(Me) Then
                Me._isSelected = True
            Else
                Me._isSelected = False
            End If
        End If
    End Sub
    ''' <summary>
    ''' Delete selected waypoint
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub deleteSelectedWP()
        If Me._selectedWP >= 0 Then
            Me._waypoints.RemoveAt(Me._selectedWP)
        End If
    End Sub
    ''' <summary>
    ''' Insert waypoint before
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub insertBeforeWP()
        If Me._selectedWP >= 0 Then
            Me._waypoints.Insert(_selectedWP, Me._waypoints(Me._selectedWP).Clone)
        End If
    End Sub
    ''' <summary>
    ''' Append waypoint
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub appendWP()
        Me._waypoints.Add(Me._waypoints(Me._waypoints.Count - 1).Clone)
    End Sub
    ''' <summary>
    ''' Destructor
    ''' </summary>
    ''' <remarks></remarks>
    Protected Overrides Sub Finalize()
        RemoveHandler clsWaypoint.SelectionChanged, AddressOf Me.selectionChangedHandler
        RemoveHandler clsCourse.SelectionChanged, AddressOf Me.selectedCourseChanged
        MyBase.Finalize()
    End Sub
    ''' <summary>
    ''' Create XML of course
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getXML() As XElement
        Dim el As New XElement("course")
        Dim idx As Integer = 1
        el.Add(New XAttribute("name", Me.Name))
        el.Add(New XAttribute("id", Me.id.ToString))
        For Each wp As clsWaypoint In _waypoints
            el.Add(wp.getXML(idx))
            idx += 1
        Next
        Return el
    End Function
    ''' <summary>
    ''' Initial waypoints for new course
    ''' </summary>
    ''' <param name="Point"></param>
    ''' <remarks></remarks>
    Public Sub initWPforNewCourse(ByVal Point As PointF)
        Dim wp As New clsWaypoint
        wp.setNewPos(Point)
        wp.Wait = True
        _waypoints.Add(wp)
        wp = New clsWaypoint
        wp.setNewPos(New PointF(Point.X + 10, Point.Y + 10))
        wp.Wait = True
        _waypoints.Add(wp)
        wp.forceSelect()
        wp = Nothing
        RaiseEvent SelectionChanged(Me)
    End Sub
    ''' <summary>
    ''' Force unselect course
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub forceUnselect()
        RaiseEvent SelectionChanged(Nothing)
    End Sub
    ''' <summary>
    ''' Calculate angle(direction) for all waypoints
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub calculateAngleAllWP()
        For i As Integer = 0 To _waypoints.Count - 1
            Me.calculateAngle(i)
        Next
    End Sub
    ''' <summary>
    ''' Calculate angle(direction) for waypoint with ID
    ''' </summary>
    ''' <param name="idx"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function calculateAngle(ByVal idx As Integer) As Double
        If _waypoints.Count < 2 Then Return 0

        Dim dir As Double

        If idx = 0 Then    'first WP
            dir = getDirection(_waypoints(idx).PositionScreen, _waypoints(idx + 1).PositionScreen)
        ElseIf idx = _waypoints.Count - 1 Then  'last WP
            dir = getDirection(_waypoints(idx - 1).PositionScreen, _waypoints(idx).PositionScreen)
        Else  'between other WPs
            Dim len_p As Double
            Dim len_n As Double
            Dim dX As Double
            Dim dY As Double
            Dim p1 As PointF
            Dim p2 As PointF

            dX = Math.Abs(_waypoints(idx).PositionScreen.X - _waypoints(idx + 1).PositionScreen.X)
            dY = Math.Abs(_waypoints(idx).PositionScreen.Y - _waypoints(idx + 1).PositionScreen.Y)
            len_p = Math.Sqrt((dX * dX) + (dY * dY))
            dX = Math.Abs(_waypoints(idx).PositionScreen.X - _waypoints(idx - 1).PositionScreen.X)
            dY = Math.Abs(_waypoints(idx).PositionScreen.Y - _waypoints(idx - 1).PositionScreen.Y)
            len_n = Math.Sqrt((dX * dX) + (dY * dY))

            If len_p < len_n Then
                p2 = _waypoints(idx + 1).PositionScreen
                dX = (_waypoints(idx).PositionScreen.X - _waypoints(idx - 1).PositionScreen.X) * (len_p / len_n)
                dY = (_waypoints(idx).PositionScreen.Y - _waypoints(idx - 1).PositionScreen.Y) * (len_p / len_n)
                p1 = New PointF(_waypoints(idx).PositionScreen.X - dX, _waypoints(idx).PositionScreen.Y - dY)
            Else
                p1 = _waypoints(idx - 1).PositionScreen
                dX = (_waypoints(idx + 1).PositionScreen.X - _waypoints(idx).PositionScreen.X) * (len_n / len_p)
                dY = (_waypoints(idx + 1).PositionScreen.Y - _waypoints(idx).PositionScreen.Y) * (len_n / len_p)
                p2 = New PointF(_waypoints(idx).PositionScreen.X + dX, _waypoints(idx).PositionScreen.Y + dY)
            End If

            dir = getDirection(p1, p2)
        End If
        _waypoints(idx).Angle = dir
        Return dir

    End Function
    ''' <summary>
    ''' Calculate angle(direction) for selected waypoint
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function calculateAngleSelWP() As Double
        If _selectedWP < 0 Then Return 0
        If _waypoints.Count < 2 Then Return 0


        Return Me.calculateAngle(_selectedWP)

    End Function
    ''' <summary>
    ''' Get direction between two points
    ''' </summary>
    ''' <param name="pt1"></param>
    ''' <param name="pt2"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function getDirection(ByVal pt1 As PointF, ByVal pt2 As PointF) As Double
        Dim dX As Double
        Dim dY As Double
        Dim v1 As Double
        Dim v2 As Double

        dX = Math.Abs(pt1.X - pt2.X)
        dY = Math.Abs(pt1.Y - pt2.Y)
        If dX = 0 And dY = 0 Then
            'same point, no direction...
            Return 0
        End If
        'dX = 0 => -180(180) / 0
        If dX = 0 Then
            If pt1.Y < pt2.Y Then
                Return 0
            Else
                Return 180
            End If
        End If
        'dY = 0 => -90 / 90
        If dY = 0 Then
            If pt1.X < pt2.X Then
                Return -90
            Else
                Return 90
            End If
        End If
        'dX <>0 and dY <> 0 ...
        v1 = (Math.Atan(dY / dX)) / (2 * Math.PI) * 360
        v2 = 90 - v1

        If pt1.X > pt2.X Then  '0 - -180
            If pt1.Y < pt2.Y Then '0 - -90
                Return -v2
            Else '-90 - 180
                Return -90 - v1
            End If
        Else '0 - +180
            If pt1.Y < pt2.Y Then '0 - +90
                Return v2
            Else '+90 - +180
                Return 90 + v1
            End If
        End If
    End Function
End Class
