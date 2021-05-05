# BDD-tests-example#
An example of building an IoC Container in test and replacing external calls to databases or APIs with a Fake instead of using a mocking framework. 

The tests go through all the layers.  Controller to faked Repository and back down to the view.  Here, we are interested in the behaviour of the system and not the implementation.   

Verfying systems behvaiour is what all tests should do, regardless of the "testing pyramid".  

This multi-layer testing is useful for when refactoring involves changes in system architecture such as introducing new layers.  

This is an 3-tier, database first, mvc app.
