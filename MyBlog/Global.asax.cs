using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MyBlog.Models;

namespace MyBlog
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // 获取访问者的IP
            string ipAddress = Request.ServerVariables["REMOTE_ADDR"];
            Console.WriteLine(ipAddress);
            // 获取访问者的来源
            string ipSrc;
            
            // 判断是否从搜索引擎导航过来
            if(Request.UrlReferrer == null)
            {
                ipSrc = "";
            }
            else
            {
                // 获取来源地址
                ipSrc = Request.UrlReferrer.ToString();
            }

            // 保存IP信息到数据库中
            IpControlRepository icr = new IpControlRepository();
            icr.TryInsertIpControl(new IpControl()
            {
                IpAddress = ipAddress,
                IpSource = ipSrc,
                PageUrl = Request.Url.ToString(),
                RequestDateTime = DateTime.Now
            });

            // 锁定变量
            Application.Lock();
            try
            {
                Application["VisitorCount"] = int.Parse(Application["VisitorCount"].ToString()) + 1;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Application["VisitorCount"] = 1;
            }
            // 解锁变量
            Application.UnLock();
        }

        protected void Session_End(object sender, EventArgs e)
        {
            // 在会话结束时运行的代码。   

            // 注意: 只有在 Web.config 文件中的 sessionstate 模式设置为 InProc 时，才会引发 Session_End 事件。  
            //如果会话模式设置为 StateServer 或 SQLServer，则不会引发该事件。   

            //锁定变量   
            Application.Lock();

            Application["VisitorCount"] = (int)Application["VisitorCount"] - 1; //在线人数减1

            //解锁   
            Application.UnLock();
        }
    }
}
