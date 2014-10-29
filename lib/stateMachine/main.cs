//-----------------------------------------------------------------------------
// Copyright (c) 2014 Daniel Buckmaster
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

function StateMachine::onEvent(%this, %event) {
   // See if there's a transition callback ssociated with this event.
   %script = "on" @ %event;
   if(%this.isMethod(%script)) {
      %this.call(%script);
   }

   // Figure out the new state to transition to.
   %newState = %this.transition[%this.state, %event];

   // If it doesn't exist, see if there's a wildcard transition for this event.
   if(%newState $= "") {
      %newState = %this.transition[_, %event];
   }

   // Apply the state change.
   if(%newState !$= "") {
      // Callback for leaving the current state.
      %script = "leave" @ %this.state;
      if(%this.isMethod(%script)) {
         %this.call(%script);
      }

      // Change the state!
      %this.state = %newState;

      // Callback upon entering the new state.
      %script = "enter" @ %this.state;
      if(%this.isMethod(%script)) {
         %this.call(%script);
      }
   }

   return %this;
}
