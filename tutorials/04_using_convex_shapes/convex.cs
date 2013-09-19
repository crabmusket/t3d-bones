//-----------------------------------------------------------------------------
// Utility functions that create convex shapes.

// Create a new block with a position and size. The block's bottom face is centered
// on the position given.
function block(%pos, %size) {
   // Determine the maximum and minimum dimensions from the input size.
   %maxX =  getWord(%size, 0) / 2; %minX = -%maxX;
   %maxY =  getWord(%size, 1) / 2; %minY = -%maxY;
   %maxZ =  getWord(%size, 2);     %minZ = 0;

   // Create the ConvexShape object itself.
   return new ConvexShape() {
      material = BlankWhite;
      position = %pos;
      scale = "1 1 1";
      rotation = "1 0 0 0";

      surface = "0 0 0 1"                SPC "0 0 " @ %maxZ;
      surface = "0 1 0 0"                SPC "0 0 " @ %minZ;
      surface = "0.707107 0 0 0.707107"  SPC "0 " @  %maxY @ " 0";
      surface = "0 0.707107 -0.707107 0" SPC "0 " @ %minY @ " 0";
      surface = "0.5 0.5 -0.5 0.5"       SPC %minX @ " 0 0";
      surface = "0.5 -0.5 0.5 0.5"       SPC %maxX @ " 0 0";
   };
}

function ramp(%pos, %size, %rot) {
   %maxX =  getWord(%size, 0) / 2; %minX = -%maxX;
   %maxY =  getWord(%size, 1) / 2; %minY = -%maxY;
   %maxZ =  getWord(%size, 2) / 2; %minZ = 0;

   // Determine the angle that the ramp should be at.
   %angle = mRadToDeg(mAtan(%maxZ, %maxY));

   return new ConvexShape() {
      material = BlankWhite;
      position = %pos;
      rotation = "1 0 0 0";
      scale = "1 1 1";

      surface = mEulerToQuat("180 0" SPC %angle) SPC "0 0 " @ %maxZ + 0.01;
      surface = "0 1 0 0"                        SPC "0 0 " @ %minZ;
      surface = "0.707107 0 0 0.707107"          SPC "0 " @  %maxY @ " 0";
      surface = "0 0.707107 -0.707107 0"         SPC "0 " @ %minY @ " 0";
      surface = "0.5 0.5 -0.5 0.5"               SPC %minX @ " 0 0";
      surface = "0.5 -0.5 0.5 0.5"               SPC %maxX @ " 0 0";
   };
}

function mEulerToQuat(%euler) {
   %r = mDegToRad(getWord(%euler, 1));
   %p = mDegToRad(getWord(%euler, 0));
   %y = mDegToRad(getWord(%euler, 2));
   %q0 = mCos(%r/2) * mCos(%p/2) * mCos(%y/2) + mSin(%r/2) * mSin(%p/2) * mSin(%y/2);
   %q1 = mSin(%r/2) * mCos(%p/2) * mCos(%y/2) - mCos(%r/2) * mSin(%p/2) * mSin(%y/2);
   %q2 = mCos(%r/2) * mSin(%p/2) * mCos(%y/2) + mSin(%r/2) * mCos(%p/2) * mSin(%y/2);
   %q3 = mCos(%r/2) * mCos(%p/2) * mSin(%y/2) - mSin(%r/2) * mSin(%p/2) * mCos(%y/2);
   return %q0 SPC %q1 SPC %q2 SPC %q3;
}

