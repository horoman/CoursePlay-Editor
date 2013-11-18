Public Class clsWaypoint
    Public Shared Event SelectionChanged(ByRef wp As clsWaypoint)
    Private Shared _isAnySelected As Boolean
    Public Shared Property mapSize As Size
    Public ReadOnly Property PositionWorld As PointF
        Get
            Dim point As New PointF
            Single.TryParse(_Pos_X, point.X)
            Single.TryParse(_Pos_Y, point.Y)
            Return point
        End Get
    End Property
    Public ReadOnly Property PositionScreen As PointF
        Get
            Dim point As New PointF
            Single.TryParse(_Pos_X, point.X)
            Single.TryParse(_Pos_Y, point.Y)
            point.X += _mapSize.Width / 2
            point.Y += _mapSize.Height / 2
            Return point
        End Get
    End Property
    Public ReadOnly Property PositionScreenDraw(ByVal zoomLvl As Integer) As PointF
        Get
            Dim point As New PointF
            Single.TryParse(_Pos_X, point.X)
            Single.TryParse(_Pos_Y, point.Y)
            point.X = (point.X + _mapSize.Width / 2) * zoomLvl / 100
            point.Y = (point.Y + _mapSize.Height / 2) * zoomLvl / 100
            Return point
        End Get
    End Property
    Public Property Pos_X As Double
    Public Property Pos_Y As Double
    Public Property Angle As Double
    Public Property Reverse As Boolean
    Public Property Wait As Boolean
    Public Property Cross As Boolean
    Public Property Speed As Double
    Public Property isSelected As Boolean
    Public ReadOnly Property ReverseTxt As String
        Get
            If _Reverse = True Then
                Return "1"
            Else
                Return "0"
            End If
        End Get
    End Property
    Public ReadOnly Property CrossTxt As String
        Get
            If _Cross = True Then
                Return "1"
            Else
                Return "0"
            End If
        End Get
    End Property
    Public ReadOnly Property WaitTxt As String
        Get
            If _Wait = True Then
                Return "1"
            Else
                Return "0"
            End If
        End Get
    End Property
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        If mapSize.IsEmpty = True Then
            mapSize = New Size(2048, 2048)
        End If
        AddHandler clsWaypoint.SelectionChanged, AddressOf Me.SelectionChangedHandler
    End Sub
    ''' <summary>
    ''' Find if this waypoint is selected (in range of point)
    ''' </summary>
    ''' <param name="point"></param>
    ''' <param name="range"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function selectWP(ByVal point As PointF, ByVal range As Double) As Boolean
        Dim xDist As Double
        Dim yDist As Double
        Dim dist As Double
        If clsWaypoint._isAnySelected = True Then
            RaiseEvent SelectionChanged(Nothing)
        End If

        xDist = point.X - Me.PositionScreen.X
        yDist = point.Y - Me.PositionScreen.Y
        dist = Math.Sqrt(xDist * xDist + yDist * yDist)

        If dist <= range Then
            RaiseEvent SelectionChanged(Me)
            Return True
        Else
            Return False
        End If
    End Function
    ''' <summary>
    ''' Set new position
    ''' </summary>
    ''' <param name="point"></param>
    ''' <remarks></remarks>
    Public Sub setNewPos(ByVal point As PointF)
        Me.Pos_X = point.X - mapSize.Width / 2
        Me.Pos_Y = point.Y - mapSize.Height / 2
    End Sub
    ''' <summary>
    ''' Set selected attribute
    ''' </summary>
    ''' <param name="wp"></param>
    ''' <remarks></remarks>
    Private Sub SelectionChangedHandler(ByRef wp As clsWaypoint)
        If wp Is Nothing Then
            Me._isSelected = False
            If clsWaypoint._isAnySelected = True Then clsWaypoint._isAnySelected = False
        Else
            If clsWaypoint._isAnySelected = False Then clsWaypoint._isAnySelected = True
            If Me.Equals(wp) Then
                Me._isSelected = True
            Else
                Me._isSelected = False
            End If
        End If
    End Sub
    ''' <summary>
    ''' Create XML from waypoint
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getXML(ByVal id As Integer) As XElement
        Dim myNam As String = "waypoint" & id
        Dim el As New XElement(myNam)
        el.Add(New XAttribute("pos", Me.Pos_X.ToString(System.Globalization.CultureInfo.InvariantCulture) & " " & Me.Pos_Y.ToString(System.Globalization.CultureInfo.InvariantCulture)))
        el.Add(New XAttribute("angle", Me.Angle.ToString(System.Globalization.CultureInfo.InvariantCulture)))
        el.Add(New XAttribute("rev", Me.ReverseTxt))
        el.Add(New XAttribute("wait", Me.WaitTxt))
        el.Add(New XAttribute("crossing", Me.CrossTxt))
        el.Add(New XAttribute("speed", Me.Speed.ToString(System.Globalization.CultureInfo.InvariantCulture)))
        Return el
    End Function
    ''' <summary>
    ''' Destructor
    ''' </summary>
    ''' <remarks></remarks>
    Protected Overrides Sub Finalize()
        RemoveHandler clsWaypoint.SelectionChanged, AddressOf Me.SelectionChangedHandler
        MyBase.Finalize()
    End Sub
    ''' <summary>
    ''' Clone waypoint (create copy)
    ''' </summary>
    ''' <param name="dX">offset X</param>
    ''' <param name="dY">offset Y</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Clone(Optional ByVal dX As Single = 10.0, Optional ByVal dY As Single = 10.0) As clsWaypoint
        Dim wp As New clsWaypoint
        wp.Pos_X = Me.Pos_X + dX
        wp.Pos_Y = Me.Pos_Y + dY
        'wp.Angle = Me.Angle
        wp.Speed = Me.Speed
        'wp.Wait = Me.Wait
        wp.Reverse = Me.Reverse
        'wp.Cross = Me.Cross
        Return wp
    End Function
    ''' <summary>
    ''' Force select this waypoint
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub forceSelect()
        RaiseEvent SelectionChanged(Me)
    End Sub
    ''' <summary>
    ''' Force unselect
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub forceUnselect()
        RaiseEvent SelectionChanged(Nothing)
    End Sub
End Class
