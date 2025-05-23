# Android Dynamic Shortcuts Plugin for Unity
A simple Unity android plugin that helps manage dynamic shortcuts.

<img src="./Misc/screenshot.jpg" alt="screenshot" width="400"/>

### Installation
- Simply add the 'Shortcut' to your project or alternatively, install the .unitypackage
- Go to 'Project Settings > Player' and under 'Publishing Settings', enable 'Custom Main Gradle Template'
- Go to 'Plugins > Android > mainTemplate.gradle' and inside dependencies, add: 
    >implementation 'androidx.appcompat:appcompat:1.3.0'
- Or, if using EDM4U, it should automatically handle this dependency

### Current Features
- Create and remove dynamic shortcuts
- Get the count and array of currently added shortcuts

### Known bugs/issues
- After launching the app through a shortcut, and then putting it in and out of background mode, it will fire the shortcut trggered event

### Future plans
- Better implementation of AAR plugin code to integrate a more robust shortcut listener