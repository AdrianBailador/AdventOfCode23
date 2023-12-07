using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CamelCards
{
    public enum HandStrength
    {
        FiveOfAKind,
        FourOfAKind,
        FullHouse,
        ThreeOfAKind,
        TwoPair,
        OnePair,
        HighCard
    }

    public class CamelCards
    {
        private Dictionary<string, int> Hands { get; }
        private List<char> Cards { get; }

        public CamelCards(string[] input)
        {
            Hands = Parse(input);
            Cards = new() { 'A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2' };
        }

        public long Play(bool wildcards = false)
        {
            OrderJoker(wildcards);
            return HandsToWinnings(HandsToStrengths(wildcards));
        }

        private static HandStrength HandToStrength(string hand)
        {
            Dictionary<char, int> cards = new();

            foreach (char card in hand)
            {
                if (!cards.ContainsKey(card))
                    cards.Add(card, 1);
                else
                    cards[card]++;
            }

            int max = cards.Max(x => x.Value);

            return max switch
            {
                5 => HandStrength.FiveOfAKind,
                4 => HandStrength.FourOfAKind,
                3 when cards.Count == 2 => HandStrength.FullHouse,
                3 => HandStrength.ThreeOfAKind,
                _ when cards.Count == 3 && max == 2 && cards.Min(x => x.Value) == 1 => HandStrength.TwoPair,
                _ => cards.Count == 5 ? HandStrength.HighCard : HandStrength.OnePair
            };
        }

        private static Dictionary<string, int> Parse(string[] input)
            => input.Select(x => x.Split(" ")).ToDictionary(x => x[0], x => int.Parse(x[1]));

        private Dictionary<string, HandStrength> HandsToStrengths(bool wildcards)
        {
            Dictionary<string, HandStrength> result = new();
            HashSet<HandStrength> hands = new();

            foreach (var (hand, count) in Hands)
            {
                if (wildcards && hand.Contains('J'))
                {
                    hands.Clear();

                    foreach (char card in Cards.Where(x => x != 'J'))
                        hands.Add(HandToStrength(hand.Replace('J', card)));

                    result.Add(hand, hands.Min());
                }
                else
                {
                    result.Add(hand, HandToStrength(hand));
                }
            }

            return result;
        }

        private long HandsToWinnings(Dictionary<string, HandStrength> hands)
        {
            int rank = 1;
            long result = 0;

            foreach (var hand in hands
                .OrderBy(x => x.Value)
                .ThenBy(x => OrderHand(x.Key))
                .Reverse())
            {
                result += Hands[hand.Key] * rank;
                rank++;
            }

            return result;
        }

        private void OrderJoker(bool wildcards)
        {
            if (wildcards)
            {
                Cards.Remove('J');
                Cards.Add('J');
            }
        }

        private string OrderHand(string value)
        {
            return new string(value.Select(chr => (char)('A' + Cards.IndexOf(chr))).ToArray());
        }
    }

    public class Program
    {
        static void Main()
        {
            Day7 day7 = new Day7(@"C:/Users/Adrián Bailador/Desktop/Day7.txt");
            Console.WriteLine($"Part 1: {day7.Silver()}");
            Console.WriteLine($"Part 2: {day7.Gold()}");
        }
    }

    public class Day7
    {
        private CamelCards camelCards;

        public Day7(string file)
        {
            camelCards = new CamelCards(File.ReadAllLines(file));
        }

        public string Silver() => $"{camelCards.Play()}";

        public string Gold() => $"{camelCards.Play(true)}";
    }
}
