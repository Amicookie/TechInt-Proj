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

# Helping variables for setting good response on SOCKETS
list_of_locked_files = []
count_list_of_locked_files = 0
created_file_id = -1
updated_file_id = -1

# Global variables defining connection type and other stuff :D
client_type = []

CORS(app)
hashing = Hashing(app)

ti_db = SqliteDatabase(app.config['DATABASE'])

# SOCKET.IO HANDLERS


@socketio.on('chat')
def handle_chat(message):
    print('Chat', message)
    message_dict = literal_eval(message)
    socketio.emit('chat', {'username': message_dict.get('username'), 'message': message_dict.get('message')})


@socketio.on('fileSaved')
def handle_file_saved(file_saved):
    print('File Saved!', file_saved)
    file_saved_dict = literal_eval(file_saved)

    global created_file_id
    file_saved_dict['file_id'] = created_file_id
    print('File Saved Details', file_saved_dict)
    socketio.emit('fileSaved', {'username': file_saved_dict.get('username'),
                                'file_name': file_saved_dict.get('file_name'),
                                'file_id': file_saved_dict.get('file_id')})
    created_file_id = -1
    print('Resetted created_file_id' + str(created_file_id))


@socketio.on('fileLocked')
def handle_file_locked(file_locked_data):
    print('File Locked!', file_locked_data)
    file_locked_data_dict = literal_eval(file_locked_data)
    global count_list_of_locked_files
    count_list_of_locked_files += 1
    file_locked_data_dict['count'] = count_list_of_locked_files

    list_of_locked_files.append(file_locked_data_dict)
    print(list_of_locked_files)
    socketio.emit('fileLocked', {'user_id': file_locked_data_dict.get('user_id'),
                                 'username': file_locked_data_dict.get('username'),
                                 'file_name': file_locked_data_dict.get('file_name'),
                                 'file_id': file_locked_data_dict.get('file_id'),
                                 'list_of_files': list_of_locked_files,
                                 'count': count_list_of_locked_files})


@socketio.on('fileUnlocked')
def handle_file_unlocked(file_unlocked_data):
    print('File Unlocked!', file_unlocked_data)
    file_unlocked_data_dict = literal_eval(file_unlocked_data)
    global count_list_of_locked_files
    index_of_unlocked_file = -1

    for locked_file in list_of_locked_files:
        index_of_unlocked_file += 1
        if locked_file.get('file_id') == file_unlocked_data_dict.get('file_id'):
            list_of_locked_files.remove(locked_file)
            count_list_of_locked_files -= 1
            break
    # list_of_locked_files.remove(file_unlocked_data_dict)

    print(list_of_locked_files)
    socketio.emit('fileUnlocked', {'username': file_unlocked_data_dict.get('username'),
                                   'file_name': file_unlocked_data_dict.get('file_name'),
                                   'file_id': file_unlocked_data_dict.get('file_id'),
                                   'user_id': file_unlocked_data_dict.get('user_id'),
                                   'index_of_unlocked_file': index_of_unlocked_file})


@socketio.on('fileUpdated')
def handle_file_updated(file_updated_data):
    print('File Updated!', file_updated_data)
    file_updated_data_dict = literal_eval(file_updated_data)
    global updated_file_id
    file_updated_data_dict['file_id'] = updated_file_id
    print('File Updated Details', file_updated_data_dict)
    socketio.emit('fileUpdated', {'username': file_updated_data_dict.get('username'),
                                  'file_name': file_updated_data_dict.get('file_name'),
                                  'file_id': file_updated_data_dict.get('file_id')})
    updated_file_id = -1


@socketio.on('fileDeletion')
def handle_file_deletion(file_deletion_data):
    print('File Deletion: ', file_deletion_data)
    file_deletion_data_dict = literal_eval(file_deletion_data)
    socketio.emit('fileDeletion', {'username': file_deletion_data_dict.get('username'),
                                   'file_name': file_deletion_data_dict.get('file_name'),
                                   'file_id': file_deletion_data_dict.get('file_id')})


@socketio.on('connect')
def test_connect():
    global thread
    print('Client connected', request.sid)


@socketio.on('socketConnectionTypeHelper')
def helper(user_id):
    print('Client sid: ', request.sid, 'User_id: ', user_id)
    global client_type
    user_id_dict = literal_eval(user_id)
    for cli in client_type:
        if cli['client_sid'] == request.sid and cli['user_id'] == -1:
            cli['user_id'] = user_id_dict.get('user_id')
            break
    print("Global client_type TYPE HELPER: ", client_type)


