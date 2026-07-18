using ProjWizInc.Core.ADT;
using ProjWizInc.Core.Definitions;
using ProjWizInc.Core.Definitions.Blueprints;
using ProjWizInc.Core.Managers;
using ProjWizInc.Core.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xUnitTester {
    public class BootstrapperTests {
        [Fact]
        public void Bootstrapper_CanBuildValidContext_InMemory() {
            // Arrange
            GameDefinitions definitions = new GameDefinitions();

            ResourceDefinition testGold = new ResourceDefinition();
            testGold.Key = "gold";
            testGold.DisplayName = "Gold";
            definitions.Resources.Add(testGold);

            GameState gameState = new GameState();
            gameState.economyState.Resources = new BigNum[1];
            gameState.economyState.Resources[0] = 0;

            // Act
            CoreContext context = Bootstrapper.BuildContext(definitions, gameState);

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.GetTimeState());
            Assert.Equal(0, context.GetTimeState().TimeElapsed);
        }

        [Fact]
        public void Bootstrapper_InitializesResourceArrayCorrectly() {
            // Arrange
            GameDefinitions gameDefinitions = new GameDefinitions();    
            GameState gameState = new GameState();
            
            // Act
            CoreContext context = Bootstrapper.BuildContext(gameDefinitions, gameState);
            
            // Assert
            Assert.NotNull(gameState.economyState.Resources);
            Assert.Equal(2, gameState.economyState.Resources.Length);
        }

        [Fact]
        public void Bootstrapper_BuildsAllManagers() {
            // Arrange
            GameDefinitions definitions = new GameDefinitions();
            GameState gameState = new GameState();
            gameState.economyState.Resources = new BigNum[0];
            
            // Act
            CoreContext context = Bootstrapper.BuildContext(definitions, gameState);
            
            // Assert
            Assert.NotNull(context.DefinitionManager);
            Assert.NotNull(context.EventManager);
            Assert.NotNull(context.EconomyManager);
            Assert.NotNull(context.GameLoopManager);
            Assert.NotNull(context.JobManager);
            Assert.NotNull(context.TimeManager);
        }
    }
}
