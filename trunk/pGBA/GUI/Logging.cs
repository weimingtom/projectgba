/*
 * Created by SharpDevelop.
 * User: user
 * Date: 6/10/2008
 * Time: 5:10 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace pGBA
{
	/// <summary>
	/// Description of Logging.
	/// </summary>
	public class Logging
	{
		private string log;
		
		public Logging()
		{
		}
		
		public void WriteLog(string text)
		{
			log += text;
		}
		
		public string ReturnLog()
		{
			return log;
		}
		
		public void ClearLog()
		{
			log = "";
		}
	}
}
