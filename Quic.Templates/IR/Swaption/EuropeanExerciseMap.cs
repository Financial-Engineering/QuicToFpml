﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 11.0.0.0
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
    
    #line 1 "D:\QuIC\etl\code\Quic.WebService\Quic.Templates\IR\Swaption\EuropeanExerciseMap.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "11.0.0.0")]
    public partial class EuropeanExerciseMap : Quic.Templates.TradeMap
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            
            #line 3 "D:\QuIC\etl\code\Quic.WebService\Quic.Templates\IR\Swaption\EuropeanExerciseMap.tt"
 
  // Run the base template:
  base.TransformText();

            
            #line default
            #line hidden
            return this.GenerationEnvironment.ToString();
        }
        
        #line 7 "D:\QuIC\etl\code\Quic.WebService\Quic.Templates\IR\Swaption\EuropeanExerciseMap.tt"

protected override void GetMapType()
{

        
        #line default
        #line hidden
        
        #line 10 "D:\QuIC\etl\code\Quic.WebService\Quic.Templates\IR\Swaption\EuropeanExerciseMap.tt"
this.Write("\tstrType, S, EuropeanSwaption\r\n");

        
        #line default
        #line hidden
        
        #line 12 "D:\QuIC\etl\code\Quic.WebService\Quic.Templates\IR\Swaption\EuropeanExerciseMap.tt"

}
protected override void MapDetails() 
{

        
        #line default
        #line hidden
        
        #line 16 "D:\QuIC\etl\code\Quic.WebService\Quic.Templates\IR\Swaption\EuropeanExerciseMap.tt"
this.Write("\tadtExpiry, AD, ");

        
        #line default
        #line hidden
        
        #line 17 "D:\QuIC\etl\code\Quic.WebService\Quic.Templates\IR\Swaption\EuropeanExerciseMap.tt"
this.Write(this.ToStringHelper.ToStringWithCulture(adtExpiry));

        
        #line default
        #line hidden
        
        #line 17 "D:\QuIC\etl\code\Quic.WebService\Quic.Templates\IR\Swaption\EuropeanExerciseMap.tt"
this.Write("\r\n\tadtSettlement, AD, ");

        
        #line default
        #line hidden
        
        #line 18 "D:\QuIC\etl\code\Quic.WebService\Quic.Templates\IR\Swaption\EuropeanExerciseMap.tt"
this.Write(this.ToStringHelper.ToStringWithCulture(adtSettlement));

        
        #line default
        #line hidden
        
        #line 18 "D:\QuIC\etl\code\Quic.WebService\Quic.Templates\IR\Swaption\EuropeanExerciseMap.tt"
this.Write("\r\n\tstrPositionType, S, ");

        
        #line default
        #line hidden
        
        #line 19 "D:\QuIC\etl\code\Quic.WebService\Quic.Templates\IR\Swaption\EuropeanExerciseMap.tt"
this.Write(this.ToStringHelper.ToStringWithCulture(strPositionType));

        
        #line default
        #line hidden
        
        #line 19 "D:\QuIC\etl\code\Quic.WebService\Quic.Templates\IR\Swaption\EuropeanExerciseMap.tt"
this.Write("\r\n\tstrSettlementType, S, ");

        
        #line default
        #line hidden
        
        #line 20 "D:\QuIC\etl\code\Quic.WebService\Quic.Templates\IR\Swaption\EuropeanExerciseMap.tt"
this.Write(this.ToStringHelper.ToStringWithCulture(strSettlementType));

        
        #line default
        #line hidden
        
        #line 20 "D:\QuIC\etl\code\Quic.WebService\Quic.Templates\IR\Swaption\EuropeanExerciseMap.tt"
this.Write("\r\n\tpSwaptionVol, C, ");

        
        #line default
        #line hidden
        
        #line 21 "D:\QuIC\etl\code\Quic.WebService\Quic.Templates\IR\Swaption\EuropeanExerciseMap.tt"
this.Write(this.ToStringHelper.ToStringWithCulture(strCurrency));

        
        #line default
        #line hidden
        
        #line 21 "D:\QuIC\etl\code\Quic.WebService\Quic.Templates\IR\Swaption\EuropeanExerciseMap.tt"
this.Write(".SwaptionVolMtx.");

        
        #line default
        #line hidden
        
        #line 21 "D:\QuIC\etl\code\Quic.WebService\Quic.Templates\IR\Swaption\EuropeanExerciseMap.tt"
this.Write(this.ToStringHelper.ToStringWithCulture(strCurrency));

        
        #line default
        #line hidden
        
        #line 21 "D:\QuIC\etl\code\Quic.WebService\Quic.Templates\IR\Swaption\EuropeanExerciseMap.tt"
this.Write("\r\n");

        
        #line default
        #line hidden
        
        #line 22 "D:\QuIC\etl\code\Quic.WebService\Quic.Templates\IR\Swaption\EuropeanExerciseMap.tt"

}

        
        #line default
        #line hidden
    }
    
    #line default
    #line hidden
}