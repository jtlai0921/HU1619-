using FluorineFx;
using System;

namespace org.zhangyafei
{

    [RemotingService("http://www.zhang-yafei.com")]
    public class HelloWorld
    {

        public string sayHelloWorld(String arg)
        {
            return "�ˣ�AMF-RPC for .NET" + arg;
        }
    }
}
