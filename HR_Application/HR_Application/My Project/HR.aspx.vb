
#Region "�פJ����Ѧ�"
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports System.Reflection.MethodBase
Imports Acer.Util
Imports Sys.Business
Imports Acer.Form.FormUtil
Imports Acer.Base
Imports Acer.Form
Imports Acer.Apps
Imports System.Data
Imports POS.Business
Imports Comm.Business
Imports Org.Business
Imports Bat.Business
Imports Acer.File
Imports File.Business

#End Region

Partial Class HR
	Inherits Acer.Base.PageBase

#Region "�������J�ʧ@ Page_Load"
	Dim rptDocument As ReportDocument = New CrystalDecisions.CrystalReports.Engine.ReportDocument()

	Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
		'=== �h��y�t�ϥ� ===
		Me.PageID = GetCurrentMethod.DeclaringType.Name
		'test
		'Session("FUNC_PERMISSION") = "0001,0002,0003,0004,0006"
		'/test
		Session("SHOW_ALL") = "Y"
	End Sub

	Protected Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
		Session("SHOW_ALL") = Nothing
	End Sub
	''' <summary>
	''' �������J�ʧ@
	''' </summary>
	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try
			Me.LogUtil.MethodStart(GetCurrentMethod.Name)

			'=== �]�w�����\�� ===
			If Not Page.IsPostBack Then
				Dim CtManageOrganization As New CtManageOrganization(Me.DBManager, Me.LogUtil)
				FormUtil.BindDropDownList(Q_UPPER_DEP_CODE, CtManageOrganization.GetOrgDDL(), "SELECT_VALUE", "SELECT_TEXT", UIType.Query, True)

				Dim CtTeachingSet As New CtTeachingSet(Me.DBManager, Me.LogUtil)
				'�b¾���A
				FormUtil.BindDropDownList(Me.Q_OJOB_STATUS, CtTeachingSet.GetJobTypeDDL(), "SELECT_VALUE", "SELECT_TEXT", UIType.Query, True)

				'Ū���v���g�JAuth�������
				If (Not Session("FUNC_PERMISSION") = Nothing) Then
					Me.PERMISSION.Value = Session("FUNC_PERMISSION")
				End If
				'/Ū���v���g�JAuth�������


				If InStr(Me.PERMISSION.Value, "0005") = 0 And InStr(Me.PERMISSION.Value, "0006") = 0 And InStr(Me.PERMISSION.Value, "0007") = 0 Then
					Q_CH_NAME.Text = SessionClass.�m�W
					Q_ACNT.Text = SessionClass.�n�J�b��
					Q_CH_NAME.Enabled = False
					Q_ACNT.Enabled = False
					ADD_BTN1.Visible = False

				End If

				DEL_BTN1.Enabled = False
				DEL_BTN2.Enabled = False
				'If InStr(Me.PERMISSION.Value, "0005") = 0 Then
				'	DEL_BTN1.Enabled = False
				'	DEL_BTN2.Enabled = False
				'Else
				'	DEL_BTN1.Enabled = True
				'	DEL_BTN2.Enabled = True
				'End If
				'If InStr(Me.PERMISSION.Value, "0008") = 0 Then
				'	PIC_BTN.Visible = False
				'Else
				'	PIC_BTN.Visible = True
				'End If
				depCode.Value = SessionClass.���N�X


				'�����٭�
				QCLEAR_BTN1_ServerClick(Nothing, Nothing)
				'=== �O�_�@�i�J��ܸ�� ===
				If Me.IsListShow Then
					DoDataBind()
				End If
			End If

			'=== ���ץX�ɳB�z ===
			If Request("TYPE") = "EXPORT" Or Request("TYPE") = "EXPORT_ALL" Then
				DoExport(Request("TYPE"))
				Exit Sub
			End If

			'Me.JScript.Script = "SetLogin()"
		Finally
			Me.LogUtil.MethodEnd(GetCurrentMethod.Name)
		End Try
	End Sub
#End Region

#Region "�d�߬���"
#Region "QUERY_BTN_Click �B�z�d�߸�ưʧ@"
	''' <summary>
	''' �B�z�d�߸�ưʧ@
	''' </summary>
	Protected Sub QUERY_BTN_Click(ByVal sender As Object, ByVal e As System.EventArgs)
		Try
			Me.LogUtil.MethodStart(GetCurrentMethod.Name)

			'=== Server Side �ˮ� ===
			Me.ServerSideValid("Q_")

			'=== �d�߹w�]�Ĥ@�� ===
			CType(PC.FindControl("PageNo"), TextBox).Text = 1

			Dim dt As DataTable = DoDataBind()

			''�p�G�d�ߥX�ӥu���@��,�h�����i�J�s�蹺��
			'If (dt.Rows.Count = 1) Then
			'    Dim s As String = dt.Rows(0).Item("OCCUP")
			'    If Not (InStr(Me.PERMISSION.Value, "0005") > 0 Or InStr(Me.PERMISSION.Value, "0006") > 0 Or InStr(Me.PERMISSION.Value, "0007") > 0) Then
			'        '�T���v�����S��
			'        Me.JScript.Script = "doEdit1_2('','PKNO|" & dt.Rows(0).Item("PKNO") & "|ACNT|" & dt.Rows(0).Item("ACNT") & "','MOD');"
			'    Else
			'        Dim OCCUP As String = dt.Rows(0).Item("OCCUP")
			'        If InStr(Me.PERMISSION.Value, "0007") > 0 Then
			'            If OCCUP = "2" Then
			'                Me.JScript.Script = "doEdit1_2('','PKNO|" & dt.Rows(0).Item("PKNO") & "|ACNT|" & dt.Rows(0).Item("ACNT") & "','MOD');"
			'            Else
			'                Me.JScript.Script = "doEdit1_2('','PKNO|" & dt.Rows(0).Item("PKNO") & "|ACNT|" & dt.Rows(0).Item("ACNT") & "','DETAIL');"
			'            End If
			'            '�i���@�ۤv
			'            If dt.Rows(0)("ACNT").ToString() = SessionClass.�n�J�b�� Then
			'                Me.JScript.Script = "doEdit1_2('','PKNO|" & dt.Rows(0).Item("PKNO") & "|ACNT|" & dt.Rows(0).Item("ACNT") & "|SELF|Y','MOD');"
			'            End If
			'            '/�i���@�ۤv
			'        End If
			'        If InStr(Me.PERMISSION.Value, "0006") > 0 Then
			'            If OCCUP = "2" Or OCCUP = "3" Or OCCUP = "4" Then
			'                Me.JScript.Script = "doEdit1_2('','PKNO|" & dt.Rows(0).Item("PKNO") & "|ACNT|" & dt.Rows(0).Item("ACNT") & "','MOD');"
			'            Else
			'                Me.JScript.Script = "doEdit1_2('','PKNO|" & dt.Rows(0).Item("PKNO") & "|ACNT|" & dt.Rows(0).Item("ACNT") & "','DETAIL');"
			'            End If
			'            '�i���@�ۤv
			'            If dt.Rows(0)("ACNT").ToString() = SessionClass.�n�J�b�� Then
			'                Me.JScript.Script = "doEdit1_2('','PKNO|" & dt.Rows(0).Item("PKNO") & "|ACNT|" & dt.Rows(0).Item("ACNT") & "|SELF|Y','MOD');"
			'            End If
			'            '/�i���@�ۤv
			'        End If
			'        If InStr(Me.PERMISSION.Value, "0005") > 0 Then
			'            Me.JScript.Script = "doEdit1_2('','PKNO|" & dt.Rows(0).Item("PKNO") & "|ACNT|" & dt.Rows(0).Item("ACNT") & "','MOD');"
			'        End If
			'    End If
			'End If
			'/�p�G�d�ߥX�ӥu���@��,�h�����i�J�s�蹺��
		Finally
			Me.LogUtil.MethodEnd(GetCurrentMethod.Name)
		End Try
	End Sub
