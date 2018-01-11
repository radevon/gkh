using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using TemplateEngine.Docx;

namespace MonoIndication
{
    public class ReportGen
    {
        
       
        private static string tempPath = Path.GetTempPath();

        // метод позволяющий генерировать отчет по шаблону
        public static byte[] GetActReport(ActModel obj,string sourceFilePath)
        {
            byte[] res=new byte[]{};
            
            string copyTempFile = tempPath+@"\"+Guid.NewGuid().ToString()+".docx";
            if (!File.Exists(sourceFilePath))
                throw new Exception("Не найден файл шаблона отчета по пути: " + sourceFilePath);
            try
            {
                // удаляю старый отчет если существует
                if (File.Exists(copyTempFile))
                    File.Delete(copyTempFile);
                File.Copy(sourceFilePath, copyTempFile);

                if (obj == null)
                    return res;

                TableContent tableContent=new TableContent("TableRep");
                for(int i=0;i<obj.KonturCount;i++){

                    tableContent.AddRow(
                        new FieldContent("Np", (i + 1).ToString()),
                        new FieldContent("Addres", obj.Address),
                        new FieldContent("KonturName",obj.Konturs[i].KonturName),
                        new FieldContent("HeatLast", obj.Konturs[i].Podacha.EndValues.HeatValue.ToString("0.0")),
                        new FieldContent("HeatFirst", obj.Konturs[i].Podacha.StartValues.HeatValue.ToString("0.0")),
                        new FieldContent("HeatDiff", obj.Konturs[i].Podacha.DiffsHeat.ToString("0.0")),
                        new FieldContent("VolumeLast", obj.Konturs[i].Podacha.EndValues.WaterValue.ToString("0.0")),
                        new FieldContent("VolumeFirst", obj.Konturs[i].Podacha.StartValues.WaterValue.ToString("0.0")),
                        new FieldContent("VolumeDiffs", obj.Konturs[i].Podacha.DiffsWater.ToString("0.0")),
                        new FieldContent("ByTimerLast", obj.Konturs[i].Podacha.EndValues.TotalHours.ToString()),
                        new FieldContent("ByTimerFirst", obj.Konturs[i].Podacha.StartValues.TotalHours.ToString()),
                        new FieldContent("ByTimerDiffs", obj.Konturs[i].Podacha.DiffsTimer.ToString()),
                        new FieldContent("DaysWork", obj.Konturs[i].Podacha.DiffsDate.Days.ToString()),

                        new FieldContent("HeatLastObr", obj.Konturs[i].Obratka.EndValues.HeatValue==0?"":obj.Konturs[i].Obratka.EndValues.HeatValue.ToString("0.0")),
                        new FieldContent("HeatFirstObr", obj.Konturs[i].Obratka.StartValues.HeatValue==0?"":obj.Konturs[i].Obratka.StartValues.HeatValue.ToString("0.0")),
                        new FieldContent("HeatDiffObr", obj.Konturs[i].Obratka.DiffsHeat==0?"":obj.Konturs[i].Obratka.DiffsHeat.ToString("0.0")),
                        new FieldContent("VolumeLastObr", obj.Konturs[i].Obratka.EndValues.WaterValue==0?"":obj.Konturs[i].Obratka.EndValues.WaterValue.ToString("0.0")),
                        new FieldContent("VolumeFirstObr", obj.Konturs[i].Obratka.StartValues.WaterValue==0?"":obj.Konturs[i].Obratka.StartValues.WaterValue.ToString("0.0")),
                        new FieldContent("VolumeDiffsObr", obj.Konturs[i].Obratka.DiffsWater==0?"":obj.Konturs[i].Obratka.DiffsWater.ToString("0.0")),
                        
                        new FieldContent("VolumeDiffAll", obj.Konturs[i].WaterDiff.ToString("0.0")),
                        new FieldContent("HeatDiffAll", obj.Konturs[i].HeatDiff.ToString("0.0"))
                        );
                  
                }
                IContentItem[] commonFields;

                if (tableContent.Rows!=null&&tableContent.Rows.Count > 0)
                {
                    commonFields = new IContentItem[]{
                    new FieldContent("AktNumber",obj.AktNumber),
                    new FieldContent("DocNumber",obj.DocNumber),
                    new FieldContent("NamePredpriatie",obj.NamePredpriatie),
                    new FieldContent("PeriodReport", obj.PeriodReport),
                    new FieldContent("PostDolgn", obj.PostDolgn),
                    new FieldContent("UserDolgn", obj.UserDolgn),
                    new FieldContent("PostFio", obj.PostFio),
                    new FieldContent("UserFio", obj.UserFio),
                    new FieldContent("UserPhone", obj.UserPhone),
                    new FieldContent("ReportDate", obj.ReportDate.ToString("dd MMM yyyy")),
                    tableContent

                    
                };
                }else
                    {
                        commonFields = new IContentItem[]{
                    new FieldContent("AktNumber",obj.AktNumber),
                    new FieldContent("DocNumber",obj.DocNumber),
                    new FieldContent("NamePredpriatie",obj.NamePredpriatie),
                    new FieldContent("PeriodReport", obj.PeriodReport),
                    new FieldContent("PostDolgn", obj.PostDolgn),
                    new FieldContent("UserDolgn", obj.UserDolgn),
                    new FieldContent("PostFio", obj.PostFio),
                    new FieldContent("UserFio", obj.UserFio),
                    new FieldContent("UserPhone", obj.UserPhone),
                    new FieldContent("ReportDate", obj.ReportDate.ToString("dd MMM yyyy"))
                };
                    
                    }
                   
               
                    using (var outputDocument = new TemplateProcessor(copyTempFile).SetRemoveContentControls(true))
                    {
                        outputDocument.FillContent(new Content(commonFields));
                        outputDocument.SaveChanges();
                        
                    }
                    res = File.ReadAllBytes(copyTempFile);
                }
            catch (Exception ex) { throw ex; }
            finally
            {
                if (File.Exists(copyTempFile))
                    File.Delete(copyTempFile);
            }
                       

            return res;
        }
    }
}