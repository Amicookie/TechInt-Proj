import os
from flask import Flask, g
from peewee import *
from flask_jsonpify import jsonify
from flask_restful import Resource, Api
from flask_cors import CORS

# import json
# from playhouse.shortcuts import model_to_dict, dict_to_model


app = Flask(__name__)
app.config.update(dict(
    SECRET_KEY='bardzosekretnawiadomosc',
    TYTUL='',
    DATABASE=os.path.join(app.root_path, 'test-peewee.db')
))

CORS(app)

ti_db = SqliteDatabase(app.config['DATABASE'])


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


class Files(Resource):
    def get(self):
        from database.db_generator import get_all_files
        return jsonify(get_all_files())


class Users(Resource):
    def get(self):
        from database.db_generator import get_all_users
        return jsonify(get_all_users())


Api(app).add_resource(Files, '/files')  # Route_1
Api(app).add_resource(Users, '/users')  # Route_3
# api.add_resource(FileName, '/files/<file_id>') # Route_3
