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


///
$SSAOFx::overallStrength = 2.0;

// TODO: Add small/large param docs.

// The small radius SSAO settings.
$SSAOFx::sRadius = 0.1;
$SSAOFx::sStrength = 6.0;
$SSAOFx::sDepthMin = 0.1;
$SSAOFx::sDepthMax = 1.0;
$SSAOFx::sDepthPow = 1.0;
$SSAOFx::sNormalTol = 0.0;
$SSAOFx::sNormalPow = 1.0;

// The large radius SSAO settings.
$SSAOFx::lRadius = 1.0;
$SSAOFx::lStrength = 10.0;
$SSAOFx::lDepthMin = 0.2;
$SSAOFx::lDepthMax = 2.0;
$SSAOFx::lDepthPow = 0.2;
$SSAOFx::lNormalTol = -0.5;
$SSAOFx::lNormalPow = 2.0;

/// Valid values: 0, 1, 2
$SSAOFx::quality = 0;

///
$SSAOFx::blurDepthTol = 0.001;

/// 
$SSAOFx::blurNormalTol = 0.95;

///
$SSAOFx::targetScale = "0.5 0.5";


function SSAOFx::onAdd( %this )
{  
   %this.wasVis = "Uninitialized";
   %this.quality = "Uninitialized";
}

function SSAOFx::preProcess( %this )
{   
   if ( $SSAOFx::quality !$= %this.quality )
   {
      %this.quality = mClamp( mRound( $SSAOFx::quality ), 0, 2 );
      
      %this.setShaderMacro( "QUALITY", %this.quality );      
   }      
   
   %this.targetScale = $SSAOFx::targetScale;
}

function SSAOFx::setShaderConsts( %this )
{      
   %this.setShaderConst( "$overallStrength", $SSAOFx::overallStrength );

   // Abbreviate is s-small l-large.   
   
   %this.setShaderConst( "$sRadius",      $SSAOFx::sRadius );
   %this.setShaderConst( "$sStrength",    $SSAOFx::sStrength );
   %this.setShaderConst( "$sDepthMin",    $SSAOFx::sDepthMin );
   %this.setShaderConst( "$sDepthMax",    $SSAOFx::sDepthMax );
   %this.setShaderConst( "$sDepthPow",    $SSAOFx::sDepthPow );
   %this.setShaderConst( "$sNormalTol",   $SSAOFx::sNormalTol );
   %this.setShaderConst( "$sNormalPow",   $SSAOFx::sNormalPow );
   
   %this.setShaderConst( "$lRadius",      $SSAOFx::lRadius );
   %this.setShaderConst( "$lStrength",    $SSAOFx::lStrength );
   %this.setShaderConst( "$lDepthMin",    $SSAOFx::lDepthMin );
   %this.setShaderConst( "$lDepthMax",    $SSAOFx::lDepthMax );
   %this.setShaderConst( "$lDepthPow",    $SSAOFx::lDepthPow );
   %this.setShaderConst( "$lNormalTol",   $SSAOFx::lNormalTol );
   %this.setShaderConst( "$lNormalPow",   $SSAOFx::lNormalPow );
   
   %blur = %this->blurY;
   %blur.setShaderConst( "$blurDepthTol", $SSAOFx::blurDepthTol );
   %blur.setShaderConst( "$blurNormalTol", $SSAOFx::blurNormalTol );   
   
   %blur = %this->blurX;
   %blur.setShaderConst( "$blurDepthTol", $SSAOFx::blurDepthTol );
   %blur.setShaderConst( "$blurNormalTol", $SSAOFx::blurNormalTol );   
   
   %blur = %this->blurY2;
   %blur.setShaderConst( "$blurDepthTol", $SSAOFx::blurDepthTol );
   %blur.setShaderConst( "$blurNormalTol", $SSAOFx::blurNormalTol );
      
   %blur = %this->blurX2;
   %blur.setShaderConst( "$blurDepthTol", $SSAOFx::blurDepthTol );
   %blur.setShaderConst( "$blurNormalTol", $SSAOFx::blurNormalTol );         
}

function SSAOFx::onEnabled( %this )
{
   // This tells the AL shaders to reload and sample
   // from our #ssaoMask texture target. 
   $AL::UseSSAOMask = true;
   
   return true;
}

function SSAOFx::onDisabled( %this )
{
  $AL::UseSSAOMask = false;
}


