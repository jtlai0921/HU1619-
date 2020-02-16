package {
	import flash.display.MovieClip;
	import flash.events.MouseEvent;
	import fl.controls.listClasses.ICellRenderer;
	import fl.controls.listClasses.ListData;
	import flash.utils.*;
	public class MyPreloader extends MovieClip implements ICellRenderer {
		private var _listData:ListData;
		private var _data:Object;
		private var _selected:Boolean;
		private var _intervalID:uint;
		public function MyPreloader() {
			//使用一個setInterval不斷更新資料
			_intervalID = setInterval(changeProgress, 200);
			this.stop_btn.addEventListener("click", stopHandler);
		}
		public function set data(d:Object):void {
			_data=d;
			this.loader_txt.text=d.data.data.toString()+"%";
			this.fileName_txt.text=d.label;
		}
		public function get data():Object {
			return _data;
		}
		public function set listData(ld:ListData):void {
			_listData=ld;
		}
		public function get listData():ListData {
			return _listData;
		}
		public function set selected(s:Boolean):void {
			_selected=s;
		}
		public function get selected():Boolean {
			return _selected;
		}
		public function setSize(width:Number, height:Number):void {
			//更改Preloader的寬度與List組件相同
			this.width=width;
		}
		public function setStyle(style:String, value:Object):void {
		}
		public function setMouseState(state:String):void {
		}
		private function changeProgress():void {
			//不斷更新資料
            if (_data.data.data >= 100) { 
                clearInterval(_intervalID);
				this.progress_mc.width=100;
				this.loader_txt.text="100%";
				this.fileName_txt.text=_data.label;
            } else { 
                this.progress_mc.width=_data.data.data;
				this.loader_txt.text=_data.data.data.toString()+"%";
            } 
        }
		private function stopHandler(evt:MouseEvent):void {
            _data.data.fileReference.cancel();
			fileName_txt.text="已被取消";
			_data.label="已被取消";
        };
	}
}
