import os
from flask import Flask, g, session, request, flash
from peewee import *
from flask_jsonpify import jsonify
from flask_restful import Resource, Api, reqparse
from flask_cors import CORS
from flask_hashing import Hashing
from threading import Lock, Thread, Event
from flask_socketio import SocketIO, emit, join_room, leave_room, close_room, rooms, disconnect, send

# import json
# from playhouse.shortcuts import model_to_dict, dict_to_model

async_mode = None

app = Flask(__name__)
app.config.update(dict(
    SECRET_KEY='bardzosekretnawiadomosc',
    TYTUL='',
    DATABASE=os.path.join(app.root_path, 'test-peewee.db')
))

socketio = SocketIO(app)#, async_mode=async_mode)
thread = Thread()
thread_stop_event = Event()
thread_lock = Lock()

CORS(app)
hashing = Hashing(app)

ti_db = SqliteDatabase(app.config['DATABASE'])


def background_thread():
    count = 0
    while True:
        socketio.sleep(10)
        count += 1
        socketio.emit('my_response', {'data': 'Server generated event', 'count': count}, namespace='/test')


# class WebSocketFileHandler(Namespace):
#     def on_file_saved(self, message):
#         session

@socketio.on('my event')
def handle_my_custom_event(json):
    print('received json: ' + str(json))


@socketio.on('fileSaved')
def handle_file_saved(fileSaved):
    print('File Saved!', fileSaved)


@socketio.on('message')
def test_message(message):
    #socketio.emit('message', message)
    print('Message: ' + message)
    send(message)

# @socketio.on('message')
# def chat_message(message):
#     print('Message:' + message)
#     send(message, broadcast=True)


@socketio.on('fileSaved', namespace='/api')
def file_saved(message):
    socketio.emit('fileSaved', message)
   # session['receive_count'] = session.get('receive_count', 0) + 1
   # emit('my_response', {'data': message['data'], 'count': session['receive_count']})


@socketio.on('my_event', namespace='/test')
def test_message(message):
    session['receive_count'] = session.get('receive_count', 0)+1
    emit('my_response', {'data': message['data'], 'count': session['receive_count']})


@socketio.on('my_broadcast_event', namespace='/test')
def test_broadcast_message(message):
    session['receive_count'] = session.get('receive_count', 0) + 1
    emit('my_response',
         {'data': message['data'], 'count': session['receive_count']},
         broadcast=True)


@socketio.on('join', namespace='/test')
def join(message):
    join_room(message['room'])
    session['receive_count'] = session.get('receive_count', 0) + 1
    emit('my_response',
         {'data': 'In rooms: ' + ', '.join(rooms()),
          'count': session['receive_count']})


@socketio.on('leave', namespace='/test')
def leave(message):
    leave_room(message['room'])
    session['receive_count'] = session.get('receive_count', 0) + 1
    emit('my_response',
         {'data': 'In rooms: ' + ', '.join(rooms()),
          'count': session['receive_count']})


@socketio.on('close_room', namespace='/test')
def close(message):
    session['receive_count'] = session.get('receive_count', 0) + 1
    emit('my_response', {'data': 'Room ' + message['room'] + ' is closing.',
                         'count': session['receive_count']},
         room=message['room'])
    close_room(message['room'])


@socketio.on('my_room_event', namespace='/test')
def send_room_message(message):
    session['receive_count'] = session.get('receive_count', 0) + 1
    emit('my_response',
         {'data': message['data'], 'count': session['receive_count']},
         room=message['room'])


@socketio.on('disconnect_request', namespace='/test')
def disconnect_request():
    session['receive_count'] = session.get('receive_count', 0) + 1
    emit('my_response',
         {'data': 'Disconnected!', 'count': session['receive_count']})
    disconnect()


@socketio.on('my_ping', namespace='/test')
def ping_pong():
    emit('my_pong')


@socketio.on('connect')
def test_connect():
    global thread
    print('Client connected')
    with thread_lock:
        if thread is None:
            thread = socketio.start_background_task(background_thread)
    emit('my_response', {'data': 'Connected', 'count': 0})


@socketio.on('disconnect')
def test_disconnect():
    print('Client disconnected', request.sid)


@app.before_request
def before_request():
    g.db = ti_db
    g.db.connect()
    # if 'login' in session:
    #     if '/login' in request.path:
    #         return render_template('index.html')
    #     active_user = User.query.filter_by(user_login=session['login']).first()
    #
    # elif request.path not in ['/', '/login', '/index']:
    #     session.clear()


