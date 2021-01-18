<%@ Page Language="VB" AutoEventWireup="false" CodeFile="HR.aspx.vb" Inherits="HR" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Src="~/UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc2" %>
<%@ Register Src="~/UserControl/PageInitControl.ascx" TagName="PageInitControl" TagPrefix="uc1" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<uc1:PageInitControl ID="PIC" runat="server" />
	
	<title>基本人員資料維護</title>
	<script type="text/javascript"> 
        var viewpage = "POS2010_02.aspx";	//編輯頁面名稱
        var printpage = "HR.aspx";	//列印頁面名稱

        //=== 定義篩選索引陣列 ===
        var columnIndexAry = [3, 4, 5, 6, 7, 8, 9];

        //=== 定義篩選中文名稱陣列 ===
        var columnNameAry = ['帳號', '姓名', '英文名稱', '電子信箱', '單位名稱(原聘任)', '單位主管(原聘任)', '出生日期'];
        var columnCodeAry = ['ACNT', 'CH_NAME', 'ENGNAME', 'EMAIL', 'DEP_NAME', 'UNIT_DIRECTOR_NAME', 'BIRTH_DATE'];
        var recordName = "POS2010_01"

        /** 
        網頁初始時處理 Client 的動作 
        */
        function page_init() {
            page_init_start();

            var keyValue = "";
            //單選：CHOICE=1，多選：CHOICE=2 [必填]
            keyValue = "CHOICE=1";
            //起始單位：ROOT_DEP_CODE=單位代碼 [非必填]
            //keyValue+="&ROOT_DEP_CODE=0001";
            //回傳欄位：回傳欄位，參考ORGT010 [非必填]
            //keyValue+="&RETURNFIELD=UNIT_LEVEL|UNIT_TYPE";
            //要帶回的控制項ID：控制項ID [必填]
            keyValue += "&SOURCEID=UPPER_DEP_NAME#";

            $("#ORG_BTN").fancybox({
                'width': '800',
                'height': '400',
                'type': 'iframe',
                'fitToView': false,
                'href': '<%=Application("vr")%>Application/COM/ORG1030.aspx?' + keyValue,
                'iframe': {
                    'preload': false
                },
                "beforeClose": function () {
                    text = $(".fancybox-iframe").contents().find("#TEXT").val();
                    value = $(".fancybox-iframe").contents().find("#VALUE").val();
                },
                "afterClose": function () {
                    if (value != '')
                        _i(0, "Q_UPPER_DEP_CODE").value = value;
                    if (text != '')
                        _i(0, "UPPER_DEP_NAME").value = text;
                }
            });
        }

        /** 
        查詢功能時呼叫 
        */
        function doQuery() {
            /** 顯示資料處理 */
            Message.showProcess();

            //=== 自定檢查 ===
            //=== 資料檢核及設定, 當有錯誤處理方式為 Form.errAppend(Message) 累計錯誤訊息 ===
            //if (Form.getInput("EDIT", "SYS_CD") == "")
            //	Form.errAppend("系統編號不可空白!!");
            /** ================ */

            //=== 檢查是否有錯誤訊息 ===
            var needSend = valideMessage("Q_");

            //=== 發生錯誤不處理 ===
            if (!needSend) {
                Message.hideProcess();
                return false;
            }

            top.rowKey = null;

            return true;
        }


        /** 
        清空查詢的資料
        */
        function clearQueryForm() {
            for (var i = 0; i < document.forms[0].length; i++) {
                //=== 僅處理 Q_ 的物件 ===
                if (_i(0, i).name.substring(0, 2) != "Q_")
                    continue;

                //=== 下拉預設選取第一筆 ===
                if (_i(0, i).type == 'select-one')
                    _i(0, i).selectedIndex = 0;
                //=== 其餘數入項目預設空白 ===
                else if (_i(0, i).type != 'radio' && _i(0, i).type != 'checkbox')
                    _i(0, i).value = '';
                //=== Radio 或 CheckBox 預設不核選 ===
                else
                    _i(0, i).checked = false;

                //=== 處理 default value ===
                if (_i(0, i).DV != null && _i(0, i).DV != "") {
                    _i(0, i).value = _i(0, i).DV
                    continue;
                }
            }
            //_i(0,"Q_DEP_CODE").value = "";
            _i(0, "UPPER_DEP_NAME").value = "";
        }
        function doOpenUrl(url) {
            window.open(url, "newWin");
        }

        var result = "";
        /*開窗--選擇樹狀組織單選*/
        function SelectOrgno1() {
            var keyValue = "";
            //單選：CHOICE=1，多選：CHOICE=2 [必填]
            keyValue = "CHOICE=1";
            //起始單位：ROOT_DEP_CODE=單位代碼 [非必填]
            //keyValue+="&ROOT_DEP_CODE=0001";
            //回傳欄位：回傳欄位，參考ORGT010 [非必填]
            //keyValue+="&RETURNFIELD=UNIT_LEVEL|UNIT_TYPE";
            //要帶回的控制項ID：控制項ID [必填]
            keyValue += "&SOURCEID=UPPER_DEP_NAME#";

            doOpen('', 800, 600, '<%=Application("vr")%>Application/COM/ORG1030.aspx?' + keyValue);
            if (result["TEXT"] != null)
                _i(0, "UPPER_DEP_NAME").value = result["TEXT"];
            if (result["VALUE"] != null)
                _i(0, "Q_UPPER_DEP_CODE").value = result["VALUE"];
            //if (result["OTHER"] !=null)
            //    _i(0,"ORGNO_OTHER").value = result["OTHER"];
        }

    </script>
	


