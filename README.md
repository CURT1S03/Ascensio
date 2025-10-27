=============================================================================
Ascencio_readme.txt
Assignment Packaging and Submission Requirements
=============================================================================

=============================================================================
i. START SCENE FILE
=============================================================================
Main Scene: Assets/Scenes/SampleScene.unity
Alternative scenes available:
- Assets/Scenes/MainMenu.unity (Main menu scene)
- Assets/Scenes/JiwonScene.unity (Additional test scene)

=============================================================================
ii. HOW TO PLAY AND TECHNOLOGY REQUIREMENTS TO OBSERVE
=============================================================================

CONTROLS:
- WASD: Move character
- Mouse: Look around/Camera control
- Space: Jump
- ESC: Pause menu

TECHNOLOGY REQUIREMENTS TO OBSERVE:
1. CHARACTER ANIMATIONS:
   - Multiple animal characters with idle, walk, run animations
   - Character controller with refined movement
   - Animation controllers for: Kitty and Tiger
   - Location: Assets/ithappy/Animals_FREE/

2. CAMERA SYSTEM:
   - Third-person camera following player
   - Smooth camera movement and rotation
   - Camera anchor system for better control

3. ENVIRONMENT ASSETS:
   - Beanstalk model with physics colliders
   - Village assets and environment props
   - Ground platforms and terrain elements
   - Location: Assets/Models/

4. PHYSICS & COLLISION:
   - Character physics and collision detection
   - Ground detection system
   - Platform interaction

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
1. The starts to the treehouse has some collider issue causing the character difficult to walk up.
2. The village is for backgournd only, it has no gameplay.
3. Camera needs to follow better on the character when it backs or turns. 

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
  ├── Models/               (3D models and textures)
  ├── Scenes/               (Unity scene files)
  ├── Scripts/              (C# scripts)
  ├── ithappy/Animals_FREE/ (Character animations and controllers)
  ├── Material/             (Materials)
  └── TextMesh Pro/         (UI text system)

=============================================================================
VERSION INFORMATION
=============================================================================
Unity Version: 6000.0fff1
Git Repository: https://github.gatech.edu/jlee3973/Ascencio
Last Updated: October 27, 2025
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
- Responsibilities: Character basic control script, character camera system, asset integration, level design
- Assets Implemented:
  * Assets/Scripts/CharacterControl/BasicControlScript.cs (character movement and control)
  * Assets/Scripts/PlayerCamera.cs (camera control system)
  * Assets/Models/beanstalk/ (imported and integrated beanstalk model)
  * Assets/Models/lowpoly-village/ (imported and integrated village assets)
  * Assets/Scenes/SampleScene.unity (first level design and layout)
  * Various supporting environment assets and materials
 
Zade Feng:
- Responsibilities: Technical algorithms, AI implementation, character jump mechanics
- Assets Implemented:
  * Jump functionality implementation in character controller
  * Technical algorithm scripts for character physics
  * AI-related code components
 
Riley Vaupel:
- Responsibilities: Project organization, sound design, audio implementation
- Assets Implemented:
  * Audio system scripts
  * Sound effect assets and audio management
  * Project documentation and goal specifications
 
Jiwon Lee:
- Responsibilities: UI system design and implementation
- Assets Implemented:
  * Assets/Scripts/PauseMenuToggle.cs (pause menu functionality)
  * Assets/Scripts/Utility/GameStarter.cs (game state management)
  * Assets/Scenes/MainMenu.unity (main menu scene)
  * UI canvas components and menu systems
  * TextMesh Pro UI integration
 
Samantha Taing:
- Responsibilities: Main character model, animations, and character scripts
- Assets Implemented:
  * Assets/ithappy/Animals_FREE/Prefabs/Kitty_001.prefab (main character model)
  * Assets/ithappy/Animals_FREE/Animations/Animation_Controllers/Kitty.controller (character animation controller)
  * Assets/ithappy/Animals_FREE/Scripts/CreatureMover.cs (character movement script)
  * Character animation files (idle, walk, run animations)
  * Character physics and collision scripts
=============================================================================
END OF README
=============================================================================
