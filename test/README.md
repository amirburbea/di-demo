Example Architecture

React for UI, redux for application state, redux-saga for side effects middleware, TypeScript throughout
.NET Core web server is the web api, using static files to serve react project

web-pack-devserver in development mode with hot reload (so small changes get auto reloaded as I change code), proxying the requests to .net core


Since you can't use .NET core on linux to interact with David's code (as .NET core C++/CLI or pinvoke seems to be windows only) perhaps there is value in making a simple java based daemon (using JNI to invoke the C++) for running David's code and interacting with the bus and having .NET just read the results over the bus.

[React] => [.NET] <=> {BUS} <=> [Java Server] <{JNI}> [C++ Code]
