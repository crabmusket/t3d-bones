# TorqueScript state machine

This file provides a single function, `StateMachine::onEvent`.
You can use it to construct your own scripted state machines easily.
Here's a basic example:

    new ScriptObject(Furby) {
       class = StateMachine;
       state = hungry;
       transition[hungry, feed] = full;
       transition[full, timePasses] = hungry;
    };

    function Furby::onFeed(%this) {
        echo("Nom nom");
    }

    function Furby::leaveFull(%this) {
       echo("Burp!");
       %this.schedule(3000, onEvent, timePasses);
    }

    function Furby::enterHungry(%this) {
       echo("Feed me!");
    }

    Furby.onEvent(feed);

## What it is

A _state machine_ is comprised of _states_ an object can be in, and _transitions_ between those states that atr triggered by _events_.
This function provides the transition logic, letting you define states and events as simple strings.

## How to use it

If your object is in the `StateMachine` class (either using `class` as shown above, `superclass`, or by inheriting the class), just call`onEvent` with the event name to trigger any state transitions that might occur.
All state transitions happen as a response to events, and only one transition can occur per event.

Callbacks occur automatically when state transitions happen.
These callbacks are `enterX`, `leaveX`, and `onY`, where `X` is a state name and `Y` is an event name.
The callbacks are not called if they aren't defined, and are always called if they do.

You can use the wildcard, `_`, in place of a state name to apply a transition to all states, unless something more specific is defined.
For example:

    new ScriptObject(Tamigochi) {
       class = StateMachine;
       state = beeping;
       transition[beeping, throw] = quiet;
       transition[_, throw] = beeping;
    };

In this case, if your `Tamigochi` is `beeping`, and you `throw` it, it will stop `beeping` - but if it's in any other state and you `throw` it, it will _start_ `beeping`.

If you have namespace hierarchy problems, here's way to make your object type behave like a state machine without using the `class` property of script objects:

    new ScriptObject(MyObject) { ... };

    function MyObject::onEvent(%this, %event) {
       StateMachine::onEvent(%this, %event);
    }
