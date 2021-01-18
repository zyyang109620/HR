
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
Imports POS.Business
Imports Comm.Business
Imports Org.Business
Imports Bat.Business
Imports Acer.File
Imports File.Business

#End Region

Partial Class HR
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

			''如果查詢出來只有一筆,則直接進入編輯劃面
			'If (dt.Rows.Count = 1) Then
			'    Dim s As String = dt.Rows(0).Item("OCCUP")
			'    If Not (InStr(Me.PERMISSION.Value, "0005") > 0 Or InStr(Me.PERMISSION.Value, "0006") > 0 Or InStr(Me.PERMISSION.Value, "0007") > 0) Then
			'        '三個權限都沒有
			'        Me.JScript.Script = "doEdit1_2('','PKNO|" & dt.Rows(0).Item("PKNO") & "|ACNT|" & dt.Rows(0).Item("ACNT") & "','MOD');"
			'    Else
			'        Dim OCCUP As String = dt.Rows(0).Item("OCCUP")
			'        If InStr(Me.PERMISSION.Value, "0007") > 0 Then
			'            If OCCUP = "2" Then
			'                Me.JScript.Script = "doEdit1_2('','PKNO|" & dt.Rows(0).Item("PKNO") & "|ACNT|" & dt.Rows(0).Item("ACNT") & "','MOD');"
			'            Else
			'                Me.JScript.Script = "doEdit1_2('','PKNO|" & dt.Rows(0).Item("PKNO") & "|ACNT|" & dt.Rows(0).Item("ACNT") & "','DETAIL');"
			'            End If
			'            '可維護自己
			'            If dt.Rows(0)("ACNT").ToString() = SessionClass.登入帳號 Then
			'                Me.JScript.Script = "doEdit1_2('','PKNO|" & dt.Rows(0).Item("PKNO") & "|ACNT|" & dt.Rows(0).Item("ACNT") & "|SELF|Y','MOD');"
			'            End If
			'            '/可維護自己
			'        End If
			'        If InStr(Me.PERMISSION.Value, "0006") > 0 Then
			'            If OCCUP = "2" Or OCCUP = "3" Or OCCUP = "4" Then
			'                Me.JScript.Script = "doEdit1_2('','PKNO|" & dt.Rows(0).Item("PKNO") & "|ACNT|" & dt.Rows(0).Item("ACNT") & "','MOD');"
			'            Else
			'                Me.JScript.Script = "doEdit1_2('','PKNO|" & dt.Rows(0).Item("PKNO") & "|ACNT|" & dt.Rows(0).Item("ACNT") & "','DETAIL');"
			'            End If
			'            '可維護自己
			'            If dt.Rows(0)("ACNT").ToString() = SessionClass.登入帳號 Then
			'                Me.JScript.Script = "doEdit1_2('','PKNO|" & dt.Rows(0).Item("PKNO") & "|ACNT|" & dt.Rows(0).Item("ACNT") & "|SELF|Y','MOD');"
			'            End If
			'            '/可維護自己
			'        End If
			'        If InStr(Me.PERMISSION.Value, "0005") > 0 Then
			'            Me.JScript.Script = "doEdit1_2('','PKNO|" & dt.Rows(0).Item("PKNO") & "|ACNT|" & dt.Rows(0).Item("ACNT") & "','MOD');"
			'        End If
			'    End If
			'End If
			'/如果查詢出來只有一筆,則直接進入編輯劃面
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
			gridEvent.IsColumnReSize = True    '設定表頭可改變大小
			gridEvent.IsColumnFilter = True    '顯示隱藏欄位

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
			'If String.IsNullOrEmpty(e.Row.DataItem("ACTU_DATEE").ToString()) Then
			'    Dim chkBox As HtmlInputCheckBox = e.Row.FindControl("chkBox")
			'    chkBox.Disabled = True
			'End If

			'If Not (InStr(Me.PERMISSION.Value, "0005") > 0 Or InStr(Me.PERMISSION.Value, "0006") > 0 Or InStr(Me.PERMISSION.Value, "0007") > 0) Then
			'    '三個權限都沒有
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
			'        If DataGrid.DataKeys(e.Row.RowIndex)("ACNT") = SessionClass.登入帳號 Then
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
			'        If DataGrid.DataKeys(e.Row.RowIndex)("ACNT") = SessionClass.登入帳號 Then
			'            e.Row.Cells(1).Visible = True
			'            e.Row.Cells(2).Visible = False
			'        End If
			'    End If
			'    If InStr(Me.PERMISSION.Value, "0005") > 0 Then
			'        e.Row.Cells(1).Visible = True
			'        e.Row.Cells(2).Visible = False
			'    End If


			'End If

			'         If SessionClass.軍學網類別 = "2" Then
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