</head>
<body class="mainBg" leftmargin="0" topmargin="0">

<form id="QUERY" onkeydown="backClick(['Q_', 'QUERY_BTN1'])" runat="server">
<div id="MAIN_DIV" runat="server">
<asp:ScriptManager ID="ScriptManager1" EnablePageMethods="true" runat="server"></asp:ScriptManager>
<script>Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);</script>
<asp:UpdatePanel ID="AjaxPanel" runat="server" >
	<ContentTemplate>
	<asp:HiddenField ID="ActivePageControl" runat="server" />
	<asp:HiddenField ID="ColumnFilter" runat="server" />
	<asp:HiddenField ID="depCode" runat="server" />
	<asp:HiddenField ID="PERMISSION" runat="server" />
	<asp:HiddenField ID="DataCount" runat="server"  />
	<asp:HiddenField ID="JobtitleIndex" runat="server" />
	<asp:HiddenField ID="Jobtitle" runat="server"  />
	<!-- 查詢標題畫面起始 -->
	<table id="QTable1" height="41" width=329 border="0" cellpadding="0" cellspacing="0">
		<tr>
			<td background="<%=WebCommon.CommonPath%>images/title1.jpg">
				<table width="100%"  border="0" cellspacing="0" cellpadding="0">
					<tr>
						<td width="15%">&nbsp;</td>
						<td width="85%" class="title" nowrap><span id="Span1" ML="CL_查詢畫面" runat="server">查詢畫面</span></td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	<!-- 查詢標題畫面結束 -->
	
	<!-- 查詢畫面開始 -->
	<table id="QTable2" width="96%" border="0" align="center" cellpadding="0" cellspacing="0">
		<tr>
			<td width="12"><img src="<%=WebCommon.CommonPath%>images/table_1_l_1.jpg" width="12" height="38"></td>
			<td width="100%" align="left" valign="bottom" background="<%=WebCommon.CommonPath%>images/table_1_bg_1.jpg">
				<!-- 查詢畫面上方按鈕起始 -->
				<table width="100%" border="0" cellspacing="0" cellpadding="0">
					<tr>
						<td height="30" align=right>
						<asp:Button ID="PIC_BTN" cssClass="btn" ML="CB_匯出照片資料"  Visible="false" text="匯出照片資料" OnClientClick="return doQuery()" OnClick="PIC_BTN_Click" runat="server" />
							<%--<input type="button" name="FILTER_BTN1" class="btn" ML="CB_欄位篩選" value='欄位篩選' onkeypress='doFilter();'onclick='doFilter();'>--%>
							<input type="button" ID="ADD_BTN1" class="btn" value='新  增' ML="CB_新增" onclick="doAdd1_2();"  runat="server"/>
							<input type="button" ID="QCLEAR_BTN1" class="btn" ML="CB_清除" value='清  除' onclick="clearQueryForm();" runat="server" onserverclick="QCLEAR_BTN1_ServerClick"/>
							<asp:Button ID="QUERY_BTN1" cssClass="btn" ML="CB_查詢" text="查  詢" OnClientClick="return doQuery()" OnClick="QUERY_BTN_Click" runat="server" />
							<input type="button" class="btn" ML="CB_匯出全部" value='匯出全部' ID="EXPALL_BTN1" onclick='doExport()' runat="server"/>
						</td>
					</tr>
				</table>
				<!-- 查詢畫面上方按鈕結束 -->
			</td>
			<td width="11"><img src="<%=WebCommon.CommonPath%>images/table_1_r_1.jpg" width="11" height="38"></td>
		</tr>
		<tr>
			<td width="11" background="<%=WebCommon.CommonPath%>images/table_1_l_2.jpg"></td>
			<td background="<%=WebCommon.CommonPath%>images/table_1_bg_2.jpg">
				<!-- 查詢畫面起始 -->
				<table width="100%" cellspacing="0" cellpadding="0" class="tableCss">
					<tr>
						<td align='right'><span id="Span2" ML="PL_姓名" runat="server">姓名</span>：</td>
						<td><asp:textbox id='Q_CH_NAME' CssClass='select' runat='server' /></td>
						<td align='right'><span id="Span3" ML="PL_帳號" runat="server">帳號</span>：</td>
						<td><asp:textbox id='Q_ACNT' CssClass='select' runat='server' /></td>
					</tr>					 
					<tr>
					    <td align='right'><span id="Span7" ML="PL_單位名稱(原聘任)" runat="server">單位名稱(原聘任)</span>：</td>
						<td>
                             <%--<asp:HiddenField ID="Q_UPPER_DEP_CODE" runat="server" />--%>
                            <asp:DropDownList ID="Q_UPPER_DEP_CODE" runat="server"></asp:DropDownList>
                            <asp:textbox id='UPPER_DEP_NAME' CssClass='select' runat='server' ValidationGroup="EQ" Enabled="false" Visible="false" />
						    <input type="button" id="ORG_BTN" value="組織開窗" class="btn" runat="server" visible="false" />
						</td>
					    <td align='right'><span id="Span5" ML="PL_在職狀態" runat="server">在職狀態</span>：</td>
						<td>
                            <asp:DropDownList ID="Q_OJOB_STATUS" runat="server"></asp:DropDownList>
						</td>
					</tr>
										<tr>
					    <td align='right'><span id="Span6" ML="PL_職稱" runat="server">職稱</span>：</td>
						<td>
                             <%--<asp:HiddenField ID="Q_UPPER_DEP_CODE" runat="server" />--%>
                            <asp:DropDownList ID="M_JOBTITLE_CODE" runat="server"></asp:DropDownList>
                            <asp:textbox id='JOBTITLE_CODE' CssClass='select' runat='server' ValidationGroup="EQ" Enabled="false" Visible="false" />
						    <input type="button" id="Button1" value="組織開窗" class="btn" runat="server" visible="false" />
						</td>
				
					</tr>
				</table>
				<!-- 查詢畫面結束 -->
				<%--<font color=red>教師人員資料只能在學網新增修改,如在軍網只提供查詢</font>--%>
			</td>
			<td width="13" background="<%=WebCommon.CommonPath%>images/table_1_r_2.jpg">&nbsp;</td>
		</tr>
		<tr>
			<td width="12"><img src="<%=WebCommon.CommonPath%>images/table_1_l_3.jpg" width="12" height="25"></td>
			<!-- 查詢畫面下方按鈕起始 -->
			<td width="100%" valign="top" align="right" background="<%=WebCommon.CommonPath%>images/table_1_bg_3.jpg">
				&nbsp;
				<div style="display:none">
				<input type="button" ID="FILTER_BTN2" class="btn" ML="CB_欄位篩選" value='欄位篩選' onclick='doFilter();' runat="server"/>
				<input type="button" ID="ADD_BTN2" class="btn" value='新  增' ML="CB_新增" onclick="doAdd1_2();"  runat="server"/>
				<input type="button" ID="QCLEAR_BTN2" class="btn" ML="CB_清除" value='清  除' onclick="clearQueryForm();" runat="server"/>
				<asp:Button ID="QUERY_BTN2" cssClass="btn" ML="CB_查詢" text="查  詢" OnClientClick="return doQuery()" OnClick="QUERY_BTN_Click" runat="server" />
				<input type="button" class="btn" ML="CB_匯出全部" value='匯出全部' ID="EXPALL_BTN2" onclick='doExportNoFilter()' runat="server"/>
				</div>
			</td>
			<!-- 查詢畫面下方按鈕結束 -->
			<td width="13"><img src="<%=WebCommon.CommonPath%>images/table_1_r_3.jpg" width="11" height="25"></td>
		</tr>
	</table>
	<!-- 查詢畫面結束 -->
	
	<table width="100%"  border="0" align="center" cellpadding="0" cellspacing="0">
		<tr>
			<td align="left" valign="top">
				<a href="JavaScript:showHideQtable();">
				<img src="<%=WebCommon.CommonPath%>images/up_windows_btn.jpg" width="100%" height="6" border="0"></a>
			</td>
		</tr>
	</table>

	<!-- 標題畫面起始 -->
	<table width="329" height="41"  border="0" cellpadding="0" cellspacing="0">
		<tr>
			<td background="<%=WebCommon.CommonPath%>images/title2.jpg">
				<table width="100%"  border="0" cellspacing="0" cellpadding="0">
					<tr>
						<td width="15%">&nbsp;</td>
						<td width="85%" class="title_2"><span id="Span4" ML="PL_人員資料" runat="server">人員資料</span></td>
					</tr>
				</table>
			</td>
			</tr>
	</table>
	<!-- 標題畫面結束 -->
	
	<!-- 查詢結果畫面起始 -->
	<table width="96%" border="0" align="center" cellpadding="0" cellspacing="0">
		<tr>
			<td width="12"><img src="<%=WebCommon.CommonPath%>images/index_tb1.jpg" width="12" height="38"></td>
			<td width="100%" align="left" valign="bottom" background="<%=WebCommon.CommonPath%>images/index_tb2.jpg">
				<table width="100%"  border="0" cellspacing="0" cellpadding="0">
					<tr>
						<!-- 查詢結果上方按鈕起始 -->
						<td align=left>
						 <span id="Span21" runat="server">選取</span>：
							<input type=button ML="CB_全選" class="btn" value='全選' ID="SELALL_BTN1" onclick="setGridCheckBox('DataGrid', 'chkBox', 1);" runat="server">
							<input type=button ML="CB_取消全選" class="btn" value='取消全選' ID="SELNONE_BTN1" onclick="setGridCheckBox('DataGrid', 'chkBox', 0);" runat="server">
							<asp:button cssClass="btn" ML="CB_刪除選取" Text="刪除選取" id="DEL_BTN1" OnClientClick="return doDelete('DataGrid', 'chkBox', 'CHECK')" OnClick="DEL_BTN_Click" runat="server" />
						</td>
						<!-- 查詢結果上方按鈕結束 -->

						<!-- 分頁字串起始 -->
						<td align="right" nowrap>
						</td>
						<!-- 分頁字串結束 -->
					</tr>
				</table>
			</td>
			<td width="11"><img src="<%=WebCommon.CommonPath%>images/index_tb3.jpg" width="11" height="38"></td>
		</tr>
			<tr>
			<td width="12" background="<%=WebCommon.CommonPath%>images/index_tb8.jpg">&nbsp;</td>
			<td width="100%" align="right">
				<uc2:PageControl ID="PC" runat="server" />
			</td>
			<td width="13" background="<%=WebCommon.CommonPath%>images/index_tb4.jpg">&nbsp;</td>
		</tr>
		<tr>
			<td width="12" background="<%=WebCommon.CommonPath%>images/index_tb8.jpg">&nbsp;</td>
			<td width="100%">
				<!-- 查詢結果功能畫面起始 -->
				<div id="grid-scroll" style="overflow-x:auto;width:100%;">
				<asp:GridView id="DataGrid" Runat="server" Width="100%" BorderWidth="1"
				CellPadding="2" CellSpacing="0" CssClass="sortable" AlternatingRowStyle-CssClass="tdWhite" RowStyle-CssClass="tdGrayLight"
				AllowPaging="false" AutoGenerateColumns="False" AllowSorting="false" DataKeyNames="PKNO,ACNT,OCCUP">
					<HeaderStyle HorizontalAlign="Center" CssClass="mtbGreenBg" />
					<RowStyle HorizontalAlign="Left" />
					<Columns>
						<asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" ItemStyle-Width="20px">
							<ItemTemplate><input type=checkbox id="chkBox" runat="server"/></ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" ItemStyle-Width="20px">
							<ItemTemplate><asp:label id="edit" Text="編" ML="CL_編" runat="server"/></ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" ItemStyle-Width="20px">
	                        <ItemTemplate><asp:label id="detail" Text="詳" ML="CL_詳" runat="server"/></ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ACNT" HeaderText="PL_帳號"></asp:BoundField>                        
                        <asp:BoundField DataField="CH_NAME" HeaderText="PL_中文姓名"></asp:BoundField>
                        <asp:BoundField DataField="ENGNAME" HeaderText="PL_英文姓名"></asp:BoundField>
                        <asp:BoundField DataField="EMAIL" HeaderText="PL_電子信箱"></asp:BoundField>
                        <asp:BoundField DataField="DEP_NAME" HeaderText="PL_單位名稱(原聘任)"></asp:BoundField>
                        <asp:BoundField DataField="UNIT_DIRECTOR_NAME" HeaderText="PL_單位主管(原聘任)"></asp:BoundField>
                        <asp:BoundField DataField="JOB_TITLE" HeaderText="PL_職稱名稱"></asp:BoundField>
                        <asp:BoundField DataField="OCCUP" HeaderText="PL_專兼職"></asp:BoundField>
					</Columns>
					<EmptyDataTemplate>
						<label class="txtRed">查無符合資料!!</label>
					</EmptyDataTemplate>
				</asp:GridView>
				</div>
				<!-- 查詢結果功能畫面結束 -->
			</td>
			<td width="13" background="<%=WebCommon.CommonPath%>images/index_tb4.jpg">&nbsp;</td>
		</tr>
		<tr>
			<td width="12" background="<%=WebCommon.CommonPath%>images/index_tb8.jpg">&nbsp;</td>
			<td width="100%" align="right">
				<uc2:PageControl ID="PC2" runat="server" />
			</td>
			<td width="13" background="<%=WebCommon.CommonPath%>images/index_tb4.jpg">&nbsp;</td>
		</tr>
		<tr>
			<td width="12" background="<%=WebCommon.CommonPath%>images/index_tb8.jpg">&nbsp;</td>
			<td width="100%">
				<table width="100%"  border="0" cellspacing="0" cellpadding="0">
					<tr>
						<!-- 查詢結果下方按鈕起始 -->
						<td align=left>
						    <span id="Span31" runat="server">選取</span>：
							<input type=button class="btn" value='全選' ML="CB_全選" ID="SELALL_BTN2" onclick="setGridCheckBox('DataGrid', 'chkBox', 1);" runat="server"/>
							<input type=button class="btn" value='取消全選' ML="CB_取消全選" ID="SELNONE_BTN2" onclick="setGridCheckBox('DataGrid', 'chkBox', 0);" runat="server"/>
							<asp:button cssClass="btn" text='刪除選取' ML="CB_刪除選取" id="DEL_BTN2" OnClientClick="return doDelete('DataGrid', 'chkBox', 'CHECK')" OnClick="DEL_BTN_Click" runat="server" />
						</td>
						<!-- 查詢結果下方按鈕結束 -->

						<!-- 分頁字串起始 -->
						<td align="right" nowrap>
						</td>
						<!-- 分頁字串結束 -->
					</tr>
				</table>
			</td>
			<td width="13" background="<%=WebCommon.CommonPath%>images/index_tb4.jpg">&nbsp;</td>
		</tr>
		<tr>
			<td width="12"><img src="<%=WebCommon.CommonPath%>images/index_tb7.jpg" width="12" height="43"></td>
			<td width="100%" valign="top" background="<%=WebCommon.CommonPath%>images/index_tb6.jpg"></td>
			<td width="13"><img src="<%=WebCommon.CommonPath%>images/index_tb5.jpg" width="11" height="43"></td>
		</tr>
	</table>
	<br />
	<!-- 查詢結果畫面結束 -->
	<asp:LinkButton ID="ReQuery" runat="server"/>
	
	<script>
        document.write("<font color=\"white\">" + document.lastModified + "</font>");
        document.write("<font color=\"white\">" + _i(0, 'PERMISSION').value + "</font>");
        $(document).ready(page_init);
    </script>
	</ContentTemplate>
</asp:UpdatePanel>
</DIV>
<%--<CR:CrystalReportViewer ID="CrystalReportViewer" runat="server" AutoDataBind="True"  
HasCrystalLogo="False" BestFitPage="True" PrintMode="ActiveX" 
HasRefreshButton="True" ReuseParameterValuesOnRefresh="True" ShowAllPageIds="True" ReportSourceID="CrystalReportSource"/>
<CR:CrystalReportSource ID="CrystalReportSource"  runat="server" />--%>
</form>
</body>
</html>