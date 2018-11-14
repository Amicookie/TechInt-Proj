# from peewee import *
from database.db_model import User, File, _get_date
from playhouse.shortcuts import model_to_dict


# def get_all_files():
#     files = []
#     for file in File.select():
#         # print(file.file_id, file.file_name, file.file_content,
#         #       file.file_creation_date, file.file_update_date,
#         #       file.file_creator_id, file.file_last_editor_id)
#         files.append(model_to_dict(file))
#     #     print(model_to_dict(file))
#     #     print("______________________________")
#     # print(files)
#     return files


def get_file(file_id):
    return model_to_dict(File.get_by_id(file_id))


def get_user(user_id):
    return model_to_dict(User.get_by_id(user_id))


def put_file(file_id, file_name, file_content, file_last_editor_id):
    file_to_update = File.select().where(File.file_id == file_id).get()
    file_to_update.file_name = file_name
    file_to_update.file_content = file_content
    file_to_update.file_last_editor_id = file_last_editor_id
    file_to_update.file_update_date = _get_date()
    file_to_update.save()

    # file_update = File.update(File.file_name=file_name, File.file_content=file_content,
    #                           File.file_last_editor_id=file_last_editor_id,
    #                           File.file_update_date=_get_date()) \
    #                   .where(File.file_id=file_id)
    # file_update.execute()


def put_user(user_id):
    return 0


def delete_file(file_id):
    # jeśli wszyscy potwierdzili, to usuwamy
    return File.delete_by_id(file_id)


def delete_user(user_id):
    return 0
#
# def get_all_users():
#     users = []
#     for user in User.select():
#         # print(user.user_id, user.user_login)
#         users.append(model_to_dict(user))
#     # print()
#     return users

# SQL ALCHEMY
# def get_user(user_input_login, user_input_password):
#     if User.query.filter_by(user_login=user_input_login).first() and user_input_password == "null":
#         return User.query.filter_by(user_login=user_input_login).first()
#     else:
#         return User.query.filter_by(user_login=user_input_login,
#                                     user_password=user_input_password).first()


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
