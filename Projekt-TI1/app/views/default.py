from flask import render_template, Blueprint, request, url_for, flash, session #, Response, json
from werkzeug.utils import redirect
from app.app import hashing

from app.dbmodels import User #, File

#from app.views.viewsUtils import send_json_response

# Blueprint sets name to import views
default_view = Blueprint('default_view', __name__, template_folder='templates')


@default_view.before_request
def check_session():
    if 'login' in session:
        if '/login' in request.path:
            return render_template('index.html')
        active_user = User.query.filter_by(user_login=session['login']).first()

        # if active_user:
        #     if active_user.group_id is not None and 'active_group' in session:
        #         session['active_group'] = active_user.group_id
        #     elif 'active_group' in session and active_user.group_id is None:
        #         session.pop('active_group')
        #         return render_template('index.html')

    elif request.path not in ['/', '/login', '/index']:
        session.clear()
        return render_template('login.html')


default_view.before_request(check_session)


@default_view.route('/')
def init_index():
    if 'login' in session:
        return render_template('index.html')
    else:
        return redirect(url_for('default_view.init_login'))


@default_view.route('/login')
def init_login():
    return render_template('login.html')


@default_view.route('/files/create')
def init_create_new_file():
    return render_template('user/createNewFile.html')


# @default_view.route('/user/manage/data')
# def init_edit_user_data():
#     active_user = User.query.filter_by(login=session['login']).first()
#     return render_template('user/changeUserData.html', active_user=active_user)

@default_view.route('/files/list')
def init_list_files():
    active_user = User.query.filter_by(user_login=session['login']).first()
    return render_template('user/listFiles.html', active_user=active_user)


# @default_view.route('/user/manage/password')
# def init_edit_user_password():
#     return render_template('user/changeUserPassword.html')


@default_view.route('/credits')
def init_view_credits():
    return render_template('user/viewCredits.html')


@default_view.route('/index', methods=['GET', 'POST'])
def handle_login():
    if request.method == 'POST':
        login_form = request.form['login']
        exists = User.query.filter_by(user_login=login_form).first()
        if not exists:
            flash('Account with such login does not exist.', 'error')
            return redirect(url_for('default_view.init_login'))
        else:
            password_form = request.form['password']
            hashed_input = hashing.hash_value(password_form, salt='secretsalt')
            check_user = User.query.filter_by(user_login=login_form,
                                              user_password=hashed_input).first()
            if check_user and check_user.user_login == login_form \
                    and check_user.user_password == hashed_input:
                session['login'] = login_form
                return redirect(url_for('default_view.init_index'))
            else:
                flash('Password is not correct. Please try again.', 'error')
                return redirect(url_for('default_view.init_login'))


@default_view.route('/logout')
def handle_logout():
    session.clear()
    flash('Successfully logged out', 'success')
    return redirect(url_for('default_view.init_index'))

