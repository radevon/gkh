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
        public static byte[] GetActReport(List<ActModel> obj,string sourceFilePath)
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

                RepeatContent repeated = new RepeatContent("ItemArray");

                // цикл по всем объектам
                foreach (ActModel m in obj)
                {
                    TableContent tableContent = new TableContent("TableRep");
                    for (int i = 0; i < m.KonturCount; i++)
                    {

                        tableContent.AddRow(
                            new FieldContent("Np", (i + 1).ToString()),
                            new FieldContent("Addres", m.Address),
                            new FieldContent("KonturName", m.Konturs[i].KonturName),
                            new FieldContent("HeatLast", m.Konturs[i].Podacha.EndValues.HeatValue.ToString("0.00")),
                            new FieldContent("HeatFirst", m.Konturs[i].Podacha.StartValues.HeatValue.ToString("0.00")),
                            new FieldContent("HeatDiff", m.Konturs[i].Podacha.DiffsHeat.ToString("0.00")),
                            new FieldContent("VolumeLast", m.Konturs[i].Podacha.EndValues.WaterValue.ToString("f0")),
                            new FieldContent("VolumeFirst", m.Konturs[i].Podacha.StartValues.WaterValue.ToString("f0")),
                            new FieldContent("VolumeDiffs", m.Konturs[i].Podacha.DiffsWater.ToString("f0")),
                            new FieldContent("ByTimerLast", m.Konturs[i].Podacha.EndValues.TotalHours.ToString()),
                            new FieldContent("ByTimerFirst", m.Konturs[i].Podacha.StartValues.TotalHours.ToString()),
                            new FieldContent("ByTimerDiffs", m.Konturs[i].Podacha.DiffsTimer.ToString()),
                            new FieldContent("DaysWork", m.Konturs[i].Podacha.DiffsDate.Days.ToString()),

                            new FieldContent("HeatLastObr", m.Konturs[i].Obratka.EndValues.HeatValue == 0 ? "" : m.Konturs[i].Obratka.EndValues.HeatValue.ToString("0.00")),
                            new FieldContent("HeatFirstObr", m.Konturs[i].Obratka.StartValues.HeatValue == 0 ? "" : m.Konturs[i].Obratka.StartValues.HeatValue.ToString("0.00")),
                            new FieldContent("HeatDiffObr", m.Konturs[i].Obratka.DiffsHeat == 0 ? "" : m.Konturs[i].Obratka.DiffsHeat.ToString("0.00")),
                            new FieldContent("VolumeLastObr", m.Konturs[i].Obratka.EndValues.WaterValue == 0 ? "" : m.Konturs[i].Obratka.EndValues.WaterValue.ToString("f0")),
                            new FieldContent("VolumeFirstObr", m.Konturs[i].Obratka.StartValues.WaterValue == 0 ? "" : m.Konturs[i].Obratka.StartValues.WaterValue.ToString("f0")),
                            new FieldContent("VolumeDiffsObr", m.Konturs[i].Obratka.DiffsWater == 0 ? "" : m.Konturs[i].Obratka.DiffsWater.ToString("f0")),

                            new FieldContent("VolumeDiffAll", m.Konturs[i].WaterDiff.ToString("f0")),
                            new FieldContent("HeatDiffAll", m.Konturs[i].HeatDiff.ToString("0.00"))
                            );

                    }
                    IContentItem[] commonFields;

                    if (tableContent.Rows != null && tableContent.Rows.Count > 0)
                    {
                        commonFields = new IContentItem[]{
                    new FieldContent("AktNumber",m.AktNumber),
                    new FieldContent("DocNumber",m.DocNumber),
                    new FieldContent("NamePredpriatie",m.NamePredpriatie),
                    new FieldContent("PeriodReport", m.PeriodReport),
                    new FieldContent("PostDolgn", m.PostDolgn),
                    new FieldContent("UserDolgn", m.UserDolgn),
                    new FieldContent("PostFio", m.PostFio),
                    new FieldContent("UserFio", m.UserFio),
                    new FieldContent("UserPhone", m.UserPhone),
                    new FieldContent("ReportDate", m.ReportDate.ToString("dd MMM yyyy")),
                    tableContent                    
                };
                    }
                    else
                    {
                        commonFields = new IContentItem[]{
                    new FieldContent("AktNumber",m.AktNumber),
                    new FieldContent("DocNumber",m.DocNumber),
                    new FieldContent("NamePredpriatie",m.NamePredpriatie),
                    new FieldContent("PeriodReport", m.PeriodReport),
                    new FieldContent("PostDolgn", m.PostDolgn),
                    new FieldContent("UserDolgn", m.UserDolgn),
                    new FieldContent("PostFio", m.PostFio),
                    new FieldContent("UserFio", m.UserFio),
                    new FieldContent("UserPhone", m.UserPhone),
                    new FieldContent("ReportDate", m.ReportDate.ToString("dd MMM yyyy"))
                };

                    }
                    repeated.AddItem(commonFields);
                }                              
                              
                    using (var outputDocument = new TemplateProcessor(copyTempFile).SetRemoveContentControls(true))
                    {
                        outputDocument.FillContent(new Content(repeated));
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