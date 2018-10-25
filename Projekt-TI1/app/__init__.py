from app.app import app
# from app.static import

from app.views.user import user_view
from app.views.default import default_view
from app.dbmodels import db

app.config.from_object('config')

app.register_blueprint(default_view)
app.register_blueprint(user_view)

db.init_app(app)