#
# @home_view.route('/candidates/create/done', methods=['GET', 'POST'])
# def create_candidate_account():
#     chosen_login = request.form['login']
#     login_exists_in_users = User.query.filter_by(login=chosen_login).first()
#     login_exists_in_candidates = Candidate.query.filter_by(login=chosen_login).first()
#     if (not login_exists_in_candidates) and (not login_exists_in_users):
#         hashed_pw = hashing.hash_value(request.form['password'], salt='secretsalt')
#         candidate = Candidate(request.form['name'],
#                               request.form['surname'],
#                               request.form['tech'],
#                               request.form['exp'],
#                               request.form['login'],
#                               hashed_pw,
#                               request.form['email'])
#         db.session.add(candidate)
#         db.session.commit()
#     else:
#         flash("Chosen login already exists in database. Please choose another login.", "error")
#         return redirect(url_for('home_view.init_candidate_account_form'))
#
#     flash("Candidate account created successfully.", "success")
#     return redirect(url_for('home_view.init_index'))
#
#
# @home_view.route('/user/manage/data/done', methods=['GET', 'POST'])
# def edit_personal_data():
#     active_user = User.query.filter_by(login=session['login']).first()
#     new_login_exists = User.query.filter_by(login=request.form['login']).first()
#     if (not new_login_exists) or (request.form['login'] == active_user.login):
#         active_user.name = request.form['name']
#         active_user.surname = request.form['surname']
#         active_user.email = request.form['email']
#         active_user.login = request.form['login']
#         session['login'] = active_user.login
#         db.session.commit()
#     else:
#         flash("Chosen login already exists in database. Please choose another login.", "error")
#         return redirect(url_for('home_view.init_edit_user_data'))
#
#     flash("User data updated successfully.", "success")
#     return redirect(url_for('home_view.init_index'))
#
#
# @home_view.route('/user/manage/password/done', methods=['GET', 'POST'])
# def edit_personal_password():
#     active_user = User.query.filter_by(login=session['login']).first()
#     old_hashed_pw = hashing.hash_value(request.form['old_password'], salt='secretsalt')
#     if active_user.password == old_hashed_pw:
#         new_hashed_pw = hashing.hash_value(request.form['new_password'], salt='secretsalt')
#         active_user.password = new_hashed_pw
#         db.session.commit()
#     else:
#         flash("Old password is not correct. Please try again.", "error")
#         return redirect(url_for('home_view.init_edit_user_password'))
#
#     flash("Password updated successfully.", "success")
#     return redirect(url_for('home_view.init_index'))
#
#
# @home_view.route('/tasks/manage', methods=['GET', 'POST'])
# def manage_tasks():
#     req = request.get_json()
#     active_user = User.query.filter_by(login=session['login']).first()
#     active_group = Group.query.filter_by(group_id=session['active_group']).first()
#     if req is not None and 'type' in req:
#         free_tasks = Task.query.filter_by(taken_by=None, group_id=active_group.group_id).all()
#         if req['type'] == 'fetch_free_tasks':
#             return send_json_response(free_tasks)
#
#         if req['type'] == 'fetch_taken_tasks':
#             taken_tasks = Task.query.filter_by(task_status='taken', group_id=active_group.group_id).all()
#             data = []
#             for task in taken_tasks:
#                 this_task_user = User.query.filter_by(login=task.taken_by).first()
#                 data.append(task.to_json())
#                 data.append(this_task_user.user_type_color_to_json())
#             return Response(json.dumps(data, sort_keys=True, indent=4), mimetype='application/json')
#
#         if req['type'] == 'take_task':
#             task_to_be_taken = Task.query.filter_by(task_id=req['current_id']).first()
#             task_to_be_taken.taken_by = active_user.login
#             task_to_be_taken.task_status = 'taken'
#             db.session.commit()
#
#     return render_template('user/manageTasks.html', active_group=active_group)
#
#
# @home_view.route('/tasks/usersTasks', methods=['GET', 'POST'])
# def manage_users_tasks():
#     active_group = Group.query.filter_by(group_id=session['active_group']).first()
#     req = request.get_json()
#     if req is not None and 'type' in req:
#         if req['type'] == 'fetch_users_tasks':
#             my_tasks = Task.query.filter_by(taken_by=session['login'], task_status='taken').all()
#             return send_json_response(my_tasks)
#
#         if req['type'] == 'fetch_tasks_to_accept':
#             tasks_to_accept = Task.query.filter_by(task_status='finished', group_id=active_group.group_id).all()
#             return send_json_response(tasks_to_accept)
#
#         if req['type'] == 'mark_as_finished':
#             task_to_be_marked = Task.query.filter_by(task_id=req['current_id']).first()
#             if session['user_type'] == 'trainee':
#                 task_to_be_marked.task_status = 'finished'
#             else:
#                 task_to_be_marked.task_status = 'archived'
#             db.session.commit()
#
#         if req['type'] == 'resign':
#             task_to_resign = Task.query.filter_by(task_id=req['current_id']).first()
#             task_to_resign.taken_by = None
#             task_to_resign.task_status = 'free'
#             db.session.commit()
#
#         if req['type'] == 'archive':
#             task_to_be_archived = Task.query.filter_by(task_id=req['current_id']).first()
#             task_to_be_archived.task_status = 'archived'
#             db.session.commit()
#
#     return render_template('user/usersTasks.html', active_group=active_group)
#
#
# @home_view.route('/tasks/create', methods=['GET', 'POST'])
# def create_task():
#     active_user = User.query.filter_by(login=session['login']).first()
#     task = Task(active_user.group_id,
#                 request.form['content'],
#                 request.form['priority'],
#                 'free')
#     db.session.add(task)
#     db.session.commit()
#     flash("Task created successfully.", "success")
#     return redirect(url_for('home_view.manage_tasks'))
#
#
# @home_view.route('/tasks/archives', methods=['GET', 'POST'])
# def view_tasks_archives():
#     active_group = Group.query.filter_by(group_id=session['active_group']).first()
#     req = request.get_json()
#     if req is not None and 'type' in req:
#         if req['type'] == 'fetch_archives':
#             archives = Task.query.filter_by(group_id=session['active_group'], task_status='archived').all()
#             return send_json_response(archives)
#     return render_template('user/tasksArchives.html', active_group=active_group)
