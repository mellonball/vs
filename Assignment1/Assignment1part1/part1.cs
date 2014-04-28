using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace Assigment1part1
{
    class part1
    {
        static void Main(string[] args)
        {
            Queue myQ = new Queue(10);

            int size = 10;
            Thread[] pros = new Thread[size];
            Thread[] cons = new Thread[size];

            // create and start size number of Producer and Consumer threads
            for (int i = 0; i < size; i++) 
            {
                pros[i] = new Thread(new ThreadStart((new Producer(myQ, i)).Run));
                pros[i].Start();
                cons[i] = new Thread(new ThreadStart((new Consumer(myQ)).Run));
                cons[i].Start();
            }

            Console.ReadLine();

        }
    }


    //in each Producer thread, within an infinite loop, add an Integer obj to Queue, wait 10 ms, then repeat
    class Producer
    {
        private Queue myQ;
        private int myR;

        public Producer(Queue q, int r)
        {
            this.myQ = q;
            this.myR = r;
        }

        public void Run()
        {
            while (true)
            {
                this.myQ.Enqueue(this.myR);
                Thread.Sleep(10);
            }
        }
    }


    class Consumer
    {
        private Queue myQ;

        public Consumer(Queue q)
        {
            this.myQ = q;
        }

        public void Run()
        {
            while (true)
            {
                this.myQ.Dequeue();
                Thread.Sleep(10);
            }
        }
    }


    class Queue
    {
        private int[] queue;
        private int count;
        private int head;
        private int tail;
        private int size;

        public Queue(int size)
        {
            this.queue = new int[size];
            this.size = size;
            this.count = 0;
            this.head = 0; //index of next place to add an item
            this.tail = 0; //index of next item to remove
        }

        public void Enqueue(int item) //producer enqueues
        {
            lock (this.queue)
            {

                while (this.count >= 10)
                {
                    //if FULL release lock on queue until it gets it back...
                    Monitor.Wait(this.queue);
                }

                Console.WriteLine("Calling Enqueue on {0}", item);
                this.queue[head] = item;
                this.count++;
                this.head = (this.head + 1) % this.size;

                PrintQueue();
                Monitor.Pulse(this.queue);

            }
        }

        public void Dequeue()// consumer dequeues
        {
            lock (this.queue)
            {
                while (this.count <= 0)
                {
                    //if EMPTY wait 
                    Monitor.Wait(this.queue);
                }

                //does not delete integers from list, simply increments tail pointer (% size) to dequeue
                Console.WriteLine("Calling Dequeue...");
                this.tail = (this.tail + 1) % this.size;
                this.count--;


                PrintQueue();
                Monitor.Pulse(this.queue);
            }
        }

        public void PrintQueue()
        {
            int tailptr = this.tail;
            Console.Write("Current List {");
            for (int i = 0; i < this.count; i++)
            {
                Console.Write("{0} ", this.queue[tailptr]);
                tailptr = (tailptr + 1) % this.size;
            }
            Console.WriteLine("}");
            Console.WriteLine();


        }

    }
}
