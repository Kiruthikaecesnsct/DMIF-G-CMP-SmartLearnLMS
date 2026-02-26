//string userName;
//int age;
//Console.WriteLine("Enter your name:");
//userName = Console.ReadLine();
//Console.WriteLine("Hello, " + userName);


//Arithmetic Operators
//int a ;
//int b;

//Console.WriteLine("Enter the value of first number");
//a=int.Parse(Console.ReadLine());

//Console.WriteLine("Enter the value of Second number");
//b = int.Parse(Console.ReadLine());
//Console.WriteLine($"{a} + {b} = {a + b}");    // 22
//Console.WriteLine($"{a} - {b} = {a - b}");    // 12
//Console.WriteLine($"{a} * {b} = {a * b}");    // 85
//Console.WriteLine($"{a} / {b} = {a / b}");    // 3   (integer division)
//Console.WriteLine($"{a} % {b} = {a % b}");    // 2   (remainder)


////Comparison Operators & Logical operators
//int age = 17;
//bool hasId = true;

//Console.WriteLine(age >= 18);                // false
//Console.WriteLine(age == 18);                // false
//Console.WriteLine(age != 20);                // true

//// Logical operators
//Console.WriteLine(age >= 13 && age <= 19);   // true  (teenager)
//Console.WriteLine(age < 13 || hasId);        // true
//Console.WriteLine(!hasId);                   // false

////Even & Odd check

//int number = 42;

//if (number % 2 == 0)
//{
//    Console.WriteLine($"{number} is EVEN");
//}
//else
//{
//    Console.WriteLine($"{number} is ODD");
//}
//string type = (number % 2 == 0) ? "even" : "odd";
//Console.WriteLine(type);


////Grade evaluation with switchcase
//Console.Write("Enter grade letter (A/B/C/D/F): ");
//string grade = Console.ReadLine().ToUpper();

//switch (grade)
//{
//    case "A":
//        Console.WriteLine("Excellent! Keep it up.");
//        break;
//    case "B":
//        Console.WriteLine("Good work.");
//        break;
//    case "C":
//        Console.WriteLine("Satisfactory.");
//        break;
//    case "D":
//        Console.WriteLine("Pass – needs improvement.");
//        break;
//    case "F":
//        Console.WriteLine("Failed. Try harder next time.");
//        break;
//    default:
//        Console.WriteLine("Invalid grade entered.");
//        break;
//}



