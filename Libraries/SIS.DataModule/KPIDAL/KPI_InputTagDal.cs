﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using SIS.DataEntity;
using SIS.DataAccess;
using SIS.DBControl;
using SIS.Loger;

using System.Runtime.InteropServices;
using System.Data.SqlClient;

namespace SIS.DataAccess {

    public class KPI_InputTagDal : DalBase<InputTagEntity> {

        #region Added by pyf 20150127

        private RelaInterface m_DataAccess = DBAccess.GetRelation();


        public List<InputTagEntity> GetInputTags(String TagCategory) {
            String SqlText = @"SELECT A.*,B.ConstantName TagCategory
                                FROM KPI_InputTag A JOIN KPI_Constant B ON A.InputType = B.ConstantValue
                                WHERE B.ConstantCode='M' AND A.InputType=@TagCategory";
            IDbDataParameter[] Parameters = new SqlParameter[] {                 
                new SqlParameter("@TagCategory",SqlDbType.Int)
            };
            Parameters[0].Value = TagCategory;
            List<InputTagEntity> Result;
            using (IDataReader Reader = m_DataAccess.ExecuteReader(CommandType.Text, SqlText, Parameters)) {
                Result = Reader.FillGenericList<InputTagEntity>();
            };          
            return Result;
        }

        public List<InputTagEntity> GetInputTags(int PageIndex, int PageSize, String TagCategory, out int RecordCount) {
            String SqlText = @"WITH  CTE AS(
                                SELECT A.*,B.ConstantName TagCategory, ROW_NUMBER() OVER (ORDER BY InputIndex) AS R 
                                FROM KPI_InputTag A JOIN KPI_Constant B ON A.InputType = B.ConstantValue
                                WHERE B.ConstantCode='M' AND A.InputType=@TagCategory)
                                SELECT * FROM CTE WHERE R BETWEEN (@pageindex - 1) * @pagesize + 1 AND (@pageindex - 1) * @pagesize + @pagesize";
            IDbDataParameter[] Parameters = new SqlParameter[] { 
                new SqlParameter("@pageindex",SqlDbType.Int),
                new SqlParameter("@pagesize",SqlDbType.Int),
                new SqlParameter("@TagCategory",SqlDbType.Int)
            };
            Parameters[0].Value = PageIndex;
            Parameters[1].Value = PageSize;
            Parameters[2].Value = TagCategory;
            List<InputTagEntity> Result;
            using (IDataReader Reader = m_DataAccess.ExecuteReader(CommandType.Text, SqlText, Parameters)) {
                Result = Reader.FillGenericList<InputTagEntity>();
            };
            SqlText = @"SELECT COUNT(InputID) FROM KPI_InputTag A 
                        JOIN KPI_Constant B ON A.InputType = B.ConstantValue
                        WHERE B.ConstantCode='M' AND A.InputType=@TagCategory";
            RecordCount = (int)m_DataAccess.ExecuteScalar(SqlText, Parameters);
            return Result;
        }

