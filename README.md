# Uppgift 7 - _Programmering med C# ASP.NET 1, Nackademin_

### Assignment
> "Build a pizza ordering website using ASP.NET Core, Entity Framework, and Identity. As a user you shall be able to log in and order dishes and gain points for discounts, as well as see previous orders (and their delivery status). As a premium user you shall receive additional discounts. As an admin you shall be able to add new dishes (composed from a list of ingredients), as well as administrate users and orders from all users."

---

# TOMASOS !! :pizza:

### Home page
![Home page, shows all blog posts](/devlog/home_page.png)

> Website is styled via a heavy combination between Bootstrap 3 and custom CSS. On the home page you can see all available dishes, or choose a category such as Pasta to filter out the rest.

### Register user
![Register view, create new user](/devlog/new_user.png)

> Users are free to register, and will be automatically referred to as "Regular User". An admin must manually promote a user to Premium user (which can be done via the Admin dashboard for Admin users) for the discounts as mentioned in the assignment. The role of admin user is however only gained through manual SQL queries.
>
> The register form contains client-side validation.

### Add to card
![Home page, add dish to cart](/devlog/ajax_cart.png)

> Adding to cart uses AJAX together with a popover to add without reloading the page. The cart is saved in session data (server-side) and will therefore not remain in a new web session.

### Order cart
![Cart view, order cart](/devlog/cart.png)

> Having items in your cart, you can then order them. As this is a school assignment, this is only fictional. The order is only saved in the database. No payment is done.
>
> You can add dishes to your cart anonymously, but you must have a registered account to be able to order your cart.

### Order receipt
![Receipt view](/devlog/receipt.png)

> After an order you are presented with the receipt. All your receipts can be accessed via a tab in your profile.
>
> All orders has the state of being delivered or not. An order can be marked as "delivered" only by an admin user (via their dashboard). This is to simulate an automatic delivery system which is not in place in this solution.

### Discounts
![Receipt view from premium user](/devlog/receipt_bonus.png)

> When ordering you gain points. 1 dish gives 10 points. Upon receiving 100 points you get one dish for free (cheapest dish in your cart).
>
> As a premium user you are granted a discount of 20% when ordering 3 or more dishes at the same time.

### Administrate ingredients
![List of ingredients, add or remove](/devlog/ingredients.png)

> As part of the admin dashboard you can add and remove ingredients. You cannot, however, remove ingredients that are used in existing dishes. The ingredients list is used when adding a new dish.

### Create/alter dish
![Edit dish page](/devlog/new_meal.png)

> New dishes can be added from the admin dashboard. A dish object contains name, category, description, price, and items from the ingredients.

### Database diagram
![Database diagram from sqldbm.com](/devlog/database_diagram.png)
