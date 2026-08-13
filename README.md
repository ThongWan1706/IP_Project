# IP_Project

**Author:** Lam Thong Wan, Xavier Ong Yi Cheng, Keno Ang Kheng Kiat

**Date:** 13 Aug 2026 

**Description:** This document will include informations regards on the project such as the gameplay, limitation & bugs faced in this project, FSM diagrams, implementation details of each AI, answers to the gameplay, references and credits to external assets.

# Project Rationale
Road safety education often struggles to engage the public when spreading awareness through traditional media such as posters, brochures, or talks. 
Despite Singapore’s world class transport infrastructure & public education campaigns, road safety risks continue to arise across all road users, drivers, and motorists. 

In high density areas such as HDB areas and busy junctions, dangerous behaviors such as jaywalking and distracted walking pose daily hazards. At the same time, driving offences such as speeding, distracted driving, or not wearing a helmet also create life-threatening situations.

To resolve this, Safetypin plans to create a game that would promote road safety to the public. SafeStreetsSG is a 1st person, interactive roadshow game that would help bridge this engagement gap.

By putting players in the shoes of a Singapore Police Officer, the game turns safety education into a hands-on experience. Instead of simply reading the rules, the player would actively spot the hazards and make real-time decisions, helping them see why road safety guidelines matter.

# Project Objective
Through this project, the team aims to achieve these objectives:
1. Help players learn to notice hazards for everyone on the road.
- Educate players on road safety by training them to spot, understand different hazards, and address critical hazards from all perspectives in realistic Singaporean environments:
  - Pedestrian Hazards: Spotting jaywalking, distracted walking, rushing across flashing Green Man signals.
  - Driver violations: Identifying unbelted passengers, distracted driving, absences of helmets and speeding.

2. Promote education over pure punishments
- Implementing a dual-metric scoring systems that rewards players for choosing educational and respectful dialogues over aggressive fining to reinforce road safety rules
  - Hazards Avoided: A point system where it accumulate every time a player successfully identifies and intervene before an accident occurs.
  - Community Trust: A dynamic health bar system where it’ll either increase or decrease based on the tone the player selected when addressing pedestrians and drivers.

# Introduction of the game
In this game, the player will be playing as a 1st person perspective as a police officer that patrols around the area to maintain the safety of the community. The goal of the player is to balance everyday community relations with hazard prevention through two core game mechanics:

1. Community Trust: A health bar system that rises or falls based on dialogue choice and tone when interacting with the NPCs.
2. Hazard Avoided: A point system where it was awarded by the scenarios where the player was able to stop the NPC before an accident occurred.

# Gameplay
The game consist of 3 levels which requires the player to investigate incidents and catch peoples that are attempting to violate the road rules, and either politely informing them or scold them.

**1. Day 1**
- For the 1st day, it will start up simple by having the player patrol around the area. While patrolling, the player will only encounter one incident for the day, which is witnessing an NPC attempting to jaywalk.
- When the NPC walks to a "trigger" area, the NPC will have a red outline while being slowed down for a while with addition of alarm sound, which provides visual cues for the player to stop the NPC.
- If the player didn't stop the NPC on time, it'll lead to the scene where the NPC encounter a road accident, which then the screen will slowly fade in black and then appear a safety message to the player, after that it will return back to the scene which in this case will be Day 1.
- If the player were to stop the NPC on time, the player will talk with the NPC regards on road safety and understanding why the NPC choose to violate the road safety rule (jaywalking in this case), after that the player can either choose to politely inform him (which resulted in increase points of both community trust & hazard avoided) or scold and fine him (which resulted in increase points of hazard avoided but decrease points for community trust)
- After ensuring all the incidents are being cleared, it will navigate the player to the next day.

