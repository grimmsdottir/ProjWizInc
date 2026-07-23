using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Data.Definitions.Specifications.Interfaces {
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(PayoutSpec), "payout")]
    [JsonDerivedType(typeof(TickProgressionSpec), "tickProgression")]

    public interface ISpecification {
    }
}
