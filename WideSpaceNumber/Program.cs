using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace WideSpaceNumber
{
    public class Program
    {
        private static float MAX, MIN;
        private static int SIZE;
        private static long pTimer;
        private static float diff;
        public static long sTimer;
        public static string fileName = "A2-Input.txt";

        static void Main(string[] args)
        {   
            Program program = new Program();
            float[] inputArray = new float[0];

            // Read into array from file
            try
            {
                inputArray = Array.ConvertAll(File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, fileName)),
                    float.Parse);
            }
            catch (FileNotFoundException)
            {
            }

            Console.WriteLine("Size of Array: {0}", inputArray.Length);     // Print Size of Array (n)
            int k = program.partition(inputArray, inputArray.Length);       // Fetch value of k (partition index)
            Console.WriteLine("\nUsing Partition Method: {0}, Difference = {1}", k, diff);  // Print index k, and max separation difference

            // Call the Test Method (QuickSort & Difference)
            TestProgram testProgram = new TestProgram();
            testProgram.Test();

            // Display time taken for each method, in clock ticks
            Console.WriteLine("\nPartition Timer: " + pTimer);    // Measures clock ticks for Bucket way
            Console.WriteLine("Sort Timer: " + sTimer);           // Measures clock ticks for QuickSort way
        }

        // Core Partition module
        // Returns the index 'k' which acts as the parititioning index
        int partition(float[] a, int n)
        {
            float[] leftArray = new float[n];
            float[] rightArray = new float[n];

            int i;
            float difference = 0;
            List<float>[] bucketArray = new List<float>[n + 1];      // Array of Lists (Buckets), of size N+1

            // Intitialize List under each array index
            for (i = 0; i < n + 1; i++)
            {
                bucketArray[i] = new List<float>();
            }

            // Set SIZE
            SIZE = n - 1;

            // Start timer
            Stopwatch partititionTimer = new Stopwatch();
            partititionTimer.Start();

            // Find and store MIN and MAX
            MAX = a.Max();
            MIN = a.Min();

            // Add MIN and MAX elements to START and END position of the BucketArray
            bucketArray[0].Add(MIN);
            bucketArray[bucketArray.Length - 1].Add(MAX);

            // Place all elements (except MIN and MAX) from InputArray into proper position in the BucketArray,
            // based on Index calculated using GetIntervalIndex method
            foreach (var element in a)
            {
                if (!element.Equals(MAX) && !element.Equals(MIN))
                {
                    bucketArray[(int)Math.Floor(((element - MIN) * SIZE) / (MAX - MIN))].Add(element);
                }
            }

            for (i = 0; i < bucketArray.Length; i++)
            {
                // For buckets which are NOT empty, store local Max and local Min in each bucket, discard all other local elements
                if (bucketArray[i].Count > 0)
                {
                    float tempMin = bucketArray[i].Min();
                    float tempMax = bucketArray[i].Max();

                    bucketArray[i].Clear();
                    bucketArray[i].Add(tempMin);
                    bucketArray[i].Add(tempMax);
                }
            }

            i = 0;
            float tmpDiff;
            int k;
            int j;
            float nextMin;
            float prevMax;
            float left = 0.0f;

            // Traverse all the bucketArray, jumping between non-empty buckets
            for (j = i + 1; j < bucketArray.Length; j++)
            {
                if (bucketArray[j].Count > 0)
                {
                    nextMin = bucketArray[j].Min();
                    prevMax = bucketArray[i].Max();
                    tmpDiff = nextMin - prevMax;

                    // If new difference greater than existing difference, replace with new difference
                    if (tmpDiff > difference)
                    {
                        difference = tmpDiff;

                        left = prevMax; // Store element as 'left' variable
                    }

                    i = j;      // Change i to jump to next non-empty bucket
                }
            }

            // Partition current array into left and right to calculate range
            i = -1;
            j = -1;
            foreach (var element in a)
            {
                if (element <= left)
                {
                    i++;
                    leftArray[i] = element;
                }
                else
                {
                    j++;
                    rightArray[j] = element;
                }
            }
            
            // Store the partition index i (range) in k
            k = i;

            // Copy partitioned array values to original array
            int m = 0;
            while(m <= i)
            {
                m++;
                a[m] = leftArray[m];
            }
            m = i;
            i = 0;
            while (i <= j)
            {
                m++;
                a[m] = rightArray[i];
                i++;
            }

            // Stop timer
            partititionTimer.Stop();
            pTimer = partititionTimer.ElapsedTicks;

            diff = difference;
            return k;
        }
    }

    /*  TEST MODULE
     */
    public class TestProgram
    {
        public void Test()
        {
            float difference = 0.0f;
            float[] array = new float[0];

            // Read into array from file
            try
            {
                array = Array.ConvertAll(File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, Program.fileName)),
                    float.Parse);
            }
            catch (FileNotFoundException)
            {
            }

            // Start timer
            Stopwatch sortTimer = new Stopwatch();
            sortTimer.Start();

            int k = 0;

            // Call QuickSort
            QuickSort(array, 0, array.Length-1);

            // Traverse the array and calculate maximum separation per element pair
            for (int i = 1; i < array.Length; i++)
            {
                float tempDiff = array[i] - array[i-1];

                if (tempDiff > difference)
                {
                    difference = tempDiff;
                    k = i-1;
                }
            }

            Console.WriteLine("Using Sorting Method: {0}, Difference: {1}", k, difference);
            
            // Stop timer
            sortTimer.Stop();
            Program.sTimer = sortTimer.ElapsedTicks;
        }

        // QuickSort code starts here
        private void QuickSort(float[] inputArray, int low, int high)
        {
            int pivotPosition;

            if (low < high)
            {
                try
                {
                    pivotPosition = Partition(inputArray, low, high);
                    QuickSort(inputArray, low, pivotPosition - 1);
                    QuickSort(inputArray, pivotPosition + 1, high);
                }catch (IndexOutOfRangeException)
                {
                }
            }
        }

        // QuickSort : Partition module
        private int Partition(float[] inputArray, int low, int high)
        {
            float pivot = inputArray[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (inputArray[j] > pivot) continue;
                i++;
                Swap(inputArray, i, j);
            }

            Swap(inputArray, i + 1, high);
            return i + 1;
        }

        // QuickSort : Swap module
        private static void Swap(float[] inputArray, int elementA, int elementB)
        {
            float temp = inputArray[elementA];
            inputArray[elementA] = inputArray[elementB];
            inputArray[elementB] = temp;
        }
        // QuickSort code ends here
    }
}
