from app.dbmodels import db, User, File

db.drop_all()
db.create_all()

user_1 = User(1,
              'SexyToaster',
              'sexytoaster69',
              '79e331f0a7209c2746d4965331bf90baa36d47f959965359c3f9f86e7b6b38f5',
              'sexytoaster69@gmail.com')

user_2 = User(2,
              'SexyToaster2',
              'sexytoaster692',
              '79e331f0a7209c2746d4965331bf90baa36d47f959965359c3f9f86e7b6b38f5',
              'sexytoaster692@gmail.com')

user_3 = User(3,
              'SexyToaster3',
              'sexytoaster693',
              '79e331f0a7209c2746d4965331bf90baa36d47f959965359c3f9f86e7b6b38f5',
              'sexytoaster693@gmail.com')

db.session.add_all([user_1, user_2, user_3])

file1 = File(1,
             'How To Be A Heartbreaker?',
             ".txt",
             1)

file2 = File(2,
             'Hate everybody!',
             ".txt",
             0)

file3 = File(3,
             'Allnighter',
             ".txt",
             0)

db.session.add_all([file1, file2, file3])

db.session.commit()