#End Region

#Region "DoDataBind ���� BIND GRIDVIEW �ʧ@"
	''' <summary>
	''' ����BIND GRIDVIEW�ʧ@
	''' </summary>
	Function DoDataBind() As DataTable
		Try
			Me.LogUtil.MethodStart(GetCurrentMethod.Name)

			'=== �I�s Control �i��d�߰ʧ@ ===
			Dim ctrl As CtTeaching = New CtTeaching(Me.DBManager, Me.LogUtil)

			'=== �]�w ����/�C������ ===
			ctrl.PageNo = Me.PageControlPageNo("PC")
			ctrl.PageSize = Me.PageControlPageSize("PC")
			'=== �]�w�ݩʰѼ� ===
			ctrl.CH_NAME = Me.Q_CH_NAME.Text
			ctrl.ACNT = Me.Q_ACNT.Text
			ctrl.DEP_CODE = Me.Q_UPPER_DEP_CODE.SelectedValue
			ctrl.OJOB_STATUS = Q_OJOB_STATUS.SelectedValue
			'ctrl.DUTY_NM = Me.Q_MEMBER_POSTION.SelectiveValue


			'=== �I�s Contril �d�� ===
			Dim dt As DataTable = ctrl.GetByContPersonData()
			'//OCCUP ���O 1-�M�� 2-�ݥ� 3-����
			'//PTTCH_FT ��¾�M¾ 1-�M¾ 0-��¾
			For Each dr As DataRow In dt.Rows
				dr("OCCUP") = dr("OCCUP").ToString.Replace("1", "�M��").Replace("2", "�ݥ�").Replace("3", "����")
				dr("PTTCH_FT") = dr("PTTCH_FT").ToString.Replace("0", "��¾").Replace("1", "�M¾")
			Next
			DataCount.Value = dt.Rows.Count
			DataGrid.DataSource = Me.BindDDFormat(dt, FormatType.Grid)
			DataGrid.DataBind()

			'=== ��ܤ������G ===
			PC.PreparpePage("doQuery", "ReQuery", ctrl.PageSize, ctrl.PageNo, Me.PageRangeSize, ctrl.TotalRowCount)
			PC2.PreparpePage("doQuery", "ReQuery", ctrl.PageSize, ctrl.PageNo, Me.PageRangeSize, ctrl.TotalRowCount)

			'=== �]�w Grid �ƥ�\�� ===
			Dim gridEvent As GridEvent = New GridEvent
			gridEvent.IsColumnReSize = True    '�]�w���Y�i���ܤj�p
			gridEvent.IsColumnFilter = True    '����������

			Me.SetupGridEvent(DataGrid, gridEvent)

			'=== ���õ����B�z���ʧ@ ===
			Me.JScript.HideProcess()

			Return dt
		Finally
			Me.LogUtil.MethodEnd(GetCurrentMethod.Name)
		End Try
	End Function

	''' <summary>
	''' ���o�U�Ԥ��Ҧ�����ର01','02�r��
	''' </summary>
	''' <returns></returns>
	''' <remarks></remarks>
	Private Function MakeDDL2StrComma(ByVal ddl As DropDownList, ByVal value As String) As String
		Dim result As String = ""
		For i As Integer = 0 To ddl.Items.Count - 1
			If (value = "") Then
				If (result <> "") Then
					result &= "','"
				End If
				If (ddl.Items(i).Value <> "") Then
					result &= ddl.Items(i).Value
				End If
			Else
				Return value
			End If
		Next
		Return result
	End Function
