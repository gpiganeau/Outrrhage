# README

This an in-engine dev documentation, mostly for now. 
But it can change, maybe if the project is public / in portfolio need !


## Menu
- [Sly Notes and Tasks](./docs/sly.md)


## Create a new Skill
1. Need/Locate/Create a Skills Controller with Setup Data
2. Call Skill with [x] appropriate index  
3. Create an existing class heriting SkillStrategy; and overoid Call (see SlashStrategy)
4. Skillshoot or Projectile Class or Custom Logic
5. Create Projectile Prefab
6. Create Skill Data
7. Add to Character / Entity who's in need  

## Known Bugs
- Riel innacurate rotation when colliding; need to set her looking forward toward inputs
- Movement Controller Look At is pretty bad (broke the drone)