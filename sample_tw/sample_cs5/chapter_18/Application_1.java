package org.zhangyafei;

import org.red5.server.adapter.ApplicationAdapter;
import org.red5.server.Client;
import org.red5.server.Scope;
import org.red5.server.api.IClient;
import org.red5.server.api.IConnection;
import org.red5.server.api.IScope;

/*****************************************************************
 * Application.java
 * 該類用於演示接受連接和斷開連接
 * @author   zhang-yafei.com
 * @version  1.0.0.128  2009/2/20
 * @since    JDK1.5 Red5 0.7
 *****************************************************************/
public class Application extends ApplicationAdapter {

    @Override  
    public boolean appStart(IScope scope) {  
        System.out.println("[01]應用程式啟動.....................");
		return super.appStart(scope);
    }  
      
    /**
	 * 處理器方法
	 * 當Flash用戶端要連接到Red5 App，它首先會調用connect方法
	 * connect方法調用appConnect方法
	 * 
	 * @param   IConnection	 conn	  當前連接,包含有連接資訊
	 * @param   Object[]     params	  連接傳遞的參數
	 * @return	boolean
	 */
	@Override 
    public boolean appConnect(IConnection conn, Object[] params) {
		System.out.println("[02]應用程式連接....................."); 
		// params包含連接的參數，例如用戶名和密碼
		// 下麵，我們檢查用戶名和密碼，從而確定是否允許連接
		if (params.length > 1) {
			if (!(("zhangyafei").equals(params[0]) && ("verysecret").equals(params[1]))) {
				// 要拒絕連接可以直接返回false，或者調用rejectClient方法
				// rejectClient方法可以返回一個提示資訊
				this.rejectClient("用戶名或密碼錯誤") ;
				return false;
			}
		} else {				
			return false;
		}
		return true;
    }  
      
    @Override  
    public void appDisconnect(IConnection conn) {  
        System.out.println("[03]用戶端斷開應用程式連接............");  
        super.appDisconnect(conn);  
    }  
      
    @Override  
    public void appStop(IScope scope) {  
        System.out.println("[04]應用程式停止.....................");  
        super.appStop(scope);  
    }
}
