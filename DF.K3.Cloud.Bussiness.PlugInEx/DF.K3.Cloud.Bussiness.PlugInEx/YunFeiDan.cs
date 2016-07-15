using DF.K3.Cloud.Bussiness.PlugIn;
using Kingdee.BOS.Core.Bill.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Core.Metadata.EntityElement;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.ServiceHelper;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
namespace DF.K3.Cloud.Bussiness.PlugIn
{
    [Description("运费单插件by fjfdszj") ]
    public class YunFeiDan : AbstractBillPlugIn
    {
        public static bool isSetYunFei = true;
        public override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            long num = Convert.ToInt64(View.Model.GetPKValue());
            if (num == 0L && View.Model.GetValue("FCUSTOMERID") != null)
            {
                string value = (View.Model.GetValue("FCUSTOMERID") as DynamicObject)["Id"].ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    this.SetInit();
                }
            }
        }
        public void SetInit()
        {
            EntryEntity entryEntity = View.Model.BusinessInfo.GetEntryEntity("FYunFuiDan_Entry");
            DynamicObjectCollection entityDataObject = View.Model.GetEntityDataObject(entryEntity);
            string text = "'nothing'";
            string text2 = "";
            for (int i = 0; i < entityDataObject.Count<DynamicObject>(); i++)
            {
                if (entityDataObject[i]["FNAME"] != null)
                {
                    string text3 = (entityDataObject[i]["FNAME"] as DynamicObject)["Id"].ToString();
                    text2 = (entityDataObject[i]["FNAME"] as DynamicObject)["UseOrgId_Id"].ToString();
                    string text4 = (entityDataObject[i]["FNAME"] as DynamicObject)["NUMBER"].ToString();
                    text = text + ",'" + text4 + "'";
                }
            }
            string text5 = string.Concat(new string[]
            {
                "SELECT t0.FUSEORGID fuseorgid_id, t0.FNUMBER fnumber, \r\n                                     t0.FDOCUMENTSTATUS fdocumentstatus, t0.FFORBIDSTATUS fforbidstatus, \r\n                                    t0.FDECIMAL TiZhi, t0.FDECIMAL1 keZhong,\r\n                                     t0.FASSISTANT fassistant_id,t1.FNUMBER YunFeiYiJu\r\n                                     FROM T_BD_MATERIAL t0 \r\n                                     left join T_BAS_ASSISTANTDATAENTRY t1 on t0.FASSISTANT=t1.FENTRYID\r\n                                     WHERE t0.FFORBIDSTATUS = 'A' AND T0.FUSEORGID='",
                text2,
                "' AND t0.FNUMBER in (",
                text,
                ") order by t0.fnumber"
            });
            DataSet dataSet = DBServiceHelper.ExecuteDataSet(base.Context, text5);
            for (int i = 0; i < entityDataObject.Count<DynamicObject>(); i++)
            {
                if (entityDataObject[i]["FNAME"] != null)
                {
                    string text4 = (entityDataObject[i]["FNAME"] as DynamicObject)["NUMBER"].ToString();
                    DataRow[] array = dataSet.Tables[0].Select("fnumber='" + text4 + "'");
                    if (array.Length > 0)
                    {
                        if (View.Model.GetValue("FYuFeiYiJu", i) == null)
                        {
                            View.Model.SetValue("FYuFeiYiJu", array[0]["fassistant_id"], i);
                            string text6 = array[0]["YunFeiYiJu"].ToString();
                        }
                        else
                        {
                            string text6 = (entityDataObject[i]["FYuFeiYiJu"] as DynamicObject)["FNumber"].ToString();
                        }
                        View.Model.SetValue("FTiZhi", array[0]["TiZhi"], i);
                        View.Model.SetValue("FZhongLiang", array[0]["keZhong"], i);
                    }
                }
            }
            this.SetYunFeiDanJia("1");
        }
        public void SetYunFeiDanJia(string leixing)
        {
            EntryEntity entryEntity = View.Model.BusinessInfo.GetEntryEntity("FYunFuiDan_Entry");
            DynamicObjectCollection entityDataObject = View.Model.GetEntityDataObject(entryEntity);
            string text = "-1";
            if (View.Model.GetValue("FCUSTOMERID") != null)
            {
                text = (View.Model.GetValue("FCUSTOMERID") as DynamicObject)["Id"].ToString();
            }
            string text2 = "-1";
            if (View.Model.GetValue("FSUPPLIERID") != null)
            {
                text2 = (View.Model.GetValue("FSUPPLIERID") as DynamicObject)["Id"].ToString();
            }
            DataSet dataSet = null;
            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
            {
                string text3 = string.Concat(new string[]
                {
                    "select * from t_YunFeiGuanLiData where FSUPPLIERID='",
                    text2,
                    "' and FCustom='",
                    text,
                    "' and FFORBIDSTATUS='A' and FDOCUMENTSTATUS='C'"
                });
                dataSet = DBServiceHelper.ExecuteDataSet(this.Context, text3);
                if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                {
                    text3 = string.Concat(new string[]
                    {
                        "select * from t_YunFeiGuanLiData t where exists (select 1 from t_BD_Supplier s where s.FMASTERID=t.FSUPPLIERID and s.FSUPPLIERID='",
                        text2,
                        "')  and FCustom='",
                        text,
                        "' and FFORBIDSTATUS='A' and FDOCUMENTSTATUS='C'"
                    });
                    dataSet = DBServiceHelper.ExecuteDataSet(this.Context, text3);
                }
            }
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                num = Convert.ToDouble(dataSet.Tables[0].Rows[0]["FDunPrice"]);
                num2 = Convert.ToDouble(dataSet.Tables[0].Rows[0]["FFangPrice"]);
                num3 = Convert.ToDouble(dataSet.Tables[0].Rows[0]["FJianPrice"]);
            }
            for (int i = 0; i < entityDataObject.Count<DynamicObject>(); i++)
            {
                if (entityDataObject[i]["FNAME"] != null)
                {
                    string value = (entityDataObject[i]["FNAME"] as DynamicObject)["Id"].ToString();
                    string text4 = (entityDataObject[i]["FNAME"] as DynamicObject)["UseOrgId_Id"].ToString();
                    string text5 = (entityDataObject[i]["FNAME"] as DynamicObject)["NUMBER"].ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        string text6 = "";
                        if (entityDataObject[i]["FYuFeiYiJu"] != null)
                        {
                            text6 = (entityDataObject[i]["FYuFeiYiJu"] as DynamicObject)["FNumber"].ToString();
                        }
                        double num4 = Convert.ToDouble(entityDataObject[i]["FQty"]);
                        double num5 = 0.0;
                        double num6 = 1.0;
                        string text7 = text6;
                        if (text7 != null)
                        {
                            if (!(text7 == "01"))
                            {
                                if (!(text7 == "02"))
                                {
                                    if (text7 == "03")
                                    {
                                        num5 = num;
                                        num6 = Convert.ToDouble(entityDataObject[i]["FZhongLiang"]);
                                        num6 = GetJieGuoZhuanHuan(num6 * num4, 2);
                                        View.Model.SetValue("F_LIN_ZhongLiang", num6, i);
                                        View.Model.SetValue("F_LIN_TiZhi", this.GetJieGuoZhuanHuan(Convert.ToDouble(entityDataObject[i]["FTiZhi"]) * num4, 2), i);
                                    }
                                }
                                else
                                {
                                    num5 = num2;
                                    num6 = Convert.ToDouble(entityDataObject[i]["FTiZhi"]);
                                    num6 = this.GetJieGuoZhuanHuan(num6 * num4, 2);
                                    View.Model.SetValue("F_LIN_TiZhi", num6, i);
                                    View.Model.SetValue("F_LIN_ZhongLiang", this.GetJieGuoZhuanHuan(Convert.ToDouble(entityDataObject[i]["FZhongLiang"]) * num4, 2), i);
                                }
                            }
                            else
                            {
                                num6 = num4;
                                num5 = num3;
                            }
                        }
                        double num7 = num5 * num6;
                        View.Model.SetValue("FYunFuiPrice", num5, i);
                        View.Model.SetValue("FYunFeiJinE", num7, i);
                        View.Model.SetValue("FYunFuiYFPrice", num7 / num4, i);
                    }
                }
            }
        }
        public double GetJieGuoZhuanHuan(double zhi, int xiaoshu)
        {
            string[] array = zhi.ToString().Split(new char[]
            {
                '.'
            });
            string text = array[0];
            if (array.Length > 1)
            {
                if (array[1].Length > xiaoshu)
                {
                    text = text + "." + array[1].Substring(0, xiaoshu);
                }
                else
                {
                    text = text + "." + array[1];
                }
            }
            return Convert.ToDouble(text);
        }
        public override void BarItemClick(BarItemClickEventArgs e)
        {
            base.BarItemClick(e);
            string barItemKey = e.BarItemKey;
            if (barItemKey != null)
            {
                if (barItemKey == "tbReSetOld")
                {
                    this.SetOldYunFei();
                }
            }
        }
        public override void DataChanged(DataChangedEventArgs e)
        {
            base.DataChanged(e);
            string text = e.Field.Key.ToString().ToUpper();
            if (text != null)
            {
                if (!(text == "FCUSTOMERID"))
                {
                    if (!(text == "FSUPPLIERID"))
                    {
                        if (!(text == "FQTY"))
                        {
                            if (!(text == "FYUFEIYIJU"))
                            {
                                if (!(text == "FYUNFEIJINE"))
                                {
                                    if (text == "F_LIN_HUIYONG")
                                    {
                                        this.ReSetJinE();
                                    }
                                }
                                else
                                {
                                    if (YunFeiDan.isSetYunFei)
                                    {
                                        this.SetYiJuHuiZong();
                                    }
                                }
                            }
                            else
                            {
                                if ( Model.GetValue("FYuFeiYiJu", e.Row) != null)
                                {
                                    if (View.Model.GetValue("FSUPPLIERID") != null)
                                    {
                                        YunFeiDan.isSetYunFei = true;
                                        this.SetFYunFeiJinE(e.Row);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (View.Model.GetValue("FYuFeiYiJu", e.Row) != null)
                            {
                                if (View.Model.GetValue("FSUPPLIERID", e.Row) != null)
                                {
                                    YunFeiDan.isSetYunFei = true;
                                    this.SetFYunFeiJinE(e.Row);
                                }
                            }
                        }
                    }
                    else
                    {
                        YunFeiDan.isSetYunFei = false;
                        this.SetYunFeiDanJia("1");
                        YunFeiDan.isSetYunFei = true;
                        this.SetYiJuHuiZong();
                    }
                }
                else
                {
                    YunFeiDan.isSetYunFei = false;
                    this.SetYunFeiDanJia("1");
                    YunFeiDan.isSetYunFei = true;
                    this.SetYiJuHuiZong();
                }
            }
        }
        public void ReSetJinE()
        {
            double num = Convert.ToDouble(View.Model.GetValue("F_LIN_ZZLJinE"));
            double num2 = Convert.ToDouble(View.Model.GetValue("F_LIN_ZTJJinE"));
            double num3 = Convert.ToDouble(View.Model.GetValue("F_LIN_ZSLJinE"));
            double num4 = Convert.ToDouble(View.Model.GetValue("F_LIN_HuiYong"));
            View.Model.SetValue("F_LIN_HeJiJinE", num + num2 + num3 + num4);
        }
        /// <summary>
        /// SetYiJuHuiZong 方法重写
        /// bug:Value was either too large or too small for a Decimal.
        /// 时间：20160715
        /// 作者：fjfdszj
        /// 修改：将数据类型double改为Decimal;
        /// </summary>
        public void SetYiJuHuiZong()
        {
            EntryEntity entryEntity = View.Model.BusinessInfo.GetEntryEntity("FYunFuiDan_Entry");
            DynamicObjectCollection entityDataObject = View.Model.GetEntityDataObject(entryEntity);
            Decimal num = 0M;
            Decimal num2 = 0M;
            Decimal num3 = 0M;
            Decimal num4 = 0M;
            Decimal num5 = 0M;
            Decimal num6 = 0M;
            Decimal num7 = 0M;
            Decimal num8 = 0M;
            Decimal num9 = 0M;
            for (int i = 0; i < entityDataObject.Count<DynamicObject>(); i++)
            {
                if (entityDataObject[i]["FNAME"] != null)
                {
                    if (View.Model.GetValue("FYuFeiYiJu", i) != null)
                    {
                        string type = (View.Model.GetValue("FYuFeiYiJu", i) as DynamicObject)["FNumber"].ToString();
                        Decimal amount = Convert.ToDecimal(View.Model.GetValue("FYunFeiJinE", i));
                        Decimal prince = Convert.ToDecimal(View.Model.GetValue("FYunFuiPrice", i));
                        if (type != null && prince!=0M)
                        {
                            if (!(type == "01"))
                            {
                                if (!(type == "02"))
                                {
                                    if (type == "03")
                                    {
                                        num += Math.Round(amount / prince, 2);
                                        num7 += amount;
                                    }
                                }
                                else
                                {
                                    num2 += Math.Round(amount / prince, 2);
                                    num8 += amount;
                                }
                            }
                            else
                            {
                                num3 += Math.Round(amount / prince, 2);
                                num9 += amount;
                            }
                        }
                    }
                }
            }
            if (num > 0.0M)
            {
                num4 = Math.Round(num7 / num, 2);
            }
            if (num2 > 0.0M)
            {
                num5 = Math.Round(num8 / num2, 2);
            }
            if (num3 > 0.0M)
            {
                num6 = Math.Round(num9 / num3, 2);
            }
            View.Model.SetValue("F_LIN_ZongZhongLiang", num);
            View.Model.SetValue("F_LIN_ZongTiZhi", num2);
            View.Model.SetValue("F_LIN_ZongShuLiang", num3);
            View.Model.SetValue("F_LIN_ZZLDanJia", num4);
            View.Model.SetValue("F_LIN_ZTJDanJia", num5);
            View.Model.SetValue("F_LIN_ZSLDanJia", num6);
            View.Model.SetValue("F_LIN_ZZLJinE", num7);
            View.Model.SetValue("F_LIN_ZTJJinE", num8);
            View.Model.SetValue("F_LIN_ZSLJinE", num9);
            View.Model.SetValue("F_LIN_HeJiJinE", num7 + num8 + num9 + Convert.ToDecimal(View.Model.GetValue("F_LIN_HuiYong")));
        }
        public void SetFYunFeiJinE(int row)
        {

            if (View.Model.GetValue("FYuFeiYiJu", row) != null)
            {
                string text = (View.Model.GetValue("FYuFeiYiJu", row) as DynamicObject)["FNumber"].ToString();
                string text2 = View.Model.GetValue("FTiZhi", row).ToString();
                string text3 = View.Model.GetValue("FZhongLiang", row).ToString();
                if (text != "01" && text2 == "0" && text3 == "0")
                {
                    this.SetYunFeiDanJia("1");
                    text2 = View.Model.GetValue("FTiZhi", row).ToString();
                    text3 = View.Model.GetValue("FZhongLiang", row).ToString();
                }
                string text4 = "-1";
                if (View.Model.GetValue("FCUSTOMERID") != null)
                {
                    text4 = (View.Model.GetValue("FCUSTOMERID") as DynamicObject)["Id"].ToString();
                }
                string text5 = "-1";
                if (View.Model.GetValue("FSUPPLIERID") != null)
                {
                    text5 = (View.Model.GetValue("FSUPPLIERID") as DynamicObject)["Id"].ToString();
                }
                DataSet dataSet = null;
                if (!string.IsNullOrEmpty(text4) && !string.IsNullOrEmpty(text5))
                {
                    string text6 = string.Concat(new string[]
                    {
                        "select * from t_YunFeiGuanLiData where FSUPPLIERID='",
                        text5,
                        "' and FCustom='",
                        text4,
                        "' and FFORBIDSTATUS='A' and FDOCUMENTSTATUS='C'"
                    });
                    dataSet = DBServiceHelper.ExecuteDataSet(this.Context, text6);
                    if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                    {
                        text6 = string.Concat(new string[]
                        {
                            "select * from t_YunFeiGuanLiData t where exists (select 1 from t_BD_Supplier s where s.FMASTERID=t.FSUPPLIERID and s.FSUPPLIERID='",
                            text5,
                            "')  and FCustom='",
                            text4,
                            "' and FFORBIDSTATUS='A' and FDOCUMENTSTATUS='C'"
                        });
                        dataSet = DBServiceHelper.ExecuteDataSet(base.Context, text6);
                    }
                }
                double num = 0.0;
                double num2 = 0.0;
                double num3 = 0.0;
                if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    num = Convert.ToDouble(dataSet.Tables[0].Rows[0]["FDunPrice"]);
                    num2 = Convert.ToDouble(dataSet.Tables[0].Rows[0]["FFangPrice"]);
                    num3 = Convert.ToDouble(dataSet.Tables[0].Rows[0]["FJianPrice"]);
                }
                double num4 = 0.0;
                double num5 = Convert.ToDouble(View.Model.GetValue("FQty", row));
                double num6 = 1.0;
                string text7 = text;
                if (text7 != null)
                {
                    if (!(text7 == "01"))
                    {
                        if (!(text7 == "02"))
                        {
                            if (text7 == "03")
                            {
                                num4 = num;
                                num6 = this.GetJieGuoZhuanHuan(Convert.ToDouble(text3) * num5, 2);
                                View.Model.SetValue("F_LIN_ZhongLiang", num6, row);
                                View.Model.SetValue("F_LIN_TiZhi", this.GetJieGuoZhuanHuan(Convert.ToDouble(text2) * num5, 2), row);
                            }
                        }
                        else
                        {
                            num4 = num2;
                            num6 = this.GetJieGuoZhuanHuan(Convert.ToDouble(text2) * num5, 2);
                            View.Model.SetValue("F_LIN_TiZhi", num6, row);
                            View.Model.SetValue("F_LIN_ZhongLiang", this.GetJieGuoZhuanHuan(Convert.ToDouble(text3) * num5, 2), row);
                        }
                    }
                    else
                    {
                        num6 = num5;
                        num4 = num3;
                    }
                }
                View.Model.SetValue("FYunFuiPrice", num4, row);
                double num7 = num4 * num6;
                View.Model.SetValue("FYunFeiJinE", num7, row);
                View.Model.SetValue("FYunFuiYFPrice", num7 / num5, row);
            }
        }
        public void SetOldYunFei()
        {
            string text = "select Fid,F_LIN_HuiYong from t_YunFuiDan_Head where F_LIN_HeJiJinE=0";
            DataSet dataSet = DBServiceHelper.ExecuteDataSet(base.Context, text);
            text = "select t0.FNUMBER YFYiJu,e.* from t_YunFuiDan_Head h,t_YunFuiDan_Entry e,T_BAS_ASSISTANTDATAENTRY t0 where h.fid=e.fid AND E.FYUFEIYIJU=t0.FENTRYID and h.F_LIN_HeJiJinE=0";
            DataSet dataSet2 = DBServiceHelper.ExecuteDataSet(base.Context, text);
            text = "";
            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                double num = 0.0;
                double num2 = 0.0;
                double num3 = 0.0;
                double num4 = 0.0;
                double num5 = 0.0;
                double num6 = 0.0;
                double num7 = 0.0;
                double num8 = 0.0;
                double num9 = 0.0;
                DataRow[] array = dataSet2.Tables[0].Select("fid='" + dataRow["Fid"] + "'");
                for (int i = 0; i < array.Length; i++)
                {
                    DataRow dataRow2 = array[i];
                    string text2 = dataRow2["YFYiJu"].ToString();
                    double num10 = Convert.ToDouble(dataRow2["FYunFeiJinE"]);
                    double num11 = Convert.ToDouble(dataRow2["FYunFuiPrice"]);
                    string text3 = text2;
                    if (text3 != null)
                    {
                        if (!(text3 == "01"))
                        {
                            if (!(text3 == "02"))
                            {
                                if (text3 == "03")
                                {
                                    num += Math.Round(num10 / num11, 2);
                                    num7 += num10;
                                }
                            }
                            else
                            {
                                num2 += Math.Round(num10 / num11, 2);
                                num8 += num10;
                            }
                        }
                        else
                        {
                            num3 += Math.Round(num10 / num11, 2);
                            num9 += num10;
                        }
                    }
                }
                if (num > 0.0)
                {
                    num4 = Math.Round(num7 / num, 2);
                }
                if (num2 > 0.0)
                {
                    num5 = Math.Round(num8 / num2, 2);
                }
                if (num3 > 0.0)
                {
                    num6 = Math.Round(num9 / num3, 2);
                }
                object obj = text;
                text = string.Concat(new object[]
                {
                    obj,
                    "update t_YunFuiDan_Head set F_LIN_ZongZhongLiang='",
                    num,
                    "',F_LIN_ZongTiZhi='",
                    num2,
                    "',F_LIN_ZongShuLiang='",
                    num3,
                    "',F_LIN_ZZLDanJia='",
                    num4,
                    "',F_LIN_ZTJDanJia='",
                    num5,
                    "',F_LIN_ZSLDanJia='",
                    num6,
                    "',F_LIN_ZZLJinE='",
                    num7,
                    "',F_LIN_ZTJJinE='",
                    num8,
                    "',F_LIN_ZSLJinE='",
                    num9,
                    "',F_LIN_HeJiJinE='",
                    (num7 + num8 + num9 + Convert.ToDouble(dataRow["F_LIN_HuiYong"])).ToString(),
                    "' where fid='",
                    dataRow["fid"].ToString(),
                    "';"
                });
            }
            if (!string.IsNullOrEmpty(text))
            {
                if (DBServiceHelper.Execute(base.Context, text) > 0)
                {
                    View.ShowMessage("批量处理旧数据成功！", 0);
                }
            }
        }
    }
}
