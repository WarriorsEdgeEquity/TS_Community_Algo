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



				Description								= @"TopStep_Community_Algo";
				Name									= "TX";
				Calculate								= Calculate.OnEachTick;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy						= true;
				ExitOnSessionCloseSeconds						= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.Standard;
				Slippage								= 0;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce								= TimeInForce.Gtc;
				TraceOrders								= false;
				RealtimeErrorHandling							= RealtimeErrorHandling.StopCancelClose;
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

   
   		protected override void OnBarUpdate()
		{

	            // Live trading logic here
	            // ---------------------------------
	
	            // 1. Identify important price levels (e.g., high and low of the day)
	            // Example placeholder:
	            highOfDay = High[0]; // Replace with actual logic to calculate high of the day
	            lowOfDay = Low[0]; // Replace with actual logic to calculate low of the day
	
	            // 2. Check for conditions to trigger a trade
	            if (TradeConditionMet()) // Placeholder method for your trade condition
	            {
	                EnterTrade(); // Method to handle trade entry
	            }
	
	            // 3. Implement exit strategies (e.g., stop loss, take profit)
	            if (Position.MarketPosition == MarketPosition.Long)
	            {
	                // Placeholder for logic to handle long positions (e.g., exit conditions)
	            }
	            else if (Position.MarketPosition == MarketPosition.Short)
	            {
	                // Placeholder for logic to handle short positions (e.g., exit conditions)
	            }
	        }
	
	        #region Methods
	
	        // Example placeholder method for trade condition logic
	        private bool TradeConditionMet()
	        {
	            // Add logic to check for trade conditions (e.g., engulfing pattern, price breakout)
	            // Return true if conditions met, otherwise false
	            return false;
	        }
	
	        // Example method for entering a trade
	        private void EnterTrade()
	        {
	            // Placeholder logic for entering a trade
	            // Example:
	            if (Position.MarketPosition == MarketPosition.Flat)
	            {
	                // Buy if flat and conditions met
	                EnterLong(); // Use EnterShort() for short trades
	            }
	        }


  		}

		#region Properties

		#region Trading Variables 
		[NinjaScriptProperty]
		[Display(Name = "Use iFVG", Order = 0, GroupName = "FVG Settings")]
		public bool UseiFVG { get; set; }
		#endregion
  
		#endregion
