def setup(T3D):
    T3D.load('game/playGui.gui')

    T3D.datablock.CameraData('Observer')

    T3D.singleton.Material('BlankWhite', {
        'diffuseColor': ['White']
    })
    T3D.singleton.Material('PythonYellow', {
        'diffuseColor': ['Yellow'],
        'mapTo': 'PythonYellow'
    })
    T3D.singleton.Material('PythonBlue', {
        'diffuseColor': ['Blue'],
        'mapTo': 'PythonBlue'
    })

    @T3D.callback('GameConnection')
    def onEnterGame(self):
        camera = T3D.new.Camera('TheCamera', {
            'datablock': 'Observer'
        })
        camera.setTransform('0 0 2 1 0 0 0')
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
        'texSize': 1024,
        'castShadows': True
    })
    group.add(sun)

    logo = T3D.new.RenderShapeExample({
        'shapeFile': 'game/PythonLogo.dae',
        'position': '0 3 2'
    })
    group.add(logo)

def end(T3D):
    T3D.get.GameGroup.delete()
