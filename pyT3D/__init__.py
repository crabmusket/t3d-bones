"""
Copyright (c) 2014 Frank Carney and Daniel Buckmaster

Permission is hereby granted, free of charge, to any person obtaining a copy of this
software and associated documentation files (the "Software"), to deal in the Software
without restriction, including without limitation the rights to use, copy, modify,
merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to the following
conditions:
The above copyright notice and this permission notice shall be included in all copies
or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
OR OTHER DEALINGS IN THE SOFTWARE.
"""

import sys
import sched, time
import scriptT3D
from functools import wraps
import inspect
import types

# just redefining some names to make it easier to read
Engine = scriptT3D
Con = Engine.Con
Globals = Engine.Globals
SimObjects = Engine.SimObjects

class _T3D:
    def call(self, function, *args):
        argList = []
        for i, arg in enumerate(args):
            if isinstance(arg, basestring):
                argList.append('"{0}"'.format(arg))
            else:
                argList.append(str(arg))
        callStr = '{0}({1});'.format(function, ','.join(argList))
        return Engine.evaluate(callStr)

    def __getattr__(self, name):
        def caller(*args):
            self.call(name, *args)
        return caller

    def method(self, obj, function, *args):
        return call('{0}.{1}'.format(obj, function), *args)

    def on(self, callback, do):
        splitted = callback.split('::')
        if len(splitted) > 1:
            cls, fn = splitted[:2]
            return Engine.ExportCallback(do, fn, cls)
        else:
            return Engine.ExportCallback(do, callback)

    def obj(self, o):
        return SimObjects[o]

    def load(self, fileName):
        if not fileName.endswith('.cs'):
            if not fileName.endswith('/'):
                fileName += '/'
            fileName += 'main.cs'
        return Engine.evaluate('exec("{0}");'.format(fileName))

    def quit(self):
        return Engine.evaluate('quit();')
    exit = quit

    def run(self, setup=None, tick=None):
        if setup is not None:
            self.schedule.enter(0, 1, setup, ())
        if not Engine.init(len(sys.argv), sys.argv):
            sys.exit(1)
        while Engine.tick():
            self.schedule.run()
            if tick is not None:
                tick()
        Engine.shutdown()

    # Decorator to bind a function to a script callback
    def callback(self, namespace = None, cls = None):
        def decorator(func):
            @wraps(func)
            def wrapper(*args):
                if hasattr(wrapper, "__SimObject__"):
                    obj = SimObjects[Engine.getSimObjectID(wrapper.__SimObject__)]
                    if cls is not None:
                        # NOT WORKING
                        obj.__class__ = cls
                    return func(obj, *args)
                else:
                    return func(*args)
            if namespace is None:
                self.on(func.__name__, wrapper)
            else:
                self.on(namespace + "::" + func.__name__, wrapper)
            return wrapper
        return decorator

    # Not working
    def scriptClass(self, cls):
        newtype = type(cls.__name__, (Engine.SimObject, object), dict(cls.__dict__))
        for name, fn in inspect.getmembers(newtype):
            if len(name) > 2 and name[:2] == '__':
                continue
            if isinstance(fn, types.UnboundMethodType):
                setattr(newtype, name, self.callback(newtype.__name__, newtype)(fn))
        return newtype

    def __init__(self):
        # Expose engine
        self.Engine = Engine
        self.Con = Con
        self.Globals = Globals
        self.get = self.SimObjects = SimObjects

        # WHat to do with lines printed from the console
        Engine.ExportConsumer(consolePrint)

        # Create scheduler object to call functions after engine.tick loop has started
        self.schedule = sched.scheduler(time.time, time.sleep)

        # Script object constructor
        self.new = _New()

# console output callback. TODO: add nice colours based on level
def consolePrint(level, data):
    print data

class _New:
    def __getattr__(self, className):
        def new(name='', props=None):
            makeStr = 'return new {0}({1}) {{'.format(className, name)
            if props is not None:
                for key, val in props.items():
                    if isinstance(val, basestring):
                        makeStr += '{0} = "{1}";'.format(key, val)
                    else:
                        makeStr += '{0} = {1};'.format(key, val)
            makeStr += '};'
            return SimObjects[Engine.evaluate(makeStr)]
        return new

T3D = _T3D()
