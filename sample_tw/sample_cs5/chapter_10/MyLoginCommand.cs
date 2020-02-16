using System;
using System.Collections;
using System.Security;
using System.Security.Principal;
using FluorineFx.Security;

namespace org.zhangyafei
{
    public class MyLoginCommand : GenericLoginCommand
    {
        // �،�DoAuthentication�������Լ��Ľ�ɫϵ�y���x
        public override IPrincipal DoAuthentication(string username,
                                                    Hashtable credentials)
        {
            // �@ȡAMF���^�е��ܴa
            string password = credentials["password"] as string;
            // �����Ñ�������һ���µ��Ñ����R
            GenericIdentity identity = new GenericIdentity(username);

            // �����Ñ������ܴa���Д��Q���Ñ��Ľ�ɫ
            // �K����һ���µ��Ñ������GenericPrincipal����
            // ���а�����ԓ�Ñ��Ę�ʾ�����ٵĽ�ɫ
            if (username == "zhangyafei" && password == "verysecret")
            {
                GenericPrincipal principal =
                             new GenericPrincipal(identity,
                                                  new string[] { "admin", "user" });
                return principal;
            }
            else if (username == "yourname" && password == "yourpwd")
            {
                GenericPrincipal principal =
                             new GenericPrincipal(identity,
                                                  new string[] { "user" });
                return principal;
            }
            else
            {
                return null;
            }
        }
    }
}
