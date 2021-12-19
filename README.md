## Introduction:

This repo hosts a Tax-Service library, and test cases to ensure that the library works.

## Start Up:

After cloning the Repo, if you want to be able to run the test cases you'll need to add an Enviorment Variable "JarApiString" with the value being your API Key.
Afterwards, you can run my TaxServiceTests.cs, and run through my test cases.

## Extending Software:

At the moment, the system is very simple, but adding onto it should be fairly straight forward.  If you have new Tax Collectors, you simply need to implement the ITaxCollector interface into a new class, with the class being placed in Models.  In terms of recommendations, we'll likely want to create a factory that picks uses dependency injection to instantiate the TaxCollector we care about, and initializes the TaxService when it starts up.  
