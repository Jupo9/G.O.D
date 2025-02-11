
/*Know issues / features: 
1. Quick Solution for Ui in Angel (SaveTime) need to be changed
        a.	UI get closed when they build and the angel is not clickable after this, for know after a short time the ui is clickable again. This need to be changed to a methode that reactivate ui after the building is ready
2.	Reset don’t work correct, it always delete all keys or ignore that keys exists
3.	Angels not hitable by mouse when they in pray or other collider (change)
        a.Angels and Devils are not clickable when they stay in a collider
4.	Known issue, lags (performance)
5.	Some transport mistakes
        a.	Jump to another action or do not what the player order them to do. Maybe they calculate wrong 
6.	Graves don’t delete grid places
        a.	A grave should always delete not only the mesh, also the grid possibility to place a building there
        b.	And if a devil build or do an Action in a building the needs fall must be paused
7.	Deactivate choosen in every case
        a.	Choosen one is still on if the player use right click or escape
8.	Rework ui for choosen one 
        a.	It is not good, I could freeze it or the npc agent stop if they are choosen ones!
        b.	Add tolltips for player, so they know what they have to do
9.	More Feedback (question marks like sims)
        a.Let them think and show what the want to do!
10.	Deactivate every action when player choose an action
        a.	In the moment when the player choose an action, it can happens that normal action will override what the player choosed. Maybe delete also goals and clean actions (it should work like that, NPC should start to ignore the player but right now it is more random than on purpose)
11.	Tutorial
12.	Duration solution for actions is ineffective (bad) also the actions will interrupt other actions
13.	More structure in scripts!
        a.	ORDER!
14.	To many Ifs - spaghetti code fix!!!
15.	Activate/deactivate ressources by transport
16.	Check isavailable state in every case
17.	Visuals and design
18.	Double current action in if’s in Agents (Obsolete?)
19. Combine bully and punsh action
20.	Add charge action
21.	Add more actions
22.	Rework of UI
23.	Add pause
24.	Reset level after game over
25.	Options menu
26.	Add calculate for Building costs and deactivate buildings with missing ressources
27.	Add storage to devil
28.	Add Start Ressources that are not in any Storage or Working Station (exist in the world, so that a player can start with 0 buildings)
29. Add visuals for devil and angel needs
30.	Connect needs with global ui
31.	Builing rotate and pivot fix
32.	Maybe add different light and Fire ressouces
33.	ADD Bandits (Souls)
34. ADD World Generation
35.	Add ressouces fields for working stations
        a.	It should only possible to place working Stations on resource Fields
        b.	Resource fields spawn random over time in the world
36.	Add Obstacles
*/
