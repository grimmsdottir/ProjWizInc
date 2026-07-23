using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Data.Blueprints.Specifications.Interfaces {
    // This tells the Serializer: "When you see an IFeatureInterface, look for the $type field"
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    // This maps the string "payout" in JSON to the actual C# class PayoutFeature
    [JsonDerivedType(typeof(PayoutSpec), "payout")]
    [JsonDerivedType(typeof(RequiresTicksSpec), "ticks")]
    public interface ISpecification {
        //for our OCP, we can use this to identify the type of component for fetching during hotpath
        //int ComponentTypeId { get; }
        //we can add some fields here later like name or description for tooltips or flavor
    }
}
