﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="KPI_CycleConfig.aspx.cs"
    Inherits="SISKPI.KPI_CycleConfig" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" id="mainWindow">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>SISKPI</title>
    <link href="../CSS/MainCSS.css" type="text/css" rel="stylesheet" />
    <base target="_self" />
    <script src="../js/jquery-1.3.2.js" type="text/javascript"></script>
    <script src="../js/ProcessBar.js" type="text/javascript"></script>
    <script src="../js/Common.js" type="text/javascript"></script>
    <%--<script src="../js/DatePicker/WdatePicker.js" type="text/javascript" defer="defer"></script>--%>
    <script src="../js/GridViewColor.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            call();
        });

        function call() {
            SetTableWidth('table1');
            SetDivWidth('div1');
            SetDivWidth('divtag');
            SetTableWidth('table2');
            //SetTableWidth('table3');
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" style="width: 100%">
    <div style="width: 100%; text-align: center;">
        <table style="width: 100%" align="center" class="table" id="table1">
            <tr style="width: 100%">
                <td align="center" style="width: 100%">
                    <asp:Image ID="Image2" runat="server" Height="30px" ImageUrl="../imgs/logo.gif" />
                    <asp:Label ID="lblTitle" runat="server" Class="title" Text="周期信息"></asp:Label>
                </td>
            </tr>
            <tr style="width: 100%">
                <td style="width: 100%" align="center">
                    <div style="width: 100%">
                        <fieldset class="field_info" style="width: 95%;">
                            <legend>岗位信息</legend>
                            <table class="table" style="width: 98%;">
                                <tr align="left" style="width: 100%;">
                                    <td align="left" style="width: 100%;">
                                        <asp:Label ID="Label1" runat="server" Text="周期定义如下【TN:按班周期计算; TD:按日周期计算; Tx: 按分钟周期计算。】"></asp:Label>
                                    </td>
                                </tr>
                                <tr align="left" style="width: 100%;">
                                    <td align="left" style="width: 100%;">
                                        <asp:GridView ID="gvCycle" CssClass="GridViewStyle" Width="98%" runat="server" AutoGenerateColumns="False"
                                            EmptyDataText="没有满足条件的数据" AllowPaging="False" OnRowCancelingEdit="gvCycle_RowCancelingEdit"
                                            OnRowEditing="gvCycle_RowEditing" OnRowUpdating="gvCycle_RowUpdating" OnRowDataBound="gvCycle_RowDataBound"
                                            OnRowCommand="gvCycle_RowCommand">
                                            <FooterStyle CssClass="GridViewFooterStyle" />
                                            <RowStyle CssClass="GridViewRowStyle" />
                                            <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
                                            <PagerStyle CssClass="GridViewPagerStyle" />
                                            <AlternatingRowStyle CssClass="GridViewAlternatingRowStyle" />
                                            <HeaderStyle CssClass="GridViewHeaderStyle" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="序号">
                                                    <ItemTemplate>
                                                        <input type="hidden" runat="server" id="Cycleid" value='<%# Eval("CycleID").ToString()%>' />
                                                        <%#  Container.DataItemIndex + 1%>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="40px" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="CycleName" HeaderText="名称" />
                                                <asp:BoundField DataField="CycleDesc" HeaderText="描述" />
                                                <asp:BoundField DataField="CycleValue" HeaderText="周期[分钟]" />
                                                <asp:BoundField DataField="CycleNote" HeaderText="备注" />
                                                <asp:CommandField HeaderText="配置" ItemStyle-HorizontalAlign="Center" ShowEditButton="True"
                                                    EditText="编辑" />
                                                <asp:TemplateField HeaderText="配置" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lb_delete" runat="server" Text="删除" CommandName="dataDelete"
                                                            CommandArgument='<%# Eval("CycleID") %>' />
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="60px" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </td>
                                </tr>
                                <tr style="width: 100%">
                                    <td width="100%" align="center">
                                        <asp:Button ID="btnAddCycle" runat="server" Text="新增周期信息" Width="100px" OnClick="btnAddCycle_Click" />
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <div id="Lay1" style="z-index: 100; visibility: hidden; left: 1%; width: 260px; cursor: crosshair;
        position: absolute; top: -20px; height: 14%; background-color: transparent; text-align: center">
        <br />
        <b></b>
        <table align="center" style="width: 260px; height: 45px" class="BoderTable">
            <tr>
                <td>
                    <div>
                        <font color="#800080" size="2" style="font-weight: bold">配置正在执行，请稍候...</font></div>
                    <div style="border-right: black 1px solid; padding-right: 2px; border-top: black 1px solid;
                        padding-left: 2px; font-size: 8pt; padding-bottom: 2px; border-left: black 1px solid;
                        padding-top: 2px; border-bottom: black 1px solid">
                        <span id="progress1">&nbsp;</span> <span id="progress2">&nbsp;</span> <span id="progress3">
                            &nbsp;</span> <span id="progress4">&nbsp;</span> <span id="progress5">&nbsp;</span>
                        <span id="progress6">&nbsp;</span> <span id="progress7">&nbsp;</span> <span id="progress8">
                            &nbsp;</span> <span id="progress9">&nbsp;</span> <span id="progress10">&nbsp;</span>
                        <span id="progress11">&nbsp;</span> <span id="progress12">&nbsp;</span> <span id="progress13">
                            &nbsp;</span> <span id="progress14">&nbsp;</span> <span id="progress15">&nbsp;</span>
                        <span id="progress16">&nbsp;</span> <span id="progress17">&nbsp;</span> <span id="progress18">
                            &nbsp;</span> <span id="progress19">&nbsp;</span> <span id="progress20">&nbsp;</span>
                        <span id="progress21">&nbsp;</span> <span id="progress22">&nbsp;</span> <span id="progress23">
                            &nbsp;</span> <span id="progress24">&nbsp;</span> <span id="progress25">&nbsp;</span>
                        <span id="progress26">&nbsp;</span> <span id="progress27">&nbsp;</span> <span id="progress28">
                            &nbsp;</span> <span id="progress29">&nbsp;</span> <span id="progress30">&nbsp;</span>
                        <span id="progress31">&nbsp;</span> <span id="progress32">&nbsp;</span> <span id="progress33">
                            &nbsp;</span> <span id="progress34">&nbsp;</span> <span id="progress35">&nbsp;</span>
                        <span id="progress36">&nbsp;</span> <span id="progress37">&nbsp;</span> <span id="progress38">
                            &nbsp;</span> <span id="progress39">&nbsp;</span> <span id="progress40">&nbsp;</span>
                        <span id="progress41">&nbsp;</span> <span id="progress42">&nbsp;</span> <span id="progress43">
                            &nbsp;</span> <span id="progress44">&nbsp;</span> <span id="progress45">&nbsp;</span>
                        <span id="progress46">&nbsp;</span> <span id="progress47">&nbsp;</span> <span id="progress48">
                            &nbsp;</span> <span id="progress49">&nbsp;</span> <span id="progress50">&nbsp;</span>
                        <span id="progress51">&nbsp;</span> <span id="progress52">&nbsp;</span> <span id="progress53">
                            &nbsp;</span> <span id="progress54">&nbsp;</span> <span id="progress55">&nbsp;</span>
                        <span id="progress56">&nbsp;</span> <span id="progress57">&nbsp;</span> <span id="progress58">
                            &nbsp;</span> <span id="progress59">&nbsp;</span> <span id="progress60">&nbsp;</span>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
