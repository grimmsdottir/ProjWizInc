using Microsoft.VisualStudio.TestPlatform.CrossPlatEngine;
using ProjWizInc.Engine.Data.ADT;
using ProjWizInc.Engine.Data.Definitions;
using ProjWizInc.Engine.Data.Definitions.Defs;
using ProjWizInc.Engine.Data.Definitions.Specifications.Interfaces;
using ProjWizInc.Engine.Simulation.Registries;
using ProjWizInc.Engine.Tests.TestGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Tests.Simulation.Registries {
    // 1. Declare minimal stub types to keep the tests isolated from real game metadata
    public class StubResource { }
    public class StubJob { }

    public class DefinitionRegistryTests {
        [Fact]
        public void RegisterMap_ShouldSuccessfullyAddMap_AndGetMapShouldRetrieveIt() {
            // Arrange - Decoupled completely from your real data layers
            DefinitionRegistry registry = new DefinitionRegistry();

            DualKeyMap<StubResource> resourceMap = TestDataFactory.CreateMinimalMap<StubResource>(2);

            // Act
            registry.RegisterMap<StubResource>(resourceMap);

            // Assert
            DualKeyMap<StubResource> retrievedMap = registry.GetMap<StubResource>();
            Assert.Same(resourceMap, retrievedMap);
        }

        [Fact]
        public void RegisterMap_ShouldThrowException_WhenRegisteringSameTypeTwice() {
            // Arrange
            DefinitionRegistry registry = new DefinitionRegistry();
            DualKeyMap<StubResource> mapA = TestDataFactory.CreateMinimalMap<StubResource>(2);
            DualKeyMap<StubResource> mapB = TestDataFactory.CreateMinimalMap<StubResource>(2);

            registry.RegisterMap<StubResource>(mapA);

            // Local helper function instead of lambda expression
            void RegisterDuplicate() {
                registry.RegisterMap<StubResource>(mapB);
            }

            // Act & Assert
            Assert.Throws<InvalidOperationException>(RegisterDuplicate);
        }

        [Fact]
        public void GetMap_ShouldThrowException_WhenTypeIsNotRegistered() {
            // Arrange
            DefinitionRegistry registry = new DefinitionRegistry();

            // Local helper function instead of lambda expression
            void GetUnregistered() {
                registry.GetMap<StubJob>(); // StubJob was never registered
            }

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(GetUnregistered);
        }
    }
}
