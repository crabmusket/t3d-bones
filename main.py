import pyT3D as T3D
import ast
import game

def t3dBones():
    #T3D.load('main.cs')
    T3D.call('displaySplashWindow', 'splash.bmp')
    T3D.call('setLogMode', 2)
    T3D.call('trace', False)

    T3D.load('sys')
    T3D.call('createCanvas', 'pyT3D')
    T3D.call('initRenderManager')
    T3D.call('initLightingSystems', 'Advanced Lighting')
    T3D.call('initPostEffects')
    T3D.call('sfxStartup')

    def doNothing():
        pass

    T3D.on('onDatablockObjectReceived', doNothing)
    T3D.on('onGhostAlwaysObjectReceived', doNothing)
    T3D.on('onGhostAlwaysStarted', doNothing)
    T3D.on('updateTSShapeLoadProgress', doNothing)

    def onConnect():
        if hasattr(onConnect, "__SimObject__"):
            obj = (str(onConnect.__SimObject__))
            print T3D.Engine.SimObject(obj)
        #T3D.obj(client).transmitDataBlocks(0)
    #T3D.on('GameConnection::onConnect', onConnect)

    def testCallback(data):
        obj = None
        if hasattr(testCallback, "__SimObject__"):
            help(testCallback.__SimObject__)
        return (obj, data)
    T3D.Engine.ExportCallback(testCallback,"testCallback","SimObject")

    print "========================="
    T3D.Engine.evaluate('new ScriptObject(testObject);')
    T3D.Engine.evaluate('testObject.testCallback("test");')
    callee = T3D.SimObjects.testObject
    print callee.getName()
    print "========================="

    def datablocksDone(self, client):
        T3D.obj(client).activateGhosting()
        T3D.obj(client).onEnterGame()
    #T3D.on('GameConnection::onDataBlocksDone', datablocksDone)

    T3D.load('console')
    game.setup()

    ServerGroup = T3D.new('SimGroup', name = 'ServerGroup')
    ServerConnection = T3D.new('GameConnection', name = 'ServerConnection')
    ServerConnection.connectLocal()

    game.start()

    def onExit():
        game.end()
        ServerGroup.delete()
        T3D.call('deleteDataBlocks')
    T3D.on('onExit', onExit)

if __name__ == '__main__':
    T3D.run(setup = t3dBones)
