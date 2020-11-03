////////////////////////////////////////////////////////////
// UltEvents // Quick Start //
////////////////////////////////////////////////////////////

[SerializeField]
private UltEvents.UltEvent _MyEvent;

1. To create an UltEvent: declare a serialized field in your script as shown above.
2. Once you have declared your event it will show up in the Inspector for your script like a regular field so you can configure it to run code.
3. To trigger the execution of the event, simply call _MyEvent.Invoke().

- There are also various Premade Event Scripts which expose MonoBehaviour events like Awake, Update, etc.
- A scene with some example events can be found in Assets/Plugins/UltEvents/Example.
- You can safely move this plugin anywhere in your project.

////////////////////////////////////////////////////////////

The full documentation is available at https://kybernetikgames.github.io/ultevents

////////////////////////////////////////////////////////////
