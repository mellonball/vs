using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Reflection;
using System.Threading;

namespace Assignment1part2
{
    public class part2
    {
        private static void Main(string[] args)
        {
            // Person objects to query upon, first arg = Person "Age", second arg = Person "Name" 
            Person a = new Person(1911, "John");
            Person b = new Person(20132, "Jim");
            Person c = new Person(10010, "Ben");
            Person d = new Person(30132, "Luke");
            Person e = new Person(1923, "Luke");
            Person f = new Person(1942, "Jill");
            Hippo g = new Hippo(1911, "Harry", "Washington");

            // List is the IEnumerable that we will pass to Querier
            List<object> t = new List<object>();
            t.Add(a);
            t.Add(b);
            t.Add(c);
            t.Add(d);
            t.Add(e);
            t.Add(f);
            t.Add(g);

            Querier queryObject = new Querier(t);

            //tests
            Console.WriteLine("1.) CountByProperty(Age, 1911): SHOULD BE 2: ");
            queryObject.CountByProperty("Age", "1911");

            Console.WriteLine("2.) SumByProperty(FirstName, Luke, FirstName, Luke): SHOULD BE 4: ");
            queryObject.sumByProperty("FirstName", "Luke", "FirstName", "Luke");

            Console.WriteLine("3.) GroupByProperty(FirstName): 1, descending order:  ");
            queryObject.GroupByProperty("FirstName");

            Console.WriteLine("4.) GroupByProperty(Age): Valid Property, should return 1 : ");
            queryObject.GroupByProperty("Age");

            Console.WriteLine("5.) GroupByProperty(BANANANA): Invalid Property, should return -1 : ");
            queryObject.GroupByProperty("BANANANA");

            queryObject.Transform("6.) WhoAmI");
            Console.ReadLine();
        }
    }

    public class Hippo
    {
        public int Age { get; set; }    //these are the properties:: Age, FirstNamem Zoo, You can use Reflection to examine these using PropertyInfo proprty = (Type Person/Hippo).GetProperty(Zoo); <-returns you the property in type, or null if that type does not have Zoo property! Type A = obj.GetType()
        public string FirstName { get; set; }
        public string Zoo { get; set; }

        public Hippo(int age, string fname, string zoo)
        {
            this.Age = age;
            this.FirstName = fname;
            this.Zoo = zoo;
        }
    }

    /*
     * Person class
     * Represents a Person with two properties ( Age, and First Name) 
     * Has various methods to perform actions with those data members. Meant primarily as testing for Querier class
     */
    public class Person
    {
        public int Age { get; set; }
        public string FirstName { get; set; }

        // Overloaded Constructor, sets valid age and name to a person
        public Person(int age, string name)
        {
            this.Age = age;
            this.FirstName = name;
        }

        // Default constructor, sets dummy variables
        public Person()
        {
            this.Age = -1;
            this.FirstName = string.Empty;
        }


        /*
         * HowOldAmI
         * Prints a string depicting how old the person is
         */
        public void HowOldAmI()
        {
            Console.WriteLine("I am {0} years old", this.Age);
        }


        /*
         * WhoAmI
         * Simply prints a string depicting what the persons name is
         */
        public void WhoAmI()
        {
            Console.WriteLine("My name is {0}", this.FirstName);
        }
    }

    /*
     * Class Querier 
     * Constructor: Takes a IEnumerable<object> and stores it as a data member.
     * Methods perform different queries on the IEnumerable based on the properties in the dataset. 
     */
    public class Querier
    {
        // IEnumerable to perform queries on
        private IEnumerable<object> collection;

        public Querier(IEnumerable<object> e)
        {
            this.collection = e;
        }


