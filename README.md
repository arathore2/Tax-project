## Introduction:

This repo hosts a Tax-Service library, and test cases to ensure that the library works.

## Requirements:

This library was tested on both VS2019, and VS2022.  Both had the .Net Desktop, Azure Development, and ASP.Net and web development modules installed.  The xUnitTest project that I'm using to run my tests runs straight out of the Visual Studio GUI, and I'm unsure whats necessary to make it work on other IDE's.  

## Start Up:

After cloning the Repo, if you want to be able to run the test cases you'll need to add an Environment Variable "JarApiString" with the value being your API Key. If you're unsure how to set Environment variables in Windows, you can follow this guide:  

https://www.architectryan.com/2018/08/31/how-to-change-environment-variables-on-windows-10/

After setting the enviornment variable launch the .sln with Visual Studio, the project will intialize after a few moments.  Once its done loading, you can simply right click on the Taxlib.tests project in the solution explorer, and click "run Tests".  If your tests are under about 300 ms, theres a good chance that my tests are running with my mock TestTaxCollector, and that your Environment Variables have not been updated by Visual Studio.  Simply restart Visual Studio, as VS only updates its Environment Variables on startup.

## Extending Software:

At the moment, the system is very simple, but adding onto it should be fairly straight forward.  If you have new Tax Collectors, you simply need to implement the ITaxCollector interface into a new class, with the class being placed in Models.  In terms of recommendations, we'll likely want to create a factory that uses dependency injection to instantiate the TaxCollector we care about, and initializes the TaxService when it starts up.  
