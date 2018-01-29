using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DBPortable;
using System.Configuration;


namespace MonoIndication.Controllers
{
    [Authorize]
    [OutputCache(Duration = 0, NoStore = true)]
    public class DeviceController : Controller
    {
        
        // GET: /Device/

        private VisualDataRepository repo = new VisualDataRepository(ConfigurationManager.AppSettings["dbPath"]);
        private Logger loger = new Logger();

        
        public JsonResult AllMarkers()
        {
            List<Marker> devices=new List<Marker>();
            try
            {
                devices = repo.GetAllMarkers().ToList();

                
            }
            catch (Exception ex)
            {
                LogMessage message = new LogMessage()
                {
                    Id = -1,
                    MessageDate = DateTime.Now,
                    UserName = User.Identity.Name,
                    MessageType = "error",
                    MessageText = ex.Message + ex.StackTrace
                };

                loger.LogToFile(message);
                loger.LogToDatabase(message);
            }
            
            return Json(devices, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult InsertNewMarker(Marker marker)
        {
            int insertedId;
            try
            {
                insertedId = repo.InsertMarker(marker);
                // логирование
                if (insertedId > 0)
                {
                    LogMessage message = new LogMessage()
                    {
                        Id = 1,
                        UserName = User.Identity.Name,
                        MessageDate = DateTime.Now,
                        MessageType = "insert",
                        MessageText = new JavaScriptSerializer().Serialize(marker)
                    };

                    loger.LogToFile(message);
                    loger.LogToDatabase(message);
                }
            }
            catch (Exception ex)
            {
                LogMessage message = new LogMessage()
                {
                    Id = -1,
                    UserName = User.Identity.Name,
                    MessageDate = DateTime.Now,
                    MessageType = "error",
                    MessageText = ex.Message + ex.StackTrace
                };

                loger.LogToFile(message);
                loger.LogToDatabase(message);
                insertedId = -1;
            }
            
            return Json(insertedId);
        }


        public ActionResult GetMarker(int MarkerId)
        {
            try
            {
                Marker marker = repo.GetMarkerById(MarkerId);
                if(marker!=null)
                    return Json(marker, JsonRequestBehavior.AllowGet);
                else
                    return HttpNotFound();
                
            }
            catch (Exception ex)
            {
                LogMessage message = new LogMessage()
                {
                    Id = -1,
                    MessageDate = DateTime.Now,
                    UserName = User.Identity.Name,
                    MessageType = "error",
                    MessageText = ex.Message + ex.StackTrace
                };

                loger.LogToFile(message);
                loger.LogToDatabase(message);
                return HttpNotFound();
            }
            
        }

        [HttpPost]
        public JsonResult UpdateMarker(Marker marker)
        {
            int updateCount;
            try
            {
                Marker old = repo.GetMarkerById(marker.MarkerId);
                updateCount = repo.UpdateMarker(marker);
                if (updateCount > 0)
                {
                    LogMessage message = new LogMessage()
                    {
                        Id = 1,
                        UserName = User.Identity.Name,
                        MessageDate = DateTime.Now,
                        MessageType = "update",
                        MessageText = "old values: " + new JavaScriptSerializer().Serialize(old) + "; new values: " + new JavaScriptSerializer().Serialize(marker)
                    };

                    loger.LogToFile(message);
                    loger.LogToDatabase(message);
                }
            }
            catch (Exception ex)
            {
                LogMessage message = new LogMessage()
                {
                    Id = -1,
                    MessageDate = DateTime.Now,
                    UserName = User.Identity.Name,
                    MessageType = "error",
                    MessageText = ex.Message + ex.StackTrace
                };

                loger.LogToFile(message);
                loger.LogToDatabase(message);
                updateCount = -1;
            }
            return Json(updateCount);
        }

        [HttpPost]
        public JsonResult DeleteMarker(int MarkerId)
        {
            int deleteCount;
            
            try
            {
                Marker deleted = repo.GetMarkerById(MarkerId);
                deleteCount = repo.DeleteMarkerById(MarkerId);
                if (deleteCount > 0)
                {
                    LogMessage message = new LogMessage()
                    {
                        Id = 1,
                        UserName = User.Identity.Name,
                        MessageDate = DateTime.Now,
                        MessageType = "delete",
                        MessageText = new JavaScriptSerializer().Serialize(deleted)
                    };
                    loger.LogToFile(message);
                    loger.LogToDatabase(message);
                }
                
            }
            catch (Exception ex)
            {
                LogMessage message = new LogMessage()
                {
                    Id = -1,
                    MessageDate = DateTime.Now,
                    UserName = User.Identity.Name,
                    MessageType = "error",
                    MessageText = ex.Message + ex.StackTrace
                };

                loger.LogToFile(message);
                loger.LogToDatabase(message);
                deleteCount = -1;
            }
            return Json(deleteCount);
        }
    }
}