        public int SaveInputTag(InputTagEntity InputTag) {
            if (Exists(InputTag)) {
                return UpateInputTag(InputTag);
            }
            else {
                return InsertInputTag(InputTag);
            }
        }
        public int DeleteInputTag(InputTagEntity InputTag) {
            string SqlText = @"DELETE FROM KPI_InputTag WHERE InputID=@InputID ";
            return m_DataAccess.ExecuteNonQuery(SqlText, GetParameters(InputTag));
        }
        private int InsertInputTag(InputTagEntity InputTag) {
            InputTag.InputID = InputTag.InputCode;
            InputTag.InputIndex = GetInputIndex();
            string SqlText = @"INSERT KPI_InputTag (InputID,InputCode,InputDesc,InputEngunit,InputIndex,
                               InputType) VALUES (@InputID,@InputCode,@InputDesc,@InputEngunit,@InputIndex,
                               @InputType) ";
            return m_DataAccess.ExecuteNonQuery(SqlText, GetParameters(InputTag));
        }
        private int UpateInputTag(InputTagEntity InputTag) {
            string SqlText = @"UPDATE KPI_InputTag SET InputCode=@InputCode,InputDesc=@InputDesc,
                               InputEngunit=@InputEngunit,InputIndex=@InputIndex,InputType=@InputType
                               WHERE InputID = @InputID ";
            return m_DataAccess.ExecuteNonQuery(SqlText, GetParameters(InputTag));
        }

        private int GetInputIndex() {
            string SqlText = @"SELECT ISNULL(MAX(InputIndex),0)+1 FROM KPI_InputTag";
            return (int)m_DataAccess.ExecuteScalar(SqlText);
        }

        private bool Exists(InputTagEntity InputTag) {
            string SqlText = "SELECT COUNT(InputID) FROM  KPI_InputTag WHERE InputID=@InputID ";
            //IDbDataParameter[] Parameters = new SqlParameter[] { 
            //    new SqlParameter("@InputID",SqlDbType.VarChar)};
            //Parameters[0].Value = InputTag.InputID;
            int count = (int)m_DataAccess.ExecuteScalar(SqlText, GetParameters(InputTag));
            return count > 0;
        }

        private IDbDataParameter[] GetParameters(InputTagEntity InputTag) {
            IDbDataParameter[] Parameters = new SqlParameter[] { 
                new SqlParameter("@InputID",SqlDbType.VarChar),
                new SqlParameter("@InputCode",SqlDbType.VarChar),  
                new SqlParameter("@InputDesc",SqlDbType.VarChar),
                new SqlParameter("@InputEngunit",SqlDbType.VarChar),
                new SqlParameter("@InputIndex",SqlDbType.Int),
                new SqlParameter("@InputType",SqlDbType.Int)
            };
            Parameters[0].Value = InputTag.InputID;
            Parameters[1].Value = InputTag.InputCode;
            Parameters[2].Value = InputTag.InputDesc;
            Parameters[3].Value = InputTag.InputEngunit;
            Parameters[4].Value = InputTag.InputIndex;
            Parameters[5].Value = InputTag.InputType;
            return Parameters;
        }



        #endregion


        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        public static bool DeleteTag(string InputID) {
            //删除参数信息
            string sql = "delete from KPI_InputTag ";
            if (InputID != "") {
                sql += " where InputID = '" + InputID + "'";
            }

            return DBAccess.GetRelation().ExecuteNonQuery(sql) > 0;
        }

        //static bool flag = false; 

        /// <summary>
        /// 判断期望值输出测点的唯一性
        /// </summary>
        /// <param name="MainExpectedTagName"></param>
        /// <param name="MainID"></param>
        /// <returns></returns>
        public static bool CodeExist(string InputCode, string InputID) {
            string sql = "select count(1) from KPI_InputTag where InputCode='{0}' ";
            sql = string.Format(sql, InputCode);

            if (InputID != "") {
                sql += " and InputID<>'" + InputID + "'";
            }

            return int.Parse(DBAccess.GetRelation().ExecuteScalar(sql).ToString()) > 0;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="SeqName"></param>
        /// <param name="SeqID"></param>
        /// <returns></returns>
        public static int CodeCounts(string inputtype) {
            string sql = "select InputCode from KPI_InputTag";
            if (inputtype != "") {
                sql += " where InputType=" + inputtype;
            }

            return DBAccess.GetRelation().ExecuteDataset(sql).Tables[0].Rows.Count;
        }

        /// <summary>
        /// 得到测点配置主表
        /// </summary>
        /// <returns></returns>       
//        public static DataTable GetInputTags(string inputtype) {
//            string sql = @"select InputID, InputCode, InputDesc, InputEngunit, InputIndex, InputType
//                            from KPI_InputTag
//                            {0}
//                            order by InputIndex ";

//            用户选择哪个机组进行计算
//            if (!inputtype.Equals("")) {
//                sql = string.Format(sql, " where InputType=" + inputtype);
//            }
//            else {
//                sql = string.Format(sql, "");

//            }

//            排序，按照输出标签的顺序
//            return DBAccess.GetRelation().ExecuteDataset(sql).Tables[0];
//        }

        /// <summary>
        /// 得到测点配置主表
        /// </summary>
        /// <returns></returns>       
        public static DataTable GetInputTagForInput(string inputtype) {
            string sql = @"select InputID, InputCode, InputDesc, InputEngunit, InputIndex, InputType,
                            '0'InputValue
                            from KPI_InputTag {0}";

            //用户选择哪个机组进行计算
            if (!inputtype.Equals("")) {
                sql = string.Format(sql, " where InputType=" + inputtype);
            }
            else {
                sql = string.Format(sql, "");
            }

            //排序，按照输出标签的顺序
            return DBAccess.GetRelation().ExecuteDataset(sql).Tables[0];
        }


        /// <summary>
        /// 得到测点配置主表
        /// </summary>
        /// <returns></returns>       
        public static DataTable GetInputTagWithValue(string inputtype, string inputtime) {
            string sql = @"select InputID, a.InputCode, a.InputDesc, a.InputEngunit, InputIndex, InputType,
                            ISNULL(InputValue,'0') AS InputValue
                            from KPI_InputTag a
                            left outer join KPI_InputValue b on (a.InputCode = b.InputCode {0})
                            where 1=1 {1}
                            order by InputIndex ";

            //用户选择哪个机组进行计算
            string condition1 = "";
            string condition2 = "";

            if (!inputtime.Equals("")) {
                condition1 = " and InputTime='" + inputtime + "'";
            }

            if (!inputtype.Equals("")) {
                condition2 = " and InputType=" + inputtype;
            }


            sql = string.Format(sql, condition1, condition2);


            //排序，按照输出标签的顺序
            return DBAccess.GetRelation().ExecuteDataset(sql).Tables[0];
        }

        /// <summary>
        /// 得到测点配置主表
        /// </summary>
        /// <returns></returns>       
        public static DataTable GetInputTagForDisplay(string inputtype) {
            string sql = @"select InputID, a.InputCode, a.InputDesc, a.InputEngunit, InputIndex, InputType,
                            InputValue, InputTime
                            from KPI_InputTag a
                            left outer join KPI_InputValue b on (a.InputCode = b.InputCode and b.InputSnap=1)
                            where 1=1 {0}
                            order by InputIndex ";

            //用户选择哪个机组进行计算
            string condition2 = "";

            if (!inputtype.Equals("")) {
                condition2 = " and InputType=" + inputtype;
            }

            sql = string.Format(sql, condition2);


            //排序，按照输出标签的顺序
            return DBAccess.GetRelation().ExecuteDataset(sql).Tables[0];
        }



        /// <summary>
        /// 得到测点配置主表
        /// </summary>
        /// <returns></returns>       
        public static DataTable GetInputTagLists() {
            string sql = @"select InputID, InputCode, InputDesc, InputEngunit, ''InputValue,InputType,InputIndex 
                            from KPI_InputTag ";
            return DBAccess.GetRelation().ExecuteDataset(sql).Tables[0];
        }


        /// <summary>
        /// 得到测点配置主表
        /// </summary>
        /// <returns></returns>       
        public static DataTable GetTAGs() {
            string sql = @"select InputCode[Code],InputCode+'----'+InputDesc+'--'+InputEngunit [Name]
                            from KPI_InputTag ";

            return DBAccess.GetRelation().ExecuteDataset(sql).Tables[0];
        }

        /// <summary>
        /// 得到测点配置主表
        /// </summary>
        /// <returns></returns>       
        public static DataTable GetTAGsLikeName(string name) {
            string sql = @"select InputCode[Code],InputCode+'----'+InputDesc+'--'+InputEngunit [Name]
                            from KPI_InputTag where InputDesc Like '%{0}%'";
            sql = string.Format(sql, name);

            return DBAccess.GetRelation().ExecuteDataset(sql).Tables[0];
        }

        ///// <summary>
        ///// 通过主键得到实体对象
        ///// </summary>
        ///// <param name="MainID">主键</param>
        ///// <param name="SetSnapShot">是否设置测点实时值</param>
        ///// <returns></returns>
        //public static string GetTagID(string TagName, string UnitID)
        //{
        //    KPI_InputTagEntity entity = new KPI_InputTagEntity();
        //    string sql = "select * from KPI_InputTag where TagName='{0}'";
        //    sql = string.Format(sql, TagName);

        //    if (UnitID != "")
        //    {
        //        sql += " and UnitID='" + UnitID + "' ";
        //    }

        //    DataTable dt = DBAccess.GetRelation().ExecuteDataset(sql).Tables[0];
        //    if (dt.Rows.Count > 0)
        //        entity.DrToMember(dt.Rows[0]);

        //    return entity.TagID;

        //}

        /// <summary>
        /// 通过主键得到实体对象
        /// </summary>
        /// <param name="MainID">主键</param>
        /// <param name="SetSnapShot">是否设置测点实时值</param>
        /// <returns></returns>
        public static InputTagEntity GetEntity(string InputID) {
            InputTagEntity entity = new InputTagEntity();
            string sql = "select * from KPI_InputTag where InputID='{0}'";
            sql = string.Format(sql, InputID);

            DataTable dt = DBAccess.GetRelation().ExecuteDataset(sql).Tables[0];
            if (dt.Rows.Count > 0)
                entity.DrToMember(dt.Rows[0]);

            return entity;

        }


        //        /// <summary>
        //        /// 得到查询结果
        //        /// </summary>
        //        /// <param name="condition">条件字符串</param>
        //        /// <returns></returns>
        //        public static DataTable GetSearchList(string condition)
        //        {
        //            string sql = @"select MainID, UnitName, MainOutTagName, MainOutTagDesc, '' SubTagName,
        //                            MainExpTagName, MainIsValid, MainIsError, '' color, '' AlarmInfo,
        //                            case MainOutTagType when 0 then '定值' when 1 then '开关量' when 2 then '必须量' when 3 then '计算量'  else '其他' end OutTagType,
        //                            case MainIsCheck when 0 then '否' when 1 then '是' else '否' end IsCheck,
        //                            case MainExpModel when 1 then '1-常数' when 2 then '2-曲线' when 3 then '3-测点' when 4 then '4-递推' else '5-无' end ExpModel
        //                            
        //                            from KPI_InputTag a
        //                            left outer join OPM_Unit b on a.UnitID=b.UnitID  
        //                            where 1=1 {0} 
        //                            order by MainIsValid DESC, MainOutTagName ";

        //            sql = string.Format(sql, condition);
        //            DataTable dt = DBAccess.GetRelation().ExecuteDataset(sql).Tables[0];
        //            foreach (DataRow dr in dt.Rows)
        //            {
        //                //获得源标签点
        //                dr["SubTagName"] = Filter_TagSubDal.GetConfigTagSubString(dr["MainID"].ToString());

        //                //该记录是否有效
        //                if (dr["MainIsValid"].ToString() == "0")
        //                {
        //                    dr["color"] = "MistyRose";
        //                }                

        //                //该记录是否存在配置缺陷
        //                //if (Filter_AlarmDal.GetLastAlarmInfo(dr["MainID"].ToString()) != "")
        //                //{
        //                //    dr["color"] = "RoyalBlue";
        //                //}
        //                //else
        //                //{

        //                //}

        //                //dr["alarminfo"] = Filter_AlarmDal.GetLastAlarmInfo(dr["MainID"].ToString());
        //            }
        //            return dt;
        //        }


        //        /// <summary>
        //        /// 得到数据校验结果
        //        /// </summary>
        //        /// <param name="condition">条件字符串</param>
        //        /// <returns></returns>
        //        public static DataTable GetSearchList2(string condition)
        //        {
        //            string sql = @" select a.MainID, MainOutTagName, MainOutTagDesc, '' SubTagName, MainOutTagUnit, MainIsValid, 
        //                            ValueTimeStamp, ValueInput, ValueSnapShot, ValueExpected, ValueDeltaAbs, ValueDeltaPer, ValueMW, ''color 
        //                            
        //                            from (select * from KPI_InputTag where 1=1 {0}) 
        //                            a  left outer join Filter_TagValue b on a.MainID=b.MainID  
        //                            order by ValueDeltaPer DESC, MainOutTagName";

        //            //case IsAlarm when 1 then '是' else '否' end IsAlarm 
        //            //IsAlarm DESC, 
        //            sql = string.Format(sql, condition);
        //            DataTable dt = DBAccess.GetRelation().ExecuteDataset(sql).Tables[0];
        //            foreach (DataRow dr in dt.Rows)
        //            {
        //                //源标签点信息
        //                dr["SubTagName"] = Filter_TagSubDal.GetConfigTagSubString(dr["MainID"].ToString());

        //                if (dr["ValueDeltaPer"].ToString().Equals(""))
        //                {
        //                    continue;
        //                }

        //                //误差大小
        //                if (Math.Abs(double.Parse(dr["ValueDeltaPer"].ToString()))>=10)
        //                {
        //                    dr["color"] = "Red";
        //                }else if (Math.Abs(double.Parse(dr["ValueDeltaPer"].ToString())) >= 5)
        //                {
        //                    dr["color"] = "Yellow";
        //                }

        //                //该记录是否存在配置缺陷
        //                //if (dr["IsError"].ToString() == "1")
        //                //{
        //                //    dr["color"] = "RoyalBlue";
        //                //}
        //            } 

        //            return dt;
        //        }


        /// <summary>
        /// 得到查询结果
        /// </summary>
        /// <param name="condition">条件字符串</param>
        /// <returns></returns>
        public static DataTable GetSearchList(string condition) {
            string sql = @"select ECID, ECCode, ECName, ECDesc, b.EngunitName, c.CycleName, ECWeb, ECIndex, ECIsValid, ECIsCalc, ECIsDisplay, ECIsTotal,
                                  ECMaxValue, ECMinValue,  ECIsSnapshot, ECIsSort
                            from KPI_InputTag a  
                            left outer join KPI_Engunit b on a.EngunitID = b.EngunitID
                            left outer join KPI_Cycle c on a.CycleID = c.CycleID
                            where 1=1 {0}  
                            order by ECIndex";

            sql = string.Format(sql, condition);

            DataTable dt = DBAccess.GetRelation().ExecuteDataset(sql).Tables[0];

            return dt;
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 获得与主键对应的实体对象
        /// </summary>
        /// <returns></returns>
        public static List<InputTagEntity> GetAllEntity() {
            List<InputTagEntity> Result = null;
            string sqlText = @"select * from KPI_InputTag";
            using (IDataReader reader = DBAccess.GetRelation().ExecuteReader(sqlText)) {
                Result = reader.FillGenericList<InputTagEntity>();
                reader.Close();
            }
            return Result;
            //List<KPI_InputTagEntity> ltInputs = new List<KPI_InputTagEntity>();
            //string sql = "select * from KPI_InputTag";
            //DataTable dt = DBAccess.GetRelation().ExecuteDataset(sql).Tables[0];
            //foreach (DataRow dr in dt.Rows) {
            //    KPI_InputTagEntity entity = new KPI_InputTagEntity();
            //    entity.DrToMember(dr);
            //    ltInputs.Add(entity);
            //}
            //return ltInputs;
        }

        //        /// <summary>
        //        /// 得到测点配置主表
        //        /// </summary>
        //        /// <returns></returns>       
        //        public static DataTable GetTagListForExcel(string UnitID)
        //        {
        //            string sql = @"select 'x'SelectX, TagName, TagDesc, TagType, TagEngunit, TagIsValid, TagIndex,
        //                             TagFilterExp, TagCalcExp, TagCalcExpType, TagCalcType, TagFactor, TagOffset
        //                            from KPI_InputTag  
        //                            where UnitID='{0}' 
        //                            order by TagIndex";

        //            sql = string.Format(sql, UnitID);

        //            DataTable dt = DBAccess.GetRelation().ExecuteDataset(sql).Tables[0];

        //            return dt;
        //        }

    }
}