**2. Day 2**
- For the 2nd day, the difficulty of the level will increase slightly by have 2 incidents in the day
- For the 1st incident, it would be an elderly attempting to cross the road while the traffic light is spoiled
- Similiar to Day 1, when the NPC walks to a "trigger" area, the NPC will have a red outline while being slowed down for a while with addition of alarm sound, which provides visual cues for the player to stop the NPC.
- If the player didn't stop the NPC on time, it'll lead to the scene where the NPC encounter a road accident, which then the screen will slowly fade in black and then appear a safety message to the player, after that it will return back to the scene which in this case will be Day 2.
- If the player were to stop the NPC on time, the player will talk with the NPC regards on road safety and understanding why the NPC choose to violate the road safety rule (dangerous crossing in this case), after that the player can either choose to politely inform him (which resulted in increase points of both community trust & hazard avoided) or scold and fine him (which resulted in increase points of hazard avoided but decrease points for community trust)
- After the 1st incident is being cleared, the 2nd incident will be released, which it will be a group of children attempting to run across the road while the Green Man is flashing.
- Similiar to the 1st incident, when the "children leader" walks to a "trigger" area, it will have a red outline while being slowed down for a while with addition of alarm sound, which provides visual cues for the player to stop them.
- If the player didn't stop the NPC on time, it'll lead to the scene where the NPC encounter a road accident, which then the screen will slowly fade in black and then appear a safety message to the player, after that it will return back to the scene which in this case will be Day 2.
- If the player were to stop the "children leader" on time, the group of children will stop and look to the player, the player then will talk with them regards on road safety and understanding why the NPC choose to violate the road safety rule (dangerous crossing in this case), after that the player can either choose to politely inform him (which resulted in increase points of both community trust & hazard avoided) or scold them (which resulted in increase points of hazard avoided but decrease points for community trust)
- After ensuring all the incidents are being cleared, it will navigate the player to the next day.

**3. Day 3**
- For the 3rd day, the difficulty of the level will escalted more by have 3 incidents in the day
- For the 1st incident, it will be the player encountering a car accident at the T junction.
- The player will then have to investigate the scene, which in this case would be investigating the phone to understand what was the reason that causes the car accident, and afterwards the player will call the doctor to help clear the scene.
- When the doctor arrived on site, it'll have a report regards on the incident with the doctor, the player then can either choose to acknowledge the accident  (which resulted in increase points of community trust only) or scold the driver (which resulted in decrease points for community trust)
- After that the scene will be cleared out, which later on the 2nd incident will happen, which is encountering a motorist that didn't wear his helmet while riding.
- The player choice here would affect the later on story of the motorist.
- If the player were to stop the NPC on time and politely inform him, the motorist will wear a helmet.
- If the player either didn't stop the NPC on time or were to stop the NPC on time but scolded and fine hime, the motorist will not wear a helmet.
- Afterwards both will lead to a speeding car accident.
- After that the ambulance and doctor will arrive on site to report the accident situation to the player.
- If the motorist did wear a helmet before the accident happen, the doctor will report to the player that the motorist live with minor injuries, which resulted in increase points of both community trust & hazard avoided
- However if the motorist didn't wear a helmet before the accident happen,  the doctor will report to the player that the motorist live but with severe injuries, which resulted in decrease points for both community trust and hazard avoided
- After acknowledging the 2nd incident, the scene will be cleared out once again, then the 3rd incident kicked in, which is a distracted pedestrian attempting to cross the road while the Green Man is flashing.
- Similiar to Day 1 and Day 2, when the NPC walks to a "trigger" area, the NPC will have a red outline while being slowed down for a while with addition of alarm sound, which provides visual cues for the player to stop the NPC.
- If the player didn't stop the NPC on time, it'll lead to the scene where the NPC encounter a road accident, which then the screen will slowly fade in black and then appear a safety message to the player, after that it will return back to the scene which in this case will be Day 3.
- If the player were to stop the NPC on time, the player will talk with the NPC regards on road safety and understanding why the NPC choose to violate the road safety rule (dangerous crossing in this case), after that the player can either choose to politely inform him (which resulted in increase points of both community trust & hazard avoided) or scold and fine him (which resulted in increase points of hazard avoided but decrease points for community trust)
- After ensuring all the incidents are being cleared, it will navigate the player to the ending screen, which will display their ending score.