#Region "刪除相關"
#Region "Delete_Click 按下刪"
	''' <summary>
	''' 單筆刪除
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

#Region "DEL_BTN_Click 按下多筆刪除"
	''' <summary>
	''' 多筆刪除
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

#Region "DoDelete 處理刪除資料動作"
	''' <summary>
	''' 處理刪除資料動作
	''' </summary>
	''' <param name="keyValue">刪除的 Key</param>
	Private Sub DoDelete(ByVal keyValue As String)
		Try
			Me.LogUtil.MethodStart(GetCurrentMethod.Name)

			'=== 呼叫 Control 進行刪除動作 ===
			Dim ctrl As CtTeaching = New CtTeaching(Me.DBManager, Me.LogUtil)
			Dim keyMap As ArrayList = FormUtil.GetGridKeyMap(keyValue)
			For i As Integer = 0 To keyMap.Count - 1
				'=== 設定屬性參數 ===
				'ctrl.PKNO = keyMap(i)("PKNO")
				ctrl.ACNT = keyMap(i)("ACNT")

				'=== 呼叫 Contril 刪除 ===
				ctrl.DeleteTeaching()

			Next

			'=== 前端處理 ===
			Me.JScript.OpenSuccess(LangUtil.LangMap("COMMON.MSG.刪除成功"))

			DoDataBind()
		Finally
			Me.LogUtil.MethodEnd(GetCurrentMethod.Name)
		End Try
	End Sub
#End Region
#End Region

#Region "匯出相關"
#Region "DoExport 處理匯出 Excel 動作"
	''' <summary>
	''' 處理匯出 Excel 動作
	''' </summary>
	''' <param name="exportType">匯出種類, 全匯出或選取匯出</param>
	Sub DoExport(ByVal exportType As String)
		Try
			Me.LogUtil.MethodStart(GetCurrentMethod.Name)

			Response.ContentType = "application/octet-stream"
			Response.AddHeader("Content-disposition", "attachment; filename=POS2010.csv")
			Response.Charset = "Big5"
			Response.ContentEncoding = System.Text.Encoding.GetEncoding("BIG5")
			Response.Clear()

			Me.EnableViewState = False

			'=== 匯出條件處理 ===
			If Request("TYPE") = "EXPORT_ALL" Then
				'=== 將 Request 資料對應到控制項上, For 匯出使用(僅處理 TextBox 及 DropDownList) ===
				Me.MapRequestToUIControl(Me)
			End If

			'=== 程式代碼及查詢條件 ===
			Response.Write("程式代碼：POS2010, ")
			Response.Write("查詢條件：姓名-" & Request("Q_CH_NAME") &
			  " 帳號-" & Request("Q_ACNT") &
			  " 上層單位-" & Request("Q_DEP_CODE_NM") &
			  " 身分類別-" & Request("Q_ID_TYPE_NM") &
			  " 在職狀態-" & Request("Q_OJOB_STATUS_NM") &
			  " 職稱-" & Request("Q_JOBTITLE_CODE") & vbCrLf)


			'匯出時請增加學歷.親屬資料及經歷,因為是多筆的,故直接算出筆數就好
			'=== 畫面篩選欄位結果 ===
			'Dim column As String() = Request("ColumnFilter").Split("$")
			Dim column As String() = {"ACNT", "OCCUP", "NATION_NAME", "RESIDENCE_NO", "IDNO", "MARRIAGE_NAME", "BIRTH_DATE", "CH_NAME", "ENGNAME", "SEX1", "BLOOD", "EMAIL", "EMAIL1", "RESIDENCE_ZIP", "RESIDENCE_ADDR", "PRESENT_ZIP", "PRESENT_ADDR", "CRRS_HOUSEHOLD_TEL", "CPHONE", "CRRS_COMPANY_TEL", "CRRS_COMPANY_EXT", "JOB_STATUS", "RMK", "ID_TYPE_NA", "PTTCH_FT_NA", "DEP_NAME", "IS_MANAGER_NA", "JOB_TITLE", "TeachingTitle"}

			'=== 處理標題部分 ===
			Dim header As String = "帳號,任別,國籍,居留證號,身分證字號,婚姻,出生日期,中文姓名,英文姓名,性別,血型,校內電子信箱,校外電子信箱,戶籍郵遞區號,戶籍地址,現居郵遞區號,現居地址,住家電話,手機,學校電話,學校分機,在職狀態,備註,身分類別,專兼職,單位名稱,是否主管,職稱, 經歷"

			'Dim header As String = column(1)
			Dim strBuff As StringBuilder = New StringBuilder()

			Response.Write(header & vbCrLf)


			'=== 呼叫 Control 進行取得資料動作 ===
			Dim ctrl As CtTeaching = New CtTeaching(Me.DBManager, Me.LogUtil)

			'=== 設定屬性參數 ===
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

			'=== 呼叫 Contril 取得資料 ===
			' Dim dt As DataTable = ctrl.GetTeachingData()
			Dim dt As DataTable = ctrl.GetByContPersonData()
			dt = Me.BindDDFormat(dt, FormatType.Grid)
			Dim content As StringBuilder = New StringBuilder()
			dt.Columns.Add("TeachingTitle")
			dt.Columns.Add("FamilyDate")
			dt.Columns.Add("PersonExperience")
			Dim ctrl1 As CtTeaching = New CtTeaching(Me.DBManager, Me.LogUtil)
			'帶出經歷
			Dim dtTeach As DataTable = ctrl1.GetTeachingTitle
			'親屬資料
			Dim dtFamily As DataTable = ctrl1.GetFamilyDate
			'學歷
			Dim dtExper As DataTable = ctrl.GetPersonExperience
			dt = Comm.Common.Common.HideIdno(dt, "IDNO")
			'column = column(0).Split(",")
			For i As Integer = 0 To dt.Rows.Count - 1
				dt.Rows(i)("TeachingTitle") = dtTeach.Select("ACNT='" & dt.Rows(i)("ACNT").ToString() & "'").Length & "筆"
				dt.Rows(i)("FamilyDate") = dtFamily.Select("ACNT='" & dt.Rows(i)("ACNT").ToString() & "'").Length & "筆"
				dt.Rows(i)("PersonExperience") = dtExper.Select("ACNT='" & dt.Rows(i)("ACNT").ToString() & "'").Length & "筆"
				dt.Rows(i)("OCCUP") = dt.Rows(i)("OCCUP").ToString.Replace("1", "專任").Replace("2", "兼任").Replace("3", "虛擬")
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

