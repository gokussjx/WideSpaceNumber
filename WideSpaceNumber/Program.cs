﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace WideSpaceNumber
{
    public class Program
    {
        private float MAX, MIN;
        private int SIZE;
        private static int n;
        public static int sortKey;
        private static long pTimer;
        public static long sTimer;
        private static float diff;
        public static float diffSort;
        private static List<float>[] bucketArray;
        public static string path = Path.Combine(Environment.CurrentDirectory, "NewTest.txt");

        static void Main(string[] args)
        {
            int testCounter=0;
            bool flag = false;
            while (testCounter != 100000)
            {
                Program program = new Program();
                Random random = new Random();
                int i = 0;

                StreamWriter streamWriter = new StreamWriter(path);
                while (i != 10)
                {
                    streamWriter.WriteLine(random.NextDouble());
                    i++;
                }
                streamWriter.Close();

                // Read into array from file
                float[] inputArray = Array.ConvertAll(File.ReadAllLines(path),
                    float.Parse);

                n = inputArray.Length;

                // Call the Test Method (QuickSort & Difference)
                TestProgram testProgram = new TestProgram();
                int kTest = testProgram.Test(inputArray, n);

                bucketArray = new List<float>[n + 1]; // Array of Lists (Buckets), of size N+1
                // Intitialize List under each array index
                for (i = 0; i < n + 1; i++)
                {
                    bucketArray[i] = new List<float>();
                }

                // Fetch value of k (partition index) by calling the PARTITION method
                int k = program.partition(inputArray, n);

                // Flag is true only if both INDEX and DIFFERENCE values match with TEST method values
                if (kTest == k && diff.Equals(diffSort))
                {
                    flag = true;
                }

                testCounter++;
            }

            Console.WriteLine(flag ? "Test Successful!" : "Test FAILED!");
            
            //Console.WriteLine("\nUsing Partition Method: Index = {0}, Difference = {1}", k, diff);  // Print index k, and max separation difference
            //Console.WriteLine("Using Sorting Method: Index = {0}, Difference = {1}", kTest, diffSort);
        }

        // Core Partition module
        // Returns the index 'k' which acts as the parititioning index
        int partition(float[] a, int n)
        {
            float[] leftArray = new float[n];
            float[] rightArray = leftArray;
            
            int i;
            float difference = 0;
            int bucketLength = bucketArray.Length;

            // Set SIZE
            SIZE = n - 1;

            // Find and store MIN and MAX
            MAX = a[0];
            MIN = a[0];

            foreach (var variable in a)
            {
                if (variable > MAX)
                {
                    MAX = variable;
                }
                if (variable < MIN)
                {
                    MIN = variable;
                }
            }

            // Add MIN and MAX elements to START and END position of the BucketArray
            bucketArray[0].Add(MIN);
            bucketArray[SIZE].Add(MAX);

            // Place all elements (except MIN and MAX) from InputArray into proper position in the BucketArray,
            // based on Index calculated using GetIntervalIndex method
            foreach (var element in a)
            {
                if (!element.Equals(MAX) && !element.Equals(MIN))
                {
                    //bucketArray[GetBucketIndex(element)].Add(element);
                    bucketArray[(int)Math.Floor(((element - MIN) * SIZE) / (MAX - MIN))].Add(element);
                }
            }

            float tempMin;
            float tempMax;

            for (i = 0; i < bucketLength; i++)
            {
                // For buckets which are NOT empty, store local Max and local Min in each bucket, discard all other local elements
                if (bucketArray[i].Count > 0)
                {
                    tempMin = 100000000;
                    tempMax = -100000000;

                    foreach (var variable in bucketArray[i])
                    {
                        if (variable > tempMax)
                        {
                            tempMax = variable;
                        }
                        if (variable < tempMin)
                        {
                            tempMin = variable;
                        }
                    }

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
            for (j = i + 1; j < bucketLength; j++)
            {
                if (bucketArray[j].Count > 0)
                {
                    nextMin = 100000000;
                    prevMax = -100000000;

                    foreach (var variable in bucketArray[j])
                    {
                        if (variable < nextMin)
                        {
                            nextMin = variable;
                        }
                    }

                    foreach (var variable in bucketArray[i])
                    {
                        if (variable > prevMax)
                        {
                            prevMax = variable;
                        }
                    }

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

            diff = difference;
            return k;
        }
    }

    /*  TEST MODULE
     */
    public class TestProgram
    {
        public int Test(float[] array, int n)
        {
            float difference = 0.0f;

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

            Program.diffSort = difference;
            return k;
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
