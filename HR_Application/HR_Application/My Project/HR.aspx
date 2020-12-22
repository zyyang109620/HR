

<%@ Page Language="VB" AutoEventWireup="false" CodeFile="POS2010_01.aspx.vb" Inherits="POS2010_01" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Src="~/UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc2" %>
<%@ Register Src="~/UserControl/PageInitControl.ascx" TagName="PageInitControl" TagPrefix="uc1" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<uc1:PageInitControl ID="PIC" runat="server" />
	
	<title>�򥻤H����ƺ��@</title>
	<script type="text/javascript"> 
        var viewpage = "HR_edit.aspx";	//�s�譶���W��
        //var printpage = "HR_print.aspx";	//�C�L�����W��

        //=== �w�q�z����ް}�C ===
        var columnIndexAry = [3, 4, 5, 6, 7, 8, 9];

        //=== �w�q�z�襤��W�ٰ}�C ===
        var columnNameAry = ['�b��', '�m�W', '�^��W��', '�q�l�H�c', '���W��(��u��)', '���D��(��u��)', '�X�ͤ��'];
        var columnCodeAry = ['ACNT', 'CH_NAME', 'ENGNAME', 'EMAIL', 'DEP_NAME', 'UNIT_DIRECTOR_NAME', 'BIRTH_DATE'];
        var recordName = "POS2010_01"

        /** 
        ������l�ɳB�z Client ���ʧ@ 
        */
        function page_init() {
            page_init_start();

            var keyValue = "";
            //���GCHOICE=1�A�h��GCHOICE=2 [����]
            keyValue = "CHOICE=1";
            //�_�l���GROOT_DEP_CODE=���N�X [�D����]
            //keyValue+="&ROOT_DEP_CODE=0001";
            //�^�����G�^�����A�Ѧ�ORGT010 [�D����]
            //keyValue+="&RETURNFIELD=UNIT_LEVEL|UNIT_TYPE";
            //�n�a�^�����ID�G���ID [����]
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
        �d�ߥ\��ɩI�s 
        */
        function doQuery() {
            /** ��ܸ�ƳB�z */
            Message.showProcess();

            //=== �۩w�ˬd ===
            //=== ����ˮ֤γ]�w, �����~�B�z�覡�� Form.errAppend(Message) �֭p���~�T�� ===
            //if (Form.getInput("EDIT", "SYS_CD") == "")
            //	Form.errAppend("�t�νs�����i�ť�!!");
            /** ================ */

            //=== �ˬd�O�_�����~�T�� ===
            var needSend = valideMessage("Q_");

            //=== �o�Ϳ��~���B�z ===
            if (!needSend) {
                Message.hideProcess();
                return false;
            }

            top.rowKey = null;

            return true;
        }


        /** 
        �M�Ŭd�ߪ����
        */
        function clearQueryForm() {
            for (var i = 0; i < document.forms[0].length; i++) {
                //=== �ȳB�z Q_ ������ ===
                if (_i(0, i).name.substring(0, 2) != "Q_")
                    continue;

                //=== �U�Թw�]����Ĥ@�� ===
                if (_i(0, i).type == 'select-one')
                    _i(0, i).selectedIndex = 0;
                //=== ��l�ƤJ���عw�]�ť� ===
                else if (_i(0, i).type != 'radio' && _i(0, i).type != 'checkbox')
                    _i(0, i).value = '';
                //=== Radio �� CheckBox �w�]���ֿ� ===
                else
                    _i(0, i).checked = false;

                //=== �B�z default value ===
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
        /*�}��--��ܾ𪬲�´���*/
        function SelectOrgno1() {
            var keyValue = "";
            //���GCHOICE=1�A�h��GCHOICE=2 [����]
            keyValue = "CHOICE=1";
            //�_�l���GROOT_DEP_CODE=���N�X [�D����]
            //keyValue+="&ROOT_DEP_CODE=0001";
            //�^�����G�^�����A�Ѧ�ORGT010 [�D����]
            //keyValue+="&RETURNFIELD=UNIT_LEVEL|UNIT_TYPE";
            //�n�a�^�����ID�G���ID [����]
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
	<!-- �d�߼��D�e���_�l -->
	<table id="QTable1" height="41" width=329 border="0" cellpadding="0" cellspacing="0">
		<tr>
			<td background="<%=WebCommon.CommonPath%>images/title1.jpg">
				<table width="100%"  border="0" cellspacing="0" cellpadding="0">
					<tr>
						<td width="15%">&nbsp;</td>
						<td width="85%" class="title" nowrap><span id="Span1" ML="CL_�d�ߵe��" runat="server">�d�ߵe��</span></td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	<!-- �d�߼��D�e������ -->
	
	<!-- �d�ߵe���}�l -->
	<table id="QTable2" width="96%" border="0" align="center" cellpadding="0" cellspacing="0">
		<tr>
			<td width="12"><img src="<%=WebCommon.CommonPath%>images/table_1_l_1.jpg" width="12" height="38"></td>
			<td width="100%" align="left" valign="bottom" background="<%=WebCommon.CommonPath%>images/table_1_bg_1.jpg">
				<!-- �d�ߵe���W����s�_�l -->
				<table width="100%" border="0" cellspacing="0" cellpadding="0">
					<tr>
						<td height="30" align=right>
						
							<%--<input type="button" name="FILTER_BTN1" class="btn" ML="CB_���z��" value='���z��' onkeypress='doFilter();'onclick='doFilter();'>--%>
							<input type="button" ID="ADD_BTN1" class="btn" value='�s  �W' ML="CB_�s�W" onclick="doAdd1_2();"  runat="server"/>
							<input type="button" ID="QCLEAR_BTN1" class="btn" ML="CB_�M��" value='�M  ��' onclick="clearQueryForm();" runat="server" onserverclick="QCLEAR_BTN1_ServerClick"/>
							<asp:Button ID="QUERY_BTN1" cssClass="btn" ML="CB_�d��" text="�d  ��" OnClientClick="return doQuery()" OnClick="QUERY_BTN_Click" runat="server" />
							<input type="button" class="btn" ML="CB_�ץX����" value='�ץX����' ID="EXPALL_BTN1" onclick='doExport()' runat="server"/>
						</td>
					</tr>
				</table>
				<!-- �d�ߵe���W����s���� -->
			</td>
			<td width="11"><img src="<%=WebCommon.CommonPath%>images/table_1_r_1.jpg" width="11" height="38"></td>
		</tr>
		<tr>
			<td width="11" background="<%=WebCommon.CommonPath%>images/table_1_l_2.jpg"></td>
			<td background="<%=WebCommon.CommonPath%>images/table_1_bg_2.jpg">
				<!-- �d�ߵe���_�l -->
				<table width="100%" cellspacing="0" cellpadding="0" class="tableCss">
					<tr>
						<td align='right'><span id="Span2" ML="PL_�m�W" runat="server">�m�W</span>�G</td>
						<td><asp:textbox id='Q_CH_NAME' CssClass='select' runat='server' /></td>
						<td align='right'><span id="Span3" ML="PL_�b��" runat="server">�b��</span>�G</td>
						<td><asp:textbox id='Q_ACNT' CssClass='select' runat='server' /></td>
					</tr>					 
					<tr>
					    <td align='right'><span id="Span7" ML="PL_���W��(��u��)" runat="server">���W��(��u��)</span>�G</td>
						<td>
                             <%--<asp:HiddenField ID="Q_UPPER_DEP_CODE" runat="server" />--%>
                            <asp:DropDownList ID="Q_UPPER_DEP_CODE" runat="server"></asp:DropDownList>
                            <asp:textbox id='UPPER_DEP_NAME' CssClass='select' runat='server' ValidationGroup="EQ" Enabled="false" Visible="false" />
						    <input type="button" id="ORG_BTN" value="��´�}��" class="btn" runat="server" visible="false" />
						</td>
					    <td align='right'><span id="Span5" ML="PL_�b¾���A" runat="server">�b¾���A</span>�G</td>
						<td>
                            <asp:DropDownList ID="Q_OJOB_STATUS" runat="server"></asp:DropDownList>
						</td>
					</tr>
										<tr>
					    <td align='right'><span id="Span6" ML="PL_���W��(��u��)" runat="server">¾��</span>�G</td>
						<td>
                             <%--<asp:HiddenField ID="Q_UPPER_DEP_CODE" runat="server" />--%>
                            <asp:DropDownList ID="DropDownList1" runat="server"></asp:DropDownList>
                            <asp:textbox id='Textbox1' CssClass='select' runat='server' ValidationGroup="EQ" Enabled="false" Visible="false" />
						    <input type="button" id="Button1" value="��´�}��" class="btn" runat="server" visible="false" />
						</td>
				
					</tr>
				</table>
			</td>
			<td width="13" background="<%=WebCommon.CommonPath%>images/table_1_r_2.jpg">&nbsp;</td>
		</tr>
		<tr>
			<td width="12"><img src="<%=WebCommon.CommonPath%>images/table_1_l_3.jpg" width="12" height="25"></td>
			<!-- �d�ߵe���U����s�_�l -->
			<td width="100%" valign="top" align="right" background="<%=WebCommon.CommonPath%>images/table_1_bg_3.jpg">
				&nbsp;
				<div style="display:none">
				<input type="button" ID="FILTER_BTN2" class="btn" ML="CB_���z��" value='���z��' onclick='doFilter();' runat="server"/>
				<input type="button" ID="ADD_BTN2" class="btn" value='�s  �W' ML="CB_�s�W" onclick="doAdd1_2();"  runat="server"/>
				<input type="button" ID="QCLEAR_BTN2" class="btn" ML="CB_�M��" value='�M  ��' onclick="clearQueryForm();" runat="server"/>
				<asp:Button ID="QUERY_BTN2" cssClass="btn" ML="CB_�d��" text="�d  ��" OnClientClick="return doQuery()" OnClick="QUERY_BTN_Click" runat="server" />
				<input type="button" class="btn" ML="CB_�ץX����" value='�ץX����' ID="EXPALL_BTN2" onclick='doExportNoFilter()' runat="server"/>
				</div>
			</td>
			<!-- �d�ߵe���U����s���� -->
			<td width="13"><img src="<%=WebCommon.CommonPath%>images/table_1_r_3.jpg" width="11" height="25"></td>
		</tr>
	</table>
	<!-- �d�ߵe������ -->
	
	<table width="100%"  border="0" align="center" cellpadding="0" cellspacing="0">
		<tr>
			<td align="left" valign="top">
				<a href="JavaScript:showHideQtable();">
				<img src="<%=WebCommon.CommonPath%>images/up_windows_btn.jpg" width="100%" height="6" border="0"></a>
			</td>
		</tr>
	</table>

	<!-- ���D�e���_�l -->
	
	<br />
	<!-- �d�ߵ��G�e������ -->
	<asp:LinkButton ID="ReQuery" runat="server"/>
	
	<script>
        document.write("<font color=\"white\">" + document.lastModified + "</font>");
        document.write("<font color=\"white\">" + _i(0, 'PERMISSION').value + "</font>");
        $(document).ready(page_init);
    </script>
	</ContentTemplate>
</asp:UpdatePanel>
</DIV>
</form>
</body>
</html>