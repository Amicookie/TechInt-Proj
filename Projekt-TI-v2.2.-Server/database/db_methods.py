# from peewee import *
from database.db_model import User, File, _get_date
from playhouse.shortcuts import model_to_dict


def get_file(file_id):
    return model_to_dict(File.get_by_id(file_id))


def get_user(user_id):
    user = User.get_by_id(user_id)
    #user = User.select().where(User.user_id == user_id).get()
    return user.to_json()


def put_file(file_id, file_name, file_content, file_last_editor_id):
    file_to_update = File.select().where(File.file_id == file_id).get()
    file_to_update.file_name = file_name
    file_to_update.file_content = file_content
    file_to_update.file_last_editor_id = file_last_editor_id
    file_to_update.file_update_date = _get_date()
    file_to_update.save()


def put_user(user_id):
    return 0


def delete_file(file_id):
    # jeśli wszyscy potwierdzili, to usuwamy
    return File.delete_by_id(file_id)


def delete_user(user_id):
    return 0


def does_user_exist(user_ilogin):
    if User.select().where(User.user_login == user_ilogin):
        return True
    else:
        return False


def log_in_user(user_ilogin, user_ipassword):
    user = User.select().where(User.user_login == user_ilogin, User.user_password == user_ipassword)
    for row in user:
        if user:
            return row
        else:
            return False


def create_file(fname, fcontent, fcid):
    File.create(file_name=fname, file_content=fcontent, file_creator_id=fcid, file_last_editor_id=fcid)
