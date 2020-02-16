package org.zhangyafei;

import org.red5.server.adapter.ApplicationAdapter;

/*****************************************************************
 * Application.java
 * 該類用於演示用戶端和服務端方法的互相調用
 * @author   zhang-yafei.com
 * @version  1.0.0.128  2009/2/20
 * @since    JDK1.5 Red5 0.7
 *****************************************************************/
public class Application extends ApplicationAdapter {
    /**
	 * 該方法將被用戶端調用
	 * 
	 * @param   String     param	  傳遞的參數
	 * @return	String
	 */
    public String calledByClient(String param) {
		return "嗨！這是你傳遞參數：" + param;
    }  
}
