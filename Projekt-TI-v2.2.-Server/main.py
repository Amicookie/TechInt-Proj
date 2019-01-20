from app import app, os, ti_db
from database.db_model import User, File
from database.db_generator import insert_data
from database.db_generator import get_all_files
from app import socketio

if __name__ == '__main__':
    if not os.path.exists(app.config['DATABASE']):
        ti_db.connect()
        ti_db.create_tables([User, File])
        insert_data()
        get_all_files()
    #socketio.run(app, host="192.168.0.108", port=5000, debug=True)
    socketio.run(app, port=5000, debug=True)
