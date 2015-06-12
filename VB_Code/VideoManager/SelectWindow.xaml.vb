' Copyright © Decebal Mihailescu 2015

' All rights reserved.
' This code is released under The Code Project Open License (CPOL) 1.02
' The full licensing terms are available at http://www.codeproject.com/info/cpol10.aspx
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
' KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
' IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
' PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
' REMAINS UNCHANGED.

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Shapes
Imports System.Windows.Interop
Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Windows.Threading
Imports WpfCustomFileDialog

Namespace VideoManager

	''' <summary>
	''' Interaction logic for AuxWindow.xaml
	''' </summary>
	Partial Public Class SelectWindow
		Inherits WpfCustomFileDialog.WindowAddOnBase
		Private Sub mediaPlay(ByVal sender As Object, ByVal e As EventArgs)
			If _isPaused Then
				_Media.SpeedRatio = _speed
				_Media.Play()
				_isPaused = False
				_playButton.Content = "Pause"
				Me._comboSpeed.IsEnabled = True
				If Not _timer.IsEnabled Then
					_timer.Start()
				End If
			Else
				_Media.Pause()
				_isPaused = True
				_playButton.Content = "Play"
			End If

		End Sub
		Private _isPaused As Boolean


		Private _volume As Double
		Private Sub mediaMute(ByVal sender As Object, ByVal e As EventArgs)
			If _Media.Volume > 0 Then
				_volume = _Media.Volume
				_Media.Volume = 0
				_muteButt.Content = "Listen"
			Else
				_Media.Volume = _volume
				_muteButt.Content = "Mute"
			End If
		End Sub
		Friend _hFileDialogWrapperHandle As IntPtr = IntPtr.Zero

		Public Sub New()
			InitializeComponent()
		End Sub


		Protected Overrides Sub OnInitialized(ByVal e As EventArgs)
			MyBase.OnInitialized(e)
			TimeSlider.Value = 0
			TimeSlider.Maximum = 100
			TimeSlider.Minimum = 0
			TimeSlider.TickFrequency = 1
			TimeSlider.Delay = 500
			TimeSlider.IsEnabled = False
		End Sub




		Private Sub Window_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
            AddHandler ParentDialog.EventFileNameChanged, AddressOf ParentDlg_EventFileNameChanged
            AddHandler ParentDialog.EventFolderNameChanged, AddressOf ParentDlg_EventFolderNameChanged
            AddHandler ParentDialog.EventFilterChanged, AddressOf ParentDlg_EventFilterChanged
			_comboSpeed.IsEnabled = False
		End Sub



		Private Sub ParentDlg_EventFilterChanged(ByVal sender As IFileDlgExt, ByVal index As Integer)

		End Sub

		Private Sub ParentDlg_EventFolderNameChanged(ByVal sender As IFileDlgExt, ByVal filePath As String)
			_Media.Stop()
			_Media.Source = Nothing
			sender.FileDlgEnableOkBtn = False
			_fsize.Content = String.Empty
			_size.Content = String.Empty
			_duration.Content = String.Empty
			TimeSlider.IsEnabled = False
			_playButton.IsEnabled = False
			_muteButt.IsEnabled = False
		End Sub
		Private _filePath As String
		Private Sub ParentDlg_EventFileNameChanged(ByVal sender As IFileDlgExt, ByVal filePath As String)
			_Media.Stop()
			_Media.SpeedRatio = 1
			_comboSpeed.SelectedIndex = 1
			_comboSpeed.IsEnabled = True
			If Not String.IsNullOrEmpty(System.IO.Path.GetFileName(filePath)) Then
				sender.FileDlgEnableOkBtn = True
				Using file As System.IO.FileStream = System.IO.File.OpenRead(filePath)
					_fsize.Content = String.Format("{0:#,#} bytes", file.Length)
					If file.Length > 0 Then
						_filePath = filePath
					End If
				End Using
				_Media.Source = New Uri(_filePath)
				_playButton.IsEnabled = False
				_muteButt.IsEnabled = False
				_Media.Volume = 50
				_Media.SpeedRatio = _speed
				_Media.Play()
				_playButton.Content = "Pause"
				_isPaused = False
				_muteButt.IsEnabled = False
			Else
				sender.FileDlgEnableOkBtn = False
				_fsize.Content = String.Empty
				_size.Content = String.Empty
				_duration.Content = String.Empty
				_Media.Source = Nothing
				_playButton.Content = "Play"
				_playButton.IsEnabled = False
				_muteButt.IsEnabled = False
			End If
			TimeSlider.IsEnabled = False
			_muteButt.Content = "Mute"
			_size.Content = String.Empty

			Me._duration.Content = String.Empty

		End Sub

		Private Sub _Media_MediaFailed(ByVal sender As Object, ByVal e As ExceptionRoutedEventArgs)
			Dim ex As COMException = TryCast(e.ErrorException, COMException)
			If ex IsNot Nothing AndAlso CUInt(ex.ErrorCode) = &H8898050cL OrElse CUInt(ex.ErrorCode) =&Hc00d11b1L Then


			ElseIf TypeOf e.ErrorException Is System.IO.FileNotFoundException Then
				TimeSlider.IsEnabled = False
				Me._playButton.IsEnabled = False
				Me._muteButt.IsEnabled = False
			End If
			_comboSpeed.IsEnabled = False
			_comboSpeed.SelectedIndex = 1
			_Media.SpeedRatio = 1
		End Sub

		Private Sub _Media_MediaOpened(ByVal sender As Object, ByVal e As RoutedEventArgs)
			Dim dur As Duration = _Media.NaturalDuration
			If dur.HasTimeSpan Then
				_duration.Content = String.Format("{0}:{1}:{2}",dur.TimeSpan.Hours,dur.TimeSpan.Minutes,dur.TimeSpan.Seconds)
				Dim height As Integer = _Media.NaturalVideoHeight
				Dim width As Integer = _Media.NaturalVideoWidth
				_size.Content = String.Format("{0} X {1}", width, height)
				If Not _timer.IsEnabled Then
					_timer.Start()
				End If
				TimeSlider.IsEnabled = True
				_comboSpeed.IsEnabled = True

				Me._playButton.IsEnabled = True
				Me._muteButt.IsEnabled = True
			Else
				TimeSlider.IsEnabled = False
				_size.Content = String.Empty
				_duration.Content = String.Empty
				Me._playButton.IsEnabled = False
				Me._muteButt.IsEnabled = False
				_comboSpeed.IsEnabled = False
			End If
			_comboSpeed.SelectedIndex = 1
		End Sub

		Private Sub _Media_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
			' Create a Timer with a Normal Priority
			_timer = New DispatcherTimer(DispatcherPriority.Normal, Me.Dispatcher)


			' Set the Interval to 1 second
			_timer.Interval = TimeSpan.FromMilliseconds(1000)

			' Set the callback to just show the time ticking away
			' NOTE: We are using a control so this has to run on 
			' the UI thread
			AddHandler _timer.Tick, Function(s, a) AnonymousMethod1(s, a)
		End Sub
		
		Private Function AnonymousMethod1(ByVal s As Object, ByVal a As EventArgs) As Object
			If Not _mouseDown Then
				Try
					Dim dur As Duration = _Media.NaturalDuration
					If dur.HasTimeSpan Then
						Dim val As Long = 100 * _Media.Position.Ticks / _Media.NaturalDuration.TimeSpan.Ticks
						TimeSlider.Value = val
					End If
				Catch e1 As Exception
				End Try
			End If
			Return Nothing
		End Function



		Private Sub _Media_Unloaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
			_Media.Source = Nothing
			_timer.Stop()
			_timer = Nothing
		End Sub

		Private Sub _Media_MediaEnded(ByVal sender As Object, ByVal e As RoutedEventArgs)
			_Media.Stop()
			_Media.Position = New TimeSpan(0L)
			TimeSlider.Value = 0
			_timer.Stop()
			_playButton.Content = "Play"
			_isPaused = True
		End Sub

		Private _speed As Double = 1
		Private Sub comboSpeed_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)

			Try
				Dim str As String = (TryCast(sender, ComboBox)).SelectedValue.ToString()
				str = str.Remove(0, str.LastIndexOf(":"c) + 1)
				_speed = Double.Parse(str)
				If _Media.SpeedRatio <> _speed Then
					_Media.SpeedRatio = _speed
				End If
			Catch e1 As Exception


			End Try
		End Sub
		Private _timer As DispatcherTimer
		Private _sliderVal As Double
		Private Sub TimeSlider_ValueChanged(ByVal sender As Object, ByVal e As RoutedPropertyChangedEventArgs(Of Double))
			_sliderVal = e.NewValue
		End Sub



		Private Sub TimeSlider_PreviewMouseUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
			_mouseDown = False
			Try
				_Media.Position = New TimeSpan(Convert.ToInt64(_Media.NaturalDuration.TimeSpan.Ticks * _sliderVal / 100))
				If Not _isPaused Then
					_Media.Play()
				End If
			Catch e1 As Exception

			End Try
		End Sub

		Private _mouseDown As Boolean
		Private Sub TimeSlider_PreviewMouseDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
			_mouseDown = True
			If Not _isPaused Then
				_Media.Stop()
			End If
		End Sub
	End Class
End Namespace
