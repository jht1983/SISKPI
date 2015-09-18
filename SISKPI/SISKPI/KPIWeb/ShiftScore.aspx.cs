﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using SIS.DataAccess;
using System.Text;
using System.IO;

namespace SISKPI.KPIWeb {

    /// <summary>
    /// 值际平均得分报表
    /// </summary>
	public partial class ShiftScore : System.Web.UI.Page {

		#region 重写方法

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if (!IsPostBack) {
				DateTime StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
				DateTime EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
				if (StartDate == EndDate) {
					StartDate = StartDate.AddMonths(-1);
				}
				txtStartDate.Text = StartDate.ToString("yyyy-MM-dd");
                txtEndDate.Text = EndDate.ToString("yyyy-MM-dd");
				DataBind();
			}
		}

		public override void DataBind() {
			using (ReportDal DataAccess = new ReportDal()) {
				String YearMonth = "";// txtYearMonth.Text;
				String SpecialField = Request.Params["SpecialField"];
				if (!string.IsNullOrEmpty(YearMonth) && (YearMonth.Length > 6)) YearMonth = YearMonth.Substring(0, 4) + YearMonth.Substring(5, 2);
                DateTime StartDate = Convert.ToDateTime(txtStartDate.Text);
                DateTime EndDate = Convert.ToDateTime(txtEndDate.Text);
				ScoreRepeater.DataSource = DataAccess.GetShiftScore(StartDate, EndDate, SpecialField);
			}
			base.DataBind();
			ColSpan();
		}

		#endregion

		#region 私有方法

		private void ColSpan() {
			for (int i = ScoreRepeater.Items.Count - 1; i > 0; i--) {
				HtmlTableCell oCell_previous = ScoreRepeater.Items[i - 1].FindControl("UnitName") as HtmlTableCell;
				HtmlTableCell oCell = ScoreRepeater.Items[i].FindControl("UnitName") as HtmlTableCell;
				oCell.RowSpan = (oCell.RowSpan == -1) ? 1 : oCell.RowSpan;
				oCell_previous.RowSpan = (oCell_previous.RowSpan == -1) ? 1 : oCell_previous.RowSpan;
				if (oCell.InnerText == oCell_previous.InnerText) {
					oCell.Visible = false;
					oCell_previous.RowSpan += oCell.RowSpan;
				}
			}
		}

		#endregion

        #region 重写方法

        protected String GetShiftName(Object Shift) {
            String[] Shifts = { "", "一值", "二值", "三值", "四值", "五值", 
                                    "六值", "七值", "八值", "九值", "十值" };
            int Index = Convert.ToInt32(Shift);
            if (Index <= 0) return "";
            return Shifts[Index];
        }

        protected String GetRank(Object Rank) {
            String[] Ranks = { "", "第一名", "第二名", "第三名", "第四名", "第五名", 
                                   "第六名", "第七名", "第八名", "第九名", "第十名" };
            int Index = Convert.ToInt32(Rank);
            if (Index <= 0) return "";
            return Ranks[Index];
        }

        #endregion

        #region 事件

        protected void btnSearch_Click(Object sender, EventArgs e) {
			DataBind();
		}

		protected void btnExport_Click(Object sender, EventArgs e) {
			Response.ContentEncoding = Encoding.UTF8;
			Response.ContentType = "application/ms-excel";
			Response.Charset = "utf-8";
			Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode("值际平均得分报表.xls", System.Text.Encoding.UTF8).ToString());
			StringWriter sw = new StringWriter();
			HtmlTextWriter htw = new HtmlTextWriter(sw);
			ScoreRepeater.RenderControl(htw);
			Response.Write(sw.ToString());
			Response.Flush();
			Response.End();
			sw.Close();
			sw.Dispose();
		}

		#endregion
	}
}