**4. Key Controls**
- Shift key: Sprint (Recommended to use it while moving around the map)
- WASD key: Move around
- E key: Interaction with items around the map and with NPCs
- Mouse: Look around the surrounding, clicking of options and the Next button for conversations

**5. Game View**
- Full HD (1920x1080) is recommended when playing

# Answer Keys
### Day 1
1. Day 1 start up simple by approaching the NPC that contains the red outline with addition of alarm sound
2. Press E key to interact with him and continue the conversation
3. After the conversation, it will pop out 2 options, can select either of them which can resulted in increase or decreasing of hazard avoided and community trust
4. Lastly when the conversation ended, it will lead to the next day scene.

### Day 2
1. Day 2 start up the 1st incident by approaching the elderly NPC that contains the red outline with addition of alarm sound
2. Press E key to interact with him and continue the conversation
3. After the conversation, it will pop out 2 options, can select either of them which can resulted in increase or decreasing of hazard avoided and community trust
4. After the incident is done, the 2nd incident will start by having the group of children running.
5. Approaching the child NPC that contains the red outline with addition of alarm sound
6. Press E key to interact with the group and continue the conversation
7. After the conversation, it will pop out 2 options, can select either of them which can resulted in increase or decreasing of hazard avoided and community trust
8. Lastly when the conversation ended, it will lead to the next day scene.

### Day 3
1. Day 3 start up the 1st incident by approaching to the car crash scene
2. Press E key to interact with the phone that's on the floor
3. After interacting, the scene will be cleared and the doctor will appear behind the player
4. Press E key to interact with him and continue the conversation
5. After the conversation, it will pop out 2 options, can select either of them which can resulted in increase or decreasing of hazard avoided and community trust
6. After the incident is done, the scene will be clear and the 2nd incident will start by having the motorist riding without a helmet.
7. Approaching the motorist that contains the red outline with addition of alarm sound
8. Press E key to interact with the group and continue the conversation
9. After the conversation, it will pop out 2 options, can select either of them which can resulted in increase or decreasing of hazard avoided and community trust after the accident (will announce whether the motorist is in good or bad condition)
10. After the incident is done, the scene will be clear and the 3rd incident will start by having a distracted pedestrian crossing the road.
11. Approaching the NPC that contains the red outline with addition of alarm sound
12. Press E key to interact with the NPC and continue the conversation
13. After the conversation, it will pop out 2 options, can select either of them which can resulted in increase or decreasing of hazard avoided and community trust
14. Lastly when the conversation ended, it will lead to the ending game scene.

