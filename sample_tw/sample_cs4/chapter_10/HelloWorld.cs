using System;
using System.Threading;
using System.Security;
using System.Security.Principal;
using FluorineFx;

namespace org.zhangyafei
{

    [RemotingService("http://www.zhang-yafei.com")]
    public class HelloWorld
    {

        // 該方法只有管理員才可以調用
        public string sayHelloWorldForAdmin(String arg)
        {
            // 獲取Principal
            IPrincipal threadPrincipal = Thread.CurrentPrincipal;
            // 檢查角色是否是admin
            bool val = threadPrincipal.IsInRole("admin");
            // 如果不是admin就拋出異常
            if (val == false)
            {
                throw new Exception("該方法只有管理員才可以調用！");
            }
            return "嗨！admin" + arg;
        }

        // 該方法必須是會員才可以調用
        public string sayHelloWorldForUser(String arg)
        {
            // 獲取Principal
            IPrincipal threadPrincipal = Thread.CurrentPrincipal;
            // 檢查角色是否是user
            bool val = threadPrincipal.IsInRole("user");
            // 如果不是admin就拋出異常
            if (val == false)
            {
                throw new Exception("該方法必須是會員才可以調用！");
            }
            return "嗨！user" + arg;
        }
    }
}
