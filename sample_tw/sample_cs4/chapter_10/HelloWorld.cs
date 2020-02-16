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

        // ԓ����ֻ�й���T�ſ����{��
        public string sayHelloWorldForAdmin(String arg)
        {
            // �@ȡPrincipal
            IPrincipal threadPrincipal = Thread.CurrentPrincipal;
            // �z���ɫ�Ƿ���admin
            bool val = threadPrincipal.IsInRole("admin");
            // �������admin�͒�������
            if (val == false)
            {
                throw new Exception("ԓ����ֻ�й���T�ſ����{�ã�");
            }
            return "�ˣ�admin" + arg;
        }

        // ԓ��������Ǖ��T�ſ����{��
        public string sayHelloWorldForUser(String arg)
        {
            // �@ȡPrincipal
            IPrincipal threadPrincipal = Thread.CurrentPrincipal;
            // �z���ɫ�Ƿ���user
            bool val = threadPrincipal.IsInRole("user");
            // �������admin�͒�������
            if (val == false)
            {
                throw new Exception("ԓ��������Ǖ��T�ſ����{�ã�");
            }
            return "�ˣ�user" + arg;
        }
    }
}
