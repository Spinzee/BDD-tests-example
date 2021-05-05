# BDD-tests-example#
An example of building an IoC Container in test and replacing external calls to databases or APIs with fakes instead of using a mocking framework. 

The tests go through all the layers.  Controller to faked repository and back down to the view.  

Here, we are interested in the behaviour of the system and not the implementation. The idea being, verifying systems behaviour is what all tests should do regardless of their placement on the "testing pyramid".  

This multi-layer testing is useful when changes are made to the system architecture such as introducing new layers.  It also means a refactoring exercise does not involve fixing hundreds of tests that are tightly coupled to specific methods.      

This is a 3-tier, database first, mvc app.
