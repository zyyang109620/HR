
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
Imports Pos.Business
Imports Comm.Business
Imports Org.Business
Imports Bat.Business
Imports Acer.File
Imports File.Business

#End Region

Partial Class POS2010_01
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
			gridEvent.IsColumnReSize = True	   '�]�w���Y�i���ܤj�p
			gridEvent.IsColumnFilter = True	   '����������

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


End Class