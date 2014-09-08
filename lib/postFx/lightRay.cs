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


$LightRayFx::brightScalar = 0.75;
$LightRayFx::numSamples = 40;
$LightRayFx::density = 0.94;
$LightRayFx::weight = 5.65;
$LightRayFx::decay = 1.0;
$LightRayFx::exposure = 0.0005;
$LightRayFx::resolutionScale = 1.0;


singleton ShaderData( LightRayOccludeShader )
{
   DXVertexShaderFile 	= "shaders/common/postFx/postFxV.hlsl";
   DXPixelShaderFile 	= "shaders/common/postFx/lightRay/lightRayOccludeP.hlsl";

   pixVersion = 3.0;   
};

singleton ShaderData( LightRayShader )
{
   DXVertexShaderFile 	= "shaders/common/postFx/postFxV.hlsl";
   DXPixelShaderFile 	= "shaders/common/postFx/lightRay/lightRayP.hlsl";

   pixVersion = 3.0;   
};

singleton GFXStateBlockData( LightRayStateBlock : PFX_DefaultStateBlock )
{
   samplersDefined = true;
   samplerStates[0] = SamplerClampLinear;
   samplerStates[1] = SamplerClampLinear;     
};

singleton PostEffect( LightRayFX )
{
   isEnabled = false;
   allowReflectPass = false;
        
   renderTime = "PFXAfterDiffuse";
   renderPriority = 0.1;
      
   shader = LightRayOccludeShader;
   stateBlock = LightRayStateBlock;
   texture[0] = "$backBuffer";
   texture[1] = "#prepass";
   target = "$outTex";
   targetFormat = "GFXFormatR16G16B16A16F";
      
   new PostEffect()
   {
      shader = LightRayShader;
      stateBlock = LightRayStateBlock;
      internalName = "final";
      texture[0] = "$inTex";
      texture[1] = "$backBuffer";
      target = "$backBuffer";
   };
};

function LightRayFx::preProcess( %this )
{   
   %this.targetScale = $LightRayFx::resolutionScale SPC $LightRayFx::resolutionScale;
}

function LightRayFx::setShaderConsts( %this )
{
   %this.setShaderConst( "$brightScalar", $LightRayFx::brightScalar );
   
   %pfx = %this-->final;
   %pfx.setShaderConst( "$numSamples", $LightRayFx::numSamples );
   %pfx.setShaderConst( "$density", $LightRayFx::density );
   %pfx.setShaderConst( "$weight", $LightRayFx::weight );
   %pfx.setShaderConst( "$decay", $LightRayFx::decay );
   %pfx.setShaderConst( "$exposure", $LightRayFx::exposure );
}
