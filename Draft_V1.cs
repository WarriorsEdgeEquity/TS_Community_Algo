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
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies
{
	public class iFVG : Strategy
	{
		
		// variables for using ATM
		private string atmStrategyId;
		private string atmStrategyOrderId;
		private bool   isAtmStrategyCreated = false;
		
		// variables for making sure only 1 order is submitted
		private bool	inTrade = false;
		private int		barNumber = -1;
		
		// List to store all detected Fair Value Gaps (FVGs)
		private List<FVG> fvgList = new List<FVG>();
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"TopStep Discord Community Strategy Build";
				Name										= "iFVG";
				Calculate									= Calculate.OnEachTick;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy				= true;
				ExitOnSessionCloseSeconds					= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.Standard;
				Slippage									= 0;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce									= TimeInForce.Gtc;
				TraceOrders									= false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;

				// turn on off inverted fvgs
				UseiFVG							= true;
				
				// variables for setting trade direction
				TradeDirection					= @"long"; 		// short
				TradeType						= @"market";	// limit
				TradeLimitPrice					= 0;
				TradeStopPrice					= 0;

				// your ATM template name needs to match this
				ATMname							= @"ATMstrategy";
			}
			else if (State == State.Configure)
			{
			}
		}

		protected override void OnAccountItemUpdate(Cbi.Account account, Cbi.AccountItem accountItem, double value)
		{
			
		}

		protected override void OnConnectionStatusUpdate(ConnectionStatusEventArgs connectionStatusUpdate)
		{
			
		}

		protected override void OnExecutionUpdate(Cbi.Execution execution, string executionId, double price, int quantity, 
			Cbi.MarketPosition marketPosition, string orderId, DateTime time)
		{
			
		}

		protected override void OnOrderUpdate(Cbi.Order order, double limitPrice, double stopPrice, 
			int quantity, int filled, double averageFillPrice, 
			Cbi.OrderState orderState, DateTime time, Cbi.ErrorCode error, string comment)
		{
			
		}

		protected override void OnPositionUpdate(Cbi.Position position, double averagePrice, 
			int quantity, Cbi.MarketPosition marketPosition)
		{
			
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
			//highOfDay = High[0]; // Replace with actual logic to calculate high of the day
			//lowOfDay = Low[0]; // Replace with actual logic to calculate low of the day

			// check for fvgs and add them to a list
			DetectFVG();
			
			// Make sure there is a few bars since last trade
			if ((barNumber + 1) < CurrentBar && inTrade)
			{
				if (PositionAccount.MarketPosition == MarketPosition.Flat)
				{
					inTrade = false;
				}
			}
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
			public bool IsClosed { get; set; } = false;
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
			if (!inTrade)
			{
				if ((UseiFVG))
				{
					if (CheckForFVGClosure())
					{
						return true;
					}
				}
			}
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
		
		// CHECK FOR FVG CLOSE
		private bool CheckForFVGClosure()
		{
		    // Make sure there are FVGs in the list before proceeding
		    if (fvgList.Count == 0)
		        return false;
			
			// Drop the oldest fair value gaps
			while (fvgList.Count > 2)
			{
				fvgList.RemoveAt(0);
			}
		
		    // Loop through all FVGs in the list to check if they are closed
		    for (int i = 0; i < fvgList.Count; i++)
		    {
		        FVG fvg = fvgList[i];
		
		        // Only check FVGs that are still open (not closed)
		        if (!fvg.IsClosed)
		        {
		            // Check if the current bar's price closes the FVG
		            if (fvg.Type == FVGType.Bullish && Close[0] < fvg.StartPrice)
		            {
		                fvg.IsClosed = true; // Mark as closed
		                Print($"Bullish FVG closed at bar {CurrentBar}, StartPrice: {fvg.StartPrice}");
						TradeDirection 	= "short";
						TradeType 		= "market";
						return true;
		            }
		            else if (fvg.Type == FVGType.Bearish && Close[0] > fvg.StartPrice)
		            {
		                fvg.IsClosed = true; // Mark as closed
		                Print($"Bearish FVG closed at bar {CurrentBar}, EndPrice: {fvg.EndPrice}");
						TradeDirection 	= "long";
						TradeType 		= "market";
						return true;
		            }
		        }
		    }
			return false;
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
			
			// flip the flag for in a trade
			inTrade = true;
			barNumber = CurrentBar;
			
			// use ATM for handling the target and stop
			useATM(isLong, isMarket, TradeLimitPrice, TradeStopPrice);
        }
		#endregion
		
		#region Properties
		[NinjaScriptProperty]
		[Display(Name="UseiFVG", Order=1, GroupName="Parameters")]
		public bool UseiFVG
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="TradeDirection", Order=2, GroupName="Parameters")]
		public string TradeDirection
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="TradeType", Order=3, GroupName="Parameters")]
		public string TradeType
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="TradeLimitPrice", Order=4, GroupName="Parameters")]
		public double TradeLimitPrice
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="TradeStopPrice", Order=5, GroupName="Parameters")]
		public double TradeStopPrice
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="ATMname", Order=6, GroupName="Parameters")]
		public string ATMname
		{ get; set; }
		#endregion

	}
}
