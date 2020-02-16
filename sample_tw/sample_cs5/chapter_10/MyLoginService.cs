using System;
using System.Data;
using System.Configuration;
using FluorineFx;

namespace org.zhangyafei
{
    [RemotingService("http://www.zhang-yafei.com")]
    public class MyLoginService
    {

        public bool login() {
            return true;
        }

        public bool logout() {
            (new MyLoginCommand()).Logout(null);
            return true;
        }
    }
}