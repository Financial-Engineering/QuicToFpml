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
    using System.IO;
    using Quic.FPML;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLegFixed.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "12.0.0.0")]
    public partial class SwapLegFixed : SwapLeg
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            
            #line 4 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLegFixed.tt"
 
  // Run the base template:
  base.TransformText();

            
            #line default
            #line hidden
            return this.GenerationEnvironment.ToString();
        }
        
        #line 8 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLegFixed.tt"

protected override void GetType()
{

        
        #line default
        #line hidden
        
        #line 11 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLegFixed.tt"
this.Write("\tstrType, S, CouponFixed\r\n");

        
        #line default
        #line hidden
        
        #line 13 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLegFixed.tt"

}
protected override void LegDetails()
{

        
        #line default
        #line hidden
        
        #line 17 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLegFixed.tt"
this.Write("\tarFixedRate, AR, ");

        
        #line default
        #line hidden
        
        #line 18 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLegFixed.tt"
this.Write(this.ToStringHelper.ToStringWithCulture(arFixedRate));

        
        #line default
        #line hidden
        
        #line 19 "C:\Users\rlewis\Dropbox\projects\Visual Studio 2013\Quic.WebService\Quic.Templates\Swap\SwapLegFixed.tt"

}

        
        #line default
        #line hidden
    }
    
    #line default
    #line hidden
}