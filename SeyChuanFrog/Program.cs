using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeyChuanFrog
{
    class Program
    {
        static void Main(string[] args)
        {
            Solution sol = new Solution();
            int[] arg = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
            Console.WriteLine("answer : " + sol.solution(arg));

            Console.ReadKey();
        }
    }

    #region Solution
    class Solution
    {

        private int longestJump = 0;

        private List<int> F = null;
        private int R = -1; // Min jump
        private List<int> Leaves = null; // Leaves position

        public int solution(int[] A)
        {
            // If no input, mean one jump will reach the bank
            if (A.Length == 0) return 1;

            // Determine longest jump
            longestJump = A.Length + 1;

            // Get the number of leaves
            GetNumberOfLeaves(A);

            // Initialize Fibonacci List
            F = new List<int>();
            GenerateFibonacci(0, 0);

            // Check if direct jump possible
            if (ValidJump(longestJump) == true)
            {
                return 1;
            }

            // Get best jump route
            GetBestRoute(0, 0);

            // Get minimum jump
            return R;
        }

        private void GenerateFibonacci(int prevNum, int currNum)
        {
            int nextNum = prevNum + currNum;
            F.Add(nextNum);

            if (nextNum == 0)
            {
                nextNum++;
            }

            if (nextNum < longestJump)
            {
                GenerateFibonacci(currNum, nextNum);
            }
        }

        private void GetNumberOfLeaves(int[] A)
        {
            Leaves = new List<int>();

            for (int i = 0; i < A.Length - 1; i++)
            {
                if (A[i] == 1)
                {
                    Leaves.Add(i + 1);
                }
            }

            // Add the bank
            Leaves.Add(A.Length + 1);

            //return Leaves.Count;
        }

        private bool ValidJump(int number)
        {
            return F.Contains(number);
        }

        private void GetBestRoute(int jumpCount, int currPosition)
        {
            // Loop all the available leaves
            foreach (int position in Leaves)
            {
                // Skip if the position lesser or equal to current
                if (position <= currPosition) continue;

                // if not in fibonacci list, exit
                if (ValidJump(position - currPosition) == false) continue;

                // Increase jump count
                jumpCount++;

                // if more than previous generated route, exit
                if (R > 0 && jumpCount >= R) return;

                // Reached the bank
                if (position == longestJump)
                {
                    R = jumpCount;
                }
                else
                {
                    // Continue possibility
                    GetBestRoute(jumpCount, position);
                }
            }
        }
    }
    #endregion

    #region Solution75
    class Solution75 //75%
    {
        int[] leavesArray;
        List<int> fibonacci;
        List<int> completedJumpList = new List<int>();
        //to improve performance by skipping subsequent jump larger than this
        int shortestJump;

        public int solution(int[] A)
        {
            // write your code in C# 6.0 with .NET 4.5 (Mono)

            //create Fibonacci list
            leavesArray = A;
            fibonacci = CreateFibonacci(A.Length);

            shortestJump = A.Length + 2;
            RecursiveJump(0, 0, null);

            if (completedJumpList.Count > 0)
                return completedJumpList.Min();
            else
                return -1;


        }

        private void RecursiveJump(int currentLocation, int numOfJump, List<int> newFib)
        {
            if (numOfJump >= shortestJump) return;

            List<int> newList;
            if (newFib != null)
            {
                int stepLeft = leavesArray.Length + 1 - currentLocation;
                newList = newFib.TakeWhile(p => p <= stepLeft).ToList<int>();
            }
            else
            {
                newList = fibonacci;
            }

            for (int fibIndex = (newList.Count - 1); fibIndex >= 0; fibIndex--)
            {
                if (numOfJump + 1 >= shortestJump) break;

                int nextLocation = currentLocation + newList[fibIndex];

                if (nextLocation > leavesArray.Length + 1) //Over jump
                {
                    //Console.WriteLine("Over jump");
                    break;
                }
                else if (nextLocation == leavesArray.Length + 1) //reach destination
                {
                    //Console.WriteLine("reach destination");
                    completedJumpList.Add(numOfJump + 1);
                    if (shortestJump > numOfJump + 1)
                    {
                        shortestJump = numOfJump + 1;
                        break;
                    }
                }
                else if (leavesArray[nextLocation - 1] == 1)//can jump
                {
                    //Console.WriteLine("can jump");
                    currentLocation += newList[fibIndex];
                    numOfJump++;

                    RecursiveJump(currentLocation, numOfJump, newList);
                    numOfJump--;
                    currentLocation -= newList[fibIndex];
                }
                else
                {
                    //Console.WriteLine("cannot jump");
                }
            }
        }

        private List<int> CreateFibonacci(int length)
        {
            List<int> fibonacci = new List<int>();
            fibonacci.Add(1);

            if (length > 0)
                fibonacci.Add(2);

            int i = 2;
            while (i <= length)
            {
                int newFib = fibonacci[i - 1] + fibonacci[i - 2];
                if (newFib > length + 1)
                    break;
                else
                    fibonacci.Add(newFib);

                i++;
            }
            return fibonacci;
        }
    }
    #endregion

    #region SolutionOriginalImproved
    class SolutionOriginalImproved
    {
        int[] leavesArray;
        bool[] accessedLeaves;
        List<int> fibonacci;
        //to improve performance by skipping subsequent jump larger than this
        int shortestJump;

        public int solution(int[] A)
        {
            // write your code in C# 6.0 with .NET 4.5 (Mono)

            //create Fibonacci list
            leavesArray = A;
            accessedLeaves = new bool[A.Length];
            fibonacci = CreateFibonacci(A.Length);

            shortestJump = A.Length + 2;
            RecursiveJump(0, 0);

            if (shortestJump == A.Length + 2)
                return -1;
            else
                return shortestJump;


        }

        private void RecursiveJump(int currentLocation, int numOfJump)
        {
            if (numOfJump >= shortestJump) return;

            if (currentLocation >= leavesArray.Length)  //ady moved more than half way, start from beginning
            {
                for (int fibIndex = 0; fibIndex < fibonacci.Count; fibIndex++)
                {
                    if (numOfJump + 1 >= shortestJump) break;

                    if (JumpLogic(currentLocation, numOfJump, fibIndex))
                        break;
                }
            }
            else //moved less than half way, use big fib num
            {
                for (int fibIndex = (fibonacci.Count - 1); fibIndex >= 0; fibIndex--)
                {
                    if (numOfJump + 1 >= shortestJump) break;

                    JumpLogic(currentLocation, numOfJump, fibIndex);
                }
            }
        }

        private bool JumpLogic(int currentLocation, int numOfJump, int fibIndex)
        {
            bool isOver = false;
            int nextLocation = currentLocation + fibonacci[fibIndex];

            if (nextLocation > leavesArray.Length + 1) //Over jump
            {
                //Console.WriteLine("Over jump");
                isOver = true;
            }
            else if (nextLocation == leavesArray.Length + 1) //reach destination
            {
                //Console.WriteLine("reach destination");
                shortestJump = numOfJump + 1;
                isOver = true;
            }
            else if (leavesArray[nextLocation - 1] == 1)//can jump
            {
                //Console.WriteLine("can jump");
                currentLocation += fibonacci[fibIndex];
                numOfJump++;

                RecursiveJump(currentLocation, numOfJump);
                numOfJump--;
                currentLocation -= fibonacci[fibIndex];
            }
            else
            {
                //Console.WriteLine("cannot jump");
            }

            return isOver;
        }

        private List<int> CreateFibonacci(int length)
        {
            List<int> fibonacci = new List<int>();
            fibonacci.Add(1);
            fibonacci.Add(2);

            int i = 2;
            while (i <= length)
            {
                int newFib = fibonacci[i - 1] + fibonacci[i - 2];
                if (newFib > length + 1)
                    break;
                else
                    fibonacci.Add(newFib);

                i++;
            }
            return fibonacci;
        }
    }
    #endregion
}
