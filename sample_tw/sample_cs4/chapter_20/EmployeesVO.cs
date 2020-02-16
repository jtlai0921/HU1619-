using System;
using System.Data;

namespace org.zhangyafei {
    
   public class EmployeesVO {
		
	  private int _totleRS;
      private DataTable _resultSet;	

       
      public int totleRS
      {
          get{ return _totleRS; }
          set{ _totleRS = value; }
      }

      public DataTable resultSet
      {
          get{ return _resultSet; }
          set{ _resultSet = value; }
      } 
   }
}
