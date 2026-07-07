SE Stuff
Phase 1: Software Engineering: Requirements
Functional Requirements
Track Player resources and their increments
Persistent player progress, ie saving
Have income generators that can be upgraded
Have a simple visual novel style dialogue thing for plot
Have both generic and unique upgrades that boost various things
Have spells that do things, and some way of automating them
Have crafting, resources and equipment
Have combat, that is mostly automated, but with configurable tactics
Instead of prestige, the player is expected to fail and need to new game plus
Non-Functional Requirements
Portability - System should use a MVCish architecture to ensure that the game can be ported to not windows in the future
Performance - Game should be able to handle nonsensicaly large numbers at decent precision
Performance - Rendering should be independent of logic, to allow for super fast speedups to avoid UI lag
Phase 2: OOAD
Conceptual Domain Modeling
Player Profile - resources, stats, items etc
Tasks - buttons that can be clicked to start generating a resource. Can be upgraded to automatically progress without needing clicking
Generators - items that generate resources by existing
Gear - items that boost stats 
Upgrades - global modifiers to specific stuff
Game Loop Manager - The thing that coordinates time delta and program flow
CRC Cards
Game Loop Manager
Responsibilities
Track IRL time delta
Drive the main game loop
Throttle renderer
Collaborators
PlayerProfile - change resources and skills as actions progress
CombatManager - tick player and enemy actions
NarrativeManager - evaluate if dialogue should play depending on triggers
NarrativeManager
Responsibilities
Check if story should play
Optionally pause game while dialogue plays out
Collaborators
PlayerProfile - reads resources to tell if story should be played
MainForm - sends data to write the dialogue to UI
SpellEvaluator
Responsibilities
Evaluate conditions - do stuff when the time is right
Target priority - target set priority
Collaborators
CombatManager - execute spells
Phase 3: SAD
Architecture Layering
Presentation Layer
Some windows form thing that captures player instructions and renders the game in the form of presentable data
Domain Layer
Business logic where the manager classes,data structs, and game logic lives
Data Layer
A something that captures the domain layer so that it can be serialised and saved to file, as well as load a file to restore the domain layer on resume
Phase 4: Behavioral Modelling
Bootup
Read config data for options
Data Loading
Load save file
Active game
Logic should tick every 10ms/ 100 UPS
Renderer should tick every 16ms/ 60 FPS
Death/Newgame+
Reset everything, get bonuses/unlocks

Phase ?: Sequence Diagrams
Combat
GameLoopManager
Manages time
CombatManager
Query active tactics
TacticEvaluater
Check if current conditions match targeted conditions and trigger spells
CombatManager
Manages player and enemy resources
MainForm
Capture domain layer changes and update screen
Sprint Goals
Core Features:
Player profile
Tasks
Upgrades
Expansion 1:
Impossible goals and new game plus
Plot and dialogue
Expansion 2:
Non-combat spells
Expansion 3:
Combat and combat spells/skills/stats
Expansion 4:
Combat tactics and resource gathering
Expansion 5:
Inventory, crafting and equipment
Notable Patterns/Architectures
Model Viewer Controller(MVC)?
There should be 4layers
Data Layer
All data, no logic
IE the fun part of modding
Touched only by the logic
Service(Logic) Layer
Manages all game logic
Contains no specific data or state
Touches the data layer and runtime layer
Runtime Layer
Interconnecting layer between logic and render
The ‘actual’ game
Contains current state etc
Touches the render layer
Presentation Layer
Reads the domain layer and updates text and stuff on screen
The renderer
Data-Driven Programming
Content should NOT be in code ever
Should exists as Jsons and contain all the data
Avoids code spaghetti by making things like SpellClass uniform, if we need brand new functionality we can just add it to the class, and the json can be modified to call on the new functionality
Event Broker Pattern
Managers should not call each other directly
Instead, Managers should write to some universal log, that every other Manager can read and react to
Avoids code spaghetti also by making managers basically uncoupled
Strategy Pattern/OOP
Actual conditions should be interfaced from a common interface
Combat engine should work with the common interface, while the various conditions can be built without causing hiccups. Yay decoupling
Time Accumator
We use a time accumulator to measure how much time has passed between frames
If the accumulated time is greater than the targeted frame time, we run one frame and deduct it
This allows the game to not freak out in case of delayed frames
Structs and Classes
We use structs where possible for performance reasons
Mostly for game state stuff, and possibly for some of my custom ADTs
