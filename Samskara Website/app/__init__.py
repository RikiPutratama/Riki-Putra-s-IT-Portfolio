
from flask import Flask,session,request
from config import Config
from flask_sqlalchemy import SQLAlchemy
from flask_bootstrap import Bootstrap4
#initiating extensions
db = SQLAlchemy()
bootstrap = Bootstrap4()

# Creating app with factory pattern
def create_app():

    app = Flask(__name__)
    app.config.from_object(Config)

    bootstrap.init_app(app)
    db.init_app(app)

    #Import the blueprints once the app is declared to avoid circular impors
    from .controllers import products,cart,checkout, main,products
    app.register_blueprint(products)
    app.register_blueprint(cart)
    app.register_blueprint(checkout)
    app.register_blueprint(main)

    return app