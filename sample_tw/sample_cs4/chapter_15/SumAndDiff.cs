using CookComputing.XmlRpc;

namespace flashRemoting.xmlrpc
{
    // 定义一个结构
    public struct SumAndDiffValue
    {
        public int sum;
        public int difference;
    }
    
    public class SumAndDiff : XmlRpcService
    {
        
        [XmlRpcMethod("flashRemoting.xmlrpc.SumAndDiff.sumAndDifference")]
        public SumAndDiffValue SumAndDifference(int x, int y)
        {
            SumAndDiffValue ret = default(SumAndDiffValue);
            ret.difference = x - y;
            ret.sum = x + y;
            return ret;
        }
    }
}