from pyT3D import T3D
import game

def t3dBones():
    T3D.displaySplashWindow('splash.bmp')
    T3D.setLogMode(2)
    T3D.trace(False)

    T3D.load('sys')
    T3D.createCanvas('pyT3D')
    T3D.initRenderManager()
    T3D.initLightingSystems('Advanced Lighting')
    T3D.initPostEffects()
    T3D.sfxStartup()

    T3D.load('console')
    game.setup(T3D)

    @T3D.callback('GameConnection')
    def onConnect(self):
        self.transmitDataBlocks(0)

    @T3D.callback('GameConnection')
    def onDatablocksDone(self, a):
        self.activateGhosting()
        self.onEnterGame()

    ServerGroup = T3D.new.SimGroup('ServerGroup')
    ServerConnection = T3D.new.GameConnection('ServerConnection')
    ServerConnection.connectLocal()

    T3D.get.GlobalActionMap.bind('keyboard', 'escape', 'quit')
    game.start(T3D)

    @T3D.callback()
    def onExit():
        ServerConnection.delete()
        game.end(T3D)
        ServerGroup.delete()
        T3D.deleteDataBlocks()

if __name__ == '__main__':
    T3D.run(setup = t3dBones)
