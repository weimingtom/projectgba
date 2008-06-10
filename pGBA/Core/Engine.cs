/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 4/14/2008
 * Time: 5:32 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace pGBA
{
	/// <summary>
	/// Description of Engine.
	/// </summary>
	public class Engine
	{
		//Universal Constants
		public Armcpu myCPU;
		public Memory myMemory;
		public Logging myLog;
		
		public Engine()
		{
			myCPU = new Armcpu(this);
			myMemory = new Memory();
			myLog = new Logging();
		}
	}
}
