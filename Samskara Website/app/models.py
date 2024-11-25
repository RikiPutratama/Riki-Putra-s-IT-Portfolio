from . import db


product_category = db.Table(
    'product_category',
    db.Column('product_id', db.Integer, db.ForeignKey('product.id')),
    db.Column('category_id', db.Integer, db.ForeignKey('category.id'))
)
product_option= db.Table(
    'product_option',
    db.Column('product_id', db.Integer, db.ForeignKey('product.id')),
    db.Column('option_id', db.Integer, db.ForeignKey('option.id')),
)
cart_product = db.Table(
    'cart_product',
    db.Column('cart_id', db.Integer, db.ForeignKey('cart.id'), primary_key=True),
    db.Column('product_id', db.Integer, db.ForeignKey('product.id'), primary_key=True)
)
cart_item_option = db.Table(
    'cart_item_option',
    db.Column('cart_item_id', db.Integer, db.ForeignKey('cart_item.id'), primary_key=True),
    db.Column('product_option_id', db.Integer, db.ForeignKey('option.id'), primary_key=True)
)
order_details_option = db.Table(
    'order_details_option',
    db.Column('order_item_id', db.Integer, db.ForeignKey('order_details.id'), primary_key=True),
    db.Column('product_option_id', db.Integer, db.ForeignKey('option.id'), primary_key=True)
)

class Product(db.Model):
    id=db.Column(db.Integer, primary_key=True,unique=True,nullable=False)
    name=db.Column(db.String(64), index=True, nullable=False)
    image=db.Column(db.String(255))
    price=db.Column(db.Numeric(precision=8, scale=2))
    description=db.Column(db.String(255))
    discount_price=db.Column(db.Float)
    categories = db.relationship('Category', secondary=product_category, backref='product', lazy='dynamic')
    options=db.relationship('Option',secondary=product_option, backref='option',lazy='dynamic')
    def __init__(self, *args, **kwargs):
        # Call validation method to ensure the discount price is valid.
        self.validate_discount()
        # Call the constructor of the parent class if discount is valid.
        super(Product, self).__init__(*args, **kwargs)

    def validate_discount(self):
        if self.discount_price is not None and self.discount_price >= self.price:
            raise ValueError("Discount price must be less than the regular price.")

    def __repr__(self):
        return '<Product: {}>'.format(self.name)

class Category(db.Model):
    id = db.Column(db.Integer, primary_key=True, unique=True, nullable=False)
    name = db.Column(db.String(64), index=True, unique=True, nullable=False)


    def __repr__(self):
        return '<Category: {}>'.format(self.name)

class Option(db.Model):
    id=db.Column(db.Integer, primary_key=True,unique=True,nullable=False)
    name=db.Column(db.String(64),nullable=False)
    option_group = db.Column(db.Integer,db.ForeignKey('option_group.id') )

class OptionGroup(db.Model):
    id=db.Column(db.Integer, primary_key=True,unique=True,nullable=False)
    name=db.Column(db.String(64),nullable=False)
    options = db.relationship('Option', backref='group' )


class Order(db.Model):
    id=db.Column(db.Integer, primary_key=True,unique=True,nullable=False)
    customer_name=db.Column(db.String(64), index=True)
    customer_email=db.Column(db.String(128),nullable=False)
    date=db.Column(db.Date, nullable=False)
    total_amount=db.Column(db.Float)
    status=db.Column(db.String(64))
    order_details = db.relationship('OrderDetails', backref='order', lazy=True)
    shipping_address_id = db.Column(db.Integer, db.ForeignKey('address.id') ,nullable=False)
    def __repr__(self):
        return '<Order n°: {} by {}>'.format(self.id,self.customer_name)

class OrderDetails(db.Model):
    id=db.Column(db.Integer, primary_key=True,unique=True,nullable=False)

    order_id = db.Column(db.Integer, db.ForeignKey('order.id'))
    product_id = db.Column(db.Integer, db.ForeignKey('product.id'))
    product = db.relationship('Product', foreign_keys=[product_id], backref='order_details')
    quantity = db.Column(db.Integer, nullable=False)
    #Store the price of the product that the user paid in, case it changes in the future
    product_price = db.Column(db.Numeric(precision=8, scale=2))

    options = db.relationship('Option', secondary=order_details_option, backref='order_details')

    def __repr__(self):
        return '<Order n°: {} Product ID: {}>'.format(self.order_id, self.product_id)

class OrderStatus(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String(50), unique=True, nullable=False)

    def __repr__(self):
        return self.name

class Cart(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    creation_date=db.Column(db.Date, nullable=False)
    # Define a one-to-many relationship between Cart and CartItem
    items = db.relationship('CartItem', backref='cart', lazy='dynamic')

class CartItem(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    cart_id = db.Column(db.Integer, db.ForeignKey('cart.id'))
    product_id = db.Column(db.Integer, db.ForeignKey('product.id'))
    quantity = db.Column(db.Integer)

    # Define a many-to-one relationship between CartItem and Product
    product = db.relationship('Product', backref='cart_items')
    # Define a one-to-many relationship between CartItem and Option
    options = db.relationship('Option', secondary=cart_item_option, backref='cart_items')

class Address(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    house_number= db.Column(db.String(16), nullable=False)
    street = db.Column(db.String(255), nullable=False)
    city = db.Column(db.String(128), nullable=False)
    state = db.Column(db.String(128), nullable=False)
    zip_code = db.Column(db.String(16), nullable=False)
    country = db.Column(db.String(128), nullable=False)
    # Define a one-to-many relationship with Order for shipping addresses
    orders_shipping = db.relationship('Order', backref='shipping_address', foreign_keys='Order.shipping_address_id')


    def __repr__(self):
        return f'Address(street="{self.street}", city="{self.city}", state="{self.state}", postal_code="{self.postal_code}")'
