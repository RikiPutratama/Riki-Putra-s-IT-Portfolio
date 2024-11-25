from flask_wtf import FlaskForm,Form
from wtforms import StringField, EmailField, SubmitField, HiddenField
from wtforms.validators import DataRequired, Length, ValidationError,Email
from wtforms.fields import FormField, FieldList,HiddenField

from wtforms import  HiddenField, SelectField


# Form argument is required to run custom validators
def validate_numeric(form, field):
    if not field.data.isdigit():
        raise ValidationError('Field must contain only numeric characters.')

def validate_alpha_string(form, field):
    if not all(char.isalpha() or char.isspace() for char in field.data):
        raise ValidationError('Field must contain only alphabetic characters.')


class OptionEntryForm(Form):
    option = SelectField('Produce',
                            [DataRequired()],
                            choices=[])


class ProductForm(FlaskForm):
    product_id = HiddenField('ID', validators=[DataRequired()])
    quantity = SelectField('Quantity', choices=[(str(i), str(i)) for i in range(1, 10)], validators=[DataRequired()])
    options = FieldList(FormField(OptionEntryForm), min_entries=1)


class AddressForm(Form):
    house_number= StringField('House Number', validators=[DataRequired()])
    street = StringField('Street Name', validators=[DataRequired()])
    city = StringField('City', validators=[DataRequired(),validate_alpha_string])
    state = StringField('State', validators=[DataRequired(),validate_alpha_string])
    zip_code = StringField('Zip Code', [DataRequired(), validate_numeric])
    country = StringField('Country', [DataRequired(),validate_alpha_string ])


class OrderForm(FlaskForm):
    cart_id = HiddenField('ID', validators=[DataRequired()])
    name = StringField('Name', validators=[DataRequired(), Length(max=64),validate_alpha_string])
    email = EmailField('Email', validators=[DataRequired(),Email(message="Invalid email",check_deliverability=True)])
    address = FormField(AddressForm)
    submit = SubmitField('Validate')
