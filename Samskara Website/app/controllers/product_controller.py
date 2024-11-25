from flask import Blueprint, render_template, request, make_response,flash
from app.models import Product, CartItem, Cart, Option,Category
from app.forms import ProductForm
from app import db
from datetime import datetime


products = Blueprint('products', __name__)

@products.route('/product/<id>', methods=['GET','POST'])
def view_product(id):
    cart_id = request.cookies.get('cart_id')
    product=Product.query.filter_by(id=id).first()

    if not product:
        return "Product not found", 404
    option_dict={}
    # Create the form  fields for options
    for option in product.options:
        option_group_name = option.group.name
        option_name = option.name

        if option_group_name not in option_dict:
            option_dict[option_group_name] = [option_name]
        else:
            option_dict[option_group_name].append(option_name)

    form = ProductForm(options=option_dict)
    form.product_id.data = str(id)
    cart_id=request.cookies.get('cart_id')

    # Iterate over option_dict items to add a form field for each option_group
    i = 0
    for key, value_array in option_dict.items():
    # Form option is an array, so access the sub-form at the current index
        sub_form = form.options[i]
        sub_form.option.label = key
        sub_form.option.id = key
        sub_form.option.choices = []
        for value in value_array:
            sub_form.option.choices.append((value, value))
        i += 1

    if form.validate_on_submit():
        quantity = form.quantity.data
        selected_options = [entry.option.data for entry in form.options.entries]
        product_id=form.product_id.data

        cart=Cart.query.filter_by(id=cart_id).first()
        if not cart:
            date=datetime.now()
            cart = Cart(creation_date=date)
            db.session.add(cart)
            db.session.commit()
            cart_id=cart.id

        # Check if the product with the same options is already in the cart
        existing_cart_items = CartItem.query.filter_by(cart_id=cart.id, product_id=product_id).all()

        if existing_cart_items:
            for existing_cart_item in existing_cart_items:
            # For each existing product, check if the selected options match the existing product's options
                existing_options = [option.name for option in existing_cart_item.options]
                same_options=set(selected_options) == set(existing_options)
                if same_options:
                # Product with the same options is already in the cart, increase quantity and stop looping
                    existing_cart_item.quantity += int(quantity)
                    break
        # Create a new cart item if there is no existing same cart item or the ones existing does not have same_options
        if not existing_cart_items or not same_options :
            cart_item = CartItem(cart_id=cart.id, product_id=product_id, quantity=quantity)
            options = []
            for option_name in selected_options:
                option = Option.query.filter_by(name=option_name).first()
                if option:
                    options.append(option)
            cart_item.options.extend(options)
            db.session.add(cart_item)


        db.session.commit()
        flash('Added {} X {} to cart'.format(
            product.name, form.quantity.data),"success")
        response = make_response(render_template('product.html',product=product,form=form,atc=True))
        response.set_cookie('cart_id', str(cart_id))
        return response

    return render_template('product.html',product=product,form=form,added_to_cart=False)






@products.route('/<gender>/', defaults={'category': None})
@products.route('/<gender>/<category>')
def shop(gender, category=None):
    valid_genders = ["man", "woman"]

    # Check if the provided gender is valid
    if gender not in valid_genders:
        return "Invalid gender", 400

    # Filter categories excluding "man" and "woman"
    clothes_categories = Category.query.filter(~Category.name.in_(valid_genders)).all()

    # Create the base query
    query = Product.query.filter(Product.categories.any(Category.name == gender))

    # Apply category filter if selected
    if category:
        query = query.filter(Product.categories.any(Category.name== category))

    products = query.all()

    return render_template('shop.html', products=products, gender=gender,category=category, clothes_categories=clothes_categories)