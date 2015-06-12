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
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports WpfCustomFileDialog
Imports System.IO
Imports WMEncoderLib
Imports System.Runtime.InteropServices
Imports System.Runtime.InteropServices.ComTypes
Imports System.ComponentModel
Imports System.Windows.Threading
Imports System.Threading

'mostly from http://www.codeproject.com/KB/audio-video/ConvertVideoFileFormats.aspx
Namespace VideoManager
	Friend Structure strucEncodeInfo
		Public Source As String
		Public Destination As String
		Public Profile As String
		Public DRMProfile As String
		Public Title As String
		Public Description As String
		Public Author As String
		Public Copyright As String
		Public Crop As Boolean
		Public CropLeft As Long
		Public CropTop As Long
		Public CropRight As Long
		Public CropBottom As Long
		Public Preproc As WMENC_VIDEO_OPTIMIZATION
		Public TwoPass As Boolean
	End Structure
	''' <summary>
	''' Interaction logic for Manager.xaml
	''' </summary>
	Partial Public Class Manager
		Inherits Window
		Public Sub New()

			InitializeComponent()
		End Sub


		Private _TwoPassEncoding As Boolean = False
		Private glbPassNumber As Integer

		Private _encodeInfo As strucEncodeInfo
		Private Sub btnSource_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
			Dim ofd As New WpfCustomFileDialog.OpenFileDialog(Of SelectWindow)()
			ofd.Filter = "avi files (*.avi)|*.avi|wmv files (*.wmv)|*.wmv|All files (*.*)|*.*"
			ofd.Multiselect = False
			ofd.Title = "Select Media file using a window"
			ofd.FileDlgStartLocation = AddonWindowLocation.Right
			ofd.FileDlgDefaultViewMode = NativeMethods.FolderViewMode.Tiles
			ofd.FileDlgOkCaption = "&Select"
			ofd.FileDlgEnableOkBtn = False
			ofd.SetPlaces(New Object() { "c:\", CInt(Fix(Places.MyComputer)), CInt(Fix(Places.Favorites)), CInt(Fix(Places.All_Users_MyVideo)), CInt(Fix(Places.MyVideos)) })
			Dim res? As Boolean = ofd.ShowDialog(Me)

			If res.Value = True Then
				txtSource.Text = ofd.FileName
				_encodeInfo.Source = txtSource.Text
			End If
		End Sub

		Private Sub btnTarget_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)

			Dim sfd As New WpfCustomFileDialog.SaveFileDialog(Of TargetWindow)()
			sfd.ValidateNames = True
			sfd.Title = "Save as using a Window"
			sfd.FileDlgStartLocation = AddonWindowLocation.Bottom
			sfd.Filter = "wmv files (*.wmv)|*.wmv|avi files (*.avi)|*.avi|wma files (*.wma)|*.wma|All files (*.*)|*.*"
			sfd.CheckPathExists = True
			If File.Exists(txtSource.Text) Then
				sfd.FileName = System.IO.Path.GetFileNameWithoutExtension(txtSource.Text) & "_converted"
			End If
			Dim test As New Microsoft.Win32.OpenFileDialog()
            sfd.SetPlaces(New Object() {"c:\", CInt(Fix(Places.MyComputer)), CInt(Fix(Places.Favorites)), CInt(Fix(Places.All_Users_MyVideo)), CInt(Fix(Places.MyVideos))})
			Dim res? As Boolean = sfd.ShowDialog()
			If res.Value = True Then
				_encodeInfo = (TryCast(sfd.ChildWnd, TargetWindow)).EncodeInfo
				txtDest.Text = sfd.FileName
				_encodeInfo.Destination = txtDest.Text

			End If

		End Sub
		Private Sub Window_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)

			Return

		End Sub

		Private glbintSourceDuration As Integer

		Private Function sReturnExtension(ByVal strValue As String) As String
			Try
				Return strValue.Substring(strValue.Length - 3).ToLower()
			Catch e1 As Exception
			End Try
			Return ""
		End Function
		Private Sub sRemoveSrcGrpColl()
			Try
				If _SrcGrpColl IsNot Nothing Then
					_SrcGrpColl.Remove(0)
					'_boolSrcGrpColl = false;
				End If
			Catch ex As Exception
				Dim strError As String = ex.ToString()
				sDisplayErrMessage(strError)
			End Try
		End Sub

		Private Sub sReportPercentComplete(ByVal glbEncoder As WMEncoder)
			If glbEncoder.RunState = WMENC_ENCODER_STATE.WMENC_ENCODER_RUNNING Then
				Dim Stats As IWMEncStatistics = glbEncoder.Statistics
				Dim FileStats As IWMEncFileArchiveStats = CType(Stats.FileArchiveStats, IWMEncFileArchiveStats)
				Dim intCurrentFileDuration As Integer
				Dim intPercentComplete As Integer
				intCurrentFileDuration = System.Convert.ToInt32(FileStats.FileDuration * 10)
				Try
					intPercentComplete = 100 * intCurrentFileDuration \ glbintSourceDuration
					_backgroundWorker.ReportProgress(intPercentComplete, intPercentComplete.ToString() & "% Complete")

				Catch ex As Exception
					Dim strError As String = ex.ToString()
					sDisplayErrMessage(strError)
				Finally
					FileStats = Nothing
					Stats = Nothing
				End Try
			End If

		End Sub
		Private Sub sDisplayErrMessage(ByVal strError As String)
			MessageBox.Show(Me, strError, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
		End Sub

		Private Sub sSetInitialValues()

			Try
				StatusBar.Content = "Ready"
			Catch e1 As Exception

				Return
			End Try
			Try

			Catch ex As Exception
				Dim strError As String = ex.ToString()
				sDisplayErrMessage(strError)
			End Try
		End Sub


		Private Sub _backgroundWorker_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs)


			Dim glbstrErrLocation As String
			Dim Encoder As WMEncoder = Nothing
			Dim Profile As WMEncProfile2 = Nothing
			Dim DRMAuthor As IWMDRMContentAuthor = Nothing
			Dim DRMProColl As IWMDRMProfileCollection = Nothing
			Dim DRMPro As IWMDRMProfile = Nothing
			Dim SrcGrp As IWMEncSourceGroup = Nothing
			Dim SrcAud As IWMEncAudioSource = Nothing
			Dim SrcVid As IWMEncVideoSource2 = Nothing
			_TwoPassEncoding = False


			Dim glbboolEncodingContinue As Boolean = True


			Dim time As DateTime = DateTime.Now
			Try

				Encoder = New WMEncoder()
				AddHandler Encoder.OnStateChange, AddressOf Encoder_OnStateChange
				_SrcGrpColl = Encoder.SourceGroupCollection

				Try
					DRMAuthor = Encoder.EncoderDRMContentAuthor
					DRMProColl = DRMAuthor.DRMProfileCollection
					DRMPro = Nothing

					Dim vKeyID As Object = Nothing
					If _encodeInfo.DRMProfile <> "None" Then
						Dim intDRMProCount As Integer = 0
						For intDRMProCount = 0 To DRMProColl.Count - 1
							If DRMProColl.Item(intDRMProCount).Name = _encodeInfo.DRMProfile Then
								DRMPro = DRMProColl.Item(intDRMProCount)
								Exit For
							End If
						Next intDRMProCount
						DRMAuthor.SetSessionDRMProfile(DRMPro.ID, vKeyID)
					Else
						DRMAuthor.SetSessionDRMProfile(System.DBNull.Value.ToString(), vKeyID)
						DRMAuthor = Nothing
						DRMProColl = Nothing
						DRMPro = Nothing
						vKeyID = Nothing
					End If
				Catch ex As Exception
					glbstrErrLocation = "Specify DRM Profile - " & ex.Message.ToString()
					Throw New ApplicationException(glbstrErrLocation, ex)
				End Try
				Profile = New WMEncProfile2()
				Dim intProContentType As Int32 = 0
				Dim intProVBRModeAudio As Integer = 0
				Dim intProVBRModeVideo As Integer = 0
				Try
					Profile.LoadFromFile(_encodeInfo.Profile)
					intProContentType = Profile.ContentType
					intProVBRModeAudio = CInt(Fix(Profile.VBRMode(WMENC_SOURCE_TYPE.WMENC_AUDIO, 0)))
					intProVBRModeVideo = CInt(Fix(Profile.VBRMode(WMENC_SOURCE_TYPE.WMENC_VIDEO, 0)))
				Catch ex As Exception
					glbstrErrLocation = "Load Profile - " & ex.Message.ToString()
					Throw New ApplicationException(glbstrErrLocation, ex)
				End Try

				Try
					SrcGrp = _SrcGrpColl.Add("BatchEncode")
				Catch ex As Exception
					glbstrErrLocation = "Source Group - " & ex.Message.ToString()
					Throw New ApplicationException(glbstrErrLocation, ex)
				End Try
				Dim strDestExtension As String = System.IO.Path.GetExtension(_encodeInfo.Destination)

				Try
					If intProContentType = 1 Then
						SrcAud = CType(SrcGrp.AddSource(WMENC_SOURCE_TYPE.WMENC_AUDIO), WMEncoderLib.IWMEncAudioSource)
						SrcAud.SetInput(_encodeInfo.Source, "", "")
					ElseIf intProContentType = 16 Then
						SrcVid = CType(SrcGrp.AddSource(WMENC_SOURCE_TYPE.WMENC_VIDEO), WMEncoderLib.IWMEncVideoSource2)
						SrcVid.SetInput(_encodeInfo.Source, "", "")
					ElseIf intProContentType = 17 Then
						SrcAud = CType(SrcGrp.AddSource(WMENC_SOURCE_TYPE.WMENC_AUDIO), WMEncoderLib.IWMEncAudioSource)
						SrcAud.SetInput(_encodeInfo.Source, "", "")
						SrcVid = CType(SrcGrp.AddSource(WMENC_SOURCE_TYPE.WMENC_VIDEO), WMEncoderLib.IWMEncVideoSource2)
						SrcVid.SetInput(_encodeInfo.Source, "", "")
					Else
						_backgroundWorker.ReportProgress(0, "BatchEncode does not support this type of profile")
					End If
				Catch ex As Exception
					glbstrErrLocation = "Video / Audio Source - " & ex.Message.ToString()
					Throw New ApplicationException(glbstrErrLocation, ex)

				End Try
				Try
                    SrcGrp.Profile = Profile
				Catch ex As Exception
					glbstrErrLocation = "Encoding Profile - " & ex.Message.ToString()

					Throw New ApplicationException(glbstrErrLocation, ex)
				End Try
				Dim Descr As IWMEncDisplayInfo = Encoder.DisplayInfo
				Try
					If _encodeInfo.Title <> "" Then
						Descr.Title = _encodeInfo.Title
					End If
					If _encodeInfo.Description <> "" Then
						Descr.Description = _encodeInfo.Description
					End If
					If _encodeInfo.Author <> "" Then
						Descr.Author = _encodeInfo.Author
					End If
					If _encodeInfo.Copyright <> "" Then
						Descr.Copyright = _encodeInfo.Copyright
					End If
				Catch ex As Exception
					glbstrErrLocation = "Display Information - " & ex.Message.ToString()
					Throw New ApplicationException(glbstrErrLocation, ex)
				End Try
				Try
					If intProContentType = 16 OrElse intProContentType = 17 Then
						If _encodeInfo.Crop = True Then
							SrcVid.CroppingBottomMargin = CInt(Fix(_encodeInfo.CropBottom))
							SrcVid.CroppingTopMargin = CInt(Fix(_encodeInfo.CropTop))
							SrcVid.CroppingLeftMargin = CInt(Fix(_encodeInfo.CropLeft))
							SrcVid.CroppingRightMargin = CInt(Fix(_encodeInfo.CropRight))
						End If
					End If
				Catch ex As Exception
					glbstrErrLocation = "Cropping - " & ex.Message.ToString()
					Throw New ApplicationException(glbstrErrLocation, ex)
				End Try
				Try
					If intProContentType = 16 OrElse intProContentType = 17 Then
						SrcVid.Optimization = _encodeInfo.Preproc
					End If
				Catch ex As Exception
					glbstrErrLocation = "Video Optimization - " & ex.Message.ToString()
					Throw New ApplicationException(glbstrErrLocation, ex)
				End Try

				Try
					If intProContentType = 1 Then
						If _encodeInfo.TwoPass = False Then
							SrcAud.PreProcessPass = 0
							_TwoPassEncoding = False
						Else
							SrcAud.PreProcessPass = 1
							_TwoPassEncoding = True
							glbPassNumber = 1
						End If
					ElseIf intProContentType = 16 Then
						If _encodeInfo.TwoPass = False Then
							SrcVid.PreProcessPass = 0
							_TwoPassEncoding = False
						Else
							SrcVid.PreProcessPass = 1
							_TwoPassEncoding = True
							glbPassNumber = 1
						End If
					ElseIf intProContentType = 17 Then
						If _encodeInfo.TwoPass = False Then
							SrcAud.PreProcessPass = 0
							SrcVid.PreProcessPass = 0
							_TwoPassEncoding = False
						Else
							Select Case intProVBRModeAudio
								Case 1
									SrcAud.PreProcessPass = 1
								Case 2
									SrcAud.PreProcessPass = 1
								Case 3
									SrcAud.PreProcessPass = 0
								Case 4
									SrcAud.PreProcessPass = 1
							End Select
							Select Case intProVBRModeVideo
								Case 1
									SrcVid.PreProcessPass = 1
								Case 2
									SrcVid.PreProcessPass = 1
								Case 3
									SrcVid.PreProcessPass = 0
								Case 4
									SrcVid.PreProcessPass = 1
							End Select
							_TwoPassEncoding = True
							glbPassNumber = 1

						End If

					Else
						_backgroundWorker.ReportProgress(0, "BatchEncode does not support this type of profile")
					End If
				Catch ex As Exception
					glbstrErrLocation = "Passes - " & ex.Message.ToString()
					Throw New ApplicationException(glbstrErrLocation, ex)
				End Try
				Dim File As IWMEncFile2 = CType(Encoder.File, IWMEncFile2)
				Try
					File.LocalFileName = _encodeInfo.Destination
				Catch ex As Exception
					glbstrErrLocation = "Output File - " & ex.Message.ToString()
					Throw New ApplicationException(glbstrErrLocation, ex)
				End Try
				Dim intDurationAudio As Integer = 0
				Dim intDurationVideo As Integer = 0
				Dim intDurationFinal As Integer
				Try
					_backgroundWorker.ReportProgress(0, "Preparing to encode")
					Encoder.PrepareToEncode(True)

				Catch ex As Exception
					glbstrErrLocation = "Encoder Prepare - " & ex.Message.ToString()


					Throw New ApplicationException(glbstrErrLocation, ex)

				End Try
				Try
					If SrcAud IsNot Nothing Then
						intDurationAudio = System.Convert.ToInt32(SrcAud.Duration \ 1000)
					End If
				Catch e1 As Exception
				End Try
				Try
					If SrcVid IsNot Nothing Then
						intDurationVideo = System.Convert.ToInt32(SrcVid.Duration \ 1000)
					End If
				Catch e2 As Exception
				End Try
				If intDurationAudio = 0 Then
					intDurationFinal = intDurationVideo
				ElseIf intDurationVideo = 0 Then
					intDurationFinal = intDurationAudio
				Else
					If intDurationVideo >= intDurationAudio Then
						intDurationFinal = intDurationVideo
					Else
						intDurationFinal = intDurationAudio
					End If
				End If
				glbintSourceDuration = intDurationFinal
				Try
					If glbboolEncodingContinue = True Then
						Encoder.Start()
						Do
							If _backgroundWorker.CancellationPending Then
								Encoder.Stop()
								e.Cancel = True
								_ev.Set()
							End If

							sReportPercentComplete(Encoder)
						Loop While Not _ev.WaitOne(1000)

					End If
				Catch ex As Exception
					glbstrErrLocation = "Encoder Start - " & ex.Message.ToString()
					Throw New ApplicationException(glbstrErrLocation, ex)
				End Try
				If e.Cancel Then
					Return
				Else
					_backgroundWorker.ReportProgress(0, "Encoding Complete")
					Return
				End If
			Finally

				If _SrcGrpColl IsNot Nothing Then
					Try
						Encoder.Stop()
						_SrcGrpColl.Remove(0)

					Catch

					End Try
					Marshal.ReleaseComObject(_SrcGrpColl)
					_SrcGrpColl = Nothing
				End If
				If Profile IsNot Nothing Then
					Marshal.ReleaseComObject(Profile)
					Profile = Nothing
				End If
				If Encoder IsNot Nothing Then
					RemoveHandler Encoder.OnStateChange, AddressOf Encoder_OnStateChange
					Marshal.ReleaseComObject(Encoder)
					Encoder = Nothing
				End If
				e.Result = DateTime.Now.Subtract(time)
			End Try
		End Sub
		' Completed Method
		Private Sub _backgroundWorker_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs)
			If e.Cancelled Then
				StatusBar.Content = "Cancelled"
			ElseIf e.Error IsNot Nothing Then
				StatusBar.Content = "Error: " & e.Error.Message
			Else
				StatusBar.Content = String.Format("Completed in {0:#,#.##} seconds", (CType(e.Result, TimeSpan)).TotalSeconds)
			End If
			btnConvert.IsEnabled = txtDest.Text.Length > 0 AndAlso txtSource.Text.Length > 0
			btnConvertCtrl.IsEnabled = txtDestCtrl.Text.Length > 0 AndAlso txtSourceCtrl.Text.Length > 0
			btnConvert.Content = "Convert"
			btnConvertCtrl.Content = btnConvert.Content

		End Sub
		Private Sub _backgroundWorker_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs)
			Try

				StatusBar.Content = If(_backgroundWorker.CancellationPending, "Cancelling...", TryCast(e.UserState, String))

			Catch e1 As Exception

			End Try
		End Sub
		Private _backgroundWorker As New BackgroundWorker()
		Private _SrcGrpColl As IWMEncSourceGroupCollection = Nothing

		Private Sub btnConvert_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)


			_encodeInfo.Destination = txtDest.Text
			_encodeInfo.Source = txtSource.Text


			If _backgroundWorker.IsBusy Then
				_backgroundWorker.CancelAsync()
				Me.btnConvert.IsEnabled = False
				StatusBar.Content = "Cancelling..."

			Else

				If Not _backgroundWorker.CancellationPending Then
					StatusBar.Content = "Ready"
					btnConvert.Content = "Cancel"
					_backgroundWorker.RunWorkerAsync(Me._encodeInfo)

				End If

			End If
		End Sub

		Protected Overrides Sub OnInitialized(ByVal e As EventArgs)
			MyBase.OnInitialized(e)
			AddHandler _backgroundWorker.DoWork, AddressOf _backgroundWorker_DoWork
			AddHandler _backgroundWorker.RunWorkerCompleted, AddressOf _backgroundWorker_RunWorkerCompleted
			AddHandler _backgroundWorker.ProgressChanged, AddressOf _backgroundWorker_ProgressChanged
			_backgroundWorker.WorkerSupportsCancellation = True
			_backgroundWorker.WorkerReportsProgress = _backgroundWorker.WorkerSupportsCancellation
		End Sub

		Private _ev As New ManualResetEvent(False)
		Private Sub Encoder_OnStateChange(ByVal enumState As WMEncoderLib.WMENC_ENCODER_STATE)
			Dim strRunState As String = ""
			Select Case enumState
				Case WMENC_ENCODER_STATE.WMENC_ENCODER_STARTING

					strRunState = "Encoder Starting"
					_ev.Reset()

				Case WMENC_ENCODER_STATE.WMENC_ENCODER_RUNNING
					If _TwoPassEncoding = True Then
						If glbPassNumber = 1 Then
							strRunState = "Encoder Running Pass 1 of 2 (No Preview)"
						Else
							strRunState = "Encoder Running Pass 2 of 2"
						End If
					Else
						strRunState = "Encoder Running Pass 1 of 1"
					End If
				Case WMENC_ENCODER_STATE.WMENC_ENCODER_END_PREPROCESS
					strRunState = "Encoder End Preprocess"
					glbPassNumber = 2
				Case WMENC_ENCODER_STATE.WMENC_ENCODER_PAUSING
					strRunState = "Encoder Pausing"
				Case WMENC_ENCODER_STATE.WMENC_ENCODER_PAUSED
					strRunState = "Encoder Paused"
				Case WMENC_ENCODER_STATE.WMENC_ENCODER_STOPPING
					strRunState = "Encoder Stopping"
				Case WMENC_ENCODER_STATE.WMENC_ENCODER_STOPPED

					strRunState = "Encoder Stopped"

					_ev.Set()
				Case Else

			End Select
			_backgroundWorker.ReportProgress(0, strRunState)
		End Sub

		Private Sub TextChanged(ByVal sender As Object, ByVal e As TextChangedEventArgs)
			Me.btnConvert.IsEnabled = txtDest.Text.Length > 0 AndAlso txtSource.Text.Length > 0
		End Sub
		Private Sub btnControl_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
			Dim ofd As New WpfCustomFileDialog.OpenFileDialog(Of SelectControl)()
			ofd.Filter = "avi files (*.avi)|*.avi|wmv files (*.wmv)|*.wmv|All files (*.*)|*.*"
			ofd.Multiselect = False
			ofd.Title = "Select Media file using a control"
			ofd.FileDlgStartLocation = AddonWindowLocation.Right
			ofd.FileDlgDefaultViewMode = NativeMethods.FolderViewMode.Tiles
			ofd.FileDlgOkCaption = "&Select"
			ofd.FileDlgEnableOkBtn = False
			ofd.SetPlaces(New Object() { "c:\", CInt(Fix(Places.MyComputer)), CInt(Fix(Places.Favorites)), CInt(Fix(Places.All_Users_MyVideo)), CInt(Fix(Places.MyVideos)) })
			Dim res? As Boolean = ofd.ShowDialog(Me)

			If res.Value = True Then
				txtSourceCtrl.Text = ofd.FileName
				_encodeInfo.Source = txtSourceCtrl.Text
			End If
		End Sub
		Private Sub btnTargetControl_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
			Dim sfd As New WpfCustomFileDialog.SaveFileDialog(Of TargetControl)()
			sfd.ValidateNames = True
			sfd.FileDlgStartLocation = AddonWindowLocation.Bottom
			sfd.Title = "Save as using a Control"
			sfd.Filter = "wmv files (*.wmv)|*.wmv|avi files (*.avi)|*.avi|wma files (*.wma)|*.wma|All files (*.*)|*.*"
			sfd.CheckPathExists = True
			If File.Exists(txtSourceCtrl.Text) Then
				sfd.FileName = System.IO.Path.GetFileNameWithoutExtension(Me.txtSourceCtrl.Text) & "_converted"
			End If
			sfd.SetPlaces(New Object() { "c:\", CInt(Fix(Places.MyComputer)), CInt(Fix(Places.Favorites)), CInt(Fix(Places.All_Users_MyVideo)), CInt(Fix(Places.MyVideos)) })
			Dim res? As Boolean = sfd.ShowDialog()
			If res.Value = True Then
				_encodeInfo = (TryCast(sfd.ChildWnd, TargetControl)).EncodeInfo
				Me.txtDestCtrl.Text = sfd.FileName
				_encodeInfo.Destination = Me.txtDestCtrl.Text

			End If
		End Sub

		Private Sub Ctrl_TextChanged(ByVal sender As Object, ByVal e As TextChangedEventArgs)
			Me.btnConvertCtrl.IsEnabled = Me.txtDestCtrl.Text.Length > 0 AndAlso txtSourceCtrl.Text.Length > 0
		End Sub


		Private Sub btnConvertCtrl_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)


			_encodeInfo.Destination = Me.txtDestCtrl.Text
			_encodeInfo.Source = Me.txtSourceCtrl.Text


			If _backgroundWorker.IsBusy Then
				_backgroundWorker.CancelAsync()
				Me.btnConvertCtrl.IsEnabled = False
				StatusBar.Content = "Cancelling..."

			Else

				If Not _backgroundWorker.CancellationPending Then
					StatusBar.Content = "Ready"
					btnConvertCtrl.Content = "Cancel"
					_backgroundWorker.RunWorkerAsync(Me._encodeInfo)

				End If

			End If
		End Sub

	End Class
End Namespace
