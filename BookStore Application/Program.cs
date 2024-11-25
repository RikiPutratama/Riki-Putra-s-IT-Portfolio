using System;
namespace BookStore
{
    class Book
    {   //Two Puboic static arrays for category codes and descriptions
        public static string[] categoryCodes = { "CS", "IS", "SE", "SO", "MI" };
        public static string[] categoryNames = { "Computer Science", "Information System", "Security", "Society", "Miscellaneous" };

        //Data Fields
        private string bookId;
        private string categoryNameOfBook;

        //Auto-implemented properties
        public string BookTitle { get; set; }
        public int NumOfPages { get; set; }
        public double Price { get; set; }


        //Constructor with no parameter
        public Book()
        {

        }

        //Constructor with parameters for all data fields
        public Book(string bookId, string bookTitle, int numPages, double price)
        {
            BookId = bookId;
            BookTitle = bookTitle;
            Price = price;
            NumOfPages = numPages;
        }

        //Properties for book id and book category name
        public string BookId
        {
            get { return bookId; }
            set
            {
                //Check if the book id is in a valid information
                if (IsValid(value))
                {
                    bookId = value.ToUpper();
                    //Assign category name based on the book id
                    if (Array.IndexOf(categoryCodes, value.Substring(0, 2)) != 1)
                    {
                        categoryNameOfBook = categoryNames[Array.IndexOf(categoryCodes, value.Substring(0, 2))];
                    }
                    else
                    {
                        categoryNameOfBook = categoryNames[4];//"MI category"
                    }
                }
                else
                {
                    //Prompt the user to re-enter a valid book id
                    Console.Write("Invalid book id. Please enter a valid book id: ");
                    BookId = Console.ReadLine();
                }
            }
        }

        //ToString method to return information of a book object
        public override string ToString()
        {
            //return $"Book ID: {BookId}\nCategory: //{categoryNameOfBook}\nTitle:  {BookTitle}\nPages: {NumOfPages}\nPrice: ${Price}\n";
            return $"{BookId}   {categoryNameOfBook} {BookTitle}   {NumOfPages}   {Price.ToString("C")}\n";
        }

        //Method to check if a book id is valid
        public static bool IsValid(string id)
        {
            if (id.Length == 5 && char.IsUpper(id[0]) && char.IsUpper(id[1]) && char.IsDigit(id[3]) && char.IsDigit(id[4]) && char.IsDigit(id[2]))

            {
                return true;
            }
            return false;
        }


    }

    class Program
    {
        //Method to input an integer number within a range
        public static int InputValue(int min, int max)
        {
            int value;
            while (true)
            {
                Console.Write("Please enter a number which is in the range of 1 and 30: ");
                if (int.TryParse(Console.ReadLine(), out value) && value >= min && value <= max)

                {
                    return value;
                }
                Console.WriteLine("Invalid input. Please enter valid number.");
            }
        }


        //Method to fill an array of book with user Input
        private static void GetBookData(int num, Book[] books)
        {
            for (int i = 0; i < num; i++)
            {
                Console.WriteLine($"\n\nBook {i + 1}: ");
                Console.Write("\nEnter Book name: ");
                string bookTitle = Console.ReadLine();
                //display category of books with its name
                Console.WriteLine("\nCategory codes are:\n");
                for (int a = 0; a < Book.categoryCodes.Length; a++)
                {
                    Console.WriteLine(Book.categoryCodes[a] + "  " + Book.categoryNames[a]);
                }
                Console.WriteLine("\nEnter book id which starts with a category code (All Capital) and ends with a 3-digit number (i.e: CS145): ");
                string bookId = Console.ReadLine();

                Console.Write("Enter book price: ");
                double price = double.Parse(Console.ReadLine());

                Console.Write("Enter number of pages: ");
                int numPages = int.Parse(Console.ReadLine());

                books[i] = new Book(bookId, bookTitle, numPages, price);

            }

        }

        //Method to display information of all books
        public static void DisplayAllBooks(Book[] books)
        {
            Console.WriteLine("\n\nInformation for all books");
            Console.WriteLine("-----------------------------------------------------------------");

            foreach (Book book in books)
            {
                Console.WriteLine(book);
            }
        }

        //Method to display book lists based on category code
        private static void GetBookLists(int num, Book[] books)
        {

        }

        //Main Method
        static void Main()
        {
            //Introduction
            Console.WriteLine("Welcome to BookStore application! \n\n");
            
            //Prompt the user for the number of books
            int numBooks = InputValue(1, 30);

            //Create an array of books
            Book[] books = new Book[numBooks];

            //Get book data from the user
            GetBookData(numBooks, books);

            //Display information of all books
            DisplayAllBooks(books);

            //Get book lists based on category code
            GetBookLists(numBooks, books);
        }

    }
}