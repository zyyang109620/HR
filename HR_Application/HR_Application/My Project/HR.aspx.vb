
#Region "匯入元件參考"
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

#Region "頁面載入動作 Page_Load"
	Dim rptDocument As ReportDocument = New CrystalDecisions.CrystalReports.Engine.ReportDocument()

	Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
		'=== 多國語系使用 ===
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
	''' 頁面載入動作
	''' </summary>
	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Try
			Me.LogUtil.MethodStart(GetCurrentMethod.Name)

			'=== 設定分頁功能 ===
			If Not Page.IsPostBack Then
                Dim CtManageOrganization As New CtManageOrganization(Me.DBManager, Me.LogUtil)
                FormUtil.BindDropDownList(Q_UPPER_DEP_CODE, CtManageOrganization.GetOrgDDL(), "SELECT_VALUE", "SELECT_TEXT", UIType.Query, True)

                Dim CtTeachingSet As New CtTeachingSet(Me.DBManager, Me.LogUtil)
                '在職狀態
                FormUtil.BindDropDownList(Me.Q_OJOB_STATUS, CtTeachingSet.GetJobTypeDDL(), "SELECT_VALUE", "SELECT_TEXT", UIType.Query, True)

				'讀取權限寫入Auth隱藏欄位
				If (Not Session("FUNC_PERMISSION") = Nothing) Then
					Me.PERMISSION.Value = Session("FUNC_PERMISSION")
				End If
				'/讀取權限寫入Auth隱藏欄位


				If InStr(Me.PERMISSION.Value, "0005") = 0 And InStr(Me.PERMISSION.Value, "0006") = 0 And InStr(Me.PERMISSION.Value, "0007") = 0 Then
					Q_CH_NAME.Text = SessionClass.姓名
					Q_ACNT.Text = SessionClass.登入帳號
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
				depCode.Value = SessionClass.單位代碼

	 
				'執行還原
				QCLEAR_BTN1_ServerClick(Nothing, Nothing)
				'=== 是否一進入顯示資料 ===
				If Me.IsListShow Then
					DoDataBind()
				End If
			End If

			'=== 為匯出時處理 ===
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

#Region "查詢相關"
#Region "QUERY_BTN_Click 處理查詢資料動作"
	''' <summary>
	''' 處理查詢資料動作
	''' </summary>
	Protected Sub QUERY_BTN_Click(ByVal sender As Object, ByVal e As System.EventArgs)
		Try
			Me.LogUtil.MethodStart(GetCurrentMethod.Name)

			'=== Server Side 檢核 ===
			Me.ServerSideValid("Q_")

			'=== 查詢預設第一頁 ===
			CType(PC.FindControl("PageNo"), TextBox).Text = 1

			Dim dt As DataTable = DoDataBind()

		Finally
			Me.LogUtil.MethodEnd(GetCurrentMethod.Name)
		End Try
	End Sub
#End Region

#Region "DoDataBind 執行 BIND GRIDVIEW 動作"
	''' <summary>
	''' 執行BIND GRIDVIEW動作
	''' </summary>
	Function DoDataBind() As DataTable
		Try
			Me.LogUtil.MethodStart(GetCurrentMethod.Name)

			'=== 呼叫 Control 進行查詢動作 ===
			Dim ctrl As CtTeaching = New CtTeaching(Me.DBManager, Me.LogUtil)

			'=== 設定 頁次/每頁筆數 ===
			ctrl.PageNo = Me.PageControlPageNo("PC")
			ctrl.PageSize = Me.PageControlPageSize("PC")
			'=== 設定屬性參數 ===
			ctrl.CH_NAME = Me.Q_CH_NAME.Text
			ctrl.ACNT = Me.Q_ACNT.Text			 
            ctrl.DEP_CODE = Me.Q_UPPER_DEP_CODE.SelectedValue
			ctrl.OJOB_STATUS = Q_OJOB_STATUS.SelectedValue
			'ctrl.DUTY_NM = Me.Q_MEMBER_POSTION.SelectiveValue


			'=== 呼叫 Contril 查詢 ===
			Dim dt As DataTable = ctrl.GetByContPersonData()
			'//OCCUP 任別 1-專任 2-兼任 3-虛擬
			'//PTTCH_FT 兼職專職 1-專職 0-兼職
			For Each dr As DataRow In dt.Rows
				dr("OCCUP") = dr("OCCUP").ToString.Replace("1", "專任").Replace("2", "兼任").Replace("3", "虛擬")
				dr("PTTCH_FT") = dr("PTTCH_FT").ToString.Replace("0", "兼職").Replace("1", "專職")
			Next
			DataCount.Value = dt.Rows.Count
			DataGrid.DataSource = Me.BindDDFormat(dt, FormatType.Grid)
			DataGrid.DataBind()

			'=== 顯示分頁結果 ===
			PC.PreparpePage("doQuery", "ReQuery", ctrl.PageSize, ctrl.PageNo, Me.PageRangeSize, ctrl.TotalRowCount)
			PC2.PreparpePage("doQuery", "ReQuery", ctrl.PageSize, ctrl.PageNo, Me.PageRangeSize, ctrl.TotalRowCount)

			'=== 設定 Grid 事件功能 ===
			Dim gridEvent As GridEvent = New GridEvent
			gridEvent.IsColumnReSize = True	   '設定表頭可改變大小
			gridEvent.IsColumnFilter = True	   '顯示隱藏欄位

			Me.SetupGridEvent(DataGrid, gridEvent)

			'=== 隱藏結束處理中動作 ===
			Me.JScript.HideProcess()

			Return dt
		Finally
			Me.LogUtil.MethodEnd(GetCurrentMethod.Name)
		End Try
	End Function

	''' <summary>
	''' 取得下拉中所有資料轉為01','02字串
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

#Region "DataGrid_RowDataBound 處理 GridView 的 RowDataBound 動作(產出資料列的動作)"
	''' <summary>
	''' 處理 GridView 的 RowDataBound 動作(產出資料列的動作)
	''' </summary>
	Protected Sub DataGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles DataGrid.RowDataBound
		'=== 判斷若為資料列才處理 ===
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

#Region "ReQuery_Click 重整 Grid"
	''' <summary>
	''' 重整 Grid
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