@app.after_request
def after_request(response):
    g.db.close()
    return response


@app.route('/')
def hello_world():
    return 'Hello World!'


# @app.route('/files')
# def get_files(Resource):
#     from database.db_generator import get_all_files
#     return jsonify(get_all_files())
#
#
# @app.route('/users')
# def get_users(Resource):
#     from database.db_generator import get_all_users
#     return jsonify(get_all_users())

# @app.route('/index', methods=['POST'])
# def handle_login():
#     from database.db_methods import get_user_2
#     if request.method == 'POST':
#         login_form = request.json['user_login']
#         exists = get_user_2(login_form, "null")
#         print(exists)
#         if not exists:
#             flash('Account with such login does not exist.', 'error')
#             return {"user_exists": 0, "logged_in": 0}
#         else:
#             password_form = request.json['user_password']
#             hashed_input = hashing.hash_value(password_form, salt='secretsalt')
#             check_user = get_user_2(login_form, hashed_input)
#             if check_user and check_user.user_login == login_form \
#                     and check_user.user_password == hashed_input:
#                 session['login'] = login_form
#                 session['user_id'] = check_user.user_id
#                 return {"user_exists": 1, "logged_in": 1}
#             else:
#                 flash('Password is not correct. Please try again.', 'error')
#                 return {"user_exists": 1, "logged_in": 0}


class FileList(Resource):
    def get(self):
        from database.db_generator import get_all_files
        return jsonify(get_all_files())

    def post(self):
        from database.db_methods import create_file
        from database.db_generator import get_all_files
        #print(request.get_json(silent=True))
        #create_file(request.json['file_name'], request.json['file_content'], session['user_id'])
        create_file(request.json['file_name'], request.json['file_content'], request.json['user_id'])
        #create_file(request.get_json()['file_name'], request.get_json()['file_content'], request.get_json()['user_id'])
        #print(request.json)
        #print(request.get_json(silent=True))
        return jsonify(get_all_files()[-1])


class OneFile(Resource):
    def get(self, file_id):
        from database.db_methods import get_file
        return jsonify(get_file(file_id))

    def delete(self, file_id):
        from database.db_methods import delete_file
        return delete_file(file_id)

    def put(self, file_id):
        from database.db_methods import put_file, get_file
        put_file(file_id, request.json['file_name'], request.json['file_content'],
                 request.json['user_id'])
        print(jsonify(get_file(request.json['file_id'])))
        return jsonify(get_file(request.json['file_id']))


class Users(Resource):
    def get(self):
        from database.db_generator import get_all_users
        return jsonify(get_all_users())

    def post(self):
        from database.db_generator import get_all_users
        from database.db_methods import does_user_exist, log_in_user
        # print(request.json['user_login'])
        # user_login = (request.json['user_login'])
        # print(user_login)
        # print(does_user_exist(user_login))
        #
        # #print(does_user_exist((request.json['user_login'])))
        # return jsonify(get_all_users())
        login_form = request.json['user_login']
        print(request.json['user_login'])
       # exists = get_user_2(login_form, "null")
        exists = does_user_exist(login_form)
        print(exists)
        if not exists:
            flash('Account with such login does not exist.', 'error')
            print({"user_exists": 0, "logged_in": 0})
            return {"user_exists": 0, "logged_in": 0}
        else:
            password_form = request.json['user_password']
            hashed_input = hashing.hash_value(password_form, salt='secretsalt')
            check_user = log_in_user(login_form, hashed_input)
            if check_user and check_user.user_login == login_form and check_user.user_password == hashed_input:
                session['login'] = login_form
                session['user_id'] = check_user.user_id
                print({"user_exists": 1, "logged_in": 1})
                return {"user_exists": 1, "logged_in": 1}
            else:
                flash('Password is not correct. Please try again.', 'error')
                print({"user_exists": 1, "logged_in": 0})
                return {"user_exists": 1, "logged_in": 0}


class OneUser(Resource):
    def get(self, user_id):
        from database.db_methods import get_user
        return jsonify(get_user(user_id))

    def delete(self, user_id):
        from database.db_methods import delete_user
        return delete_user(user_id)

    def put(self, user_id):
        from database.db_methods import put_user
        return 0


Api(app).add_resource(FileList, '/files')  # Route_1
Api(app).add_resource(Users, '/users')  # Route_2
Api(app).add_resource(OneFile, '/files/<file_id>')  # Route_3
Api(app).add_resource(OneUser, '/users/<user_id>')  # Route_4
