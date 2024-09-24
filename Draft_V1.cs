#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
// using NinjaTrader.NinjaScript.Indicators.Gemify;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies
{

	public class OBR : Strategy
	{	

		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{



				Description									    = @"TopStep_Community_Algo";
				Name										        = "TX";
				Calculate									      = Calculate.OnEachTick;
				EntriesPerDirection							= 1;
				EntryHandling								    = EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy		= true;
				ExitOnSessionCloseSeconds				= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.Standard;
				Slippage									      = 0;
				StartBehavior								    = StartBehavior.WaitUntilFlat;
				TimeInForce									    = TimeInForce.Gtc;
				TraceOrders									    = false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;


			}
			else if (State == State.Configure)
			{

			}
			
			else if (State == State.DataLoaded)
			{
				ClearOutputWindow(); //Clears Output window every time strategy is enabled
			}