@socketio.on('connectionType')
def connection_type(conn_type):
    print('Client type and ID: ', conn_type, ', Client sid: ', request.sid)
    conn_type_dict = literal_eval(conn_type)

    global client_type
    # for cli in client_type:
    #     if cli.sid == request.sid:
    #         cli.client_type = connection_type
    #         break
    #     else
    #

    # if conn_type_dict.get('user_id') == NULL

    # conn_type_dict.get('user_id', -1) --- set '-1' if user_id is null

    client_type.append({"client_sid": request.sid, "client_type": conn_type_dict.get('connectionType'),
                        "user_id": conn_type_dict.get('user_id', -1)})
    print("Global client_type connected: ", client_type)

    # with thread_lock:
    #     if thread is None:
    #         thread = socketio.start_background_task(background_thread)
    # emit('my_response', {'data': 'Connected', 'count': 0})


@socketio.on('disconnect')
def test_disconnect():
    print('Client disconnected', request.sid)
    global client_type
    for cli in client_type:
        if cli['client_sid'] == request.sid:
            for file in list_of_locked_files:
                if file.get('user_id') == cli['user_id']:
                    socketio.emit('fileUnlocked', file)
            client_type.remove(cli)
            break
    print("Global client_type disconnected:", client_type)


# REST HANDLERS


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
        from database.db_methods import create_file, get_user
        from database.db_generator import get_all_files
        create_file(request.json['file_name'], request.json['file_content'], request.json['user_id'])
        print('create_file user id by request: ', request.json['user_id'])
        global created_file_id
        created_file_id = get_all_files()[-1].get('file_id')
        created_file = get_all_files()[-1]
        print('Created file contents: ', created_file)
        print('Created file try to get one of the contents', created_file.get('file_creator_id'))
        print('Is_Online_File value:', request.json['is_online_file'])

        print('Created file id:' + str(created_file_id))
        if not request.json['is_online_file']:
            print('File created OFFLINE')
            created_file_username = get_user(created_file.get('file_creator_id')).get('user_login')
            print('created file username: ', created_file_username)
            socketio.emit('fileSaved', {'username': created_file_username,
                                        'file_name': created_file.get('file_name'),
                                        'file_id': created_file.get('file_id')})
            created_file_id = -1
            print('Resetted created_file_id [offline]: ' + str(created_file_id))
        elif request.json['is_online_file'] is None or request.json['is_online_file'] != '':
            print('File created ONLINE by Webapp')
        else:
            print('File created ONLINE')
        return jsonify(get_all_files()[-1])


class OneFile(Resource):
    def get(self, file_id):
        from database.db_methods import get_file
        print(str((get_file(file_id))))
        return jsonify(get_file(file_id))

    def delete(self, file_id):
        from database.db_methods import delete_file
        return delete_file(file_id)

    def put(self, file_id):
        from database.db_methods import put_file, get_file, get_user
        put_file(file_id, request.json['file_name'], request.json['file_content'],
                 request.json['user_id'])
        print(jsonify(get_file(request.json['file_id'])))
        global updated_file_id
        updated_file_id = request.json['file_id']
        updated_file = get_file(request.json['file_id'])

        print("Request.json['file_id']: ", request.json['file_id'])
        print("file_id:", file_id)

        print('Updated file id:' + str(updated_file_id))

        if not request.json['is_online_file']:
            print('File edited OFFLINE')

            updated_file_username = get_user(updated_file.get('file_last_editor_id')).get('user_login')
            print('Edited file username: ', updated_file_username)
            socketio.emit('fileUpdated', {'username': updated_file_username,
                                          'file_name': updated_file.get('file_name'),
                                          'file_id': updated_file.get('file_id')})
            updated_file_id = -1
            print('Resetted updated_file_id [offline]: ' + str(updated_file_id))
        elif request.json['is_online_file'] is None or request.json['is_online_file'] != '':
            print('File edited ONLINE by Webapp')
        else:
            print('File edited ONLINE')

        return jsonify(get_file(request.json['file_id']))


class Users(Resource):
    def get(self):
        from database.db_generator import get_all_users
        # print(get_all_users())
        return jsonify(get_all_users())

    def post(self):

        from database.db_methods import does_user_exist, log_in_user
        #if request.json['connection_type'] == "desktop":
            #{
                #print('Native user is logged in')
            #}
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

                print({"user_exists": 1, "logged_in": 1, "user_id": check_user.user_id,
                       "list_of_locked_files": list_of_locked_files})
                return {"user_exists": 1, "logged_in": 1, "user_id": check_user.user_id,
                        "list_of_locked_files": list_of_locked_files}
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
