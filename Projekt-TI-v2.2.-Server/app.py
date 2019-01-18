import os
from flask import Flask, g, session, request, flash
from peewee import *
from flask_jsonpify import jsonify
from flask_restful import Resource, Api
from flask_cors import CORS
from flask_hashing import Hashing
from threading import Lock, Thread, Event
from flask_socketio import SocketIO, emit, join_room, leave_room, close_room, rooms, disconnect, send
from ast import literal_eval

async_mode = None

app = Flask(__name__)
app.config.update(dict(
    SECRET_KEY='bardzosekretnawiadomosc',
    TYTUL='',
    DATABASE=os.path.join(app.root_path, 'test-peewee.db')
))

socketio = SocketIO(app)  # async_mode=async_mode)
thread = Thread()
thread_stop_event = Event()
thread_lock = Lock()

CORS(app)
hashing = Hashing(app)

ti_db = SqliteDatabase(app.config['DATABASE'])


@socketio.on('chat')
def handle_chat(message):
    print('Chat', message)
    message_dict = literal_eval(message)
    socketio.emit('chat', {'username': message_dict.get('username'), 'message': message_dict.get('message')})


@socketio.on('fileSaved')
def handle_file_saved(file_saved):
    print('File Saved!', file_saved)
    file_saved_dict = literal_eval(file_saved)
    socketio.emit('fileSaved', {'username': file_saved_dict.get('username'), 'file_name': file_saved_dict.get('file_name')})


@socketio.on('fileLocked')
def handle_file_locked(file_locked_data):
    print('File Locked!', file_locked_data)
    file_locked_data_dict = literal_eval(file_locked_data)
    socketio.emit('fileLocked', {'user_id': file_locked_data_dict.get('user_id'), 'username': file_locked_data_dict.get('username'), 'file_name': file_locked_data_dict.get('file_name'), 'file_id': file_locked_data_dict.get('file_id')})


@socketio.on('fileUnlocked')
def handle_file_unlocked(file_unlocked_data):
    print('File Unlocked!', file_unlocked_data)
    file_unlocked_data_dict = literal_eval(file_unlocked_data)
    socketio.emit('fileUnlocked', {'username': file_unlocked_data_dict.get('username'),
                                 'file_name': file_unlocked_data_dict.get('file_name')})


@socketio.on('fileUpdated')
def handle_file_unlocked(file_updated_data):
    print('File Updated!', file_updated_data)
    file_updated_data_dict = literal_eval(file_updated_data)
    socketio.emit('fileUpdated', {'username': file_updated_data_dict.get('username'),
                                   'file_name': file_updated_data_dict.get('file_name')})


@socketio.on('connect')
def test_connect():
    global thread
    print('Client connected')
    # with thread_lock:
    #     if thread is None:
    #         thread = socketio.start_background_task(background_thread)
    # emit('my_response', {'data': 'Connected', 'count': 0})


@socketio.on('disconnect')
def test_disconnect():
    print('Client disconnected', request.sid)


@app.before_request
def before_request():
    g.db = ti_db
    g.db.connect()


@app.after_request
def after_request(response):
    g.db.close()
    return response


@app.route('/')
def hello_world():
    return 'Hello World!'


class FileList(Resource):
    def get(self):
        from database.db_generator import get_all_files
        return jsonify(get_all_files())

    def post(self):
        from database.db_methods import create_file
        from database.db_generator import get_all_files
        create_file(request.json['file_name'], request.json['file_content'], request.json['user_id'])
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
        # print(get_all_users())
        return jsonify(get_all_users())

    def post(self):
        from database.db_methods import does_user_exist, log_in_user

        login_form = request.json['user_login']
        print(request.json['user_login'])
        exists = does_user_exist(login_form)
        print(exists)
        if not exists:
            # flash('Account with such login does not exist.', 'error')
            print({"user_exists": 0, "logged_in": 0})
            return {"user_exists": 0, "logged_in": 0}
        else:
            password_form = request.json['user_password']
            hashed_input = hashing.hash_value(password_form, salt='secretsalt')
            check_user = log_in_user(login_form, hashed_input)
            if check_user and check_user.user_login == login_form and check_user.user_password == hashed_input:
                session['login'] = login_form
                session['user_id'] = check_user.user_id
                print({"user_exists": 1, "logged_in": 1, "user_id": check_user.user_id})
                return {"user_exists": 1, "logged_in": 1, "user_id": check_user.user_id}
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
        # from database.db_methods import put_user
        return 0


Api(app).add_resource(FileList, '/files')  # Route_1
Api(app).add_resource(Users, '/users')  # Route_2
Api(app).add_resource(OneFile, '/files/<file_id>')  # Route_3
Api(app).add_resource(OneUser, '/users/<user_id>')  # Route_4