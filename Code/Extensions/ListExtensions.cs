using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackstone.Code.Extensions
{
    public static class ListExtensions
    {
        private static Random _rand = new Random();

        public static void Shuffle<T>(this List<T> list, int numberOfShuffles = 1) where T : new()
        { 
            for (int i = 0; i < numberOfShuffles; i++) 
            {
                var num = list.Count;

                while (num > 1)
                {
                    num--; // Decrease the loop count so we eventually exit when we go through the collection
                    var randIndex = _rand.Next(num + 1); // Get a random int no larger than the current index
                    T randItem = list[randIndex]; // Get the item at the random location in the list and save it to memory in new variable
                    list[randIndex] = list[num]; // At that random indix in the list, replace with the item at the current index
                    list[num] = randItem; // Set the value at the current index to the random one we saved. Essentially, swapping;
                }
            }
        }
    }
}
