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
import unittest
import scriptT3D

# just redefining some names to make it easier to read
Engine = scriptT3D
Con = Engine.Con
Globals = Engine.Globals
SimObjects = Engine.SimObjects

# console output callback. TODO: add nice colours based on level
def consolePrint(level, data):
    print data
Engine.ExportConsumer(consolePrint)

# create scheduler object to call functions after engine.tick loop has started
schedule = sched.scheduler(time.time, time.sleep)

def call(function, *args):
    argList = []
    for i, arg in enumerate(args):
        if isinstance(arg, basestring):
            argList.append('"{0}"'.format(arg))
        else:
            argList.append(str(arg))
    callStr = '{0}({1});'.format(function, ','.join(argList))
    return Engine.evaluate(callStr)

def method(obj, function, *args):
    return call('{0}.{1}'.format(obj, function), *args)

def on(callback, do):
    splitted = callback.split('::')
    if len(splitted) > 1:
        cls, fn = splitted[:2]
        return Engine.ExportCallback(do, fn, cls)
    else:
        return Engine.ExportCallback(do, callback)

def obj(o):
    return SimObjects[o]

# convenience function for creating sim objects
def new(className, name='', props=None):
    makeStr = 'return new {0}({1}) {{'.format(className, name)
    if props is not None:
        for key, val in props.items():
            if isinstance(val, basestring):
                makeStr += '{0} = "{1}";'.format(key, val)
            else:
                makeStr += '{0} = {1};'.format(key, val)
    makeStr += '};'
    return SimObjects[Engine.evaluate(makeStr)]

def load(fileName):
    if not fileName.endswith('.cs'):
        if not fileName.endswith('/'):
            fileName += '/'
        fileName += 'main.cs'
    return Engine.evaluate('exec("{0}");'.format(fileName))

def quit():
    return Engine.evaluate('quit();')
exit = quit

def run(setup=None, tick=None):
    if setup is not None:
        schedule.enter(0, 1, setup, ())

    # init the engine
    if not Engine.init(len(sys.argv), sys.argv):
        sys.exit(1)

    # engine loop
    while Engine.tick():
        schedule.run()
        if tick is not None:
            tick()

    # clean up
    Engine.shutdown()

# define unit tests
class GlobalsConTests(unittest.TestCase):
    # tests
    def testa_GlobalsAttribException(self):
        with self.assertRaises(AttributeError):
            val = Globals.badGlobal

    def testb_GlobalsAttrib(self):
        Globals.goodGlobal1 = 10
        self.assertEqual(Globals.goodGlobal1, "10")
        Globals.goodGlobal2 = "text"
        self.assertEqual(Globals.goodGlobal2, "text")

    def testc_ConAttribException(self):
        with self.assertRaises(AttributeError):
            val = Con.badConFunction()

    def testd_ConAttrib(self):
        # this tests the initial creation of the lambda
        val = Con.eval('return "10";')
        self.assertEqual(val, "10")
        # this tests the usage of the same lambda
        val = Con.eval('return "10";')
        self.assertEqual(val, "10")

