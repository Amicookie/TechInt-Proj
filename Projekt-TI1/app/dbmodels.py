# from datetime import datetime
from flask_sqlalchemy import SQLAlchemy
from sqlalchemy import ForeignKey

from app import app


app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False
app.config['SQLALCHEMY_DATABASE_URI'] = 'mysql+pymysql://root:root@127.0.0.1:3306/text_editor_db'
db = SQLAlchemy(app)


class User(db.Model):
    user_id = db.Column(db.Integer, primary_key=True, autoincrement=True)
    user_name = db.Column(db.String(40), nullable=False)
    user_login = db.Column(db.String(40), nullable=False, unique=True)
    user_password = db.Column(db.String(80), nullable=False)
    user_email = db.Column(db.String(40))

    def __init__(self, user_id, user_name, user_login, user_password, user_email):
        self.user_id = user_id
        self.user_name = user_name
        self.user_email = user_email
        self.user_login = user_login
        self.user_password = user_password

    def to_json(self):
        return dict(user_name=self.user_name,
                    user_id=self.user_id,
                    user_email=self.user_email,
                    user_login=self.user_login,
                    user_password=self.user_password)


class File(db.Model):
    file_id = db.Column(db.Integer, primary_key=True, autoincrement=True)
    file_name = db.Column(db.String(40), nullable=False, unique=True)
    file_extension = db.Column(db.String(40), nullable=False)
    file_type = db.Column(db.Boolean, nullable=False)
    user_id = db.Column(db.Integer, ForeignKey('user.user_id'), nullable=True)

    def __init__(self, file_id, file_name, file_extension, file_type, user_id=None):
        self.file_id = file_id
        self.file_name = file_name
        self.file_extension = file_extension
        self.file_type = file_type
        self.user_id = user_id

    def to_json(self):
        return dict(file_name=self.file_name,
                    file_id=self.file_id,
                    file_extension=self.file_extension,
                    file_type=self.file_type,
                    user_id=self.user_id)

# class Users(db.Model):
#     candidate_id = db.Column(db.Integer, primary_key=True, autoincrement=True)
#     name = db.Column(db.String(40), nullable=False)
#     surname = db.Column(db.String(60), nullable=False)
#     technologies = db.Column(db.String(1000))
#     experience = db.Column(db.String(2000))
#     login = db.Column(db.String(40), nullable=False, unique=True)
#     password = db.Column(db.String(80), nullable=False)
#     email = db.Column(db.String(40))
#
#     def __init__(self, name, surname, technologies,
#                  experience, login, password, email):
#         self.name = name
#         self.surname = surname
#         self.technologies = technologies
#         self.experience = experience
#         self.login = login
#         self.password = password
#         self.email = email
#
#     def to_json(self):
#         return dict(name=self.name,
#                     candidate_id=self.candidate_id,
#                     surname=self.surname,
#                     email=self.email,
#                     technologies=self.technologies,
#                     experience=self.experience,
#                     login=self.login,
#                     password=self.password)
#
#
# class User(db.Model):
#     user_id = db.Column(db.Integer, primary_key=True, autoincrement=True)
#     group_id = db.Column(db.Integer, db.ForeignKey('group.group_id'))
#     user_type = db.Column(db.String(10), nullable=False)
#     name = db.Column(db.String(40), nullable=False)
#     surname = db.Column(db.String(60), nullable=False)
#     login = db.Column(db.String(40), nullable=False, unique=True)
#     password = db.Column(db.String(80), nullable=False)
#     email = db.Column(db.String(40))
#     Task = db.relationship("Task", uselist=False, backref="user")
#
#     def __init__(self, group_id, user_type, name,
#                  surname, login, password, email):
#         self.group_id = group_id
#         self.user_type = user_type
#         self.name = name
#         self.surname = surname
#         self.login = login
#         self.password = password
#         self.email = email
#
#     def to_json(self):
#         return dict(group_id=self.group_id,
#                     user_id=self.user_id,
#                     user_type=self.user_type,
#                     name=self.name,
#                     surname=self.surname,
#                     login=self.login,
#                     password=self.password,
#                     email=self.email,
#                     link='/admin/manage/users/progress/?login=' + self.login,
#                     user_type_color='cyan' if self.user_type == 'guardian' else 'blue')
#
#     def guardian_info_to_json(self):
#         return dict(guardian_name=self.name,
#                     guardian_surname=self.surname,
#                     guardian_email=self.email)
#
#     def user_type_color_to_json(self):
#         return dict(user_type_color='cyan' if self.user_type == 'guardian' else 'blue')
#
#
# class Group(db.Model):
#     group_id = db.Column(db.Integer, primary_key=True, autoincrement=True)
#     name = db.Column(db.String(40), nullable=False, unique=True)
#     user = db.relationship("User", uselist=False, backref="group")
#     Task = db.relationship("Task", uselist=False, backref="group")
#
#     def __init__(self, name):
#         self.name = name
#
#     def to_json(self):
#         return dict(group_id=self.group_id,
#                     name=self.name,
#                     link='/admin/manage/groups/progress/?id=' + str(self.group_id))
#
#
# class Task(db.Model):
#     task_id = db.Column(db.Integer, primary_key=True, autoincrement=True)
#     group_id = db.Column(db.Integer, db.ForeignKey('group.group_id'), nullable=False)
#     taken_by = db.Column(db.String(40), db.ForeignKey('user.login'))
#     content = db.Column(db.String(1000), nullable=False)
#     task_priority = db.Column(db.String(40), nullable=False)
#     task_status = db.Column(db.String(40), nullable=False)
#     date = db.Column(db.DateTime, nullable=False)
#
#     def __init__(self, group_id, content,
#                  task_priority, task_status):
#         self.group_id = group_id
#         self.content = content
#         self.task_priority = task_priority
#         self.task_status = task_status
#         self.date = datetime.now()
#
#     def to_json(self):
#         return dict(task_id=self.task_id,
#                     group_id=self.group_id,
#                     content=self.content,
#                     task_priority=self.task_priority,
#                     task_status=self.task_status,
#                     date=self.date,
#                     taken_by=self.taken_by,
#                     task_color=self.determine_task_color(self.task_priority))
#
#     @staticmethod
#     def determine_task_color(priority):
#         if priority == 'High priority':
#             return 'red'
#         elif priority == 'Medium priority':
#             return 'orange'
#         elif priority == 'Low priority':
#             return 'green'