//-----------------------------------------------------------------------------
// GFXStateBlockData / ShaderData
//-----------------------------------------------------------------------------

singleton GFXStateBlockData( SSAOStateBlock : PFX_DefaultStateBlock )
{   
   samplersDefined = true;
   samplerStates[0] = SamplerClampPoint;
   samplerStates[1] = SamplerWrapLinear;
   samplerStates[2] = SamplerClampPoint;
};

singleton GFXStateBlockData( SSAOBlurStateBlock : PFX_DefaultStateBlock )
{   
   samplersDefined = true;
   samplerStates[0] = SamplerClampLinear;
   samplerStates[1] = SamplerClampPoint;
};

singleton ShaderData( SSAOShader )
{   
   DXVertexShaderFile 	= "shaders/common/postFx/postFxV.hlsl";
   DXPixelShaderFile 	= "shaders/common/postFx/ssao/SSAO_P.hlsl";            
   pixVersion = 3.0;
};

singleton ShaderData( SSAOBlurYShader )
{
   DXVertexShaderFile 	= "shaders/common/postFx/ssao/SSAO_Blur_V.hlsl";
   DXPixelShaderFile 	= "shaders/common/postFx/ssao/SSAO_Blur_P.hlsl";   
   pixVersion = 3.0;      
   
   defines = "BLUR_DIR=float2(0.0,1.0)";         
};

singleton ShaderData( SSAOBlurXShader : SSAOBlurYShader )
{
   defines = "BLUR_DIR=float2(1.0,0.0)";
};

//-----------------------------------------------------------------------------
// PostEffects
//-----------------------------------------------------------------------------

singleton PostEffect( SSAOFx )
{     
   allowReflectPass = false;
     
   renderTime = "PFXBeforeBin";
   renderBin = "AL_LightBinMgr";   
   renderPriority = 10;
   
   shader = SSAOShader;
   stateBlock = SSAOStateBlock;
         
   texture[0] = "#prepass";         
   texture[1] = "noise.png";
   texture[2] = "#ssao_pow_table";
   
   target = "$outTex";
   targetScale = "0.5 0.5";
   
   singleton PostEffect()
   {
      internalName = "blurY";
      
      shader = SSAOBlurYShader;
      stateBlock = SSAOBlurStateBlock;
      
      texture[0] = "$inTex";
      texture[1] = "#prepass";
      
      target = "$outTex"; 
   };
      
   singleton PostEffect()
   {
      internalName = "blurX";
      
      shader = SSAOBlurXShader;
      stateBlock = SSAOBlurStateBlock;
      
      texture[0] = "$inTex";
      texture[1] = "#prepass";
      
      target = "$outTex"; 
   };   
   
   singleton PostEffect()
   {
      internalName = "blurY2";
      
      shader = SSAOBlurYShader;
      stateBlock = SSAOBlurStateBlock;
            
      texture[0] = "$inTex";
      texture[1] = "#prepass";
      
      target = "$outTex"; 
   };
   
   singleton PostEffect()
   {
      internalName = "blurX2";
            
      shader = SSAOBlurXShader;
      stateBlock = SSAOBlurStateBlock;
            
      texture[0] = "$inTex";
      texture[1] = "#prepass";
            
      // We write to a mask texture which is then
      // read by the lighting shaders to mask ambient.
      target = "#ssaoMask";   
   };  
};


/// Just here for debug visualization of the 
/// SSAO mask texture used during lighting.
singleton PostEffect( SSAOVizFx )
{      
   allowReflectPass = false;
        
   shader = PFX_PassthruShader;
   stateBlock = PFX_DefaultStateBlock;
   
   texture[0] = "#ssaoMask";
   
   target = "$backbuffer";
};

singleton ShaderData( SSAOPowTableShader )
{
   DXVertexShaderFile 	= "shaders/common/postFx/ssao/SSAO_PowerTable_V.hlsl";
   DXPixelShaderFile 	= "shaders/common/postFx/ssao/SSAO_PowerTable_P.hlsl";            
   pixVersion = 2.0;
};

singleton PostEffect( SSAOPowTableFx )
{
   shader = SSAOPowTableShader;
   stateBlock = PFX_DefaultStateBlock;
   
   renderTime = "PFXTexGenOnDemand";
   
   target = "#ssao_pow_table"; 
   
   targetFormat = "GFXFormatR16F";   
   targetSize = "256 1";
};
