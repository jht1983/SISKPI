﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SIS.DataAccess;
using SIS.DataEntity;
using System.Web.UI.HtmlControls;

namespace SISKPI.KPIWeb {

    /// <summary>
    /// 综合煤耗
    /// </summary>
    public partial class CoalConsumptionPage : System.Web.UI.Page {

        #region 重写方法

        protected override void OnLoad(EventArgs e) {
            if (!IsPostBack) {
                DataBind();
            }
            base.OnLoad(e);
        }

        public override void DataBind() {
            string SpecialField = Request.Params["ecweb"];
            string queryTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:00");
            String PlantCode = Request.Params["plantcode"];
            String UnitCode = Request.Params["unitcode"];
            String PlantID = KPI_PlantDal.GetPlantIDByCode(PlantCode);
            String UnitID = KPI_UnitDal.GetUnitIDByCode(UnitCode);
            ECValueRepeater.DataSource = ECSSSnapshotDal.GetSearchList(SpecialField, queryTime, PlantID, "");
            base.DataBind();
            ColSpan();


            //String CurrentTimeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:00");
            nowtime.Text = "计算时间：" + queryTime;
            nowshift.Text = "轮班值次：";
            if (!String.IsNullOrWhiteSpace(UnitID)) {
                string workid = KPI_UnitDal.GetWorkIDByID(UnitID);
                string ShiftName = "";
                string PeriodName = "";
                string StartTime = "";
                string EndTime = "";
                bool bGood = KPI_WorkDal.GetShiftAndPeriod(workid, queryTime, ref ShiftName,
                    ref PeriodName, ref StartTime, ref EndTime);
                nowshift.Text = "轮班值次：" + ShiftName + "值";
            }
        }

        #endregion

        #region 事件

        protected void Timer1_Tick(object sender, EventArgs e) {
            DataBind();
        }       

        #endregion

        #region 私有方法

        protected String GetECName(Object Value) {
            String ECName = Convert.ToString(Value);
            if (String.IsNullOrWhiteSpace(ECName)) return "";
            return HttpUtility.UrlEncode(ECName);
        }

        protected String GetClass(Object ECID) {
            String  Value = Convert.ToString(ECID);
            if ((Value == "0090")||(Value=="0091")) {
                return "Alarm";
            }
            return "";
        }

        private void ColSpan() {
            for (int i = ECValueRepeater.Items.Count - 1; i > 0; i--) {
                HtmlTableCell oCell_previous = ECValueRepeater.Items[i - 1].FindControl("UnitName") as HtmlTableCell;
                HtmlTableCell oCell = ECValueRepeater.Items[i].FindControl("UnitName") as HtmlTableCell;
                oCell.RowSpan = (oCell.RowSpan == -1) ? 1 : oCell.RowSpan;
                oCell_previous.RowSpan = (oCell_previous.RowSpan == -1) ? 1 : oCell_previous.RowSpan;
                if (oCell.InnerText == oCell_previous.InnerText) {
                    oCell.Visible = false;
                    oCell_previous.RowSpan += oCell.RowSpan;
                }
            }
        }

        #endregion
    }
}