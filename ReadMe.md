# Fart Knight

[![Discord](https://img.shields.io/discord/879125729936298015.svg?logo=discord&logoColor=white&logoWidth=20&labelColor=7289DA&label=Discord&color=17cf48)](https://discord.gg/F6Y5TeFQ8j) [![License](https://img.shields.io/badge/license-MIT-green)](./LICENSE)


![Demo gif](assets/demo.gif)

Adds a fart "attack" ability to Hollow Knight.
More or less this was just a goofy project to explore the basics of writing a Hollow Knight mod.
I don't really intend on developing this further, but rather make an effor to document the anatomy of this mod for new modder like myself to hopefully stumble upon and use as a intermediate step between the elementry sample in the modding docs and more advanced mods.
I made an attempt to roughly document the code with reference links to different repos I got guidance from.
Many of the basic concepts in this mod can be extended to create potentially new weapons, companions, etc.

This mod has the following features:

- A mod options menu using the Modding API `ICustomMenuMod`
- Adds a new action with customizable key bind
- Adds and play a small animation on the knight
- Spawns new game object into the scene with an animation and hitbox
- Plays a SFX on attack

Everything in this mod was done via C# scripting in VSCode and looking at online documentation / APIs.
There are a number of older legacy tools that can make moding easier that I don't know how to use.
Additionally, one can also use Unity and bundle assets to then inject into the game.
I consider these a requirement for more advanced mods, but for a simple concept like this I wanted to try to approach this through pure scripting.

## Repository Layout

```
FartKnight
├── assets                  # Various asset files used
├── bin                     # Compiled project files
├── src                     # Source folder
│   ├── Resources           # Binary resources (wav / pngs)
│   ├── ModClass.cs         # Core mod class for hooking on Modding API
│   ├── ModMenu.cs          # Building function for Custom Mod Menu
│   ├── GlobalSettings.cs   # Data-structure for global state / settings
│   ├── FartHandler.cs      # Fart attack runner / animation+sfx handler
│   ├── FartBehavior.cs     # Fart Collision / hitbox behavior
│   ├── KnightHandler.cs    # Hollow Knight animation handler
│   └── FartKnight.csproj   # C# project file
└── FartKnight.sln          # Visual Studio solution file
```

## Dependencies

These should be installed via the mod installer for development.

- [Satchel](https://github.com/PrashantMohta/Satchel)
- [SFCore](https://github.com/SFGrenade/SFCore)
- [FrogCore](https://github.com/RedFrog6002/FrogCore/)

## Resources

- [Modding Docs](https://prashantmohta.github.io/ModdingDocs/)

## Support

For issues / bugs, I probably won't fix them but feel free to open an issue.
The modding discord has a lot of very helpful and active devs there which can also answer various question but don't bug them about this stupid thing.
