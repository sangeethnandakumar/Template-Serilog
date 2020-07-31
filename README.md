# Express.UnitTesting

Express Unit testing is not a library. It's a template project structure for unit and integration testing by making use of XUnit framework

![alt text](https://lh3.googleusercontent.com/proxy/B_7eIUlcSWIhijMsKkvsKeB4sv5ZqG8cOGXyWFKIrIAgPlQTL_RyHreEs5bCSthMBUgPzIuifuFL89mIzpZfMhA)

### Repository Contents
This repo maintains 2 projects. One is a class library which mocks our program to be tested and another is a Unit test project which implements XUnit core features and its configurations. The repo includes template for running unit testing and integration testing with proper file organisation

### PreRequesties
Create an XUnit Unit Test project from Visual Studio Templates and you are good to go

### Folder Structure
The XUnit unit test project is organised with 4 levels of top level directories
| Level | Directory Name | Description
| ------ | ------ | ------
| 0 | Collection | Contains "CollectionBehaviourOverride.cs". By default tests are running in parallel. For integration tests on many scenerios we can't use parallel tests since one test depends on another's output. This project configures xUnit to run not in parallel
| 1 | Fixtures | Fixes are shared classes across different collections. In this project we have a fixture called "LoginFixture". A fixture is responsible for sharing data between tests running under different classes and also to setup environment for tests to run. For example add dummy entries to database before a test login test runs. Fixtures will be run by xUnit before any tests. Fixtures can be associated with test classes using [Collection] attribute
| 2 | Integration Tests | This directoy is where our integration testing classes lives
| 3 | Unit Tests | This directoy is where our unit testing classes lives

# UNIT TESTING
Unit testing in xUnit is very easy. Unit testing tests individual components of a program.
Suppose we have a function to login that accepets only a set of usernames and passwords in our code
```csharp
public class Credential {
  public string Username { get; set; }
  public string Password { get; set; }
  
  public Credential(string username, string password) {
    Username = username;
    Password = password;
  }
}
```
```csharp
public class LoginService {
  public bool PerformLogin(Credential credential) 
  {
    if (credential.Username.Equals("sangeeth") && credential.Password.Equals("sangee")) {
      return true;
    }
    if (credential.Username.Equals("navaneeth") && credential.Password.Equals("navu")) {
      return true;
    }
    if (credential.Username.Equals("surya") && credential.Password.Equals("surya")) {
      return true;
    }
    if (credential.Username.Equals("nandakumar") && credential.Password.Equals("nandu")) {
      return true;
    }
    return false;
  }
}
```
We need to test the above function. So we need todesign the unit test functions.
The reccomended approch is to stick to this naming convention while writing test methords
```csharp
//Naming Convention
private void Methord_Scenerio_ExpectedBehavior() {
  //Arrange
  //Act
  //Assert
}
```
## Simple tests
This is a simple test to check if the function is working properly
```csharp
[Fact]
public void PerformLogin_VaidCredential_ReturnTrue() 
{
  var loginService = new LoginService();
  var isLoggedIn = loginService.PerformLogin(new Credential("sangeeth", "sangee"));
  Assert.True(isLoggedIn);
}
```
To skip the function from testing, Decorate the attribu to this
```csharp
[Fact(Skip = "It's buggy can't test until fixed monday")]
public void PerformLogin_VaidCredential_ReturnTrue()  {
}
```
## Use TestData to test against test functions
Create a function that yields test data result set. Here I'm taking the 1st parameter as expected result and the 2nd parameter as parameters to be give to the function to be tested
```csharp
//Arrange Test Data
public static IEnumerable < object[] > TestData() 
{
  yield return new object[] { true, new Credential("sangeeth", "sangee") };
  yield return new object[] { true, new Credential("navaneeth", "navu") };
  yield return new object[] { true, new Credential("surya", "surya") };
  yield return new object[] { true, new Credential("nandakumar", "nandu") };
  yield return new object[] { false, new Credential("admin", "test") };
}
```
## Create Test Function
After creating the test data function, Let's call it inside a test function that iterates and checks results
```csharp
[Theory]
[MemberData(nameof(TestData))]
public void PerformLogin_TestData_ReturnStatus(object expected, params Credential[] args) 
{
  //Act
  var loginService = new LoginService();
  foreach(var arg in args) {
    var result = loginService.PerformLogin(arg);
    //Assert
    Assert.Equal(expected, result);
  }
}
```


# INTEGRATION TESTING
Integration testing will test the work flow of a specific feature by checking all connected modules are working on set of scenerios without breaking. XUnit is capable enough to drive integration testing. To setup integration testing we need to setup these classes (Better to arrange on the respective directories)

### CollectionBehaviourOverride.cs
Setting this configuration on xUnit assembly allows tests to run in series
```csharp
using Xunit;
[assembly: CollectionBehavior(DisableTestParallelization = true)]
```
### LoginFixture.cs
This is an example of a LoginFixture, That deals with prerequesties of setting and clearing of data before the integration test which we can say -"A user tries to login"
```csharp
using ClassLibrary;
using System.Data.SqlClient;
using Xunit;

namespace Demo.Fixtures { 

  [CollectionDefinition("LoginFixture")]
  public class LoginFixture: IClassFixture < LoginFixture > , IDisposable
  {
    //Initialise required classes and properties
    public SqlConnection Db { get;  private set; }
    public Credential Credential { get; set; }
    public LoginService LoginService { get; set; }

    public LoginFixture() {
      LoginService = new LoginService();
    }

    public void SetupTestdataOnDatabase() {
     // Setup test data or any prerequesties before running tests here
    }

    public void Dispose() {
      // Clean up test data from the database
    }
  }
}
```
### Login.cs
This class contains the tests to be run on integration test mode. This class uses the above mentioned LoginFixture. The Fixture will be run first and after all prerequisties are done, The tests starts to run. The fixture is connected to this class through an attribute [Collection]. A class can have only one fixture which means integration tests to be written like that way. Using ITestOutputHelper allows to log test data to test window on Visual Studio
```csharp
using ClassLibrary;
using Demo.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace Demo.IntegrationTests 
{ 
  [Collection("LoginFixture")]
  public class Login {
    private readonly LoginFixture _login;
    private readonly ITestOutputHelper _output;

    public Login(ITestOutputHelper outputHelper, LoginFixture login) {
      _output = outputHelper;
      _login = login;
    }

    [Fact]
    public void Step1_SetupCredential() {
      //Create credential
      _login.Credential = new Credential("sangeeth", "sangee");
      _output.WriteLine($ "Step1 - Created new credential");
    }

    [Fact]
    public void Step2_SetupDatabase() {
      //Setup database
      _login.SetupTestdataOnDatabase();
      _output.WriteLine($ "Step2 - Setup database");
    }

    [Fact]
    public void Step3_TryLoggingIn() {
      //Try to login
      var result = _login.LoginService.PerformLogin(_login.Credential);
      Assert.True(result);
      _output.WriteLine($ "Step3 - Login : {result}");
    }
  }
}
```
