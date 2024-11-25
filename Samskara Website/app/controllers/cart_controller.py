from flask import Blueprint, render_template, request,redirect, url_for,flash
from app.models import  CartItem, Cart
from app import db
from app.utils import get_products_in_cart,calculate_cart_totals

cart = Blueprint('cart', __name__)

@cart.route('/cart')
def view_cart():
    cart_id=request.cookies.get("cart_id")
    cart = Cart.query.filter_by(id=cart_id).first()
    totals=0
    products_in_cart=[]

    if cart:
        products_in_cart=get_products_in_cart(cart)
        totals=calculate_cart_totals(cart)

    return render_template('cart.html',products_in_cart=products_in_cart, totals=totals,title="Cart")

@cart.route('/cart/delete_item/<int:item_id>', methods=['POST'])
def delete_item(item_id):
    if request.method == 'POST':
        cart_item = CartItem.query.get(item_id)
        if cart_item:
            db.session.delete(cart_item)
            db.session.commit()
            flash('Item removed from the cart', 'success')
        else:
            flash('Item not found in the cart, or already deleted', 'danger')

    return redirect(url_for('cart.view_cart'))

