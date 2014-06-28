def setup(T3D):
    T3D.load('game/playGui.gui')

    # :(
    T3D.Engine.evaluate('datablock CameraData(Observer) {};')
    T3D.Engine.evaluate('''
    singleton Material(BlankWhite) {
        diffuseColor[0] = "White";
    };
    ''')

    @T3D.callback("GameConnection")
    def onEnterGame(self):
        camera = T3D.new.Camera('TheCamera', {
            'datablock': 'Observer'
        })
        camera.setTransform("0 0 2 1 0 0 0")
        camera.scopeToClient(self)
        T3D.get.GameGroup.add(camera)
        self.setControlObject(camera)
        T3D.get.Canvas.setContent('PlayGui')
        T3D.activateDirectInput()

def start(T3D):
    group = T3D.new.SimGroup('GameGroup')

    info = T3D.new.LevelInfo({
        'canvasClearColor': 'Black'
    })
    group.add(info)

    ground = T3D.new.GroundPlane('TheGround', {
        'position': '0 0 0',
        'material': 'BlankWhite'
    })
    group.add(ground)

    sun = T3D.new.Sun('TheSun', {
        'azimuth': 230,
        'elevation': 45,
        'color': 'White',
        'ambient': '0.1 0.1 0.1',
        'castShadows': True
    })
    group.add(sun)

def end(T3D):
    T3D.get.GameGroup.delete()
