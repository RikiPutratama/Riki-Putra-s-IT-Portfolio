def get_products_in_cart(cart):
    cart_items = cart.items

    if cart_items == []:
        return None

    cart_item_details = []

    for cart_item in cart_items:
            cart_item_detail = {
            "id": cart_item.id,
            "name": cart_item.product.name,
            "image": cart_item.product.image ,
            "price":cart_item.product.price ,
            "discount_price":cart_item.product.discount_price ,
            "quantity": cart_item.quantity,
            "options": []  # Initialize an empty list for options
            }

            # Access associated options for the cart item
            options = cart_item.options
            for option in options:
                option_detail = {
                    "id": option.id,
                    "name": option.name,
                    "group":option.group.name
                }
                cart_item_detail["options"].append(option_detail)
            # Add cart item detail to the list of cart items
            cart_item_details.append(cart_item_detail)
    return cart_item_details


def calculate_cart_totals(cart):
    cart_items = cart.items

    if cart_items == []:
        return None
    #initiate the totals
    total_before_discount=0
    total_discount=0
    total=0
    total_quantity=0

    for cart_item in cart_items:

        total_before_discount+=cart_item.product.price*cart_item.quantity

        if cart_item.product.discount_price:
            total_discount+=((cart_item.product.price-cart_item.product.discount_price)*cart_item.quantity)
            total+=(cart_item.product.discount_price)*cart_item.quantity

        else:
            total+=(cart_item.product.price)*cart_item.quantity

        total_quantity+=cart_item.quantity

    #Parse the totals as a dict
    totals_object={
        "before_discount":round(total_before_discount,2),
        "discount":round(total_discount,2),
        "subtotal":round(total,2),
        "quantity":round(total_quantity,2),
        }
    return totals_object
