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

Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Shapes
Imports WpfCustomFileDialog
Imports WMEncoderLib
Imports System.Runtime.InteropServices

Namespace VideoManager
	''' <summary>
	''' Interaction logic for TargetControl.xaml
	''' </summary>
	Partial Public Class TargetControl
		Inherits UserControl
		Implements IWindowExt
		Public Sub New()
			InitializeComponent()
		End Sub

		Private Const optSTANDARD As WMENC_VIDEO_OPTIMIZATION = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_STANDARD
		Private Const optDEINTERLACE As WMENC_VIDEO_OPTIMIZATION = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_DEINTERLACE
		Private Const optTELECINE_AUTO As WMENC_VIDEO_OPTIMIZATION = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE Or WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE
		Private Const optTELECINE_AA_TOP As WMENC_VIDEO_OPTIMIZATION = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE Or WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_TELECINE_AA_TOP
		Private Const optTELECINE_BB_TOP As WMENC_VIDEO_OPTIMIZATION = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE Or WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_TELECINE_BB_TOP
		Private Const optTELECINE_BC_TOP As WMENC_VIDEO_OPTIMIZATION = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE Or WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_TELECINE_BC_TOP
		Private Const optTELECINE_CD_TOP As WMENC_VIDEO_OPTIMIZATION = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE Or WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_TELECINE_CD_TOP
		Private Const optTELECINE_DD_TOP As WMENC_VIDEO_OPTIMIZATION = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE Or WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_TELECINE_DD_TOP
		Private Const optTELECINE_AA_BOTTOM As WMENC_VIDEO_OPTIMIZATION = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE Or WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_TELECINE_AA_BOTTOM
		Private Const optTELECINE_BB_BOTTOM As WMENC_VIDEO_OPTIMIZATION = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE Or WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_TELECINE_BB_BOTTOM
		Private Const optTELECINE_BC_BOTTOM As WMENC_VIDEO_OPTIMIZATION = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE Or WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_TELECINE_BC_BOTTOM
		Private Const optTELECINE_CD_BOTTOM As WMENC_VIDEO_OPTIMIZATION = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE Or WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_TELECINE_CD_BOTTOM
		Private Const optTELECINE_DD_BOTTOM As WMENC_VIDEO_OPTIMIZATION = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE Or WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_TELECINE_DD_BOTTOM
		Private Const optINTERLACED_AUTO As WMENC_VIDEO_OPTIMIZATION = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_PROCESS_INTERLACED Or WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INTERLACED_AUTO
		Private Const optINTERLACED_TOP_FIRST As WMENC_VIDEO_OPTIMIZATION = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_PROCESS_INTERLACED Or WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INTERLACED_TOP_FIRST
		Private Const optINTERLACED_BOTTOM_FIRST As WMENC_VIDEO_OPTIMIZATION = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_PROCESS_INTERLACED Or WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INTERLACED_BOTTOM_FIRST

		Private Sub Control_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
			_parentDlg.FileDlgDefaultViewMode = NativeMethods.FolderViewMode.List
			Dim fd As SaveFileDialog(Of TargetControl) = TryCast(_parentDlg, SaveFileDialog(Of TargetControl))
			AddHandler fd.FileOk, AddressOf CreateEncodeInfo
		End Sub

		Private Sub CreateEncodeInfo(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs)
			_encodeInfo = New strucEncodeInfo()
			Try
				_encodeInfo.Profile = ProfileFile.Text
				If ComboBox_PreProc.Text = "Standard" Then
					_encodeInfo.Preproc = optSTANDARD
				ElseIf ComboBox_PreProc.Text = "Deinterlace" Then
					_encodeInfo.Preproc = optDEINTERLACE
				ElseIf ComboBox_PreProc.Text = "Inverse Telecine Auto" Then
					_encodeInfo.Preproc = optTELECINE_AUTO
				ElseIf ComboBox_PreProc.Text = "Inverse Telecine AA Top" Then
					_encodeInfo.Preproc = optTELECINE_AA_TOP
				ElseIf ComboBox_PreProc.Text = "Inverse Telecine BB Top" Then
					_encodeInfo.Preproc = optTELECINE_BB_TOP
				ElseIf ComboBox_PreProc.Text = "Inverse Telecine BC Top" Then
					_encodeInfo.Preproc = optTELECINE_BC_TOP
				ElseIf ComboBox_PreProc.Text = "Inverse Telecine CD Top" Then
					_encodeInfo.Preproc = optTELECINE_CD_TOP
				ElseIf ComboBox_PreProc.Text = "Inverse Telecine DD Top" Then
					_encodeInfo.Preproc = optTELECINE_DD_TOP
				ElseIf ComboBox_PreProc.Text = "Inverse Telecine AA Bottom" Then
					_encodeInfo.Preproc = optTELECINE_AA_BOTTOM
				ElseIf ComboBox_PreProc.Text = "Inverse Telecine BB Bottom" Then
					_encodeInfo.Preproc = optTELECINE_BB_BOTTOM
				ElseIf ComboBox_PreProc.Text = "Inverse Telecine BC Bottom" Then
					_encodeInfo.Preproc = optTELECINE_BC_BOTTOM
				ElseIf ComboBox_PreProc.Text = "Inverse Telecine CD Bottom" Then
					_encodeInfo.Preproc = optTELECINE_CD_BOTTOM
				ElseIf ComboBox_PreProc.Text = "Inverse Telecine DD Bottom" Then
					_encodeInfo.Preproc = optTELECINE_DD_BOTTOM
				ElseIf ComboBox_PreProc.Text = "Process Interlaced Auto" Then
					_encodeInfo.Preproc = optINTERLACED_AUTO
				ElseIf ComboBox_PreProc.Text = "Process Interlaced Top First" Then
					_encodeInfo.Preproc = optINTERLACED_TOP_FIRST
				ElseIf ComboBox_PreProc.Text = "Process Interlaced Bottom First" Then
					_encodeInfo.Preproc = optINTERLACED_BOTTOM_FIRST
				End If
				_encodeInfo.DRMProfile = ComboBox_DRMProfile.Text
				If ComboBox_DRMProfile.Text.Length = 0 Then
					_encodeInfo.DRMProfile = "None"
				End If
				_encodeInfo.Crop = CheckBox_Crop.IsChecked = True
				_encodeInfo.CropTop = If((Text_Top.Text.Length = 0), 0, Long.Parse(Text_Top.Text))
				_encodeInfo.CropLeft = If((Text_Left.Text.Length = 0), 0, Long.Parse(Text_Left.Text))
				_encodeInfo.CropRight = If((Text_Right.Text.Length = 0), 0, Long.Parse(Text_Right.Text))
				_encodeInfo.CropBottom = If((Text_Bottom.Text.Length = 0), 0, Long.Parse(Text_Bottom.Text))
				_encodeInfo.TwoPass = CheckBox_TwoPass.IsChecked.Value
				_encodeInfo.Title = TextBox_Title.Text
				_encodeInfo.Description = TextBox_Description.Text
				_encodeInfo.Author = TextBox_Author.Text
				_encodeInfo.Copyright = TextBox_Copyright.Text



			Catch ex As Exception
				MessageBox.Show(ex.Message, "error in the encoder parameters", MessageBoxButton.OK, MessageBoxImage.Stop)
				e.Cancel = True
			End Try
		End Sub

		Private _encodeInfo As strucEncodeInfo

		Friend Property EncodeInfo() As strucEncodeInfo
			Get
				Return _encodeInfo
			End Get
			Set(ByVal value As strucEncodeInfo)
				_encodeInfo = value
			End Set
		End Property

		Private Sub sEnumPreprocess()
			Try
				If ComboBox_PreProc.Items.Count > 0 Then
					Return
				End If
				ComboBox_PreProc.Items.Add("Standard")
				ComboBox_PreProc.Items.Add("Deinterlace")
				ComboBox_PreProc.Items.Add("Inverse Telecine Auto")
				ComboBox_PreProc.Items.Add("Inverse Telecine AA Top")
				ComboBox_PreProc.Items.Add("Inverse Telecine BB Top")
				ComboBox_PreProc.Items.Add("Inverse Telecine BC Top")
				ComboBox_PreProc.Items.Add("Inverse Telecine CD Top")
				ComboBox_PreProc.Items.Add("Inverse Telecine DD Top")
				ComboBox_PreProc.Items.Add("Inverse Telecine AA Bottom")
				ComboBox_PreProc.Items.Add("Inverse Telecine BB Bottom")
				ComboBox_PreProc.Items.Add("Inverse Telecine BC Bottom")
				ComboBox_PreProc.Items.Add("Inverse Telecine CD Bottom")
				ComboBox_PreProc.Items.Add("Inverse Telecine DD Bottom")
				ComboBox_PreProc.Items.Add("Process Interlaced Auto")
				ComboBox_PreProc.Items.Add("Process Interlaced Top First")
				ComboBox_PreProc.Items.Add("Process Interlaced Bottom First")
				ComboBox_PreProc.SelectedIndex = 0
			Catch ex As Exception
				Dim strError As String = ex.ToString()
			End Try
		End Sub

        Private Sub TabControl_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
            If e.AddedItems.Count <= 0 Then
                Return
            End If
            If e.AddedItems(0).Equals(DRMProfile) Then
                sEnumDRMProfiles()
            ElseIf e.AddedItems(0).Equals(Profile) Then
                sEnumPreprocess()
                If ProfileFile.Text.Length = 0 Then
                    ComboBox_PreProc.IsEnabled = False
                    ComboBox_PreProc.SelectedIndex = -1
                Else
                    ComboBox_PreProc.IsEnabled = True
                End If
            End If

        End Sub


		Private _source As System.Windows.Interop.HwndSource
		Private _parentDlg As IFileDlgExt
		#Region "IWindowExt Members"

        Public WriteOnly Property Source() As System.Windows.Interop.HwndSource Implements IWindowExt.Source
            Set(ByVal value As System.Windows.Interop.HwndSource)
                _source = value
            End Set
        End Property

        Public WriteOnly Property ParentDlg() As IFileDlgExt Implements IWindowExt.ParentDlg
            Set(ByVal value As IFileDlgExt)
                _parentDlg = value
            End Set
        End Property
	   #End Region
		Protected Overrides Function ArrangeOverride(ByVal arrangeBounds As Size) As Size
			If Height > 0 AndAlso Width > 0 Then
				arrangeBounds.Height = Me.Height
				arrangeBounds.Width = Me.Width
			End If
			Return MyBase.ArrangeOverride(arrangeBounds)
		End Function



		Private Sub SelectProfile_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
			Dim ofd As New Microsoft.Win32.OpenFileDialog()
			ofd.InitialDirectory = "C:\Program Files\Windows Media Components\Encoder\Profiles"
			ofd.Filter = "prx files (*.prx)|*.prx|All files (*.*)|*.*"
			Dim res? As Boolean = ofd.ShowDialog()
			If res.HasValue AndAlso res.Value Then
				ProfileFile.Text = ofd.FileName
				sEvalProfile(ofd.FileName)
				ComboBox_PreProc.SelectedIndex = -1
				ComboBox_PreProc.IsEnabled = True
			End If
		End Sub
		Private Sub sEvalProfile(ByVal strProfilePath As String)
			Dim pro As WMEncProfile2 = Nothing
			Dim intVideoMode As Integer = 0
			Dim intAudioMode As Integer = 0
			Dim boolVideoTwoPassLock As Boolean = False
			Dim intVideoPasses As Integer = 0
			Dim boolAudioTwoPassLock As Boolean = False
			Dim intAudioPasses As Integer = 0
			Dim intProContentType As Integer = 0
			Try
				pro = New WMEncProfile2()
				pro.LoadFromFile(strProfilePath)
				intProContentType = pro.ContentType
				intVideoMode = CInt(Fix(pro.VBRMode(WMENC_SOURCE_TYPE.WMENC_VIDEO, 0)))
				intAudioMode = CInt(Fix(pro.VBRMode(WMENC_SOURCE_TYPE.WMENC_AUDIO, 0)))
				If intVideoMode = 1 Then
					boolVideoTwoPassLock = False
					intVideoPasses = 0
				ElseIf intVideoMode = 2 Then
					boolVideoTwoPassLock = True
					intVideoPasses = 2
				ElseIf intVideoMode = 3 Then
					boolVideoTwoPassLock = True
					intVideoPasses = 1
				ElseIf intVideoMode = 4 Then
					boolVideoTwoPassLock = True
					intVideoPasses = 2
				End If
				If intAudioMode = 1 Then
					boolAudioTwoPassLock = False
					intAudioPasses = 0
				ElseIf intAudioMode = 2 Then
					boolAudioTwoPassLock = True
					intAudioPasses = 2
				ElseIf intAudioMode = 3 Then
					boolAudioTwoPassLock = True
					intAudioPasses = 1
				ElseIf intAudioMode = 4 Then
					boolAudioTwoPassLock = True
					intAudioPasses = 2
				End If
				If boolAudioTwoPassLock = True Or boolVideoTwoPassLock = True Then
					CheckBox_TwoPass.IsEnabled = False
				Else
					CheckBox_TwoPass.IsEnabled = True
				End If
				If intVideoPasses > intAudioPasses Then
					If intVideoPasses = 0 Then
						CheckBox_TwoPass.IsChecked = False
					ElseIf intVideoPasses = 1 Then
						CheckBox_TwoPass.IsChecked = False
					ElseIf intVideoPasses = 2 Then
						CheckBox_TwoPass.IsChecked = True
					End If
				Else
					If intAudioPasses = 0 Then
						CheckBox_TwoPass.IsChecked = False
					ElseIf intAudioPasses = 1 Then
						CheckBox_TwoPass.IsChecked = False
					ElseIf intAudioPasses = 2 Then
						CheckBox_TwoPass.IsChecked = True
					End If
				End If
			Catch ex As System.Runtime.InteropServices.COMException
				InstallEncoderQuery(ex)
			Catch ex As Exception
				Dim strError As String = ex.ToString()
				_parentDlg.FileDlgEnableOkBtn = False
			Finally
				If pro IsNot Nothing Then
				Marshal.ReleaseComObject(pro)
				End If

			End Try
		End Sub


		Private Sub sEnumDRMProfiles()
			If ComboBox_DRMProfile.Items.Count > 0 Then
				Return
			End If
			Try
				Dim tempEncoder As New WMEncoder()
				Dim DRM As IWMDRMContentAuthor = tempEncoder.EncoderDRMContentAuthor
				Dim DRMProColl As IWMDRMProfileCollection = DRM.DRMProfileCollection
 Dim i As Integer

				ComboBox_DRMProfile.Items.Add("None")
				For i = 0 To DRMProColl.Count - 1
					ComboBox_DRMProfile.Items.Add(DRMProColl.Item(i).Name)
				Next i
				ComboBox_DRMProfile.SelectedIndex = 0
				tempEncoder = Nothing
			Catch ex As System.Runtime.InteropServices.COMException
				InstallEncoderQuery(ex)
			Catch ex As Exception
				Dim strError As String = ex.ToString()
			End Try
		End Sub

		Private Sub SelectProfile_Initialized(ByVal sender As Object, ByVal e As EventArgs)


		End Sub

		Private Sub ComboBox_PreProc_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
			Dim pro As WMEncProfile2 = Nothing
			Try
				If System.IO.File.Exists(ProfileFile.Text) AndAlso ComboBox_PreProc.SelectedIndex <> -1 Then
					pro = New WMEncProfile2()
					pro.LoadFromFile(ProfileFile.Text)
					Dim intProContentType As Integer = pro.ContentType
					Dim strPreProc As String = (TryCast(sender, ComboBox)).SelectedValue.ToString()
					strPreProc = strPreProc.Remove(0, strPreProc.LastIndexOf(":"c) + 1)

					If intProContentType = 16 OrElse intProContentType = 17 Then
						pro.InterlaceMode(0) = False
						Select Case strPreProc
							Case "Process Interlaced Auto", "Process Interlaced Top First", "Process Interlaced Bottom First"
								MessageBox.Show("The profile does not support the type of preprocessing selected." & Environment.NewLine & Environment.NewLine & "Change the preprocessing selected or check the box" & Environment.NewLine & "in the profile editor next to 'Allow Interlaced Processing' for this profile.", "Error")
								ComboBox_PreProc.Focus()
								ComboBox_PreProc.SelectedIndex = -1
						End Select

					End If

					If intProContentType = 1 Then
						Dim dlg As SaveFileDialog(Of TargetWindow) = TryCast(_parentDlg, SaveFileDialog(Of TargetWindow))
						dlg.FilterIndex = 3
					End If

				End If
				_parentDlg.FileDlgEnableOkBtn = ComboBox_PreProc.SelectedIndex <> -1
			Catch ex As System.Runtime.InteropServices.COMException
				InstallEncoderQuery(ex)
			Finally
				If pro IsNot Nothing Then
					Marshal.ReleaseComObject(pro)
				End If

			End Try

		End Sub

		Private Sub InstallEncoderQuery(ByVal ex As COMException)
            If ex.ErrorCode = -2147221164 Then
                If MessageBox.Show("Windows Media Encoder 9 is not installed." & Environment.NewLine & "Do you want to download and install it?", "Error", MessageBoxButton.YesNo) = MessageBoxResult.Yes Then
                    System.Diagnostics.Process.Start("www.softpedia.com/progDownload/Windows-Media-Encoder-Download-1393.html")
                Else
                    'Application.Current.Shutdown(-1);
                End If
                _parentDlg.FileDlgEnableOkBtn = False
            End If
		End Sub

		Private Sub CheckBox_Crop_Checked(ByVal sender As Object, ByVal e As RoutedEventArgs)
			Text_Top.IsEnabled = True
			Text_Top.IsReadOnly = False
			Text_Top.Focusable = True

		End Sub
	End Class
End Namespace