        /*
         * Function: CountbyProperty
         * Arguments: Takes two strings
         * Searches through IEnumerable list to find objects with a property correlating to "propertyName" argument, and a value correlating to "propertyValue argument"
         */
        public int CountByProperty(string propertyName, string propertyValue)
        {
            // keeps count of property value matches 
            int count = 0;

            // Iterate through objects
            foreach (object iterate in collection)
            {
                // Get type of objects 
                Type A = iterate.GetType();

                //Find if iterate has the propertyName
                PropertyInfo proper = A.GetProperty(propertyName);

                if (proper != null)
                {
                    //check if the property of iterate has the value propertyValue
                    if (proper.GetValue(iterate).ToString().Equals(propertyValue))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        /*
         * sumByProperty
         * Inputs: 4 strings, two property names, with two corresponding property values to check for
         * Launches two threads, each uses CountByProperty to count the amount of elements that have a certain propertyName and property Value 
         * Returns: The sum of how many elements have propertyName1 with propertyValue1, and propertyName2 with propertyValue2
         */
        public int sumByProperty(string propertyName1, string propertyValue1, string propertyName2, string propertyValue2)
        {
            // Shared variables to get the results of the functions called by each thread
            int threadAResult = 0;
            int threadBResult = 0;

            // Call threads to count the number of elements with each propertyname and corresponding propertyvalue
            Thread threadA = new Thread(() => { threadAResult = CountByProperty(propertyName1, propertyValue1); });
            Thread threadB = new Thread(() => { threadBResult = CountByProperty(propertyName2, propertyValue2); });

            threadA.Start();
            threadB.Start();

            // Tell Main() to wait for threads
            threadA.Join();
            threadB.Join();

            return threadAResult + threadBResult;
        }


        /*
         * Function: GroupByProperty
         * Arguments: Takes one strings
         * Searches through IEnumerable list to find objects with a property correlating to "propertyName" argument, and prints out a table of how many elements have each value. 
         * This additionally returns 1 for a valid propertyName and 0 if there are no properties matching that name 
         */
        public int GroupByProperty(string propertyName)
        {
            // Keeps track of the values found in the property that were looking at
            SortedDictionary<object, int> dictionary = new SortedDictionary<object, int>();

            // keeps track of whether the property is valid or not
            bool foundProperty = false;

            // Iterate through objects
            foreach (object iterate in collection)
            {
                // Get type of objects 
                Type A = iterate.GetType();

                //check if iterate has the property propertyName
                PropertyInfo proper = A.GetProperty(propertyName);
                if (proper != null)
                {
                    foundProperty = true;

                    //saves the value of the property
                    var val = proper.GetValue(iterate);

                    // If the dictionary contains the value, increment the value for that key. 
                    if (dictionary.ContainsKey(val))
                    {
                        dictionary[val] = dictionary[val] + 1;
                    }
                    //If it's not in there, add a new instance to the dictionary and set the value to 1.   
                    else
                    {
                        dictionary.Add(val, 1);
                    }
                }


            }

            if (foundProperty)
            {
                // Print out the table 
                foreach (KeyValuePair<object, int> temp in dictionary.OrderByDescending(key => key.Key))
                {
                    Console.WriteLine(temp.Key.ToString() + " : " + temp.Value.ToString());
                }
            }
            else { Console.WriteLine("Property {0} not found.", propertyName); }
            Console.WriteLine();

            // If it was a valid property to search for return 1, else return -1. 
            return (foundProperty == true) ? 1 : -1;
        }

        /*
         * Function: Transform
         * Arguments: Takes a function name
         * Searches through IEnumerable list to find objects with a method with the name of the given argument. Applies that method to each item. If the method was not found
         * returns -1. If it was found, returns 1. 
         */
        public bool Transform(string functionName)
        {
            // stores whether the method exists 
            bool validMethod = false;

            // Iterate through objects
            foreach (object iterate in collection)
            {
                // Get type of objects 
                Type A = iterate.GetType();

                // Get list of object methods
                MethodInfo[] methods = A.GetMethods();

                // Iterate through methods of that object
                foreach (MethodInfo method in methods)
                {
                    // If the object were iterating through has the method name that were looking for, invoke it and remember that the method is valid. 
                    if (method.Name.Equals(functionName))
                    {
                        method.Invoke(iterate, null);
                        validMethod = true;
                    }
                }
            }

            return validMethod;
        }

    }


}
