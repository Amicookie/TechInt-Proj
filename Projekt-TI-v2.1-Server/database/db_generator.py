from database.db_model import User, File, _get_date
# from flask_jsonpify import jsonify
from playhouse.shortcuts import model_to_dict  # , dict_to_model
# import json


def insert_data():
    users = [
        {'user_login': 'user',
         'user_password': '79e331f0a7209c2746d4965331bf90baa36d47f959965359c3f9f86e7b6b38f5'},
        {'user_login': 'sexytoaster69',
         'user_password': '79e331f0a7209c2746d4965331bf90baa36d47f959965359c3f9f86e7b6b38f5'},
    ]

    User.insert_many(users).execute()

    files = [
        {'file_name': 'File1', 'file_content': 'This is how to be a heartbreaaaaaaaaaaaaker! XD',
         'file_creation_date': _get_date(), 'file_update_date': _get_date(), 'file_creator_id': 1,
         'file_last_editor_id': 1},
        {'file_name': 'File2',
         'file_content': 'Description of this text is unbelievably bad. And the content is bad, too.',
         'file_creation_date': _get_date(), 'file_update_date': _get_date(), 'file_creator_id': 1,
         'file_last_editor_id': 1},
        {'file_name': 'File3',
         'file_content': 'It\'s MIIIIIIIIIIIDNIGHT, losing control now, '
                         'IT\'S ALLLLLLLLLLRIGHT, Music is so loud! IT\'S MIDNIGHT! PA PA PARA PA PA',
         'file_creation_date': _get_date(), 'file_update_date': _get_date(), 'file_creator_id': 1,
         'file_last_editor_id': 1},
        {'file_name': 'File4',
         'file_content': 'Jestem bardzo kreatywnym plikiem, czcijcie mnie! Bo jestem zmieniony! I już nie',
         'file_creation_date': _get_date(), 'file_update_date': _get_date(), 'file_creator_id': 1,
         'file_last_editor_id': 1},
        {'file_name': 'File5',
         'file_content': 'Jestem bardzo inspirującym plikiem i kocham być inspirujący! Quote: Ja inspiruję -Plik.',
         'file_creation_date': _get_date(), 'file_update_date': _get_date(), 'file_creator_id': 1,
         'file_last_editor_id': 1}
    ]

    File.insert_many(files).execute()
    print("Users and files added!")


def get_all_files():
    files = []
    for file in File.select():
        # print(file.file_id, file.file_name, file.file_content,
        #       file.file_creation_date, file.file_update_date,
        #       file.file_creator_id, file.file_last_editor_id)
        files.append(model_to_dict(file))
    #     print(model_to_dict(file))
    #     print("______________________________")
    # print(files)
    return files


def get_all_users():
    users = []
    for user in User.select():
        users.append(user.to_json())

    return users

# get_all_files()
# get_all_users()
