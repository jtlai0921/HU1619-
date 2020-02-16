package org.flashremoting
{
   public class PersonVO {
	
	  // 定義屬性
	  public var name:String ="Sharon沙朗";
	  public var surname:String ="Stone斯通";
	  public var country:String ="美利堅合眾國";
	  public var favourites_artists:Array = ["Michael Douglas邁克爾 道格拉斯",
                                      "William Baldwin威廉 鮑德溫", 
                                      "Gene Hackman吉恩 哈克曼",
                                      "David Morrissey大衛 莫里斯"];

	  protected var city:String ="賓夕法尼亞州";
	  private   var age:int = 48;
	  internal  var role:String ="主角"; 
	
      // 定義一個方法
      public function printDetails():void{
		// 遍曆物件屬性及其值
		trace("name======="+this.name);
		trace("surname======="+this.surname);
		trace("country======="+this.country);
		trace("favourites_artists======="+this.favourites_artists);
      }
   }
}
