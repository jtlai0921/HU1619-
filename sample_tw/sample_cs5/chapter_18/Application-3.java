package org.zhangyafei;

import org.red5.server.adapter.ApplicationAdapter;
import org.red5.server.api.IConnection;
import org.red5.server.api.Red5;
import org.red5.server.api.service.IServiceCapableConnection;

/*****************************************************************
 * Application.java
 * 該類用於演示用戶端和服務端方法的互相調用
 * @author   zhang-yafei.com
 * @version  1.0.0.128  2009/2/20
 * @since    JDK1.5 Red5 0.7
 *****************************************************************/
public class Application extends ApplicationAdapter { 
    /**
	 * 被用戶端調用的方法，當接收到調用時，在該方法體內會調用用戶端上的方法
	 * 
	 * @return
	 */
    public void calledByClient() {  
		// 使用靜態方法獲取當前連接
        IConnection conn = Red5.getConnectionLocal(); 
		// 檢查連接是否是實現IServiceCapableConnection介面
		// 只有實現該介面才能實現調用
        if (conn instanceof IServiceCapableConnection) {
			// 顯式類型轉換
            IServiceCapableConnection sc = (IServiceCapableConnection) conn; 
			// 調用用戶端上的方法，並傳遞參數
            sc.invoke("calledByServer", new Object[]{"嗨！這是伺服器傳遞的參數。"});  
        }
    }  
}
