namespace TextFileDatabase.Tests;

internal class TestObject
{
    public string PropertyA { get; set; }
    public string PropertyB { get; set; }
}

public class CsvObjectLoaderTests
{

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestLoadStandardHeaders_Deserialize()
    {
        var testContents = "PropertyA,PropertyB\ntestAPassed,testBPassed";
        File.WriteAllText("testLoad.csv", testContents);
        
        var testLoader = new CsvObjectLoader<TestObject>("testLoad.csv");
        var testObjects = testLoader.Load().ToList();
        
        File.Delete("testLoad.csv");
        Assert.Multiple(() =>
        {
            Assert.AreEqual(1, testObjects.Count);
            Assert.AreEqual("testAPassed", testObjects[0].PropertyA);
            Assert.AreEqual("testBPassed", testObjects[0].PropertyB);
        });
    }
    
    [Test]
    public void TestLoadNoHeaders_Deserialize()
    {
        var testContents = "testAPassed,testBPassed";
        File.WriteAllText("testLoad.csv", testContents);
        
        var testLoader = new CsvObjectLoader<TestObject>("testLoad.csv", false);
        var testObjects = testLoader.Load().ToList();
        
        File.Delete("testLoad.csv");
        Assert.Multiple(() =>
        {
            Assert.AreEqual(1, testObjects.Count);
            Assert.AreEqual("testAPassed", testObjects[0].PropertyA);
            Assert.AreEqual("testBPassed", testObjects[0].PropertyB);
        });
    }

    [Test]
    public void TestLoadWrongHeaders_ThrowError()
    {
        var testContents = "PropertyC,PropertyD\ntestAPassed,testBPassed";
        File.WriteAllText("testLoad.csv", testContents);
        
        var testLoader = new CsvObjectLoader<TestObject>("testLoad.csv");
        Assert.Throws<NullReferenceException>(() => testLoader.Load().ToList(), "Object reference not set to an instance of an object.");
        File.Delete("testLoad.csv");
    }
    
    
    [Test]
    public void TestSaveStandardHeaders_Serialize()
    {
        var testLoader = new CsvObjectLoader<TestObject>("testSave.csv");
        var testObjects = new List<TestObject>()
        {
            new()
            {
                PropertyA = "testAPassed",
                PropertyB = "testBPassed"
            }
        };
        testLoader.Save(testObjects);
        var testContents = "PropertyA,PropertyB\r\ntestAPassed,testBPassed\r\n";
        var actualContents = File.ReadAllText("testSave.csv");
        File.Delete("testSave.csv");
        Assert.AreEqual(testContents, actualContents);
    }
    
    [Test]
    public void TestSaveNoHeaders_Serialize()
    {
        var testLoader = new CsvObjectLoader<TestObject>("testSave.csv", false);
        var testObjects = new List<TestObject>()
        {
            new()
            {
                PropertyA = "testAPassed",
                PropertyB = "testBPassed"
            }
        };
        testLoader.Save(testObjects);
        var testContents = "testAPassed,testBPassed\r\n";
        var actualContents = File.ReadAllText("testSave.csv");
        File.Delete("testSave.csv");
        Assert.AreEqual(testContents, actualContents);
    }
}