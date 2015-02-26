using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWModels;

namespace PDWModels.SpecRequests
{
	public class ProjectStatus
	{
		public const int Step1 = 1;
		public const int Step2 = 2;
		public const int Step3 = 3;
		public const int Step4 = 4;
		public const int Step5 = 5;
		public const int Step6 = 6;
		public const int Step7 = 7;

		public static string GetDisplayString( int stepNum )
		{
			switch( stepNum )
			{
				case Step1:
					return "Step 1";
				case Step2:
					return "Step 2";
				case Step3:
					return "Step 3";
				case Step4:
					return "Step 4";
				case Step5:
					return "Step 5";
				case Step6:
					return "Step 6";
				case Step7:
					return "Step 7";
			}

			return "";
		}

		public static IEnumerable<IDToTextItem> List
		{
			get
			{
				return new List<IDToTextItem>()
				{
					new IDToTextItem() { ID = Step1, Text = GetDisplayString( Step1 ) },
					new IDToTextItem() { ID = Step2, Text = GetDisplayString( Step2 ) },
					new IDToTextItem() { ID = Step3, Text = GetDisplayString( Step3 ) },
					new IDToTextItem() { ID = Step4, Text = GetDisplayString( Step4 ) },
					new IDToTextItem() { ID = Step5, Text = GetDisplayString( Step5 ) },
					new IDToTextItem() { ID = Step6, Text = GetDisplayString( Step6 ) },
					new IDToTextItem() { ID = Step7, Text = GetDisplayString( Step7 ) },
				};
			}
		}
	}
}
