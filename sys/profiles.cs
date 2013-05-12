//-----------------------------------------------------------------------------
// Copyright (c) 2012 GarageGames, LLC
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

// If we got back no prefs path modification
if( $Gui::fontCacheDirectory $= "")
{
   $Gui::fontCacheDirectory = expandFilename( "./fonts" );
}

// ----------------------------------------------------------------------------
// GuiDefaultProfile is a special profile that all other profiles inherit
// defaults from. It must exist.
// ----------------------------------------------------------------------------

if( !isObject( GuiDefaultProfile ) )
new GuiControlProfile (GuiDefaultProfile)
{
   tab = false;
   canKeyFocus = false;
   hasBitmapArray = false;
   mouseOverSelected = false;

   // fill color
   opaque = false;
   fillColor = "242 241 240";
   fillColorHL ="228 228 235";
   fillColorSEL = "98 100 137";
   fillColorNA = "255 255 255 ";

   // border color
   border = 0;
   borderColor   = "100 100 100"; 
   borderColorHL = "50 50 50 50";
   borderColorNA = "75 75 75"; 

   // font
   fontType = "Arial";
   fontSize = 14;
   fontCharset = ANSI;

   fontColor = "0 0 0";
   fontColorHL = "0 0 0";
   fontColorNA = "0 0 0";
   fontColorSEL= "255 255 255";

   // bitmap information
   bitmap = "";
   bitmapBase = "";
   textOffset = "0 0";

   // used by guiTextControl
   modal = true;
   justify = "left";
   autoSizeWidth = false;
   autoSizeHeight = false;
   returnTab = false;
   numbersOnly = false;
   cursorColor = "0 0 0 255";

   // sounds
   //soundButtonDown = "";
   //soundButtonOver = "";
};

if( !isObject( GuiToolTipProfile ) )
new GuiControlProfile (GuiToolTipProfile)
{
   // fill color
   fillColor = "239 237 222";

   // border color
   borderColor   = "138 134 122";

   // font
   fontType = "Arial";
   fontSize = 14;
   fontColor = "0 0 0";

   category = "Core";
};

if( !isObject( GuiWindowProfile ) )
new GuiControlProfile (GuiWindowProfile)
{
   opaque = false;
   border = 2;
   fillColor = "242 241 240";
   fillColorHL = "221 221 221";
   fillColorNA = "200 200 200";
   fontColor = "50 50 50";
   fontColorHL = "0 0 0";
   bevelColorHL = "255 255 255";
   bevelColorLL = "0 0 0";
   text = "untitled";
   bitmap = "./images/window";
   textOffset = "8 4";
   hasBitmapArray = true;
   justify = "left";
   category = "Core";
};


if( !isObject( GuiTextEditProfile ) )
new GuiControlProfile( GuiTextEditProfile )
{
   opaque = true;
   bitmap = "./images/textEdit";
   hasBitmapArray = true; 
   border = -2; // fix to display textEdit img
   //borderWidth = "1";  // fix to display textEdit img
   //borderColor = "100 100 100";
   fillColor = "242 241 240 0";
   fillColorHL = "255 255 255";
   fontColor = "0 0 0";
   fontColorHL = "255 255 255";
   fontColorSEL = "98 100 137";
   fontColorNA = "200 200 200";
   textOffset = "4 2";
   autoSizeWidth = false;
   autoSizeHeight = true;
   justify = "left";
   tab = true;
   canKeyFocus = true;   
   category = "Core";
};

if( !isObject( GuiScrollProfile ) )
new GuiControlProfile( GuiScrollProfile )
{
   opaque = true;
   fillcolor = "255 255 255";
   fontColor = "0 0 0";
   fontColorHL = "150 150 150";
   //borderColor = GuiDefaultProfile.borderColor;
   border = true;
   bitmap = "./images/scrollBar";
   hasBitmapArray = true;
   category = "Core";
};

if( !isObject( GuiOverlayProfile ) )
new GuiControlProfile( GuiOverlayProfile )
{
   opaque = true;
   fillcolor = "255 255 255";
   fontColor = "0 0 0";
   fontColorHL = "255 255 255";
	fillColor = "0 0 0 100";
   category = "Core";
};


if( !isObject( GuiConsoleProfile ) )
new GuiControlProfile( GuiConsoleProfile )
{
   fontType = ($platform $= "macos") ? "Monaco" : "Lucida Console";
   fontSize = ($platform $= "macos") ? 13 : 12;
   fontColor = "255 255 255";
   fontColorHL = "0 255 255";
   fontColorNA = "255 0 0";
   fontColors[6] = "100 100 100";
   fontColors[7] = "100 100 0";
   fontColors[8] = "0 0 100";
   fontColors[9] = "0 100 0";
   category = "Core";
};

if( !isObject( GuiConsoleTextProfile ) )
new GuiControlProfile( GuiConsoleTextProfile )
{   
   fontColor = "0 0 0";
   autoSizeWidth = true;
   autoSizeHeight = true;   
   textOffset = "2 2";
   opaque = true;   
   fillColor = "255 255 255";
   border = true;
   borderThickness = 1;
   borderColor = "0 0 0";
   category = "Core";
};

if( !isObject( ConsoleScrollProfile ) )
new GuiControlProfile( ConsoleScrollProfile : GuiScrollProfile )
{
	opaque = true;
	fillColor = "0 0 0 175";
	border = 1;
	//borderThickness = 0;
	borderColor = "0 0 0";
   category = "Core";
};

if( !isObject( ConsoleTextEditProfile ) )
new GuiControlProfile( ConsoleTextEditProfile : GuiTextEditProfile )
{
   fillColor = "242 241 240 255";
   fillColorHL = "255 255 255";   
   category = "Core";
};
