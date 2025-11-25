=============================================================================
Ascensio_Ascencio_readme.txt
Assignment Packaging and Submission Requirements
=============================================================================

=============================================================================
i. START SCENE FILE
=============================================================================
Main Scene: Assets/Scenes/SampleScene.unity
Alternative scenes available:
- Assets/Scenes/MainMenu.unity (Main menu scene)

=============================================================================
ii. HOW TO PLAY AND TECHNOLOGY REQUIREMENTS TO OBSERVE
=============================================================================

CONTROLS:
- WASD: Move character
- Mouse: Look around/Camera control
- Space: Jump
- R: Return to last safe point (Safety Net)
- ESC: Pause menu

TECHNOLOGY REQUIREMENTS TO OBSERVE:
1. CHARACTER ANIMATIONS:
   - Multiple animal characters with idle, walk, run animations
   - Character controller with refined movement
   - Animation controllers for: Kitty and Tiger
   - Location: Assets/Models/ithappy/Animals_FREE/

2. CAMERA SYSTEM:
   - Third-person camera following player
   - Smooth camera movement and rotation
   - Camera anchor system for better control

3. ENVIRONMENT ASSETS:
   - Beanstalk model with physics colliders
   - Village assets and environment props
   - Ground platforms and terrain elements
   - Bridge models (Assets/Models/bridge/)
   - Cloud platforms (Assets/SimpleSky/)
   - Giant Fish model (Assets/Models/fish/)
   - Location: Assets/Models/

4. PHYSICS & COLLISION:
   - Character physics and collision detection
   - Ground detection system
   - Platform interaction
   - Movable box interaction

5. UI SYSTEM:
   - Pause menu functionality
   - Main menu system
   - TextMesh Pro integration for UI text

6. ASSET MANAGEMENT:
   - Git LFS for large assets
   - Proper prefab organization
   - Material and texture management

=============================================================================
iii. KNOWN PROBLEM AREAS
=============================================================================

CURRENT KNOWN ISSUES:
1. The stairs to the treehouse has some collider issue causing the character difficult to walk up.
2. The village is for background only, it has no gameplay.
3. Camera needs to follow better on the character when it backs or turns. 
4. Camera collides and blocks view of objects covering the camera
5. There are some collider issues with the cloud implementations

POTENTIAL AREAS FOR IMPROVEMENT:
1. Animation transitions could be smoother
2. Camera collision with environment objects
3. Physics tuning for character movement

TESTING RECOMMENDATIONS:
- Test character movement on all platform types
- Verify animal animations play correctly
- Check pause menu functionality
- Test camera behavior in different environments

