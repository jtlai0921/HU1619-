package org.zhangyafei
{
	import fl.controls.Button;
	import fl.controls.DataGrid;
	import fl.controls.listClasses.CellRenderer;
	import fl.controls.listClasses.ListData;
	import flash.events.MouseEvent;

	/**
	 * 該類用於將Button嵌入到基於List的組件
	 * @author   zhang-yafei.com
	 * @version  1.0.0.128  2008/10/20
	 * @since    FlashPlayer8.5
	 */
	public class ButtonCell
	             extends CellRenderer{
					 
		public var button:Button;

		public function ButtonCell() {
			button = new Button();
			addChild(button);
			this.addEventListener(MouseEvent.CLICK, clickHandler);
		}
		override public function set data(d:Object):void {
			_data=d;
			button.label="刪除";                // Button的Label
			button.setStyle("icon", Icon_Del);
		}
		override public function get data():Object {
			return _data;
		}		
		override protected function draw():void {
			super.draw();
		}
		override protected function drawLayout():void {
			button.setSize(50, 20);
			button.drawNow();
			if (contains(textField)) {
				removeChild(textField);
			}
		}

		/**
	     * 該方法處理ButtonCell單擊事件，刪除當前行資料
	     */        
		private function clickHandler(evt:MouseEvent):void {
			// if DataGrid
			var oDataGrid:DataGrid = DataGrid(parent.parent.parent);
			oDataGrid.removeItemAt(_listData.row);
		}				
	}
}