#End Region

#Region "DataGrid_RowDataBound �B�z GridView �� RowDataBound �ʧ@(���X��ƦC���ʧ@)"
	''' <summary>
	''' �B�z GridView �� RowDataBound �ʧ@(���X��ƦC���ʧ@)
	''' </summary>
	Protected Sub DataGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles DataGrid.RowDataBound
		'=== �P�_�Y����ƦC�~�B�z ===
		If e.Row.RowType = DataControlRowType.DataRow Then
			Me.PrepareRowDataBoundEvent(DataGrid, e.Row)
			'If String.IsNullOrEmpty(e.Row.DataItem("ACTU_DATEE").ToString()) Then
			'    Dim chkBox As HtmlInputCheckBox = e.Row.FindControl("chkBox")
			'    chkBox.Disabled = True
			'End If

			'If Not (InStr(Me.PERMISSION.Value, "0005") > 0 Or InStr(Me.PERMISSION.Value, "0006") > 0 Or InStr(Me.PERMISSION.Value, "0007") > 0) Then
			'    '�T���v�����S��
			'    e.Row.Cells(1).Visible = True
			'    e.Row.Cells(2).Visible = False
			'Else
			'    Dim OCCUP As String = DataGrid.DataKeys(e.Row.RowIndex)("OCCUP")
			'    If InStr(Me.PERMISSION.Value, "0007") > 0 Then
			'        If OCCUP = "2" Then
			'            e.Row.Cells(1).Visible = True
			'            e.Row.Cells(2).Visible = False
			'        Else
			'            e.Row.Cells(1).Visible = False
			'            e.Row.Cells(2).Visible = True
			'        End If
			'        If DataGrid.DataKeys(e.Row.RowIndex)("ACNT") = SessionClass.�n�J�b�� Then
			'            e.Row.Cells(1).Visible = True
			'            e.Row.Cells(2).Visible = False
			'        End If
			'    End If
			'    If InStr(Me.PERMISSION.Value, "0006") > 0 Then
			'        If OCCUP = "2" Or OCCUP = "3" Or OCCUP = "4" Then
			'            e.Row.Cells(1).Visible = True
			'            e.Row.Cells(2).Visible = False
			'        Else
			'            e.Row.Cells(1).Visible = False
			'            e.Row.Cells(2).Visible = True
			'        End If
			'        If DataGrid.DataKeys(e.Row.RowIndex)("ACNT") = SessionClass.�n�J�b�� Then
			'            e.Row.Cells(1).Visible = True
			'            e.Row.Cells(2).Visible = False
			'        End If
			'    End If
			'    If InStr(Me.PERMISSION.Value, "0005") > 0 Then
			'        e.Row.Cells(1).Visible = True
			'        e.Row.Cells(2).Visible = False
			'    End If


			'End If

			'         If SessionClass.�x�Ǻ����O = "2" Then
			'             If DataGrid.DataKeys(e.Row.RowIndex).Item("ID_TYPE").ToString() = "1" Then
			'                 e.Row.Cells(2).Visible = True
			'                 e.Row.Cells(1).Visible = False
			'             Else
			'                 e.Row.Cells(1).Visible = True
			'                 e.Row.Cells(2).Visible = False
			'             End If
			'         Else

			'End If

			If InStr(Me.PERMISSION.Value, "0005") = 0 Then
				Dim chkBox As HtmlInputCheckBox = e.Row.FindControl("chkBox")
				chkBox.Disabled = True
			Else
				Dim chkBox As HtmlInputCheckBox = e.Row.FindControl("chkBox")
				chkBox.Disabled = False
			End If

			e.Row.Cells(1).Visible = True
			e.Row.Cells(2).Visible = False
		ElseIf DataCount.Value > 0 Then
			e.Row.Cells(0).Visible = False
		End If
	End Sub
#End Region

#Region "ReQuery_Click ���� Grid"
	''' <summary>
	''' ���� Grid
	''' </summary>
	Protected Sub ReQuery_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ReQuery.Click
		Try
			Me.LogUtil.MethodStart(GetCurrentMethod.Name)

			DoDataBind()

		Finally
			Me.LogUtil.MethodEnd(GetCurrentMethod.Name)
		End Try
	End Sub
