using CookComputing.XmlRpc;

namespace flashRemoting.xmlrpc
{
    // 必須繼承XmlRpcService
    public class HelloWorld : XmlRpcService
    {
        // 使用XmlRpcMethod屬性聲明可調用的方法
        [XmlRpcMethod("flashRemoting.xmlrpc.HelloWorld.sayHelloWorld")]
        public string sayHelloWorld(string str)
        {
            string ret = str + "Flash XML-RPC";
            return ret;
        }
    }
}
