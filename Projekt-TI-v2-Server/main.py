from app import app, os, ti_db
from database.db_model import User, File
from database.db_generator import insert_data, get_all_files

if __name__ == '__main__':
    if not os.path.exists(app.config['DATABASE']):
        ti_db.connect()
        ti_db.create_tables([User, File])
        insert_data()
        get_all_files()
    app.run(port=5000, debug=True)