#End Region
#End Region

#Region "�R������"
#Region "Delete_Click ���U�R"
	''' <summary>
	''' �浧�R��
	''' </summary>
	Protected Sub Delete_Click(ByVal sender As Object, ByVal e As System.EventArgs)
		Try
			Me.LogUtil.MethodStart(GetCurrentMethod.Name)

			DoDelete(CType(sender, LinkButton).Attributes("KEY"))
		Finally
			Me.LogUtil.MethodEnd(GetCurrentMethod.Name)
		End Try
	End Sub
#End Region

#Region "DEL_BTN_Click ���U�h���R��"
	''' <summary>
	''' �h���R��
	''' </summary>
	Protected Sub DEL_BTN_Click(ByVal sender As Object, ByVal e As System.EventArgs)
		Try
			Me.LogUtil.MethodStart(GetCurrentMethod.Name)

			DoDelete(Me.GetKeyValueByGridCheckBox(DataGrid, "chkBox"))
		Finally
			Me.LogUtil.MethodEnd(GetCurrentMethod.Name)
		End Try
	End Sub
#End Region

#Region "DoDelete �B�z�R����ưʧ@"
	''' <summary>
	''' �B�z�R����ưʧ@
	''' </summary>
	''' <param name="keyValue">�R���� Key</param>
	Private Sub DoDelete(ByVal keyValue As String)
		Try
			Me.LogUtil.MethodStart(GetCurrentMethod.Name)

			'=== �I�s Control �i��R���ʧ@ ===
			Dim ctrl As CtTeaching = New CtTeaching(Me.DBManager, Me.LogUtil)
			Dim keyMap As ArrayList = FormUtil.GetGridKeyMap(keyValue)
			For i As Integer = 0 To keyMap.Count - 1
				'=== �]�w�ݩʰѼ� ===
				'ctrl.PKNO = keyMap(i)("PKNO")
				ctrl.ACNT = keyMap(i)("ACNT")

				'=== �I�s Contril �R�� ===
				ctrl.DeleteTeaching()

			Next

			'=== �e�ݳB�z ===
			Me.JScript.OpenSuccess(LangUtil.LangMap("COMMON.MSG.�R�����\"))

			DoDataBind()
		Finally
			Me.LogUtil.MethodEnd(GetCurrentMethod.Name)
		End Try
	End Sub
#End Region
#End Region

#Region "�ץX����"
#Region "DoExport �B�z�ץX Excel �ʧ@"
	''' <summary>
	''' �B�z�ץX Excel �ʧ@
	''' </summary>
	''' <param name="exportType">�ץX����, ���ץX�ο���ץX</param>
	Sub DoExport(ByVal exportType As String)
		Try
			Me.LogUtil.MethodStart(GetCurrentMethod.Name)

			Response.ContentType = "application/octet-stream"
			Response.AddHeader("Content-disposition", "attachment; filename=POS2010.csv")
			Response.Charset = "Big5"
			Response.ContentEncoding = System.Text.Encoding.GetEncoding("BIG5")
			Response.Clear()

			Me.EnableViewState = False

			'=== �ץX����B�z ===
			If Request("TYPE") = "EXPORT_ALL" Then
				'=== �N Request ��ƹ����챱��W, For �ץX�ϥ�(�ȳB�z TextBox �� DropDownList) ===
				Me.MapRequestToUIControl(Me)
			End If

			'=== �{���N�X�άd�߱��� ===
			Response.Write("�{���N�X�GPOS2010, ")
			Response.Write("�d�߱���G�m�W-" & Request("Q_CH_NAME") &
			  " �b��-" & Request("Q_ACNT") &
			  " �W�h���-" & Request("Q_DEP_CODE_NM") &
			  " �������O-" & Request("Q_ID_TYPE_NM") &
			  " �b¾���A-" & Request("Q_OJOB_STATUS_NM") &
			  " ¾��-" & Request("Q_JOBTITLE_CODE") & vbCrLf)


			'�ץX�ɽмW�[�Ǿ�.���ݸ�Ƥθg��,�]���O�h����,�G������X���ƴN�n
			'=== �e���z����쵲�G ===
			'Dim column As String() = Request("ColumnFilter").Split("$")
			Dim column As String() = {"ACNT", "OCCUP", "NATION_NAME", "RESIDENCE_NO", "IDNO", "MARRIAGE_NAME", "BIRTH_DATE", "CH_NAME", "ENGNAME", "SEX1", "BLOOD", "EMAIL", "EMAIL1", "RESIDENCE_ZIP", "RESIDENCE_ADDR", "PRESENT_ZIP", "PRESENT_ADDR", "CRRS_HOUSEHOLD_TEL", "CPHONE", "CRRS_COMPANY_TEL", "CRRS_COMPANY_EXT", "JOB_STATUS", "RMK", "ID_TYPE_NA", "PTTCH_FT_NA", "DEP_NAME", "IS_MANAGER_NA", "JOB_TITLE", "TeachingTitle"}

			'=== �B�z���D���� ===
			Dim header As String = "�b��,���O,���y,�~�d�Ҹ�,�����Ҧr��,�B��,�X�ͤ��,����m�W,�^��m�W,�ʧO,�嫬,�դ��q�l�H�c,�ե~�q�l�H�c,���y�l���ϸ�,���y�a�},�{�~�l���ϸ�,�{�~�a�},��a�q��,���,�Ǯչq��,�Ǯդ���,�b¾���A,�Ƶ�,�������O,�M��¾,���W��,�O�_�D��,¾��, �g��"

			'Dim header As String = column(1)
			Dim strBuff As StringBuilder = New StringBuilder()

			Response.Write(header & vbCrLf)


			'=== �I�s Control �i����o��ưʧ@ ===
			Dim ctrl As CtTeaching = New CtTeaching(Me.DBManager, Me.LogUtil)

			'=== �]�w�ݩʰѼ� ===
			'If exportType = "EXPORT" Then
			'	'Dim keyMap As ArrayList = FormUtil.GetGridKeyMap(Utility.CheckNull(Request("chkBox"), "").Replace(",", "$"))
			'	'For i As Integer = 0 To keyMap.Count - 1
			'	'	ctrl.PKNO = keyMap(i)("PKNO").ToString()
			'	'	ctrl.AddQueryOr("DoQuery")
			'	'Next
			'Else
			ctrl.CH_NAME = Request("Q_CH_NAME")
			ctrl.ACNT = Request("Q_ACNT")
			ctrl.DEP_CODE = Request("Q_UPPER_DEP_CODE")
			ctrl.OJOB_STATUS = Request("Q_OJOB_STATUS")
			ctrl.JOBTITLE_CODE = Request("Q_JOBTITLE_CODE")

			'=== �I�s Contril ���o��� ===
			' Dim dt As DataTable = ctrl.GetTeachingData()
			Dim dt As DataTable = ctrl.GetByContPersonData()
			dt = Me.BindDDFormat(dt, FormatType.Grid)
			Dim content As StringBuilder = New StringBuilder()
			dt.Columns.Add("TeachingTitle")
			dt.Columns.Add("FamilyDate")
			dt.Columns.Add("PersonExperience")
			Dim ctrl1 As CtTeaching = New CtTeaching(Me.DBManager, Me.LogUtil)
			'�a�X�g��
			Dim dtTeach As DataTable = ctrl1.GetTeachingTitle
			'���ݸ��
			Dim dtFamily As DataTable = ctrl1.GetFamilyDate
			'�Ǿ�
			Dim dtExper As DataTable = ctrl.GetPersonExperience
			dt = Comm.Common.Common.HideIdno(dt, "IDNO")
			'column = column(0).Split(",")
			For i As Integer = 0 To dt.Rows.Count - 1
				dt.Rows(i)("TeachingTitle") = dtTeach.Select("ACNT='" & dt.Rows(i)("ACNT").ToString() & "'").Length & "��"
				dt.Rows(i)("FamilyDate") = dtFamily.Select("ACNT='" & dt.Rows(i)("ACNT").ToString() & "'").Length & "��"
				dt.Rows(i)("PersonExperience") = dtExper.Select("ACNT='" & dt.Rows(i)("ACNT").ToString() & "'").Length & "��"
				dt.Rows(i)("OCCUP") = dt.Rows(i)("OCCUP").ToString.Replace("1", "�M��").Replace("2", "�ݥ�").Replace("3", "����")
				For j As Integer = 0 To column.Length - 1
					If j > 0 Then
						content.Append(",")
					End If

					content.Append(Utility.CsvConvert(dt.Rows(i)(column(j))))
				Next

				Response.Write(content.ToString & vbCrLf)
				content.Length = 0
			Next

			Me.OnPreRender(Nothing)

			Response.End()
		Finally
			Me.LogUtil.MethodEnd(GetCurrentMethod.Name)
		End Try
	End Sub
#End Region
#End Region

#Region "����v���ܰ�"
	Private Sub ColClose()
		Q_ACNT.Text = SessionClass.�n�J�b��
		Q_CH_NAME.Text = SessionClass.�m�W
		Q_CH_NAME.Enabled = False
		Q_ACNT.Enabled = False
	End Sub
	Private Sub ColOpen()
		Q_CH_NAME.Enabled = True
		Q_ACNT.Enabled = True
	End Sub
#End Region

#Region "�٭�_OnClick"
	Protected Sub QCLEAR_BTN1_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs)


		If InStr(Me.PERMISSION.Value, "0005") = 0 And InStr(Me.PERMISSION.Value, "0006") = 0 And InStr(Me.PERMISSION.Value, "0007") = 0 Then
			Q_CH_NAME.Text = SessionClass.�m�W
			Q_ACNT.Text = SessionClass.�n�J�b��
			Q_CH_NAME.Enabled = False
			Q_ACNT.Enabled = False
			ADD_BTN1.Visible = False

		End If
	End Sub
#End Region

#Region "�ץX�Ӥ�"
	Protected Sub PIC_BTN_Click(ByVal sender As Object, ByVal e As System.EventArgs)
		Try
			Me.LogUtil.MethodStart(GetCurrentMethod.Name)


			Dim ctrlFile As CtAttchFile = New CtAttchFile(Me.DBManager, Me.LogUtil)
			Dim ctrl As CtTeaching = New CtTeaching(Me.DBManager, Me.LogUtil)

			'=== �]�w ����/�C������ ===
			ctrl.PageNo = Me.PageControlPageNo("PC")
			ctrl.PageSize = Me.PageControlPageSize("PC")
			'=== �]�w�ݩʰѼ� ===
			ctrl.CH_NAME = Me.Q_CH_NAME.Text
			ctrl.ACNT = Me.Q_ACNT.Text
			ctrl.OJOB_STATUS = Q_OJOB_STATUS.SelectedValue


			'=== �I�s Contril �d�� ===
			Dim dt As DataTable = ctrl.GetByContPersonData()
			'//OCCUP ���O 1-�M�� 2-�ݥ� 3-����
			'//PTTCH_FT ��¾�M¾ 1-�M¾ 0-��¾
			For Each dr As DataRow In dt.Rows
				dr("OCCUP") = dr("OCCUP").ToString.Replace("1", "�M��").Replace("2", "�ݥ�").Replace("3", "����")
				dr("PTTCH_FT") = dr("PTTCH_FT").ToString.Replace("0", "��¾").Replace("1", "�M¾")
			Next
			If dt.Rows.Count = 0 Then
				Me.JScript.ShowMessage("�d�L���!")
				Me.JScript.HideProcess()
				Exit Try
			End If
			'�إ߸�Ƨ�
			Dim filePath As String = APConfig.GetProperty("TMP_EXP_FILE_PATH") & "POS2010_" & SessionClass.�n�J�b�� & "\"
			Dim directoryPath As System.IO.DirectoryInfo = New System.IO.DirectoryInfo(filePath)
			If directoryPath.Exists() Then
				directoryPath.Delete(True)
			End If
			FileUtil.CreateDir(filePath)

			Dim filename As ArrayList = New ArrayList()
			Dim fnlist As ArrayList = New ArrayList()
			For Each dr As DataRow In dt.Rows
				ctrlFile.ACCE_SOURCE = "POS2010_02"
				ctrlFile.FILE_NO = dr("ACNT").ToString()
				Dim fileDt As DataTable = ctrlFile.GetAllFile()
				If fileDt.Rows.Count > 0 Then
					Dim picFile As String = APConfig.GetProperty("TEMP_UPLOADFILE_PATH") & fileDt.Rows(0).Item("FILE_ACCESS_PATH").ToString & fileDt.Rows(0).Item("ACTUAL_FILENAME").ToString
					Dim newPicFile As String = filePath & dr.Item("IDNO").ToString & "_" & dr.Item("CH_NAME").ToString & ".jpg"


					If System.IO.File.Exists(picFile) Then
						System.IO.File.Copy(picFile, newPicFile, True)

						filename.Add(newPicFile)
						fnlist.Add(dr.Item("IDNO").ToString & "_" & dr.Item("CH_NAME").ToString & ".jpg")
					End If
				End If
			Next

			Dim zipfile As String = DateTime.Now.Year.ToString() & Now.Month.ToString.PadLeft(2, "0") & Now.Day.ToString.PadLeft(2, "0") & Now.Hour.ToString.PadLeft(2, "0") & Now.Minute.ToString.PadLeft(2, "0") & Now.Second.ToString.PadLeft(2, "0") & Now.Millisecond.ToString.PadLeft(2, "0") & ".zip"
			Dim outfile As String = Application("vr") & "utility/download.aspx?name=" & Server.UrlEncode(filePath & zipfile) & "#"
			Comm.Common.Common.ZipOneFilesbyArr(filename, fnlist, filePath & zipfile)

			Me.JScript.Script = "doOpenUrl('" & outfile & "');"
			Me.JScript.HideProcess()

			Me.JScript.HideProcess()
		Finally
			Me.LogUtil.MethodEnd(GetCurrentMethod.Name)
		End Try
	End Sub
#End Region

End Class