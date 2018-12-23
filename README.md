# ![](OgmoEditor/assets/icon32.ico "Logo Title Text 1") ToGmo Editor 


This version of Ogmo Editor has __mutated__ to produce hellacious doom-murderscapes with furious performance.


## Changes
Rampant genetic shifts have produced viral abnormalities:

### Apply Tileset to Layer button

This functionality is now relocated a button instead of automatic when switching tilesets in viewer. Chance of destroying tile layers decreased.

### performance improvements

- Much improved rendering times for large maps
- Improved rendering clarity
- Improvements to entity scaling

### mac and linux support (via wine)

You can build and run this version of Ogmo Editor on macOS and Linux.

__macOS Setup:__
1. Install [Homebrew](https://brew.sh)
2. Run `brew install wine; brew install winetricks`.
3. Run `winetricks dotnet40`. Go through the installation process.

__Linux Setup:__
1. Install wine and winetricks.
2. Run `winetricks dotnet40`. Go through the installation process.

Now you can run OgmoEditor.exe with `WINEDEBUG=-all FREETYPE_PROPERTIES="truetype:interpreter-version=35" wine OgmoEditor.exe`.
Note that you can only load assets from within Wine's virtual filesystem (i.e. `drive_c` and its subdirectories).

### more stuff that I can't remember

Features lost in time, like tears in the rain.

# Credit where it's due:

For more info, submit your biological self to http://www.ogmoeditor.com/