=============================================================================
PROJECT STRUCTURE NOTES
=============================================================================
Assets/
  ├── Models/               (3D models, textures, and animation controllers)
  ├── Scenes/               (Unity scene files)
  ├── Scripts/              (C# scripts)
  ├── Material/             (Materials)
  └── TextMesh Pro/         (UI text system)

=============================================================================
VERSION INFORMATION
=============================================================================
Unity Version: 6000.0.55f1
Git Repository: https://github.gatech.edu/jlee3973/Ascencio
Last Updated: November 24, 2025
Branch: main

=============================================================================
TEAM MEMBERS:
Curtis Cao
Zade Feng
Riley Vaupel
Jiwon Lee
Samantha Taing
=============================================================================

=============================================================================
iv. MANIFEST OF FILES AUTHORED BY EACH TEAMMATE
=============================================================================
Curtis Cao:
- Responsibilities: Character basic control script, character camera system, asset integration, level design, gameplay mechanics (collectibles, checkpoints, interactive platforms)
- Assets Implemented:
  * Assets/Scripts/CharacterControl/BasicControlScript.cs (character movement and control)
  * Assets/Scripts/PlayerCamera.cs (camera control system)
  * Assets/Scenes/SampleScene.unity (Complete level redesign with new platforms)
  * Assets/Scripts/Leaf/BouncyLeaf.cs (Bouncy leaf mechanic)
  * Assets/CloudElevator.cs (Elevator cloud mechanic)
  * Assets/Scripts/Coin/CoinPickup.cs & CoinVFX.cs (Coin collection system)
  * Assets/Scripts/Utility/SafetyNet.cs (Safety net/checkpoint system)
  * Assets/Models/bridge/ & Assets/Models/low-poly-bridge/ (Bridge assets)
  * Assets/Models/beanstalk/ (imported and integrated beanstalk model)
  * Assets/Models/lowpoly-village/ (imported and integrated village assets)
 
Zade Feng:
- Responsibilities: Technical algorithms, AI implementation, character jump mechanics
- Assets Implemented:
  * Assets/Scripts/CharacterControl/BasicControlScript.cs (Jump functionality implementation in character controller)
  * Technical algorithm scripts for character physics
  * Assets/Scripts/CharacterControl/EnemyMovement.cs (AI-related code components)
  * Added particle effects
 
Riley Vaupel:
- Responsibilities: Project organization, sound design, audio implementation
- Assets Implemented:
  * Assets/Scripts/AppEvents/AudioEventManager.cs (sound effect management)
  * Assets/Scripts/AppEvents/EnemyCollisionEvent.cs (trigger hit sound)
  * Assets/Scripts/AppEvents/FootstepEvent.cs (trigger footstep sound)
  * Assets/Scripts/AppEvents/HissEvent.cs (trigger hiss sound)
  * Assets/Scripts/AppEvents/HitGroundEvent.cs (trigger land sound)
  * Assets/Scripts/AppEvents/HitGroundReporter.cs (report sound on object collision)
  * Assets/Scripts/AppEvents/TigerGrowlEvent.cs (trigger tiger growl sound)
  * Assets/Scripts/AppEvents/TigerRoarEvent.cs (trigger tiger roar sound)
  * Assets/Scripts/CharacterControl/BasicControlScript.cs (character movement)
  * Assets/Scripts/CharacterControl/CharacterCommon.cs (ground detection)
  * Assets/Scripts/CharacterControl/EnemyMovement.cs (Enemy AI behavior)
  * Assets/Scripts/Utility/MusicBehavior.cs (background music behavior)
  * All sounds files in Assets/Sound/
  * Project documentation and goal specifications
  * World bounds to keep player in playable area
 
Jiwon Lee:
- Responsibilities: UI system design and implementation
- Assets Implemented:
  * Assets/Scripts/SceneUI/PauseMenuToggle.cs (pause menu functionality)
  * Assets/Scripts/SceneUI/GameStarter.cs (game state management)
  * Assets/Scenes/MainMenu.unity (main menu scene)
  * UI canvas components and menu systems
  * TextMesh Pro UI integration
  * Assets/Scenes/FishWinScene.unity (1 of 2 win screens)
  * Assets/Scenes/BoatWinScene.unity (1 of 2 win screens)
  * Assets/CreditShow.cs (showing the credits in main menu functionality)
  * Assets/Images (Tutorial Signs and Sign designs implementation)
  * Assets/Scripts/SceneUI/WinScreen.cs (win screen functionality)
  * Assets/Scripts/SceneUI/SceneTransition (fade to black scene transition functionality)
 
Samantha Taing:
- Responsibilities: Main character model, animations, character scripts, movable crate model, day/night skybox lighting
- Assets Implemented:
  * Assets/Models/ithappy/Animals_FREE/Prefabs/Kitty_001.prefab (main character model)
  * Assets/Models/ithappy/Animals_FREE/Animations/Animation_Controllers/Kitty.controller (character animation controller)
  * Assets/Models/Hocker/Worn wooden crate/Box/Prefabs/box_low (1).prefab (wooden crate model)
  * Assets/Scripts/AppEvents/DayNightCycle.cs (day/night skybox lighting)
  * Assets/SimpleSky (cloud model integration)
  * Assets/Scripts/CharacterControl/BasicControlScript.cs (main character movement/animation)
  * Character animation files (idle, walk, run animations)
  * Character physics and collision scripts

v. RECENT UPDATES
- Redesigned level layout in SampleScene.unity with new platforms (bridges, clouds)
- Implemented interactive elements: Bouncy Leaves and Elevator Clouds
- Added Coin Collection system and Giant Fish reward
- Added Safety Net system (Respawn on 'R')
- Improved navigation mesh

=============================================================================
END OF README
=============================================================================