class SimObjectTests(unittest.TestCase):
    # setup for each test
    def setUp(self):
        # create a new SimObject for each test
        self.simobject = engine.evaluate('return new SimObject(TestSimObject);')
    # cleanup for each test
    def tearDown(self):
        # delete SimObject after each test
        if self.simobject is not None:
            engine.evaluate('{0}.delete();'.format(self.simobject))
            self.simobject = None

    # tests
    def testa_SimObjectsAccessByID(self):
        obj = SimObjects[self.simobject]
        objId = obj.getId()
        self.assertEqual(self.simobject, objId)
        #self.assertTrue(self.simobject == objId)

    def testa_SimObjectsAccessByName(self):
        obj = SimObjects.TestSimObject
        objId = obj.getId()
        self.assertEqual(self.simobject, objId)
        #self.assertTrue(self.simobject == objId)

    def testa_SimObjectsAttribException(self):
        with self.assertRaises(AttributeError):
            obj = SimObjects.badObject

    def testa_SimObjectsKeyException(self):
        with self.assertRaises(KeyError):
            obj = SimObjects["badObject"]
        obj = SimObjects.TestSimObject
        objId = obj.getId()
        with self.assertRaises(KeyError):
            obj = SimObjects[int(objId)+100]

    def testb_SimObjectAttribException(self):
        obj = SimObjects.TestSimObject
        with self.assertRaises(AttributeError):
            val = str(obj.badAttrib)

    def testb_SimObjectDeletedObjectException(self):
        obj = SimObjects.TestSimObject
        obj.delete()
        with self.assertRaises(RuntimeError):
            val = obj.getName()

        # prevent error on tear down
        self.simobject = None

    def testb_SimObjectAttrib(self):
        obj = SimObjects.TestSimObject
        obj.attrib1 = 10
        self.assertEqual(obj.attrib1,str(10))
        obj.attrib2 = "text"
        self.assertEqual(obj.attrib2,"text")
        obj.attrib3[0] = "more text"
        self.assertEqual(obj.attrib3[0],"more text")
        obj.attrib3[1] = "even more text"
        self.assertEqual(obj.attrib3[1],"even more text")
        self.assertEqual(obj.attrib3[2],"")
        self.assertNotEqual(obj.attrib3[3],"more text")

# test callbacks
def testCallback(data):
    obj = None
    if hasattr(testCallback, "__SimObject__"):
        obj = str(testCallback.__SimObject__)

    return (obj, data)

class testObject(object):
    def testClassCallback(self, data):
        obj = None
        if hasattr(self.testClassCallback.__func__, "__SimObject__"):
            obj = str(self.testClassCallback.__func__.__SimObject__)

        return (obj, data)

def unboundCallback(*args):
    obj = None
    if hasattr(unboundCallback, "__SimObject__"):
        obj = str(unboundCallback.__SimObject__)

    # grab last value as data from args when called as object method
    data = args[len(args)-1]
    return (obj, data)

testObject.unboundCallback = unboundCallback

def doCallbackExport():
    # callback tests
    # ExportCallback(func,name,ns,usage,overrides=(true/false))
    tObj = testObject()

    engine.ExportCallback(testCallback,"testCallback")
    engine.ExportCallback(testCallback,"testCallback","SimObject")

    engine.ExportCallback(tObj.testClassCallback,"testClassCallback")
    engine.ExportCallback(tObj.testClassCallback,"testClassCallback","SimObject")

    # this works fine
    engine.ExportCallback(tObj.unboundCallback,"unboundCallback")
    engine.ExportCallback(tObj.unboundCallback,"unboundCallback","SimObject")

    Con.eval('new SimObject(testObject);')

    return tObj

class CallbackTests(unittest.TestCase):
    def testa_CallbackFunction(self):
        obj, data = Con.eval('testCallback("text");')
        self.assertEqual(obj, None)
        self.assertEqual(data, "text")

    def testa_CallbackMethod(self):
        obj, data = Con.eval('testObject.testCallback("text");')
        self.assertEqual(obj, str(engine.getSimObject("testObject")))
        self.assertEqual(data, "text")

    def testb_CallbackClassFunction(self):
        obj, data = Con.eval('testClassCallback("text");')
        self.assertEqual(obj, None)
        self.assertEqual(data, "text")

    def testb_CallbackClassMethod(self):
        obj, data = Con.eval('testObject.testClassCallback("text");')
        self.assertEqual(obj, str(engine.getSimObject("testObject")))
        self.assertEqual(data, "text")

    def testc_CallbackUnboundFunction(self):
        obj, data = Con.eval('unboundCallback("text");')
        self.assertEqual(obj, None)
        self.assertEqual(data, "text")

    def testc_CallbackUnboundMethod(self):
        obj, data = Con.eval('testObject.unboundCallback("text");')
        self.assertEqual(obj, str(engine.getSimObject("testObject")))
        self.assertEqual(data, "text")

displaySplashWindow = 'displaySplashWindow'
