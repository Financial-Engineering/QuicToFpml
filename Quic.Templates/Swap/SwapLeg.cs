﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 12.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Quic.Templates
{
    using Quic.FPML;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLeg.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "12.0.0.0")]
    public partial class SwapLeg : Quic.Templates.Trade
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            
            #line 3 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLeg.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(strName));
            
            #line default
            #line hidden
            this.Write(", {\r\n");
            
            #line 4 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLeg.tt"

	GetType();

            
            #line default
            #line hidden
            this.Write("\tstrCurrency, S, ");
            
            #line 7 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLeg.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(strCurrency));
            
            #line default
            #line hidden
            this.Write("\r\n\tstrNotionalExchange, S, ");
            
            #line 8 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLeg.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(strNotionalExchange));
            
            #line default
            #line hidden
            this.Write("\r\n\tarNotional, AR,  ");
            
            #line 9 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLeg.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(arNotional));
            
            #line default
            #line hidden
            this.Write("\r\n\tadtStart, AD,  ");
            
            #line 10 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLeg.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(adtStart));
            
            #line default
            #line hidden
            this.Write("\r\n\tadtEnd, AD,  ");
            
            #line 11 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLeg.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(adtEnd));
            
            #line default
            #line hidden
            this.Write("\r\n\tadtPayment, AD,  ");
            
            #line 12 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLeg.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(adtPayment));
            
            #line default
            #line hidden
            this.Write("\r\n\tstrBusDayConv, S, ");
            
            #line 13 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLeg.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(strBusDayConv));
            
            #line default
            #line hidden
            this.Write("\r\n\tstrDaycount, S, ");
            
            #line 14 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLeg.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(strDaycount));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 15 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLeg.tt"
 
  LegDetails(); 

            
            #line default
            #line hidden
            this.Write("\t\r\n\tpYieldInfo, C, ");
            
            #line 18 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLeg.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(strCurrency));
            
            #line default
            #line hidden
            this.Write(".Yield.");
            
            #line 18 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLeg.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(strCurrency));
            
            #line default
            #line hidden
            this.Write("\r\n\tpFXInfo, C, ");
            
            #line 19 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLeg.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(strCurrency));
            
            #line default
            #line hidden
            this.Write(".Exchange.USD\r\n\tstrName, S, ");
            
            #line 20 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLeg.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(strName));
            
            #line default
            #line hidden
            this.Write("\r\n}\r\n");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 22 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLeg.tt"

	protected virtual void GetType() {}
	protected virtual void LegDetails() {}

        
        #line default
        #line hidden
    }
    
    #line default
    #line hidden
}
