# ![](https://github.com/TheSpydog/Ogmo-Editor/blob/master/OgmoEditor/Content/icons/icon32.png) Ogmo Editor: Community Version

[Ogmo Editor](http://www.ogmoeditor.com/) is a fantastic level editor for 2D games, originally written by Matt Thorson. This fork of the project aims to improve the level editing experience by fixing longstanding bugs, adding awesome new features, and bringing the editor to new platforms like Mac and Linux!

## New Features

### Mac and Linux Support (via Wine)

You can build and run this version of Ogmo Editor on macOS and Linux.

__macOS Setup:__
1. Install [Homebrew](https://brew.sh)
2. Run `brew install wine; brew install winetricks`.
3. Run `winetricks dotnet40`. Go through the installation process.

__Linux Setup:__
1. Install wine and winetricks.
2. Run `winetricks dotnet40`. Go through the installation process.

To run Ogmo Editor:

`WINEDEBUG=-all FREETYPE_PROPERTIES="truetype:interpreter-version=35" wine OgmoEditor.exe`

Note that you can only load assets from within Wine's virtual filesystem (i.e. `drive_c` and its subdirectories).

### JSON Export

When creating projects, you can now choose between XML (.oel) and JSON (.json) file exports for your levels. Choose whichever best fits your needs!

NOTE: You cannot change the project export type after creation.

### Tilemap Zooming
You can now zoom in on your tile palette windows. This helps you get a better view of your assets and save precious screen real estate. Use the mouse wheel to zoom and right/middle mouse button to pan.

### Performance and Rendering Enhancements
Thanks to [talesofgames' ToGmo project](https://github.com/talesofgames/Ogmo-Editor), the editor is faster and smoother than ever, especially when dealing with large maps!

### Bug Fixes
Many longstanding Ogmo bugs are fixed in this fork. No more annoying exceptions or inconsistent hotkeys!

### Other Stuff!
See the [changelog.html](https://github.com/TheSpydog/Ogmo-Editor/blob/master/OgmoEditor/Content/changelog.html#L50) for a list of all the fixes and additions.

## Credits

Thanks to [Matt Thorson](http://www.mattmakesgames.com/) for making Ogmo Editor originally! Thanks to talesofgames for their [ToGmo fork](https://github.com/talesofgames/Ogmo-Editor), which added some great new features and improvements!
