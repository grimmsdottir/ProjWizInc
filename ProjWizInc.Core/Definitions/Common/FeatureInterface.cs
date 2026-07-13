using ProjWizInc.Core.Definitions.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Definitions.Common {
    // This tells the Serializer: "When you see an IFeatureInterface, look for the $type field"
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    // This maps the string "payout" in JSON to the actual C# class PayoutFeature
    [JsonDerivedType(typeof(PayoutFeature), "payout")]
    [JsonDerivedType(typeof(RequiresTicksFeature), "ticks")]
    public interface IFeatureInterface {
        //we can add some fields here later like name or description for tooltips or flavor
    }
}
