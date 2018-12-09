import datetime
from app import ti_db
from peewee import *


def _get_date():
    return datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")


class base_model(Model):
    class Meta:
        database = ti_db


class User(base_model):
    user_id = AutoField(primary_key=True)
    user_login = CharField(null=False, unique=True)
    user_password = CharField(null=False)

    def to_json(self):
        return dict(user_id=self.user_id,
                    user_login=self.user_login,
                    user_password=self.user_password)


class File(base_model):
    file_id = AutoField(primary_key=True)
    file_name = CharField(null=False)
    file_content = TextField(null=False)
    file_creation_date = DateTimeField(null=False, default=_get_date())
    file_update_date = DateTimeField(null=False, default=_get_date())
    file_creator_id = SmallIntegerField(null=False)
    file_last_editor_id = SmallIntegerField(null=False)
    user_id = ForeignKeyField(User, related_name='user_id', null=True)

    def to_json(self):
        return dict(file_id=self.file_id,
                    file_name=self.file_name,
                    file_content=self.file_content,
                    file_creation_date=self.file_creation_date,
                    file_update_date=self.file_update_date,
                    file_creator_id=self.file_creator_id,
                    file_last_editor_id=self.file_last_editor_id,
                    user_id=self.user_id)

# ti_db.connect()
# ti_db.create_tables([User, File])
