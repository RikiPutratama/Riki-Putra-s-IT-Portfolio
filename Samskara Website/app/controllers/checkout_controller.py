from flask import Blueprint, render_template, request, flash
from app.models import  Cart, Order,OrderDetails,OrderStatus,Address
from app.forms import OrderForm
from app import db
from app.utils import calculate_cart_totals,get_products_in_cart
from datetime import datetime


checkout = Blueprint('checkout', __name__)


def get_cart_and_totals(cart_id):
    cart = Cart.query.filter_by(id=cart_id).first()
    totals = 0
    products_in_cart = []
    if cart:
        totals = calculate_cart_totals(cart)
        products_in_cart = get_products_in_cart(cart)
    return cart, totals, products_in_cart


@checkout.route('/checkout/',methods=['GET'])
def view_checkout():
    form=OrderForm()
    cart_id=request.cookies.get("cart_id")
    cart, totals, products_in_cart = get_cart_and_totals(cart_id)
    form.cart_id.data = cart_id
    if not cart_id or not cart or not products_in_cart :
        flash("Cart was not found or is empty" , 'error')
        return render_template('index.html')
    return render_template('checkout.html', title="Checkout" , form=form, products_in_cart=products_in_cart, totals=totals)

@checkout.route('/checkout/verify', methods=['POST'])
def verify_checkout():
        form=OrderForm()
        cart_id = form.cart_id.data
        cart, totals, products_in_cart = get_cart_and_totals(cart_id)

        if form.validate_on_submit():
            try:

                address_data = form.address.data

                found_address = Address.query.filter_by(
                                street=address_data["street"],
                                city=address_data["city"],
                                state=address_data["state"],
                                zip_code=address_data["zip_code"],
                                country=address_data["country"],
                                house_number=address_data["house_number"]
                                ).first()

                if found_address:
                    address=found_address
                # Address dont exist yeat, create a new one
                else: address=Address(street=address_data["street"],
                              city=address_data["city"],
                              state=address_data["state"],
                              zip_code=address_data["zip_code"],
                              country=address_data["country"],
                              house_number=address_data["house_number"])


                status=OrderStatus.query.filter_by(name="pending").first()
                order = Order(
                    customer_name=form.name.data,
                    date=datetime.now(),
                    total_amount=0,
                    status=status.name,
                    customer_email=form.email.data,
                    shipping_address=address,
                )

                order_details_list = []
                cart = Cart.query.filter_by(id=form.cart_id.data).first()
                total_amount=0
                for cart_item in cart.items:

                    total_amount += cart_item.product.price*cart_item.quantity

                    order_detail = OrderDetails(
                        product_id=cart_item.id,
                        quantity=cart_item.quantity,
                        product_price=cart_item.product.price
                    )
                    order_details_list.append(order_detail)

                order.total_amount = round(total_amount,2)

                # Need to commit the order to access its id in next loop
                db.session.add(order)
                db.session.commit()

                for order_detail in order_details_list:
                    order_detail.order_id = order.id  # Set the order_id to the newly created order's ID
                    db.session.add(order_detail)
                db.session.delete(cart)
                db.session.commit()


            except Exception as e:
            # Handle any exceptions or validation errors here
                flash(f"Error: {str(e)}", 'error')
                return render_template('index.html')
        else:
            flash('Form validation failed. Please check your inputs.', 'error')
            return render_template('checkout.html', title="Checkout", form=form,totals=totals,products_in_cart=products_in_cart)

        return render_template('checkout_success.html',order=order)


