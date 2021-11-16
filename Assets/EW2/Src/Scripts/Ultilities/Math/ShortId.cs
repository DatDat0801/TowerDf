﻿using System;
using System.Text;
using Random = System.Random;
namespace EW2
{
    public class ShortId
    {
         // app variables
        private static Random _random = new Random();
        private const string Bigs = "ABCDEFGHIJKLMNOPQRSTUVWXY";
        private const string Smalls = "abcdefghjlkmnopqrstuvwxyz";
        private const string Numbers = "0123456789";
        private const string Specials = "-_";
        private static string _pool = $"{Smalls}{Bigs}";

        // thread management variables
        private static readonly object ThreadLock = new object();

        /// <summary>
        /// Generates a random string of varying length with special characters and without numbers 
        /// </summary>
        /// <param name="useNumbers">Whether or not to include numbers</param>
        /// <param name="useSpecial">Whether or not special characters are included</param>
        /// <returns>A random string</returns>
        public static string Generate(bool useNumbers = false, bool useSpecial = true)
        {
            var length = _random.Next(7, 15);
            return Generate(useNumbers, useSpecial, length);
        }

        /// <summary>
        /// Generates a random string of a specified length with the option to add numbers and special characters
        /// </summary>
        /// <param name="useNumbers">Whether or not numbers are included in the string</param>
        /// <param name="useSpecial">Whether or not special characters are included</param>
        /// <param name="length">The length of the generated string</param>
        /// <returns>A random string</returns>
        public static string Generate(bool useNumbers, bool useSpecial, int length)
        {
            if (length < 7)
            {
                throw new ArgumentException($"The specified length of {length} is less than the lower limit of 7.");
            }

            string characterPool;
            Random rand;

            lock (ThreadLock)
            {
                characterPool = _pool;
                rand = _random;
            }

            var poolBuilder = new StringBuilder(characterPool);
            if (useNumbers)
            {
                poolBuilder.Append(Numbers);
            }

            if (useSpecial)
            {
                poolBuilder.Append(Specials);
            }

            var pool = poolBuilder.ToString();

            var output = new char[length];
            for (var i = 0; i < length; i++)
            {
                var charIndex = rand.Next(0, pool.Length);
                output[i] = pool[charIndex];
            }

            return new string(output);
        }

        /// <summary>
        /// Generates a random string of a specified length with special characetrs and without numbers
        /// </summary>
        /// <param name="length">The length of the generated string</param>
        /// <returns>A random string</returns>
        public static string Generate(int length)
        {
            return Generate(false, true, length);
        }

        /// <summary>
        /// Changes the character set that id's are generated from
        /// </summary>
        /// <param name="characters">The new character set</param>
        /// <exception cref="InvalidOperationException">Thrown when the new character set is less than 20 characters</exception>
        public static void SetCharacters(string characters)
        {
            if (string.IsNullOrWhiteSpace(characters))
            {
                throw new ArgumentException("The replacement characters must not be null or empty.");
            }

            var stringBuilder = new StringBuilder();
            foreach (var character in characters)
            {
                if (!char.IsWhiteSpace(character))
                {
                    stringBuilder.Append(character);
                }
            }

            if (stringBuilder.Length < 20)
            {
                throw new InvalidOperationException(
                    "The replacement characters must be at least 20 letters in length and without whitespace.");
            }

            _pool = stringBuilder.ToString();
        }

        /// <summary>
        /// Sets the seed that the random generator works with.
        /// </summary>
        /// <param name="seed">The seed for the random number generator</param>
        public static void SetSeed(int seed)
        {
            lock (ThreadLock)
            {
                _random = new Random(seed);
            }
        }

        /// <summary>
        /// Resets the random number generator and character set
        /// </summary>
        public static void Reset()
        {
            lock (ThreadLock)
            {
                _random = new Random();
                _pool = $"{Smalls}{Bigs}";
            }
        }
    }
}