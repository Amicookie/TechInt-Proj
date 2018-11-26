# TechInt-Proj

## Installation

WebApp requires [Node.js](https://nodejs.org/), [Angular CLI](https://cli.angular.io/), [Python 3.6.](https://www.python.org/) to run.
NativeApp requires [ASP.NET](https://www.asp.net/) to run.
Using following IDE to develop: [PyCharm](https://www.jetbrains.com/pycharm/), [Visual Studio Code](https://code.visualstudio.com/), [Visual Studio 2017](https://visualstudio.microsoft.com/).

Install the following frameworks to start the server:

### [ORM DB] peewee
```sh
$ pip install peewee
```

### [REST API & SOCKET SERVER] Flask
```sh
$ pip install flask
$ pip install flask-cors
$ pip install flask-restful
$ pip install flask-jsonpify
$ pip install flask-hashing
$ pip install flask-socketio
```

## Running the server 
> Without database included 
- Right click on app.py -> run -> stop
- Right click on main.py -> run
- Server runs @ http://127.0.0.1:5000
> With database included 
- Right click on main.py -> run
- Server runs @ http://127.0.0.1:5000

## Running the WebApp
> Check installed Node.js and AngularCLI version in terminal:
```sh
C:\Users\User> node -v
C:\Users\User> npm -v
C:\Users\User> ng v
```
- If above-mentioned commands are not working, check PATH environment variable
> Terminal commands:
```sh
...\Angular-TI-v1-FrontEnd> ng serve
```
- WebApp runs @ http://localhost:4200/
