Module main
    ''' <summary>
    ''' Get true world coordinates (abstracting from zoom)
    ''' </summary>
    ''' <param name="X"></param>
    ''' <param name="Y"></param>
    ''' <param name="zoomLevel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getWorlCoordinates(ByVal X As Long, ByVal Y As Long, ByVal zoomLevel As Integer) As PointF
        Return New PointF(X / zoomLevel * 100, Y / zoomLevel * 100)
    End Function
    ''' <summary>
    ''' Get true world coordinates (abstracting from zoom)
    ''' </summary>
    ''' <param name="point"></param>
    ''' <param name="zoomLevel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getWorlCoordinates(ByVal point As Point, ByVal zoomLevel As Integer) As PointF
        Return New PointF(point.X / zoomLevel * 100, point.Y / zoomLevel * 100)
    End Function
    ''' <summary>
    ''' Get true world coordinates (abstracting from zoom)
    ''' </summary>
    ''' <param name="point"></param>
    ''' <param name="zoomLevel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getWorlCoordinates(ByVal point As PointF, ByVal zoomLevel As Integer) As PointF
        Return New PointF(point.X / zoomLevel * 100, point.Y / zoomLevel * 100)
    End Function
    ''' <summary>
    ''' Get draw coordinates from world coordinates (zoom)
    ''' </summary>
    ''' <param name="X"></param>
    ''' <param name="Y"></param>
    ''' <param name="zoomLevel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getDrawCoordinates(ByVal X As Long, ByVal Y As Long, ByVal zoomLevel As Integer) As Point
        Return New Point(X * zoomLevel / 100, Y * zoomLevel / 100)
    End Function
    ''' <summary>
    ''' Get draw coordinates from world coordinates (zoom)
    ''' </summary>
    ''' <param name="point"></param>
    ''' <param name="zoomLevel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getDrawCoordinates(ByVal point As Point, ByVal zoomLevel As Integer) As Point
        Return New Point(point.X * zoomLevel / 100, point.Y * zoomLevel / 100)
    End Function
    ''' <summary>
    ''' Get draw coordinates from world coordinates (zoom)
    ''' </summary>
    ''' <param name="point"></param>
    ''' <param name="zoomLevel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getDrawCoordinates(ByVal point As PointF, ByVal zoomLevel As Integer) As Point
        Return New Point(point.X * zoomLevel / 100, point.Y * zoomLevel / 100)
    End Function
End Module
