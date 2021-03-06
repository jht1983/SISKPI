﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Data.OleDb;


using SIS.Assistant;
using SIS.Loger;
using SIS.DBControl;
using SIS.DataAccess;
using SIS.DataEntity;
using SIS.Exceler;


namespace SISKPI
{
    public partial class KPI_SubTeamBatch : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Excel
                //btnExcelExport.Attributes.Add("onclick", "setDivPos('Lay1');Lay1.style.visibility=''; progress_update();");
                btnExcelImport.Attributes.Add("onclick", "setDivPos('Lay1');Lay1.style.visibility=''; progress_update();");
                                
                ddlExcelMode.Items.Add(new ListItem("创建", "0"));
                //ddlExcelMode.Items.Add(new ListItem("创建并修改", "1"));
                //ddlExcelMode.Items.Add(new ListItem("修改", "2"));
                //ddlExcelMode.Items.Add(new ListItem("删除", "3"));
            }


        }

        #region Excel标签点操作

        public static void ExportToSpreadsheet(DataTable dtData, string name)
        {
            DocExport docEexport = new DocExport();

            docEexport.Dt = dtData; //GridViewHelper.GridView2DataTable(gvValue);

            docEexport.Export(name);

            //System.Web.UI.WebControls.DataGrid dgExport = null;
            //// 当前对话 
            //System.Web.HttpContext curContext = System.Web.HttpContext.Current;
            //// IO用于导出并返回excel文件 
            //System.IO.StringWriter strWriter = null;
            //System.Web.UI.HtmlTextWriter htmlWriter = null;

            //if (dtData != null)
            //{
            //    // 设置编码和附件格式 
            //    curContext.Response.ContentType = "application/vnd.ms-excel";
                
            //    //通过超链接跳转到下载页面，解决弹出该页面的aspx文件要求下载的问题
            //    //指定下载文件名称
            //    curContext.Response.AddHeader("Content-Disposition", "attachment;filename=KPIRealTag.xls");

            //    curContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
            //    curContext.Response.Charset = "GB2312";

            //    // 导出excel文件 
            //    strWriter = new System.IO.StringWriter();
            //    htmlWriter = new System.Web.UI.HtmlTextWriter(strWriter);

            //    // 为了解决dgData中可能进行了分页的情况，需要重新定义一个无分页的DataGrid 
            //    dgExport = new System.Web.UI.WebControls.DataGrid();
            //    dgExport.DataSource = dtData.DefaultView;
            //    dgExport.AllowPaging = false;
            //    dgExport.DataBind();

            //    // 返回客户端 
            //    dgExport.RenderControl(htmlWriter);
            //    curContext.Response.Write(strWriter.ToString());
            //    curContext.Response.End();
            //}

        }
        
        //导出
        protected void btnExcelExport_Click(object sender, EventArgs e)
        {
            string strExcelFile = "SIS班组信息表";

            try
            {
                System.Data.DataTable dt = KPI_TeamDal.GetPersonListForExcel();

                if (dt == null)
                    return;

                ExportToSpreadsheet(dt, strExcelFile);    

            }
            catch (Exception ee)
            {
                MessageBox.popupClientMessage(this.Page, "导出信息错误！" + ee.Message, "call();");

                return;
            }
       
            return;
        }

        //导入
        protected void btnExcelImport_Click(object sender, EventArgs e)
        {
            if (ddlExcelMode.Value == "")
            {
                MessageBox.popupClientMessage(this.Page, "请选择Excel表操作方式！", "call();");
        
                return;
            }

            if (!fuExcel.HasFile)
            {
                MessageBox.popupClientMessage(this.Page, "请选择Excel文件！", "call();");

                return;
            }
            
            string IsXls = System.IO.Path.GetExtension(fuExcel.FileName).ToString().ToLower();
            if (IsXls != ".xls")
            {
                MessageBox.popupClientMessage(this.Page, "只允许选择Excel文件！", "call();");

                return;
            }

             try
            {
                //删除临时文件夹的文件
                System.IO.DirectoryInfo path = new System.IO.DirectoryInfo(Server.MapPath("~\\excel\\"));
                foreach (System.IO.FileInfo f in path.GetFiles())
                {
                    f.Delete();
                }  

                //获取Execle文件名  DateTime日期函数   
                string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + fuExcel.FileName;
                //Server.MapPath 获得虚拟服务器相对路径
                string savePath = Server.MapPath(("~\\excel\\") + filename);
                //SaveAs 将上传的文件内容保存在服务器上
                fuExcel.SaveAs(savePath);

                string sheetname = tbxSheet.Text;
                //
                DataSet ds = ImportExeclToDataSet(savePath, filename, sheetname);
            
                int nDo = int.Parse(ddlExcelMode.Value);
                 
                switch (nDo)
                {
                    case 0:
                        //创建
                        ImportFromExcelToCreate(ds);

                        break;

                    //case 1:
                    //    //创建并修改
                    //    ImportFromExcelToCreateAndModify(ds);
                    //    break;

                    //case 2:
                    //    //修改
                    //    ImportFromExcelToModify(ds);
                    //    break;

                    //case 3:
                    //    //删除
                    //    ImportFromExcelToDelete(ds);
                    //    break;

                    default:
                        break;
                }

            }
             catch (Exception ee)
             {
                 MessageBox.popupClientMessage(this.Page, "检查Excle文件！" + ee.Message, "call();");

                 return;
             }
       

        }

        protected DataSet ImportExeclToDataSet(string filenameurl, string table, string sheetname)   
        {
            string strConn = "Provider=Microsoft.Jet.OleDb.4.0;" + "data source=" + filenameurl 
                                                              + ";Extended Properties='Excel 8.0; HDR=YES; IMEX=1'";

            OleDbConnection conn = new OleDbConnection(strConn);

            conn.Open();

            DataSet ds = new DataSet();

            OleDbDataAdapter odda = new OleDbDataAdapter("select * from [" + sheetname + "$]", conn);

            odda.Fill(ds, table);

            conn.Close();

            return ds;
        }
        
        protected bool ImportFromExcelToCreate(DataSet ds)
        {
            try
            {
                System.Data.DataTable dt = ds.Tables[0];

                int nAll = dt.Rows.Count;
                int nCreate = 0;
                int nExist = 0;

                foreach(System.Data.DataRow dr in dt.Rows)
                {
                    if (dr["SelectX"].ToString().ToLower() == "x")
                    {
                        //string PersonCode = dr["PersonCode"].ToString().Trim();

                        ////判断是否存在
                        //if (KPI_PersonDal.PersonCodeExists(PersonCode, ""))
                        //{
                        //    //MessageBox.popupClientMessage(this.Page, " 该机组的输出标签已存在！", "call();");
                        //    nExist += 1;
                        //    continue;
                        //}

                        //main tag
                        string keyid = PageControl.GetGuid();
                        KPI_TeamEntity mEntity = new KPI_TeamEntity();
                        mEntity.TeamID = keyid;
                        mEntity.PlantID = KPI_PlantDal.GetPlantID(dr["PlantName"].ToString().Trim());
                        mEntity.ShiftID = KPI_ShiftDal.GetShiftID(dr["ShiftName"].ToString().Trim());
                        mEntity.PersonID = KPI_PersonDal.GetPersonID(dr["PersonCode"].ToString().Trim());
                        mEntity.PositionID = KPI_PersonDal.GetPositionID(mEntity.PersonID);
                        //PersonName,无多大意义
                        mEntity.PositionID = KPI_PositionDal.GetPositionID(dr["PositionName"].ToString().Trim());
                        mEntity.TeamNote = dr["TeamNote"].ToString().Trim();

                        KPI_TeamDal.Insert(mEntity);

                        nCreate += 1;

                    }


                }    
                
                string strInfor = "标签点总数为：{0}个, 创建成功:{1}个，已存在标签点: {2}个。";
                strInfor = string.Format(strInfor, nAll, nCreate, nExist);

                MessageBox.popupClientMessage(this.Page, strInfor, "call();");

                return true;
                                
            }
            catch(Exception ee)
            {
                //
                MessageBox.popupClientMessage(this.Page, ee.Message, "call();");

                return false;
            }              
        }
        
        #region no use code

        //protected bool ImportFromExcelToCreateAndModify(DataSet ds)
        //{
        //    try
        //    {
        //        System.Data.DataTable dt = ds.Tables[0];

        //        int nAll = dt.Rows.Count;
        //        int nCreate = 0;
        //        int nModify = 0;
        //        bool bExist = false;

        //        foreach (System.Data.DataRow dr in dt.Rows)
        //        {
        //            if (dr["SelectX"].ToString().ToLower() == "x")
        //            {
        //                string PersonCode = dr["PersonCode"].ToString().Trim();

        //                //判断是否存在
        //                bExist = KPI_PersonDal.PersonCodeExists(PersonCode, "");

        //                string keyid = PageControl.GetGuid();
        //                if (bExist)
        //                {
        //                    keyid = KPI_PersonDal.GetPersonID(PersonCode);

        //                    //MessageBox.popupClientMessage(this.Page, " 该机组的输出标签已存在！", "call();");
        //                    //nExist += 1;
        //                    //continue;
        //                }

        //                //main tag

        //                KPI_TeamEntity mEntity = new KPI_TeamEntity();
        //                mEntity.PersonID = keyid;
        //                mEntity.PositionID = KPI_PositionDal.GetPositionID(dr["PositionName"].ToString().Trim());
        //                mEntity.PersonCode = PersonCode;
        //                mEntity.PersonName = dr["PersonName"].ToString().Trim();
        //                mEntity.PersonDesc = dr["PersonDesc"].ToString().Trim();
        //                mEntity.PersonIsValid = dr["PersonIsValid"].ToString().Trim();
        //                mEntity.PersonNote = dr["PersonNote"].ToString().Trim();

        //                if (bExist)
        //                {
        //                    KPI_PersonDal.Update(mEntity);
        //                    nModify += 1;
        //                }
        //                else
        //                {
        //                    KPI_PersonDal.Insert(mEntity);
        //                    nCreate += 1;
        //                }                        

        //            }
        //        }

        //        string strInfor = "标签点总数为：{0}个, 创建成功:{1}个， 修改成功: {2}个。";
        //        strInfor = string.Format(strInfor, nAll, nCreate, nModify);

        //        MessageBox.popupClientMessage(this.Page, strInfor, "call();");

        //        return true;

        //    }
        //    catch (Exception ee)
        //    {
        //        //
        //        MessageBox.popupClientMessage(this.Page, ee.Message, "call();");

        //        return false;
        //    }
        //}

        //protected bool ImportFromExcelToModify(DataSet ds)
        //{
        //    try
        //    {
        //        System.Data.DataTable dt = ds.Tables[0];

        //        int nAll = dt.Rows.Count;
        //        int nModify = 0;
        //        int nNoExist = 0;

        //        foreach (System.Data.DataRow dr in dt.Rows)
        //        {
        //            if (dr["SelectX"].ToString().ToLower() == "x")
        //            {
        //                string PersonCode = dr["PersonCode"].ToString().Trim();

        //                //判断是否存在
        //                if (!KPI_PersonDal.PersonCodeExists(PersonCode, ""))
        //                {
        //                    //MessageBox.popupClientMessage(this.Page, " 该机组的输出标签已存在！", "call();");
        //                    nNoExist += 1;
        //                    continue;
        //                }

        //                //main tag
        //                string keyid = KPI_PersonDal.GetPersonID(PersonCode);

        //                KPI_TeamEntity mEntity = new KPI_TeamEntity();
        //                mEntity.PersonID = keyid;
        //                mEntity.PositionID = KPI_PositionDal.GetPositionID(dr["PositionName"].ToString().Trim());
        //                mEntity.PersonCode = PersonCode;
        //                mEntity.PersonName = dr["PersonName"].ToString().Trim();
        //                mEntity.PersonDesc = dr["PersonDesc"].ToString().Trim();
        //                mEntity.PersonIsValid = dr["PersonIsValid"].ToString().Trim();
        //                mEntity.PersonNote = dr["PersonNote"].ToString().Trim();
                        
        //                KPI_PersonDal.Update(mEntity);

        //                nModify += 1;
        //            }


        //        }

        //        string strInfor = "标签点总数为：{0}个, 修改成功:{1}个，不存在标签点: {2}个。";
        //        strInfor = string.Format(strInfor, nAll, nModify, nNoExist);

        //        MessageBox.popupClientMessage(this.Page, strInfor, "call();");

        //        return true;

        //    }
        //    catch (Exception ee)
        //    {
        //        //
        //        MessageBox.popupClientMessage(this.Page, ee.Message, "call();");

        //        return false;
        //    }
        //}

        //protected bool ImportFromExcelToDelete(DataSet ds)
        //{
        //    try
        //    {
        //        System.Data.DataTable dt = ds.Tables[0];

        //        int nAll = dt.Rows.Count;
        //        int nDelete = 0;
        //        int nEmpty = 0;

        //        foreach (System.Data.DataRow dr in dt.Rows)
        //        {
        //            if (dr["SelectX"].ToString().ToLower() == "x")
        //            {
        //                string PersonCode = dr["PersonCode"].ToString().Trim();

        //                //判断是否存在
        //                if (!KPI_PersonDal.PersonCodeExists(PersonCode, ""))
        //                {
        //                    //MessageBox.popupClientMessage(this.Page, " 该机组的输出标签已存在！", "call();");
        //                    nEmpty += 1;
        //                    continue;
        //                }
        //                else
        //                {
        //                    //main tag
        //                    //string RealID = KPI_PersonDal.GetRealID(PersonCode);

        //                    KPI_TeamEntity mEntity = new KPI_TeamEntity();
        //                    mEntity.PersonID = KPI_PersonDal.GetPersonID(PersonCode);

        //                    KPI_PersonDal.Delete(mEntity);
        //                    nDelete += 1;
        //                }
        //            }
        //        }

        //        string strInfor = "标签点总数为：{0}个, 删除成功:{1}个，空标签点: {2}个。";
        //        strInfor = string.Format(strInfor, nAll, nDelete, nEmpty);

        //        MessageBox.popupClientMessage(this.Page, strInfor, "call();");

        //        return true;

        //    }
        //    catch (Exception ee)
        //    {
        //        //
        //        MessageBox.popupClientMessage(this.Page, ee.Message, "call();");

        //        return false;
        //    }

        //}

        #endregion

        #endregion

        protected void btnReturn_Click(object sender, EventArgs e)
        {
            Response.Redirect("KPI_TeamSetting.aspx");
        }

    }
}
