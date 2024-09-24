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

	public class iFVG : Strategy
	{	

		private string atmStrategyId;
		private string atmStrategyOrderId;
		private bool   isAtmStrategyCreated = false;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{



				Description								= @"TopStep_Community_Algo";
				Name									= "iFVG";
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

				// variables for entering trade
				TradeDirection							= "long"; // short
				TradeType								= "market"; // limit
				TradeLimitPrice							= 0; // depends on symbol or bars
				TradeStopPrice							= 0; // depends on symbol or bars

				// the name of your ATM strategy in NinjaTrader should match this name v
				ATMname														= @"ATMstrategy";


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

				// ATM strategy only works on realtime data
				if (State < State.Realtime)
					return;
	
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
	            //if (Position.MarketPosition == MarketPosition.Long)
	            //{
	                // Placeholder for logic to handle long positions (e.g., exit conditions)
	            //}
	            //else if (Position.MarketPosition == MarketPosition.Short)
	            //{
	                // Placeholder for logic to handle short positions (e.g., exit conditions)
	            //}
	        }
	
	        #region Methods

			// Detect FVGs
			private void DetectFVG()
			{
				// Make sure there's enough historical data to compare
				if (CurrentBar < 3)
					return;

				// Check for Bullish Fair Value Gap
				if ((High[2] < Low[0]) && (Open[1] < Close[1]))
				{
					// Create and store a bullish FVG
					FVG fvg = new FVG
					{
						Type = FVGType.Bullish,
						StartBar = CurrentBar,
						StartPrice = High[2],
						EndPrice = Low[0]
					};
					fvgList.Add(fvg);

					Print($"Bullish FVG detected from {High[2]} to {Low[0]} at bar {CurrentBar}");
				}

				// Check for Bearish Fair Value Gap
				else if ((Low[2] > High[0]) && (Close[1] < Open[1]))
				{
					// Create and store a bearish FVG
					FVG fvg = new FVG
					{
						Type = FVGType.Bearish,
						StartBar = CurrentBar,
						StartPrice = Low[2],
						EndPrice = High[0]
					};
					fvgList.Add(fvg);

					Print($"Bearish FVG detected from {Low[2]} to {High[0]} at bar {CurrentBar}");
				}
			}
			// Example class to represent a Fair Value Gap (FVG)
			public class FVG
			{
				public FVGType Type { get; set; }
				public int StartBar { get; set; }
				public double StartPrice { get; set; }
				public double EndPrice { get; set; }
			}

			// Enum to differentiate between bullish and bearish FVGs
			public enum FVGType
			{
				Bullish,
				Bearish
			}
	        // Example placeholder method for trade condition logic
	        private bool TradeConditionMet()
	        {
	            // Add logic to check for trade conditions (e.g., engulfing pattern, price breakout)
	            // Return true if conditions met, otherwise false
	            return false;
	        }
			
			// ATM logic 
			private void useATM(bool isLong, bool isMarket, double limitPrice, double stopPrice)
			{
				// ATM variables
				OrderAction action = isLong ? OrderAction.Buy : OrderAction.Sell;
    			OrderType orderType = isMarket ? OrderType.Market : OrderType.Limit;

				if (isMarket)
				{
					limitPrice = 0;
					stopPrice = 0;
				}

				atmStrategyId = GetAtmStrategyUniqueId();
      			atmStrategyOrderId = GetAtmStrategyUniqueId();
 
      			AtmStrategyCreate(action, orderType, limitPrice, stopPrice, TimeInForce.Day,
          			atmStrategyOrderId, ATMname, atmStrategyId, (atmCallbackErrorCode, atmCallbackId) => {
 
          			// checks that the call back is returned for the current atmStrategyId stored
          			if (atmCallbackId == atmStrategyId)
          			{
              			// check the atm call back for any error codes
              			if (atmCallbackErrorCode == Cbi.ErrorCode.NoError)
              			{
                  			// if no error, set private bool to true to indicate the atm strategy is created
                  			isAtmStrategyCreated = true;
              			}
          			}
      			});
 
	  			if(isAtmStrategyCreated)
	  			{
	      			if (atmStrategyOrderId.Length > 0)
		  			{
			  			string[] status = GetAtmStrategyEntryOrderStatus(atmStrategyOrderId);
			  			// If the status call can't find the order specified, the return array length will be zero otherwise it will hold elements
			  			if (status.GetLength(0) > 0)
			  			{
				  			// Print out some information about the order to the output window
				  			Print("The entry order average fill price is: " + status[0]);
				  			Print("The entry order filled amount is: " + status[1]);
				  			Print("The entry order order state is: " + status[2]);
				  
				  			// If the order state is terminal, reset the order id value
				  			if (status[2] == "Filled" || status[2] == "Cancelled" || status[2] == "Rejected")
					  			atmStrategyOrderId = string.Empty;
							}
		  			} // If the strategy has terminated reset the strategy id
		  			else if (atmStrategyId.Length > 0 && GetAtmStrategyMarketPosition(atmStrategyId) == Cbi.MarketPosition.Flat)
			  			atmStrategyId = string.Empty;
		  
		  			if (atmStrategyId.Length > 0)
		  			{
			  			// You can change the stop price even when its being managed by the ATM strategy STOP19 is to prevent the STOP1 from being changed
						// more logic needed here if you want to do fancy things with the stop loss while it is still being managed by ATM strategy
			 			//if (GetAtmStrategyMarketPosition(atmStrategyId) != MarketPosition.Flat)
				  		//	AtmStrategyChangeStopTarget(0, Low[0] - 3 * TickSize, "STOP19", atmStrategyId);
			  
			  			// Print some information about the strategy to the output window, please note you access the ATM strategy specific position object here
			  			// the ATM would run self contained and would not have an impact on your NinjaScript strategy position and PnL
			  			Print("The current ATM Strategy market position is: " + GetAtmStrategyMarketPosition(atmStrategyId));
			  			Print("The current ATM Strategy position quantity is: " + GetAtmStrategyPositionQuantity(atmStrategyId));
			  			Print("The current ATM Strategy average price is: " + GetAtmStrategyPositionAveragePrice(atmStrategyId));
			  			Print("The current ATM Strategy Unrealized PnL is: " + GetAtmStrategyUnrealizedProfitLoss(atmStrategyId));
		  			}
	  			}
			}

	        // Example method for entering a trade
	        private void EnterTrade()
	        {
	            // Placeholder logic for entering a trade
	            // Example:

				bool isLong = false;
				bool isMarket = false;

				if (TradeDirection == "long")
				{
					isLong = true;
				}
				if (TradeType == "market")
				{
					isMarket = true;
				}

				useATM(isLong, isMarket, TradeLimitPrice, TradeStopPrice);
	        }


  		}

		#region Properties

		#region Trading Variables 
		[NinjaScriptProperty]
		[Display(Name = "Use iFVG", Order = 0, GroupName = "FVG Settings")]
		public bool UseiFVG { get; set; }
		#endregion
  
		#endregion
