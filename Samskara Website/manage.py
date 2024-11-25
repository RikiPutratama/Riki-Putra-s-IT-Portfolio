import sys
from app import create_app, db
from app.models import Category, OptionGroup, Option, Product, OrderStatus

app = create_app()
app.app_context().push()


def create_db():
    print("Creating the database...")
    db.create_all()
    print("Database created !")


def drop_db():
    print("Deleting all tables...")
    db.drop_all()
    print("All tables deleted")


def populate_db():
    print("Populating the database...")

    session = db.session

    # Change ths to add categories
    category_names = ["woman", "man", "pants", "jacket", "top"]

    # Create categories and add them to the session
    # We loop over each name in category, and create a Category() entrance.The print statement returns None, hence or statement.
    categories = [(print("Creating category :", name) or Category(name=name))
                  for name in category_names]
    session.add_all(categories)

    # Define order status
    order_status = ["pending", "paid", "processing", "shipped"]
    order_status = [(print("Creating category :", status)
                     or OrderStatus(name=status)) for status in order_status]

    session.add_all(order_status)
    # Define option names
    option_names = ["xs", "s", "m", "l", "xl", "xxl", "xxxl"]

    # Create an option group and options
    size_group = OptionGroup(name="size")
    options = [(print("Creating options :", name) or Option(name=name))
               for name in option_names]

    # Add options to the group and add the group to the session
    size_group.options.extend(options)
    session.add(size_group)

    # Define product data
    product_data = [
        {
            "name": "Adhyaksa",
            "image": "../static/images/Man/Jackets/1.png",
            "price": 50.00,
            "description": " Inspired by riders. This will guarantee your cool everytime",
            "discount_price": None,
        },
        {
            "name": "Aksara",
            "image": "../static/images/Man/Jackets/2.png",
            "price": 50.00,
            "description": "Giving you the chill vibe. Be the coolest in the room",
            "discount_price": None,
        },
        {
            "name": "AbdiChandra",
            "image": "../static/images/Man/Jackets/3.png",
            "price": 50.00,
            "description": "Giving you a stronger impression. Show them how cultured you are",
            "discount_price": None,
        },
        {
            "name": "Adhyaksa",
            "image": "../static/images/Man/Pants/1.png",
            "price": 40.00,
            "description": "A fusion of jeans and batik",
            "discount_price": None,
        },
        {
            "name": "Karma",
            "image": "../static/images/Man/Pants/2.png",
            "price": 40.00,
            "description": "To wear in relax environment. Giving you an instant chill vibe",
            "discount_price": None,
        },
        {
            "name": "Angkara",
            "image": "../static/images/Man/Pants/3.png",
            "price": 40.00,
            "description": "Show them how bold you are even in relax situation",
            "discount_price": None,
        },
        {
            "name": "Angkara",
            "image": "../static/images/Man/Tops/1.png",
            "price": 30.00,
            "description": "Express the fire in you. Show them how bold you are",
            "discount_price": None,
        },
        {
            "name": "AbdhiChandra",
            "image": "../static/images/Man/Tops/2.png",
            "price": 30.00,
            "description": "Show them how cultured you are while also giving the chill vibe",
            "discount_price": None,
        },

        {
            "name": "Aryatama",
            "image": "../static/images/Woman/Jackets/1.jpg",
            "price": 50.00,
            "description": "When the bold of jeans meet the feminine side of batik",
            "discount_price": None,
        },
        {
            "name": "Aksara",
            "image": "../static/images/Woman/Jackets/2.jpg",
            "price": 50.00,
            "description": "Be cool, chill and cultured with combination of jeans and batik",
            "discount_price": None,
        },
        {
            "name": "Angkara",
            "image": "../static/images/Woman/Jackets/3.jpg",
            "price": 50.00,
            "description": " Express the fire in you. Red batik blends perfectly with jeans",
            "discount_price": None,
        },
        {
            "name": "Angkara",
            "image": "../static/images/Woman/Pants/1.png",
            "price": 40.00,
            "description": "Be bold and express your fire even in relax situation",
            "discount_price": None,
        },
        {
            "name": "Aksara",
            "image": "../static/images/Woman/Pants/2.png",
            "price": 40.00,
            "description": "Express your passion of culture. Even when you're chilling",
            "discount_price": None,
        },
        {
            "name": "Adhyaksa",
            "image": "../static/images/Woman/Pants/3.png",
            "price": 40.00,
            "description": "Gently express your passion and collness with this cream batik pants",
            "discount_price": None,
        },
        {
            "name": "Aswara",
            "image": "../static/images/Woman/Tops/1.png",
            "price": 30.00,
            "description": "Blending fire and water, wth batik style",
            "discount_price": None,
        },
        {
            "name": "Adhyaksa",
            "image": "../static/images/Woman/Tops/2.png",
            "price": 30.00,
            "description": "Show your coolness with batik arms",
            "discount_price": None,
        },
        {
            "name": "Mahaswara",
            "image": "../static/images/Woman/Tops/3.png",
            "price": 30.00,
            "description": "A little strip of batik is enough to show how cool you are",
            "discount_price": None,
        },

    ]

    # Create products and add them to the session
    products = [(print('Adding product : ', data["name"])
                 or Product(**data)) for data in product_data]
    session.add_all(products)

    # Commit changes to the database
    session.commit()
    print("success")


def add_categories():
    print("Adding categories...")
    session = db.session
    # query for all products whose image path contains the woman folder
    products = Product.query.all()
    categories = Category.query.all()


    for product in products:
        for category in categories:
            if '/' + category.name + '/' in product.image.lower() or '/' + category.name + 's/' in product.image.lower():
                print("Adding", category.name, "to", product.name)
                product.categories.append(category)

    session.add_all(products)
    session.commit()
    print("Categories added !")


def add_options():
    print("Adding options...")
    session = db.session
    products = Product.query.all()
    options = Option.query.all()

    xs = []
    s = []
    m = []
    l = []
    xl = []
    xxl = []
    xxxl = []

    for product in products:
        for option in options:
            print("Adding", option.name, "to", product.name)
            product.options.append(option)

    session.add_all(products)
    session.commit()
    print("Options added !")


if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python manage.py <command>")
        print("Available commands: create_db, populate_db,drop_db,add_categories,add_options,refresh_db")
    else:
        command = sys.argv[1]
        if command == "create_db":
            create_db()

        elif command == "populate_db":
            populate_db()

        elif command == "drop_db":
            drop_db()

        elif command == "add_categories":
            add_categories()
        elif command == "add_options":
            add_options()
        elif command == "refresh_db":
            drop_db()
            print('-'*30)
            create_db()
            print('-'*30)
            populate_db()
            print('-'*30)
            add_categories()
            print('-'*30)
            add_options()
            print('-'*30)

        else:
            print("Invalid command. Use 'create_db' or 'populate_db'.")