# Limitations & Bugs
Throughout this project our team meet a few limitations and some bugs while developing the game.
### Limitations
- For the accident scene (if didn't stop the NPC on time), initially it should have a similar panel like the homepage where it contains buttons like Restart or Quit, but while developing the buttons somehow was unable to click. With this happening, we resulted in a black out screen and having a safety message then lead the player back to the scene, which can be bad as the game would continue until the player reaches the end of the game.
- For the 3rd day, we were planning to have 3 items to investigate to understand why the car accident happened, but due to time constraint we only make the phone interactive, but we modified the conversation content where it will also mention the other 2 reasons.
- For the player perspective, the camera will move together with the screen when the player need to click onto the Next Button in order to read the next line, we tried to set fixed camera but it will make the user experience weird as the camera would be stuck in place.
- Due to difference of screen sizes, the text would either looks too small or look misalign in the player screen.

### Bugs
- For our game in order to restrict the player walking area, we implemented airwall at 4 areas of the map, but sometime the player can walk pass the airwall barrier as it's not long enough.

# FSM Diagrams
- The FSM diagrams pictures (.jpeg) are located in the FSM Diagram Folder (https://github.com/ThongWan1706/IP_Project/tree/main/Assets/FSM%20diagrams)

# Implementations of AIs
For our team, each of us handled one section of AI througout the gameplay:
1. Thong Wan
- Handled the vehicles where it follows the NavMesh path until it reaches a stop point at the traffic light, the car waits if the light is red while it moves if it turns green
- When it goes, the vehicle can choose either to go straight or turn left (random).
- After they reaches the destintaion, it will stay there for a few seconds then respawn back to where it originally come from
- The vehicles also has front sensor detectors where it'll stop when they detect a vehicle being too close.
- For buses, they worked slightly different as they can only go straight but have an extra stop at the bus stop. It will stay there for a few seconds and then move again.

2. Xavier
- NPC follows the NavMesh path until reaching a traffic light. If the light is red, the NPC waits; if it is green, or if the NPC turns left or right, it continues walking, crosses the road at the zebra crossing, reaches the nearest endpoint, and then resets to the starting position.
- NPC walks to the traffic light and decides whether to jaywalk or listen the police officer (player). If the NPC chooses to jaywalk, it continues walking; if it listens to the police officer, it stops moving.

4. Keno
- Programmed an interactable AI NPC Doctor that spawns on clue discovery, follows the player from a distance and despawns after dialogue ends.

# References and credits to external assets
### 3D models & Prefabs
- Anime Nature Skybox: https://assetstore.unity.com/packages/3d/environments/anime-nature-276931
- PolyPeople Series - City People (NPCs): https://assetstore.unity.com/packages/3d/characters/polypeople-series-city-people-325134
- Drivable-Low Poly Cars Pack: https://assetstore.unity.com/packages/3d/vehicles/drivable-low-poly-cars-pack-327315
- Tree Creator Toolkit 2: https://assetstore.unity.com/packages/tools/terrain/tree-creator-toolkit-2-279185
- Low Poly Emergency Ambulance Vehicles Pack : https://assetstore.unity.com/packages/3d/vehicles/low-poly-emergency-ambulance-vehicles-pack-328921
- Dublin Bus Model Wright Gemini 2 TFI Livery: https://sketchfab.com/3d-models/dublin-bus-model-wright-gemini-2-tfi-livery-fef3a71354d045d3b6df3ac3a79cc7bd
- Low Poly Vespa: https://sketchfab.com/3d-models/low-poly-vespa-565dcd577e0e47ed974d982a31771557
- Helmet: https://sketchfab.com/3d-models/helmet-effeb491118c4e92862bc7478b198b03
- Doctor - Sketchfab Weekly - 13 Mar'23: https://sketchfab.com/3d-models/doctor-sketchfab-weekly-13-mar23-9c89a438a5e940e59a0f9a07c22d6ade
- Headphones: https://sketchfab.com/3d-models/headphone-5348e229f87f4dc581cfc4dd84aff2e3

### Audios
- Ambulance sound: https://pixabay.com/sound-effects/city-distant-ambulance-siren-6108/
- Police whistle: https://pixabay.com/sound-effects/film-special-effects-police-whistle-fx-1-566726/
- Car crash (to people): https://pixabay.com/sound-effects/film-special-effects-sound-effect-car-crash-394903/
- Car crash (to motorist): https://pixabay.com/sound-effects/film-special-effects-car-crash-377291/
- Teenagers laughing: https://pixabay.com/sound-effects/people-group-of-friends-joking-and-laughing-300997/
- Raining sound: https://pixabay.com/sound-effects/nature-copyright-free-rain-sounds-331497/
- Point increase sound: https://pixabay.com/sound-effects/film-special-effects-awesome-notification-351720/
- Point decrease sound: https://pixabay.com/sound-effects/film-special-effects-ui-chime-alert-554690/
- Background music (day 1): https://pixabay.com/sound-effects/musical-kids-music-54-second-499481/
- Background music(day 2): https://pixabay.com/sound-effects/musical-podcast-intro-28sec-576200/
- Background music(day 3): https://pixabay.com/sound-effects/musical-relax-27sec-575310/
- Victory Sound: https://pixabay.com/sound-effects/musical-victory-chime-366449/
- Nice try sound: https://pixabay.com/sound-effects/film-special-effects-achievement-unlock-243762/