#Region "欄位權限變動"
	Private Sub ColClose()
		Q_ACNT.Text = SessionClass.登入帳號
		Q_CH_NAME.Text = SessionClass.姓名
		Q_CH_NAME.Enabled = False
		Q_ACNT.Enabled = False
	End Sub
	Private Sub ColOpen()
		Q_CH_NAME.Enabled = True
		Q_ACNT.Enabled = True
	End Sub
#End Region

#Region "還原_OnClick"
	Protected Sub QCLEAR_BTN1_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs)


		If InStr(Me.PERMISSION.Value, "0005") = 0 And InStr(Me.PERMISSION.Value, "0006") = 0 And InStr(Me.PERMISSION.Value, "0007") = 0 Then
			Q_CH_NAME.Text = SessionClass.姓名
			Q_ACNT.Text = SessionClass.登入帳號
			Q_CH_NAME.Enabled = False
			Q_ACNT.Enabled = False
			ADD_BTN1.Visible = False

		End If
	End Sub
#End Region

#Region "匯出照片"
	Protected Sub PIC_BTN_Click(ByVal sender As Object, ByVal e As System.EventArgs)
		Try
			Me.LogUtil.MethodStart(GetCurrentMethod.Name)


			Dim ctrlFile As CtAttchFile = New CtAttchFile(Me.DBManager, Me.LogUtil)
			Dim ctrl As CtTeaching = New CtTeaching(Me.DBManager, Me.LogUtil)

			'=== 設定 頁次/每頁筆數 ===
			ctrl.PageNo = Me.PageControlPageNo("PC")
			ctrl.PageSize = Me.PageControlPageSize("PC")
			'=== 設定屬性參數 ===
			ctrl.CH_NAME = Me.Q_CH_NAME.Text
			ctrl.ACNT = Me.Q_ACNT.Text
			ctrl.OJOB_STATUS = Q_OJOB_STATUS.SelectedValue


			'=== 呼叫 Contril 查詢 ===
			Dim dt As DataTable = ctrl.GetByContPersonData()
			'//OCCUP 任別 1-專任 2-兼任 3-虛擬
			'//PTTCH_FT 兼職專職 1-專職 0-兼職
			For Each dr As DataRow In dt.Rows
				dr("OCCUP") = dr("OCCUP").ToString.Replace("1", "專任").Replace("2", "兼任").Replace("3", "虛擬")
				dr("PTTCH_FT") = dr("PTTCH_FT").ToString.Replace("0", "兼職").Replace("1", "專職")
			Next
			If dt.Rows.Count = 0 Then
				Me.JScript.ShowMessage("查無資料!")
				Me.JScript.HideProcess()
				Exit Try
			End If
			'建立資料夾
			Dim filePath As String = APConfig.GetProperty("TMP_EXP_FILE_PATH") & "POS2010_" & SessionClass.登入帳號 & "\"
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