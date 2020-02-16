package org.flashremoting
{
   public dynamic class Person {
	   
      // 定義一個方法
      public function printDetails():void{
		// 遍曆物件屬性及其值
		var i:Object;
		for (i in this) {
			trace(i+"======="+this[i]);
		}
      }
   }
}
