singleton TSShapeConstructor(CubeDAE)
{
   baseShape = "./cube.dae";
   forceUpdateMaterials = false;
};

function CubeDAE::onLoad(%this)
{
   %this.addSequence("ambient", "rise", 0, 156);
   %this.setSequenceCyclic("rise", false);
}
