using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Data.Definitions.Specifications.Interfaces {
    //this big block makes it so that if we bump into a string that isnt here, it will throw
    [JsonPolymorphic(
    TypeDiscriminatorPropertyName = "$type",
    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
    [JsonDerivedType(typeof(PayoutSpec), "payout")]
    [JsonDerivedType(typeof(TickProgressionSpec), "tickProgression")]
    public interface ISpecification {
    }
}
