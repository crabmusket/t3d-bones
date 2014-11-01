function ArgParser() {
   return new ScriptObject() {
      class = ArgParser;
      _argCount = 0;
   };
}

function ArgParser::arg(%this, %a, %b) {
   if (%a $= "" && %b $= "") {
      error("Cannot add argument with no name!");
   }

   %la = strlen(%a);
   %lb = strlen(%b);
   if (%la > 1 && %lb > 1) {
      error("Cannot determine which of" SPC %a SPC and SPC %b "is the short argument");
   }

   %short = %la < %lb ? %la : %lb;
   %long  = %la < %lb ? %lb : %la;

   if(%long !$= "") {
      if(%this._getLongArg(%short) != -1) {
         error("Flag multiply defined: --" @ %long);
      }
   } else {
      error("Flag -" @ %short SPC "must have a long name!");
      return;
   }

   if(%short !$= "") {
      if(%this._getShortArg(%short) != -1) {
         error("Flag multiply defined" SPC "-" @ %short);
      }
   }

   %this._shorts[%this._argCount] = %short;
   %this._longs[%this._argCount] = %long;
   %this._argCount++;
   return %this;
}

function help(%this, %help) {
   %this._helps[%this.argCount] = %help;
}

function defaults(%this, %def) {
   %this._defaults[%this.argCount] = %def;
}

// Runs the parser on $Game::argc and $Game::argv.
function ArgParser::parseDefaultArgs(%this) {
}

function ArgParser::parse(%this, %argv) {
   // Object to hold all our parsed values.
   %out = new ScriptObject() {
      class = Args;
   };

   %argc = getWordCount(%argv);
   for(%a = 0; %a < %argc; %a++) {
      %arg = getRecord(%argv, %a);

      // Ignore things that are weird.
      if(%arg $= "--" || %arg $= "-") {
      }

      // Long format argument. Parse the body as a long name.
      else if(strpos(%arg, "--") == 0) {
         %long = getSubStr(%arg, 2);
         %index = %this._getLongArg(%long);
      }

      // Short format argument. Check whether the next character is one of our
      // short names.
      else if(strpos(%arg, "-") == 0) {
         %short = getSubStr(%arg, 1, 1);
         %index = %this._getShortArg(%short);
         if(%index > -1) {
         } else {
            %out.error = true;
            error("Misunderstood command-line argument" SPC %arg);
         }
      }

      // Ignore things that aren't args.
      else {
      }
   }

   return %out;
}

function ArgParser::_setOption(%this, %index, %out, %value) {
}

function ArgParser::_getShortArg(%this, %short) {
   for(%i = 0; %i < %this._argCount; %i++) {
      if(%this._shorts[%i] $= %short) {
         return %i;
      }
   }
   return -1;
}

function ArgParser::_getLongArg(%this, %long) {
   for(%i = 0; %i < %this._argCount; %i++) {
      if(%this._longs[%i] $= %long) {
         return %i;
      }
   }
   return -1;
}

function Args::assignToGlobals(%this, %prefix) {
   return %this;
}

function Args::print(%this) {
   return %this;
}

function testParser() {
   return ArgParser()
      .flag("q", "quiet", "Suppress output")
      .flag("n", "no-warn", "No warnings?")
      .option("d", "difficulty", "Select difficulty mode");
}

function test() {
   testParser()
      .parse("-nq" NL "--difficulty=easy" NL "ha" NL "--no-warn")
      .print();
}
