using System;
using System.Collections;
using System.Security;
using System.Security.Principal;
using FluorineFx.Security;

namespace org.zhangyafei
{
    public class MyLoginCommand : GenericLoginCommand
    {
        // 重寫DoAuthentication以作出自己的角色系統定義
        public override IPrincipal DoAuthentication(string username,
                                                    Hashtable credentials)
        {
            // 獲取AMF報頭中的密碼
            string password = credentials["password"] as string;
            // 根據用戶名創建一個新的用戶標識
            GenericIdentity identity = new GenericIdentity(username);

            // 根據用戶名和密碼的判斷決定用戶的角色
            // 並創建一個新的用戶物件：GenericPrincipal實例
            // 其中包含了該用戶的標示和所屬的角色
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
