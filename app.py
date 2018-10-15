from flask import (
    Flask,
    request
)

app = Flask(__name__, template_folder="templates")


@app.route('/', methods=['GET', 'POST'])
def hello():
    if request.method == 'POST':
        name = request.form.get('name')
        return '''<h1>Hello, {}</h1>'''.format(name)

    return '''<h1>Hello, World!</h1>
                <form method="POST">
                 Name: <input type="text" name="name"><br>
                 <input type="submit" value="Submit"><br>
                </form>'''


if __name__ == '__main__':
    app.run()
